using System.Collections;

using UnityEngine;
using UnityEngine.Rendering;

namespace MagicFX5
{
    public class MagicFX5_SkinnedMeshClone : MonoBehaviour
    {
        public MagicFX5_EffectSettings        EffectSettings;
        public Material                  CloneMaterial;
        public RuntimeAnimatorController AnimController;
        public bool                      DeactivateNonSkinnedMeshes = true;

        public Gradient                                    ColorOverLifetime = new Gradient();
        public MagicFX5_ShaderColorCurve.ME2_ShaderPropertyName ShaderNameColor   = MagicFX5_ShaderColorCurve.ME2_ShaderPropertyName._Color;
        public float                                       LifeTime          = 8;

        private GameObject _cloneInstance;
        private Material   _materialInstance;

        private float _leftTime;
        private int   _shaderColorID;
        private Color _startColor;

        void OnEnable()
        {
            _shaderColorID                        =  Shader.PropertyToID(ShaderNameColor.ToString());
            _startColor                           =  CloneMaterial.GetColor(_shaderColorID);

            EffectSettings.OnEffectCollisionEnter += OnCollisionImpactEnter;
        }

        void OnDisable()
        {
            EffectSettings.OnEffectCollisionEnter -= OnCollisionImpactEnter;
          
            StopCoroutine(UpdateMaterial());
            if (_cloneInstance != null) Destroy(_cloneInstance);
            if (_materialInstance != null) Destroy(_materialInstance);
        }

        public void OnCollisionImpactEnter(MagicFX5_EffectSettings.EffectCollisionHit hitInfo)
        {
            var anim = hitInfo.Target.GetComponent<Animator>();
            if (anim == null) return;

            _cloneInstance    = Instantiate(anim.gameObject, anim.transform.position, anim.transform.rotation);
            _materialInstance = new Material(CloneMaterial) { hideFlags = HideFlags.HideAndDontSave };
            _materialInstance.SetVector(_shaderColorID, ColorOverLifetime.Evaluate(0) * _startColor);

            var cloneAnim                               = _cloneInstance.GetComponent<Animator>();
            var cloneSkins                              = _cloneInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (DeactivateNonSkinnedMeshes)
            { 
                var particles = _cloneInstance.GetComponentsInChildren<ParticleSystem>();
                foreach (var particle in particles)
                {
                    particle.gameObject.SetActive(false);
                }
               
            } 

            cloneAnim.runtimeAnimatorController = AnimController;
            foreach (var skin in cloneSkins)
            {
                var rend = skin.GetComponent<Renderer>();
                rend.sharedMaterial    = _materialInstance;
                rend.shadowCastingMode = ShadowCastingMode.Off;
            }
            

            Destroy(_cloneInstance, LifeTime);

            StartCoroutine(UpdateMaterial());
        }

        IEnumerator UpdateMaterial()
        {
            while (_leftTime < LifeTime)
            {
                _leftTime += Time.deltaTime;
                var evaluatedTime = _leftTime                                                         / LifeTime;
                _materialInstance.SetVector(_shaderColorID, ColorOverLifetime.Evaluate(evaluatedTime) * _startColor);

                yield return null;
            }
        }
    }
}
