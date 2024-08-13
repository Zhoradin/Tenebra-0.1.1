using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlot : MonoBehaviour
{
    public int slotNumber; // Bu save slotun numarasý
    private SaveLoadSystem saveLoadSystem;
    private GameController gameController;

    private void Start()
    {
        saveLoadSystem = FindObjectOfType<SaveLoadSystem>();
        gameController = FindObjectOfType<GameController>();
    }

    public void OnSlotClicked()
    {
        // Save veya load iþlemi için slot numarasýný diðer scriptlere aktar
        saveLoadSystem.currentSlot = slotNumber;
        gameController.currentSlot = slotNumber;

        // Slot týklandýðýnda kaydetme veya yükleme iþlemini baþlatabilirsiniz
        if (MainMenu.instance.isLoadGame)
        {
            gameController.LoadGame();
        }
        else if(MainMenu.instance.isNewGame)
        {
            gameController.currentSlot = slotNumber;
            gameController.SaveGame();
            SceneManager.LoadScene("Hub");
        }
    }
}
