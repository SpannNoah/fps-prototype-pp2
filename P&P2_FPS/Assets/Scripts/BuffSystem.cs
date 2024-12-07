using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    [Header("Buff Settings")]
    [SerializeField] private float healthMult = 2f;
    [SerializeField] private float speedMult = 2f;
    [SerializeField] private float sprintMult = 5f;
    [SerializeField] private float buffDuration = 6f;

    private PlayerController playerController;
    private bool isBuffActive = false;
    private float origSpeed;
    private float origSprintMod;

    private void Start()
    {
        playerController = GameManager.Instance.m_playerController;
    }

    public void ApplyBuff()
    {
        if (!isBuffActive)
        {
            isBuffActive = true;
            StartCoroutine(BuffCoroutine());
        }
    }

    private IEnumerator BuffCoroutine()
    {
        origSpeed = playerController.Speed;
        origSprintMod = playerController.SprintModifier;

        playerController.SetHealth((int)(playerController.Health * healthMult));
        playerController.SetSpeed(origSpeed * speedMult);
        playerController.SetSprintModifier(origSprintMod * sprintMult);
        Debug.Log("Buff Applied");

        yield return new WaitForSeconds(buffDuration);

        RemoveBuff(origSpeed, origSprintMod);

    }

    private void RemoveBuff(float origSpeed, float origSprintMod)
    {
        playerController.SetHealth(playerController.playerHealthOrig);
        playerController.SetSpeed(origSpeed);
        playerController.SetSprintModifier(origSprintMod);
        isBuffActive = false;
        Debug.Log("Buff Removed");
    }

}
