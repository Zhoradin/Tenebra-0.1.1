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
        Mend,
        Leech,
        Revelation,
        Metamorphosis,
        PrimalPact,
        Scattershot,
        Growth,
        Decay,
        Guardian,
        Reckoning,
        Benevolence,
        HealingTouch,
        Snowball,
        Duality, 
        Resurrect,
        Doppelganger,
        Gratis,
        Stun,
        HealBlock,
        Mirror,
        Harvester,
        Dreamweaving,
        Bulwark,
        SoulSucking,
        Switch,
        Armor,
        AnimateDead
    }

    public AbilityType abilityType;
    public int value;
    [TextArea]
    public string description;
}
