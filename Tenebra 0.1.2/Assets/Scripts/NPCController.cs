using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public static NPCController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject dialogueButton;
    public List<TextAsset> dialogueJSONs; // T�m inkJSON'lar� liste olarak tutuyoruz
    public int currentOrder = 1;         // En son kullan�lan ORDER_TAG

    public void OpenDialoguePanel()
    {
        AudioManager.instance.PlaySFX(0);
        if (FindObjectOfType<BarController>() != null)
        {
            BarController.instance.leaveButton.SetActive(false);
        }
        // E�er mevcut order de�erine g�re bir JSON yoksa, varsay�lan� kullan
        if (currentOrder < dialogueJSONs.Count)
        {
            DialogueManager.instance.EnterDialogueMode(dialogueJSONs[currentOrder]);
        }
        else
        {
            Debug.LogError("Invalid order index: " + currentOrder);
        }
    }

    // ORDER_TAG g�ncelleme fonksiyonu
    public void UpdateOrder(int newOrder)
    {
        currentOrder = newOrder;
    }
}


