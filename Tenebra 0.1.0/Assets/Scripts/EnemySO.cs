using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    public enum EnemyRarity
    {
        Common,
        Uncommon,
        Rare, 
        Epic,
        Legendary
    }

    public int enemyHealth;

    public Sprite enemySprite;

    public List<CardSO> deckToUse = new List<CardSO>();
}
