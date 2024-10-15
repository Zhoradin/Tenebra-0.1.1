using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem instance;

    public int currentSlot;
    public string currentSotName;
    public string slotName;

    private void Awake()
    {
        // Eðer bir baþka SaveLoadSystem instance'ý varsa bu nesneyi yok et
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Bu nesneyi koru ve Singleton yap
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private string GetSavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, "savegame_slot" + slotNumber + ".json");
    }

    public void SaveGame(PlayerData playerData)
    {
        string savePath = GetSavePath(currentSlot);
        playerData.slotName = slotName;
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(savePath, json);
    }

    public PlayerData LoadGame()
    {
        string savePath = GetSavePath(currentSlot);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            Time.timeScale = 1f;
            return playerData;
        }
        else
        {
            Debug.Log("Save file not found.");
            return null;
        }
    }

    public bool SaveFileExists(int slotNumber)
    {
        return File.Exists(GetSavePath(slotNumber));
    }

    public int GetMostRecentSaveSlot()
    {
        int mostRecentSlot = -1;
        System.DateTime mostRecentTime = System.DateTime.MinValue;

        for (int i = 1; i <= 3; i++)
        {
            string path = GetSavePath(i);
            if (File.Exists(path))
            {
                System.DateTime creationTime = File.GetLastWriteTime(path);
                if (creationTime > mostRecentTime)
                {
                    mostRecentTime = creationTime;
                    mostRecentSlot = i;
                }
            }
        }

        return mostRecentSlot;
    }

    public PlayerData LoadGame(int slotNumber)
    {
        string savePath = GetSavePath(slotNumber);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.Log("Save file not found for slot " + slotNumber);
            return null;
        }
    }

    public void DeleteSaveFile(int slotNumber)
    {
        string path = Application.persistentDataPath + "/savegame_slot" + slotNumber + ".json";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted for slot: " + slotNumber);
        }
    }
}
