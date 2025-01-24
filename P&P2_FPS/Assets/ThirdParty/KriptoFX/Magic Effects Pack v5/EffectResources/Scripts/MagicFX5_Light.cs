using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_Light : MagicFX5_IScriptInstance
    {
        public AnimationCurve IntensityOverTime = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float          Duration          = 2;
        public bool           Loop              = false;
        public bool           AutoDeactivation  = true;
       
        [Space] 
        public bool     UseColor      = false;
        public         Gradient ColorOverTime = new Gradient();

        [Space] public bool UseSixPointLighting = false;

        private Light _light;
        private float _startTime;
        private Color _startColor;
        private float _startIntensity;
        private bool  _frozen;


        void Awake()
        {
            _light = GetComponent<Light>();
        }

        internal override void OnEnableExtended()
        {
            if(UseSixPointLighting) MagicFX5_GlobalUpdate.SixPointsLightInstances.Add(_light);
            _startTime       = Time.time;
            _startIntensity  = _light.intensity;
            _startColor      = _light.color;
            _frozen          = false;
            _light.enabled   = true;
            _light.intensity = IntensityOverTime.Evaluate(0);
        }

        internal override void OnDisableExtended()
        {
            if (UseSixPointLighting) MagicFX5_GlobalUpdate.SixPointsLightInstances.Remove(_light);
            _light.intensity = _startIntensity;
        }

        internal override void ManualUpdate()
        {
            if (_frozen) return;

            var leftTime       = Time.time - _startTime;
            if (Loop) leftTime %= Duration;
            _light.intensity = IntensityOverTime.Evaluate(leftTime / Duration) * _startIntensity;
            if(UseColor) _light.color = ColorOverTime.Evaluate(leftTime / Duration) * _startColor;

            if (!Loop && leftTime > Duration)
            {
                _frozen        = true;
                if (AutoDeactivation)
                {
                    _light.enabled = false;
                    if (UseSixPointLighting) MagicFX5_GlobalUpdate.SixPointsLightInstances.Remove(_light);
                }
               
            }
        }

    }
}