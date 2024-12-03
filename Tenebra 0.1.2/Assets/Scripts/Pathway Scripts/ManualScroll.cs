using UnityEngine;

public class ManualScroll : MonoBehaviour
{
    public RectTransform panelRectTransform; // Panelin RectTransform'u
    public float scrollSpeed = 10f; // Kayd�rma h�z� (scroll tekerle�i i�in)
    public float dragSpeed = 0.1f; // S�r�kleme h�z� (sol t�klama i�in)
    public float minY = -100f; // Minimum Y pozisyonu
    public float maxY = 100f;  // Maksimum Y pozisyonu

    private BoxCollider2D boxCollider;
    private bool isDragging = false; // Mouse s�r�kleme durumu
    private Vector2 lastMousePosition;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D component is missing from this GameObject.");
        }
    }

    void Update()
    {
        if (boxCollider != null && IsMouseOverCollider())
        {
            // Mouse sol t�klama ile paneli s�r�kleme
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(lastMousePosition);
                Vector3 newPosition = panelRectTransform.localPosition;
                newPosition.y += delta.y * dragSpeed; // S�r�kleme hareketini tersine �evir
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                panelRectTransform.localPosition = newPosition;
                lastMousePosition = Input.mousePosition;
            }

            // Mouse'un scroll �zelli�ini kullanarak paneli yukar� ve a�a�� kayd�rma
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Vector3 newPosition = panelRectTransform.localPosition;
                newPosition.y -= scroll * scrollSpeed; // Scroll y�n�n� tersine �evir
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                panelRectTransform.localPosition = newPosition;
            }
        }
    }

    // Mouse'un BoxCollider'�n �zerinde olup olmad���n� kontrol eden fonksiyon
    private bool IsMouseOverCollider()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return boxCollider.OverlapPoint(mousePosition);
    }
}
