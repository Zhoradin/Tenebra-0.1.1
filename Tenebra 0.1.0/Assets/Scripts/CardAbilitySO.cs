using UnityEngine;

[CreateAssetMenu(fileName = "New Card Ability", menuName = "Scriptable Object/Card Ability", order = 1)]
public class CardAbilitySO : ScriptableObject
{
    public enum AbilityType
    {
        Heal,
        DirectHit,
        DoubleTap,
        QuickAttack,
        GlassCannon,
        Mend,
        Leech,
        Revelation
    }

    public AbilityType abilityType;
    public int value;
    public string description;
}
