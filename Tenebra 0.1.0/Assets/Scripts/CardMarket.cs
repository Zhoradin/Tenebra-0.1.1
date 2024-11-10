using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;

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
    public GameObject abilityDescription1, abilityDescription2;
    public TMP_Text abilityDescriptionText, abilityDescriptionTextToo;
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
        abilityDescription1.gameObject.SetActive(false);
        abilityDescription2.gameObject.SetActive(false);
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
        UpdateAbilityDescription(card);
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

    private void UpdateAbilityDescription(CardSO cardSO)
    {
        if (cardSO.abilities.Length > 0)
        {
            abilityDescriptionText.text = AddSpacesToAbilityName(cardSO.abilities[0].abilityType.ToString()) + "\n" + cardSO.abilities[0].description;

            if (cardSO.abilities.Length > 1)
            {
                abilityDescriptionTextToo.text = AddSpacesToAbilityName(cardSO.abilities[1].abilityType.ToString()) + "\n" + cardSO.abilities[1].description;
            }
        }
    }

    private string AddSpacesToAbilityName(string abilityName)
    {
        if (string.IsNullOrEmpty(abilityName))
            return "";

        StringBuilder newAbilityName = new StringBuilder();
        newAbilityName.Append(abilityName[0]);

        for (int i = 1; i < abilityName.Length; i++)
        {
            if (char.IsUpper(abilityName[i]) && abilityName[i - 1] != ' ')
            {
                newAbilityName.Append(' ');
            }
            newAbilityName.Append(abilityName[i]);
        }

        return newAbilityName.ToString();
    }

    public void OnMouseOver()
    {
        isHovering = true;
        if (Time.timeScale != 0f && cardData.abilities.Length > 0)
        {
            abilityDescription1.SetActive(true);

            if (cardData.abilities.Length > 1)
            {
                abilityDescription2.SetActive(true);
            }
        }
    }

    public void OnMouseExit()
    {
        isHovering = false;
        abilityDescription1.gameObject.SetActive(false);
        abilityDescription2.gameObject.SetActive(false);
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
