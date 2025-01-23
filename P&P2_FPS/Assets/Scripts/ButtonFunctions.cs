using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    int Count = 0;
    public void Resume()
    {
        GameManager.Instance.StateUnpaused();
    }

    public void Load()
    {
        
        if (SceneManager.sceneCountInBuildSettings > 0)
        {
            PlayerController player = GameManager.Instance.m_playerController;
            player = player.player;
            PlayerData data = new PlayerData(player);
            SaveSystem.LoadPlayer();
            player.LoadPlayerData();
            SceneManager.LoadScene(GameManager.Instance.currentLevel);
            GameManager.Instance.StateUnpaused();
        }
        else
        {
            Debug.Log("Can't Load Right Now");
            GameManager.Instance.StateUnpaused();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.StateUnpaused();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
