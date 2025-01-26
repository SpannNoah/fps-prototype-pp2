using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace MagicFX5
{
    public class MagicFX5_GUI : MonoBehaviour
    {
        [Space]
        public GameObject  EnemyPrefab;
        public Transform[] EnemyPositions;
        public float       DelayBetweenRespawn = 0.2f;
        
        public Animator    CharacterAnimator;
        public MagicFX5_AnimationEvents CharacterAnimationEvents;

        public Transform RightHandPosition;

        public string[]     EnemyDeadTriggerNames = new[] { "Dead1", "Dead2", "Dead3" };

        public EffectInfo[] Effects;

        private GameObject _currentMainEffectInstance;
        private GameObject _currentBuffEffectInstance;
        private GameObject _currentHandEffectInstance;

        private List<Transform> _targetInstances = new List<Transform>();
        public  int             _currentEffectIndex;


        [Serializable]
        public class EffectInfo
        {
            public float                  EnemyDisintegrationStartDelay = 5;
            
            [Space]
            public GameObject MainEffect;
            public Transform MainEffectTransform;
            [Space]
            public GameObject HandEffect;

            [Space]
            public GameObject CharacterEffect;
            public Transform CharacterEffectTransform;


            [Space]
            public string CharacterAnimatorTriggerName;
        }

      
        void OnEnable()
        {
            //_currentEffectIndex = -1;

            CharacterAnimationEvents.OnTriggerMainEffect += TriggerMainEffect;
            CharacterAnimationEvents.OnTriggerBuffEffect += TriggerBuffEffect;
            CharacterAnimationEvents.OnTriggerHandEffect += TriggerHandEffect;
        }

        void OnDisable()
        {
            CharacterAnimationEvents.OnTriggerMainEffect -= TriggerMainEffect;
            CharacterAnimationEvents.OnTriggerBuffEffect -= TriggerBuffEffect;
            CharacterAnimationEvents.OnTriggerHandEffect -= TriggerHandEffect;
        }

   
        public void NextEffect()
        {
            if (++_currentEffectIndex >= Effects.Length) _currentEffectIndex = 0;
            UpdateEffect();
        }

        public void PreviousEffect()
        {
            if (--_currentEffectIndex < 0) _currentEffectIndex = Effects.Length - 1;
            UpdateEffect();
        }

        void UpdateEffect()
        {
            if (_currentMainEffectInstance) Destroy(_currentMainEffectInstance);
            if (_currentBuffEffectInstance) Destroy(_currentBuffEffectInstance);
            if (_currentHandEffectInstance) Destroy(_currentHandEffectInstance);

            StopAllCoroutines();
            StartCoroutine(RespawnEnemies());
        }

        void TriggerMainEffect()
        {
            var effect = Effects[_currentEffectIndex];
            if (effect.MainEffect == null) return;

            var pos = effect.MainEffectTransform != null ? effect.MainEffectTransform.position : RightHandPosition.position;
            var rotation = effect.MainEffectTransform != null ? effect.MainEffectTransform.rotation : RightHandPosition.rotation;

            _currentMainEffectInstance = Instantiate(effect.MainEffect, pos, rotation);
            UpdateEffectSettings(_currentMainEffectInstance);
        }

        private void UpdateEffectSettings(GameObject effectInstance)
        {
            var currentEffectInfo = Effects[_currentEffectIndex];
            var settings          = effectInstance.GetComponent<MagicFX5_EffectSettings>();
            if (settings != null)
            {
                settings.Targets = _targetInstances.ToArray();
                if (EnemyDeadTriggerNames.Length > 0) settings.AnimatorRandomTriggerNames = EnemyDeadTriggerNames;
                settings.OnEffectCollisionEnter += hit =>
                {
                    var enemyDisintegration = hit.Target.GetComponent<MagicFX5_EnemyDisintegration>();
                    if (enemyDisintegration != null && currentEffectInfo.EnemyDisintegrationStartDelay > 0) enemyDisintegration.Disintegrate(currentEffectInfo.EnemyDisintegrationStartDelay);
                };
            }
        }

   
        void TriggerBuffEffect()
        {
            var effect                                                = Effects[_currentEffectIndex];
            if (effect.CharacterEffect == null) return;

            _currentBuffEffectInstance = Instantiate(effect.CharacterEffect, effect.CharacterEffectTransform.position, effect.CharacterEffectTransform.rotation);

        }

        void TriggerHandEffect()
        {
            var effect = Effects[_currentEffectIndex];
            if (effect.HandEffect == null) return;

            _currentHandEffectInstance = Instantiate(effect.HandEffect, RightHandPosition.position, Quaternion.identity, RightHandPosition.transform);
            UpdateEffectSettings(_currentHandEffectInstance);
        }


        IEnumerator RespawnEnemies()
        {
            foreach (var targetInstance in _targetInstances) { if (targetInstance != null) Destroy(targetInstance.gameObject); }
            _targetInstances.Clear();
            
            for (int i = 0; i < EnemyPositions.Length; i++)
            {
                var instance = Instantiate(EnemyPrefab, EnemyPositions[i].position, EnemyPositions[i].rotation);
                _targetInstances.Add(instance.transform);
                yield return new WaitForSeconds(DelayBetweenRespawn);
            }


            CharacterAnimator.SetTrigger(Effects[_currentEffectIndex].CharacterAnimatorTriggerName);
        }
    }
}