using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Make this class serializable
public class PlayerData
{
    public float m_crouchColliderHeight;
    public float m_crouchCameraHeight;
    public float m_baseSpeed;
    public float m_sprintMod;
    public int m_HP;
    public int m_ogHP;
    public float m_speed;
    public float[] position; // Store position as an array
    public int levelNumber;

    // Constructor
    public PlayerData(PlayerController player)
    {
        m_crouchColliderHeight = player.crouchColliderHeight;
        m_crouchCameraHeight = player.crouchCameraHeight;
        m_baseSpeed = player.m_baseSpeed;
        m_sprintMod = player.m_baseSprintModifier;
        m_HP = player.Health;
        m_ogHP = player.playerHealthOrig;
        m_speed = player.Speed;

        // Save player position
        position = new float[3];
        position[0] = player.transform.position.x; // X
        position[1] = player.transform.position.y; // Y
        position[2] = player.transform.position.z; // Z

        levelNumber = player.CurrentLevel; // Stores current level
    }
}
