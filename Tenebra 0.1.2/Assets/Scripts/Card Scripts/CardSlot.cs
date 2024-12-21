using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections;

public class CardSlot : MonoBehaviour
{
    public Image characterImage, backCharacterImage;
    public Image typeFrameImage, backTypeFrameImage;
    public Image moonPhaseImage, backMoonPhaseImage;
    public Image typeImage, backTypeImage;
    public Image attackImage, backAttackImage, healthImage, backHealthImage;
    public TMP_Text cardNameText, backCardNameText;
    public TMP_Text cardDescriptionText;
    public TMP_Text healthText, backHealthText;
    public TMP_Text attackText, backAttackText;
    public TMP_Text costText, backCostText;
    public GameObject abilityDescription1, backAbilityDescription1, abilityDescription2, backAbilityDescription2;
    public TMP_Text abilityDescriptionText, backAbilityDescriptionText, abilityDescriptionTextToo, backAbilityDescriptionTextToo;
    public GameObject frontCard, backCard, cardTrigger, backCardTrigger;
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
        backAbilityDescription1.gameObject.SetActive(false);
        backAbilityDescription2.gameObject.SetActive(false);
        originalScale = transform.localScale;
    }

    public void SetupCardSlot(CardSO card)
    {
        cardData = card;
        characterImage.sprite = card.characterSprite;
        typeFrameImage.sprite = card.typeFrameSprite;
        moonPhaseImage.sprite = card.moonPhaseSprite;
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.cardDescription;
        healthText.text = card.currentHealth.ToString();
        attackText.text = card.attackPower.ToString();
        costText.text = card.essenceCost.ToString();

        backCharacterImage.sprite = card.characterSprite;
        backTypeFrameImage.sprite = card.typeFrameSprite;
        backMoonPhaseImage.sprite = card.moonPhaseSprite;
        backCardNameText.text = card.cardName;
        backHealthText.text = card.currentHealth.ToString();
        backAttackText.text = card.attackPower.ToString();
        backCostText.text = card.essenceCost.ToString();

        if(cardData.cardKind != CardKind.Field)
        {
            moonPhaseImage.gameObject.SetActive(false);
            healthImage.gameObject.SetActive(false);
            attackImage.gameObject.SetActive(false);
            healthText.gameObject.SetActive(false);
            attackText.gameObject.SetActive(false);

            backMoonPhaseImage.gameObject.SetActive(false);
            backHealthImage.gameObject.SetActive(false);
            backAttackImage.gameObject.SetActive(false);
            backHealthText.gameObject.SetActive(false);
            backAttackText.gameObject.SetActive(false);
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

    public void SetCardTriggerInteractable(bool state)
    {
        cardTriggerButton.interactable = state;
    }

    private IEnumerator FlipToFront()
    {
        if (backCard.activeSelf)
        {
            yield return FlipCardCo(); // Arka y�z� kapat�p �n y�z� a�ar
        }
    }

    // Kart�n d�nd�rme fonksiyonu
    public void FlipCard()
    {
        StartCoroutine(FlipCardCo());
    }

    private IEnumerator FlipCardCo()
    {
        cardTrigger.GetComponent<Button>().interactable = false;
        backCardTrigger.GetComponent<Button>().interactable = false;

        float elapsed = 0f;

        Transform cardTransform = this.transform; // Kart�n transform'u
        Quaternion startRotation = cardTransform.rotation; // Ba�lang�� rotasyonu
        Quaternion midRotation = startRotation * Quaternion.Euler(0, 90, 0); // 90 derece rotasyonu

        // �lk 90 derece d�n��
        while (elapsed < flipDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2f);

            cardTransform.rotation = Quaternion.Lerp(startRotation, midRotation, t);
            yield return null;
        }

        // Tam 90 dereceye ula�t���nda g�rselleri de�i�tir
        cardTransform.rotation = midRotation;

        if (frontCard.activeSelf)
        {
            frontCard.SetActive(false); // �n y�z� kapat
            backCard.SetActive(true);  // Arka y�z� a�
        }
        else
        {
            frontCard.SetActive(true);
            backCard.SetActive(false);
        }

        // �kinci d�n�� (90 derece geri d�n��)
        elapsed = 0f; // S�reyi s�f�rla
        while (elapsed < flipDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (flipDuration / 2f);

            cardTransform.rotation = Quaternion.Lerp(midRotation, startRotation, t);
            yield return null;
        }

        // Tam ba�lang�� rotasyonuna geri d�n
        cardTransform.rotation = startRotation;
        cardTrigger.GetComponent<Button>().interactable = true;
        backCardTrigger.GetComponent<Button>().interactable = true;

        // E�er kart�n backCard y�z� a��k ve mouse kart�n �zerinde de�ilse �n y�ze d�nd�r
        if (backCard.activeSelf && !isHovering)
        {
            StartCoroutine(FlipToFront());
        }
    }
}
