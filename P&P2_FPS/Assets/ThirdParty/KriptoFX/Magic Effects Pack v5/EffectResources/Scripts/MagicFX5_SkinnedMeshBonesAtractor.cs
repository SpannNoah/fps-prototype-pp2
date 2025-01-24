using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_SkinnedMeshBonesAtractor : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;
        public AnimationCurve     AttractionStrength = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float              StrengthMultiplier = 1;
        public float              Duration           = 5;
        public float              StartDelay         = 3;
        public  Transform OverrideTransform;


        private float     _leftTime;

        private bool _isFinished;
        private bool _stateChanged;

        private List<SkinnedMeshRenderer> _skins = new List<SkinnedMeshRenderer>();
        private List<VirtualBone>         _bones = new List<VirtualBone>();

        class VirtualBone
        {
            public Transform Bone;
            Vector3          _virtualPos;

            public bool IsBoneInitialized = false;

            public VirtualBone(Transform bone)
            {
                Bone              = bone;
                IsBoneInitialized = false;
            }

            public Vector3 GetBonePosition()
            {
                if (!IsBoneInitialized)
                {
                    _virtualPos       = Bone.position;
                    IsBoneInitialized = true;
                }
                return _virtualPos;
            }

            public void UpdateBone(Vector3 pos)
            {
                Bone.position = pos;
                _virtualPos   = pos;
            }
          
        }

        void OnEnable()
        {
            _leftTime     = 0;
            _isFinished   = false;
            _stateChanged = false;

            EffectSettings.OnEffectCollisionEnter += OnCollisionImpactEnter;
        }

        void OnDisable()
        {
            EffectSettings.OnEffectCollisionEnter -= OnCollisionImpactEnter;

            SetSkinState(true);
            _bones.Clear();
            _skins.Clear();
        }

        public void OnCollisionImpactEnter(MagicFX5_EffectSettings.EffectCollisionHit hitInfo)
        {
            var skins     = hitInfo.Target.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skin in skins)
            {
               // _bones.Add(new VirtualBone(skin.rootBone));
                var bones = skin.rootBone.GetComponentsInChildren<Transform>();
                foreach (var skinBone in bones)
                {
                    if(skinBone == skin.rootBone) continue;
                    
                    _bones.Add(new VirtualBone(skinBone));
                }
                _skins.Add(skin);
            }
        }

        void LateUpdate()
        {
            if (_isFinished) return;

            var deltaTime = Time.deltaTime;
            _leftTime += deltaTime;

            if (_leftTime < StartDelay) return;

            if (!_stateChanged)
            {
               // SetRagdollState(false);
               // SetAnimatorState(false);
                _stateChanged = true;
            }

            var centerPos = OverrideTransform ? OverrideTransform.position : transform.position;
            var timeValue  = Mathf.Clamp01((_leftTime - StartDelay) / Duration);

            var strength = AttractionStrength.Evaluate((_leftTime - StartDelay) / Duration) * StrengthMultiplier;
            for (var i = 0; i < _bones.Count; i++)
            {
                var newPos   = Vector3.MoveTowards(_bones[i].GetBonePosition(), centerPos, deltaTime * strength);
                _bones[i].UpdateBone(newPos);
            }

            if ((_leftTime - StartDelay) > Duration)
            {
                SetSkinState(false);
             
                _isFinished = true;

            }
        }
        

        private void SetSkinState(bool isEnabled)
        {
            foreach (var skin in _skins)
            {
                if (skin != null) skin.enabled = isEnabled;
            }
        }

    }
}
