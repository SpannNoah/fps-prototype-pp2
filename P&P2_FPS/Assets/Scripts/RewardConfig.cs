using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewReward", menuName = "Rewards/Reward")]
public class RewardConfig : ScriptableObject
{
    public enum RewardType
    {
        Weapon,
        Buff
    }

    [SerializeField]
    private RewardType m_rewardType = RewardType.Weapon;
    [SerializeField]
    private string m_rewardName = String.Empty;
    [SerializeField]
    private GameObject m_rewardPrefab = null;

    public RewardType GetRewardType()
    {
        return m_rewardType;
    }

    public string GetRewardName()
    {
        return m_rewardName;
    }

    public GameObject GetRewardPrefab()
    {
        return m_rewardPrefab;
    }
}
