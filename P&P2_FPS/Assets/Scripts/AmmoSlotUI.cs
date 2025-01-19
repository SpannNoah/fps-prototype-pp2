using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmmoSlotUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private TextMeshProUGUI m_ammoNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_ammoCountText = null;
    [SerializeField]
    private GameObject m_ammoImage = null;
    [SerializeField]
    private GameObject m_draggedImagePrefab = null;

    private GameObject m_tempDraggedImage = null;
    private AmmoTypeConfig m_ammoType = null;
    private int m_ammoCount = 0;
    private AmmoSelectionUI m_selectionUI = null;
    private Transform m_originalParent = null;

    public void Setup(AmmoTypeConfig ammoType, int ammoCount, AmmoSelectionUI selectionUI)
    {
        m_ammoType = ammoType;
        m_ammoCount = ammoCount;
        m_selectionUI = selectionUI;

        m_ammoNameText.text = ammoType.m_ammoName;
        m_ammoCountText.text = ammoCount.ToString();
        m_originalParent = m_ammoImage.transform.parent;
    }

    public void Clear()
    {
        m_ammoNameText.text = string.Empty;
        m_ammoCountText.text = string.Empty;
        m_ammoType = null;
        m_ammoCount = 0;
    }

    public void UpdateAmmoCounts()
    {
        foreach(AmmoSlotUI inventorySlot in m_selectionUI.m_inventorySpaces)
        {
            if(inventorySlot.m_ammoType == null)
            {
                continue;
            }

            int ammoCount = AmmoManager.Instance.GetRemaining(inventorySlot.m_ammoType);
            inventorySlot.m_ammoCountText.text = ammoCount.ToString();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_tempDraggedImage = Instantiate(m_draggedImagePrefab, m_selectionUI.transform);
        m_tempDraggedImage.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        m_tempDraggedImage.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_tempDraggedImage != null)
        {
            m_tempDraggedImage.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_tempDraggedImage != null)
        {
            Destroy(m_tempDraggedImage);
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(m_selectionUI.m_cartridgeSlotLeft.GetComponent<RectTransform>(), Input.mousePosition))
        {
            SwapAmmoInCartridge(true);
        }
        else if(RectTransformUtility.RectangleContainsScreenPoint(m_selectionUI.m_cartridgeSlotRight.GetComponent<RectTransform>(), Input.mousePosition))
        {
            SwapAmmoInCartridge(false);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(m_selectionUI.m_inventoryGrid.GetComponent<RectTransform>(), Input.mousePosition))
        {
            MoveAmmoToInventory();
        }
        else
        {
            m_ammoImage.transform.SetParent(m_originalParent);
            m_ammoImage.transform.localPosition = Vector3.zero;
        }
    }

    private void SwapAmmoInCartridge(bool isLeft)
    {
        AmmoSlotUI targetSlot = isLeft ? m_selectionUI.m_cartridgeSlotLeft : m_selectionUI.m_cartridgeSlotRight;

        if (targetSlot.m_ammoType != null)
        {
            // Swap out the ammo
            AmmoTypeConfig previousAmmo = targetSlot.m_ammoType;
            targetSlot.Setup(m_ammoType, 1, m_selectionUI);
            if (isLeft)
            {
                AmmoManager.Instance.SetLeftAmmoType(m_ammoType);
            }
            else
            {
                AmmoManager.Instance.SetRightAmmoType(m_ammoType);
            }

            AmmoManager.Instance.RemoveAmmoFromInventory(m_ammoType, 1);
            AmmoManager.Instance.UpdateCartridge();

            if(AmmoManager.Instance.GetRemaining(m_ammoType) <= 0)
            {
                Clear(); // Clear the previous slot name
            }
            else
            {
                Setup(m_ammoType, AmmoManager.Instance.GetRemaining(m_ammoType), m_selectionUI);
            }

            bool foundSlot = false;
            foreach (AmmoSlotUI inventorySlot in m_selectionUI.m_inventorySpaces)
            {
                if (inventorySlot.m_ammoType == previousAmmo)
                {
                    foundSlot = true;
                    AmmoManager.Instance.AddAmmoToInventory(previousAmmo, 1);
                    UpdateAmmoCounts();
                    break;
                }
            }

            if (!foundSlot)
            {
                foreach (AmmoSlotUI inventorySlot in m_selectionUI.m_inventorySpaces)
                {
                    if (inventorySlot.m_ammoType == null)
                    {
                        inventorySlot.Setup(previousAmmo, 1, m_selectionUI);
                        AmmoManager.Instance.AddAmmoToInventory(previousAmmo, 1);
                        UpdateAmmoCounts();
                        break;
                    }
                }
            }
        }
        else
        {
            targetSlot.Setup(m_ammoType, 1, m_selectionUI);
            if (isLeft)
            {
                AmmoManager.Instance.SetLeftAmmoType(m_ammoType);
            }
            else
            {
                AmmoManager.Instance.SetRightAmmoType(m_ammoType);
            }

            AmmoManager.Instance.RemoveAmmoFromInventory(m_ammoType, 1);
            AmmoManager.Instance.UpdateCartridge();
            if (AmmoManager.Instance.GetRemaining(m_ammoType) <= 0)
            {
                Clear(); // Clear the previous slot name
            }
            else
            {
                Setup(m_ammoType, AmmoManager.Instance.GetRemaining(m_ammoType), m_selectionUI);
            }
        }
    }

    private void MoveAmmoToInventory()
    {
        if (m_ammoType == null) return;

        bool foundSlot = false;
        foreach (AmmoSlotUI inventorySlot in m_selectionUI.m_inventorySpaces)
        {
            if (inventorySlot.m_ammoType == m_ammoType)
            {
                AmmoManager.Instance.AddAmmoToInventory(m_ammoType, 1);
                UpdateAmmoCounts();
                foundSlot = true;
                break;
            }
        }

        if (!foundSlot)
        {
            foreach (AmmoSlotUI inventorySlot in m_selectionUI.m_inventorySpaces)
            {
                if (inventorySlot.m_ammoType == null)
                {
                    AmmoManager.Instance.AddAmmoToInventory(m_ammoType, 1);
                    inventorySlot.Setup(m_ammoType, 1, m_selectionUI);
                    break;
                }
            }
        }

        if (this == m_selectionUI.m_cartridgeSlotLeft)
        {
            AmmoManager.Instance.SetLeftAmmoType(null);
            m_selectionUI.m_cartridgeSlotLeft.Clear();
        }
        else if (this == m_selectionUI.m_cartridgeSlotRight)
        {
            AmmoManager.Instance.SetRightAmmoType(null);
            m_selectionUI.m_cartridgeSlotRight.Clear();
        }

        AmmoManager.Instance.UpdateCartridge();
    }

    
}
