using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Card> heldCards = new List<Card>();

    public Transform minPos, maxPos;
    public Vector3 lockedPosition;
    public Quaternion lockedRotation;
    public List<Vector3> cardPositions = new List<Vector3>();
    public List<Quaternion> cardRotations = new List<Quaternion>();
    public float curveHeight = 2f; // Kartlarýn y eksenindeki þiþkinlik miktarý
    public float maxSpacing = 2f; // Maksimum kartlar arasýndaki mesafe, Inspector'da ayarlanabilir
    public float minSpacing = 0.5f; // Minimum kartlar arasýndaki mesafe, Inspector'da ayarlanabilir

    // Start is called before the first frame update
    void Start()
    {
        SetCardPositionsInHand();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCardPositionsInHand()
    {
        cardPositions.Clear();
        cardRotations.Clear();

        float spacing = maxSpacing;
        if (heldCards.Count > 1)
        {
            spacing = Mathf.Lerp(maxSpacing, minSpacing, (float)(heldCards.Count - 1) / (heldCards.Count));
        }

        Vector3 midpoint = (minPos.position + maxPos.position) / 2;

        for (int i = 0; i < heldCards.Count; i++)
        {
            float t = heldCards.Count > 1 ? (float)i / (heldCards.Count - 1) : 0.5f;
            Vector3 straightPosition = midpoint + new Vector3((i - (heldCards.Count - 1) / 2f) * spacing, 0, 0);

            // Kenar kartlar minPos ve maxPos'a göre z konumunu alacak
            float zPos = Mathf.Lerp(minPos.position.z, maxPos.position.z, t);
            straightPosition.z = zPos;

            // Create a parabolic curve
            float parabolicOffset = curveHeight * Mathf.Sin(t * Mathf.PI);
            Vector3 curvedPosition = new Vector3(straightPosition.x, straightPosition.y + parabolicOffset, straightPosition.z);
            cardPositions.Add(curvedPosition);

            // Interpolating rotation between minPos and maxPos
            Quaternion interpolatedRotation = Quaternion.Lerp(minPos.rotation, maxPos.rotation, t);
            cardRotations.Add(interpolatedRotation);

            heldCards[i].MoveToPoint(cardPositions[i], interpolatedRotation);

            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }
    }

    public void SpreadCards(int hoveredIndex, float spreadAmount)
    {
        for (int i = 0; i < heldCards.Count; i++)
        {
            if (i == hoveredIndex)
            {
                // Kartýn pozisyonunu deðiþtirme
                continue;
            }

            float offset = (i < hoveredIndex) ? -spreadAmount : spreadAmount;
            Vector3 newPosition = cardPositions[i] + new Vector3(offset, 0, 0);
            heldCards[i].MoveToPoint(newPosition, cardRotations[i]);
        }
    }

    public void ResetCardPositions()
    {
        for (int i = 0; i < heldCards.Count; i++)
        {
            heldCards[i].MoveToPoint(cardPositions[i], cardRotations[i]);
        }
    }


    public void RemoveCardFromHand(Card cardToRemove)
    {
        if (heldCards.Contains(cardToRemove))
        {
            if (heldCards[cardToRemove.handPosition] == cardToRemove)
            {
                heldCards.RemoveAt(cardToRemove.handPosition);
            }
        }
        else if(cardToRemove.isLocked)
        {
            
        }
        else
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the card being removed from hand.");
        }

        SetCardPositionsInHand();
    }

    public void AddCardToHand(Card cardToAdd)
    {
        heldCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    public void EmptyHand()
    {
        foreach (Card heldCard in heldCards)
        {
            heldCard.inHand = false;
            heldCard.MoveToPoint(BattleController.instance.discardPoint.position, Quaternion.identity);

            // Eðer bu kart isPlayer'a aitse discardPile'a ekle
            if (heldCard.isPlayer)
            {
                DiscardPileController.instance.AddToDiscardPile(heldCard.cardSO);
            }

            // Kartý discardPoint'e taþýdýktan sonra küçültme iþlemini baþlat ve sonra yok et
            StartCoroutine(DestroyCardAfterMove(heldCard));
        }

        heldCards.Clear();
    }

    private IEnumerator DestroyCardAfterMove(Card card)
    {
        // Kartýn hareketini tamamlamasýný beklemek için kýsa bir süre bekleyelim
        yield return card.StartCoroutine(card.ScaleDownCo());

        // Kartý discardPoint'e taþýdýktan sonra yok et
        Destroy(card.gameObject);
    }

}
