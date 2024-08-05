using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<CardSO> deck;    // Oyuncunun kart destesi
    public List<ItemSO> items;   // Oyuncunun sahip olduðu itemler
    public int health;           // Oyuncunun caný
    public int essence;          // Oyuncunun essence miktarý
    public int money;            // Oyuncunun parasý

    public PlayerData()
    {
        deck = new List<CardSO>();
        items = new List<ItemSO>();
        health = 100;
        essence = 4;
        money = 0;
    }
}
