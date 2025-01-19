using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : EnemyAI, IDamage
{
    public override void Start()
    {
        base.Start();
    }

    public override IEnumerator Shoot()
    {
        m_isShooting = true;

        // randomly select one of the three attack animations
        int attackIndex = Random.Range(1, 4); // randomly select an attack type (1, 2, or 3)

        switch (attackIndex) // select the attack animation based on the attack index
        {
            case 1:
                m_animator.SetTrigger("Attack1"); // play the attack animation
                break;
            case 2:
                m_animator.SetTrigger("Attack2"); 
                break;
            case 3:
                m_animator.SetTrigger("Attack3");
                break;
        }

        yield return new WaitForSeconds(1.0f); // wait for the attack animation to finish 
                                               // will need to be fine tuned
        DealDamage(); // deal damage to the player

        yield return new WaitForSeconds(1.0f / m_fireRate); // wait for the fire rate
        m_isShooting = false; // stop shooting

    }
}
