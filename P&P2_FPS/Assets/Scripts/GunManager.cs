using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public static List<gunStats> weaponInventory = new List<gunStats>();
    [SerializeField]
    private Transform m_gunHolder = null;
    [SerializeField]
    private Transform m_meleeHolder = null;
    [SerializeField]
    private float m_maxHeat = 100f;
    [SerializeField]
    private float m_cooldownRate = 20.0f;
    [SerializeField]
    private float m_heatIncrement = 20.0f;
    [SerializeField]
    private float m_overheatCooldownTime = 3.0f;

    [Space]
    [Header("Shooting Settings")]
    [SerializeField]
    private int m_shootDamage = 25;
    [SerializeField]
    private float m_shootDistance = 50;
    [SerializeField]
    private float m_fireRate = 20;
    [SerializeField] GameObject gunModel;

    private Gun m_currentGun = null;
    private MeleeWeapon m_currentMelee = null;
    private int weaponInvPos;
    private float m_currentHeat = 0f;
    private bool m_isOverheated = false;

    public void EquipGun(gunStats gunStats, AmmoCartridge ammoCartridge)
    {
        //PlayerData.gunPos[0] = m_gunHolder.transform.position.x;
        //PlayerData.gunPos[0] = m_gunHolder.transform.position.y;
        //PlayerData.gunPos[0] = m_gunHolder.transform.position.z;
        if (m_currentGun)
        {
            Destroy(m_currentGun.gameObject);
        }
        else if(m_currentMelee)
        {
            Destroy(m_currentMelee.gameObject);
        }

        GameObject newGun = Instantiate(gunStats.gunModel, m_gunHolder);
        m_currentGun = newGun.GetComponent<Gun>();
        m_currentGun.m_gunStats = gunStats;
        m_currentGun.m_ammoCartridge = ammoCartridge;
        weaponInventory.Add(m_currentGun.m_gunStats);
    }

    public void EquipMelee(gunStats gunStats)
    {
        //PlayerData.meleePos[0] = m_gunHolder.transform.position.x;
        //PlayerData.meleePos[0] = m_gunHolder.transform.position.y;
        //PlayerData.meleePos[0] = m_gunHolder.transform.position.z;
        if (m_currentGun)
        {
            Destroy(m_currentGun.gameObject);
        }
        else if (m_currentMelee)
        {
            Destroy(m_currentMelee.gameObject);
        }

        GameObject newMelee = Instantiate(gunStats.gunModel, m_meleeHolder);
        m_currentMelee = newMelee.GetComponent<MeleeWeapon>();
        m_currentMelee.m_gunStats = gunStats;
        weaponInventory.Add(m_currentMelee.m_gunStats);
    }
    private void Update()
    {
        if (!m_isOverheated)
        {
            if(Input.GetMouseButton(0)) // Left Mouse Button
            {
                if(m_currentGun)
                {
                    m_currentGun.Fire(true);
                    IncreaseHeat(m_heatIncrement * Time.deltaTime);
                }
                else
                {
                    m_currentMelee?.Attack();
                }
            }
            if(Input.GetMouseButton(1)) // Right Mouse Button
            {
                if(m_currentGun)
                {
                    m_currentGun.Fire(false);
                    IncreaseHeat(m_heatIncrement * Time.deltaTime);
                }
            }
        }
        Cooldown();
        selectedGun();
        reload();
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
        // Destroy the currently equipped weapon
        if (m_currentGun)
        {
            Destroy(m_currentGun.gameObject);
            m_currentGun = null;
        }
        if (m_currentMelee)
        {
            Destroy(m_currentMelee.gameObject);
            m_currentMelee = null;
        }

        // Get the selected weapon stats
        gunStats selectedWeapon = weaponInventory[weaponInvPos];

        // Determine if it's a gun or melee weapon
        if (selectedWeapon.m_isMelee)
        {
            GameObject newMelee = Instantiate(selectedWeapon.gunModel, m_meleeHolder);
            m_currentMelee = newMelee.GetComponent<MeleeWeapon>();
            m_currentMelee.m_gunStats = selectedWeapon;
        }
        else
        {
            GameObject newGun = Instantiate(selectedWeapon.gunModel, m_gunHolder);
            m_currentGun = newGun.GetComponent<Gun>();
            m_currentGun.m_gunStats = selectedWeapon;
            m_currentGun.m_ammoCartridge = AmmoManager.Instance.GetCurrentAmmoCartridge();
        }

        // Update weapon stats
        m_shootDamage = selectedWeapon.shootDamage;
        m_shootDistance = selectedWeapon.shootDist;
        m_fireRate = selectedWeapon.shootRate;
        GameManager.Instance.AmmoCount(selectedWeapon.ammoMax.ToString(), selectedWeapon.ammoCur.ToString());
    }
<<<<<<< HEAD
    public void LoadWeapons()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            weaponInventory = data.weapons;
        }
    }
=======

    private void IncreaseHeat(float amount)
    {
        if (m_isOverheated) return; // Don't increase heat if already overheated

        m_currentHeat += amount;
        //m_currentHeat = Mathf.Clamp(m_currentHeat, 0, m_maxHeat); // Ensure heat stays within range

        Debug.Log($"Heat Increased: {m_currentHeat}");

        if (m_currentHeat >= m_maxHeat)
        {
            Debug.Log("Weapon Overheated!");
            m_isOverheated = true;
            StartCoroutine(CooldownCoroutine());
        }
    }

    private void Cooldown()
    {
        if(!m_isOverheated && m_currentHeat > 0 
            && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            m_currentHeat -= m_cooldownRate * Time.deltaTime;
            Debug.Log("Weapon Cooling Down: " + m_currentHeat);
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(3f);

        m_currentHeat = 0f;
        m_isOverheated = false;
        Debug.Log("Weapon Cooled Down");
    }
>>>>>>> 04bb82452de7b8193a4a7a642953ebb0a7552de2
}
