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

    public Sprite moonPhaseSprite;

    public CardAbilitySO[] abilities;

    public MoonPhase moonPhase;

    public CardType cardType;
}
public enum MoonPhase
{
    NewMoon,
    WaxingCrescent,
    FirstQuarter,
    WaxingGibbous,
    FullMoon,
    WaningGibbous,
    LastQuarter,
    WaningCrescent
}
