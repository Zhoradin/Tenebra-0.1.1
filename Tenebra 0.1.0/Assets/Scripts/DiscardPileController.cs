using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPileController : MonoBehaviour
{
    public static DiscardPileController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> discardPile = new List<CardSO>();

    public GameObject cardSlotPrefab;
    public Transform cardSlotParent;

    // Start is called before the first frame update
    void Start()
    {
        CreateDiscardPileCardSlots();
    }

    private void CreateDiscardPileCardSlots()
    {
        ClearCardSlots();
        UIController.instance.ShowDiscardPileCount();
        foreach (CardSO card in discardPile)
        {
            GameObject newCardSlot = Instantiate(cardSlotPrefab, cardSlotParent);
            CardSlot cardSlot = newCardSlot.GetComponent<CardSlot>();
            cardSlot.SetupCardSlot(card);
        }
    }

    private void ClearCardSlots()
    {
        foreach (Transform child in cardSlotParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddToDiscardPile(CardSO card)
    {
        discardPile.Add(card);
        CreateDiscardPileCardSlots();
    }

    public void DiscardToDraw()
    {
        if (discardPile.Count > 0)
        {
            DrawPileController.instance.drawPile.AddRange(discardPile);
            DrawPileController.instance.CreateDrawPileCardSlots();
            discardPile.Clear();
            CreateDiscardPileCardSlots();
        }
    }
}
