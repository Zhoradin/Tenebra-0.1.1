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
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 15;
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

    [SerializeField] private bool showNullSpheres = false;

    private Room[,] grid;
    private System.Random random = new System.Random();

    void Start()
    {
        GenerateMap();
        AssignRoomLocations();
        AllocateBossRoom();
        RemoveUnconnectedRooms();
    }

    private void GenerateMap()
    {
        grid = new Room[width, height + 1]; // Allow room for boss room

        // Create rooms
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
        if (currentFloor >= height - 1)
            return;

        int nextFloor = currentFloor + 1;
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
        foreach (Room room in GetRoomsOnFloor(14)) { room.RoomType = RoomType.RestSite; }

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
        Room bossRoom = new Room(width / 2, height);
        bossRoom.RoomType = RoomType.Boss;
        grid[width / 2, height] = bossRoom;

        List<Room> connectedRooms = new List<Room>();
        foreach (Room room in GetRoomsOnFloor(14))
        {
            if (room.Connections.Count > 0)
            {
                room.Connect(bossRoom);
                connectedRooms.Add(room);
            }
        }

        // Set unconnected rooms on 15th floor to null
        for (int x = 0; x < width; x++)
        {
            if (!connectedRooms.Contains(grid[x, 14]))
            {
                grid[x, 14] = null;
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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = grid[x, y];
                if (room != null && room.Connections.Count == 0)
                {
                    grid[x, y] = null; // Set unconnected room to null
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int y = 0; y < height + 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Room room = grid[x, y];
                if (room != null)
                {
                    Vector3 position = new Vector3(x, y, 0);
                    Gizmos.color = GetColorForRoomType(room.RoomType);
                    Gizmos.DrawSphere(position, 0.2f);

                    foreach (Room connection in room.Connections)
                    {
                        Vector3 connectionPosition = new Vector3(connection.X, connection.Y, 0);
                        Gizmos.DrawLine(position, connectionPosition);
                    }
                }
                else if (showNullSpheres)
                {
                    Vector3 position = new Vector3(x, y, 0);
                    Gizmos.color = defaultColor;
                    Gizmos.DrawSphere(position, 0.1f); // Draw smaller sphere for null rooms
                }
            }
        }
    }

    private Color GetColorForRoomType(RoomType roomType)
    {
        switch (roomType)
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
