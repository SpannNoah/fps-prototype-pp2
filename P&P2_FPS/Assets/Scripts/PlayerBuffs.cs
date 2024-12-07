using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [Header("Player Buffs")]
    [SerializeField] private float overShieldDuration = 5f;
    [SerializeField] private float speedBoostMultiplier = 2f;
    [SerializeField] private float sprintModfier = .5f;

    private PlayerController playerController = null;
    private bool hasOvershield = false;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEvent(Collider other)
    {
        if (other.CompareTag("OverShield"))
        {
            ActivateOversheild();
        }
        else if (other.CompareTag("SpeedBoost"))
        {
            ActivateSpeedBoost();
        }
    }

    private void ActivateOversheild()
    {
        if (!hasOvershield)
        {
            hasOvershield = true;
            playerController.SetHealth(playerController.Health + 50);
            StartCoroutine(OverShieldCoroutine());
            Debug.Log("Overshield Activated");
        }
    }

    private IEnumerator OverShieldCoroutine()
    {
        yield return new WaitForSeconds(overShieldDuration);
        hasOvershield = false;
        Debug.Log("Overshield Deactivated");
    }

    private void ActivateSpeedBoost()
    {
        playerController.SetSpeed(playerController.Speed * speedBoostMultiplier);
        playerController.SetSprintModifier(playerController.SprintModifier * sprintModfier);
        StartCoroutine(SpeedBoostCoroutine());
        Debug.Log("Speed Boost Activated");
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        yield return new WaitForSeconds(overShieldDuration);
        playerController.SetSpeed(playerController.Speed / speedBoostMultiplier);
        playerController.SetSprintModifier(playerController.SprintModifier / sprintModfier);
        Debug.Log("Speed Boost Deactivated");
    }



}
