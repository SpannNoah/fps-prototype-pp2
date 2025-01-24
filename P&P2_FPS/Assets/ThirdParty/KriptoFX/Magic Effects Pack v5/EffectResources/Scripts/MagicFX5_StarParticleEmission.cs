
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace MagicFX5
{
    public class MagicFX5_StarParticleEmission : MonoBehaviour
    {
        //public float Radius     = 5f;
        public float StartDelay = 0f;
        public float Delay      = 0.25f;
        public float Velocity   = 0.01f;

        ParticleSystem                     _ps;
        private ParticleSystem.ShapeModule _psShape;
        private int                        _currentParticleIndex;

        private Dictionary<int, int> _indexRemap = new Dictionary<int, int>()
        {
            {0, 0},
            {2, 1},
            {4, 2},
            {1, 3},
            {3, 4}
        };

        void Awake()
        {
            _ps      = GetComponent<ParticleSystem>();
            _psShape = _ps.shape;
        }

        void OnEnable()
        {
            StartCoroutine(Emit());
        }

        void OnDisable()
        {
            StopCoroutine(Emit());
        }

        private IEnumerator Emit()
        {
            float angleStep  = 360f                  / _indexRemap.Count;
            angleStep             += transform.rotation.eulerAngles.z / 360f;
            _currentParticleIndex =  0;
            var radius = _psShape.radius;

            if(StartDelay > 0.0001f) yield return new WaitForSeconds(StartDelay);

            while (_currentParticleIndex < _indexRemap.Count)
            {
                float angle = _indexRemap[_currentParticleIndex] * angleStep * Mathf.Deg2Rad;
                var   pos   = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);

                float nextAngle = _indexRemap[(_currentParticleIndex + 1) % _indexRemap.Count] * angleStep * Mathf.Deg2Rad;
                var   nextPos   = new Vector3(Mathf.Cos(nextAngle) * radius, Mathf.Sin(nextAngle) * radius, 0f);

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = pos;
                emitParams.velocity = (nextPos - pos).normalized * Velocity;
                _ps.Emit(emitParams, 1);

                _currentParticleIndex++;

                yield return new WaitForSeconds(Delay);
            }
        }
    }
}