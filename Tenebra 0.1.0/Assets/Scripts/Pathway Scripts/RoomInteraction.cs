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

    public void UpdateClickableVisuals()
    {
        if (SpriteRenderer != null)
        {
            // Change color or add a visual indicator to show whether the room is clickable
            if (IsClickable)
            {
                SpriteRenderer.color = Color.green; // Green for clickable
            }
            else
            {
                // Example hex code
                string hexColor = "#92B0DB"; // Replace this with your hex code

                // Convert hex to Color
                if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
                {
                    // Apply the color to a sprite's SpriteRenderer component
                    SpriteRenderer.color = color;
                }
                else
                {
                    Debug.LogError("Invalid hex color code");
                }
            }

            // Additional logic to visually indicate the room's current state
            // This can include enabling/disabling a UI element, changing sprite, etc.
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
            Color currentRoomColor = Color.magenta;
            //SpriteRenderer.color = Color.cyan; // Example: yellow blink color
            yield return new WaitForSeconds(0.1f); // Blink duration
            SpriteRenderer.color = currentRoomColor;
            //SpriteRenderer.color = originalColor; // Reset to original color
        }
    }
}
