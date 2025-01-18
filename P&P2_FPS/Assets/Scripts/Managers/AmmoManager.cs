using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    private AmmoTypeConfig m_currentLeftAmmo = null;
    private AmmoTypeConfig m_currentRightAmmo = null;

    private List<AmmoTypeConfig> m_unlockedAmmoTypes = new List<AmmoTypeConfig>();
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
    }

    public void SetAmmoTypes(AmmoTypeConfig rightAmmo, AmmoTypeConfig leftAmmo)
    {
        m_currentLeftAmmo = leftAmmo;
        m_currentRightAmmo = rightAmmo;
    }

    public List<AmmoTypeConfig> GetActiveAmmoTypes()
    {
        List<AmmoTypeConfig> activeAmmoTypes = new List<AmmoTypeConfig>();

        activeAmmoTypes.Add(m_currentLeftAmmo);
        activeAmmoTypes.Add(m_currentRightAmmo);

        return activeAmmoTypes;
    }


}
