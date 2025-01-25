using System;
using UnityEngine;


namespace MagicFX5
{

    public class MagicFX5_AnimationEvents : MonoBehaviour
    {
        public Action OnTriggerMainEffect;
        public Action OnTriggerHandEffect;
        public Action OnTriggerBuffEffect;
        public Action OnTriggerAudio;

        public void TriggerMainEffect()
        {
            OnTriggerMainEffect?.Invoke();
        }

        public void TriggerHandEffect()
        {
            OnTriggerHandEffect?.Invoke();
        }

        public void TriggerBuffEffect()
        {
            OnTriggerBuffEffect?.Invoke();
        }
        public void TriggerAudio()
        {
            OnTriggerAudio?.Invoke();
        }
    }
}