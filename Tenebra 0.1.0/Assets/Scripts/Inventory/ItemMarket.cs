using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemMarket : MonoBehaviour
{
    public Image itemImage;
    //public TMP_Text itemNameText;
    //public TMP_Text itemDescriptionText;
    public TMP_Text itemCostText;

    private ItemSO itemData;
    private int itemCost;

    private Vector3 originalScale;
    public Vector3 hoverScale;
    public float transitionSpeed = 5f;

    private bool isHovering = false;

    private Vector3 coinTextOriginalScale;
    private Vector3 coinImageOriginalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isHovering)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, hoverScale, Time.deltaTime * transitionSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * transitionSpeed);
        }
    }

    public void OnMouseOver()
    {
        isHovering = true;
    }

    public void OnMouseExit()
    {
        isHovering = false;
    }

    public void SetupItemSlot(ItemSO item)
    {
        itemData = item;
        itemImage.sprite = item.itemSprite;
        //itemNameText.text = item.itemName;
        //itemDescriptionText.text = item.itemDescription;
        itemCostText.text = item.itemCost.ToString();
        itemCost = item.itemCost;
    }

    public void OnSelectButtonClick()
    {
        if(DataCarrier.instance.playerCoin > itemCost)
        {
            DataCarrier.instance.playerCoin -= itemCost;
            MerchantController.instance.UpdateCoin();
            FindObjectOfType<GameController>().SaveGame();
            Destroy(gameObject);
        }
        else
        {
            MerchantController.instance.lowEssenceWarning.SetActive(true);
            StartCoroutine(HideLowEssenceWarning());
        }
    }

    private IEnumerator HideLowEssenceWarning()
    {
        yield return new WaitForSeconds(3f);
        MerchantController.instance.lowEssenceWarning.SetActive(false);
    }
}
