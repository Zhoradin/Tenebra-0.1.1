using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectController : MonoBehaviour
{
    public static CardSelectController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> cardToSelect = new List<CardSO>();
    public List<CardSelect> activeCardSlots = new List<CardSelect>();
    public GameObject cardSlotPrefab;
    public Transform scrollViewContent; // Scroll view i�indeki Content alan�

    private Vector2 originalPosition;
    public Vector2 panelOpenPosition;

    public float moveSpeed;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ShowRandomCards()
    {
        // Kart slotlar� listesini temizleyin
        activeCardSlots.Clear();

        // Scroll view i�indeki �nceki kartlar� temizle
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Rastgele �� kart se�
        List<CardSO> randomCards = new List<CardSO>();
        while (randomCards.Count < 3)
        {
            CardSO randomCard = cardToSelect[Random.Range(0, cardToSelect.Count)];
            if (!randomCards.Contains(randomCard))
            {
                randomCards.Add(randomCard);
            }
        }

        // Se�ilen kartlar� olu�tur ve scroll view i�inde g�ster
        foreach (CardSO card in randomCards)
        {
            GameObject cardSlot = Instantiate(cardSlotPrefab, scrollViewContent);
            CardSelect slotScript = cardSlot.GetComponent<CardSelect>();
            slotScript.SetupCardSlot(card);

            // Kart slotunu aktif kart slotlar� listesine ekleyin
            activeCardSlots.Add(slotScript);
        }

        StartCoroutine(SlideMenuCo(panelOpenPosition));
    }

    // Di�er kart slotlar�n�n t�klanabilirli�ini devre d��� b�rak
    public void DisableOtherCardSlots(CardSelect selectedSlot)
    {
        foreach (CardSelect slot in activeCardSlots)
        {
            if (slot != selectedSlot)
            {
                // Sadece Card Trigger i�indeki butonu devre d��� b�rak
                slot.SetCardTriggerInteractable(false);
            }
        }
    }

    IEnumerator SlideMenuCo(Vector2 targetPosition)
    {
        yield return new WaitForSeconds(1f);

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
