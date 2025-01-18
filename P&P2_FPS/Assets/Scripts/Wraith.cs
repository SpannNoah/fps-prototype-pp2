using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wraith : EnemyAI, IDamage
{
    [SerializeField]
    private int m_phase = 1;

    public void SummonSubBoss(GameObject subBossPrefab)
    {
        GameObject subBoss = Instantiate(subBossPrefab, transform.position, Quaternion.identity);

    }



    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if (m_health <= 75 && m_phase == 1)
        {
            m_phase = 2;
            m_speedTransition = 5;
            m_health = 75;
        }
        else if (m_health <= 50 && m_phase == 2)
        {
            m_phase = 3;
            m_speedTransition = 7;
            m_health = 50;
        }
        else if (m_health <= 25 && m_phase == 3)
        {
            m_phase = 4;
            m_speedTransition = 10;
            m_health = 25;
        }
    }

    public void ChangePhase(int newPhase)
    {
        m_phase = newPhase; // update phase
        if (m_phase == 2)
        {
            // summon two sub-bosses
            SummonSubBoss(GameManager.Instance.golemPrefab); // summon a golem
            SummonSubBoss(GameManager.Instance.giantSpiderPrefab); // summon a giant spider
            BecomeImmune();
        } else if (m_phase == 3)
        {
            // no summons, but becomes more aggressive
            StartAggressivePhase();
        }
        Debug.Log("Phase: " + m_phase);
    }

    private void StartAggressivePhase()
    {
        m_fireRate = 5.0f;
        m_speedTransition = 7;
        damage = 300;
    }

    public override IEnumerator Shoot()
    {
        m_isShooting = true;

        // randomly select one of the three attack animations 
        int attackIndex = Random.Range(1, 4); // randomly select an attack type (1, 2, or 3)
        switch (attackIndex)
        {
            case 1:
                m_animator.SetTrigger("Attack1");
                break;
            case 2:
                m_animator.SetTrigger("Attack2");
                break;
            case 3:
                m_animator.SetTrigger("Attack3");
                break;
        }

        yield return new WaitForSeconds(1.0f / m_fireRate);
        DealDamage();

        yield return new WaitForSeconds(1.0f / m_fireRate);
        m_isShooting = false;

    }
    public void BecomeImmune()
    {
    //  TakeDamage() = false;
    }

    
} 
