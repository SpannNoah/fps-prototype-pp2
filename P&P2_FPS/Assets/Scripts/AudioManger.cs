using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(AudioSource))]
public class AudioManger : MonoBehaviour
{

    public static AudioManger instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusic;
    public AudioSource leftAmmoAudioSource;
    public AudioSource rightAmmoAudioSource;
    public AudioSource buttonClick;
    public AudioSource playerJump;
    public AudioSource playerHurt;
    public AudioSource playerDeath;
    public AudioSource portalSound;
    public AudioSource enemyDeath;

    [Header("Audio Clips")]
    public AudioClip basicAmmoSFX;
    public AudioClip lightningAmmoSFX;
    public AudioClip fireAmmoSFX;
    public AudioClip iceAmmoSFX;

    [Header("Background Music")]
    public AudioClip mainMenuMusic;
    public List<AudioClip> levelMusic = new List<AudioClip>();

    [Header("Volume Settings")]
    [Range(0, 1)] public float backgroundMusicVolume = 0.5f;
    [Range(0, 1)] public float soundEffectVolume = 0.5f;


    private const string LEVEL1_NAME = "Level1";
    private const string LEVEL2_NAME = "Level 2";
    private const string LEVEL3_NAME = "Room3-Michael";
    private const string LEVEL4_NAME = "BossScene";

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

        backgroundMusic = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        string levelName = scene.name;

        switch (scene.name)
        {
            case LEVEL1_NAME:
                PlayBackgroundMusic(levelMusic[0]);
                break;
            case LEVEL2_NAME:
                PlayBackgroundMusic(levelMusic[1]);
                break;
            case LEVEL3_NAME:
                PlayBackgroundMusic(levelMusic[2]);
                break;
            case LEVEL4_NAME:
                PlayBackgroundMusic(levelMusic[3]);
                break;
            default:
                break;
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

    public void PlayAmmoSFX(DamageType damageType, bool isLeft)
    {
        switch (damageType)
        {
            case DamageType.Basic:
                if (isLeft)
                {
                    leftAmmoAudioSource.PlayOneShot(basicAmmoSFX);
                }
                else
                {
                    rightAmmoAudioSource.PlayOneShot(basicAmmoSFX);
                }
                break;
            case DamageType.Lightning:
                if (isLeft)
                {
                    leftAmmoAudioSource.PlayOneShot(lightningAmmoSFX);
                }
                else
                {
                    rightAmmoAudioSource.PlayOneShot(lightningAmmoSFX);
                }
                break;
            case DamageType.Ice:
                if (isLeft)
                {
                    leftAmmoAudioSource.PlayOneShot(iceAmmoSFX);
                }
                else
                {
                    rightAmmoAudioSource.PlayOneShot(iceAmmoSFX);
                }
                break;
            case DamageType.Fire:
                if (isLeft)
                {
                    leftAmmoAudioSource.PlayOneShot(fireAmmoSFX);
                }
                else
                {
                    rightAmmoAudioSource.PlayOneShot(fireAmmoSFX);
                }
                break;
            default:
                break;
        }

        if (isLeft)
        {
            leftAmmoAudioSource.volume = isMuted ? 0 : soundEffectVolume;
        }
        else
        {
            rightAmmoAudioSource.volume = isMuted ? 0 : soundEffectVolume;
        }
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
        leftAmmoAudioSource.volume = isMuted ? 0 : soundEffectVolume;
        rightAmmoAudioSource.volume = isMuted ? 0 : soundEffectVolume;
        portalSound.volume = isMuted ? 0 : soundEffectVolume;
        enemyDeath.volume = isMuted ? 0 : soundEffectVolume;
        
    }

    public void AdjustBackgroundMusicVolume(float targetVolume)
    {
        backgroundMusicVolume = targetVolume;
        backgroundMusic.volume = targetVolume;
        SaveVolumeSettings();
    }

    public void AdjustSoundEffectVolume(float targetVolume)
    {
        soundEffectVolume = targetVolume;
        buttonClick.volume = isMuted ? 0 : soundEffectVolume;
        playerJump.volume = isMuted ? 0 : soundEffectVolume;
        playerHurt.volume = isMuted ? 0 : soundEffectVolume;
        playerDeath.volume = isMuted ? 0 : soundEffectVolume;
        leftAmmoAudioSource.volume = isMuted ? 0 : soundEffectVolume;
        rightAmmoAudioSource.volume = isMuted ? 0 : soundEffectVolume;
        portalSound.volume = isMuted ? 0 : soundEffectVolume;
        enemyDeath.volume = isMuted ? 0 : soundEffectVolume;
        SaveVolumeSettings();
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


