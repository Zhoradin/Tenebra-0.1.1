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

    public float maxSpacing = 2f; // Maksimum kartlar aras�ndaki mesafe, Inspector'da ayarlanabilir
    public float minSpacing = 0.5f; // Minimum kartlar aras�ndaki mesafe, Inspector'da ayarlanabilir

    [HideInInspector]
    public bool isHandReady = false;

    void Start()
    {
        SetCardPositionsInHand();
    }

    public void SetCardPositionsInHand()
    {
        enemyCardPositions.Clear();

        if (enemyHeldCards.Count == 0) return;

        // �lk kart minPos'ta, son kart maxPos'ta olacak �ekilde da��l�m
        Vector3 minPosition = minPos.position;
        Vector3 maxPosition = maxPos.position;

        // Kartlar aras�ndaki aral��� hesapla
        float totalSpacing = maxSpacing * (enemyHeldCards.Count - 1); // Toplam mesafe
        float midPointX = (minPosition.x + maxPosition.x) / 2; // Ortada bir referans noktas� olu�turuyoruz

        // Kartlar aras�ndaki mesafeyi dinamik olarak hesapla
        float spacing = Mathf.Clamp((maxPosition.x - minPosition.x) / (enemyHeldCards.Count + 1), minSpacing, maxSpacing);

        for (int i = 0; i < enemyHeldCards.Count; i++)
        {
            // Kartlar� ortalayarak yerle�tir
            float offset = (i - (enemyHeldCards.Count - 1) / 2f) * spacing; // Ortada hizalamak i�in offset hesaplama
            Vector3 newPosition = new Vector3(midPointX + offset, minPosition.y, minPosition.z);
            Quaternion interpolatedRotation = Quaternion.Lerp(minPos.rotation, maxPos.rotation, 0.5f); // Rotasyonun ortas�nda kalmas� i�in

            // Pozisyonu listeye ekle
            enemyCardPositions.Add(newPosition);

            // Kart� yeni pozisyonuna ta��
            enemyHeldCards[i].MoveToPoint(newPosition, interpolatedRotation);

            // Kart bilgilerini g�ncelle
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
                // Kart�n pozisyonunu de�i�tirme
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
            heldCard.MoveToPoint(BattleController.instance.enemyDiscardPoint.position, Quaternion.identity);

            // E�er bu kart isPlayer'a aitse discardPile'a ekle
            if (heldCard.isPlayer)
            {
                DiscardPileController.instance.AddToDiscardPile(heldCard.cardSO);
            }

            // Kart� discardPoint'e ta��d�ktan sonra k���ltme i�lemini ba�lat ve sonra yok et
            StartCoroutine(DestroyCardAfterMove(heldCard));
        }

        enemyHeldCards.Clear();
    }

    private IEnumerator DestroyCardAfterMove(Card card)
    {
        // Kart�n hareketini tamamlamas�n� beklemek i�in k�sa bir s�re bekleyelim
        yield return card.StartCoroutine(card.ScaleDownCo());

        // Kart� discardPoint'e ta��d�ktan sonra yok et
        Destroy(card.gameObject);
    }
}
