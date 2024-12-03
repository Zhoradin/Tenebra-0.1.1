using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomInteraction : MonoBehaviour
{
    public Room Room { get; set; }
    public Image buttonImage; // Replaces SpriteRenderer with Image component
    public Button button; // Button component

    private void Awake()
    {
        // Initialize Image and Button components
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("Button component is missing from this GameObject.");
        }

        if (buttonImage == null)
        {
            Debug.LogError("Image component is missing from this GameObject.");
        }
    }

    private void Start()
    {

    }

    public void OnButtonClick()
    {
        if (button.interactable) // Check if the button is clickable
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
        if (buttonImage != null)
        {
            // Change visual appearance based on button interactability
            if (button.interactable)
            {
                //buttonImage.color = Color.green; // Green for clickable
            }
            else
            {
                // Example hex code
                string hexColor = "#92B0DB"; // Replace this with your hex code

                // Convert hex to Color
                if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
                {
                    buttonImage.color = color;
                }
                else
                {
                    Debug.LogError("Invalid hex color code");
                }
            }
        }
    }

    public void BlinkSprite()
    {
        // Implement button blinking effect, e.g., changing color briefly
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            Color blinkColor = Color.cyan; // Example blink color
            buttonImage.color = blinkColor;
            yield return new WaitForSeconds(0f); // Blink duration
            buttonImage.color = originalColor;
        }
    }
}
