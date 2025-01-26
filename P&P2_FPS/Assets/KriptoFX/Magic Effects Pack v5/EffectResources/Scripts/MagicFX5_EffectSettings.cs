using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicFX5_EffectSettings : MonoBehaviour
{
    public                                       Transform[] Targets;
    public                                       float       TargetCenterHeightOffset = 0.35f;
#if UNITY_EDITOR
    [ShowIf("_isProjectile", true, true)]
# endif
    public float       ProjectileSpeed          = 15;

    [Space(25)]
    public bool            UseSkinMeshImpactEffects  = true;
    public bool            UseCameraShakeCinemachine = true;

    public                            bool      UseForce        = true;
#if UNITY_EDITOR
    [ShowIf("UseForce", true)]
#endif 
    public LayerMask ForceLayerMask  = -1;
#if UNITY_EDITOR
    [ShowIf("UseForce", true)]
#endif 
    public float     Force           = 3;
#if UNITY_EDITOR
    [ShowIf("UseForce", true)]
#endif  
    public float     ForceRadius     = 10;
    public bool      UseRagdollForce = true;

    [Space] 
    public bool UseAnimatorTriggerAfterCollision = false;
    public string[] AnimatorRandomTriggerNames;

    public Action<EffectCollisionHit> OnEffectCollisionEnter;
    public Action<EffectCollisionHit> OnEffectSkinActivated;
    public Action<EffectCollisionHit> OnEffectImpactActivated;

    private int _animatorTriggerLastIndex;
#if UNITY_EDITOR
    [HideInNormalInspector]
#endif
    public bool _isProjectile = true;

    public struct EffectCollisionHit
    {
        public Transform Target;
        public Vector3      Position;
        public Vector3      Normal;
    }

    void OnEnable()
    {
        if (UseAnimatorTriggerAfterCollision) OnEffectCollisionEnter += OnEffectCollisionEnterTrigger;
    }

    void OnDisable()
    {
        if (UseAnimatorTriggerAfterCollision) OnEffectCollisionEnter -= OnEffectCollisionEnterTrigger;

        _targetCache.Clear();
        _triggeredAnimators.Clear();
        _animatorTriggerLastIndex = 0;
    }


    private void OnEffectCollisionEnterTrigger(EffectCollisionHit hit)
    {
        TriggerAnimator(hit.Target);
    }

    void TriggerAnimator(Transform target)
    {
        if (!UseAnimatorTriggerAfterCollision || AnimatorRandomTriggerNames.Length == 0) return;

        var currentAnimator = target.GetComponent<Animator>();
        if (currentAnimator == null) currentAnimator = target.GetComponentInParent<Animator>(); //try again to find an animator, for example when collided body parts

        if (currentAnimator != null && !_triggeredAnimators.Contains(currentAnimator))
        {
            _triggeredAnimators.Add(currentAnimator);
            currentAnimator.SetTrigger(AnimatorRandomTriggerNames[_animatorTriggerLastIndex]);
            if (++_animatorTriggerLastIndex >= AnimatorRandomTriggerNames.Length) _animatorTriggerLastIndex = 0;
        }
    }


    private Dictionary<Transform, TargetCache> _targetCache        = new Dictionary<Transform, TargetCache>();
    private List<Animator>                     _triggeredAnimators = new List<Animator>();

    private class TargetCache
    {
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        public Renderer            Renderer;

        public TargetCache(Transform target)
        {
            SkinnedMeshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
            if(SkinnedMeshRenderer == null) Renderer            = target.GetComponentInChildren<Renderer>();
        }
    }

    internal Vector3 GetTargetCenter(Transform target)
    {
        if(!_targetCache.ContainsKey(target)) _targetCache.Add(target, new TargetCache(target));
      
        var cache = _targetCache[target]; 

        if (cache.SkinnedMeshRenderer != null) return cache.SkinnedMeshRenderer.bounds.center + Vector3.up * TargetCenterHeightOffset;
        if (cache.Renderer            != null) return cache.Renderer.bounds.center            + Vector3.up * TargetCenterHeightOffset;
       


        return target.position + Vector3.up * TargetCenterHeightOffset;
    }


}
