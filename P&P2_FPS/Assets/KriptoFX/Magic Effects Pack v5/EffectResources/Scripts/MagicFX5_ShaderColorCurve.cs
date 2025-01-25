using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_ShaderColorCurve : MagicFX5_IScriptInstance
    {
        public ME2_ShaderPropertyName ShaderName = ME2_ShaderPropertyName._Color;
        public Material OverrideMaterial;

        public Gradient ColorOverTime = new Gradient();
        public float Duration = 2;
        public bool Loop = false;

        private Renderer _rend;
        private float _startTime;
        private Color _startColor;
        private bool _frozen;
        private int _shaderID;

        public enum ME2_ShaderPropertyName
        {
            _Color,
            _MainColor,
            _EmissionColor1,
            _FresnelEmissionColor
        }

        void Awake()
        {
            _rend = GetComponent<Renderer>();
            _shaderID = Shader.PropertyToID(ShaderName.ToString());
        }

        internal override void OnEnableExtended()
        {
            _startTime = Time.time;
            _frozen    = false;

            if (OverrideMaterial != null)
            {
                _startColor = OverrideMaterial.GetColor(_shaderID);
                OverrideMaterial.SetColor(_shaderID, ColorOverTime.Evaluate(0) * _startColor);
            }
            else
            {
                _startColor = _rend.sharedMaterial.GetColor(_shaderID);
                _rend.SetColorPropertyBlock(_shaderID, ColorOverTime.Evaluate(0) * _startColor);
            }
        }

        internal override void OnDisableExtended()
        {
            if (OverrideMaterial != null)
            {
                OverrideMaterial.SetColor(_shaderID, _startColor);
            }
            else
            {
                _rend.SetColorPropertyBlock(_shaderID, _startColor);
            }
        }

        internal override void ManualUpdate()
        {
            if (_frozen) return;

            var leftTime = Time.time - _startTime;
            if (Loop) leftTime %= Duration;
            var shaderValue = ColorOverTime.Evaluate(leftTime / Duration) * _startColor;
            
            if (OverrideMaterial != null)
            {
                OverrideMaterial.SetVector(_shaderID, shaderValue);
            }
            else
            {
                _rend.SetColorPropertyBlock(_shaderID, shaderValue);
            }

            if (!Loop && leftTime > Duration) _frozen = true;
        }

    }
}