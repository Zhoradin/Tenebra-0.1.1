using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour, IDataPersistence
{
    public static DeckController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> deckToUse = new List<CardSO>();
    public List<CardSO> drawDeck = new List<CardSO>();

    public Card cardToSpawn;
    public int drawCardCost = 1;
    public float waitBetweenDrawingCards = 0.5f;

    private bool isStarting = true;

    void Start()
    {
        InitializeDrawDeck();
    }

    private void Update()
    {

    }

    public void InitializeDrawDeck()
    {
        // drawDeck listesini temizleyip deckToUse listesinden kart ekliyoruz
        drawDeck.Clear();
        deckToUse.Clear();
        deckToUse.AddRange(FindObjectOfType<DataCarrier>().deckToUse);
        drawDeck.AddRange(deckToUse);
        DrawPileController.instance.SetDrawPile();
        UIController.instance.ShowDrawPileCount();
        UIController.instance.ShowDiscardPileCount();
    }

    public void DrawDeckRedeck()
    {
        drawDeck.AddRange(DrawPileController.instance.drawPile);
    }

    public void DrawCardToHand()
    {
        if (drawDeck.Count == 0)
        {
            if (isStarting)
            {
                InitializeDrawDeck();
                isStarting = false;
            }
        }

        int selected = Random.Range(0, drawDeck.Count);
        CardSO selectedCard = drawDeck[selected];

        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        newCard.cardSO = selectedCard;
        newCard.SetupCard();

        drawDeck.RemoveAt(selected);

        HandController.instance.AddCardToHand(newCard);

        AudioManager.instance.PlaySFX(3);

        DrawPileController.instance.RemoveCardFromDrawPile(selectedCard);

        UIController.instance.ShowDrawPileCount();
    }

    public void DrawCardForEssence()
    {
        if (BattleController.instance.playerEssence >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerEssence(drawCardCost);
            if (drawDeck.Count == 0)
            {
                DiscardPileController.instance.DiscardToDraw();
                DrawDeckRedeck();
                Debug.Log("Kart Listesi Boş!");
            }
        }
        else
        {
            UIController.instance.ShowEssenceWarning();
            UIController.instance.drawCardButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

    IEnumerator DrawMultipleCo(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();
            yield return new WaitForSeconds(waitBetweenDrawingCards);
            if (drawDeck.Count == 0)
            {
                DiscardPileController.instance.DiscardToDraw();
                DrawDeckRedeck();
                Debug.Log("Kart Listesi Boş!");
            }
        }
    }

    public void LoadData(PlayerData data)
    {
        deckToUse.Clear();
        deckToUse.AddRange(data.deck);
    }

    public void SaveData(PlayerData data)
    {
        data.deck.Clear();
        data.deck.AddRange(deckToUse);
    }
}