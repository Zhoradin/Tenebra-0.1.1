using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectController : MonoBehaviour
{
    public static CardSelectController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> cardToSelect = new List<CardSO>();
    public GameObject cardSlotPrefab;
    public Transform scrollViewContent; // Scroll view içindeki Content alaný

    private Vector2 originalPosition;
    public Vector2 panelOpenPosition;

    public float moveSpeed;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ShowRandomCards()
    {
        // Scroll view içindeki önceki kartlarý temizle
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Rastgele üç kart seç
        List<CardSO> randomCards = new List<CardSO>();
        while (randomCards.Count < 3)
        {
            CardSO randomCard = cardToSelect[Random.Range(0, cardToSelect.Count)];
            if (!randomCards.Contains(randomCard))
            {
                randomCards.Add(randomCard);
            }
        }

        // Seçilen kartlarý oluþtur ve scroll view içinde göster
        foreach (CardSO card in randomCards)
        {
            GameObject cardSlot = Instantiate(cardSlotPrefab, scrollViewContent);
            CardSlot slotScript = cardSlot.GetComponent<CardSlot>();
            slotScript.SetupCardSlot(card);
        }

        StartCoroutine(SlideMenuCo(panelOpenPosition));
    }

    IEnumerator SlideMenuCo(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 1f / moveSpeed;

        Vector2 startingPosition = transform.localPosition;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
    }

    public IEnumerator SlideMenuToOriginalPosition()
    {
        yield return StartCoroutine(SlideMenuCo(originalPosition));
    }
}
