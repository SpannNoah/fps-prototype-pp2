using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_gunHolder = null;

    private Gun m_currentGun = null;

    public void EquipGun(gunStats gunStats, AmmoCartridge ammoCartridge)
    {
        if (m_currentGun) Destroy(m_currentGun.gameObject);

        GameObject newGun = Instantiate(gunStats.gunModel, m_gunHolder);
        m_currentGun = newGun.GetComponent<Gun>();
        m_currentGun.m_gunStats = gunStats;
        m_currentGun.m_ammoCartridge = ammoCartridge;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            m_currentGun?.Fire(true); // Fire Left Ammo
        }
        if(Input.GetMouseButton(1))
        {
            m_currentGun?.Fire(false); // Fire Right Ammo
        }
    }
}
