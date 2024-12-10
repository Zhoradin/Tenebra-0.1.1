using System.Collections;
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

        if (FindObjectOfType<SettingsController>())
        {
            LoadSettingsOnly();
        }
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

    private void ApplyFPSLimit()
    {
        //Debug.Log($"FPSToggleOn: {DataCarrier.instance.FPSToggleOn}, FPSIndex: {DataCarrier.instance.FPSIndex}");

        if (DataCarrier.instance.FPSToggleOn)
        {
            Application.targetFrameRate = DataCarrier.instance.FPSIndex;
        }
        else
        {
            Application.targetFrameRate = -1;
        }

        //Debug.Log($"Application.targetFrameRate set to: {Application.targetFrameRate}");
    }

    private void LoadSettingsOnly()
    {
        PlayerData loadedData = saveLoadSystem.LoadGame();

        if (loadedData != null)
        {
            DataCarrier.instance.FPSToggleOn = loadedData.FPSToggleOn;
            DataCarrier.instance.FPSIndex = loadedData.FPSIndex;
            DataCarrier.instance.FPSValue = loadedData.FPSValue; // FPSValue'yu yüklüyoruz

            //Debug.Log("FPS ayarları yüklendi.");

            // Update the FPS Toggle
            SettingsController.instance.limitFPSToggle.isOn = DataCarrier.instance.FPSToggleOn;

            // Update the FPS Dropdown
            if (SettingsController.instance.fpsDropdown != null && DataCarrier.instance.FPSValue >= 0 && DataCarrier.instance.FPSValue < SettingsController.instance.fpsDropdown.options.Count)
            {
                SettingsController.instance.fpsDropdown.value = DataCarrier.instance.FPSValue; // Yüklenen FPSValue'yu dropdown'da güncelliyoruz
            }

            // Apply FPS ayarlarını uygula
            ApplyFPSLimit();

            // Apply butonunu tıklanamaz yap
            if (SettingsController.instance.applyButton != null)
            {
                SettingsController.instance.applyButton.interactable = false; // Apply butonunu tıklanamaz yap
            }
        }
    }
}
