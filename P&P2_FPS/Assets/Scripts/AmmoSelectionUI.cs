using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSelectionUI : MonoBehaviour
{
    public List<AmmoSlotUI> m_inventorySpaces = new List<AmmoSlotUI>();
    [SerializeField]
    private List<AmmoSlotUI> m_cartridgeSpaces = new List<AmmoSlotUI>();

    public GameObject m_ammoMenu = null;
    public Transform m_inventoryGrid = null;
    public AmmoSlotUI m_cartridgeSlotLeft = null;
    public AmmoSlotUI m_cartridgeSlotRight = null;
    public GameObject m_ammoSlotPrefab = null;


    private Dictionary<AmmoTypeConfig, int> m_playerAmmoInventory;

    private void OnEnable()
    {
        OpenSelectionUI();
    }
    public void Setup(Dictionary<AmmoTypeConfig, int> inventory)
    {
        m_playerAmmoInventory = inventory;
    }
    public void OpenSelectionUI()
    {
        PopulateInventoryGrid();
        PopulateCartridgeGrid();
    }

    private void PopulateInventoryGrid()
    {
        foreach(AmmoSlotUI ammoSlot in m_inventorySpaces)
        {
            ammoSlot.Clear();
        }

        int i = 0;
        foreach (var ammo in m_playerAmmoInventory)
        {
            if (i >= m_inventorySpaces.Count) break;
            m_inventorySpaces[i].Setup(ammo.Key, ammo.Value, this);
            i++;
        }
    }

    private void PopulateCartridgeGrid()
    {
        foreach(AmmoSlotUI ammoSlot in m_cartridgeSpaces)
        {
            ammoSlot.Clear();
        }

        AmmoCartridge cartridge = AmmoManager.Instance.GetCurrentAmmoCartridge();

        if (cartridge == null) return;

        if(cartridge.m_leftAmmoType)
        {
            m_cartridgeSlotLeft.Setup(cartridge.m_leftAmmoType, 1, this);
        }

        if(cartridge.m_rightAmmoType)
        {
            m_cartridgeSlotRight.Setup(cartridge.m_rightAmmoType, 1, this);
        }
    }
}
