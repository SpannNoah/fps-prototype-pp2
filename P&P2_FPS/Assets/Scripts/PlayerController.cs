using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
   Rigidbody rb;

    [Header("Components")]
    [SerializeField]
    private CharacterController m_controller = null;
    [SerializeField]
    private LayerMask m_ignoreMask = 0;

    [Space]
    [Header("Player Settings")]
    [SerializeField] [Range(5, 20)]
    private float m_speed;
    [SerializeField] [Range(2, 20)]
    private float m_sprintModifier;
    [SerializeField]
    private int m_jumpMax = 2;
    [SerializeField]
    private int m_jumpSpeed = 10;
    [SerializeField]
    private int m_gravity = 5;
    [SerializeField]
    private int m_health = 10;
    [SerializeField]
    private float m_healthLerpSpeed = .25f;
    
    [Space][Header("Shooting Settings")]
    [SerializeField]
    private int m_shootDamage = 25;
    [SerializeField]
    private int m_shootDistance = 50;
    [SerializeField]
    private int m_fireRate = 20;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchMoveSpeed;
    public float crouchYScale;
    private float startYScale;
    private bool isCrouched;

    [Header("Keybinds")]
    public KeyCode crouchKey = KeyCode.LeftControl;

    private Vector3 m_moveDir = Vector3.zero;
    private Vector3 m_playerVelocity = Vector3.zero;
    private int m_jumpCount = 0;
    private int m_playerHealthOrig = 100;
    private bool m_isSprinting = false;
    private bool m_isShooting = false;
    public float m_baseSpeed = 0.0f;
    public float m_baseSprintModifier = 0.0f;

    private Coroutine m_healthLerpCoroutine = null;

    // getters
    public int Health { get { return m_health; } }
    public float Speed { get { return m_speed; } }
    public float SprintModifier { get { return m_sprintModifier; } }
    public int playerHealthOrig { get { return m_playerHealthOrig; } }

    // setters
    public void SetSpeed(float v)
    {
        m_speed = m_baseSpeed;
    }

    public void SetSprintModifier(float v)
    {
        m_sprintModifier = m_baseSprintModifier;
    }

    public void SetHealth(int v)
    {
        m_health = m_playerHealthOrig;
    }

    // Start is called before the first frame update
    void Start()
    {

        m_playerHealthOrig = m_health;
        m_baseSpeed = m_speed;
        m_baseSprintModifier = m_sprintModifier;
        UpdatePlayerUI();

        // Sets starting Y scale
        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * m_shootDistance, Color.red);
        Move();
        Sprint();
        Crouch();
    }

    private void Move()
    {
        if(m_controller.isGrounded)
        {
            m_jumpCount = 0;
            m_playerVelocity = Vector3.zero;
        }

        m_moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        m_controller.Move(m_moveDir * m_speed * Time.deltaTime);

        Jump();

        m_controller.Move(m_playerVelocity * Time.deltaTime);
        m_playerVelocity.y -= m_gravity * Time.deltaTime;

        if((m_controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_playerVelocity.y -= m_jumpSpeed;
        }

        if(Input.GetButton("Fire1") && !m_isShooting)
        {
            StartCoroutine(ShootingCoroutine());
        }

        // start crouch
    }

    private void Jump()
    {
        if(Input.GetButtonDown("Jump") && m_jumpCount < m_jumpMax)
        {
            m_jumpCount++;
            m_playerVelocity.y = m_jumpSpeed;
        }
    }

    private void Crouch()
    {
        if(Input.GetKeyDown(crouchKey))
        {
            m_speed = crouchMoveSpeed;
            isCrouched = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            m_speed = m_baseSpeed;
            isCrouched = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

        }
    }

    private void Sprint()
    {
        if(Input.GetButtonDown("Sprint") && !isCrouched)
        {
            m_speed = m_baseSpeed * m_sprintModifier;
            m_isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint") && !isCrouched)
        {
            m_speed = m_baseSpeed;
            m_isSprinting = false;
        }
    }

    private IEnumerator ShootingCoroutine()
    {
        m_isShooting = true;
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_shootDistance, ~m_ignoreMask))
        {
            IDamage damage;

            if(hit.collider.TryGetComponent<IDamage>(out damage))
            {
                damage.TakeDamage(m_shootDamage);
            }
        }
        yield return new WaitForSeconds(m_fireRate);
        m_isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        m_health -= amount;
        UpdatePlayerUI();
        StartCoroutine(DamageFlashCoroutine());
        if(m_health <= 0)
        {
            if(m_healthLerpCoroutine != null)
            {
                StopCoroutine(m_healthLerpCoroutine);
                m_healthLerpCoroutine = null;
            }
            GameManager.Instance.m_playerHealthBar.fillAmount = 0.0f;
            GameManager.Instance.Lose();
        }
    }

    private IEnumerator DamageFlashCoroutine()
    {
        GameManager.Instance.m_damageFlash.SetActive(true);

        yield return new WaitForSeconds(.1f);
        
        GameManager.Instance.m_damageFlash.SetActive(false);
    }

    public void UpdatePlayerUI()
    {
        m_healthLerpCoroutine = StartCoroutine(LerpPlayerHealthCoroutine());
    }
    
    private IEnumerator LerpPlayerHealthCoroutine()
    {
        float startValue = GameManager.Instance.m_playerHealthBar.fillAmount;
        float endValue = m_health / (float)m_playerHealthOrig;

        float elapsedTime = 0;

        while (elapsedTime < m_healthLerpSpeed)
        {
            GameManager.Instance.m_playerHealthBar.fillAmount =
                Mathf.Lerp(startValue, endValue, elapsedTime / m_healthLerpSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.m_playerHealthBar.fillAmount = endValue;
    }
}
