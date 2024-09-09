using UnityEngine;

public class ManualScroll : MonoBehaviour
{
    public RectTransform panelRectTransform; // Panelin RectTransform'u
    public float scrollSpeed = 10f; // Kaydýrma hýzý (scroll tekerleði için)
    public float dragSpeed = 0.1f; // Sürükleme hýzý (sol týklama için)
    public float minY = -100f; // Minimum Y pozisyonu
    public float maxY = 100f;  // Maksimum Y pozisyonu

    private BoxCollider2D boxCollider;
    private bool isDragging = false; // Mouse sürükleme durumu
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
            // Mouse sol týklama ile paneli sürükleme
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
                newPosition.y += delta.y * dragSpeed; // Sürükleme hareketini tersine çevir
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                panelRectTransform.localPosition = newPosition;
                lastMousePosition = Input.mousePosition;
            }

            // Mouse'un scroll özelliðini kullanarak paneli yukarý ve aþaðý kaydýrma
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Vector3 newPosition = panelRectTransform.localPosition;
                newPosition.y -= scroll * scrollSpeed; // Scroll yönünü tersine çevir
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                panelRectTransform.localPosition = newPosition;
            }
        }
    }

    // Mouse'un BoxCollider'ýn üzerinde olup olmadýðýný kontrol eden fonksiyon
    private bool IsMouseOverCollider()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return boxCollider.OverlapPoint(mousePosition);
    }
}
