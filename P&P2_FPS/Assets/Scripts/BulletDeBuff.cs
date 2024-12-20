using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class  BulletDeBuff : MonoBehaviour
{
    [SerializeField] private scriptableDeBuff deBuff;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.m_playerController.ApplyDeBuff(deBuff);
            Destroy(gameObject);
        }
    }
}
