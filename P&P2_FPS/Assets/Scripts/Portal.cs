using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Portal : MonoBehaviour
{
    [FormerlySerializedAs("m_targetPortalPos")]
    [SerializeField]
    private GameObject m_targetPortal = null;

    [SerializeField]
    private float m_teleportDelay = 0.25f;

    public bool m_isTeleporting = false;
    public static int currentLevel = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_isTeleporting)
        {
            
            PlayerController player = other.GetComponent<PlayerController>();
            SaveSystem.SavePlayer(player);
            Debug.Log("Game Saved");

            if (player != null)
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    //PlayerPrefs.SetInt("CurrentLevel", nextSceneIndex);

                    currentLevel = nextSceneIndex;
                    SceneManager.LoadScene(nextSceneIndex);
                    GameManager.Instance.currentLevel = nextSceneIndex;
                }
            }
        }
    }
}
