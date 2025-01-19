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
    //public int currentLevel;
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
        
        //currentLevel = currentLevel + 1;

        m_isTeleporting = false;
        m_targetPortal.GetComponent<Portal>().m_isTeleporting = false;
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        PlayerController.player.crouchColliderHeight = data.m_crouchColliderHeight;
        PlayerController.player.crouchCameraHeight = data.m_crouchColliderHeight;
        PlayerController.player.m_baseSpeed = data.m_baseSpeed;
        PlayerController.player.m_baseSprintModifier = data.m_sprintMod;
        PlayerController.player.Health = data.m_HP;
        PlayerController.player.playerHealthOrig = data.m_ogHP;
        PlayerController.player.Speed = data.m_speed;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        PlayerController.player.transform.position = position;
    }

}
