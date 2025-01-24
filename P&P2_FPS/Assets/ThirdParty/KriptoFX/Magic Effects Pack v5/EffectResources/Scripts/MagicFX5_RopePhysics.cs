using System.Collections;
using UnityEngine;

namespace MagicFX5
{
    [RequireComponent(typeof(LineRenderer))]
    public class MagicFX5_RopePhysics : MonoBehaviour
    {
        public Transform Target;
        public int SegmentCount = 10;
     
        public float Gravity                   = -1.0f;
        public float Damping                   = 0.95f;
        public int   ConstraintIterations      = 1;
        public float MoveToTargetAnimationTime = 0.5f;

        [Space]
        public bool  UseTargetForce = true;
        public float Force    = 1;
        //public float ForceDistanceToTargetThreshold= 0.5f;
        public float ForceStartDelay               = 0;
        public float ForceLifeTime                 = -1;
        public float TargetDistanceOffset          = 0.25f;
        public float TargetForceDistanceMultiplier = 1f;

        [Space]
        public bool  UseTargetWindMode   = false;
        public float WindToTargetStrength  = 1;
       
        [Space]
        public bool  UseTurbulence       = false;
        public float TurbulenceFrequency = 1.5f;
        public float TurbulenceAmplitude = 0.1f;
        public float TurbulenceTimeScale = 1.0f;

        float        _segmentLength;
       // private float _forceSegmentLength;

        internal LineRenderer LineRenderer;
        private Vector3[]    _segmentPositions;
        private Vector3[]    _lineRendererPositions;
        private Vector3[]    _previousPositions;

        internal Rigidbody TargetRigidbody;

        private float _animLeftTime;
        private float _animLerpVal;
        private float _forceLeftTime;

        private bool  _isInitialized       = false;


      
        void OnEnable()
        {
            _isInitialized = false;
            _animLeftTime  = 0;
            _forceLeftTime = 0;
        }

        public void ForceInitialize()
        {
            if (!_isInitialized) Initialize();
        }

        void Initialize()
        {

            TargetRigidbody            = Target.GetComponent<Rigidbody>();
            LineRenderer               = GetComponent<LineRenderer>();
            LineRenderer.positionCount = 0;

            _segmentPositions      = new Vector3[SegmentCount];
            _previousPositions     = new Vector3[SegmentCount];
            _lineRendererPositions = new Vector3[SegmentCount];

            Vector3 initialPosition = transform.position;
            Vector3 direction       = (Target.position - transform.position).normalized;
            _segmentLength      = ((Target.position - transform.position).magnitude * TargetForceDistanceMultiplier + TargetDistanceOffset) / SegmentCount;
           // _forceSegmentLength = ((Target.position - transform.position).magnitude * TargetForceDistanceMultiplier)                        / SegmentCount;

            for (int i = 0; i < SegmentCount; i++)
            {
                _segmentPositions[i]  = initialPosition + direction * _segmentLength * i;
                _previousPositions[i] = _segmentPositions[i];
            }
            DrawRope();

            _isInitialized = true;
        }

        void Update()
        {
            if (Target == null) return;

            if (!_isInitialized) Initialize();
            float deltaTime = Time.deltaTime;
            _forceLeftTime += deltaTime;

            Simulate(deltaTime);
            DrawRope();
        }

        void FixedUpdate()
        {
            if (Target == null || !UseTargetForce || TargetRigidbody == null) return;
            
            var force                                                    = GetTensionForce();
            if (force > 0.0f) TargetRigidbody.velocity = ((GetForceEndPoint() - Target.position)) * (1 / Time.fixedDeltaTime) * Mathf.Clamp01(force);
        }

