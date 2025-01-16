using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    private bool isInrange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && animator.GetBool("isOpen") != true)
        {
            animator.SetTrigger("Open");
            animator.SetBool("isOpen", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", false);
        }
    }
}