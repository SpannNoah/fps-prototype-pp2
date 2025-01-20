using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    public List<gunStats> weaponInventory = new List<gunStats>();
    [SerializeField] private Transform m_gunHolder = null;
    [SerializeField] private Transform m_meleeHolder = null;

    [Space]
    [Header("Shooting Settings")]
    [SerializeField] private int m_shootDamage = 25;
    [SerializeField] private float m_shootDistance = 50;
    [SerializeField] private float m_fireRate = 20;
    [SerializeField] private GameObject gunModel;

    private Gun m_currentGun = null;
    private MeleeWeapon m_currentMelee = null;
    private int weaponInvPos;

    private void Start()
    {
        LoadWeaponsFromSave();
    }

    private void LoadWeaponsFromSave()
    {
        if (GameSaveManager.Instance.playerData.weaponInventory.Count > 0)
        {
            weaponInventory.Clear();
            foreach (var savedWeapon in GameSaveManager.Instance.playerData.weaponInventory)
            {
                gunStats newWeapon = Resources.Load<gunStats>("Weapons/" + savedWeapon.weaponName);
                if (newWeapon != null)
                {
                    weaponInventory.Add(newWeapon);
                }
            }
            weaponInvPos = GameSaveManager.Instance.playerData.selectedWeaponIndex;
            changeWeapon();
        }
    }

    public void SaveWeaponsToGameData()
    {
        GameSaveManager.Instance.playerData.weaponInventory.Clear();
        foreach (var weapon in weaponInventory)
        {
            WeaponSaveData saveData = new WeaponSaveData
            {
                weaponName = weapon.name,
                ammoCurrent = weapon.ammoCur,
                ammoMax = weapon.ammoMax,
                isMelee = weapon.m_isMelee
            };
            GameSaveManager.Instance.playerData.weaponInventory.Add(saveData);
        }
        GameSaveManager.Instance.playerData.selectedWeaponIndex = weaponInvPos;
        GameSaveManager.Instance.SaveGame();
    }

    public void EquipGun(gunStats gunStats, AmmoCartridge ammoCartridge)
    {
        if (m_currentGun)
        {
            Destroy(m_currentGun.gameObject);
        }
        else if (m_currentMelee)
        {
            Destroy(m_currentMelee.gameObject);
        }

        GameObject newGun = Instantiate(gunStats.gunModel, m_gunHolder);
        m_currentGun = newGun.GetComponent<Gun>();
        m_currentGun.m_gunStats = gunStats;
        m_currentGun.m_ammoCartridge = ammoCartridge;
        weaponInventory.Add(m_currentGun.m_gunStats);

        SaveWeaponsToGameData();
    }

    public void EquipMelee(gunStats gunStats)
    {
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
        if (Input.GetMouseButton(0))
        {
            m_currentGun?.Fire(true); // Fire Left Ammo
            m_currentMelee?.Attack();
        }
        if (Input.GetMouseButton(1))
        {
            m_currentGun?.Fire(false); // Fire Right Ammo
        }

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

        SaveWeaponsToGameData();
    }

    private void OnApplicationQuit()
    {
        SaveWeaponsToGameData();
    }
}
