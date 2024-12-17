using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField]
    private CharacterController m_characterController = null;
    [SerializeField]
    private CharacterController m_controller = null;
    [SerializeField]
    private LayerMask m_ignoreMask = 0;

    [Space]
    [Header("Player Settings")]
    [SerializeField]
    [Range(5, 20)]
    private float m_speed = 10.0f;
    [SerializeField]
    [Range(2, 20)]
    private float m_sprintModifier = 2.0f;
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

    [Space]
    [Header("Buff Settings")]
    [SerializeField] List<BuffSystem> buffList = new List<BuffSystem>();
    [SerializeField] private float m_buffDuration = 0.0f;
    [SerializeField] private float m_maxOverShield = 100.0f;
    [SerializeField] private float m_damageTaken = 0.0f;



    [Space]
    [Header("Shooting Settings")]
    [SerializeField]
    private int m_shootDamage = 25;
    [SerializeField]
    private int m_shootDistance = 50;
    [SerializeField]
    private int m_fireRate = 20;

    [Header("Crouching")]
    [SerializeField] 
    private float crouchSpeed = 3.5f;
    [SerializeField]
    private float crouchMoveSpeed = 5.0f;
    [SerializeField]
    private float crouchYScale = .5f;
    

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
    private Coroutine m_overShieldLerpCoroutine = null;
    private float m_originalHeight = 2.0f;
    private float startYScale = 1.0f;
    private bool isCrouched = false;

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

    private void Awake()
    {
        m_originalHeight = m_characterController.height;
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
    }

    private void Move()
    {
        if (m_controller.isGrounded)
        {
            m_jumpCount = 0;
            m_playerVelocity = Vector3.zero;
        }

        m_moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        m_controller.Move(m_moveDir * m_speed * Time.deltaTime);

        Jump();
        Crouch();
        m_controller.Move(m_playerVelocity * Time.deltaTime);
        m_playerVelocity.y -= m_gravity * Time.deltaTime;

        if ((m_controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_playerVelocity.y -= m_jumpSpeed;
        }

        if (Input.GetButton("Fire1") && !m_isShooting)
        {
            StartCoroutine(ShootingCoroutine());
        }

        // start crouch
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && m_jumpCount < m_jumpMax)
        {
            m_jumpCount++;
            m_playerVelocity.y = m_jumpSpeed;
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            m_speed = crouchMoveSpeed;
            isCrouched = true;
            m_characterController.height /= 2;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            m_speed = m_baseSpeed;
            isCrouched = false;
            m_characterController.height = m_originalHeight;
        }
    }

    private void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isCrouched)
        {
            m_speed = m_baseSpeed * m_sprintModifier;
            m_isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint") && !isCrouched)
        {
            m_speed = m_baseSpeed;
            m_isSprinting = false;
        }
    }

    private IEnumerator ShootingCoroutine()
    {
        m_isShooting = true;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_shootDistance, ~m_ignoreMask))
        {
            IDamage damage;

            if (hit.collider.TryGetComponent<IDamage>(out damage))
            {
                damage.TakeDamage(m_shootDamage);
            }
        }
        yield return new WaitForSeconds(m_fireRate);
        m_isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        if (m_maxOverShield > 0)
        {
            float damageToOvershield = Mathf.Min(amount, m_maxOverShield);
            m_maxOverShield -= damageToOvershield;
            amount -= (int)damageToOvershield;

            StartCoroutine(LerpOverShieldCoroutine());
            //GameManager.Instance.m_playerOverShield.fillAmount = m_maxOverShield / m_maxOverShield;

            if (m_maxOverShield <= 0)
            {
                m_maxOverShield = 0;
                GameManager.Instance.DeactivateOverShieldUI();
            }

            m_health -= amount;

            if (m_health <= 0)
            {
                m_health = 0;
            }

            UpdatePlayerUI();
            StartCoroutine(DamageFlashCoroutine());
            if (m_health <= 0)
            {
                if (m_healthLerpCoroutine != null)
                {
                    StopCoroutine(m_healthLerpCoroutine);
                    m_healthLerpCoroutine = null;
                }
                GameManager.Instance.m_playerHealthBar.fillAmount = 0.0f;
                GameManager.Instance.Lose();
            }
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
        m_overShieldLerpCoroutine = StartCoroutine(LerpOverShieldCoroutine());
        // GameManager.Instance.m_playerOverShield.fillAmount = m_maxOverShield / m_maxOverShield;
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

   public void ApplyBuff(BuffSystem buff)
    {
        if (buff.healthResotration > 0)
        {
            m_health = m_playerHealthOrig;
        }
       else if (buff.healthMult > 0)
        {
            m_health = (int)(m_health * buff.healthMult);
            GameManager.Instance.ActivateOverShieldUI();
        }
        else
        {
            GameManager.Instance.DeactivateOverShieldUI();
        }
        if (buff.speedMult > 0)
        {
            m_speed = m_baseSpeed * buff.speedMult;
        }
        
       
        UpdatePlayerUI();

        StartCoroutine(RemoveBuff(buff));

    }

    private IEnumerator RemoveBuff(BuffSystem buff)
    {
        yield return new WaitForSeconds(buff.duration);
        if (buff.healthMult > 0)
        {
            m_health = (int)(m_health / buff.healthMult);
            GameManager.Instance.DeactivateOverShieldUI();
        }
        if (buff.speedMult > 0)
        {
            m_speed = m_baseSpeed;
        }
        UpdatePlayerUI();
    }

    private IEnumerator LerpOverShieldCoroutine()
    {
        
        float startValue = GameManager.Instance.m_playerOverShield.fillAmount;
        float endValue = m_maxOverShield / m_maxOverShield;

        float elapsedTime = 0;

        while (elapsedTime < m_healthLerpSpeed)
        {
            GameManager.Instance.m_playerOverShield.fillAmount =
                Mathf.Lerp(startValue, endValue, elapsedTime / m_healthLerpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GameManager.Instance.m_playerOverShield.fillAmount = endValue;
    }
}
