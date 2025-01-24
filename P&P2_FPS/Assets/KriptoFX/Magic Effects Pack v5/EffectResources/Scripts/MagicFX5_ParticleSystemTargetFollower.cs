using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MagicFX5
{
    [ExecuteAlways]
    [RequireComponent(typeof(ParticleSystem))]
    public class MagicFX5_ParticleSystemTargetFollower : MagicFX5_IScriptInstance
    {
        public Transform          Target;
        public MagicFX5_EffectSettings OverrideTargetFromEffectSettings;

        private ParticleSystem _ps;

        internal override void OnEnableExtended()
        {
            _ps = GetComponent<ParticleSystem>();
            UpdateParticlePosition();
#if UNITY_EDITOR
            EditorApplication.update += OnEditorUpdate;
#endif
        }

        internal override void OnDisableExtended()
        {
#if UNITY_EDITOR
            EditorApplication.update -= OnEditorUpdate;
#endif
        }

        private void UpdateParticlePosition()
        {
            if (OverrideTargetFromEffectSettings != null && OverrideTargetFromEffectSettings.Targets.Length > 0) Target = OverrideTargetFromEffectSettings.Targets[0];
            if (Target == null) return;

            var shape = _ps.shape;
            shape.position = transform.InverseTransformPoint(Target.position);
        }

        internal override void ManualUpdate()
        {
#if !UNITY_EDITOR
        UpdateParticlePosition();
#endif
        }

        private void OnEditorUpdate()
        {
            UpdateParticlePosition();
        }
    }
}