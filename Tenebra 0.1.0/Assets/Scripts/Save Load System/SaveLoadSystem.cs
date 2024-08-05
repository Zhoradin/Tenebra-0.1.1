using System.Collections.Generic;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/savefile.json";
    }

    public void SaveGame(PlayerData playerData)
    {
        string jsonData = JsonUtility.ToJson(playerData);
        System.IO.File.WriteAllText(saveFilePath, jsonData);
    }

    public PlayerData LoadGame()
    {
        if (System.IO.File.Exists(saveFilePath))
        {
            string jsonData = System.IO.File.ReadAllText(saveFilePath);
            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(jsonData);
            return loadedData;
        }
        return null;
    }
}