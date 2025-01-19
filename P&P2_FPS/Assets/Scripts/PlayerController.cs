using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField]
    private CharacterController m_characterController = null;
    [SerializeField]
    private LayerMask m_ignoreMask = 0;

    [Header("Collider Settings")]
    private CapsuleCollider playerCollider;
    public float crouchColliderHeight = 1.0f;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    [Header("Camera Settings")]
    public Camera playerCamera; // Reference to the player camera
    public float crouchCameraHeight; // Camera Height when crouched
    private float originalCameraHeight; // Original camera's height

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
    private List<ScriptableBuff> activeBuff = new List<ScriptableBuff>();
    private bool isImmune = false;
    private List<scriptableDeBuff> activeDeBuff = new List<scriptableDeBuff>();
    private Coroutine currentDoTCoroutine;

    [Space]
    [Header("Shooting Settings")]
    [SerializeField]
    private int m_shootDamage = 25;
    [SerializeField]
    private float m_shootDistance = 50;
    [SerializeField]
    private float m_fireRate = 20;
    [SerializeField] GameObject gunModel;
    [SerializeField] List<gunStats> weaponInventory = new List<gunStats>();
    private int weaponInvPos;

    [Header("Crouching")]
    [SerializeField]
    private float crouchSpeed = 3.5f;
    [SerializeField]
    private float crouchMoveSpeed = 5.0f;

    [Header("Keybinds")]
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Other")]
    private Vector3 m_moveDir = Vector3.zero;
    private Vector3 m_playerVelocity = Vector3.zero;
    private int m_jumpCount = 0;
    private int m_playerHealthOrig = 100;
    private bool m_isSprinting = false;
    private bool m_isShooting = false;
    public float m_baseSpeed = 0.0f;
    public float m_baseSprintModifier = 0.0f;

    private Coroutine m_healthLerpCoroutine = null;
    private float m_originalHeight = 2.0f;
    private float startYScale = 1.0f;
    private bool isCrouched = false;

    // Getters
    public int Health { get { return m_health; } }
    public float Speed { get { return m_speed; } }
    public float SprintModifier { get { return m_sprintModifier; } }
    public int playerHealthOrig { get { return m_playerHealthOrig; } }

    // Setters
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

    void Start()
    {
        originalCameraHeight = playerCamera.transform.localPosition.y;
        playerCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = playerCollider.height;
        originalColliderCenter = playerCollider.center;

        m_playerHealthOrig = m_health;
        m_baseSpeed = m_speed;
        m_baseSprintModifier = m_sprintModifier;
        UpdatePlayerUI();
    }

    void Update()
    {
        Move();
        Sprint();
        Crouch();
        selectedGun();
        reload();
    }

    private void Move()
    {
        if (m_characterController.isGrounded)
        {
            m_jumpCount = 0;
            m_playerVelocity = Vector3.zero;
        }

        m_moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        m_characterController.Move(m_moveDir * m_speed * Time.deltaTime);

        Jump();
        Crouch();
        m_characterController.Move(m_playerVelocity * Time.deltaTime);
        m_playerVelocity.y -= m_gravity * Time.deltaTime;

        if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_playerVelocity.y -= m_jumpSpeed;
        }
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

            playerCollider.height = crouchColliderHeight;
            playerCollider.center = new Vector3(playerCollider.center.x, playerCollider.center.y / 2, playerCollider.center.z);

            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, crouchCameraHeight, playerCamera.transform.localPosition.z);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            m_speed = m_baseSpeed;
            isCrouched = false;

            playerCollider.height = originalColliderHeight;
            playerCollider.center = originalColliderCenter;

            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, originalCameraHeight, playerCamera.transform.localPosition.z);
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

    public void TakeDamage(int amount)
    {
        if (isImmune)
        {
            return;
        }
        m_health -= amount;
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

    public void ApplyJumpPadForce(float force)
    {
        m_playerVelocity.y = 0;
        m_playerVelocity.y = force;
    }

    public void ApplyBuff(ScriptableBuff buff)
    {
        if (activeBuff.Contains(buff))
        {
            return;
        }
        activeBuff.Add(buff);
        if (buff.speedBoost > 0)
        {
            m_speed += buff.speedBoost;
        }
        if (buff.HealthRestored > 0)
        {
            m_health += (int)buff.HealthRestored;
            UpdatePlayerUI();
        }
        if (buff.Immunity > 0)
        {
            isImmune = true;
        }
    }

    public void RemoveBuff(ScriptableBuff buff)
    {
        if (activeBuff.Contains(buff))
        {
            activeBuff.Remove(buff);
        }
    }

    public void ApplyDeBuff(scriptableDeBuff debuff)
    {
        if (activeDeBuff.Contains(debuff))
        {
            return;
        }
        activeDeBuff.Add(debuff);
        if (debuff.speedDeBuff > 0)
        {
            m_speed -= debuff.speedDeBuff;
        }
        if (debuff.applyDamageOverTime)
        {
            if (currentDoTCoroutine != null)
            {
                StopCoroutine(currentDoTCoroutine);
            }
            currentDoTCoroutine = StartCoroutine(ApplyDamageOverTimeCoroutine());
        }
        StartCoroutine(RemoveDeBuff(debuff));
    }

    private IEnumerator ApplyDamageOverTimeCoroutine()
    {
        while (true)
        {
            TakeDamage(1);
            UpdatePlayerUI();
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator RemoveDeBuff(scriptableDeBuff debuff)
    {
        yield return new WaitForSeconds(debuff.Duration);
        if (activeDeBuff.Contains(debuff))
        {
            activeDeBuff.Remove(debuff);
        }
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && weaponInventory.Count > 0)
        {
            weaponInventory[weaponInvPos].ammoCur = weaponInventory[weaponInvPos].ammoMax;
            GameManager.Instance.AmmoCount(weaponInventory[weaponInvPos].ammoMax.ToString(), weaponInventory[weaponInvPos].ammoCur.ToString());
        }
    }

    void selectedGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponInvPos < weaponInventory.Count - 1)
        {
            weaponInvPos++;
            changeWeapon();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponInvPos > 0)
        {
            weaponInvPos--;
            changeWeapon();
        }
    }

    void changeWeapon()
    {
        m_shootDamage = weaponInventory[weaponInvPos].shootDamage;
        m_shootDistance = weaponInventory[weaponInvPos].shootDist;
        m_fireRate = weaponInventory[weaponInvPos].shootRate;
        GameManager.Instance.AmmoCount(weaponInventory[weaponInvPos].ammoMax.ToString(), weaponInventory[weaponInvPos].ammoCur.ToString());

        gunModel.GetComponent<MeshFilter>().sharedMesh = weaponInventory[weaponInvPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = weaponInventory[weaponInvPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
}


