using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewAmmoType", menuName ="Ammo/AmmoType")]
public class AmmoTypeConfig : ScriptableObject
{
    public string m_ammoName = string.Empty;
    public int m_baseDamage = 0;
    public bool m_isProjectile = false;
    public GameObject m_projectilePrefab = null;
    public float m_projectileSpeed = 0.0f;
    public float m_range = 100.0f;
    public GameObject m_effectPrefab = null;
    public MonoBehaviour m_effectScript = null;
    

    public void ApplyEffect(Vector3 hitPosition, Collider hitTarget)
    {
        if(m_effectScript is IAmmoEffect effect)
        {
            effect.ApplyAmmoEffect(hitPosition, hitTarget);
        }
    }
}
