using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffect : MonoBehaviour, IAmmoEffect
{
    public int m_chainCount = 3;
    public int m_chainDamage = 1;
    public float m_chainRadius = 10f;
    public GameObject m_lightningEffectPrefab;
    public LayerMask m_enemyLayerMask;

    public void ApplyAmmoEffect(Vector3 hitPosition, Collider hitTarget, AmmoTypeConfig ammoType)
    {
        HashSet<Collider> affectedTargets = new HashSet<Collider>(); // Hash set has O(1) lookup
        

        ChainLightning(hitPosition, hitTarget, m_chainCount, affectedTargets, ammoType);
    }

    // Recursive Method that chains until remaining chains = 0
    private void ChainLightning(Vector3 position, Collider currentTarget, int remainingChains, HashSet<Collider> affectedTargets, AmmoTypeConfig ammoType)
    {
        if (remainingChains <= 0) return;

        if (currentTarget != null)
        {
            affectedTargets.Add(currentTarget);
            if (m_lightningEffectPrefab)
            {
                Instantiate(m_lightningEffectPrefab, position, Quaternion.identity);
            }

            if (ammoType.m_currentUpgrade.m_upgradeEffect is IAmmoEffect effect)
            {
                effect.ApplyAmmoEffect(position, currentTarget, ammoType);
        }
        }

        Collider[] nearbyEnemies = Physics.OverlapSphere(position, m_chainRadius, m_enemyLayerMask);
        Collider nextTarget = null;
        float minDistance = float.MaxValue;

        // Find closest enemy
        foreach (var enemy in nearbyEnemies)
        {
            if (affectedTargets.Contains(enemy)) continue;
            
            if(enemy.TryGetComponent(out IDamage damage))
            {
                damage.TakeDamage(m_chainDamage);
            }
            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nextTarget = enemy;
            }
        }

        if (nextTarget != null)
        {
            ChainLightning(nextTarget.transform.position, nextTarget, remainingChains - 1, affectedTargets, ammoType); 
        }
    }
}
