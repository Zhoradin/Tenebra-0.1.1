using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public GameObject roomPrefab;
    
    
    private Room[,] Grid = new Room[7, 15];

    private void Start()
    {
        GenerateMap();
        AssignRoomTypes();
        AllocateBossRoom();
    }

    private void GenerateMap()
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (ShouldGenerateRoom(x, y))
                {
                    GameObject roomObject = Instantiate(roomPrefab, new Vector3(x * 2, y * 2, 0), Quaternion.identity);
                    Room room = roomObject.GetComponent<Room>();
                    room.X = x;
                    room.Y = y;
                    Grid[x, y] = room;
                }
            }
        }
    }

    private bool ShouldGenerateRoom(int x, int y)
    {
        // Custom logic to decide if a room should be generated
        return Random.value > 0.5f;
    }

    private void AssignRoomTypes()
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
                        room.RoomType = RoomType.Monster;
                    }
                    else if (y == 8)
                    {
                        room.RoomType = RoomType.Treasure;
                    }
                    else if (y == 14)
                    {
                        room.RoomType = RoomType.RestSite;
                    }
                    else
                    {
                        double randValue = rand.NextDouble() * 100;
                        if (randValue < 45)
                        {
                            room.RoomType = RoomType.Monster;
                        }
                        else if (randValue < 67)
                        {
                            room.RoomType = RoomType.Event;
                        }
                        else if (randValue < 77)
                        {
                            room.RoomType = RoomType.EliteMonster;
                        }
                        else if (randValue < 89)
                        {
                            room.RoomType = RoomType.RestSite;
                        }
                        else if (randValue < 94)
                        {
                            room.RoomType = RoomType.Merchant;
                        }
                        else
                        {
                            room.RoomType = RoomType.Treasure;
                        }
                    }
                }
            }
        }
    }

    private void AllocateBossRoom()
    {
        System.Random rand = new System.Random();
        List<Room> topFloorRooms = new List<Room>();
        for (int x = 0; x < 7; x++)
        {
            Room room = Grid[x, 14];
            if (room != null && room.RoomType == RoomType.RestSite)
            {
                topFloorRooms.Add(room);
            }
        }

        // Instantiate the boss room in the middle of the top floor
        GameObject bossRoomObject = Instantiate(roomPrefab, new Vector3(3 * 2, 15 * 2, 0), Quaternion.identity);
        Room bossRoom = bossRoomObject.GetComponent<Room>();
        bossRoom.X = 3;
        bossRoom.Y = 15;
        bossRoom.RoomType = RoomType.Boss;

        // Connect the boss room to all rest site rooms on the top floor
        foreach (Room room in topFloorRooms)
        {
            room.ConnectedRooms.Add(bossRoom);
            bossRoom.ConnectedRooms.Add(room);
        }
    }
}
