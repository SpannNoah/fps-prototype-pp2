using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CagedEnemy : MonoBehaviour
{
    public float m_moveSpeed = 2f;
    public float m_patrolRadius = 3f; // Radius for random movement
    public float m_attackIntervalMin = 5f;
    public float m_attackIntervalMax = 10f;

    private Animator m_animator;
    private Vector3 m_initialPosition;
    private Vector3 m_targetPosition;
    private bool m_isAttacking = false;
    private bool m_isTaunting = false;
    private float m_attackTimer;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_initialPosition = transform.position;
        SetNewTargetPosition(); 
        m_attackTimer = Random.Range(m_attackIntervalMin, m_attackIntervalMax);
    }

    void Update()
    {
        if (m_isAttacking || m_isTaunting) return; 

        m_attackTimer -= Time.deltaTime;

        if (m_attackTimer <= 0)
        {
            AttackCage();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        m_animator.SetBool("isWalking", true);

        
        transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_moveSpeed * Time.deltaTime);

        
        Vector3 direction = (m_targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        
        if (Vector3.Distance(transform.position, m_targetPosition) < 0.2f)
        {
            SetNewTargetPosition(); 
        }
    }

    void SetNewTargetPosition()
    {
        
        Vector3 randomOffset = Random.insideUnitSphere * m_patrolRadius;
        randomOffset.y = 0; 
        m_targetPosition = m_initialPosition + randomOffset;
    }

    void AttackCage()
    {
        if (m_isAttacking) return; 

        m_isAttacking = true;
        m_animator.SetBool("isWalking", false);
        m_animator.SetTrigger("attack1");

        
        float totalAttackTime = 3.0f;
        StartCoroutine(ResetAfterAttack(totalAttackTime));
    }

    IEnumerator ResetAfterAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); 

        m_isAttacking = false;
        m_attackTimer = Random.Range(m_attackIntervalMin, m_attackIntervalMax); 
        SetNewTargetPosition(); 
    }

    public void PlayTauntAnimation()
    {
        if (m_isAttacking || m_isTaunting) return;

        transform.LookAt(GameManager.Instance.m_player.transform);
        m_isTaunting = true;
        m_animator.SetBool("isWalking", false);
        m_animator.SetTrigger("taunt"); 

        StartCoroutine(ResetAfterTaunt(3.5f)); 
    }

    IEnumerator ResetAfterTaunt(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        m_isTaunting = false;
        SetNewTargetPosition();
    }
}
