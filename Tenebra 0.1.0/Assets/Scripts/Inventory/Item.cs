using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemSO itemSO;

    public string itemName;

    public TMP_Text itemCostText;
    public int itemCost;

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
        itemCostText.text = itemSO.itemCost.ToString();
        itemDescription = itemSO.itemDescription;
        itemImage.sprite = itemSO.itemSprite;
        itemLongevity = itemSO.itemLongevity;
    }

    public void BuyObject()
    {
        if (DataCarrier.instance.playerCoin > itemCost)
        {
            DataCarrier.instance.playerCoin -= itemCost;
            BarController.instance.SetPlayerCoin();
            FindObjectOfType<GameController>().SaveGame();
            Destroy(gameObject);
            CheckItemSkill();
        }
        else
        {
            BarController.instance.lowCoinWarning.SetActive(true);
            StartCoroutine(HideLowEssenceWarning());
        }
    }

    public void CheckItemSkill()
    {
        switch (itemSO.itemSkill)
        {
            case ItemSO.ItemSkill.Heal:
                DataCarrier.instance.playerMaxHealth += itemSO.effectAmount;
                DataCarrier.instance.playerHealth += itemSO.effectAmount;
                BarController.instance.SetPlayerHealth();
                break;
        }
    }

    private IEnumerator HideLowEssenceWarning()
    {
        yield return new WaitForSeconds(3f);
        BarController.instance.lowCoinWarning.SetActive(false);
    }
}
