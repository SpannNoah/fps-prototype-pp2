using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_CollisionImpactEffect : MagicFX5_IScriptInstance
    {
        public MagicFX5_EffectSettings EffectSettings;

        public Gradient ColorOverLifetime = new Gradient();
        public MagicFX5_ShaderColorCurve.ME2_ShaderPropertyName ShaderNameColor = MagicFX5_ShaderColorCurve.ME2_ShaderPropertyName._Color;

        //public bool UseFloatCurve = false;
        //public AnimationCurve FloatOverLifetime = AnimationCurve.EaseInOut(0, 0, 1, 1);
        //public MagicFX5_ShaderFloatCurve.ME2_ShaderPropertyName ShaderNameFloat = MagicFX5_ShaderFloatCurve.ME2_ShaderPropertyName._Cutout;

        public float      DestroyTime = 10;
        public Material   ImpactSkinMaterial;
        public GameObject ImpactSkinParticles;
        public GameObject ImpactPrefab;
        public bool       UseImpactZeroRotationInsteadHitNormal;

        [Space] 
        public bool  UseMaterialDirection;
        public         bool  DeactivateMesh;
        public         float DeactivateMeshDelay = 3;

        //[Space]
        //public         bool  ReduceMeshSize;
        //public AnimationCurve MeshSizeCurve    = AnimationCurve.EaseInOut(0, 1, 1, 0);
        //public float          MeshSizeDelay    = 3;
        //public float          MeshSizeDuration = 1;

        private List<MaterialSettings>            _materialInstances = new List<MaterialSettings>();
        private Dictionary<GameObject, Transform> _instances         = new Dictionary<GameObject, Transform>();
        private int                               _shaderColorID;
        //private        int                                                     _shaderFloatID;

        private        Color                                                   _startColor;

        class MaterialSettings
        {
            public Transform      Target;
            public Material       MaterialInstance;
            public float          AnimationLeftTime = 0;
            public bool           IsMeshActive      = true;
            public List<Renderer> Renderers         = new List<Renderer>();

            public Vector3 StartScale;

            public MaterialSettings(Transform target, Material instance, SkinnedMeshRenderer[] skinsRenderers, MeshRenderer[] meshRenderers)
            {
                Target            = target;
                MaterialInstance  = instance;
                AnimationLeftTime = 0;
                StartScale       = Target.localScale;

                if (skinsRenderers != null) Renderers.AddRange(skinsRenderers);
                if (meshRenderers  != null) Renderers.AddRange(meshRenderers);
            }

            public void Restore(bool deactivateMeshFeatureEnabled, bool reduceMeshSizeFeatureEnabled)
            {
                foreach (var rend in Renderers)
                {
                    if (rend == null) continue;

                    rend.RemoveMaterialInstance(MaterialInstance);

                    if (deactivateMeshFeatureEnabled) rend.enabled      = true;
                    if (reduceMeshSizeFeatureEnabled) Target.localScale = StartScale;
                }
            }
        }


        private int _rotationMatrixID = Shader.PropertyToID("_RotationMatrix");
        private int _rotationVectorID = Shader.PropertyToID("_RotationVector");

        internal override void OnEnableExtended()
        {
            
            _materialInstances.Clear();
            _instances.Clear();
            _shaderColorID = Shader.PropertyToID(ShaderNameColor.ToString());
           // _shaderFloatID = Shader.PropertyToID(ShaderNameFloat.ToString());
           

            if (ImpactSkinMaterial != null) _startColor = ImpactSkinMaterial.GetColor(_shaderColorID);
            EffectSettings.OnEffectCollisionEnter += OnCollisionImpactEnter;
            EffectSettings.OnEffectSkinActivated      +=  OnSkinImpactActivated;
            EffectSettings.OnEffectImpactActivated += OnEffectImpactActivated;
            //Invoke("RemoveMaterialFromSkinMeshes", DestroyTime);
        }

        

        internal override void OnDisableExtended()
        {
            EffectSettings.OnEffectCollisionEnter  -= OnCollisionImpactEnter;
            EffectSettings.OnEffectSkinActivated   -= OnSkinImpactActivated;
            EffectSettings.OnEffectImpactActivated -= OnEffectImpactActivated;

            if (_materialInstances.Count > 0) RemoveMaterialFromSkinMeshes();
            //CancelInvoke("RemoveMaterialFromSkinMeshes");
        }


        public void OnCollisionImpactEnter(MagicFX5_EffectSettings.EffectCollisionHit hitInfo)
        {
           
        }

        public void OnSkinImpactActivated(MagicFX5_EffectSettings.EffectCollisionHit hitInfo)
        {
            if (!EffectSettings.UseSkinMeshImpactEffects) return;


            Material matInstance = null;
            
            if (ImpactSkinMaterial != null)
            {
                matInstance = new Material(ImpactSkinMaterial) { hideFlags = HideFlags.HideAndDontSave };
                matInstance.SetVector(_shaderColorID, ColorOverLifetime.Evaluate(0) * _startColor);
            }

            var skins = hitInfo.Target.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skins.Length > 5)
            {
                Debug.LogError("KriptoFX Warning: The skinned mesh (" + hitInfo.Target.name +  ") has multiple skin meshes, so particles/materials will be applied to each of them. " +
                                 "This can significantly impact performance! Combine the skinned mesh to optimize performance.");
            }
            foreach (var skin in skins)
            {
                AddSkinEffect(hitInfo, skin, null, matInstance);
            }

            var meshRenderers = hitInfo.Target.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in meshRenderers)
            {
                AddSkinEffect(hitInfo, null, mr, matInstance);
            }

            if (ImpactSkinMaterial != null)
            {
                var matSettings = new MaterialSettings(hitInfo.Target, matInstance, skins, meshRenderers);
                _materialInstances.Add(matSettings);
            }
        }
        private void OnEffectImpactActivated(MagicFX5_EffectSettings.EffectCollisionHit hitInfo)
        {
            if (ImpactPrefab != null)
            {
                var rotation             = UseImpactZeroRotationInsteadHitNormal ? Quaternion.Euler(0, 0, 0) : Quaternion.LookRotation(-hitInfo.Normal);
                var impactPrefabInstance = Instantiate(ImpactPrefab, hitInfo.Position, rotation);
                _instances.Add(impactPrefabInstance, hitInfo.Target);
            }
        }

        private void AddSkinEffect(MagicFX5_EffectSettings.EffectCollisionHit hitInfo, SkinnedMeshRenderer skin, MeshRenderer meshRenderer, Material matInstance)
        {
            if (ImpactSkinParticles != null)
            {
                var particlesInstance = Instantiate(ImpactSkinParticles, transform.position, transform.rotation, EffectSettings.transform);
                var particleSystems   = particlesInstance.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var ps in particleSystems)
                {
                    var shape = ps.shape;
                    shape.enabled = true;
                    if (skin != null)
                    {
                        shape.shapeType           = ParticleSystemShapeType.SkinnedMeshRenderer;
                        shape.skinnedMeshRenderer = skin;
                    }
                    else if (meshRenderer != null)
                    {
                        shape.shapeType    = ParticleSystemShapeType.MeshRenderer;
                        shape.meshRenderer = meshRenderer;
                    }
                }

                _instances.Add(particlesInstance, hitInfo.Target);
            }

            if (ImpactSkinMaterial != null)
            {
                Renderer rend = null;
                if (skin         != null) rend = skin;
                if (meshRenderer != null) rend = meshRenderer;

                if (rend != null)
                {
                    rend.AddMaterialInstance(matInstance);
                    

                    if (UseMaterialDirection)
                    {
                        SetMaterialDirection(hitInfo.Target, hitInfo.Normal, matInstance);
                    }
                }
            }

        }

        //I can use destroy(instance, time), but anyway I need to remove additional skin material

        void RemoveMaterialFromSkinMeshes()
        {
            foreach (var instance in _instances)
            {
                Destroy(instance.Key);
            }
            _instances.Clear();


            foreach (var instance in _materialInstances)
            {
                instance.Restore(DeactivateMesh, false);
                Destroy(instance.MaterialInstance);
            }

            _materialInstances.Clear();
        }

        void SetMaterialDirection(Transform target, Vector3 normal, Material mat)
        {
            var relativeRotation = Quaternion.Inverse(target.transform.rotation) * Quaternion.FromToRotation(Vector3.right, -normal);
            var rotMatrix = Matrix4x4.Rotate(relativeRotation);

            mat.SetMatrix(_rotationMatrixID, rotMatrix);
            mat.SetVector(_rotationVectorID, -(relativeRotation * Vector3.right));
        }


        internal override void ManualUpdate()
        {
            foreach (var instance in _materialInstances)
            {
                instance.AnimationLeftTime += Time.deltaTime;
                var animationTime          = DeactivateMesh ? DeactivateMeshDelay : DestroyTime;
                var evaluatedTime = instance.AnimationLeftTime / animationTime;

                //if (ReduceMeshSize && instance.IsMeshActive && instance.AnimationLeftTime > MeshSizeDelay)
                //{
                //    var meshSize                           = MeshSizeCurve.Evaluate((instance.AnimationLeftTime - MeshSizeDelay) / MeshSizeDuration);
                //    instance.Target.localScale = meshSize * instance.StartScale;
                //}

                if (DeactivateMesh && instance.IsMeshActive && instance.AnimationLeftTime > DeactivateMeshDelay)
                {
                    instance.IsMeshActive = false;
                    foreach (var rend in instance.Renderers){ rend.enabled        = false; }
                }



                if (evaluatedTime > 1)
                {
                    foreach (var rend in instance.Renderers)
                    {
                        if (rend != null) rend.RemoveMaterialInstance(instance.MaterialInstance);
                    }

                    Destroy(instance.MaterialInstance);
                    continue;
                }

                var shaderColorValue = ColorOverLifetime.Evaluate(evaluatedTime) * _startColor;
                instance.MaterialInstance.SetVector(_shaderColorID, shaderColorValue);

            }


            foreach (var instance in _instances)
            {
                instance.Key.transform.position = instance.Value.position;
            }


        }
    }
}
