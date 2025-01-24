using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_CutoutVolume : MonoBehaviour
    {
        public Renderer[] Renderers;
        public Material   OverrideMaterial;
        public MagicFX5_EffectSettings       AffectTarget;

        private int      _shaderID;

        private Vector4 _volumeData;

        public enum ModeEnum
        {
            UpdateMaterial,
            UpdatePropertyBlock
        }
        void Awake()
        {
            _shaderID = Shader.PropertyToID("_VolumeCutoutPos");
        }

        void OnEnable()
        {
            var pos = transform.position;
            var scale    = transform.lossyScale;
            var maxScale = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
            _volumeData = new Vector4(pos.x, pos.y, pos.z, maxScale);

            if (OverrideMaterial != null) OverrideMaterial.SetVector(_shaderID, _volumeData);
            else
            {
                foreach (var rend in Renderers) rend.SetColorPropertyBlock(_shaderID, _volumeData);
            }

            if(AffectTarget != null) AffectTarget.OnEffectSkinActivated += OnEffectSkinActivated;
        }

        void OnDisable()
        {
            if (AffectTarget != null) AffectTarget.OnEffectSkinActivated -= OnEffectSkinActivated;
        }

        private void OnEffectSkinActivated(MagicFX5_EffectSettings.EffectCollisionHit hit)
        {
            var renderers = hit.Target.GetComponentsInChildren<Renderer>();

            foreach (var rend in renderers) rend.SetColorPropertyBlock(_shaderID, _volumeData);
        }

        private void OnDrawGizmosSelected()
        {
            var scale    = transform.lossyScale;
            var maxScale = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
            Gizmos.DrawWireSphere(transform.position, maxScale);
        }
    }
}
