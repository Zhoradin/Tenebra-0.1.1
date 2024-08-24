using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Button için gerekli

public class RoomInteraction : MonoBehaviour
{
    public Room Room { get; set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Button button; // Button bileşeni

    private void Awake()
    {
        // Initialize SpriteRenderer and Button
        SpriteRenderer = GetComponent<SpriteRenderer>();
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Button component is missing from this GameObject.");
        }
    }

    private void Start()
    {
        // Initialize Button interaction
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        if (button.interactable) // Kontrol et: Button tıklanabilir mi?
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
        if (button != null)
        {
            button.interactable = clickable; // Set button interactability
        }
        UpdateClickableVisuals();
    }

    public void UpdateClickableVisuals()
    {
        if (SpriteRenderer != null)
        {
            if (button != null)
            {
                // Change visual appearance based on button interactability
                if (button.interactable)
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
                        SpriteRenderer.color = color;
                    }
                    else
                    {
                        Debug.LogError("Invalid hex color code");
                    }
                }
            }
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
            SpriteRenderer.color = Color.cyan; // Example: yellow blink color
            yield return new WaitForSeconds(0.1f); // Blink duration
            SpriteRenderer.color = currentRoomColor;
        }
    }
}