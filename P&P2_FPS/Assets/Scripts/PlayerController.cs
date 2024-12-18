using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField]
    private CharacterController m_characterController = null;
    [SerializeField]
    private LayerMask m_ignoreMask = 0;

    [Space]
    [Header("Collider Settings")]
    private CapsuleCollider playerCollider;
    public float crouchColliderHeight = 1.0f;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    [Space]
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
    private float m_health = 10;
    [SerializeField]
    private float m_healthLerpSpeed = .25f;

    public float m_baseSpeed = 0.0f;
    public float m_baseSprintModifier = 0.0f;

    [Space]
    [Header("Buff Settings")]
    [SerializeField] List<BuffSystem> buffList = new List<BuffSystem>();
    [SerializeField] private float m_buffDuration = 0.0f;
    [SerializeField] private float m_maxOverShield = 10.0f;
    [SerializeField] private float m_damageTaken = 0.0f;

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
    [SerializeField]
    private float m_headShotMultiplier = 2.0f;
    private int weaponInvPos;

    [Header("Weapon Attachments")]
    [SerializeField] GameObject[] m4_Attachments;
    [SerializeField] GameObject[] m1911_Attachments;
    [SerializeField] GameObject[] m107_Attachments;

    [Space]
    [Header("Crouching")]
    [SerializeField]
    private float crouchSpeed = 3.5f;
    [SerializeField]
    private float crouchMoveSpeed = 5.0f;
    [SerializeField]
    private float crouchYScale = .5f;

    [Space]
    [Header("Keybinds")]
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Space]
    [Header("IK Settings")]
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform IKRightHandPos;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform IKLeftHandPos;

    [Header("Stored Weapon POS")]
    [SerializeField] private Transform m4_LeftHandPos;
    [SerializeField] private Transform m4_RightHandPos;
    [Space]

    [SerializeField] private Transform m1911_LeftHandPos;
    [SerializeField] private Transform m1911_RightHandPos;
    [Space]

    [SerializeField] private Transform m107_LeftHandPos;
    [SerializeField] private Transform m107_RightHandPos;

    [Space]
    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource m_audioSource = null;
    [SerializeField]
    private AudioClip[] m_audioJump = null;
    [SerializeField]
    [Range(0, 1)]
    private float m_audioJumpVolume = .5f;
    [SerializeField]
    private AudioClip[] m_audioHurt = null;
    [SerializeField]
    [Range(0, 1)]
    private float m_audioHurtVolume = .5f;
    [SerializeField]
    private AudioClip[] m_audioSteps = null;
    [SerializeField]
    [Range(0, 1)]
    private float m_audioStepsVolume = .5f;
    [SerializeField]
    private float m_audioStepFrequencyWalking = .75f;
    [SerializeField]
    private float m_audioStepFrequencySprinting = .3f;

    private Vector3 m_moveDir = Vector3.zero;
    private Vector3 m_playerVelocity = Vector3.zero;
    private int m_jumpCount = 0;
    private float m_playerHealthOrig = 10;
    public int m_currentHealth;
    private bool m_isSprinting = false;
    private bool m_isShooting = false;

    private Coroutine m_healthLerpCoroutine = null;
    private Coroutine m_overShieldLerpCoroutine = null;
    private float m_originalHeight = 2.0f;
    private float startYScale = 1.0f;
    private bool isCrouched = false;
    private bool m_isPlayingStep = false;

    private float m_OrigSpeed;
    private float m_OrigSprintSpeed;

    // getters
    public float Health { get { return m_health; } }
    public float Speed { get { return m_speed; } }
    public float SprintModifier { get { return m_sprintModifier; } }
    public float playerHealthOrig { get { return m_playerHealthOrig; } }

    // setters
    public void SetSpeed(float v)
    {
        m_speed = m_baseSpeed;
    }

    public void SetSprintModifier(float v)
    {
        m_sprintModifier = m_baseSprintModifier;
    }

    public void SetHealth(float v)
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
        originalCameraHeight = playerCamera.transform.localPosition.y;
        playerCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = playerCollider.height;
        originalColliderCenter = playerCollider.center;

        m_playerHealthOrig = m_health;

        m_baseSpeed = m_speed;
        m_baseSprintModifier = m_sprintModifier;
        m_OrigSpeed = m_speed;
        m_OrigSprintSpeed = m_sprintModifier;
        UpdatePlayerUI();

        // Sets starting Y scale

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * m_shootDistance, Color.red);

        if (!GameManager.Instance.m_isPaused)
        {
            Move();
            selectedGun();
        }
        Sprint();
        Crouch();
    }

    private void Move()
    {
        if (m_characterController.isGrounded)
        {
            if (m_moveDir.magnitude > 0.3f && !m_isPlayingStep)
            {
                StartCoroutine(PlayStepAudioCoroutine());
            }

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

        if (Input.GetButton("Fire1") && !m_isShooting && weaponInventory.Count > 0)
        {
            StartCoroutine(ShootingCoroutine());
        }

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && m_jumpCount < m_jumpMax)
        {
            m_jumpCount++;
            m_playerVelocity.y = m_jumpSpeed;
            m_audioSource.PlayOneShot(m_audioJump[Random.Range(0, m_audioJump.Length)], m_audioJumpVolume);
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

    private IEnumerator ShootingCoroutine()
    {
        m_isShooting = true;

        gunStats currentGun = weaponInventory[weaponInvPos];
        m_audioSource.PlayOneShot(currentGun.shootSound[Random.Range(0, currentGun.shootSound.Length)], currentGun.shootSoundVol);

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_shootDistance, ~m_ignoreMask))
        {
            IDamage damage;

            if (hit.collider.TryGetComponent<IDamage>(out damage))
            {
                // Headshot
                if (hit.collider is SphereCollider)
                {
                    damage.TakeDamage(m_shootDamage * m_headShotMultiplier);
                }
                else // Body Shot
                {
                    damage.TakeDamage(m_shootDamage);
                }

            }

            if (weaponInventory[weaponInvPos].hitEffect != null)
            {
                Instantiate(weaponInventory[weaponInvPos].hitEffect, hit.point, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(m_fireRate);
        m_isShooting = false;
    }

    public void TakeDamage(float amount)
    {
        m_health -= amount;
        UpdatePlayerUI();
        StartCoroutine(DamageFlashCoroutine());
        m_audioSource.PlayOneShot(m_audioHurt[Random.Range(0, m_audioHurt.Length)], m_audioHurtVolume);
        if (m_health <= 0)
            if (m_maxOverShield > 0)
            {
                if (m_healthLerpCoroutine != null)
                {
                    float damageToOVerShield = Mathf.Min(m_maxOverShield - m_damageTaken, amount);
                    m_maxOverShield -= damageToOVerShield;
                    amount -= (int)damageToOVerShield;

                    StartCoroutine(lerpPlayerOverShieldCoroutine());

                    if (m_maxOverShield <= 0)
                    {
                        m_maxOverShield = 0;
                        GameManager.Instance.DeactivateOverShieldUI();
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
                        GameManager.Instance.m_playerHealthBar.fillAmount = 0;
                        GameManager.Instance.Lose();
                    }
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
        m_overShieldLerpCoroutine = StartCoroutine(lerpPlayerOverShieldCoroutine());
    }

    private IEnumerator LerpPlayerHealthCoroutine()
    {
        float startValue = GameManager.Instance.m_playerHealthBar.fillAmount;
        float endValue = m_health / m_playerHealthOrig;

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

    public void getGunStats(gunStats gun)
    {
        weaponInventory.Add(gun);
        weaponInvPos = weaponInventory.Count - 1;

        m_shootDamage = gun.shootDamage;
        m_shootDistance = gun.shootDist;
        m_fireRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        if (gun.name == "M4")
        {

            IKRightHandPos = m4_RightHandPos;
            IKLeftHandPos = m4_LeftHandPos;
            changeIKTarget();

            foreach (var item in m1911_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m107_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m4_Attachments)
            {
                item.SetActive(true);
            }
        }

        if (gun.name == "M1911")
        {
            IKRightHandPos = m1911_RightHandPos;
            IKLeftHandPos = m1911_LeftHandPos;
            changeIKTarget();

            foreach (var item in m4_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m107_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m1911_Attachments)
            {
                item.SetActive(true);
            }
        }


        if (gun.name == "M107")
        {
            IKRightHandPos = m107_RightHandPos;
            IKLeftHandPos = m107_LeftHandPos;
            changeIKTarget();

            foreach (var item in m4_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m1911_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m107_Attachments)
            {
                item.SetActive(true);
            }
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

        gunModel.GetComponent<MeshFilter>().sharedMesh = weaponInventory[weaponInvPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = weaponInventory[weaponInvPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        if (weaponInventory[weaponInvPos].name == "M4")
        {
            foreach (var item in m1911_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m107_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m4_Attachments)
            {
                item.SetActive(true);
            }
            IKRightHandPos = m4_RightHandPos;
            IKLeftHandPos = m4_LeftHandPos;
            changeIKTarget();
        }

        if (weaponInventory[weaponInvPos].name == "M1911")
        {
            foreach (var item in m4_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m107_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m1911_Attachments)
            {
                item.SetActive(true);
            }
            IKRightHandPos = m1911_RightHandPos;
            IKLeftHandPos = m1911_LeftHandPos;
            changeIKTarget();
        }


        if (weaponInventory[weaponInvPos].name == "M107")
        {
            foreach (var item in m4_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m1911_Attachments)
            {
                item.SetActive(false);
            }
            foreach (var item in m107_Attachments)
            {
                item.SetActive(true);
            }
            IKRightHandPos = m107_RightHandPos;
            IKLeftHandPos = m107_LeftHandPos;
            changeIKTarget();
        }
    }

    void changeIKTarget()
    {
        rightHandIK.weight = 1f;
        leftHandIK.weight = 1f;
        leftHandTarget.position = IKLeftHandPos.position;
        leftHandTarget.rotation = IKLeftHandPos.rotation;

        rightHandTarget.position = IKRightHandPos.position;
        rightHandTarget.rotation = IKRightHandPos.rotation;
    }

    private IEnumerator PlayStepAudioCoroutine()
    {
        m_isPlayingStep = true;

        m_audioSource.PlayOneShot(m_audioSteps[Random.Range(0, m_audioSteps.Length)], m_audioStepsVolume);

        if (!m_isSprinting)
        {
            yield return new WaitForSeconds(m_audioStepFrequencyWalking);
        }
        else
        {
            yield return new WaitForSeconds(m_audioStepFrequencySprinting);
        }

        m_isPlayingStep = false;

    }

    private IEnumerator lerpPlayerOverShieldCoroutine()
    {
        float startValue = GameManager.Instance.m_playerOverShield.fillAmount;
        float endValue = m_damageTaken / m_maxOverShield;
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

    public void ApplyBuff(BuffSystem buffSystem)
    {
        if (buffSystem.healthResotration)
        {
            m_health = m_playerHealthOrig;
        }
        else if (buffSystem.isOverShield)
        {
            m_playerHealthOrig = Mathf.RoundToInt(m_health * buffSystem.healthMult);
            m_health = Mathf.Min(m_playerHealthOrig, m_health);
        }
        else
        {
            m_health = Mathf.RoundToInt(m_health * buffSystem.healthMult);
            m_health = Mathf.Min(m_playerHealthOrig, m_health);
        }
        if (buffSystem.speedMult > 0)
        {
            m_speed = m_baseSpeed * buffSystem.speedMult;
            m_sprintModifier = m_baseSprintModifier * buffSystem.speedMult;
        }
        StartCoroutine(RemoveBuff(buffSystem));
    }

    private IEnumerator RemoveBuff(BuffSystem buffSystem)
    {
        yield return new WaitForSeconds(buffSystem.duration);
        m_speed = m_baseSpeed;
        m_sprintModifier = m_baseSprintModifier;
        m_playerHealthOrig = m_health;
    }
}
