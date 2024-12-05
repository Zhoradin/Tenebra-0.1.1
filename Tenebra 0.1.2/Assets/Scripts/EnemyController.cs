using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    private void Awake()
    {
        instance = this;
    }

    public EnemySO enemySO;

    public List<CardSO> deckToUse = new List<CardSO>();
    public List<CardSO> activeCards = new List<CardSO>();
    public List<CardSO> cardsInHand = new List<CardSO>();
    public List<Card> wannabeHeldCards = new List<Card>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;

    public enum AIType { placeFromDeck, handRandomPlace, handDefensive, handAttacking }
    public AIType enemyAIType;

    public int startHandSize;
    public int cardsToDrawEachTurn = 1;

    [HideInInspector]
    public int enemyHealth, enemyEssence;
    [HideInInspector]
    public Image enemyImage;
    [HideInInspector]
    public bool isStarting = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForEnemyCo());
    }

    public IEnumerator WaitForEnemyCo()
    {
        yield return new WaitForSeconds(.5f);

        if (FindObjectOfType<DataCarrier>().enemy != null)
        {
            enemySO = FindObjectOfType<DataCarrier>().enemy;
        }

        yield return new WaitForSeconds(.5f);

        if (enemySO != null)
        {
            deckToUse.AddRange(enemySO.deckToUse);
            enemyHealth = enemySO.enemyHealth;
            enemyEssence = enemySO.enemyEssence;
            BattleController.instance.SetEnemyHealth();
        }

        SetupDeck();

        if (enemyAIType != AIType.placeFromDeck)
        {
            SetupHand();
        }

        enemyImage.sprite = enemySO.enemySprite;
    }

    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardSO> tempDeck = new List<CardSO>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }
    }

    public void SetupHand()
    {
        StartCoroutine(SetupHandCo());
    }

    private IEnumerator SetupHandCo()
    {
        for (int i = 0; i < startHandSize; i++)
        {
            if (activeCards.Count == 0)
            {
                SetupDeck();
            }

            cardsInHand.Add(activeCards[0]);
            StartCoroutine(InstantiateCardCo(activeCards[0]));
            activeCards.RemoveAt(0);

            yield return new WaitForSeconds(.3f); // Her kart eklenmeden önce bekleme süresi
        }
    }

    private IEnumerator InstantiateCardCo(CardSO cardSO)
    {
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
        newCard.cardSO = cardSO; // Kart Scriptable Object'ini ata
        newCard.SetupCard(); // Kartýn görsel ve diðer özelliklerini ayarla
        EnemyHandController.instance.AddCardToHand(newCard); // Yeni kartý düþman eline ekle
        yield return null; // Coroutine'den çýkýþ
    }

    public void StartAction()
    {
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        yield return new WaitForSeconds(2f);

        if (enemyAIType != AIType.placeFromDeck && isStarting == false)
        {
            for (int i = 0; i < cardsToDrawEachTurn; i++)
            {
                cardsInHand.Add(activeCards[0]);
                activeCards.RemoveAt(0);

                if (activeCards.Count == 0)
                {
                    SetupDeck();
                }
            }
        }

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(CardPointsController.instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
        {
            cardPoints.Remove(selectedPoint);

            while (selectedPoint.activeCard != null && cardPoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPoints.Count);
                selectedPoint = cardPoints[randomPoint];
                cardPoints.RemoveAt(randomPoint);
            }
        }

        CardSO selectedCard = null;
        int iterations = 0;
        List<CardPlacePoint> preferredPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> secondaryPoints = new List<CardPlacePoint>();

        switch (enemyAIType)
        {
            case AIType.placeFromDeck:

                if (selectedPoint.activeCard == null)
                {
                    Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
                    newCard.cardSO = activeCards[0];
                    activeCards.RemoveAt(0);
                    newCard.SetupCard();
                    newCard.MoveToPoint(selectedPoint.transform.position, selectedPoint.transform.rotation);

                    selectedPoint.activeCard = newCard;
                    newCard.assignedPlace = selectedPoint;
                }

                break;

            case AIType.handRandomPlace:

                selectedCard = SelectedCardToPlay();

                iterations = 50;
                while (selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    PlayCard(selectedCard, selectedPoint);

                    //check if we should try play another card
                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.timeBetweenAttacks);

                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }
                }

                break;

            case AIType.handDefensive:

                selectedCard = SelectedCardToPlay();

                preferredPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerCardPoints[i].activeCard != null)
                        {
                            preferredPoints.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {
                    //pick a point to use
                    if (preferredPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoints.Count);
                        selectedPoint = preferredPoints[selectPoint];

                        preferredPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];

                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    //check if we should try play another
                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.timeBetweenAttacks);
                }

                break;

            case AIType.handAttacking:

                selectedCard = SelectedCardToPlay();

                preferredPoints.Clear();
                secondaryPoints.Clear();

                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerCardPoints[i].activeCard == null)
                        {
                            preferredPoints.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {
                    //pick a point to use
                    if (preferredPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoints.Count);
                        selectedPoint = preferredPoints[selectPoint];

                        preferredPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];

                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    //check if we should try play another
                    selectedCard = SelectedCardToPlay();

                    iterations--;

                    yield return new WaitForSeconds(CardPointsController.instance.timeBetweenAttacks);
                }

                break;
        }

        yield return new WaitForSeconds(.5f);

        BattleController.instance.AdvanceTurn();
    }

    public void PlayCard(CardSO cardSO, CardPlacePoint placePoint)
    {
        // EnemyHandController'daki kartý bul
        Card selectedCard = null;
        foreach (Card card in EnemyHandController.instance.enemyHeldCards)
        {
            if (card.cardSO == cardSO)
            {
                selectedCard = card;
                break;
            }
        }

        // Eðer kart bulunamadýysa iþlemi sonlandýr
        if (selectedCard == null)
        {
            Debug.LogError("Selected card not found in enemyHeldCards.");
            return;
        }

        // Kartý hedef noktaya taþý
        selectedCard.MoveToPoint(placePoint.transform.position + new Vector3(0f, 0.75f, 0f), placePoint.transform.rotation);
        placePoint.activeCard = selectedCard;
        selectedCard.assignedPlace = placePoint;

        // Kartýn özelliklerini aktive et
        AbilityManager.instance.ActivateAbility(selectedCard);
        selectedCard.isActive = true;
        if (selectedCard.instaKill == true)
        {
            StartCoroutine(AbilityManager.instance.QuickAttackCoroutine(selectedCard));
        }
        MoonPhaseController.instance.CheckMoonPhase(selectedCard);

        // Kartý eldeki kartlar listesinden kaldýr
        EnemyHandController.instance.enemyHeldCards.Remove(selectedCard);
        EnemyHandController.instance.SetCardPositionsInHand();

        // Essence harcama
        BattleController.instance.SpendEnemyEssence(cardSO.essenceCost);
    }

    CardSO SelectedCardToPlay()
    {
        Card selectedCard = null;

        // essenceCost ve diðer uygunluk kriterlerine uyan kartlarý filtreleyelim
        List<Card> cardsToPlay = new List<Card>();
        foreach (Card card in EnemyHandController.instance.enemyHeldCards)
        {
            if (card.cardSO.cardKind == CardKind.Field && card.cardSO.essenceCost <= BattleController.instance.enemyEssence)
            {
                cardsToPlay.Add(card);
            }
        }

        // Uygun kartlar varsa rastgele birini seç
        if (cardsToPlay.Count > 0)
        {
            int selected = Random.Range(0, cardsToPlay.Count);
            selectedCard = cardsToPlay[selected];
        }
        else
        {
            Debug.Log("No valid card to play.");
        }

        // Kart bulunamadýðýnda null kontrolü yapýyoruz
        if (selectedCard != null && selectedCard.cardSO != null)
        {
            return selectedCard.cardSO;
        }
        else
        {
            // Eðer geçerli bir kart seçilemediyse null döndür
            return null;
        }
    }

    //Hide the informations of the enemy card, will flip the card later
    /*
    public void HideEssentials(Card card)
    {
        card.abilityDescriptionText.gameObject.SetActive(false);
    }*/
}
