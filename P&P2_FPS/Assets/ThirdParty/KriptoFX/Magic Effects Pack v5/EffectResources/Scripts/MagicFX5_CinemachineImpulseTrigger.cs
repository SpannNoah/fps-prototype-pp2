using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_CINEMACHINE
using Cinemachine;
#endif

namespace MagicFX5
{
    public class MagicFX5_CinemachineImpulseTrigger : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;
#if USING_CINEMACHINE

        
        void OnEnable()
        {
            if (EffectSettings.UseCameraShakeCinemachine) StartCoroutine(OneFrameDelayInitialization());
        }

        void OnDisable()
        {
            if (EffectSettings.UseCameraShakeCinemachine) StopCoroutine(OneFrameDelayInitialization());
        }

        IEnumerator OneFrameDelayInitialization()
        {
            yield return null; //1 frame delay for correct shaking initialization by time, else unity call Instantiate -> OnEnable -> Shake(!) -> Start Delay script -> Shake again later. 
            
            var source = GetComponent<CinemachineImpulseSource>();
            source.GenerateImpulse(1);
        }

#endif
    }
}
