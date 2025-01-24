using System;
using System.Collections;
using System.Collections.Generic;
using MagicFX5;
using UnityEngine;

public class MagicFX5_EnemyDisintegration : MonoBehaviour
{
    public SkinnedMeshRenderer   Renderer;
    public GameObject Particles;
    public float      ParticlesLifeTime = 7;
    [Space]
    public Gradient FadeInColorOverLifeTime;
    public float    FadeInTime   = 0.8f;
   
    [Space]
    public Gradient FadeOutColorOverLifeTime;
    public float    FadeOutTime = 4;
    
  
    public MagicFX5_ShaderColorCurve.ME2_ShaderPropertyName ShaderName = MagicFX5_ShaderColorCurve.ME2_ShaderPropertyName._Color;
    
    private int                                         _shaderID;
    private Renderer                                    _rend;
    private Color                                       _startColor;
    private float                                       _leftTime;

    private EffectStateEnum _state;

    private GameObject _particlesInstance;
    private Material   _materialInstance;


    enum EffectStateEnum
    {
        InitialStarted,
        InitialFinished,
        ImpactStarted,
        ImpactFinished
    }
    void Awake()
    {
        _rend       = GetComponent<Renderer>();
        _shaderID   = Shader.PropertyToID(ShaderName.ToString());
        _startColor = Renderer.sharedMaterial.GetColor(_shaderID);
    }

    void OnEnable()
    {
        _leftTime            = 0;
        _state               = EffectStateEnum.InitialStarted;

        _materialInstance = Renderer.material;
        _materialInstance.SetVector(_shaderID, Vector4.zero);
    }

    void OnDisable()
    {
        if(_particlesInstance != null) Destroy(_particlesInstance);

        CancelInvoke("DelayImpact");
    }

    public void Disintegrate(float delay)
    {
        Invoke("DelayImpact", delay);
    }

    void DelayImpact()
    {
        _particlesInstance = Instantiate(Particles, transform);
        Destroy(_particlesInstance, ParticlesLifeTime);

        var shape = _particlesInstance.GetComponentInChildren<ParticleSystem>().shape;
        shape.enabled             = true;
        shape.skinnedMeshRenderer = Renderer;

        _state    = EffectStateEnum.ImpactStarted;
        _leftTime = 0;

        _materialInstance.SetVector(_shaderID, Vector4.one);
    }

    void Update()
    {
        _leftTime += Time.deltaTime;

        switch (_state)
        {
            case EffectStateEnum.InitialStarted:
                UpdateColor(FadeInColorOverLifeTime, FadeInTime, EffectStateEnum.InitialFinished);
                break;

            case EffectStateEnum.ImpactStarted:
                UpdateColor(FadeOutColorOverLifeTime, FadeOutTime, EffectStateEnum.ImpactFinished);
                break;
           
        }
    }

    private void UpdateColor(Gradient grad, float lifeTime, EffectStateEnum endState)
    {
        var currentColor = grad.Evaluate(_leftTime / lifeTime) * _startColor;
        _materialInstance.SetVector(_shaderID, currentColor);
        if (_leftTime > lifeTime) _state = endState;
    }
}
