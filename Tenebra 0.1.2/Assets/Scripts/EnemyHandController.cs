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

    public List<Card> enemyHeldCards = new List<Card>();
    public Transform minPos, maxPos;

    public List<Vector3> enemyCardPositions = new List<Vector3>();
    //public List<Quaternion> enemyCardRotations = new List<Quaternion>();

    public float maxSpacing = 2f; // Maksimum kartlar arasýndaki mesafe, Inspector'da ayarlanabilir
    public float minSpacing = 0.5f; // Minimum kartlar arasýndaki mesafe, Inspector'da ayarlanabilir

    void Start()
    {
        SetCardPositionsInHand();
    }

    public void SetCardPositionsInHand()
    {
        enemyCardPositions.Clear();

        // Aralýðý ayarla
        float spacing = maxSpacing;
        if (enemyHeldCards.Count > 1)
        {
            spacing = Mathf.Lerp(maxSpacing, minSpacing, (float)(enemyHeldCards.Count - 1) / (enemyHeldCards.Count));
        }

        // Kart pozisyonlarýný hesapla ve kartlarý konumlandýr
        for (int i = 0; i < enemyHeldCards.Count; i++)
        {
            // Her bir kart için pozisyon hesapla
            Vector3 straightPosition = minPos.position + new Vector3(i * spacing, 0, 0); // Sadece yatay eksende hizala
            enemyCardPositions.Add(straightPosition);

            // Rotasyonu minimum ve maksimum pozisyon arasýnda doðrusal bir þekilde ayarla
            Quaternion interpolatedRotation = Quaternion.Lerp(minPos.rotation, maxPos.rotation, (float)i / (enemyHeldCards.Count - 1));

            // Kartý yeni pozisyonuna taþý
            enemyHeldCards[i].MoveToPoint(enemyCardPositions[i], interpolatedRotation);

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
        enemyHeldCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    public void EmptyHand()
    {
        foreach (Card heldCard in enemyHeldCards)
        {
            heldCard.inPlayerHand = false;
            heldCard.MoveToPoint(BattleController.instance.discardPoint.position, Quaternion.identity);

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
}
