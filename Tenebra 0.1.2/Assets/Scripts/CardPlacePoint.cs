using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlacePoint : MonoBehaviour
{
    public Card activeCard;
    public bool isFieldPoint;
    public bool isPlayerPoint;
    public bool isImpactPoint;
    public bool isLockedPoint;
    public int placePointOrder;
    public CardPlacePoint oppositeCardPlacePoint;
}
