using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPileController : MonoBehaviour
{
    public static DrawPileController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> drawPile = new List<CardSO>();

    public GameObject cardSlotPrefab;
    public Transform cardSlotParent;

    // Start is called before the first frame update
    void Start()
    {
        // Kart listesini doðru þekilde temizleyip yeniden ekleme iþlemi yapýyoruz.
        InitializeDrawPile();
    }

    public void InitializeDrawPile()
    {
        drawPile.Clear();
        drawPile.AddRange(DeckController.instance.deckToUse);
        CreateDrawPileCardSlots();
    }

    // Kartlarý CardSlot prefablarýna yerleþtir
    public void CreateDrawPileCardSlots()
    {
        ClearCardSlots();
        UIController.instance.ShowDrawPileCount();
        foreach (CardSO card in drawPile)
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

    public void RemoveCardFromDrawPile(CardSO card)
    {
        drawPile.Remove(card);
        CreateDrawPileCardSlots();
    }
}
