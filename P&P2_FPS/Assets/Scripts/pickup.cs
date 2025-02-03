using UnityEngine;
using System.Collections.Generic;

public class pickup : MonoBehaviour
{
    enum pickupType { gun, HP, armor, ammo, melee}

    public bool m_isReward = false;
    [SerializeField] pickupType m_type;
    [SerializeField] gunStats m_gunStats;
    [SerializeField] AmmoCartridge m_ammoCartridge = null;

    [SerializeField] bool m_playVoiceLineOnPickup = false;
    [SerializeField] List<AudioClip> m_voiceLines = new List<AudioClip>();

    void Start()
    {
        if(m_type == pickupType.gun)
        {
            m_gunStats.ammoCur = m_gunStats.ammoMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(m_playVoiceLineOnPickup)
            {
                foreach(AudioClip voiceLine in m_voiceLines)
                {
                    int voiceLineIndex = VoiceSystemManager.Instance.GetVoiceLineIndex(voiceLine);
                    VoiceSystemManager.Instance.QueueVoiceLine(voiceLineIndex);
                }
            }
            if (m_type == pickupType.gun)
            {
                if (other.TryGetComponent(out GunManager gunManager))
                {
                    gunManager.EquipGun(m_gunStats, m_ammoCartridge);
                    AmmoManager.Instance.SetCurrentCartridge(m_ammoCartridge);
                    Destroy(gameObject);
                }
            }
            else if (m_type == pickupType.melee)
            {
                if (other.TryGetComponent(out GunManager gunManager))
                {
                    gunManager.EquipMelee(m_gunStats);
                    Destroy(gameObject);
                }
            }
        }
    }
}
