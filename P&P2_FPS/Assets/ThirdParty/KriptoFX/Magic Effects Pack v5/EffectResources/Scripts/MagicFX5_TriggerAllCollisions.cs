using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_TriggerAllCollisions : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;

        public TriggerTypeEnum TriggerType            = TriggerTypeEnum.All;
        public float           Delay                  = 1;
        public float           DelayBetweenCollisions = 0;

        private WaitForSeconds _waitTime;
        public enum TriggerTypeEnum
        {
            All,
            CollisionEnter,
            SkinEnter,
            ImpactEnter,
        }

        void OnEnable()
        {
            _waitTime = new WaitForSeconds(DelayBetweenCollisions);
            StartCoroutine(LateInitialize());
        }

        private void OnDisable()
        {
            StopCoroutine(LateInitialize());
        }

        IEnumerator LateInitialize()
        {
            yield return null; //1 frame delay for correct targets initialization, because Instantiate -> OnEnable -> SetTargets
            yield return new WaitForSeconds(Delay);

            foreach (var target in EffectSettings.Targets)
            {
                var pos    = target.position;
                var normal = (pos - transform.position).normalized;
                var hit    = new MagicFX5_EffectSettings.EffectCollisionHit() { Target = target, Position = pos, Normal = normal };

                switch (TriggerType)
                {
                    case TriggerTypeEnum.All:
                        EffectSettings.OnEffectCollisionEnter?.Invoke(hit);
                        EffectSettings.OnEffectSkinActivated?.Invoke(hit);
                        EffectSettings.OnEffectImpactActivated?.Invoke(hit);
                        break;
                    case TriggerTypeEnum.CollisionEnter:
                        EffectSettings.OnEffectCollisionEnter?.Invoke(hit);
                        break;
                    case TriggerTypeEnum.SkinEnter:
                        EffectSettings.OnEffectSkinActivated?.Invoke(hit);
                        break;
                    case TriggerTypeEnum.ImpactEnter:
                        EffectSettings.OnEffectImpactActivated?.Invoke(hit);
                        break;
                   
                }
               
                yield return _waitTime;
            }
        }
    }
}
