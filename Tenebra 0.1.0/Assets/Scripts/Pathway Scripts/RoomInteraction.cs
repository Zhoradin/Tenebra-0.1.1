using UnityEngine;

public class RoomInteraction : MonoBehaviour
{
    public Room Room { get; private set; }
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private bool isClickable;

    public void InitializeRoom(Room room)
    {
        Room = room;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        SetClickable(false);  // Initially, set all rooms to not clickable

        //Debug.Log("Room initialized: " + Room.X + ", " + Room.Y);
        
    }

    public void SetClickable(bool value)
    {
        isClickable = value;
        spriteRenderer.color = value ? Color.yellow : originalColor;
        Debug.Log($"Room at ({Room.X}, {Room.Y}) isClickable set to: {isClickable}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"Room at ({Room.X}, {Room.Y}) clicked!");
        MapGenerator.Instance.OnRoomClicked(this);
        
        if (isClickable)
        {
            Debug.Log("Room clicked: " + Room.X + ", " + Room.Y);

            MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
            if (mapGenerator != null)
            {
                Debug.Log("MapGenerator found, calling OnRoomClicked.");
                mapGenerator.OnRoomClicked(this);
            }
            else
            {
                Debug.LogError("MapGenerator not found in the scene.");
            }
        }
    }

    public void BlinkSprite()
    {
        // Implement blinking effect for visual feedback (optional)
        Debug.Log("Blinking room: " + Room.X + ", " + Room.Y);
    }
}
