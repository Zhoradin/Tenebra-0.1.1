using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public int X { get; set; } // X position in the grid
    public int Y { get; set; } // Y position in the grid
    public RoomType RoomType { get; set; } // Type of the room (e.g., Monster, Treasure)
    public List<Room> ConnectedRooms { get; set; } = new List<Room>(); // List of connected rooms

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Visualize the rooms and paths in the Unity Editor
    void OnDrawGizmos()
    {
        if (RoomType != RoomType.None)
        {
            
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        if (ConnectedRooms != null)
        {
            Gizmos.color = Color.white;
            foreach (Room connectedRoom in ConnectedRooms)
            {
                Gizmos.DrawLine(transform.position, connectedRoom.transform.position);
                Debug.Log("Çizgi oluşturuldu");
            }
        }
    }
}

