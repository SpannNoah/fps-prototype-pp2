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
    
    private Vector3 m_reward1SpawnPos = Vector3.zero;
    private Vector3 m_reward2SpawnPos = Vector3.zero;
    private RewardConfig m_currentWeaponReward = null;
    private RewardConfig m_currentBuffReward = null;

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
        
        Instantiate(m_currentWeaponReward.GetRewardPrefab(), m_reward1SpawnPos, Quaternion.identity);
        Instantiate(m_currentBuffReward.GetRewardPrefab(), m_reward2SpawnPos, Quaternion.identity);
        
    }
}
