using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_RagdollTrigger : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;
        public float StartDelayRagdoll = 0.05f;
        public float RagdollLifeTime = -1;

        [Space]
        public bool UseAntigravity = false;
        public float          AntigravityLifeTime        = 3;
        public AnimationCurve AntigravityForceCurve      = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float          AntigravityForceMultiplier = 0.1f;
        public float          AntigravityTorqueMultiplier = 0.1f;
        [Range(0, 100)]
        public float AntigravityForceRandomRangePercent = 25;

        private List<Hip>       _ragdollHips        = new List<Hip>();
        private List<Bone>      _startBonesTransform               = new List<Bone>();
        private List<Rigidbody> _ragdollRigidbodies = new List<Rigidbody>();
        private List<Animator>  _animators          = new List<Animator>();
        private float           _gravityLiftTime    = 0;


        struct Bone
        {
            public Transform  BoneTransform;
            public Vector3    LocalPos;
            public Quaternion LocalRotation;

            public Bone(Rigidbody rg)
            {
                BoneTransform = rg.transform;
                LocalPos      = rg.transform.localPosition;
                LocalRotation = rg.transform.localRotation;
            }
        }


        struct Hip
        {
            public Rigidbody  HipRigidbody;
            public float      InitialForceMultiplier;
            public Vector3 InitialRotationTorque;

            public Hip(Rigidbody rg, float forceMultiplier, float randomRangePercent)
            {
                HipRigidbody           = rg;
                InitialForceMultiplier = forceMultiplier + forceMultiplier * Random.Range(-randomRangePercent, randomRangePercent) / 100.0f;
                InitialRotationTorque  = Random.insideUnitSphere;
            }
        }

        internal void OnEnable()
        {
            StartCoroutine(Initialize());
        }

        internal void OnDisable()
        {
            StopCoroutine(Initialize());

            foreach (var anim in _animators)
            {
                if (anim != null) anim.enabled = true;
            }
            if (UseAntigravity) foreach (var rg in _ragdollRigidbodies)
            {
                if (rg != null) rg.useGravity = true;
            }

            CancelInvoke("DisableAntigravity");
            CancelInvoke("DisableRagdoll");
        }

       
        void FixedUpdate()
        {
            if (UseAntigravity) UpdateGravity();
        }

        IEnumerator Initialize()
        {
            yield return null; //1 frame delay for correct targets initialization, because Instantiate -> OnEnable -> SetTargets

            yield return new WaitForSeconds(StartDelayRagdoll);

            _ragdollHips.Clear();
            _startBonesTransform.Clear();
            _ragdollRigidbodies.Clear();
            _animators.Clear();
            _gravityLiftTime = 0;

            foreach (var target in EffectSettings.Targets)
            {
                var rigids = target.GetComponentsInChildren<Rigidbody>();
                if(rigids == null || rigids.Length == 0) continue;
                

                foreach (var rg in rigids)  
                {
                    rg.isKinematic = true;
                    if (UseAntigravity) rg.useGravity = false;

                    _ragdollRigidbodies.Add(rg);
                    _startBonesTransform.Add(new Bone(rg));
                }

                var anim = target.GetComponent<Animator>();
                if (anim != null) _animators.Add(anim);

                var skin = target.GetComponentInChildren<SkinnedMeshRenderer>();
                if (skin != null)
                {
                    _ragdollHips.Add(new Hip(skin.rootBone.GetComponent<Rigidbody>(), AntigravityForceMultiplier, AntigravityForceRandomRangePercent));
                }
                else
                {
                    _ragdollHips.Add(new Hip(rigids[0], AntigravityForceMultiplier, AntigravityForceRandomRangePercent));
                }

            }
            //I need to use kinematic = true for the correct ragdoll start forces. Without unity apply random forces to colliders.  
            //also loooks like I can't disable animator in the same frame, in this case physics just freezes. 
            yield return null; 
            foreach (var rg in _ragdollRigidbodies) { rg.isKinematic = false; }
            yield return null;
            foreach (var anim in _animators) anim.enabled = false;

            if (UseAntigravity) Invoke("DisableAntigravity", AntigravityLifeTime);
            if (RagdollLifeTime > 0) Invoke("DisableRagdoll", RagdollLifeTime);

        }

        void DisableAntigravity()
        {
            foreach (var rg in _ragdollRigidbodies) rg.useGravity = true;
        }

        void DisableRagdoll()
        {
            foreach (var anim in _animators) anim.enabled = true;
        }

        void UpdateGravity()
        {
            if (_ragdollHips.Count == 0) return;

            _gravityLiftTime += Time.fixedDeltaTime;

            foreach (var hip in _ragdollHips)
            {
                var force = AntigravityForceCurve.Evaluate(_gravityLiftTime / AntigravityLifeTime);
                hip.HipRigidbody.AddForce(Vector3.up * force * hip.InitialForceMultiplier, ForceMode.Force);
                hip.HipRigidbody.AddTorque(hip.InitialRotationTorque * AntigravityTorqueMultiplier);
            }
        }

    }
}
