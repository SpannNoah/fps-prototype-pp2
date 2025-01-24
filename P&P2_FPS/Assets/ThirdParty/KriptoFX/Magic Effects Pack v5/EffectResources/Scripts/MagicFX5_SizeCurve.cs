using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_SizeCurve : MagicFX5_IScriptInstance
    {

        public AnimationCurve SizeOverLifeTime      = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public Vector2        RandomRangeMultiplier = new Vector2(0.75f, 1.25f);
        public Vector3        AffectedAxis                  = new Vector3(1, 1, 1);
        public float          Duration              = 1;
        public bool           Loop                  = false;

        private float     _startTime;
        private Vector3   _startSize;
        private bool      _frozen;
        private float   _randomScaleMultiplier;


        internal override void OnEnableExtended()
        {
            _startTime             = Time.time;
            _frozen                = false;
            _randomScaleMultiplier = Random.Range(RandomRangeMultiplier.x, RandomRangeMultiplier.y);
            _startSize             = transform.localScale * _randomScaleMultiplier;
            var sizeValue = SizeOverLifeTime.Evaluate(0) * _startSize;
            sizeValue            = LerpAxis(_startSize, sizeValue, AffectedAxis);
            transform.localScale = sizeValue;
        }

        internal override void OnDisableExtended()
        {
            transform.localScale = _startSize;
        }

        internal override void ManualUpdate()
        {
            if (_frozen) return;

            var leftTime       = Time.time - _startTime;
            if (Loop) leftTime %= Duration;
            var sizeValue      = SizeOverLifeTime.Evaluate(leftTime / Duration) * _startSize;
            sizeValue            = LerpAxis(_startSize, sizeValue, AffectedAxis);
            transform.localScale = sizeValue;

            if (!Loop && leftTime > Duration) _frozen = true;
        }

        Vector3 LerpAxis(Vector3 a, Vector3 b, Vector3 axis)
        {
            a.x = Mathf.Lerp(a.x, b.x, axis.x);
            a.y = Mathf.Lerp(a.y, b.y, axis.y);
            a.z = Mathf.Lerp(a.z, b.z, axis.z);
            return a;
        }
    }
}