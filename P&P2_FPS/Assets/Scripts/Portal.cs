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
    //public int currentLevel;
    [SerializeField]
    private float m_teleportDelay = .25f;

    public bool m_isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && m_isTeleporting == false)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

}
