using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public int slotNumber;
    public GameObject editButton, deleteButton;
    public TMP_Text inputText, slotNameText;
    public TMP_InputField slotNameInput;
    public TMP_Text healthText, coinText;
    public Image healthImage, coinImage;
    public Image caerulisnBadge, amarunisBadge, poulviBadge, arstelloBadge, logiumBadge, rohvBadge, soliriaBadge, tenebraBadge, abororBadge;
    public Image caerulisnBg, amarunisBg, poulviBg, arstelloBg, logiumBg, rohvBg, soliriaBg, tenebraBg, abororBg;
    public GameObject areYouSurePanel, overwritePanel;
    private SaveLoadSystem saveLoadSystem;
    private GameController gameController;

    private void Start()
    {
        saveLoadSystem = FindObjectOfType<SaveLoadSystem>();
        gameController = FindObjectOfType<GameController>();

        areYouSurePanel.gameObject.SetActive(false);
        overwritePanel.gameObject.SetActive(false);

        GameObject[] badges = { caerulisnBadge.gameObject, amarunisBadge.gameObject, poulviBadge.gameObject, arstelloBadge.gameObject, logiumBadge.gameObject, rohvBadge.gameObject, soliriaBadge.gameObject, tenebraBadge.gameObject, abororBadge.gameObject };
        GameObject[] backgrounds = { caerulisnBg.gameObject, amarunisBg.gameObject, poulviBg.gameObject, arstelloBg.gameObject, logiumBg.gameObject, rohvBg.gameObject, soliriaBg.gameObject, tenebraBg.gameObject, abororBg.gameObject };

        foreach (GameObject badge in badges) badge.SetActive(false);
        foreach (GameObject bg in backgrounds) bg.SetActive(false);

        slotNameInput.gameObject.SetActive(false);

        // E�er isNewGame true ise ve bu slotta bir save dosyas� varsa, slot t�klanamaz hale gelsin
        CheckActiveness();

        LoadSlotData();  // Mevcut save verilerini y�kle
    }

    private void Update()
    {
        CheckActiveness();
    }

    public void CheckActiveness()
    {
        /*if (MainMenu.instance.isNewGame && saveLoadSystem.SaveFileExists(slotNumber))
        {
            GetComponent<Button>().interactable = false;
            editButton.GetComponent<Button>().interactable = false;
            deleteButton.GetComponent<Button>().interactable = true;
        }
        else */
        if (MainMenu.instance.isLoadGame && !saveLoadSystem.SaveFileExists(slotNumber))
        {
            GetComponent<Button>().interactable = false;
            editButton.GetComponent<Button>().interactable = false;
            deleteButton.GetComponent<Button>().interactable = false;
        }
        else if(MainMenu.instance.isNewGame && !saveLoadSystem.SaveFileExists(slotNumber))
        {
            deleteButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
            editButton.GetComponent<Button>().interactable = true;
            deleteButton.GetComponent<Button>().interactable = true;
        }

    }

    public void OnSlotClicked()
    {
        areYouSurePanel.SetActive(false);

        saveLoadSystem.currentSlot = slotNumber;
        gameController.currentSlot = slotNumber;
        saveLoadSystem.slotName = slotNameText.text;

        if (MainMenu.instance.isLoadGame)
        {
            gameController.SaveGame();
            gameController.LoadGame();
        }
        else if (MainMenu.instance.isNewGame && !saveLoadSystem.SaveFileExists(slotNumber))
        {
            gameController.currentSlot = slotNumber;
            gameController.SaveGame();
            SceneManager.LoadScene("Hub");
        }
        else if(MainMenu.instance.isNewGame && saveLoadSystem.SaveFileExists(slotNumber))
        {
            overwritePanel.gameObject.SetActive(true);
        }
    }

    public void OnEditClicked()
    {
        areYouSurePanel.SetActive(false);

        slotNameInput.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(slotNameText.text))
        {
            inputText.color = Color.black;
        }
        slotNameText.gameObject.SetActive(false);
        slotNameInput.ActivateInputField();
        editButton.GetComponent<Button>().interactable = false;
    }

    public void EditDone()
    {
        // inputText'in bo� olup olmad���n� kontrol et (bo�luk karakterlerini de hesaba kat)
        if (!string.IsNullOrWhiteSpace(slotNameInput.text))
        {
            // slotNameInput doluysa slotNameText'e girilen metni ata
            slotNameText.text = slotNameInput.text.Trim();  // slotNameInput'tan gelen metni al�yoruz
            slotNameText.color = Color.black;
        }
        else
        {
            // slotNameInput bo�sa 'New Save' yaz�s�n� ata
            slotNameText.text = "New Save";
            slotNameText.color = Color.gray;  // Varsay�lan bir renk kullanabilirsiniz
        }

        // D�zenleme i�lemini tamamla
        slotNameInput.gameObject.SetActive(false);
        slotNameText.gameObject.SetActive(true);
        editButton.GetComponent<Button>().interactable = true;
    }

    private void LoadSlotData()
    {
        areYouSurePanel.SetActive(false);

        if (saveLoadSystem.SaveFileExists(slotNumber))
        {
            PlayerData playerData = saveLoadSystem.LoadGame(slotNumber);

            healthText.text = playerData.maxHealth.ToString();
            coinText.text = playerData.money.ToString();
            slotNameText.text = playerData.slotName;
            slotNameText.color = Color.black;

            // Badge'leri aktif/pasif yap
            caerulisnBadge.gameObject.SetActive(playerData.hasCaerulisnBadge);
            amarunisBadge.gameObject.SetActive(playerData.hasAmarunisBadge);
            poulviBadge.gameObject.SetActive(playerData.hasPoulviBadge);
            arstelloBadge.gameObject.SetActive(playerData.hasArstelloBadge);
            logiumBadge.gameObject.SetActive(playerData.hasLogiumBadge);
            rohvBadge.gameObject.SetActive(playerData.hasRohvBadge);
            soliriaBadge.gameObject.SetActive(playerData.hasSoliriaBadge);
            tenebraBadge.gameObject.SetActive(playerData.hasTenebraBadge);
            abororBadge.gameObject.SetActive(playerData.hasAbororBadge);

            // T�m arka planlar� kapat, sonra sadece en son gidilen God'a g�re aktif et
            caerulisnBg.gameObject.SetActive(false);
            amarunisBg.gameObject.SetActive(false);
            poulviBg.gameObject.SetActive(false);
            arstelloBg.gameObject.SetActive(false);
            logiumBg.gameObject.SetActive(false);
            rohvBg.gameObject.SetActive(false);
            soliriaBg.gameObject.SetActive(false);
            tenebraBg.gameObject.SetActive(false);
            abororBg.gameObject.SetActive(false);

            // lastGod de�i�kenine g�re uygun arka plan� aktif et
            switch (playerData.lastGod)
            {
                case "Caerulisn":
                    caerulisnBg.gameObject.SetActive(true);
                    break;
                case "Amarunis":
                    amarunisBg.gameObject.SetActive(true);
                    break;
                case "Poulvi":
                    poulviBg.gameObject.SetActive(true);
                    break;
                case "Arstello":
                    arstelloBg.gameObject.SetActive(true);
                    break;
                case "Logium":
                    logiumBg.gameObject.SetActive(true);
                    break;
                case "Rohv":
                    rohvBg.gameObject.SetActive(true);
                    break;
                case "Soliria":
                    soliriaBg.gameObject.SetActive(true);
                    break;
                case "Tenebra":
                    tenebraBg.gameObject.SetActive(true);
                    break;
                case "Aboror":
                    abororBg.gameObject.SetActive(true);
                    break;
                default:
                    // E�er lastGod bilinmiyorsa hi�bir arka plan aktif olmas�n
                    break;
            }
        }
        else
        {
            healthText.gameObject.SetActive(false);
            coinText.gameObject.SetActive(false);
            healthImage.gameObject.SetActive(false);
            coinImage.gameObject.SetActive(false);
        }
    }

    public void OnDeleteClicked()
    {
        areYouSurePanel.SetActive(true);
    }

    public void OnCancelClicked()
    {
        areYouSurePanel.SetActive(false);
    }

    public void DeleteSave()
    {
        // Save dosyas�n� sil
        saveLoadSystem.DeleteSaveFile(slotNumber);

        areYouSurePanel.SetActive(false);

        // Slot �zerindeki bilgileri temizle
        healthText.text = "";
        coinText.text = "";
        healthImage.gameObject.SetActive(false);
        coinImage.gameObject.SetActive(false);
        slotNameText.text = "New Save";
        slotNameText.color = Color.gray;

        // T�m badge ve background'lar� pasif yap
        caerulisnBadge.gameObject.SetActive(false);
        amarunisBadge.gameObject.SetActive(false);
        poulviBadge.gameObject.SetActive(false);
        arstelloBadge.gameObject.SetActive(false);
        logiumBadge.gameObject.SetActive(false);
        rohvBadge.gameObject.SetActive(false);
        soliriaBadge.gameObject.SetActive(false);
        tenebraBadge.gameObject.SetActive(false);
        abororBadge.gameObject.SetActive(false);

        caerulisnBg.gameObject.SetActive(false);
        amarunisBg.gameObject.SetActive(false);
        poulviBg.gameObject.SetActive(false);
        arstelloBg.gameObject.SetActive(false);
        logiumBg.gameObject.SetActive(false);
        rohvBg.gameObject.SetActive(false);
        soliriaBg.gameObject.SetActive(false);
        tenebraBg.gameObject.SetActive(false);
        abororBg.gameObject.SetActive(false);

        // Silme i�lemi sonras� slotu t�klanabilir hale getir
        GetComponent<Button>().interactable = true;

        gameController.SaveGame();
    }

    public void OnOverwriteClicked()
    {
        DeleteSave();
        gameController.currentSlot = slotNumber;
        gameController.SaveGame();
        SceneManager.LoadScene("Hub");
    }

    public void OnOverwriteCancelled()
    {
        overwritePanel.gameObject.SetActive(false);
    }
}
