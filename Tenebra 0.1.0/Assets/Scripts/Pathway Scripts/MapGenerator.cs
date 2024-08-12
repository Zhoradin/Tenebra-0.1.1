using System;
using System.Collections.Generic;
using UnityEngine;

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
    public SpriteRenderer SpriteRenderer { get; set; }

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

[Serializable]
public struct RoomTypeSprite
{
    public RoomType roomType;
    public Sprite sprite;
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 16;
    [SerializeField] private int minPaths = 3;
    [SerializeField] private int maxPaths = 4;

    [SerializeField] private Color monsterColor = Color.red;
    [SerializeField] private Color eventColor = Color.blue;
    [SerializeField] private Color eliteMonsterColor = Color.magenta;
    [SerializeField] private Color restSiteColor = Color.green;
    [SerializeField] private Color merchantColor = Color.yellow;
    [SerializeField] private Color treasureColor = Color.cyan;
    [SerializeField] private Color bossColor = Color.black;
    [SerializeField] private Color defaultColor = Color.white;

    [SerializeField] private List<RoomTypeSprite> roomTypeSprites;

    [SerializeField] private bool showNullSpheres = false; // Reintroduced

    private Room[,] grid;
    private System.Random random = new System.Random();
    private int extendedHeight;

    void Start()
{
    extendedHeight = height + 1; // Add extra floor for boss room
    GenerateMap();
    AssignRoomLocations();
    RemoveUnconnectedRooms(); // Ensure rooms are cleaned up before boss room allocation
    AllocateBossRoom();      // Allocate and connect boss room
    AssignRoomSprites();
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

    // Ensure that only the boss room is on floor 16
    if (nextFloor == extendedHeight - 1) // This is the boss floor
    {
        Room bossRoom = grid[width / 2, nextFloor]; // Assuming the boss room is always centered
        room.Connect(bossRoom);
        return; // Stop further connections for the boss room
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
        // Assign predefined locations
        foreach (Room room in GetRoomsOnFloor(0)) { room.RoomType = RoomType.Monster; }
        foreach (Room room in GetRoomsOnFloor(8)) { room.RoomType = RoomType.Treasure; }
        foreach (Room room in GetRoomsOnFloor(15)) { room.RoomType = RoomType.RestSite; }

        // Randomly assign remaining rooms
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
        // Determine the position of the boss room
        int bossRoomX = width / 2;
        int bossRoomY = extendedHeight - 1; // One floor above the grid

        // Create and assign the boss room
        Room bossRoom = new Room(bossRoomX, bossRoomY)
        {
            RoomType = RoomType.Boss
        };
        grid[bossRoomX, bossRoomY] = bossRoom;

        // Connect the boss room to all non-null rooms on the highest regular floor
        foreach (Room room in GetRoomsOnFloor(extendedHeight - 2)) // Previous top floor before boss room
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
                if (y == extendedHeight - 2) // Check rooms on the floor below the boss room
                {
                    bool hasConnectionToLowerFloor = false;
                    bool hasConnectionToBossRoom = false;

                    // Check if the room has a connection to the lower floor (floor 13)
                    foreach (Room connection in room.Connections)
                    {
                        if (connection.Y == extendedHeight - 3) // Floor 13
                        {
                            hasConnectionToLowerFloor = true;
                            break;
                        }
                    }

                    // Check if the room has a connection to the boss room
                    if (room.Connections.Exists(r => r.Y == extendedHeight - 1)) // Boss room
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


    private void AssignRoomSprites()
    {
        foreach (Room room in grid)
        {
            if (room != null && room.RoomType != RoomType.None)
            {
                RoomTypeSprite rts = roomTypeSprites.Find(r => r.roomType == room.RoomType);
                if (rts.sprite != null)
                {
                    GameObject roomObj = new GameObject($"Room {room.X} Floor {room.Y} Room Type: {room.RoomType}");
                    roomObj.transform.position = new Vector3(room.X, room.Y, 0);
                    SpriteRenderer sr = roomObj.AddComponent<SpriteRenderer>();
                    sr.sprite = rts.sprite;
                    room.SpriteRenderer = sr;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int y = 0; y < extendedHeight; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = grid[x, y];
                Vector3 position = new Vector3(x, y, 0);
                Gizmos.color = room != null ? GetColorForRoomType(room.RoomType) : Color.grey;

                if (room != null)
                {
                    Gizmos.DrawSphere(position, 0.15f);

                    foreach (Room connection in room.Connections)
                    {
                        Vector3 connectionPosition = new Vector3(connection.X, connection.Y, 0);
                        Gizmos.DrawLine(position, connectionPosition);
                    }
                }
                else if (showNullSpheres)
                {
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }
    }

    private Color GetColorForRoomType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Monster: return monsterColor;
            case RoomType.Event: return eventColor;
            case RoomType.EliteMonster: return eliteMonsterColor;
            case RoomType.RestSite: return restSiteColor;
            case RoomType.Merchant: return merchantColor;
            case RoomType.Treasure: return treasureColor;
            case RoomType.Boss: return bossColor;
            default: return defaultColor;
        }
    }
}