using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Portal : MonoBehaviour
{
    [FormerlySerializedAs("m_targetPortalPos")]
    [SerializeField]
    private GameObject m_targetPortal = null;
    public int currentLevel;
    [SerializeField]
    private float m_teleportDelay = .25f;

    public bool m_isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && m_isTeleporting == false)
        {
            m_isTeleporting = true;
            m_targetPortal.GetComponent<Portal>().m_isTeleporting = true;
            StartCoroutine(TeleportCoroutine(other));
        }
    }

    private IEnumerator TeleportCoroutine(Collider player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.gameObject.transform.position = m_targetPortal.transform.position;
        player.gameObject.transform.Rotate(0, 180, 0);
        controller.enabled = true;
        
        yield return new WaitForSeconds(m_teleportDelay);
        
        currentLevel = currentLevel + 1;

        m_isTeleporting = false;
        m_targetPortal.GetComponent<Portal>().m_isTeleporting = false;
    }

}
