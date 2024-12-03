using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BarController : MonoBehaviour
{
    public static BarController instance;

    public Vector2 shopPanelClosedPosition, shopPanelOpenPosition;

    public GameObject shopPanel;
    public GameObject shopButton, dialogueButton, leaveButton;

    public bool shopPanelOpen, dialoguePanelOpen = false;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text healthText, coinText;

    public GameObject cardSlot1, cardSlot2, cardSlot3, cardSlot4, cardSlot5, cardSlot6, cardSlot7;
    public GameObject itemSlot1, itemSlot2, itemSlot3, itemSlot4, itemSlot5, itemSlot6;

    public GameObject lowCoinWarning;

    public List<CardSO> unlockedCards = new List<CardSO>();
    public List<ItemSO> unlockedItems = new List<ItemSO>();

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        AssignRandomCardsToSlots(); // Kartlarý slotlara ata
        AssignRandomItemsToSlots(); // Item'larý slotlara ata

        SetPlayerHealth();
        SetPlayerCoin();

        shopPanel.GetComponent<RectTransform>().anchoredPosition = shopPanelClosedPosition;
    }

    void Update()
    {

    }

    public void OpenShopPanel()
    {
        shopPanel.transform.localPosition = shopPanelOpenPosition;
        dialogueButton.SetActive(false);
        leaveButton.SetActive(false);
        shopButton.SetActive(false);
    }

    public void CloseShopPanel()
    {
        shopPanel.transform.localPosition = shopPanelClosedPosition;
        dialogueButton.SetActive(true);
        leaveButton.SetActive(true);
        shopButton.SetActive(false);
    }

    public void OnLeaveClicked()
    {
        SceneManager.LoadScene("Hub");
    }

    public void SetPlayerHealth()
    {
        healthText.text = DataCarrier.instance.playerHealth + "/" + DataCarrier.instance.playerMaxHealth;
    }

    public void SetPlayerCoin()
    {
        coinText.text = DataCarrier.instance.playerCoin.ToString();
    }

    private void AssignRandomCardsToSlots()
    {
        List<GameObject> cardSlots = new List<GameObject>
        {
            cardSlot1, cardSlot2, cardSlot3, cardSlot4, cardSlot5, cardSlot6, cardSlot7
        };

        List<CardSO> rareCards = unlockedCards.FindAll(card => card.cardRarity == CardRarity.Rare);
        List<CardSO> uncommonCards = unlockedCards.FindAll(card => card.cardRarity == CardRarity.Uncommon);
        List<CardSO> commonCards = unlockedCards.FindAll(card => card.cardRarity == CardRarity.Common);

        System.Random random = new System.Random();

        AssignCardsToSlots(cardSlots.GetRange(0, 2), rareCards, random);
        AssignCardsToSlots(cardSlots.GetRange(2, 2), uncommonCards, random);
        AssignCardsToSlots(cardSlots.GetRange(4, 3), commonCards, random);

        SelectRandomCardSlotAndLogName(cardSlots, random);
    }

    private void SelectRandomCardSlotAndLogName(List<GameObject> cardSlots, System.Random random)
    {
        int randomSlotIndex = random.Next(cardSlots.Count);
        GameObject selectedSlot = cardSlots[randomSlotIndex];

        CardMarket cardMarket = selectedSlot.GetComponent<CardMarket>();

        if (cardMarket != null && cardMarket.GetCard() != null)
        {
            int updatedCoinAmount = cardMarket.GetCoinAmount() / 2;
            cardMarket.SetCoinAmount(updatedCoinAmount);
        }
    }

    private void AssignCardsToSlots(List<GameObject> slots, List<CardSO> cards, System.Random random)
    {
        foreach (var slot in slots)
        {
            if (cards.Count > 0)
            {
                int randomIndex = random.Next(cards.Count);
                CardSO selectedCard = cards[randomIndex];

                CardMarket cardMarket = slot.GetComponent<CardMarket>();
                cardMarket.SetupCardSlot(selectedCard);

                cards.RemoveAt(randomIndex);
            }
        }
    }

    private void AssignRandomItemsToSlots()
    {
        List<GameObject> itemSlots = new List<GameObject>
        {
            itemSlot1, itemSlot2, itemSlot3, itemSlot4, itemSlot5, itemSlot6
        };

        List<ItemSO> availableItems = new List<ItemSO>(unlockedItems);
        System.Random random = new System.Random();

        foreach (var slot in itemSlots)
        {
            if (availableItems.Count > 0)
            {
                int randomIndex = random.Next(availableItems.Count);
                ItemSO selectedItem = availableItems[randomIndex];

                ItemMarket itemMarket = slot.GetComponent<ItemMarket>();
                itemMarket.SetupItemSlot(selectedItem);

                availableItems.RemoveAt(randomIndex);
            }
        }
    }
}
