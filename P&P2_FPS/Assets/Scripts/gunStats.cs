using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("Stats")]
    public GameObject gunModel;
    public int shootDamage;
    public float shootRate;
    public float shootDist;
    public int ammoCur, ammoMax;

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
}
