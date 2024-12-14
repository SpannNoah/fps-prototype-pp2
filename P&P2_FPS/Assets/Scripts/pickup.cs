using UnityEngine;

public class pickup : MonoBehaviour
{
    enum pickupType { gun, HP, armor, ammo}
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
            GameManager.Instance.m_playerController.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
