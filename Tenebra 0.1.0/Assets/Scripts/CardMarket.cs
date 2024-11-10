using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CardMarket : MonoBehaviour
{
    public Image cardImage;
    public Image cardBgImage;
    public TMP_Text cardNameText;
    public TMP_Text cardDescriptionText;
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text costText;
    public TMP_Text coinText;
    public Image coinImage;
    private int coinAmount;

    private CardSO cardData;

    private Vector3 originalScale;
    public Vector3 hoverScale;
    public float transitionSpeed = 5f;

    private bool isHovering = false;

    private Vector3 coinTextOriginalScale;
    private Vector3 coinImageOriginalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        coinTextOriginalScale = coinText.transform.localScale;
        coinImageOriginalScale = coinImage.transform.localScale;
    }

    public void SetupCardSlot(CardSO card)
    {
        cardData = card;
        cardImage.sprite = card.characterSprite;
        cardBgImage.sprite = card.bgSprite;
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.cardDescription;
        healthText.text = card.currentHealth.ToString();
        attackText.text = card.attackPower.ToString();
        costText.text = card.essenceCost.ToString();
        coinText.text = card.coinCost.ToString();
        coinAmount = card.coinCost;
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

        // coinText ve coinImage'ýn ölçeðini sabit tut
        coinText.transform.localScale = coinTextOriginalScale;
        coinImage.transform.localScale = coinImageOriginalScale;
    }

    public void OnMouseOver()
    {
        isHovering = true;
    }

    public void OnMouseExit()
    {
        isHovering = false;
    }

    public void OnSelectButtonClick()
    {
        if (DataCarrier.instance.playerCoin >= coinAmount)
        {
            DataCarrier.instance.playerCoin -= coinAmount;
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
