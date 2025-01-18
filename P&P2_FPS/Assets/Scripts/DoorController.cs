using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    private bool isInrange = false;

    void start()
    {
        animator.SetBool("isOpen", false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Open");
            animator.SetBool("isOpen", true);
            Debug.Log("Door Open");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Closed");
            animator.SetBool("isOpen", false);
            Debug.Log("Door Closed");
        }
    }
}