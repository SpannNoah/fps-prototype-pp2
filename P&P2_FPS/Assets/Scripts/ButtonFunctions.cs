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
        
        if (Portal.currentLevel > 0)
        {
            PlayerData data = new PlayerData(PlayerController.player);
            SaveSystem.LoadPlayer();
            PlayerController.player.LoadPlayerData();
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
