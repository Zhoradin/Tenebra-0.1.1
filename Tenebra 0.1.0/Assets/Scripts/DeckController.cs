using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
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

    public void InitializeDrawDeck()
    {
        drawDeck = deckToUse;
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
            else
            {
                UIController.instance.drawCardButton.GetComponent<Button>().interactable = false;
                Debug.Log("Kart Listesi Boþ!");
                return;
            }
        }

        // Kartý çek
        int selected = Random.Range(0, drawDeck.Count);
        CardSO selectedCard = drawDeck[selected];

        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        newCard.cardSO = selectedCard;
        newCard.SetupCard();

        // Kartý drawDeck'ten kaldýr
        drawDeck.RemoveAt(selected);

        HandController.instance.AddCardToHand(newCard);

        if (CardPileController.instance != null)
        {
            CardPileController.instance.SetupPile();
        }
    }

    public void DrawCardForEssence()
    {
        if (BattleController.instance.playerEssence >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerEssence(drawCardCost);
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
        }
    }
}
