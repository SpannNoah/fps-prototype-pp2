using UnityEngine;

public class pickup : MonoBehaviour
{
    enum pickupType { gun, HP, armor, ammo}

    public bool m_isReward = false;
    [SerializeField] pickupType type;
    [SerializeField] gunStats gun;

    void Start()
    {
        if(type == pickupType.gun)
        {
            gun.ammoCur = gun.ammoMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(type == pickupType.gun)
            {
                GameManager.Instance.m_playerController.getGunStats(gun);
            }
            Destroy(gameObject);
        }
    }
}
