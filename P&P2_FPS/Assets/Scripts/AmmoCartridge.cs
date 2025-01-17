using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCartridge : MonoBehaviour
{
    public AmmoTypeConfig m_rightAmmoType = null;
    public AmmoTypeConfig m_leftAmmoType = null;
    public int m_rightAmmoCount = 0;
    public int m_leftAmmoCount = 0;

    public void AssignAmmo(bool isLeftSlot, AmmoTypeConfig ammoType, int count)
    {
        if(isLeftSlot)
        {
            m_leftAmmoType = ammoType;
            m_leftAmmoCount = count;
        }
        else
        {
            m_rightAmmoType = ammoType;
            m_rightAmmoCount = count;
        }
    }

    public bool ConsumeAmmo(bool isLeftSlot)
    {
        if (isLeftSlot && m_leftAmmoCount > 0)
        {
            m_leftAmmoCount--;
            return true;
        }
        else if (!isLeftSlot && m_rightAmmoCount > 0)
        {
            m_rightAmmoCount--;
            return true;
        }
        return false;
    }
}

