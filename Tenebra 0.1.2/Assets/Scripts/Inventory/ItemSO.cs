using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public string itemName;

    public int itemCost;

    [TextArea]
    public string itemDescription;

    public Sprite itemSprite;

    public enum Permanence
    {
        Permanent,
        Temporary,
        Exhaustible
    }

    public enum ItemSkill
    {
        Heal,
        EssenceIncrease
    }

    public ItemSkill itemSkill;

    public Permanence permanence;

    public int effectAmount;

    public int itemLongevity;
}

