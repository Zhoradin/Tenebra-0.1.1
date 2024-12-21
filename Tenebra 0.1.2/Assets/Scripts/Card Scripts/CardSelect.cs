using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections;

public class CardSelect : MonoBehaviour
{
    public Image characterImage;
    public Image typeFrameImage;
    public Image moonPhaseImage;
    public Image typeImage;
    public Image attackImage, healthImage;
    public TMP_Text cardNameText;
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text costText;
    public GameObject abilityDescription1, abilityDescription2;
    public TMP_Text abilityDescriptionText, abilityDescriptionTextToo;
    public float flipDuration = 1f;

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
        characterImage.sprite = card.characterSprite;
        typeFrameImage.sprite = card.typeFrameSprite;
        moonPhaseImage.sprite = card.moonPhaseSprite;
        cardNameText.text = card.cardName;
        healthText.text = card.currentHealth.ToString();
        attackText.text = card.attackPower.ToString();
        costText.text = card.essenceCost.ToString();

        if (cardData.cardKind != CardKind.Field)
        {
            moonPhaseImage.gameObject.SetActive(false);
            healthImage.gameObject.SetActive(false);
            attackImage.gameObject.SetActive(false);
            healthText.gameObject.SetActive(false);
            attackText.gameObject.SetActive(false);
        }

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
        CardSelectController.instance.DisableOtherCardSlots(this);
        Destroy(gameObject);
        BattleController.instance.StartCoroutine(BattleController.instance.ShowResultsCo());
        UIController.instance.CardSelected(cardNameText.text.ToString());        
    }

    public void SetCardTriggerInteractable(bool state)
    {
        cardTriggerButton.interactable = state;
    }
}
