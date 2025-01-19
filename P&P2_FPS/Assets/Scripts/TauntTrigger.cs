using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntTrigger : MonoBehaviour
{
    public CagedEnemy m_cagedEnemy = null;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_cagedEnemy.PlayTauntAnimation();
        }
    }
}
