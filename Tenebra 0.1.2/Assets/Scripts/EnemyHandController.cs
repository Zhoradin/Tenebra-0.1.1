using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandController : MonoBehaviour
{
    public static EnemyHandController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject enemyCardContainer;
    public Transform cardContainerOpenPosition, cardContainerClosePosition;

    public List<Card> enemyHeldCards = new List<Card>();
    public List<EnemyCardContainer> enemyPortraitHeldCards = new List<EnemyCardContainer>();
    public float slideDistance = -2f;
    public Transform minPos, maxPos;

    public List<Vector3> enemyCardPositions = new List<Vector3>();
    //public List<Quaternion> enemyCardRotations = new List<Quaternion>();

    public float maxSpacing = 2f; // Maksimum kartlar arasýndaki mesafe, Inspector'da ayarlanabilir
    public float minSpacing = 0.5f; // Minimum kartlar arasýndaki mesafe, Inspector'da ayarlanabilir

    [HideInInspector]
    public bool isHandReady = false;

    void Start()
    {
        enemyCardContainer.transform.localPosition = cardContainerClosePosition.transform.localPosition;
        SetCardPositionsInHand();
    }

    public void SetCardPositionsInHand()
    {
        enemyCardPositions.Clear();

        if (enemyHeldCards.Count == 0) return;

        // Ýlk kart minPos'ta, son kart maxPos'ta olacak þekilde daðýlým
        Vector3 minPosition = minPos.position;
        Vector3 maxPosition = maxPos.position;

        // Kartlar arasýndaki aralýðý hesapla
        float totalSpacing = maxSpacing * (enemyHeldCards.Count - 1); // Toplam mesafe
        float midPointX = (minPosition.x + maxPosition.x) / 2; // Ortada bir referans noktasý oluþturuyoruz

        // Kartlar arasýndaki mesafeyi dinamik olarak hesapla
        float spacing = Mathf.Clamp((maxPosition.x - minPosition.x) / (enemyHeldCards.Count + 1), minSpacing, maxSpacing);

        for (int i = 0; i < enemyHeldCards.Count; i++)
        {
            // Kartlarý ortalayarak yerleþtir
            float offset = (i - (enemyHeldCards.Count - 1) / 2f) * spacing; // Ortada hizalamak için offset hesaplama
            Vector3 newPosition = new Vector3(midPointX + offset, minPosition.y, minPosition.z);
            Quaternion interpolatedRotation = Quaternion.Lerp(minPos.rotation, maxPos.rotation, 0.5f); // Rotasyonun ortasýnda kalmasý için

            // Pozisyonu listeye ekle
            enemyCardPositions.Add(newPosition);

            // Kartý yeni pozisyonuna taþý
            enemyHeldCards[i].MoveToPoint(newPosition, interpolatedRotation);

            // Kart bilgilerini güncelle
            enemyHeldCards[i].inEnemyHand = true;
            enemyHeldCards[i].handPosition = i;
        }
    }

    public void SpreadCards(int hoveredIndex, float spreadAmount)
    {
        for (int i = 0; i < enemyHeldCards.Count; i++)
        {
            if (i == hoveredIndex)
            {
                // Kartýn pozisyonunu deðiþtirme
                continue;
            }

            float offset = (i < hoveredIndex) ? -spreadAmount : spreadAmount;
            Vector3 newPosition = enemyCardPositions[i] + new Vector3(offset, 0, 0);
            enemyHeldCards[i].MoveToPoint(newPosition, minPos.rotation);
        }
    }

    public void ResetCardPositions()
    {
        for (int i = 0; i < enemyHeldCards.Count; i++)
        {
            enemyHeldCards[i].MoveToPoint(enemyCardPositions[i], minPos.rotation);
        }
    }

    public void RemoveCardFromHand(Card cardToRemove)
    {
        if (enemyHeldCards.Contains(cardToRemove))
        {
            if (enemyHeldCards[cardToRemove.handPosition] == cardToRemove)
            {
                enemyHeldCards.RemoveAt(cardToRemove.handPosition);
            }
        }
        else
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the card being removed from hand.");
        }

        SetCardPositionsInHand();
    }

    public void AddCardToHand(Card cardToAdd)
    {
        // Kartý düþman eline ekle
        enemyHeldCards.Add(cardToAdd);

        // Kartýn tip frame'ini enemyPortraitHeldCards'a atama
        if (enemyPortraitHeldCards.Count > enemyHeldCards.Count - 1) // Geçerli kart sayýsýný kontrol et
        {
            var enemyCardContainer = enemyPortraitHeldCards[enemyHeldCards.Count - 1];
            if (cardToAdd.cardSO != null && cardToAdd.cardSO.typeFrameSprite != null)
            {
                enemyCardContainer.cardTypeFrame.sprite = cardToAdd.cardSO.typeFrameSprite;
            }
            else
            {
                Debug.LogWarning("cardSO or typeFrameSprite is null for card: " + cardToAdd.name);
            }
        }
        else
        {
            Debug.LogError("No EnemyCardContainer available for card: " + cardToAdd.name);
        }

        // Kart pozisyonlarýný güncelle
        SetCardPositionsInHand();
    }

    public void EmptyHand()
    {
        foreach (Card heldCard in enemyHeldCards)
        {
            heldCard.inPlayerHand = false;
            heldCard.MoveToPoint(BattleController.instance.enemyDiscardPoint.position, Quaternion.identity);

            // Eðer bu kart isPlayer'a aitse discardPile'a ekle
            if (heldCard.isPlayer)
            {
                DiscardPileController.instance.AddToDiscardPile(heldCard.cardSO);
            }

            // Kartý discardPoint'e taþýdýktan sonra küçültme iþlemini baþlat ve sonra yok et
            StartCoroutine(DestroyCardAfterMove(heldCard));
        }

        enemyHeldCards.Clear();
    }

    private IEnumerator DestroyCardAfterMove(Card card)
    {
        // Kartýn hareketini tamamlamasýný beklemek için kýsa bir süre bekleyelim
        yield return card.StartCoroutine(card.ScaleDownCo());

        // Kartý discardPoint'e taþýdýktan sonra yok et
        Destroy(card.gameObject);
    }

    public IEnumerator SlideCardLeft(int cardIndex, float slideDuration)
    {
        // Kartý sola kaydýrmaya baþla
        EnemyCardContainer cardContainer = enemyPortraitHeldCards[cardIndex];
        Vector3 originalPosition = cardContainer.transform.position;

        // Kaydýrma mesafesini slideDistance kullanarak hesapla
        Vector3 targetPosition = originalPosition + new Vector3(slideDistance, 0f, 0f); // X ekseninde sola kaydýrma

        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            cardContainer.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardContainer.transform.position = targetPosition; // Tam hedef pozisyonuna ulaþ
    }

    public void ResetCardContainerPositions()
    {
        // Her bir kartýn baþlangýç pozisyonunu sýfýrlýyoruz
        foreach (EnemyCardContainer portraitCard in enemyPortraitHeldCards)
        {
            RectTransform rectTransform = portraitCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Kartýn yerel pozisyonunu sýfýrla veya istediðiniz baþlangýç pozisyonuna ayarla
                rectTransform.anchoredPosition = new Vector2(3f, rectTransform.anchoredPosition.y);
            }
            else
            {
                Debug.LogWarning("EnemyPortraitHeldCards öðesinde RectTransform bulunamadý.");
            }
        }
    }


    public void OpenCloseSwitchContainer()
    {
        StartCoroutine(SwitchContainerCo());
    }

    private IEnumerator SwitchContainerCo()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        Vector3 startPosition = enemyCardContainer.transform.position;
        Vector3 targetPosition;

        // Eðer kapalýysa açýlacak pozisyonu ayarla, açýksa kapanacak pozisyonu ayarla
        if (!enemyCardContainer.activeSelf)
        {
            targetPosition = cardContainerOpenPosition.position;
            enemyCardContainer.SetActive(true); // Açýlma animasyonu baþlýyor
        }
        else
        {
            targetPosition = cardContainerClosePosition.position;
        }

        // Animasyon
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            enemyCardContainer.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Son pozisyonu ayarla
        enemyCardContainer.transform.position = targetPosition;

        // Eðer kapanma tamamlandýysa görünürlüðü kapat
        if (targetPosition == cardContainerClosePosition.position)
        {
            enemyCardContainer.SetActive(false);
            ResetCardContainerPositions(); // Kapanma tamamlandýktan sonra çaðýr
        }
    }
}
