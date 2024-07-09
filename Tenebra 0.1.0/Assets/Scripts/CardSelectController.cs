using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectController : MonoBehaviour
{
    public static CardSelectController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardSO> cardToSelect = new List<CardSO>();
    public GameObject cardSlotPrefab;
    public Transform targetPosition; // Hedef pozisyonu tanýmlamak için
    public float moveSpeed = 5f; // Taþýma hýzý

    private Vector3 originalPosition;

    private void Start()
    {
        // Baþlangýç pozisyonu ve rotasyonu kaydediyoruz
        originalPosition = transform.position;
    }

    private void Update()
    {
        
    }

    public void ShowCardSelect()
    {
        StartCoroutine(MoveToTargetPosition());
        PopulateCardSlots();
    }

    private IEnumerator MoveToTargetPosition()
    {
        while (Vector3.Distance(transform.position, targetPosition.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void PopulateCardSlots()
    {
        if (cardToSelect.Count < 3)
        {
            Debug.LogError("Card list does not contain enough cards.");
            return;
        }

        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < 3)
        {
            int randomIndex = Random.Range(0, cardToSelect.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            Transform cardSlot = transform.GetChild(i); // Alt nesne olarak cardSlot'larý alýyoruz
            CardSlot cardSlotScript = cardSlot.GetComponent<CardSlot>();
            cardSlotScript.SetupCardSlot(cardToSelect[selectedIndices[i]]);
        }
    }
}
