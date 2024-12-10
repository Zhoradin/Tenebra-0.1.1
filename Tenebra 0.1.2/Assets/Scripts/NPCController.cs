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
    public List<TextAsset> dialogueJSONs; // Tüm inkJSON'larý liste olarak tutuyoruz
    public int currentOrder = 1;         // En son kullanýlan ORDER_TAG

    public void OpenDialoguePanel()
    {
        AudioManager.instance.PlaySFX(0);
        if (FindObjectOfType<BarController>() != null)
        {
            BarController.instance.leaveButton.SetActive(false);
        }
        // Eðer mevcut order deðerine göre bir JSON yoksa, varsayýlaný kullan
        if (currentOrder < dialogueJSONs.Count)
        {
            DialogueManager.instance.EnterDialogueMode(dialogueJSONs[currentOrder]);
        }
        else
        {
            Debug.LogError("Invalid order index: " + currentOrder);
        }
    }

    // ORDER_TAG güncelleme fonksiyonu
    public void UpdateOrder(int newOrder)
    {
        currentOrder = newOrder;
    }
}


