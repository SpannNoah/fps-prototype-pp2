using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_PositionCurve : MagicFX5_IScriptInstance
    {

        public AnimationCurve PositionOverLifeTime = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public Vector3        Axis                 = new Vector3(0, 1, 0);
        public float          Duration             = 1;
        public bool           Loop                 = false;
        
        private float     _startTime;
        private Vector3   _startPosition;
        private bool      _frozen;


        internal override void OnEnableExtended()
        {
            MagicFX5_GlobalUpdate.CreateInstanceIfRequired();
            MagicFX5_GlobalUpdate.ScriptInstances.Add(this);
            _startTime = Time.time;
            _frozen    = false;

            _startPosition     = transform.position;
            transform.position = PositionOverLifeTime.Evaluate(0) * Axis + _startPosition;
        }


        internal override void OnDisableExtended()
        {
            MagicFX5_GlobalUpdate.ScriptInstances.Remove(this);
            transform.position = _startPosition;
        }

        internal override void ManualUpdate()
        {
            if (_frozen) return;
            
            var leftTime       = Time.time - _startTime;
            if (Loop) leftTime %= Duration;
            var sizeValue      = PositionOverLifeTime.Evaluate(leftTime / Duration) * Axis + _startPosition;
            transform.position = sizeValue;

            if (!Loop && leftTime > Duration) _frozen = true;
        }

    }
}