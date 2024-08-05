using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<CardSO> deck;       // Oyuncunun kart destesi
    public List<ItemSO> items;      // Oyuncunun sahip olduğu itemler
    public int health;              // Oyuncunun canı
    public int essence;             // Oyuncunun essence miktarı
    public int money;               // Oyuncunun parası
    public string currentSceneName; // Oyuncunun en son bulunduğu sahne

    public PlayerData()
    {
        deck = new List<CardSO>();
        items = new List<ItemSO>();
        health = 100;
        essence = 4;
        money = 0;
        currentSceneName = "";
    }
}