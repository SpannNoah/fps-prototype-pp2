using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class  BulletDeBuff : MonoBehaviour
{
    [SerializeField] private scriptableDeBuff deBuff;
    [SerializeField]
    [Range(0, 1f)] private float applyChance = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float randomValue = UnityEngine.Random.Range(0, 1f);
            if (randomValue <= applyChance)
            {
                GameManager.Instance.m_playerController.ApplyDeBuff(deBuff);
            }
            else
            {
                Debug.Log("Debuff not applied");
            }
            Destroy(gameObject);
        }
    }
}
