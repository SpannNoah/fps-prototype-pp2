using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public float m_crouchColliderHeight;
    public float m_crouchCameraHeight;
    public float m_baseSpeed;
    public float m_sprintMod;
    public int m_HP;
    public int m_ogHP;
    public float m_speed;
    public float[] position;
    //public int levelNumber;

    // Constructor
    public PlayerData(PlayerController player)
    {
        // Initialize the fields using the player's properties
        m_crouchColliderHeight = player.crouchColliderHeight; // Assuming these properties exist
        m_crouchCameraHeight = player.crouchCameraHeight; // Assuming these properties exist
        m_baseSpeed = player.m_baseSpeed; // Assuming these properties exist
        m_sprintMod = player.m_baseSprintModifier; // Assuming these properties exist
        m_HP = player.Health; // Assuming these properties exist
        m_ogHP = player.playerHealthOrig; // Assuming these properties exist
        m_speed = player.Speed; // Assuming these properties exist
        position[0] = player.transform.position.x; //save player x pos
        position[1] = player.transform.position.y; //save player y pos
        position[2] = player.transform.position.z; //save player z pos

        //levelNumber = currentLevel.currentLevel; //Stores current level
    }
}
