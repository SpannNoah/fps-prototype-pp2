using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeBuff : MonoBehaviour
{
    [SerializeField] private scriptableDeBuff debuff;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.m_playerController.ApplyDeBuff(debuff);
            Destroy(gameObject);
        }
    }
}