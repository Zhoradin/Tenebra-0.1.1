using UnityEngine;

public abstract class CardAbility : ScriptableObject
{
    public string abilityName;
    public abstract void ActivateAbility(Card card);
}