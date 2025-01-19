using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public gunStats m_gunStats = null;
    [SerializeField] private Transform m_attackPoint = null;
    [SerializeField] private LayerMask m_ignoreMask;
    [SerializeField] private Animator m_animator = null;

    private float m_nextAttackTime = 0;
    private bool m_canAttack = true;
    // Start is called before the first frame update
    public void Attack()
    {
        if (Time.time < m_nextAttackTime) return;
        m_nextAttackTime = Time.time + (1f / m_gunStats.shootRate);

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        m_canAttack = false;
        if (m_animator)
        {
            m_animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(.2f);

        Collider[] hitEnemies = Physics.OverlapSphere(m_attackPoint.position, m_gunStats.shootDist, ~m_ignoreMask);
        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out IDamage damageable))
            {
                damageable.TakeDamage(m_gunStats.shootDamage);
            }
        }
        m_canAttack = true;

    }

    private void OnDrawGizmosSelected()
    {
        if (m_attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_attackPoint.position, m_gunStats.shootDist);
    }
}
