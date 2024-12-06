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

    private bool m_isShooting = false;
    private bool m_isPlayerInRange = false;
    private Color m_colorOriginal = Color.white;
    private Vector3 m_playerDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_colorOriginal = m_model.material.color;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameGoal(1);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.UpdateGameGoal(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPlayerInRange)
        {
            m_playerDirection = GameManager.Instance.m_player.transform.position - gameObject.transform.position;
            m_navMeshAgent.SetDestination(GameManager.Instance.m_player.transform.position);

            if(m_navMeshAgent.remainingDistance < m_navMeshAgent.stoppingDistance)
            {
                FaceTarget();
            }

            if(!m_isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    public void TakeDamage(int amount)
    {
        m_health -= amount;
        StartCoroutine(DamageFlashCoroutine());

        if(m_health <= 0)
        {
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
        Quaternion rot = Quaternion.LookRotation(m_playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * m_faceTargetSpeed);
    }
}
