using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{

    public class MagicFX5_FollowTarget : MagicFX5_IScriptInstance
    {
        public Transform Target;
        public bool      UsePosition = true;
        public bool      UseRotation = true;
        public bool      FreezeYPos  = false;

        public bool LookAtTarget          = false;
        public bool LookAtMotionVector = false;

        private Vector3 _startPos;
        private Vector3 _lastTarget;

        internal override void OnEnableExtended()
        {
            _startPos   = transform.position;
            _lastTarget = _startPos;
            ManualUpdate();
        }

        internal override void OnDisableExtended()
        {
            transform.localPosition = Vector3.zero;
        }

        internal override void ManualUpdate()
        {
            if (UseRotation) transform.rotation = Target.rotation;

            if (LookAtTarget)
            {
                if ((Target.position - _startPos).sqrMagnitude > 0.001f)
                {
                    var dir = (Target.position - _startPos).normalized;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }

            if (LookAtMotionVector)
            {
                if ((Target.position - _lastTarget).sqrMagnitude > 0.001f)
                {
                    var dir = (Target.position - _lastTarget).normalized;
                    transform.rotation = Quaternion.LookRotation(dir);
                }

                _lastTarget = Target.position;
            }

            if (UsePosition)
            {
                var pos               = Target.position;
                if (FreezeYPos) pos.y = _startPos.y;
                transform.position = pos;
            }
           
           
        }
    }
}