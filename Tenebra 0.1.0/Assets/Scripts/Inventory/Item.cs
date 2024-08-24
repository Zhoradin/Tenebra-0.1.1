using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemSO itemSO;

    public string itemName;

    public TMP_Text itemCost;

    [TextArea]
    public string itemDescription;

    public Image itemImage;

    public ItemSO.Permanence permanence;

    public int itemLongevity;

    //public GameObject itemDescriptionPanel;

    // Start is called before the first frame update
    void Start()
    {
        SetupItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupItem()
    {
        itemName = itemSO.itemName;
        itemCost.text = itemSO.itemCost.ToString();
        itemDescription = itemSO.itemDescription;
        itemImage.sprite = itemSO.itemSprite;
        itemLongevity = itemSO.itemLongevity;
    }

    public void BuyObject()
    {
        if(TestUIController.instance.testPlayerCoin >= itemSO.itemCost)
        {
            TestUIController.instance.testPlayerCoin -= itemSO.itemCost;
            TestUIController.instance.SetPlayerCoin();
            CheckItemSkill();
            FindObjectOfType<GameController>().SaveGame();
            DataCarrier.instance.possessedItems.Add(itemSO);
            Destroy(gameObject);
        }
    }

    public void CheckItemSkill()
    {
        switch (itemSO.itemSkill)
        {
            case ItemSO.ItemSkill.Heal:
                DataCarrier.instance.playerMaxHealth += itemSO.effectAmount;
                DataCarrier.instance.playerHealth += itemSO.effectAmount;
                TestUIController.instance.SetPlayerHealth();
                break;
        }
    }

    /*
    public void OnMouseOver()
    {
        itemDescriptionPanel.SetActive(true);
    }

    public void OnMouseExit()
    {
        itemDescriptionPanel.SetActive(false);
    }

    */
}
