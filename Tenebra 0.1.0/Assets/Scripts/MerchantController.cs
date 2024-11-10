using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MerchantController : MonoBehaviour
{
    public static MerchantController instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text healthText, coinText;

    public GameObject cardSlot1, cardSlot2, cardSlot3, cardSlot4, cardSlot5, cardSlot6, cardSlot7;
    public GameObject itemSlot1, itemSlot2, itemSlot3, itemSlot4, itemSlot5, itemSlot6;

    public GameObject lowEssenceWarning;

    public List<CardSO> unlockedCards = new List<CardSO>();
    public List<ItemSO> unlockedItems = new List<ItemSO>();  // ItemSO listesi eklendi

    public string whichTower;

    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        UpdateCoin();
        healthText.text = DataCarrier.instance.playerHealth + "/" + DataCarrier.instance.playerMaxHealth;

        whichTower = "Pathway " + DataCarrier.instance.lastGod;

        AssignRandomCardsToSlots(); // Kartlarý slotlara ata
        AssignRandomItemsToSlots(); // Item'larý slotlara ata
    }

    public void UpdateCoin()
    {
        coinText.text = DataCarrier.instance.playerCoin.ToString();
    }

    public void OnLeaveClicked()
    {
        SceneManager.LoadScene(whichTower);
    }

    private void AssignRandomCardsToSlots()
    {
        List<GameObject> cardSlots = new List<GameObject>
        {
            cardSlot1, cardSlot2, cardSlot3, cardSlot4, cardSlot5, cardSlot6, cardSlot7
        };

        List<CardSO> availableCards = new List<CardSO>(unlockedCards); // Kartlarý geçici bir listeye kopyala
        System.Random random = new System.Random();

        foreach (var slot in cardSlots)
        {
            if (availableCards.Count > 0)
            {
                int randomIndex = random.Next(availableCards.Count);
                CardSO selectedCard = availableCards[randomIndex];

                CardMarket cardMarket = slot.GetComponent<CardMarket>();
                cardMarket.SetupCardSlot(selectedCard);

                availableCards.RemoveAt(randomIndex);
            }
        }
    }

    // Item'larý slotlara atamak için yeni fonksiyon
    private void AssignRandomItemsToSlots()
    {
        List<GameObject> itemSlots = new List<GameObject>
        {
            itemSlot1, itemSlot2, itemSlot3, itemSlot4, itemSlot5, itemSlot6
        };

        List<ItemSO> availableItems = new List<ItemSO>(unlockedItems); // Item'larý geçici bir listeye kopyala
        System.Random random = new System.Random();

        foreach (var slot in itemSlots)
        {
            if (availableItems.Count > 0)
            {
                int randomIndex = random.Next(availableItems.Count);
                ItemSO selectedItem = availableItems[randomIndex];

                ItemMarket itemMarket = slot.GetComponent<ItemMarket>();
                itemMarket.SetupItemSlot(selectedItem); // Item'ý slot'a ata

                availableItems.RemoveAt(randomIndex);
            }
        }
    }
}
