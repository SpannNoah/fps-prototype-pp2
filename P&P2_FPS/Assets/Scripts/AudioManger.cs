using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class AudioManger : MonoBehaviour
{

    public static AudioManger instance;

    [Header("Audio Clips")]
    public AudioSource backgroundMusic;
    public AudioSource buttonClick;
    public AudioSource playerJump;
    public AudioSource playerHurt;
    public AudioSource playerDeath;
    public AudioSource fireGun;
    public AudioSource lightningAmmo;
    public AudioSource fireAmmo;
    public AudioSource iceAmmo;
    public AudioSource portalSound;
    public AudioSource enemyDeath;

    [Header("Background Music")]
    public AudioClip mainMenuMusic;
    public AudioClip levelMusic;

    [Header("Volume Settings")]
    [Range(0, 1)] public float backgroundMusicVolume = 0.5f;
    [Range(0, 1)] public float soundEffectVolume = 0.5f;

    private bool isMuted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic(mainMenuMusic);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusic.clip != clip)
        {
            backgroundMusic.clip = clip;
            backgroundMusic.volume = backgroundMusicVolume;
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    public void PlayButtonClick()
    {
        buttonClick.volume = isMuted ? 0 : soundEffectVolume;
        buttonClick.Play();
    }

    public void PlayPlayerJump()
    {
        playerJump.volume = isMuted ? 0 : soundEffectVolume;
        playerJump.Play();
    }

    public void PlayPlayerHurt()
    {
        playerHurt.volume = isMuted ? 0 : soundEffectVolume;
        playerHurt.Play();
    }

    public void PlayPlayerDeath()
    {
        playerDeath.volume = isMuted ? 0 : soundEffectVolume;
        playerDeath.Play();
    }

    public void PlayFireGun()
    {
        fireGun.volume = isMuted ? 0 : soundEffectVolume;
        fireGun.Play();
    }

    public void PlayLightningAmmo()
    {
        lightningAmmo.volume = isMuted ? 0 : soundEffectVolume;
        lightningAmmo.Play();
    }

    public void PlayFireAmmo()
    {
        fireAmmo.volume = isMuted ? 0 : soundEffectVolume;
        fireAmmo.Play();
    }

    public void PlayIceAmmo()
    {
        iceAmmo.volume = isMuted ? 0 : soundEffectVolume;
        iceAmmo.Play();
    }

    public void PlayPortalSound()
    {
        portalSound.volume = isMuted ? 0 : soundEffectVolume;
        portalSound.Play();
    }

    public void PlayEnemyDeath()
    {
        enemyDeath.volume = isMuted ? 0 : soundEffectVolume;
        enemyDeath.Play();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        backgroundMusic.volume = isMuted ? 0 : backgroundMusicVolume;
        buttonClick.volume = isMuted ? 0 : soundEffectVolume;
        playerJump.volume = isMuted ? 0 : soundEffectVolume;
        playerHurt.volume = isMuted ? 0 : soundEffectVolume;
        playerDeath.volume = isMuted ? 0 : soundEffectVolume;
        fireGun.volume = isMuted ? 0 : soundEffectVolume;
        lightningAmmo.volume = isMuted ? 0 : soundEffectVolume;
        fireAmmo.volume = isMuted ? 0 : soundEffectVolume;
        iceAmmo.volume = isMuted ? 0 : soundEffectVolume;
        portalSound.volume = isMuted ? 0 : soundEffectVolume;
        enemyDeath.volume = isMuted ? 0 : soundEffectVolume;

    }

    private void LoadVolumeSettings()
    {
        backgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.5f);
        soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", 0.5f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicVolume);
        PlayerPrefs.SetFloat("SoundEffectVolume", soundEffectVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}


