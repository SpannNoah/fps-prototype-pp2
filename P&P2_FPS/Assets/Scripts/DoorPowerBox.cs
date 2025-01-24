using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPowerBox : MonoBehaviour, IDamage
{
    [SerializeField] private bool m_isBroken = false;
    [SerializeField] List<ParticleSystem> m_particleSystems = new List<ParticleSystem>();
    [SerializeField] private InteractableDoor m_doorToOpen = null;
    [SerializeField] private bool m_playVoiceLine = false;
    [SerializeField] private List<int> m_voiceLineIndexes = new List<int>();

    public void TakeDamage(int amount)
    {
        m_doorToOpen.Interact();

        if (m_playVoiceLine)
        {
            foreach(int index in m_voiceLineIndexes)
            {
                VoiceSystemManager.Instance.QueueVoiceLine(index);
            }
            
            m_playVoiceLine = false;
        }

    }
}
