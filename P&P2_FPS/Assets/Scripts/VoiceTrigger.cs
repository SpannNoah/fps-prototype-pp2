using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    public int voiceLineIndex; // Which line to play

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VoiceSystemManager.Instance.QueueVoiceLine(voiceLineIndex);
            Destroy(gameObject); // Prevents repeat triggers
        }
    }
}
