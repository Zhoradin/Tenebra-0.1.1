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

    private List<CardSO> activeCards = new List<CardSO>();

    public Card cardToSpawn;

    public int drawCardCost = 1;

    public float waitBetweenDrawingCards = .5f;

    // Start is called before the first frame update
    void Start()
    {
        SetupDeck();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            DrawCardToHand();
        }*/
    }

    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardSO> tempDeck = new List<CardSO>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while(tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }
    }

    public void DrawCardToHand()
    {
        if(activeCards.Count == 0)
        {
            SetupDeck();
        }

        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        newCard.cardSO = activeCards[0];
        newCard.SetupCard();

        activeCards.RemoveAt(0);

        HandController.instance.AddCardToHand(newCard);
    }

    public void DrawCardForEssence()
    {
        if(BattleController.instance.playerEssence >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerEssence(drawCardCost);
        }
        else
        {
            UIController.instance.ShowEssenceWarning();
            UIController.instance.drawCardButton.GetComponent<Button>().interactable = false;
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
