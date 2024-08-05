using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<CardSO> deck;    // Oyuncunun kart destesi
    public List<ItemSO> items;   // Oyuncunun sahip oldu�u itemler
    public int health;           // Oyuncunun can�
    public int essence;          // Oyuncunun essence miktar�
    public int money;            // Oyuncunun paras�

    public PlayerData()
    {
        deck = new List<CardSO>();
        items = new List<ItemSO>();
        health = 100;
        essence = 4;
        money = 0;
    }
}