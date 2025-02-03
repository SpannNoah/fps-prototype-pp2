using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutsceneManager : MonoBehaviour
{
    public PlayableDirector m_cutscene;
    public PlayerController m_playerController;
    public GameObject m_gameplayCamera;
    public CinemachineVirtualCamera m_cutsceneCamera;
    public GameObject m_blackScreen;
    public List<GameObject> m_hudElementsToDisable = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_playerController.m_isInCutScene = true;
        foreach (GameObject element in m_hudElementsToDisable)
        {
            element.SetActive(false);
        }
        m_cutsceneCamera.Priority = 10;
        m_playerController.enabled = false;
        //m_gameplayCamera.SetActive(false);
        m_cutscene.Play();

        m_cutscene.stopped += OnCutSceneStopped;
    }

    private void LateUpdate()
    {
        if (!m_playerController.m_isInCutScene) return; // Only apply during cutscene

        Transform cameraTransform = m_gameplayCamera.transform;
        cameraTransform.localPosition = new Vector3(0f, 0.728f, 0f);
        cameraTransform.localRotation = Quaternion.identity;

        Debug.Log("LateUpdate - Forcing Camera Position: " + cameraTransform.localPosition);
    }

    private void OnCutSceneStopped(PlayableDirector director)
    {
        m_cutsceneCamera.Priority = 0;

        Transform cameraTransform = m_gameplayCamera.transform;
        cameraTransform.localPosition = new Vector3(0f, 0.728f, 0f);
        cameraTransform.localRotation = Quaternion.identity;

        Debug.Log("Camera Local Position (After Manual Set): " + cameraTransform.localPosition);
        Debug.Log("Camera Local Rotation (After Manual Set): " + cameraTransform.localRotation.eulerAngles);

        // Disable CinemachineBrain temporarily to force Unity to apply this change
        CinemachineBrain brain = m_gameplayCamera.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            Debug.Log("Disabling Cinemachine Brain...");
            brain.enabled = false;
        }

        m_blackScreen.SetActive(false);
        m_gameplayCamera.SetActive(true);

        foreach (GameObject element in m_hudElementsToDisable)
        {
            element.SetActive(true);
        }

        

        StartCoroutine(FadeInHUD());
    }

    private IEnumerator FadeInHUD()
    {
        float duration = 1.5f; // Fade duration
        float elapsedTime = 0f;

        foreach (GameObject element in m_hudElementsToDisable)
        {
            if (element.TryGetComponent(out CanvasGroup canvasGroup))
            {
                element.SetActive(true);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            foreach (GameObject element in m_hudElementsToDisable)
            {
                if (element.TryGetComponent(out CanvasGroup canvasGroup))
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                }
            }

            yield return null;
        }

        // Ensure full visibility at the end
        foreach (GameObject element in m_hudElementsToDisable)
        {
            if (element.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.alpha = 1;
            }
        }
        m_playerController.enabled = true;
        m_playerController.m_isInCutScene = false;
        Debug.Log("Camera Local Position After A few Seconds: " + m_gameplayCamera.transform.localPosition);
        Debug.Log("Camera Local Rotation After a few seconds: " + m_gameplayCamera.transform.localRotation.eulerAngles);
        Destroy(gameObject);
    }
}
