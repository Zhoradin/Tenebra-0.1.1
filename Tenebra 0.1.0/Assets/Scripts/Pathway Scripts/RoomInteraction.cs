using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInteraction : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Room associatedRoom;
    private bool isCurrentRoom;

    public void InitializeRoom(Room room)
    {
        associatedRoom = room;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject.");
            return;
        }

        originalColor = spriteRenderer.color;
        SetClickable(false); // Initialize as non-clickable
    }

    public void SetClickable(bool value)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is null in SetClickable.");
            return;
        }

        spriteRenderer.color = value ? Color.yellow : originalColor;

        if (value)
        {
            // Add BoxCollider2D if it doesn't already exist
            if (GetComponent<BoxCollider2D>() == null)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        }
        else
        {
            // Remove BoxCollider2D if it exists
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }
    }

    private void OnMouseDown()
    {
        if (!isCurrentRoom && associatedRoom != null)
        {
            MapGenerator.Instance.OnRoomClicked(this);
        }
    }

    public void SetAsCurrentRoom()
    {
        isCurrentRoom = true;
        StartCoroutine(BlinkSprite());
    }

    public void SetAsNonCurrentRoom()
    {
        isCurrentRoom = false;
        StopAllCoroutines();
        spriteRenderer.color = originalColor;
    }

    private IEnumerator BlinkSprite()
    {
        while (isCurrentRoom)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
