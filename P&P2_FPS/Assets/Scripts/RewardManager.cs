using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardManager : MonoBehaviour
{
    [SerializeField]
    private List<RewardConfig> m_weaponRewards = new List<RewardConfig>();
    [SerializeField]
    private List<RewardConfig> m_buffRewards = new List<RewardConfig>();
    [SerializeField]
    private GameObject m_platformPrefab = null;
    
    private Vector3 m_reward1SpawnPos = Vector3.zero;
    private Vector3 m_reward2SpawnPos = Vector3.zero;
    private RewardConfig m_currentWeaponReward = null;
    private RewardConfig m_currentBuffReward = null;
    private GameObject m_currentRewardPlatformWeapon = null;
    private GameObject m_currentRewardPlatformBuff = null;
    
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
        
        float weaponYOffset = m_currentWeaponReward.m_yOffset;
        float buffYOffset = m_currentBuffReward.m_yOffset;
        
        Vector3 weaponPlatformPos = m_currentRewardPlatformWeapon.transform.position;
        Vector3 buffPlatformPos = m_currentRewardPlatformBuff.transform.position;

        Vector3 weaponSpawnPos =
            new Vector3(weaponPlatformPos.x, weaponPlatformPos.y + weaponYOffset, weaponPlatformPos.z);
        Vector3 buffSpawnPos = 
            new Vector3(buffPlatformPos.x, buffPlatformPos.y + buffYOffset, buffPlatformPos.z);

        Instantiate(m_currentWeaponReward.m_rewardPrefab, weaponSpawnPos, Quaternion.identity);
        Instantiate(m_currentBuffReward.m_rewardPrefab, buffSpawnPos, Quaternion.identity);
    }

    public void ClaimReward(RewardConfig reward)
    {
        Debug.Log($"Player has claimed {reward.m_rewardName}");
    }
}