        void Simulate(float deltaTime)
        {
            Vector3 gravityVector = new Vector3(0, Gravity, 0) * deltaTime;

            if (UseTargetWindMode)
            {
                var dir = (Target.position - transform.position).normalized;
                gravityVector += dir * WindToTargetStrength * deltaTime;
            }

            for (int i = 1; i < SegmentCount; i++)
            {
                Vector3 currentPosition        = _segmentPositions[i];
                Vector3 velocity               = (_segmentPositions[i] - _previousPositions[i]) * Damping;
                Vector3 newPosition            = _segmentPositions[i] + velocity + gravityVector;
                //if (UseTurbulence) newPosition += GenerateTurbulentNoise(currentPosition, Time.time * TurbulenceTimeScale) * deltaTime;
                if (UseTurbulence) newPosition += GenerateTurbulentNoise(currentPosition, Time.time * TurbulenceTimeScale) * deltaTime;

                _previousPositions[i] = currentPosition;
                _segmentPositions[i] = newPosition;
            }

            UpdatePositions();

            for (int i = 0; i < ConstraintIterations; i++)
            {
                ApplyConstraints();
            }

        }
        public Vector3 GenerateTurbulentNoise(Vector3 position, float time)
        {
            var noise = Vector3.zero;
            var x = position.x * TurbulenceFrequency + time;
            var y = position.y * TurbulenceFrequency + time;
            var z = position.z * TurbulenceFrequency + time;

            var noiseX = Mathf.PerlinNoise(x, y) * 2.0f - 1.0f;
            var noiseY = Mathf.PerlinNoise(y, z) * 2.0f - 1.0f;
            var noiseZ = Mathf.PerlinNoise(z, x) * 2.0f - 1.0f;

            noise += new Vector3(noiseX, noiseY, noiseZ) * TurbulenceAmplitude;
            

            return noise;
        }
       

        void UpdatePositions()
        {
            if (UseTargetWindMode)
            {
                _segmentPositions[0]                = transform.position;
            }
            else
            {
                var dir = (Target.position - transform.position).normalized * _segmentLength * SegmentCount;
                _segmentPositions[0]                = transform.position;
                _segmentPositions[SegmentCount - 1] = transform.position + dir;
            }
        }


        private void ApplyConstraints()
        {     
            
            for (int i = 0; i < SegmentCount - 1; i++)
            {
                var topSection = _segmentPositions[i];

                var bottomSection = _segmentPositions[i + 1];

                //The distance between the sections
                float dist = (topSection - bottomSection).magnitude;

                //What's the stretch/compression
                float distError = Mathf.Abs(dist - _segmentLength);

                Vector3 changeDir = Vector3.zero;

                //Compress this sections
                if (dist > _segmentLength)
                {
                    changeDir = (topSection - bottomSection).normalized;
                }
                //Extend this section
                else if (dist < _segmentLength)
                {
                    changeDir = (bottomSection - topSection).normalized;
                }
                //Do nothing
                else
                {
                    continue;
                }


                Vector3 change = changeDir * distError;

                if (i != 0)
                {
                    bottomSection += change * 0.5f;

                    _segmentPositions[i + 1] = bottomSection;

                    topSection -= change * 0.5f;

                    _segmentPositions[i] = topSection;
                }
                //Because the rope is connected to something
                else
                {
                    bottomSection += change;

                    _segmentPositions[i + 1] = bottomSection;
                }
            }
        }

        void DrawRope()
        {
            if (_animLeftTime < MoveToTargetAnimationTime)
            {
               
                _animLerpVal  =  Mathf.Clamp01(_animLeftTime / (MoveToTargetAnimationTime + 0.00001f));
                var maxVertexCount = Mathf.CeilToInt(_animLerpVal * SegmentCount);
                var segmentLerpVal = (_animLerpVal * SegmentCount) % 1;

                LineRenderer.positionCount = maxVertexCount;


                for (int i = 0; i < maxVertexCount; i++)
                {
                    _lineRendererPositions[i] = _segmentPositions[maxVertexCount - 1 - i];
                }
                
                if(!UseTargetWindMode && maxVertexCount == SegmentCount) _lineRendererPositions[0] = Target.position;

                var p1 = _lineRendererPositions[0];
                var p2 = _lineRendererPositions[1];
                _lineRendererPositions[0] = Vector3.Lerp(p2, p1, segmentLerpVal);
                
                LineRenderer.SetPositions(_lineRendererPositions);
                _animLeftTime += Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < SegmentCount; i++)
                {
                    _lineRendererPositions[i] = _segmentPositions[SegmentCount - 1 - i];
                }
                if(!UseTargetWindMode) _lineRendererPositions[0] = Target.position;
                LineRenderer.SetPositions(_lineRendererPositions);
            }
            
        }

        public Vector3 GetForceEndPoint()
        {
            //return transform.position + (Target.position - transform.position).normalized * _forceSegmentLength * SegmentCount;
            return _segmentPositions[SegmentCount - 1];
        }

        public float GetTensionForce()
        {
            if (_forceLeftTime < ForceStartDelay || (ForceLifeTime > 0 && _forceLeftTime > ForceLifeTime + ForceStartDelay)) return 0;
            
            var currentSegmentDistance = ((Target.position - transform.position).magnitude) / SegmentCount;
            if (currentSegmentDistance > _segmentLength) return (currentSegmentDistance - _segmentLength) * SegmentCount * Force * _animLerpVal;
            else return 0;
        }

    }
}
