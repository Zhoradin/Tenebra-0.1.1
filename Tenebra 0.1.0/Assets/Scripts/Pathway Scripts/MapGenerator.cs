using System;
using System.Collections.Generic;
using UnityEngine;
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

public class Room
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public RoomType RoomType { get; set; }
    public List<Room> Connections { get; private set; }

    public Room(int x, int y)
    {
        X = x;
        Y = y;
        RoomType = RoomType.None;
        Connections = new List<Room>();
    }

    public void Connect(Room other)
    {
        if (!Connections.Contains(other))
        {
            Connections.Add(other);
            other.Connect(this); // Ensure bidirectional connection
        }
    }
}

public class MapGenerator : MonoBehaviour
{
    public GameObject roomButtonPrefab; // The button prefab to use for rooms
    public RectTransform contentTransform; // The content RectTransform inside the Scroll View

    [SerializeField] private int width = 7;
    [SerializeField] private int height = 16;
    [SerializeField] private int minPaths = 3;
    [SerializeField] private int maxPaths = 4;

    [SerializeField] private float verticalOffset = 100f; // Adjust this to control spacing
    [SerializeField] private float horizontalOffset = 100f; // Adjust this to control spacing

    private Room[,] grid;
    private System.Random random = new System.Random();
    private int extendedHeight;

    private void Start()
    {
        extendedHeight = height + 1; // Add extra floor for boss room
        GenerateMap();
        AssignRoomLocations();
        RemoveUnconnectedRooms();
        AllocateBossRoom();
        GenerateRoomButtons();
    }

    private void GenerateMap()
    {
        grid = new Room[width, extendedHeight];

        // Create rooms
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < extendedHeight; y++)
            {
                grid[x, y] = new Room(x, y);
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
    }

    private void ConnectToNextFloor(Room room, int currentFloor)
    {
        if (currentFloor >= extendedHeight - 1)
            return;

        int nextFloor = currentFloor + 1;

        if (nextFloor == extendedHeight - 1)
        {
            Room bossRoom = grid[width / 2, nextFloor];
            room.Connect(bossRoom);
            return;
        }

        List<Room> possibleConnections = new List<Room>();
        for (int dx = -1; dx <= 1; dx++)
        {
            int nx = room.X + dx;
            if (nx >= 0 && nx < width)
            {
                possibleConnections.Add(grid[nx, nextFloor]);
            }
        }

        Room nextRoom = possibleConnections[random.Next(possibleConnections.Count)];
        room.Connect(nextRoom);
        ConnectToNextFloor(nextRoom, nextFloor);
    }

    private void AssignRoomLocations()
    {
        foreach (Room room in GetRoomsOnFloor(0)) { room.RoomType = RoomType.Monster; }
        foreach (Room room in GetRoomsOnFloor(8)) { room.RoomType = RoomType.Treasure; }
        foreach (Room room in GetRoomsOnFloor(15)) { room.RoomType = RoomType.RestSite; }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = grid[x, y];
                if (room.RoomType == RoomType.None)
                {
                    room.RoomType = GetRandomRoomType(y);
                }
            }
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

        Room bossRoom = new Room(bossRoomX, bossRoomY)
        {
            RoomType = RoomType.Boss
        };
        grid[bossRoomX, bossRoomY] = bossRoom;

