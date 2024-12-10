using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Object/Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    public enum EnemyRarity
    {
        Common,
        Epic, 
        Boss
    }

    public EnemyRarity enemyRarity;

    public int enemyHealth;

    public int enemyEssence;

    public int gainedCoin;

    public Sprite enemySprite;

    public List<CardSO> deckToUse = new List<CardSO>();

    public AudioClip[] backgroundMusic;
}
