using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class FacePlayer : MonoBehaviour
{
    [SerializeField]
    private float m_faceTargetSpeed = 4.0f;
    private Vector3 m_playerDirection = Vector3.zero;
    private void Start()
    {
        GameObject player = FindObjectOfType<PlayerController>().gameObject;
        m_playerDirection = player.transform.position - gameObject.transform.position;
    }
    private void Update()
    {
        FaceTarget();
    }
    
    public void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(m_playerDirection.x, 0, m_playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * m_faceTargetSpeed);
    }
}
