using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public string hubScene = "Hub";
    private SaveLoadSystem saveLoadSystem;

    public Button continueButton, loadGameButton;

    public GameObject saveSlotPanel;
    public bool isLoadGame = false;
    public bool isNewGame = false;

    private void Start()
    {
        instance = this;
        saveLoadSystem = FindObjectOfType<SaveLoadSystem>();

        // Butonun aktifliðini kontrol et
        CheckButtonAvailability();
    }

    public void NewGame()
    {
        isNewGame = true;
        saveSlotPanel.SetActive(true);
    }

    public void ContinueGame()
    {
        int mostRecentSlot = saveLoadSystem.GetMostRecentSaveSlot();
        if (mostRecentSlot != -1)
        {
            saveLoadSystem.currentSlot = mostRecentSlot;
            GameController gameController = FindObjectOfType<GameController>();
            gameController.currentSlot = mostRecentSlot;
            gameController.LoadGame();
        }
        else
        {
            Debug.LogWarning("No saved game found.");
        }
    }


    public void LoadGame()
    {
        isLoadGame = true;
        saveSlotPanel.SetActive(true);
    }

    public void OnReturnClicked()
    {
        saveSlotPanel.SetActive(false);
    }

    private void CheckButtonAvailability()
    {
        bool saveExists = false;
        for (int i = 1; i <= 3; i++) // Slot sayýsý kadar döngü yap
        {
            if (saveLoadSystem.SaveFileExists(i))
            {
                saveExists = true;
                break;
            }
        }

        continueButton.interactable = saveExists;
        loadGameButton.interactable = saveExists;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