        foreach (Room room in GetRoomsOnFloor(extendedHeight - 2))
        {
            if (room != null)
            {
                room.Connect(bossRoom);
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

                        foreach (Room connection in room.Connections)
                        {
                            if (connection.Y == extendedHeight - 3)
                            {
                                hasConnectionToLowerFloor = true;
                                break;
                            }
                        }

                        if (room.Connections.Exists(r => r.Y == extendedHeight - 1))
                        {
                            hasConnectionToBossRoom = true;
                        }

                        if (!hasConnectionToLowerFloor || !hasConnectionToBossRoom)
                        {
                            grid[x, y] = null;
                        }
                    }
                    else if (room.Connections.Count == 0)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    private void GenerateRoomButtons()
{
    // Calculate map center offset
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
            // Instantiate the button prefab
            GameObject roomButton = Instantiate(roomButtonPrefab, contentTransform);

            // Set the position of the button based on the grid
            RectTransform buttonRect = roomButton.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = new Vector2(
                room.X * horizontalOffset + horizontalCenterOffset, 
                room.Y * verticalOffset + verticalCenterOffset
            );

            // Customize the button based on the room's properties
            Image buttonImage = roomButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = GetColorForRoomType(room.RoomType);
            }

            // Set the name of the button
            roomButton.name = $"Room {room.X}, Floor {room.Y}, Type {room.RoomType}";

            // Add interaction logic to the button
            RoomInteraction roomInteraction = roomButton.GetComponent<RoomInteraction>();
            if (roomInteraction != null)
            {
                roomInteraction.InitializeRoom(room);
                roomInteraction.button.interactable = room.Y == 0; // Only starting rooms are clickable

                Button button = roomButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnRoomClicked(roomInteraction));
                }
            }
        }
    }
}


    private void DrawConnectionLines()
    {
        // foreach (Room room in grid)
        // {
        //     if (room != null && room.RoomType != RoomType.None)
        //     {
        //         foreach (Room connectedRoom in room.Connections)
        //         {
        //             if (connectedRoom.Y > room.Y)
        //             {
        //                 GameObject lineObj = new GameObject("ConnectionLine");
        //                 LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        //                 lr.startWidth = 0.05f;
        //                 lr.endWidth = 0.05f;
        //                 lr.positionCount = 2;
        //                 lr.SetPosition(0, new Vector3(room.X * horizontalOffset, room.Y * verticalOffset, 0));
        //                 lr.SetPosition(1, new Vector3(connectedRoom.X * horizontalOffset, connectedRoom.Y * verticalOffset, 0));

        //                 lr.startColor = Color.black;
        //                 lr.endColor = Color.black;
        //                 lr.material = new Material(Shader.Find("Sprites/Default"));
        //                 lr.sortingOrder = -2;

        //                 lineObj.transform.SetParent(contentTransform, false);
        //             }
        //         }
        //     }
        // }
    }

    private Color GetColorForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Monster: return Color.red;
            case RoomType.Event: return Color.green;
            case RoomType.EliteMonster: return Color.magenta;
            case RoomType.RestSite: return Color.blue;
            case RoomType.Merchant: return Color.yellow;
            case RoomType.Treasure: return Color.cyan;
            case RoomType.Boss: return Color.black;
            default: return Color.white;
        }
    }



    // Add this variable to keep track of the currently selected room button
    private Button currentRoomButton;

    // Singleton instance
    public static MapGenerator Instance { get; private set; }

    private void Awake()
    {
        // Ensure there's only one instance of MapGenerator
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
        Debug.Log("OnRoomClicked method called.");

        // Set the clicked room as the current room
        SetCurrentRoom(clickedRoom);

        // Set all other rooms unclickable
        SetAllRoomsUnclickable();

        // Allow moving only to connected rooms on higher floors
        foreach (var connection in clickedRoom.Room.Connections)
        {
            if (connection.Y > clickedRoom.Room.Y)  // Ensure movement is only upwards
            {
                RoomInteraction nextRoom = GetRoomInteraction(connection);
                if (nextRoom != null)
                {
                    nextRoom.SetClickable(true);
                    nextRoom.UpdateClickableVisuals(); // Update visuals for clickable rooms
                }
            }
        }

        // Update visuals for the clicked room as well
        clickedRoom.UpdateClickableVisuals();
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

        // Update the current room button reference
        currentRoomButton = clickedRoom.GetComponent<Button>();
        currentRoomButton.interactable = true;  // Enable interaction for the current room button
        currentRoomButton.GetComponent<RoomInteraction>().BlinkSprite(); // Trigger any visual effects if needed

        Debug.Log("Current room set to: " + clickedRoom.Room.X + ", " + clickedRoom.Room.Y);
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
}
