using System.Collections.Generic;
using UnityEngine;

public class MerchantItemController : MonoBehaviour
{
    public List<ItemSO> itemList = new List<ItemSO>(); // ItemSO t�r�nde bir liste
    public GameObject itemPrefab; // Item prefab'i
    public Transform content; // ScrollView'in content nesnesi

    private DataCarrier dataCarrier;

    // Start is called before the first frame update
    void Start()
    {
        dataCarrier = DataCarrier.instance;
        RemovePossessedItems(); // Oyuncunun sahip oldu�u itemleri kald�r
        PopulateItems();
    }

    void RemovePossessedItems()
    {
        // possessedItems i�inde yer alan ��eleri itemList'ten ��kar
        foreach (ItemSO possessedItem in dataCarrier.possessedItems)
        {
            if (itemList.Contains(possessedItem))
            {
                itemList.Remove(possessedItem);
            }
        }
    }

    void PopulateItems()
    {
        foreach (ItemSO itemSO in itemList)
        {
            GameObject newItem = Instantiate(itemPrefab, content);
            Item itemComponent = newItem.GetComponent<Item>();
            itemComponent.itemSO = itemSO;
            itemComponent.SetupItem();
        }
    }
}
