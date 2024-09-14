using System.Collections.Generic;
using UnityEngine;

public class DataCarrier : MonoBehaviour, IDataPersistence
{
    public static DataCarrier instance;

    public int playerMaxHealth;
    public int playerHealth;
    public int playerEssence;
    public int playerCoin;
    public List<ItemSO> possessedItems;
    public List<CardSO> deckToUse;
    public List<Room> pathwayRooms;
    public string currentRoomName;
    public EnemySO enemy;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //TransferData(instance);
            Destroy(gameObject); // Eski DataCarrier'ý yok et
        }
    }

    // Eski DataCarrier'dan verileri yeni DataCarrier'a aktarma
    private void TransferData(DataCarrier oldDataCarrier)
    {
        playerMaxHealth = oldDataCarrier.playerMaxHealth;
        playerHealth = oldDataCarrier.playerHealth;
        playerEssence = oldDataCarrier.playerEssence;
        // Listeyi derin kopyalama
        deckToUse = new List<CardSO>(oldDataCarrier.deckToUse);
        Debug.Log("Veriler aktarýldý.");
    }

    // Health'i güncellemek için metod
    public void UpdatePlayerHealth(int health)
    {
        playerMaxHealth = health;
    }

    // Essence'i güncellemek için metod
    public void UpdatePlayerEssence(int essence)
    {
        playerEssence = essence;
    }

    // Deck'i güncellemek için metod
    public void UpdateDeck(List<CardSO> deck)
    {
        deckToUse = new List<CardSO>(deck);
    }

    public void MaxOutHealth()
    {
        playerHealth = playerMaxHealth;
    }

    public void LoadData(PlayerData data)
    {
        playerHealth = data.health;
        playerEssence = data.essence;
        playerCoin = data.money;
        deckToUse = new List<CardSO>(data.deck);
        possessedItems = new List<ItemSO>(data.items);
        pathwayRooms.Clear();
        pathwayRooms = new List<Room>(data.rooms);
        currentRoomName = data.currentRoom;
    }

    public void SaveData(PlayerData data)
    {
        data.health = playerHealth;
        data.essence = playerEssence;
        data.money = playerCoin;
        data.deck = new List<CardSO>(deckToUse);
        data.items = new List<ItemSO>(possessedItems);
        data.rooms = new List<Room>(pathwayRooms);
        data.currentRoom = currentRoomName;
    }
}
