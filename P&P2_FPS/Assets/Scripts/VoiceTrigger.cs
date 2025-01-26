using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    public List<AudioClip> m_voiceLines = new List<AudioClip>(); // Which line to play

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(AudioClip voiceLine in m_voiceLines)
            {
                int voiceLineIndex = VoiceSystemManager.Instance.GetVoiceLineIndex(voiceLine);
                VoiceSystemManager.Instance.QueueVoiceLine(voiceLineIndex);
            }
            Destroy(gameObject); // Prevents repeat triggers
        }
    }
}
