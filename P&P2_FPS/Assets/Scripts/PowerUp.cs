using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] ScriptableBuff buff;

    // Start is called before the first frame update
    void Start()
    {
        //buff = FindObjectOfType<ScriptableBuff>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.m_playerController.ApplyBuff(buff);
            Destroy(gameObject);
        }
    }
}

