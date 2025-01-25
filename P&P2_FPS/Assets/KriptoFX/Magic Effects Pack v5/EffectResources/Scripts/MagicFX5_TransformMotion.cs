using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_TransformMotion : MagicFX5_IScriptInstance
    {
        public MagicFX5_EffectSettings EffectSettings;
        public Transform          Transform;
        public MoveModeEnum       MoveMode = MoveModeEnum.ForwardDirection;
        public Transform          Target;

        public float              Speed                   = 10;
        public float              Gravity                 = 0;
        public float              AccelerationTime        = 1.0f; 
        public float              MaxDistance             = 100;
        public float              TargetDistanceThreshold = -1;
        public float              StartDelay              = 0;

        [Space] public GameObject ImpactGameObjectAtFinish;
        public         GameObject DeactivateGameobjectAfterImpact;


        private bool  _isFinished;
        private float _leftTime;

        private Vector3 _startLocalPos;
        private Vector3 _startWorldPos;
        private float   _currentSpeed;
        private Vector3 _velocity;


        public enum MoveModeEnum
        {
            ForwardDirection,
            TargetDirection,
            EffectSettingsTargetDirection,
            AnimatorRootMotion
        }

        internal override void OnEnableExtended()
        {
            _currentSpeed  = 0f;
            _leftTime      = 0;
            _isFinished    = false;
            _velocity      = Vector3.zero;
            _startLocalPos = Transform.localPosition;
            _startWorldPos = Transform.position;

            if (ImpactGameObjectAtFinish != null) ImpactGameObjectAtFinish.SetActive(false);
            if (DeactivateGameobjectAfterImpact != null) DeactivateGameobjectAfterImpact.SetActive(true);

            if(MoveMode == MoveModeEnum.AnimatorRootMotion) EffectSettings.OnEffectCollisionEnter += OnEffectCollisionEnter;
        }

        private void OnEffectCollisionEnter(MagicFX5_EffectSettings.EffectCollisionHit hit)
        {
            _isFinished = true;
            if (ImpactGameObjectAtFinish        != null) ImpactGameObjectAtFinish.SetActive(true);
            if (DeactivateGameobjectAfterImpact != null) DeactivateGameobjectAfterImpact.SetActive(false);
           
        }

        internal override void OnDisableExtended()
        {
            Transform.localPosition = _startLocalPos;

            if (MoveMode == MoveModeEnum.AnimatorRootMotion) EffectSettings.OnEffectCollisionEnter -= OnEffectCollisionEnter;
        }

        internal override void ManualUpdate()
        {
            if (_isFinished) return;

            var deltaTime = Time.deltaTime;
            _leftTime += deltaTime;
            if (_leftTime < StartDelay) return;

            var speed = EffectSettings.ProjectileSpeed;

            switch (MoveMode)
            {
                case MoveModeEnum.ForwardDirection:
                    _velocity          += Vector3.up * Gravity * deltaTime;
                    _currentSpeed      =  (_leftTime - StartDelay) <= AccelerationTime ? Mathf.Lerp(0f, speed, (_leftTime - StartDelay) / AccelerationTime) : speed;
                    Transform.position += (Transform.forward * _currentSpeed + _velocity) * deltaTime;
                    if (Vector3.Distance(_startWorldPos, Transform.position) >= MaxDistance)
                    {
                        _isFinished = true;
                        TriggerImpact(Target);
                    }
                    break;
                case MoveModeEnum.TargetDirection:
                    if (Target != null)
                    {
                        Transform.position = Vector3.MoveTowards(Transform.position, Target.position, speed * deltaTime);
                        CheckTargetDistance(Target, Target.position);
                    }
                    break;
                case MoveModeEnum.EffectSettingsTargetDirection:
                    if (EffectSettings.Targets.Length > 0)
                    {
                        Target             = EffectSettings.Targets[0];
                        var center = EffectSettings.GetTargetCenter(Target);
                        Transform.position = Vector3.MoveTowards(Transform.position, center, speed * deltaTime);
                        CheckTargetDistance(Target, center);
                    }
                    break;
                case MoveModeEnum.AnimatorRootMotion:
                    {
                        if (Vector3.Distance(_startWorldPos, Transform.position) >= MaxDistance)
                        {
                            _isFinished = true;
                            TriggerImpact(Target);
                        }
                    }
                    break;

            }
        }

        void CheckTargetDistance(Transform target, Vector3 targetPosition)
        {
            if (TargetDistanceThreshold > 0 && VectorMagnitudeXZ(Transform.position - targetPosition) < TargetDistanceThreshold)
            {
                _isFinished = true;
                TriggerImpact(target);
            }
        }

        void TriggerImpact(Transform target)
        {
            if (ImpactGameObjectAtFinish        != null) ImpactGameObjectAtFinish.SetActive(true);
            if (DeactivateGameobjectAfterImpact != null) DeactivateGameobjectAfterImpact.SetActive(false);
            if (target != null)
            {
                var normal = (Transform.position - _startWorldPos).normalized;
                var hit    = new MagicFX5_EffectSettings.EffectCollisionHit() { Target = target, Position = Transform.position, Normal = normal };
                EffectSettings.OnEffectCollisionEnter?.Invoke(hit);
                EffectSettings.OnEffectSkinActivated?.Invoke(hit);
            }
        }

        float VectorMagnitudeXZ(Vector3 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
        }

    }

}
