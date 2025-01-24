using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundType
{
    BGM,
    Jump,
    Footsteps,
    TakeDamage,
    Teleport,
    ShootNormal,
    ShootFire,
    ShootLightning,
    ShootIce,
    DoorOpen,
    DoorClose
}

[RequireComponent(typeof(AudioSource))]
public class AudioManger : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static AudioManger audioManager;
    private AudioSource audioSource;

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = this;
            DontDestroyOnLoad(audioManager);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    public static void PlaySound(SoundType sound, float volume = 1)
    {
      audioManager.audioSource.PlayOneShot(audioManager.soundList[(int)sound], volume);
    }

    
   

}
