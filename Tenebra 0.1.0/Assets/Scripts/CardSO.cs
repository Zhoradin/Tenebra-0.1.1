using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class CardSO : ScriptableObject
{
    public string cardName;

    public bool isGraveyard;

    [TextArea]
    public string cardDescription;

    public int currentHealth, attackPower, essenceCost, coinCost;

    public Sprite characterSprite, bgSprite;

    public Sprite moonPhaseSprite;

    public CardAbilitySO[] abilities;

    public MoonPhase moonPhase;

    public CardType cardType;

    public CardKind cardKind;

    public CardRarity cardRarity;
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

public enum CardKind
{
    Field,
    Efect
}

public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Crimson, // consumes health to use this rarity of cards
    Cursed // has good and bad effects together
}