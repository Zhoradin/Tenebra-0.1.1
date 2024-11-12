using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum RoomType
{
    None,
    Monster,
    Event,
    EliteMonster,
    RestSite,
    Merchant,
    Treasure,
    Boss
}

public class RoomConnection
{
    public Room RoomA { get; private set; }
    public Room RoomB { get; private set; }

    public RoomConnection(Room roomA, Room roomB)
    {
        RoomA = roomA;
        RoomB = roomB;
    }
}


[Serializable]
public class Room
{
    public int X { get; set; }
    public int Y { get; set; }
    public RoomType RoomType { get; set; }
    public int Id { get; private set; } // Oda için benzersiz ID
    public string Name;

    public Room(int x, int y, int id, string name, RoomType roomType)
    {
        X = x;
        Y = y;
        this.Name = name;
        RoomType = roomType;
        Id = id;
        UpdateRoomName();  // Oda ismi, tipi de içerecek şekilde güncelleniyor
    }

    public void UpdateRoomName()
    {
        Name = $"Room_{X}_{Y}_{RoomType}"; // Oda adında oda tipi de var
    }
}


public class MapGenerator : MonoBehaviour, IDataPersistence
{
    public GameObject roomButtonPrefab;
    public RectTransform contentTransform;

    public GameObject lineSegmentPrefab;
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 16;
    [SerializeField] private int minPaths = 3;
    [SerializeField] private int maxPaths = 4;

    [SerializeField] private float verticalOffset = 100f;
    [SerializeField] private float horizontalOffset = 100f;

    public Sprite monsterRoomSprite, eventRoomSprite, eliteMonsterRoomSprite, restSiteRoomSprite, merchantRoomSprite, treasureRoomSprite, bossRoomSprite;
    public EnemySO monsterEnemySO, eliteEnemySO, bossEnemySO;

    private RoomManager roomManager;
    private Room[,] grid;
    private System.Random random = new System.Random();
    private int extendedHeight;

    public List<Room> remainingRooms = new List<Room>();
    public string activeRoomName;

    private Dictionary<string, RoomInteraction> roomInteractionMap = new Dictionary<string, RoomInteraction>();


    private void Start()
    {
        remainingRooms.Clear();
        roomManager = new RoomManager(); // RoomManager'ı başlat
        extendedHeight = height + 1;

        StartCoroutine(DrawRoomsCo());
    }

    public IEnumerator DrawRoomsCo()
    {
        yield return null;

        if (FindObjectOfType<DataCarrier>().pathwayRooms.Count > 0)
        {
            remainingRooms.Clear();
            remainingRooms.AddRange(FindObjectOfType<DataCarrier>().pathwayRooms);
            activeRoomName = FindObjectOfType<DataCarrier>().currentRoomName;
        }

        GenerateMap();

        if (FindObjectOfType<DataCarrier>().pathwayRooms.Count > 0)
        {
            AssignRemainingRoomLocations(); 
        }
        else
        {
            AssignRoomLocations();
        }

        if(activeRoomName != "")
        {
            SetCurrentRoom(roomInteractionMap[activeRoomName]);
            SetAllRoomsUnclickable();

            List<Room> connectedRooms = roomManager.GetConnectedRooms(roomInteractionMap[activeRoomName].Room);

            foreach (var connectedRoom in connectedRooms)
            {
                if (connectedRoom.Y > roomInteractionMap[activeRoomName].Room.Y)  // Ensure movement is only upwards
                {
                    if (roomInteractionMap.TryGetValue(connectedRoom.Name, out RoomInteraction nextRoom))
                    {
                        nextRoom.SetClickable(true);
                        nextRoom.UpdateClickableVisuals(); // Update visuals for clickable rooms
                    }
                }
            }

            activeRoomName = roomInteractionMap[activeRoomName].Room.Name; // Use the room's name as the string identifier
            FindObjectOfType<DataCarrier>().currentRoomName = activeRoomName;
            FindObjectOfType<GameController>().SaveGame();
            roomInteractionMap[activeRoomName].UpdateClickableVisuals();
        }
        FindObjectOfType<DataCarrier>().pathwayRooms.Clear();
        FindObjectOfType<DataCarrier>().pathwayRooms.AddRange(remainingRooms);
        FindObjectOfType<GameController>().SaveGame();
    }

