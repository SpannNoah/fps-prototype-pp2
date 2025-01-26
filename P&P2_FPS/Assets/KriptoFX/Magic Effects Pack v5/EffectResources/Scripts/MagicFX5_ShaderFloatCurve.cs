using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_ShaderFloatCurve : MagicFX5_IScriptInstance
    {
        public ME2_ShaderPropertyName ShaderName = ME2_ShaderPropertyName._Cutoff;

        public AnimationCurve FloatOverTime   = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float          Duration        = 2;
        public bool           Loop            = false;

        [Space]
        public bool           OverrideDefault = false;

        public float DefaultValue = 1;

        private Renderer _rend;
        private float    _leftTime;
        private float    _startFloat;
        private bool     _frozen;
        private int      _shaderID;


        public enum ME2_ShaderPropertyName
        {
            _Cutoff,
            _Cutout,
            _CustomTime,
            _CutoutOffset,
            _VertexCutout
        }

        void Awake()
        {
            _rend     = GetComponent<Renderer>();
            _shaderID = Shader.PropertyToID(ShaderName.ToString());
        }

        internal override void OnEnableExtended()
        {
            _leftTime = 0;
            _frozen   = false;

            _startFloat = _rend.sharedMaterial.GetFloat(_shaderID);
            var multiplier = OverrideDefault ? DefaultValue : _startFloat;
            _rend.SetFloatPropertyBlock(_shaderID, multiplier);
        }

        internal override void OnDisableExtended()
        {
            _rend.SetFloatPropertyBlock(_shaderID, _startFloat);
        }

        internal override void ManualUpdate()
        {
            if (_frozen) return;

            _leftTime += Time.deltaTime;
            if (Loop && _leftTime >= Duration) _leftTime = 0;

            var multiplier  = OverrideDefault ? DefaultValue : _startFloat;
            var shaderValue = FloatOverTime.Evaluate(_leftTime / Duration) * multiplier;
           
            _rend.SetFloatPropertyBlock(_shaderID, shaderValue);

            if (!Loop && _leftTime >= Duration) _frozen = true;
        }

    }
}