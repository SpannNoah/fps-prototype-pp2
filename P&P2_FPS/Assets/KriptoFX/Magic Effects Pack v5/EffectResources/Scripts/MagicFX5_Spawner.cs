using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MagicFX5
{
    public class MagicFX5_Spawner : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;
        public GameObject         SpawnedObject;
        public SpawnModeEnum      SpawnMode        = SpawnModeEnum.Interval;
        public TargetModeEnum     TargetMode       = TargetModeEnum.None;
        public float              InstanceLifeTime = 8;

        [Space]
        public Vector3            SpawnRadius  = new Vector3(5, 0, 5);
        public bool               OnUnitSphere = true;
        [Range(0, 100)]
        public float              SpawnRandomPercent = 25;
        public float      Duration = 4;
        public float      Interval = 0.5f;
        [Range(0, 100)]
        public float IntervalRandomPercent = 25;


        private float _leftTime;
        private int   _targetIndex;

        private List<GameObject> _instances = new List<GameObject>();

        public enum SpawnModeEnum
        {
            Interval,
            TargetsCount,
        }

        public enum TargetModeEnum
        {
            None,
            NearTarget,
            TargetRoot,
            TransformCenter
        }

        void OnEnable()
        {
            StartCoroutine(CoroutineLoop());
        }

        void OnDisable()
        {
            StopCoroutine(CoroutineLoop());
            foreach (var instance in _instances)
            {
                if(instance != null) Destroy(instance);
            }
        }

        IEnumerator CoroutineLoop()
        {
            yield return null; //1 frame delay for correct targets initialization, because Instantiate -> OnEnable -> SetTargets
            _instances.Clear();
            _leftTime    = 0;
            _targetIndex = 0;

            float percent = Interval * (IntervalRandomPercent / 100.0f);
            while (true)
            {
                if (SpawnMode      == SpawnModeEnum.Interval) { if (_leftTime        >= Duration) break; }
                else if (SpawnMode == SpawnModeEnum.TargetsCount) { if (_targetIndex >= EffectSettings.Targets.Length) break; }
                else break;


                var nextTime = Interval + Random.Range(-percent, percent);
                _leftTime += nextTime;

                var randomPos      = OnUnitSphere ? GetRandomPositionOnCircle() : Vector3.Scale(Random.insideUnitSphere, SpawnRadius);
                var pos            = transform.position + randomPos;

                var instance = Instantiate(SpawnedObject, pos, transform.rotation, transform);
                _instances.Add(instance);
                Destroy(instance, InstanceLifeTime);

                var effectSettings = instance.GetComponent<MagicFX5_EffectSettings>();

                if (TargetMode != TargetModeEnum.None)
                {
                    Transform target = null;
                    switch (TargetMode)
                    {
                        case TargetModeEnum.NearTarget:
                            target = FindNearTarget(EffectSettings.Targets, pos);
                            break;
                        case TargetModeEnum.TargetRoot:
                            target = FindTargetRoot(EffectSettings.Targets[_targetIndex++]);
                            break;
                        case TargetModeEnum.TransformCenter:
                            target = transform;
                            break;
                    }

                    var lookVector = target.position - pos;
                    lookVector.y = pos.y;
                    var rotation = Quaternion.LookRotation(lookVector);
                    instance.transform.SetPositionAndRotation(pos, rotation);
                    effectSettings.Targets = new[] { target };
                }

                if (effectSettings != null) effectSettings.OnEffectCollisionEnter += hit => EffectSettings.OnEffectCollisionEnter?.Invoke(hit);
              

                yield return new WaitForSeconds(nextTime);
            }
        }
        public Vector3 GetRandomPositionOnCircle()
        {
            float angle = Random.Range(0f, Mathf.PI * 2);

            float x            = Mathf.Cos(angle) * SpawnRadius.x;
            float z            = Mathf.Sin(angle) * SpawnRadius.z;
            var   randomOffset = Vector3.Scale(Random.insideUnitSphere, SpawnRadius      * (SpawnRandomPercent / 100.0f));

            return new Vector3(x, 0, z) + randomOffset;
        }

        Transform FindNearTarget(Transform[] targets, Vector3 currentPos)
        {
            float     nearestDistance = float.MaxValue;
            Transform nearTarget = transform;
            foreach (var target in targets)
            {
                var currentDistance = (target.position - currentPos).sqrMagnitude;
                if (currentDistance < nearestDistance)
                {
                    nearestDistance = currentDistance;
                    nearTarget      = target;
                }
            }

            return nearTarget;
        }

        Transform FindTargetRoot(Transform target)
        {
            var skin       = target.GetComponentInChildren<SkinnedMeshRenderer>();
            return skin != null ? skin.rootBone : target;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, SpawnRadius);
            Gizmos.DrawWireSphere(Vector3.zero, 1);
        }
    }
}
