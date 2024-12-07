using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    [Header("Buff Settings")]
    [SerializeField] private float healthMult = 2f;
    [SerializeField] private float speedMult = 5f;
    [SerializeField] private float sprintMult = 5f;
    [SerializeField] private float buffDuration = 6f;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GameManager.Instance.m_playerController;
    }

    public void ApplyBuff()
    {
        StartCoroutine(BuffCoroutine());
    }

    private IEnumerator BuffCoroutine()
    {
        playerController.SetHealth((int)(playerController.Health * healthMult));
        playerController.SetSpeed(playerController.Speed * speedMult);
        playerController.SetSprintModifier(playerController.SprintModifier * sprintMult);
        yield return new WaitForSeconds(buffDuration);
        playerController.SetHealth((int)(playerController.Health / healthMult));
        playerController.SetSpeed(playerController.Speed / speedMult);
        playerController.SetSprintModifier(playerController.SprintModifier / sprintMult);
    }

}
