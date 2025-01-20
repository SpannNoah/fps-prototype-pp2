using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private AmmoTypeConfig m_ammoType;
    public float m_speed = 50f;
    public float m_lifetime = 5f;

    public void Init(AmmoTypeConfig ammo)
    {
        m_ammoType = ammo;
        Destroy(gameObject, m_lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * m_speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamage damageable))
        {
            damageable.TakeDamage(m_ammoType.m_baseDamage);
        }

        if(m_ammoType.m_effectScript != null && m_ammoType.m_effectScript is IAmmoEffect effect)
        {
            effect.ApplyAmmoEffect(transform.position, other, m_ammoType);
        }

        Destroy(gameObject);
    }
}
