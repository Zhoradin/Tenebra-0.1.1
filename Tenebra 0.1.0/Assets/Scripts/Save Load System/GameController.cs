using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private SaveLoadSystem saveLoadSystem;
    private List<IDataPersistence> dataPersistenceObjects;

    private void Start()
    {
        saveLoadSystem = FindObjectOfType<SaveLoadSystem>();
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void SaveGame()
    {
        PlayerData playerData = new PlayerData();

        // T�m veri saklama s�n�flar�n�n SaveData fonksiyonunu �a��r
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(playerData);
        }

        saveLoadSystem.SaveGame(playerData);
        Debug.Log("oyun kaydedildi");
    }

    public void LoadGame()
    {
        PlayerData loadedData = saveLoadSystem.LoadGame();
        if (loadedData != null)
        {
            // T�m veri saklama s�n�flar�n�n LoadData fonksiyonunu �a��r
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(loadedData);
            }

            Debug.Log("oyun yüklendi");
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}