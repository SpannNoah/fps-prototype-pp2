using UnityEngine;

public class pickup : MonoBehaviour
{
    enum pickupType { gun, HP, armor, ammo, melee}

    public bool m_isReward = false;
    [SerializeField] pickupType m_type;
    [SerializeField] gunStats m_gunStats;
    [SerializeField] AmmoCartridge m_ammoCartridge = null;
    void Start()
    {
        if(m_type == pickupType.gun)
        {
            m_gunStats.ammoCur = m_gunStats.ammoMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (m_type == pickupType.gun)
            {
                if (other.TryGetComponent(out GunManager gunManager))
                {
                    gunManager.EquipGun(m_gunStats, m_ammoCartridge);
                    AmmoManager.Instance.SetCurrentCartridge(m_ammoCartridge);
                    Destroy(gameObject);
                }
            }
            else if (m_type == pickupType.melee)
            {
                if (other.TryGetComponent(out GunManager gunManager))
                {
                    gunManager.EquipMelee(m_gunStats);
                    Destroy(gameObject);
                }
            }
        }
    }
}
