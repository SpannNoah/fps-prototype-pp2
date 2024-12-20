using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float m_jumpPadForce = 10f;

    [SerializeField] private LayerMask m_playerLayer;
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
        if (((1 << other.gameObject.layer) & m_playerLayer) != 0)
        {
            PlayerController playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.ApplyJumpPadForce(m_jumpPadForce);
            }
        }
    }
}
