using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("Stats")]
    public string m_gunName = string.Empty;
    public GameObject gunModel;
    public bool m_isProjectile;
    public GameObject m_projectilePrefab = null;
    public int shootDamage;
    public float shootRate;
    public float shootDist;
    public int ammoCur, ammoMax;

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
}
