using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float m_jumpPadForce = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name}");

        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            Debug.Log($"Player detected: {other.name}");
            playerController.ApplyJumpPadForce(m_jumpPadForce);
        }
        else
        {
            Debug.Log($"PlayerController not found on: {other.name}");
        }
    }
}
