
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace MagicFX5
{
    public class MagicFX5_ParticleGravity : MonoBehaviour
    {
      
        public Transform      Target;
        public AnimationCurve ForceCurve    = AnimationCurve.EaseInOut(0, 1, 1, 1);
        public float          Force         = 1;
        public float          ForceLifeTime = -1;

        public bool    UseRotation       = false;
        public Vector3 RotationForceAxis = Vector3.up;
        public float   RotationSpeed     = 1;

        public Vector2 ForceByDistanceRemap = new Vector2(0, 1f);

        public bool  UseStopDistance = false;
        public float StopDistance    = 0.5f;
        public float Delay           = 0;

        [Space] 
        public bool AffectParticlesSequentially;
        public int AffectParticlesPerFrame = 1;


        ParticleSystem             _ps;
        private UpdateParticlesJob _job = new UpdateParticlesJob();

        private float _leftTime;
        private int   _leftAffectedParticles = 0;


        void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        void OnEnable()
        {
            _leftTime              = 0;
            _leftAffectedParticles = 0;
        }

        void OnParticleUpdateJobScheduled()
        {
            _leftTime                 += Time.deltaTime;
            if (_leftTime < Delay) return;
            if (ForceLifeTime > 0 && _leftTime - Delay > ForceLifeTime) return;

            var currentForce = ForceLifeTime > 0 ? ForceCurve.Evaluate((_leftTime - Delay) / ForceLifeTime) : 1;

            _job.CurrentForce                = Time.deltaTime * Force * currentForce;
            _job.TargetPosition              = Target.position;
            _job.ForceByDistanceRemap        = ForceByDistanceRemap;
            _job.UseStopDistance             = UseStopDistance;
            _job.StopDistance                = StopDistance;
            _job.UseRotation                 = UseRotation;
            _job.RotationForceAxis           = RotationForceAxis;
            _job.RotationSpeed               = RotationSpeed;
            _job.AffectParticlesSequentially = AffectParticlesSequentially;
            _job.AffectParticlesPerFrame     = AffectParticlesPerFrame;
            _job.LeftAffectedParticles       = _leftAffectedParticles;

            _leftAffectedParticles += AffectParticlesPerFrame;

            _job.Schedule(_ps);
        }

        struct UpdateParticlesJob : IJobParticleSystem
        {
            public float   CurrentForce;
            public Vector3 TargetPosition;
            public Vector2 ForceByDistanceRemap; 
            public bool    UseStopDistance;
            public float   StopDistance;

            public bool    UseRotation;
            public Vector3 RotationForceAxis;
            public float   RotationSpeed;

            public bool AffectParticlesSequentially;
            public int  AffectParticlesPerFrame;
            public int  LeftAffectedParticles;

            public void Execute(ParticleSystemJobData particles)
            {
                int particleCount       = particles.count;
                var particlesVelocities = particles.velocities;
                var positions           = particles.positions;

                var endPos                         = AffectParticlesSequentially ? LeftAffectedParticles + AffectParticlesPerFrame : particleCount;
                if (endPos > particleCount) endPos = particleCount;

              
                for (int i = 0; i < endPos; i++)
                {
                    var distanceToTarget  = Vector3.Distance(TargetPosition, positions[i]);
                    var directionToTarget = Vector3.Normalize(TargetPosition - positions[i]);

                    if (UseStopDistance && distanceToTarget < StopDistance) particlesVelocities[i] = directionToTarget * 0.0001f;
                    else
                    {
                        //var distanceForce     = Mathf.SmoothStep(ForceByDistanceRemap.x, ForceByDistanceRemap.y, distanceToTarget);
                        var seekForce = directionToTarget * CurrentForce * ForceByDistanceRemap.y;

                        if (UseRotation)
                        {
                            var directionToCenter = TargetPosition - positions[i];
                            var rotationForce     = Vector3.Cross(directionToCenter, RotationForceAxis).normalized * CurrentForce * RotationSpeed;

                            particlesVelocities[i] += seekForce;
                            particlesVelocities[i] += rotationForce;
                        }
                        else particlesVelocities[i] += seekForce;
                    }
                }
                
            }
        }
    }
}