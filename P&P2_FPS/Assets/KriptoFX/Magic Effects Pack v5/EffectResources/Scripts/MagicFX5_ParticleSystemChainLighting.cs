using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MagicFX5
{
    public class MagicFX5_ParticleSystemChainLighting : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;
        public float TargetBehindOffsetMeters = 2;

        public GameObject ChainTrailEffect;
        public float ChainTrailEffectDestroyTime = 5;

        public GameObject StartChainImpactEffect;
        public float StartChainImpactEffectDestroyTime = 5;


        public float        ColliderSize            = 0.25f;
        public float        AnimationTimeToNextJump = 0.17f;
        public MoveModeEnum MoveMode                = MoveModeEnum.Parabolic;

        public enum MoveModeEnum
        {
            Parabolic,
            Straight
        }

        internal bool            IsFinishedJump;
        int                      _currentTargetIndex;
        private       GameObject _currentChainInstance;
        private       int        _currentTargetImpactEffectsPerChain = 0;
        private const int        MaxTargetImpactEffectsPerChain      = 1;
        private       Vector3    _startJumpPosition;
        private       float      _leftTime;

        void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            _currentTargetIndex     = 0;
            _leftTime               = 0;
            IsFinishedJump          = true;

            StartCoroutine(MoveToNextTarget());
        }

        void OnDisable()
        {
            StopCoroutine(MoveToNextTarget());
        }


        IEnumerator MoveToNextTarget()
        {
            yield return null; //1 frame delay for correct targets initialization, because Instantiate -> OnEnable -> SetTargets

            var targets = EffectSettings.Targets;

            if (EffectSettings.Targets.Length == 0)
            {
                Debug.LogError("You must set targets in the EffectSettings script!");
                yield break;
            }

            while (_currentTargetIndex < targets.Length)
            {
                if (IsFinishedJump)
                {
                    IsFinishedJump = false;
                    _leftTime = 0;

                    var target = targets[_currentTargetIndex];
                    if (target != null)
                    {
                        Vector3 footPos      = Vector3.zero;
                        var     targetCenter = EffectSettings.GetTargetCenter(target);

                        switch (MoveMode)
                        {
                            case MoveModeEnum.Parabolic:
                                footPos = target.position; //skin meshes always has foot pivot
                                break;
                            case MoveModeEnum.Straight:
                                footPos = targetCenter;
                                break;

                        }
                        
                        var direction = (footPos - transform.position).normalized;

                        var endPoint    = footPos + direction * TargetBehindOffsetMeters;
                        var apex        = targetCenter;
                        var triggerTime = CalculateTriggerTime(apex, endPoint);

                        if (ChainTrailEffect != null)
                        {
                            _currentChainInstance = Instantiate(ChainTrailEffect, transform.position, transform.rotation, EffectSettings.transform);
                            Destroy(_currentChainInstance, ChainTrailEffectDestroyTime);
                        }
                        if (StartChainImpactEffect != null) Destroy(Instantiate(StartChainImpactEffect, transform.position, transform.rotation, EffectSettings.transform), StartChainImpactEffectDestroyTime);

                        _currentTargetImpactEffectsPerChain = 0;
                        _startJumpPosition                  = transform.position;
                      

                        while (!IsFinishedJump)
                        {
                            _leftTime += Time.deltaTime;
                            var normalizedDistance = AnimationTimeToNextJump > 0.00001f ? _leftTime / AnimationTimeToNextJump : 1;

                            switch (MoveMode)
                            {
                                case MoveModeEnum.Parabolic:
                                    transform.position = ParabolicLerp(_startJumpPosition, apex, endPoint, normalizedDistance);
                                    break;
                                case MoveModeEnum.Straight:
                                    transform.position = Vector3.Lerp(_startJumpPosition, footPos, normalizedDistance);
                                    break;

                            }

                            if (_currentChainInstance != null) _currentChainInstance.transform.position = transform.position;

                            if (_currentTargetImpactEffectsPerChain < MaxTargetImpactEffectsPerChain && normalizedDistance >= triggerTime)
                            {
                                _currentTargetImpactEffectsPerChain++;
                                var normal = (_startJumpPosition - transform.position).normalized;
                                var hit    = new MagicFX5_EffectSettings.EffectCollisionHit() { Normal = normal, Position = apex, Target = EffectSettings.Targets[_currentTargetIndex] };
                                EffectSettings.OnEffectCollisionEnter?.Invoke(hit);
                                EffectSettings.OnEffectSkinActivated?.Invoke(hit);
                                EffectSettings.OnEffectImpactActivated?.Invoke(hit);
                            }

                            if (_leftTime >= AnimationTimeToNextJump)
                            {
                                IsFinishedJump = true;
                                _currentTargetIndex++;
                            }

                            yield return null;
                        }

                    }
                }
                
            }
        }

        private float CalculateTriggerTime(Vector3 apex, Vector3 endPoint)
        {
            var distanceToApex = Vector3.Distance(transform.position, apex);
            var fullDistance = Vector3.Distance(transform.position, endPoint);
            var triggerTime = distanceToApex / fullDistance;
            return triggerTime;
        }


        Vector3 ParabolicLerp(Vector3 start, Vector3 apex, Vector3 end, float t)
        {
            var x0 = start.x;
            var y0 = start.y;
            var x1 = apex.x;
            var y1 = apex.y;
            var x2 = end.x;
            var y2 = end.y;

            var a = ((y2 - y0) - (y1 - y0) * (x2 - x0) / (x1 - x0)) / ((x2 * x2 - x0 * x0) - (x1 * x1 - x0 * x0) * (x2 - x0) / (x1 - x0));
            var b = (y1 - y0 - a * (x1 * x1 - x0 * x0)) / (x1 - x0);
            var c = y0 - a * x0 * x0 - b * x0;

            var currentX = Mathf.Lerp(start.x, end.x, t);
            var currentY = a * currentX * currentX + b * currentX + c;
            var currentZ = Mathf.Lerp(start.z, end.z, t);
            if (float.IsNaN(currentY)) currentY = y0;

            return new Vector3(currentX, currentY, currentZ);
        }

      
    }

}
