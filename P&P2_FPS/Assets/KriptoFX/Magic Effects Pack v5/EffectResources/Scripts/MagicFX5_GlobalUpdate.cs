using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;


namespace MagicFX5
{
    public partial class MagicFX5_GlobalUpdate : MonoBehaviour
    {
        public static GameObject Instance;
        public static List<MagicFX5_IScriptInstance> ScriptInstances = new List<MagicFX5_IScriptInstance>();
        public static HashSet<MagicFX5_CommandBufferDistortion> DistortionInstances = new HashSet<MagicFX5_CommandBufferDistortion>();
        public static HashSet<Light> SixPointsLightInstances = new HashSet<Light>();
        public static HashSet<MagicFX5_SixWayLighting> SixWayLightingInstances = new HashSet<MagicFX5_SixWayLighting>();

        NativeArray<LightData> _lightsData;
        ComputeBuffer _lightsDataBuffer;
        int _screenCopyID = Shader.PropertyToID("_CameraOpaqueTextureRT");
        private int _globalBuiltintOpaqueTextureID = Shader.PropertyToID("_CameraOpaqueTexture");
        public struct LightData
        {
            public Vector4 color;

            public Vector3 position;
            public float range;
        }



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RunOnStart()
        {
            Destroy(Instance);
            Instance = null;

            ScriptInstances.Clear();
            DistortionInstances.Clear();
        }

        public static void CreateInstanceIfRequired()
        {
            if (Instance != null) return;

            Instance = new GameObject("MagicFX5_GlobalUpdate") { hideFlags = HideFlags.HideAndDontSave };
            Instance.AddComponent<MagicFX5_GlobalUpdate>();
        }


        void Update()
        {
            UnityEngine.Profiling.Profiler.BeginSample("KriptoFX.UpdateAllEffectScripts");
            for (var i = 0; i < ScriptInstances.Count; i++)
            {
                ScriptInstances[i].ManualUpdate();
            }

            UnityEngine.Profiling.Profiler.EndSample();


            UnityEngine.Profiling.Profiler.BeginSample("KriptoFX.CollectEffectLightsToBuffer");
            if (SixWayLightingInstances.Count > 0) CollectLightsBuffer();
            UnityEngine.Profiling.Profiler.EndSample();
        }

        float ConvertIntensity(float intensity)
        {
#if USING_HDRP
            if (intensity <= 0.00001) return 0;
            float ev100 = (float)(Mathf.Log(intensity / 4, 2) + 5);
            return ev100;
#elif USING_URP
            if (intensity <= 0.00001) return 0;
            float newIntensity = Mathf.Sqrt(intensity);
            return newIntensity;
#else
            return intensity;
#endif
        }

        //collect all effect lights to compute buffer for shadergraph
        void CollectLightsBuffer()
        {
            var sun = RenderSettings.sun;
            if (sun != null)
            {
                Shader.SetGlobalVector("MagicFX5_DirLightColor", sun.color.linear * ConvertIntensity(sun.intensity));
                Shader.SetGlobalVector("MagicFX5_DirLightVector", sun.transform.forward);
            }

            Shader.SetGlobalInteger("MagicFX5_AdditionalLightsCount", SixPointsLightInstances.Count);

            if (SixPointsLightInstances.Count == 0)
            {
                return;
            }

            if (_lightsDataBuffer == null)
            {
                _lightsDataBuffer = new ComputeBuffer(SixPointsLightInstances.Count, System.Runtime.InteropServices.Marshal.SizeOf<LightData>(), ComputeBufferType.Default, ComputeBufferMode.Immutable);
            }
            else if (SixPointsLightInstances.Count > _lightsDataBuffer.count)
            {
                _lightsDataBuffer.Dispose();
                _lightsDataBuffer = new ComputeBuffer(SixPointsLightInstances.Count, System.Runtime.InteropServices.Marshal.SizeOf<LightData>(), ComputeBufferType.Default, ComputeBufferMode.Immutable);
            }

            _lightsData = new NativeArray<LightData>(SixPointsLightInstances.Count, Allocator.Temp);
            var idx = 0;
            foreach (var currentLight in SixPointsLightInstances)
            {
                var data = _lightsData[idx];
                data.color = currentLight.color.linear * ConvertIntensity(currentLight.intensity);
                data.position = currentLight.transform.position;
                data.range = currentLight.range;
                //Debug.Log(currentLight + " " + data.color);

                _lightsData[idx] = data;
                idx++;
            }
            _lightsDataBuffer.SetData(_lightsData);

            Shader.SetGlobalBuffer("MagicFX5_AdditionalLightsBuffer", _lightsDataBuffer);
        }




