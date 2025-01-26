using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [System.Serializable]
    public class AmmoRecoil
    {
        public DamageType damageType;
        public float verticalKick = 1.0f;
        public float horizontalKick = 0.5f;
        public float gunKickBack = 0.1f;  
    }

    public gunStats m_gunStats = null;
    public Transform m_firePoint = null;
    public Transform m_gunTransform = null;
    public LayerMask m_ignoreMask;
    public AmmoCartridge m_ammoCartridge = null;

    [Header("Recoil")]
    public float m_recoilDuration = .1f;
    public Transform m_cameraTransform = null;
    public List<AmmoRecoil> m_recoilSettings = new List<AmmoRecoil>();

    private float m_nextFiretime = 0;
    private Vector3 m_originalGunPosition = Vector3.zero;
    private Quaternion m_originalGunRotation;

    private void Start()
    {
        m_cameraTransform = GameManager.Instance.m_playerController.GetCamera().transform;
        m_originalGunRotation = m_gunTransform.localRotation;
        m_originalGunPosition = m_gunTransform.localPosition;
    }

    public void Fire(bool isLeft)
    {
        if (Time.time < m_nextFiretime) return;
        m_nextFiretime = Time.time + (1f / m_gunStats.shootRate);


        //if (!m_ammoCartridge.ConsumeAmmo(isLeft)) return; // don't fire if no ammo left
        AmmoTypeConfig selectedAmmo = isLeft ? AmmoManager.Instance.GetLeftAmmoType() : AmmoManager.Instance.GetRightAmmoType();


        if(AudioManger.instance != null && selectedAmmo != null)
        {
            ApplyRecoil(selectedAmmo.m_damageType);
            AudioManger.instance.PlayAmmoSFX(selectedAmmo.m_damageType, isLeft);
        }

        if(m_gunStats.m_isProjectile)
        {
            FireProjectile(selectedAmmo);
        }
        else
        {
            FireHitScan(selectedAmmo);
        }
    }

    public void FireDouble()
    {
        if (Time.time < m_nextFiretime) return;
        m_nextFiretime = Time.time + (1f / m_gunStats.shootRate);

        AmmoTypeConfig rightAmmo = AmmoManager.Instance.GetRightAmmoType();
        AmmoTypeConfig leftAmmo = AmmoManager.Instance.GetLeftAmmoType();

        if (rightAmmo == null || leftAmmo == null) return;

        ApplyRecoil(rightAmmo.m_damageType);
        ApplyRecoil(leftAmmo.m_damageType);

        AudioManger.instance.PlayAmmoSFX(rightAmmo.m_damageType, false);
        AudioManger.instance.PlayAmmoSFX(leftAmmo.m_damageType, true);

        FireHitScan(rightAmmo);
        FireHitScan(leftAmmo);
    }

    private void FireProjectile(AmmoTypeConfig ammoType)
    {
        GameObject projectile = Instantiate(m_gunStats.m_projectilePrefab, m_firePoint.transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Init(ammoType);
    }

    private void FireHitScan(AmmoTypeConfig ammoType)
    {
        if (ammoType == null) return;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_gunStats.shootDist, ~m_ignoreMask))
        {
            IDamage damage;

            if (hit.collider.TryGetComponent<IDamage>(out damage))
            {
                damage.TakeDamage(m_gunStats.shootDamage, ammoType.m_damageType);
            }

            if(ammoType.m_effectScript != null && ammoType.m_effectScript is IAmmoEffect effect)
            {
                effect.ApplyAmmoEffect(hit.point, hit.collider, ammoType);
            }
        }
    }

    private void ApplyRecoil(DamageType damageType)
    {
        AmmoRecoil recoil = m_recoilSettings.Find(r => r.damageType == damageType);
        if (recoil == null) return;

        StartCoroutine(CameraRecoil(recoil.verticalKick, recoil.horizontalKick));
        StartCoroutine(GunKickBack(recoil.gunKickBack));
    }

    private IEnumerator CameraRecoil(float verticalAmount, float horizontalAmount)
    {
        float duration = m_recoilDuration;
        float elapsed = 0f;

        Vector3 originalRotation = m_cameraTransform.localEulerAngles;
        Vector3 targetRotation = new Vector3(
            originalRotation.x - verticalAmount,
            originalRotation.y + Random.Range(-horizontalAmount, horizontalAmount),
            originalRotation.z
            );

        while(elapsed < duration)
        {
            m_cameraTransform.localEulerAngles = Vector3.Lerp(originalRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator GunKickBack(float kickbackAmount)
    {
        float duration = 0.05f;
        float elapsed = 0f;
        Vector3 kickPosition = m_originalGunPosition - new Vector3(0, 0, kickbackAmount);
        Quaternion kickRotation = m_originalGunRotation * Quaternion.Euler(-kickbackAmount * 10f, Random.Range(-2f, 2f), 0f);

        while (elapsed < duration)
        {
            m_gunTransform.localPosition = Vector3.Lerp(m_originalGunPosition, kickPosition, elapsed / duration);
            m_gunTransform.localRotation = Quaternion.Lerp(m_originalGunRotation, kickRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            m_gunTransform.localPosition = Vector3.Lerp(kickPosition, m_originalGunPosition, elapsed / duration);
            m_gunTransform.localRotation = Quaternion.Lerp(kickRotation, m_originalGunRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
