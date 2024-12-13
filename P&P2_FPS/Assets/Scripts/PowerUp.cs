using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private BuffSystem buffSystem;

    // Start is called before the first frame update
    void Start()
    {
        buffSystem = FindObjectOfType<BuffSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.m_playerController.ApplyBuff(buffSystem);
            Destroy(gameObject);
        }
    }
}

