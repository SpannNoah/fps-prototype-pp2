using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private enum DamageType { moving, stationary}
    [SerializeField] DamageType m_damageType = DamageType.moving;
    [SerializeField] Rigidbody m_rb = null;

    [SerializeField] int m_damageAmount = 0;
    [SerializeField] int m_speed = 0;
    [SerializeField] int m_destroyTime = 0;

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

        damage.TakeDamage(m_damageAmount);

        if(m_damageType == DamageType.moving)
        {
            Destroy(gameObject);
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

}
