using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlot : MonoBehaviour
{
    public Image cardImage;
    public Image cardBgImage;
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
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.cardDescription;
        healthText.text = card.currentHealth.ToString();
        attackText.text = card.attackPower.ToString();
        costText.text = card.essenceCost.ToString();
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

    private void OnMouseOver()
    {
        isHovering = true;
    }

    private void OnMouseExit()
    {
        isHovering = false;
    }

    public void OnSelectButtonClick()
    {
        DeckController.instance.deckToUse.Add(cardData);
        BattleController.instance.StartCoroutine(BattleController.instance.ShowResultsCo());
        UIController.instance.CardSelected(cardNameText.text.ToString());

        // Card Trigger'daki button interactable durumu devre d��� b�rak�lacak
        CardSelectController.instance.DisableOtherCardSlots(this);
    }

    public void SetCardTriggerInteractable(bool state)
    {
        cardTriggerButton.interactable = state;
    }
}
