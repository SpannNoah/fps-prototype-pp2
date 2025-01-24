using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MagicFX5
{
    public class MagicFX5_ChainEffect : MagicFX5_IScriptInstance
    {
        public MagicFX5_EffectSettings EffectSettings;
        public TargetModeEnum     TargetMode                         = TargetModeEnum.UseRoot;
        public bool               OverrideStartPositionToChainCenter = false;
        public GameObject         ChainPrefab;
        public float              DestroyTime = 10;

        [Space]
        public Gradient LineRendererColorOverTime;
        public float Duration = 6;

        [Space] public ParticleSystem ParticlesAtStartRopePositions;
        public         bool           UpdateFollowTarget = false;

        private float _leftTime;
        private bool _isFinished;
        private LineRenderer[] _lineRenderers;
        private List<MagicFX5_RopePhysics> _ropes = new List<MagicFX5_RopePhysics>();
        private List<GameObject> _instances = new List<GameObject>();

        public enum TargetModeEnum
        {
            UseRoot,
            UseRandomBone
        }


        internal override void OnEnableExtended()
        {
            _isFinished = false;
            _leftTime = 0;
            _instances.Clear();
            _ropes.Clear();
            StartCoroutine(Initialize());
        }

        internal override void OnDisableExtended()
        {
            StopCoroutine(Initialize());
            foreach (var instance in _instances) { Destroy(instance); }
        }

        internal override void ManualUpdate()
        {
            if (_isFinished || _lineRenderers == null || _lineRenderers.Length == 0) return;

            _leftTime += Time.deltaTime;
            var shaderValue = LineRendererColorOverTime.Evaluate(_leftTime / Duration);
            
            foreach (var lineRenderer in _lineRenderers)
            {
                if (lineRenderer != null) lineRenderer.startColor = lineRenderer.endColor = shaderValue;
            }

            if (_leftTime > Duration) _isFinished = true;
        }


        IEnumerator Initialize()
        {
            yield return null; //1 frame delay for correct targets initialization, because Instantiate -> OnEnable -> SetTargets

            
            foreach (var target in EffectSettings.Targets)
            {
                var pos      = OverrideStartPositionToChainCenter ? transform.position : target.position;
                var instance = Instantiate(ChainPrefab, pos, Quaternion.identity, transform);
                _instances.Add(instance);
                var ropes = instance.GetComponentsInChildren<MagicFX5_RopePhysics>(true);
                _ropes.AddRange(ropes);

                if (TargetMode == TargetModeEnum.UseRoot)
                {
                    var skin = target.GetComponentInChildren<SkinnedMeshRenderer>();
                    var boneTarget = skin != null ? skin.rootBone : target;
                    foreach (var rope in ropes) rope.Target = boneTarget;

                    if (UpdateFollowTarget) UpdateFollowTargetScripts(instance, boneTarget);
                }

                if (TargetMode == TargetModeEnum.UseRandomBone)
                {
                    var rigids                              = target.GetComponentsInChildren<Rigidbody>();
                    foreach (var rope in ropes) rope.Target = (rigids == null || rigids.Length == 0) ? target : rigids[Random.Range(0, rigids.Length - 1)].transform;

                    if (UpdateFollowTarget) UpdateFollowTargetScripts(instance, rigids == null ? target : rigids[Random.Range(0, rigids.Length - 1)].transform);
                }

                if (ParticlesAtStartRopePositions != null) InstantiateParticlesAtRopePosition(ropes);

                Destroy(instance, DestroyTime);
            }

            _lineRenderers = GetComponentsInChildren<LineRenderer>(true);

            var shaderValue = LineRendererColorOverTime.Evaluate(0);
            foreach (var lineRenderer in _lineRenderers) { lineRenderer.startColor = lineRenderer.endColor = shaderValue; }
        }


        private void InstantiateParticlesAtRopePosition(MagicFX5_RopePhysics[] ropes)
        {
            foreach (var rope in ropes)
            {
                var emit = new ParticleSystem.EmitParams();
                emit.position = rope.transform.position;
                emit.velocity = (rope.Target.position - rope.transform.position) * 0.00001f;
                ParticlesAtStartRopePositions.Emit(emit, 1);
            }
        }

        private void UpdateFollowTargetScripts(GameObject instance, Transform currentTarget)
        {
            var followTarget                              = instance.GetComponentInChildren<MagicFX5_FollowTarget>(true);
            if (followTarget != null) followTarget.Target = currentTarget;
        }
    }
}
