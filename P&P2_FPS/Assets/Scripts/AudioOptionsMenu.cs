using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AudioOptionsMenu : MonoBehaviour
{
    public Slider backgroundMusicSlider;
    public Slider soundEffectSlider;


    // Start is called before the first frame update
    void Start()
    {
     // Load audio settings
     backgroundMusicSlider.value = AudioManger.instance.backgroundMusicVolume;
     soundEffectSlider.value = AudioManger.instance.soundEffectVolume;

        // Add listeners to sliders
     backgroundMusicSlider.onValueChanged.AddListener(SetBackGroundMusicVolume);
     soundEffectSlider.onValueChanged.AddListener(SetSoundEffectVolume);
    }

    public void SetBackGroundMusicVolume(float volume)
    {
        AudioManger.instance.AdjustBackgroundMusicVolume(volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        AudioManger.instance.AdjustSoundEffectVolume(volume);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
