using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class CardSlot : MonoBehaviour
{
    public Image cardImage;
    public Image cardBgImage;
    public Image cardMoonPhaseImage;
    public TMP_Text cardNameText;
    public TMP_Text cardDescriptionText;
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text costText;
    public GameObject abilityDescription1, abilityDescription2;
    public TMP_Text abilityDescriptionText, abilityDescriptionTextToo;

    private CardSO cardData;

    private Vector3 originalScale;
    public Vector3 hoverScale;
    public float transitionSpeed = 5f;

    private bool isHovering = false;

    // Card Trigger alt nesnesindeki Button'a referans
    public Button cardTriggerButton;

    private void Start()
    {
        abilityDescription1.gameObject.SetActive(false);
        abilityDescription2.gameObject.SetActive(false);
        originalScale = transform.localScale;
    }

    public void SetupCardSlot(CardSO card)
    {
        cardData = card;
        cardImage.sprite = card.characterSprite;
        cardBgImage.sprite = card.bgSprite;
        cardMoonPhaseImage.sprite = card.moonPhaseSprite;
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.cardDescription;
        healthText.text = card.currentHealth.ToString();
        attackText.text = card.attackPower.ToString();
        costText.text = card.essenceCost.ToString();

        UpdateAbilityDescription();
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

    private void UpdateAbilityDescription()
    {
        if (cardData.abilities.Length > 0)
        {
            abilityDescriptionText.text = AddSpacesToAbilityName(cardData.abilities[0].abilityType.ToString()) + "\n" + cardData.abilities[0].description;

            if (cardData.abilities.Length > 1)
            {
                abilityDescriptionTextToo.text = AddSpacesToAbilityName(cardData.abilities[1].abilityType.ToString()) + "\n" + cardData.abilities[1].description;
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
        DeckController.instance.deckToUse.Add(cardData);
        BattleController.instance.StartCoroutine(BattleController.instance.ShowResultsCo());
        UIController.instance.CardSelected(cardNameText.text.ToString());

        // Card Trigger'daki button interactable durumu devre dýþý býrakýlacak
        CardSelectController.instance.DisableOtherCardSlots(this);
    }

    public void SetCardTriggerInteractable(bool state)
    {
        cardTriggerButton.interactable = state;
    }
}
