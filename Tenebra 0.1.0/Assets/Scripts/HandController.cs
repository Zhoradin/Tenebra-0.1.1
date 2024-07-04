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
    public List<Vector3> cardPositions = new List<Vector3>();
    public List<Quaternion> cardRotations = new List<Quaternion>();
    public float curveHeight = 2f; // Kartlarýn y eksenindeki þiþkinlik miktarý

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

        Vector3 distanceBetweenPoints = Vector3.zero;
        if (heldCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        Vector3 midpoint = (minPos.position + maxPos.position) / 2;

        for (int i = 0; i < heldCards.Count; i++)
        {
            float t = heldCards.Count > 1 ? (float)i / (heldCards.Count - 1) : 0.5f;
            Vector3 straightPosition = minPos.position + (distanceBetweenPoints * i);

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

    public void RemoveCardFromHand(Card cardToRemove)
    {
        if (heldCards[cardToRemove.handPosition] == cardToRemove)
        {
            heldCards.RemoveAt(cardToRemove.handPosition);
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
        foreach(Card heldCard in heldCards)
        {
            heldCard.inHand = false;
            heldCard.MoveToPoint(BattleController.instance.discardPoint.position, Quaternion.identity);
        }

        heldCards.Clear();
    }
}
