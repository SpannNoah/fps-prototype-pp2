using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour, IDamage
{
    public enum AttackType
    {
        Range,
        Melee
    }

    [SerializeField]
    private AttackType m_attackType = AttackType.Range;

    [Space][SerializeField]
    private Image m_healthBar = null;
    [SerializeField]
    private Renderer m_model = null;
    [SerializeField]
    private Transform m_shootPos = null;
    [SerializeField]
    private Transform m_headPos = null;
    [SerializeField]
    private NavMeshAgent m_navMeshAgent = null;
    [SerializeField]
    private Animator m_animator = null;

    [SerializeField]
    private List<PowerUp> m_powerUps = new List<PowerUp>();
    [SerializeField]
    private int m_chanceToDropPowerUp = 25;

    [SerializeField]
    private int m_health = 100;
    [SerializeField]
    private int m_faceTargetSpeed = 20;
    [SerializeField]
    private Color m_damageColor = Color.red;
    [SerializeField]
    GameObject m_bullet = null;
    [SerializeField]
    float m_fireRate = 10.0f;
    [SerializeField]
    private int m_fieldOfView = 90;
    [SerializeField]
    private int m_roamDistance = 10;
    [SerializeField]
    private int m_roamTimer = 3;
    [SerializeField]
    private int m_speedTransition = 3;

    private bool m_isShooting = false;
    private bool m_isPlayerInRange = false;
    private Color m_colorOriginal = Color.white;
    private Vector3 m_playerDirection = Vector3.zero;
    private float m_angleToPlayer = 0f;
    private bool m_isRoaming = false;
    private Vector3 m_startingPos = Vector3.zero;
    private float m_originalStoppingDist = 0.0f;
    private Coroutine m_coroutine = null;
    private int m_originalHP = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_colorOriginal = m_model.material.color;
        m_startingPos = transform.position;
        m_originalStoppingDist = m_navMeshAgent.stoppingDistance;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameGoal(1);
        }

        m_originalHP = m_health;
        
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_navMeshAgent.isActiveAndEnabled)
        {
            float agentSpeed = m_navMeshAgent.velocity.normalized.magnitude;
            float animSpeed = m_animator.GetFloat("Speed");


            m_animator.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * m_speedTransition));

            m_navMeshAgent.SetDestination(GameManager.Instance.m_player.transform.position);
            if (m_isPlayerInRange && !CanSeePlayer())
            {
                if (!m_isRoaming && m_navMeshAgent.remainingDistance < .01f)
                {
                    m_coroutine = StartCoroutine(RoamCoroutine());
                }
            }
            else if (!m_isPlayerInRange)
            {
                if (!m_isRoaming && m_navMeshAgent.remainingDistance < .01f)
                {
                    m_coroutine = StartCoroutine(RoamCoroutine());
                }
            }
        }
    }

    private IEnumerator RoamCoroutine()
    {
        m_isRoaming = true;
        yield return new WaitForSeconds(m_roamTimer);
        
        m_navMeshAgent.stoppingDistance = 0;
        Vector3 randPos = Random.insideUnitSphere * m_roamDistance;

        randPos += m_startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, m_roamDistance, 1);
        m_navMeshAgent.SetDestination(hit.position);
        
        m_isRoaming = false;
    }

    public bool CanSeePlayer()
    {
        m_playerDirection = GameManager.Instance.m_player.transform.position - m_headPos.position;
        m_angleToPlayer = Vector3.Angle(m_playerDirection, transform.forward);
        
        
        Debug.DrawRay(m_headPos.position, m_playerDirection);
       

        RaycastHit hit;
        if(Physics.Raycast(m_headPos.position, m_playerDirection, out hit))
        {
            if(hit.collider.CompareTag("Player") && m_angleToPlayer <= m_fieldOfView)
            {

                if(m_navMeshAgent.remainingDistance < m_navMeshAgent.stoppingDistance)
                {
                    FaceTarget();
                }

                if(!m_isShooting)
                {
                    StartCoroutine(Shoot());
                }

                m_navMeshAgent.stoppingDistance = m_originalStoppingDist;
                return true;
            }

        }

        m_navMeshAgent.stoppingDistance = 0;
        return false;
    }
    
    public void TakeDamage(int amount)
    {
        m_health -= amount;

        if(m_navMeshAgent.isActiveAndEnabled)
        {
            m_navMeshAgent.SetDestination(GameManager.Instance.m_player.transform.position);
        }
        if(m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
        }
        m_isRoaming = false;
        
        StartCoroutine(DamageFlashCoroutine());
        UpdateUI();

        if(m_health <= 0)
        {
            GameManager.Instance.UpdateGameGoal(-1);
            DropRandomPowerUp();
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlashCoroutine()
    {
        m_model.material.color = m_damageColor;
        yield return new WaitForSeconds(.1f);
        m_model.material.color = m_colorOriginal;
    }

    private IEnumerator Shoot()
    {
        m_isShooting = true;

        switch(m_attackType)
        {
            case AttackType.Range:
                m_animator.SetTrigger("Shoot");
                Instantiate(m_bullet, m_shootPos.position, transform.rotation);                
                break;
            case AttackType.Melee:
                m_animator.SetTrigger("attack1");
                break;
        }
        
        yield return new WaitForSeconds(m_fireRate);

        m_isShooting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInRange = false;
            m_navMeshAgent.stoppingDistance = 0;
        }
    }

    public void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(m_playerDirection.x, 0, m_playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * m_faceTargetSpeed);
    }

    public void UpdateUI()
    {
        if(m_healthBar != null)
        {
            m_healthBar.fillAmount = m_health / (float)m_originalHP;
        }
        
    }

    public void DropRandomPowerUp()
    {
        print("Dropping Powerup");
        int rand = Random.Range(0, 100);

        if(m_chanceToDropPowerUp <= rand)
        {
            Instantiate(m_powerUps[Random.Range(0, m_powerUps.Count)].gameObject, gameObject.transform.position, quaternion.identity);
        }
    }
}
