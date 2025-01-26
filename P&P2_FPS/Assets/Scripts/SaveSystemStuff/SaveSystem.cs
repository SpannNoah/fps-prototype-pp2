using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string playerSaveFilePath = Path.Combine(Application.persistentDataPath, "playerSave.json");

    public static void SavePlayer(PlayerController player)
    {
        PlayerData playerData = new PlayerData(player);
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(playerSaveFilePath, json);
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(playerSaveFilePath))
        {
            string json = File.ReadAllText(playerSaveFilePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
    }
}