using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private AmmoSelectionUI m_ammoSelectionUI = null;

    private AmmoTypeConfig m_currentLeftAmmo = null;
    private AmmoTypeConfig m_currentRightAmmo = null;
    private AmmoCartridge m_currentCartridge = null;

    public List<AmmoTypeConfig> m_unlockedAmmoTypes = new List<AmmoTypeConfig>();
    private Dictionary<AmmoTypeConfig, int> m_ammoInventory = new Dictionary<AmmoTypeConfig, int>();
    public static AmmoManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        m_ammoSelectionUI.Setup(m_ammoInventory);
    }

    private void Start()
    {
        foreach(AmmoTypeConfig ammo in m_unlockedAmmoTypes)
        {
            AddAmmoToInventory(ammo, 1);
        }
    }


    public void AddAmmoToInventory(AmmoTypeConfig ammoType, int amount)
    {
        if(!m_ammoInventory.ContainsKey(ammoType))
        {
            m_ammoInventory[ammoType] = amount;
        }
        else
        {
            m_ammoInventory[ammoType] += amount;
        }
    }

    public void RemoveAmmoFromInventory(AmmoTypeConfig ammoType, int amount)
    {
        if (!m_ammoInventory.ContainsKey(ammoType)) return;

        m_ammoInventory[ammoType] -= amount;

        if (m_ammoInventory[ammoType] <= 0)
        {
            m_ammoInventory.Remove(ammoType);
        }

    }

    public void SetRightAmmoType(AmmoTypeConfig rightAmmo)
    {
        m_currentRightAmmo = rightAmmo;
    }

    public void SetLeftAmmoType(AmmoTypeConfig leftAmmo)
    {
        m_currentLeftAmmo = leftAmmo;
    }

    public AmmoTypeConfig GetLeftAmmoType()
    {
        return m_currentLeftAmmo ? m_currentLeftAmmo : null;
    }

    public AmmoTypeConfig GetRightAmmoType()
    {
        return m_currentRightAmmo ? m_currentRightAmmo : null;
    }

    public void ClearAmmo(bool isLeft)
    {
        if(isLeft)
        {
            m_currentLeftAmmo = null;
        }
        else
        {
            m_currentRightAmmo = null;
        }

        
    }

    public int GetRemaining(AmmoTypeConfig ammoType)
    {
        if (!m_ammoInventory.ContainsKey(ammoType))
        {
            return 0;
        }
        else
        {
            return m_ammoInventory[ammoType];
        }
    }

    public AmmoCartridge GetCurrentAmmoCartridge()
    {
        return m_currentCartridge;
    }

    public void SetCurrentCartridge(AmmoCartridge ammoCartridge)
    {
        m_currentCartridge = Instantiate(ammoCartridge);

        m_currentLeftAmmo = m_currentCartridge.m_leftAmmoType;
        m_currentRightAmmo = m_currentCartridge.m_rightAmmoType;
    }

    public void UpdateCartridge()
    {
        if (m_currentCartridge == null) return;

        m_currentCartridge.m_rightAmmoType = m_currentRightAmmo;
        m_currentCartridge.m_leftAmmoType = m_currentLeftAmmo;
    }

    public List<AmmoTypeConfig> GetActiveAmmoTypes()
    {
        List<AmmoTypeConfig> activeAmmoTypes = new List<AmmoTypeConfig>();

        activeAmmoTypes.Add(m_currentLeftAmmo);
        activeAmmoTypes.Add(m_currentRightAmmo);

        return activeAmmoTypes;
    }


}
