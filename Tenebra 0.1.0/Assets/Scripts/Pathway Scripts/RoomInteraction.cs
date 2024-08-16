using System.Collections;
using UnityEngine;

public class RoomInteraction : MonoBehaviour
{
    public bool IsClickable; // Public property for accessibility
    public Room Room { get; set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        // Initialize SpriteRenderer
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (IsClickable)
        {
            // Notify MapGenerator that this room was clicked
            MapGenerator.Instance.OnRoomClicked(this);
        }
    }

    public void InitializeRoom(Room room)
    {
        Room = room;
        // Initialize other properties as needed
        UpdateClickableVisuals(); // Ensure visuals reflect the initial clickability state
    }

    public void SetClickable(bool clickable)
    {
        IsClickable = clickable;
        UpdateClickableVisuals();
    }

    private void UpdateClickableVisuals()
    {
        if (SpriteRenderer != null)
        {
            // Change color or add a visual indicator to show whether the room is clickable
            SpriteRenderer.color = IsClickable ? Color.green : Color.red; // Example: green for clickable, red for not clickable
        }
    }

    public void BlinkSprite()
    {
        // Implement sprite blinking effect, e.g., changing color briefly
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        if (SpriteRenderer != null)
        {
            Color originalColor = SpriteRenderer.color;
            SpriteRenderer.color = Color.yellow; // Example: yellow blink color
            yield return new WaitForSeconds(0.2f); // Blink duration
            SpriteRenderer.color = originalColor; // Reset to original color
        }
    }
}
