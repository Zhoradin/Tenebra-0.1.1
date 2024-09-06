using System.Collections.Generic;

public class RoomManager
{
    private List<Room> rooms;
    private List<RoomConnection> connections;

    public RoomManager()
    {
        rooms = new List<Room>();
        connections = new List<RoomConnection>();
    }

    public void AddRoom(Room room)
    {
        rooms.Add(room);
    }

    public void AddConnection(Room roomA, Room roomB)
    {
        var connection = new RoomConnection(roomA, roomB);
        connections.Add(connection);
    }

    public List<RoomConnection> GetConnections()
    {
        return connections;
    }

    public List<Room> GetRooms()
    {
        return rooms;
    }

    public Room GetRoomById(int id)
    {
        return rooms.Find(r => r.Id == id);
    }

    // Belirli bir oda için bağlantıları döndüren yöntem
    public List<Room> GetConnectedRooms(Room room)
    {
        List<Room> connectedRooms = new List<Room>();
        foreach (var connection in connections)
        {
            if (connection.RoomA == room)
            {
                connectedRooms.Add(connection.RoomB);
            }
            else if (connection.RoomB == room)
            {
                connectedRooms.Add(connection.RoomA);
            }
        }
        return connectedRooms;
    }
}

