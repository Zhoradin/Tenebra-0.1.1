using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public string hubScene = "Hub";
    private SaveLoadSystem saveLoadSystem;
    public GameObject saveSlot1, saveSlot2, saveSlot3;

    public Button continueButton, loadGameButton;

    public GameObject saveSlotPanel, optionsPanel;
    public bool isLoadGame = false;
    public bool isNewGame = false;

    public AudioClip menuMusic;

    private void Start()
    {
        saveSlotPanel.gameObject.SetActive(false);
        instance = this;
        saveLoadSystem = FindObjectOfType<SaveLoadSystem>();

        // Butonun aktifliðini kontrol et
        CheckButtonAvailability();

        AudioManager.instance.PullMenuMusic();
    }

    public void NewGame()
    {
        isNewGame = true;
        saveSlotPanel.SetActive(true);

        AudioManager.instance.PlaySFX(0);
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
        AudioManager.instance.PlaySFX(0);
    }


    public void LoadGame()
    {
        isLoadGame = true;
        saveSlotPanel.SetActive(true);
        AudioManager.instance.PlaySFX(0);
    }

    public void OnReturnClicked()
    {
        saveSlot1.GetComponent<Button>().interactable = true;
        saveSlot2.GetComponent<Button>().interactable = true;
        saveSlot3.GetComponent<Button>().interactable = true;
        CheckButtonAvailability();
        saveSlotPanel.SetActive(false);       
        isLoadGame = false;
        isNewGame = false;
        AudioManager.instance.PlaySFX(0);
    }

    public void OnOptionsClicked()
    {
        optionsPanel.SetActive(true);
        SettingsController.instance.OnVideoClicked();
        AudioManager.instance.PlaySFX(0);
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
        AudioManager.instance.PlaySFX(0);
    }
}
