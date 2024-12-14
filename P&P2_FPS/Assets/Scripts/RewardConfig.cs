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
    
    public RewardType m_rewardType = RewardType.Weapon;
    public string m_rewardName = String.Empty;
    public GameObject m_rewardPrefab = null;
    [Tooltip("Dictates Y Offset from Platform on Spawn")]
    public float m_yOffset = 0.0f;
}
