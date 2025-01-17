using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public gunStats m_gunStats = null;
    public Transform m_firePoint = null;
    public LayerMask m_ignoreMask;
    public AmmoCartridge m_ammoCartridge = null;

    private float m_nextFiretime = 0;

    public void Fire(bool isLeft)
    {
        if (Time.time < m_nextFiretime) return;
        m_nextFiretime = Time.time + (1f / m_gunStats.shootRate);


        if (!m_ammoCartridge.ConsumeAmmo(isLeft)) return; // don't fire if no ammo left
        AmmoTypeConfig selectedAmmo = isLeft ? m_ammoCartridge.m_leftAmmoType : m_ammoCartridge.m_rightAmmoType;

        if(m_gunStats.m_isProjectile)
        {
            FireProjectile(selectedAmmo);
        }
        else
        {
            FireHitScan(selectedAmmo);
        }
    }

    private void FireProjectile(AmmoTypeConfig ammoType)
    {
        GameObject projectile = Instantiate(m_gunStats.m_projectilePrefab, m_firePoint.transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Init(ammoType);
    }

    private void FireHitScan(AmmoTypeConfig ammoType)
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_gunStats.shootDist, ~m_ignoreMask))
        {
            IDamage damage;

            if (hit.collider.TryGetComponent<IDamage>(out damage))
            {
                damage.TakeDamage(m_gunStats.shootDamage);
            }

            if(ammoType.m_effectScript != null && ammoType.m_effectScript is IAmmoEffect effect)
            {
                effect.ApplyAmmoEffect(hit.point, hit.collider);
            }
        }
    }
}
