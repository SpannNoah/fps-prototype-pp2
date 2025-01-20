using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceSystemManager : MonoBehaviour
{
    public static VoiceSystemManager Instance;

    [Header("Audio Components")]
    public AudioSource audioSource;
    public AudioClip staticNoise;
    public VoiceLine[] voiceLines;

    [Header("Subtitle Components")]
    public TextMeshProUGUI subtitleText;
    public GameObject m_subtitlePanel = null;

    private int currentLine = 0;
    private bool isPlaying = false;

    private Queue<VoiceLine> voiceQueue = new Queue<VoiceLine>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        subtitleText.gameObject.SetActive(false);
        m_subtitlePanel.SetActive(false);
    }

    public void QueueVoiceLine(int index)
    {
        if (index < voiceLines.Length)
        {
            voiceQueue.Enqueue(voiceLines[index]);
            if (!isPlaying)
            {
                PlayNextFromQueue();
            }
        }
    }

    private void PlayNextFromQueue()
    {
        if (voiceQueue.Count > 0)
        {
            StartCoroutine(PlayWithStatic(voiceQueue.Dequeue()));
        }
    }

    private IEnumerator PlayWithStatic(VoiceLine voiceLine)
    {
        isPlaying = true;

        if (staticNoise)
        {
            audioSource.PlayOneShot(staticNoise);
            yield return new WaitForSeconds(staticNoise.length);
        }

        audioSource.PlayOneShot(voiceLine.audioClip);
        ShowSubtitle(voiceLine.subtitle);

        yield return new WaitForSeconds(voiceLine.audioClip.length + 0.5f);

        if (voiceQueue.Count > 0)
        {
            PlayNextFromQueue(); 
        }
        else
        {
            isPlaying = false;
        }
    }

    private void ShowSubtitle(string text)
    {
        subtitleText.text = text;
        m_subtitlePanel.SetActive(true);
        subtitleText.gameObject.SetActive(true);
        StartCoroutine(HideSubtitleAfterDelay());
    }

    private IEnumerator HideSubtitleAfterDelay()
    {
        yield return new WaitForSeconds(voiceLines[currentLine].audioClip.length);
        m_subtitlePanel.SetActive(false);
        subtitleText.gameObject.SetActive(false);
    }
}
