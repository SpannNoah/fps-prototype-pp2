using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCartridge : MonoBehaviour
{
    public AmmoTypeConfig m_rightAmmoType = null;
    public AmmoTypeConfig m_leftAmmoType = null;
    public int m_rightAmmoCount = 0;
    public int m_leftAmmoCount = 0;

    private int m_currentRightAmmo = 0;
    private int m_currentLeftAmmo = 0;

    private void Start()
    {
        m_currentRightAmmo = m_rightAmmoCount;
        m_currentLeftAmmo = m_leftAmmoCount;
    }
    public void AssignAmmo(bool isLeftSlot, AmmoTypeConfig ammoType, int count)
    {
        if(isLeftSlot)
        {
            m_leftAmmoType = ammoType;
        }
        else
        {
            m_rightAmmoType = ammoType;
        }
    }

    public bool ConsumeAmmo(bool isLeftSlot)
    {
        if (isLeftSlot && m_leftAmmoCount > 0)
        {
            m_currentLeftAmmo--;
            return true;
        }
        else if (!isLeftSlot && m_rightAmmoCount > 0)
        {
            m_currentRightAmmo--;
            return true;
        }
        return false;
    }
}

