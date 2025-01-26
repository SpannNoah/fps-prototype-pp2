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
    public AttackType m_attackType = AttackType.Range;

    [Space][SerializeField]
    public Image m_healthBar = null;
    [SerializeField]
    public Renderer m_model = null;
    [SerializeField]
    public Transform m_shootPos = null;
    [SerializeField]
    public Transform m_headPos = null;
    [SerializeField]
    public NavMeshAgent m_navMeshAgent = null;
    [SerializeField]
    public Animator m_animator = null;

    [SerializeField]
    public List<PowerUp> m_powerUps = new List<PowerUp>();
    [SerializeField]
    public int m_chanceToDropPowerUp = 25;

    [SerializeField]
    public int m_health = 100;
    [SerializeField]
    public int m_faceTargetSpeed = 20;
    [SerializeField]
    public Color m_damageColor = Color.red;
    [SerializeField]
    [Range(0, 500)]
    public int damage;
    [SerializeField]
    public GameObject m_bullet = null;
    [SerializeField]
    public float m_fireRate = 10.0f;
    [SerializeField]
    public int m_fieldOfView = 90;
    [SerializeField]
    public int m_roamDistance = 10;
    [SerializeField]
    public int m_roamTimer = 3;
    [SerializeField]
    public int m_speedTransition = 3;

    public bool m_isShooting = false;
    public bool m_isPlayerInRange = false;
    public Color m_colorOriginal = Color.white;
    public Vector3 m_playerDirection = Vector3.zero;
    public float m_angleToPlayer = 0f;
    public bool m_isRoaming = false;
    public Vector3 m_startingPos = Vector3.zero;
    public float m_originalStoppingDist = 0.0f;
    public Coroutine m_coroutine = null;
    public int m_originalHP = 0;
    public float distanceToPlayer = 0f;

    private bool m_isPlayingDeathAnim = false;
    private Transform playerTransform = null;
    // Start is called before the first frame update
    public virtual void Start()
    {
        m_colorOriginal = m_model.material.color;
        m_startingPos = transform.position;
        m_originalStoppingDist = m_navMeshAgent.stoppingDistance;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameGoal(1);
        }

        m_originalHP = m_health;
        playerTransform = GameObject.FindWithTag("Player").transform;

        UpdateUI();
    }

    private void OnDestroy()
    {
        GameManager.Instance.UpdateGameGoal(-1);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(this.transform.position, playerTransform.position);
            Debug.Log("Distance to Player: " + distanceToPlayer);
        }
        if (m_isPlayingDeathAnim)
        {
            m_navMeshAgent.isStopped = true;
            return;
        }
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

    public IEnumerator RoamCoroutine()
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
                m_navMeshAgent.stoppingDistance = m_originalStoppingDist;
                if (m_navMeshAgent.remainingDistance < m_navMeshAgent.stoppingDistance)
                {
                    FaceTarget();
                }

                if(!m_isShooting)
                {
                    StartCoroutine(Shoot());
                }

                return true;
            }

        }

        m_navMeshAgent.stoppingDistance = 0;
        return false;
    }
    
    public void TakeDamage(int amount, DamageType damageType)
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
            m_isPlayingDeathAnim = true;
            m_animator.SetTrigger("death");
            
            DropRandomPowerUp();
            //Destroy(gameObject); --> Animation State Destroys Game Object Now to Allow for Death Animation to Complete
        }
    }

    private IEnumerator DamageFlashCoroutine()
    {
        m_model.material.color = m_damageColor;
        yield return new WaitForSeconds(.1f);
        m_model.material.color = m_colorOriginal;
    }

    public virtual IEnumerator Shoot()
    {
        m_isShooting = true;

        switch(m_attackType)
        {
            case AttackType.Range:
                m_animator.SetTrigger("Shoot");
                Instantiate(m_bullet, m_shootPos.position, transform.rotation);                
                break;
            case AttackType.Melee:
                if (m_angleToPlayer <= m_fieldOfView && m_isPlayerInRange)
                {
                    //yield return new WaitForSeconds(m_fireRate);
                    m_animator.SetTrigger("attack1");
                    //yield return new WaitForSeconds(0.2f);
                    
                }
                //yield return new WaitForSeconds(m_fireRate);
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

    // Used in enemy animator
    public void DealDamage()
    {
        if (!m_isPlayerInRange) return;
        GameManager.Instance.m_playerController.TakeDamage(damage, DamageType.Basic);
    }
}
