using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField]
    private Renderer m_model = null;
    [SerializeField]
    private Transform m_shootPos = null;
    [SerializeField]
    private Transform m_headPos = null;
    [SerializeField]
    private NavMeshAgent m_navMeshAgent = null;

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

    private bool m_isShooting = false;
    private bool m_isPlayerInRange = false;
    private Color m_colorOriginal = Color.white;
    private Vector3 m_playerDirection = Vector3.zero;
    private float m_angleToPlayer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        m_colorOriginal = m_model.material.color;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameGoal(1);
        }
    }

    // Update is called once per frame
    void Update()
    { 
        m_navMeshAgent.SetDestination(GameManager.Instance.m_player.transform.position);
        if (m_isPlayerInRange && CanSeePlayer())
        {
            
        }
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

                return true;
            }

        }

        return false;
    }
    
    public void TakeDamage(int amount)
    {
        m_health -= amount;
        m_navMeshAgent.SetDestination(GameManager.Instance.m_player.transform.position);
        StartCoroutine(DamageFlashCoroutine());

        if(m_health <= 0)
        {
            GameManager.Instance.UpdateGameGoal(-1);
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

        Instantiate(m_bullet, m_shootPos.position, transform.rotation);
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
        }
    }

    public void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(m_playerDirection.x, 0, m_playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * m_faceTargetSpeed);
    }
}
