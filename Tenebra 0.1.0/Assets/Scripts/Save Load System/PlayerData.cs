using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public List<CardSO> deck;
    public List<ItemSO> items;
    public List<Room> rooms;
    public int health;
    public int maxHealth;
    public int essence;
    public int money;
    public string currentSceneName;
    public string currentRoom;
    public string slotName;

    // Badge bool değerleri
    public bool hasCaerulisnBadge;
    public bool hasAmarunisBadge;
    public bool hasPoulviBadge;
    public bool hasArstelloBadge;
    public bool hasLogiumBadge;
    public bool hasRohvBadge;
    public bool hasSoliriaBadge;
    public bool hasTenebraBadge;
    public bool hasAbororBadge;

    // Oyuncunun en son uğradığı tower/god
    public string lastGod;

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
        slotName = "";

        // Badge bool değerlerini başlangıçta false olarak ayarla
        hasCaerulisnBadge = false;
        hasAmarunisBadge = false;
        hasPoulviBadge = false;
        hasArstelloBadge = false;
        hasLogiumBadge = false;
        hasRohvBadge = false;
        hasSoliriaBadge = false;
        hasTenebraBadge = false;
        hasAbororBadge = false;

        // En son uğranılan kuleyi başlangıçta boş bırak
        lastGod = "";
    }
}