    private void GenerateMap()
    {
        grid = new Room[width, extendedHeight];

        if (remainingRooms.Count == 0)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < extendedHeight; y++)
                {
                    string name = $"Room_{x}_{y}";
                    var room = new Room(x, y, x + y * width, name, RoomType.None); // Oda tipi şu an None olarak ayarlandı
                    grid[x, y] = room;
                    roomManager.AddRoom(room); // Odayı yöneticisine ekle
                }
            }

            // Generate starting paths
            int pathCount = random.Next(minPaths, maxPaths + 1);
            List<Room> startingRooms = new List<Room>();
            for (int i = 0; i < pathCount; i++)
            {
                Room startRoom;
                do
                {
                    startRoom = grid[random.Next(width), 0];
                } while (startingRooms.Contains(startRoom));
                startingRooms.Add(startRoom);
            }

            // Connect starting rooms to next floor
            foreach (Room startRoom in startingRooms)
            {
                ConnectToNextFloor(startRoom, 0);
            }

            // Odaların türlerini atama
            AssignRoomLocations();
            UpdateRoomNamesWithTypes();  // Oda isimlerini güncelle
        }
        else
        {
            // remainingRooms listesindeki odaları grid'e yerleştirme süreci
            AssignRemainingRoomLocations();
            UpdateRoomNamesWithTypes();  // Oda isimlerini güncelle
            EraseRemainingRooms();
        }

        // Gereksiz odaları temizliyoruz
        RemoveUnconnectedRooms();

        // Harita oluşturma işlemi tamamlandıktan sonra boss odasını yerleştiriyoruz
        AllocateBossRoom();

        // Odaları görsel olarak oluşturuyoruz
        GenerateRoomButtons();
    }

    private void UpdateRoomNamesWithTypes()
    {
        foreach (Room room in grid)
        {
            if (room != null)
            {
                room.UpdateRoomName();  // Oda isimlerini RoomType ile güncelle
            }
        }
    }


    private void ConnectToNextFloor(Room room, int currentFloor)
    {
        if (currentFloor >= extendedHeight - 1)
            return;

        int nextFloor = currentFloor + 1;

        // Eğer son kata ulaştıysak boss odasını yerleştir
        if (nextFloor == extendedHeight - 1)
        {
            Room bossRoom = grid[width / 2, nextFloor];
            roomManager.AddConnection(room, bossRoom);
            return;
        }

        List<Room> possibleConnections = new List<Room>();

        // Eğer remainingRooms doluysa, sadece remainingRooms içindeki odalarla bağlantı kur
        if (remainingRooms.Count > 0)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = room.X + dx;
                if (nx >= 0 && nx < width)
                {
                    Room nextRoom = grid[nx, nextFloor];
                    if (nextRoom != null && remainingRooms.Contains(nextRoom)) // Sadece remainingRooms odalarını ekle
                    {
                        possibleConnections.Add(nextRoom);
                    }
                }
            }
        }
        // Eğer remainingRooms boşsa, normal bağlantı kur
        else
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = room.X + dx;
                if (nx >= 0 && nx < width)
                {
                    Room nextRoom = grid[nx, nextFloor];
                    if (nextRoom != null) // Herhangi bir odayla bağlantı kurabilir
                    {
                        possibleConnections.Add(nextRoom);
                    }
                }
            }
        }

        // Olası bağlantılar arasında rastgele bir oda seç ve bağlantı kur
        if (possibleConnections.Count > 0)
        {
            Room nextRoom = possibleConnections[random.Next(possibleConnections.Count)];
            roomManager.AddConnection(room, nextRoom);
            ConnectToNextFloor(nextRoom, nextFloor);
        }
    }

    private void AssignRemainingRoomLocations()
    {
        // HashSet, grid üzerinde zaten var olan odaların isimlerini takip eder.
        HashSet<string> processedRoomNames = new HashSet<string>();

        foreach (Room room in remainingRooms)
        {
            // Odaların isimlerinden (örneğin "Room_1_0_Monster") x, y koordinatlarını ve RoomType'ı çıkart
            string[] nameParts = room.Name.Split('_');
            if (nameParts.Length == 4 &&
                int.TryParse(nameParts[1], out int x) &&
                int.TryParse(nameParts[2], out int y) &&
                Enum.TryParse(nameParts[3], out RoomType roomType))
            {
                room.X = x;
                room.Y = y;
                room.RoomType = roomType;

                // Eğer koordinatlar grid sınırları içerisindeyse grid'e yerleştir
                if (room.X >= 0 && room.X < width && room.Y >= 0 && room.Y < extendedHeight)
                {
                    // Eğer gridde zaten aynı isimde bir oda varsa, önceki öğeyi listeden çıkar
                    string roomName = room.Name;
                    if (grid[room.X, room.Y] != null)
                    {
                        processedRoomNames.Add(roomName);
                    }
                    else
                    {
                        grid[room.X, room.Y] = room;
                        roomManager.AddRoom(room); // Odayı yöneticisine ekle
                    }
                }
            }
            else
            {
                Debug.LogError($"Invalid room name format: {room.Name}");
            }
        }

        // Kalan odalar için bağlantı oluştur
        foreach (Room room in remainingRooms)
        {
            if (room.Y < extendedHeight - 1)
            {
                ConnectToNextFloor(room, room.Y); // Bağlantıları kalan odalar arasında oluştur
            }
        }
    }


    private void AssignRoomLocations()
    {
        // Assign specific room types to specific floors
        foreach (Room room in GetRoomsOnFloor(0)) 
        {
            room.RoomType = RoomType.Monster; 
        }
        
        foreach (Room room in GetRoomsOnFloor(8)) 
        { 
            room.RoomType = RoomType.Treasure; 
        }
        
        foreach (Room room in GetRoomsOnFloor(15)) 
        { 
            room.RoomType = RoomType.RestSite; 
        }

        // Iterate over the grid and assign room types
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = grid[x, y];

                if (room != null)
                {
                    // If the room has no type assigned, give it a random type based on floor level
                    if (room.RoomType == RoomType.None)
                    {
                        room.RoomType = GetRandomRoomType(y);
                    }
                }
            }
        }
    }

    public void EraseRemainingRooms()
    {
        // İkinci yarıyı temizle
        if (remainingRooms.Count > 0)
        {
            remainingRooms.RemoveRange(0, remainingRooms.Count);
        }
    }

    private RoomType GetRandomRoomType(int floor)
    {
        int rand = random.Next(100);
        if (rand < 45) return RoomType.Monster;
        if (rand < 67) return RoomType.Event;
        if (rand < 77 && floor >= 5) return RoomType.EliteMonster;
        if (rand < 89 && floor >= 5) return RoomType.RestSite;
        if (rand < 94) return RoomType.Merchant;
        return RoomType.Treasure;
    }

    private void AllocateBossRoom()
    {
        int bossRoomX = width / 2;
        int bossRoomY = extendedHeight - 1;

        string name = $"Room_{bossRoomX}_{bossRoomY}";
        Room bossRoom = new Room(bossRoomX, bossRoomY, bossRoomX + bossRoomY * width, name, RoomType.Boss);

        grid[bossRoomX, bossRoomY] = bossRoom;

        UpdateRoomNamesWithTypes();

        roomManager.AddRoom(bossRoom); // Boss odasını yöneticisine ekle

        foreach (Room room in GetRoomsOnFloor(extendedHeight - 2))
        {
            if (room != null)
            {
                roomManager.AddConnection(room, bossRoom);
            }
        } 
    }

    private List<Room> GetRoomsOnFloor(int floor)
    {
        List<Room> rooms = new List<Room>();
        for (int x = 0; x < width; x++)
        {
            if (grid[x, floor] != null)
            {
                rooms.Add(grid[x, floor]);
            }
        }
        return rooms;
    }

    private void RemoveUnconnectedRooms()
    {
        for (int y = 0; y < extendedHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = grid[x, y];
                if (room != null)
                {
                    if (y == extendedHeight - 2)
                    {
                        bool hasConnectionToLowerFloor = false;
                        bool hasConnectionToBossRoom = false;

                        foreach (RoomConnection connection in roomManager.GetConnections())
                        {
                            if (connection.RoomA == room && connection.RoomB.Y == extendedHeight - 3)
                            {
                                hasConnectionToLowerFloor = true;
                            }
                            if (connection.RoomB == room && connection.RoomA.Y == extendedHeight - 3)
                            {
                                hasConnectionToLowerFloor = true;
                            }
                            if (connection.RoomA == room && connection.RoomB.Y == extendedHeight - 1)
                            {
                                hasConnectionToBossRoom = true;
                            }
                            if (connection.RoomB == room && connection.RoomA.Y == extendedHeight - 1)
                            {
                                hasConnectionToBossRoom = true;
                            }
                        }

                        if (!hasConnectionToLowerFloor || !hasConnectionToBossRoom)
                        {
                            grid[x, y] = null;
                        }
                        else
                        {
                            remainingRooms.Add(room);
                        }
                    }
                    else if (roomManager.GetConnections().FindAll(c => c.RoomA == room || c.RoomB == room).Count == 0)
                    {
                        grid[x, y] = null;
                    }
                    else
                    {
                        remainingRooms.Add(room);
                    }
                }
            }

            //FindObjectOfType<GameController>().SaveGame();
        }
    }

    private void GenerateRoomButtons()
    {
        float mapWidth = width * horizontalOffset;
        float mapHeight = height * verticalOffset;

        float contentWidth = contentTransform.rect.width;
        float contentHeight = contentTransform.rect.height;

        float horizontalCenterOffset = (contentWidth - mapWidth) / 2;
        float verticalCenterOffset = (contentHeight - mapHeight) / 2;

        foreach (Room room in grid)
        {
            if (room != null && room.RoomType != RoomType.None)
            {
                GameObject roomButton = Instantiate(roomButtonPrefab, contentTransform);

                RectTransform buttonRect = roomButton.GetComponent<RectTransform>();
                buttonRect.anchoredPosition = new Vector2(
                    room.X * horizontalOffset + horizontalCenterOffset,
                    room.Y * verticalOffset + verticalCenterOffset
                );

                Image buttonImage = roomButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = GetSpriteForRoomType(room.RoomType);
                }

                roomButton.name = $"Room {room.X}, Floor {room.Y}, Type {room.RoomType}";

                RoomInteraction roomInteraction = roomButton.GetComponent<RoomInteraction>();
                if (roomInteraction != null)
                {
                    roomInteraction.InitializeRoom(room);
                    roomInteractionMap[room.Name] = roomInteraction; // Add to dictionary
                    roomInteraction.button.interactable = room.Y == 0;

                    Button button = roomButton.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnRoomClicked(roomInteraction));
                    }
                }

                // Draw connections
                CreateConnections(room, buttonRect);
            }
        }
    }

    private void CreateConnections(Room room, RectTransform roomButtonRect)
    {
        foreach (RoomConnection connection in roomManager.GetConnections())
        {
            if (connection.RoomA == room || connection.RoomB == room)
            {
                Room connectedRoom = connection.RoomA == room ? connection.RoomB : connection.RoomA;
                if (connectedRoom != null)
                {
                    RoomInteraction connectedRoomInteraction = GetRoomInteraction(connectedRoom);

                    if (connectedRoomInteraction != null)
                    {
                        RectTransform connectedRoomRect = connectedRoomInteraction.GetComponent<RectTransform>();

                        // Çizgiyi oluştur
                        GameObject lineSegment = Instantiate(lineSegmentPrefab, contentTransform);
                        RectTransform lineRect = lineSegment.GetComponent<RectTransform>();

                        // Çizgiyi hiyerarşide geriye taşı
                        lineSegment.transform.SetSiblingIndex(0);

                        // İki oda arasındaki farkı hesapla
                        Vector2 direction = connectedRoomRect.anchoredPosition - roomButtonRect.anchoredPosition;
                        float distance = direction.magnitude;

                        // Çizgiyi ortasına yerleştir
                        lineRect.anchoredPosition = roomButtonRect.anchoredPosition + direction / 2;

                        // Çizginin genişliğini ayarla
                        lineRect.sizeDelta = new Vector2(distance, 2f); // Yükseklik olarak 2f değerini kullanarak kalınlığı düşürüyoruz

                        // Çizgiyi doğru yöne döndür
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        lineRect.rotation = Quaternion.Euler(0, 0, angle);

                        // Çizgi rengini ayarlamak isterseniz
                        Image lineImage = lineSegment.GetComponent<Image>();
                        if (lineImage != null)
                        {
                            lineImage.color = Color.gray; // Renk tercihinizi burada ayarlayabilirsiniz
                        }
                    }
                }
            }
        }
    }

    private Sprite GetSpriteForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Monster: return monsterRoomSprite;
            case RoomType.Event: return eventRoomSprite;
            case RoomType.EliteMonster: return eliteMonsterRoomSprite;
            case RoomType.RestSite: return restSiteRoomSprite;
            case RoomType.Merchant: return merchantRoomSprite;
            case RoomType.Treasure: return treasureRoomSprite;
            case RoomType.Boss: return bossRoomSprite;
            default: return null;  // Eğer bir sprite atanmadıysa null döner.
        }
    }

    private Button currentRoomButton;

    public static MapGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void OnRoomClicked(RoomInteraction clickedRoom)
    {
        SetCurrentRoom(clickedRoom);

        // Get the room type
        RoomType roomType = clickedRoom.Room.RoomType;

        // Determine which EnemySO to assign based on the room type
        EnemySO enemySOToAssign = null;
        switch (roomType)
        {
            case RoomType.Monster:
                enemySOToAssign = monsterEnemySO;
                break;
            case RoomType.EliteMonster:
                enemySOToAssign = eliteEnemySO;
                break;
            case RoomType.Boss:
                enemySOToAssign = bossEnemySO;
                break;
                // Add more cases for other room types if necessary
        }

        // Assign the selected EnemySO to DataCarrier
        if (enemySOToAssign != null)
        {
            FindObjectOfType<DataCarrier>().enemy = enemySOToAssign;
        }
        else
        {
            if(roomType != RoomType.RestSite)
            {
                Debug.LogWarning("No EnemySO assigned for room type: " + roomType);
            } 
        }

        SetAllRoomsUnclickable();

        List<Room> connectedRooms = roomManager.GetConnectedRooms(clickedRoom.Room);

        foreach (var connectedRoom in connectedRooms)
        {
            if (connectedRoom.Y > clickedRoom.Room.Y)  // Ensure movement is only upwards
            {
                if (roomInteractionMap.TryGetValue(connectedRoom.Name, out RoomInteraction nextRoom))
                {
                    nextRoom.SetClickable(true);
                    nextRoom.UpdateClickableVisuals(); // Update visuals for clickable rooms
                }
            }
        }

        activeRoomName = clickedRoom.Room.Name; // Use the room's name as the string identifier
        FindObjectOfType<DataCarrier>().currentRoomName = activeRoomName;
        FindObjectOfType<GameController>().SaveGame();
        clickedRoom.UpdateClickableVisuals();

        if(clickedRoom.Room.RoomType == RoomType.RestSite)
        {
            SceneManager.LoadScene("Rest Site");
        }
        else if(clickedRoom.Room.RoomType == RoomType.Merchant)
        {
            SceneManager.LoadScene("Merchant");
        }
        else
        {
            SceneManager.LoadScene("Battle 1");
        } 
    }

    private RoomInteraction GetRoomInteraction(Room room)
    {
        foreach (var interaction in FindObjectsOfType<RoomInteraction>())
        {
            if (interaction.Room == room)
            {
                return interaction;
            }
        }
        return null;
    }

    private void SetCurrentRoom(RoomInteraction clickedRoom)
    {
        if (currentRoomButton != null)
        {
            currentRoomButton.interactable = false;  // Disable interaction for the previous room button
        }

        currentRoomButton = clickedRoom.GetComponent<Button>();
        currentRoomButton.interactable = true;  // Enable interaction for the current room button
        currentRoomButton.GetComponent<RoomInteraction>().BlinkSprite(); // Trigger any visual effects if needed

        Debug.Log("Current room set to: " + clickedRoom.Room.Name);
    }


    private void SetAllRoomsUnclickable()
    {
        Button[] allRooms = FindObjectsOfType<Button>();
        foreach (var roomButton in allRooms)
        {
            roomButton.interactable = false;  // Disable interaction for all room buttons
        }
        Debug.Log("All rooms set to unclickable.");
    }

    public void LoadData(PlayerData data)
    {
        remainingRooms.Clear();
        remainingRooms.AddRange(data.rooms);
        activeRoomName = data.currentRoom;
    }

    public void SaveData(PlayerData data)
    {
        data.rooms.Clear();
        data.rooms.AddRange(remainingRooms);
        data.currentRoom = activeRoomName;
    }
}