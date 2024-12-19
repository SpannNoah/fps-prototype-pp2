using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_weaponRewards = new List<GameObject>();
    [SerializeField]
    private List<RewardConfig> m_buffRewards = new List<RewardConfig>();
    [SerializeField]
    private GameObject m_platformPrefab = null;
    
    private Vector3 m_reward1SpawnPos = Vector3.zero;
    private Vector3 m_reward2SpawnPos = Vector3.zero;
    private GameObject m_currentWeaponReward = null;
    private RewardConfig m_currentBuffReward = null;
    private GameObject m_currentRewardPlatformWeapon = null;
    private GameObject m_currentRewardPlatformBuff = null;
    private List<GameObject> m_rewardPlatforms = new List<GameObject>();
    private List<GameObject> m_currentRewards = new List<GameObject>();
    
    public static RewardManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        m_reward1SpawnPos = GameObject.FindWithTag("Platform1").transform.position;
        m_reward2SpawnPos = GameObject.FindWithTag("Platform2").transform.position;
    }
    public void SpawnRewards()
    {
        int randomWeaponIndex = Random.Range(0, m_weaponRewards.Count);
        int randomBuffIndex = Random.Range(0, m_buffRewards.Count);

        m_currentBuffReward = m_buffRewards[randomBuffIndex];
        m_currentWeaponReward = m_weaponRewards[randomWeaponIndex];
        
        m_currentRewardPlatformWeapon = Instantiate(m_platformPrefab, m_reward1SpawnPos, Quaternion.identity);
        m_currentRewardPlatformBuff = Instantiate(m_platformPrefab, m_reward2SpawnPos, Quaternion.identity);

        m_rewardPlatforms.Add(m_currentRewardPlatformBuff);
        m_rewardPlatforms.Add(m_currentRewardPlatformWeapon);

        //float weaponYOffset = m_currentWeaponReward.m_yOffset;
        float buffYOffset = m_currentBuffReward.m_yOffset;
        
        Vector3 weaponPlatformPos = m_currentRewardPlatformWeapon.transform.position;
        Vector3 buffPlatformPos = m_currentRewardPlatformBuff.transform.position;

        Vector3 weaponSpawnPos =
            new Vector3(weaponPlatformPos.x, weaponPlatformPos.y + .5f, weaponPlatformPos.z);
        Vector3 buffSpawnPos = 
            new Vector3(buffPlatformPos.x, buffPlatformPos.y + buffYOffset, buffPlatformPos.z);

        GameObject weapon = Instantiate(m_currentWeaponReward, weaponSpawnPos, Quaternion.identity);
        if(weapon.TryGetComponent<pickup>(out pickup pickup))
        {
            pickup.m_isReward = true;
        }
        GameObject buff = Instantiate(m_currentBuffReward.m_rewardPrefab, buffSpawnPos, Quaternion.identity);

        m_currentRewards.Add(weapon);
        m_currentRewards.Add(buff);
    }

    public void ClaimReward(GameObject reward)
    {
        Debug.Log($"Player has claimed {reward.name}");
        
        foreach(GameObject platform in m_rewardPlatforms)
        {
            if(platform != null) Destroy(platform);
        }

        foreach(GameObject obj in m_currentRewards)
        {
            if(obj != null) Destroy(obj);
        }
    }
}
