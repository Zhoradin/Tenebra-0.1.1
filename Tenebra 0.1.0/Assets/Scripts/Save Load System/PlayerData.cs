using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<CardSO> deck;       // Oyuncunun kart destesi
    public List<ItemSO> items;      // Oyuncunun sahip olduğu itemler
    public List<Room> rooms;        // Oyuncunun pathwayi
    public int health;              // Oyuncunun canı
    public int maxHealth;           // Oyuncunun max canı
    public int essence;             // Oyuncunun essence miktarı
    public int money;               // Oyuncunun parası
    public string currentSceneName; // Oyuncunun en son bulunduğu sahne
    public string currentRoom;

    public PlayerData()
    {
        deck = new List<CardSO>();
        items = new List<ItemSO>();
        rooms = new List<Room>();
        maxHealth = 150;
        health = 100;
        essence = 4;
        money = 0;
        currentSceneName = "";
        currentRoom = "";
    }
}