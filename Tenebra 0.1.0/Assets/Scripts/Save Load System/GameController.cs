﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private SaveLoadSystem saveLoadSystem;
    private List<IDataPersistence> dataPersistenceObjects;
    public int currentSlot;

    private void Start()
    {
        saveLoadSystem = SaveLoadSystem.instance;
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        currentSlot = saveLoadSystem.currentSlot;
    }

    public void SaveGame()
    {
        PlayerData playerData = new PlayerData();

        // Tüm data persistence objelerinden verileri kaydet
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(playerData);
        }

        // Sahne adı "MainMenu" ise "Hub" olarak kaydet, diğer sahneler olduğu gibi kalsın
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Main Menu" || currentScene == "Hub")
        {
            playerData.currentSceneName = "Hub";
        }
        else if (currentScene == "Rest Site")
        {
            playerData.currentSceneName = "Pathway " + DataCarrier.instance.lastGod;
        }
        else
        {
            playerData.currentSceneName = currentScene;
        }

        saveLoadSystem.currentSlot = currentSlot;
        saveLoadSystem.SaveGame(playerData);
        Debug.Log("Oyun kaydedildi");
    }


    public void LoadGame()
    {
        saveLoadSystem.currentSlot = currentSlot;
        PlayerData loadedData = saveLoadSystem.LoadGame();
        if (loadedData != null)
        {
            StartCoroutine(LoadSceneAndSetData(loadedData.currentSceneName));
        }
        Debug.Log("Oyun yüklendi");
    }

    private IEnumerator LoadSceneAndSetData(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        PlayerData loadedData = saveLoadSystem.LoadGame();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(loadedData);
        }

        Debug.Log("Oyun yüklendi");

        yield return null;
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
