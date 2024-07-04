using UnityEngine;

[CreateAssetMenu(fileName = "New Card Ability", menuName = "Card Ability", order = 1)]
public class CardAbilitySO : ScriptableObject
{
    public enum AbilityType
    {
        Heal,
        DirectHit
    }

    public AbilityType abilityType;
    public int value;
}
