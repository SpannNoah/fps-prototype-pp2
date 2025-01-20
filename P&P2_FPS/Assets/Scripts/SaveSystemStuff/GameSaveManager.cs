using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }
    public GameData playerData = new GameData();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playerData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game Loaded!");
        }
    }
}