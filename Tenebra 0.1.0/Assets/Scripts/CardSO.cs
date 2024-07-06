using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class CardSO : ScriptableObject
{
    public string cardName;

    [TextArea]
    public string cardDescription;

    public int currentHealth, attackPower, essenceCost;

    public Sprite characterSprite, bgSprite;

    public CardAbilitySO[] abilities;
    internal object transform;
}
