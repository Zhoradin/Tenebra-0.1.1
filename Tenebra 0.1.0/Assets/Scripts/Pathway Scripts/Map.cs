using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public GameObject roomPrefab;    // Prefab for room instances
    public Room[,] Grid { get; private set; }    // 2D array of rooms

    public void InitializeMap(int width, int height)
    {
        Grid = new Room[width, height];    // Initialize the grid
        GenerateMapTemplate();    // Generate the initial map template
        GeneratePaths();    // Create paths between rooms
        AssignLocations();    // Assign locations to rooms
        AllocateBossRoom();    // Allocate the boss room
    }

    // Generating Map Template
    private void GenerateMapTemplate()
    {
        System.Random rand = new System.Random();
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                // Instantiate the room prefab at the appropriate position
                GameObject roomObject = Instantiate(roomPrefab, new Vector3(x * 2, y * 2, 0), Quaternion.identity);
                Room room = roomObject.GetComponent<Room>();
                room.X = x;
                room.Y = y;
                Grid[x, y] = room;    // Add the room to the grid
            }
        }

        // Create a list of rooms on the first floor
        List<Room> firstFloorRooms = new List<Room>();
        for (int x = 0; x < 7; x++)
        {
            firstFloorRooms.Add(Grid[x, 0]);
        }

        // Randomly select two different starting rooms
        Room startRoom1 = firstFloorRooms[rand.Next(firstFloorRooms.Count)];
        Room startRoom2;
        do
        {
            startRoom2 = firstFloorRooms[rand.Next(firstFloorRooms.Count)];
        } while (startRoom1 == startRoom2);
    }


    // Generating Paths
    private void GeneratePaths()
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < 14; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Room currentRoom = Grid[x, y];
                if (currentRoom != null)
                {
                    int nextY = y + 1;
                    List<Room> potentialRooms = new List<Room>();
                    if (x > 0) potentialRooms.Add(Grid[x - 1, nextY]);
                    potentialRooms.Add(Grid[x, nextY]);
                    if (x < 6) potentialRooms.Add(Grid[x + 1, nextY]);

                    Room nextRoom = potentialRooms[rand.Next(potentialRooms.Count)];
                    currentRoom.ConnectedRooms.Add(nextRoom);
                    nextRoom.ConnectedRooms.Add(currentRoom);
                }
            }
        }

        // Remove unpaired rooms
        for (int y = 0; y < 15; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Room room = Grid[x, y];
                if (room != null && room.ConnectedRooms.Count == 0)
                {
                    Destroy(room.gameObject);
                    Grid[x, y] = null;
                }
            }
        }
    }


    // Assigning Different Room Types !--add new types below this --!
    public RoomTypeSO monsterRoomType;
    public RoomTypeSO eventRoomType;
    public RoomTypeSO eliteMonsterRoomType;
    public RoomTypeSO restSiteRoomType;
    public RoomTypeSO merchantRoomType;
    public RoomTypeSO treasureRoomType;

    private void AssignLocations()
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < 15; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Room room = Grid[x, y];
                if (room != null)
                {
                    if (y == 0)
                    {
                        room.RoomType = monsterRoomType;
                    }
                    else if (y == 8)
                    {
                        room.RoomType = treasureRoomType;
                    }
                    else if (y == 14)
                    {
                        room.RoomType = restSiteRoomType;
                    }
                    else
                    {
                        double randValue = rand.NextDouble() * 100;
                        if (randValue < 45) room.RoomType = monsterRoomType;
                        else if (randValue < 67) room.RoomType = eventRoomType;
                        else if (randValue < 77) room.RoomType = eliteMonsterRoomType;
                        else if (randValue < 89) room.RoomType = restSiteRoomType;
                        else if (randValue < 94) room.RoomType = merchantRoomType;
                        else room.RoomType = treasureRoomType;

                        // Reassign if rules are broken
                        // Implement rule checks here...
                    }
                }
            }
        }
    }

    public RoomTypeSO bossRoomType;

    private void AllocateBossRoom()
    {
        System.Random rand = new System.Random();
        List<Room> topFloorRooms = new List<Room>();
        for (int x = 0; x < 7; x++)
        {
            Room room = Grid[x, 14];
            if (room != null && room.RoomType == restSiteRoomType)
            {
                topFloorRooms.Add(room);
            }
        }

        // Instantiate the boss room in the middle of the top floor
        GameObject bossRoomObject = Instantiate(roomPrefab, new Vector3(3 * 2, 15 * 2, 0), Quaternion.identity);
        Room bossRoom = bossRoomObject.GetComponent<Room>();
        bossRoom.X = 3;
        bossRoom.Y = 15;
        bossRoom.RoomType = bossRoomType;

        // Connect the boss room to all rest site rooms on the top floor
        foreach (Room room in topFloorRooms)
        {
            room.ConnectedRooms.Add(bossRoom);
            bossRoom.ConnectedRooms.Add(room);
        }
    }

}
