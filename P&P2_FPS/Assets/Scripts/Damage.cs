using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private enum DamageType { moving, stationary}
    [SerializeField] DamageType m_damageType = DamageType.moving;
    [SerializeField] Rigidbody m_rb = null;

    [SerializeField] int m_damageAmount = 0;
    [SerializeField] int m_speed = 0;
    [SerializeField] int m_destroyTime = 0;
    [SerializeField] private bool m_isDamageOverTime = false;

    private Coroutine m_damageOverTimeCoroutine = null;
    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage damage = other.GetComponent<IDamage>();

        if(damage == null)
        {
            return;
        }

        if(m_isDamageOverTime)
        {
            m_damageOverTimeCoroutine = StartCoroutine(DamageOverTimeCoroutine(damage));
        }
        else
        {
            damage.TakeDamage(m_damageAmount);
        }

        if(m_damageType == DamageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_damageOverTimeCoroutine != null)
        {
            StopCoroutine(m_damageOverTimeCoroutine);
            m_damageOverTimeCoroutine = null;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if(m_damageType == DamageType.moving)
        {
            m_rb.velocity = transform.forward * m_speed;
            Destroy(gameObject, m_destroyTime);
        }
    }

    private IEnumerator DamageOverTimeCoroutine(IDamage dmg)
    {
        while (true)
        {
            dmg.TakeDamage(m_damageAmount);
            yield return new WaitForSeconds(1.0f);
        }
    }

}