        //builtin distortion rendering command buffer

        private CommandBuffer _cmd;
        private CameraEvent _cameraEvent = CameraEvent.BeforeForwardAlpha;


        void OnEnable()
        {
            if (GraphicsSettings.currentRenderPipeline == null)
            {
                Camera.onPreCull += OnBeforeCameraRendering;
                Camera.onPostRender += OnAfterCameraRendering;
            }
            else
            {
                RenderPipelineManager.beginCameraRendering += OnBeforeCameraRendering;
                RenderPipelineManager.endCameraRendering += OnAfterCameraRendering;

            }
        }

        void OnDisable()
        {
            if (GraphicsSettings.currentRenderPipeline == null)
            {
                Camera.onPreCull -= OnBeforeCameraRendering;
                Camera.onPostRender -= OnAfterCameraRendering;
            }
            else
            {
                RenderPipelineManager.beginCameraRendering -= OnBeforeCameraRendering;
                RenderPipelineManager.endCameraRendering -= OnAfterCameraRendering;
            }
        }

        private void OnBeforeCameraRendering(Camera cam)
        {
            RenderDistortion(cam);
            UpdateCameraParams(cam);
        }


        private void OnAfterCameraRendering(Camera cam)
        {
            ClearDistortion(cam);
        }


        private void OnBeforeCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            RenderDistortion(cam, context);
            UpdateCameraParams(cam, context);
        }

        private void OnAfterCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            ClearDistortion(cam);
        }


        void RenderDistortion(Camera cam, ScriptableRenderContext context = default)
        {
            if (DistortionInstances.Count == 0) return;

#if USING_URP
            var data = cam.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            if (data != null)
            {
                data.requiresDepthOption = UnityEngine.Rendering.Universal.CameraOverrideOption.On;
                data.requiresColorOption = UnityEngine.Rendering.Universal.CameraOverrideOption.On;

                return;
            }
#elif USING_HDRP

#else

            if (_cmd == null)
            {
                _cmd = new CommandBuffer() { name = "MagicFX5_CameraDistortionRendering" };
            }
            _cmd.Clear();


            _cmd.GetTemporaryRT(_screenCopyID, Screen.width, Screen.height, 0, FilterMode.Bilinear, MagicFX5_CoreUtils.GetGraphicsFormatHDR());
            _cmd.Blit(BuiltinRenderTextureType.CurrentActive, _screenCopyID);
            _cmd.SetGlobalTexture(_globalBuiltintOpaqueTextureID, _screenCopyID);
            cam.AddCommandBuffer(_cameraEvent, _cmd);
#endif


        }

        void ClearDistortion(Camera cam)
        {
#if !USING_URP && !USING_HDRP
            if (DistortionInstances.Count == 0) return;

            if (_cmd != null)
            {
                cam.RemoveCommandBuffer(_cameraEvent, _cmd);
            }
#endif

        }

        void UpdateCameraParams(Camera cam, ScriptableRenderContext context = default)
        {
#if !USING_URP && !USING_HDRP
            cam.depthTextureMode |= DepthTextureMode.Depth;
#endif
        }
    }
}