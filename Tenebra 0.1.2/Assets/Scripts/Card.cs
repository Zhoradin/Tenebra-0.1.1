using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class Card : MonoBehaviour
{
    public static Card instance;

    private void Awake()
    {
        instance = this;
    }

    public CardSO cardSO;

    public int currentHealth, attackPower, essenceCost;
    [HideInInspector]
    public int originalHealth, originalAttack, originalEssence, metamorphosisTurnCount, decayTurns, waxingCrescentCount, stunDuration;

    public TMP_Text healthText, attackText, costText, nameText, descriptionText, abilityDescriptionText, abilityDescriptionTextToo, superEffectiveText, notEffectiveText;

    public Image characterArt, bgArt, moonPhaseArt, healthArt, attackArt, stunImage;

    public bool inPlayerHand, inEnemyHand, isActive, isSelected, returningToHand, justPressed, isPlayer, isGraveyard, isLocked, isTransformed;
    public int handPosition;

    public GameObject abilityDescription, abilityDescriptionToo;

    public CardType cardType;

    public CardKind cardKind;

    public CardRarity cardRarity;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public float selectedRotateSpeed = 720f;
    public float scaleSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

    private HandController theHC;

    [HideInInspector]
    public Collider2D theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;

    public CardPlacePoint assignedPlace;

    public Animator anim;

    [HideInInspector]
    public bool directHit, doubleTap, glassCannon, instaKill, mend, leech, metamorphosis, primalPact, scattershot, growth, decay, decayed, guardian, benevolence, snowball, multipleHit, duality, 
        doppelganger, usedWaxingCrescent, gratis, stun, stunned, healBlock, mirror, harvester = false;
    [HideInInspector]
    public Card decayedBy;

    public bool fullMoon = false;

    // Start is called before the first frame update
    void Start()
    {
        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }

        theHC = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider2D>();

        originalScale = transform.localScale;
        targetScale = originalScale;

        abilityDescription.SetActive(false);
        abilityDescriptionToo.SetActive(false);
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        essenceCost = cardSO.essenceCost;

        originalHealth = currentHealth;
        originalAttack = attackPower;
        originalEssence = essenceCost;

        UpdateCardDisplay();
        stunImage.gameObject.SetActive(false);

        nameText.text = cardSO.cardName;
        descriptionText.text = cardSO.cardDescription;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;

        cardType = cardSO.cardType;
        cardKind = cardSO.cardKind;
        cardRarity = cardSO.cardRarity;

        if(cardKind == CardKind.Field)
        {
            moonPhaseArt.sprite = cardSO.moonPhaseSprite;
        }
        else
        {
            moonPhaseArt.gameObject.SetActive(false);
            healthArt.gameObject.SetActive(false);
            attackArt.gameObject.SetActive(false);
            healthText.gameObject.SetActive(false);
            attackText.gameObject.SetActive(false);
        }

        isGraveyard = cardSO.isGraveyard;

        UpdateAbilityDescription();

        MoonPhaseController.instance.CheckMoonPhase(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0; // Ensure the card stays on the same z-plane
            MoveToPoint(worldPosition + new Vector3(0f, 2f, -4f), Quaternion.identity); // Resets rotation while moving

            if(cardKind == CardKind.Field)
            {
                CheckForSuperEffectiveText();
            }

            if (Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
            {
                ReturnToHand();
            }

            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.battleEnded == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, whatIsPlacement);

                //Check for Field Card
                if (hit.collider != null && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && cardKind == CardKind.Field )
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint && selectedPoint.isFieldPoint)
                    {
                        FieldUsage(selectedPoint);
                    }
                    //Check for Doppelganger
                    else if (selectedPoint.activeCard != null && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && cardKind == CardKind.Field && 
                        cardSO.abilities[0].name == "Doppelganger")
                    {
                        if (BattleController.instance.playerEssence >= essenceCost)
                        {
                            // Oynandığı kartı ele geri gönder
                            HandController.instance.AddCardToHand(selectedPoint.activeCard);
                            selectedPoint.activeCard.ReturnToHand();
                            selectedPoint.activeCard.isActive = false;
                            selectedPoint.activeCard.returningToHand = false;

                            // Hedef kartı alanı boşalt
                            selectedPoint.activeCard = null;

                            // Doppelganger kartını yerine yerleştir
                            FieldUsage(selectedPoint);

                            // Essence azalt
                            BattleController.instance.playerEssence -= essenceCost;
                        }
                        else
                        {
                            ReturnToHand();
                            UIController.instance.ShowEssenceWarning();
                        }
                    }
                    // Check if a Field card can replace a Doppelganger card
                    else if (selectedPoint.activeCard != null && selectedPoint.activeCard.cardSO.abilities[0].name == "Doppelganger" && cardKind == CardKind.Field)
                    {
                        if (BattleController.instance.playerEssence >= essenceCost)
                        {
                            // Doppelganger kartını sahadan kaldır ve discard pile'a ekle
                            Card currentCard = selectedPoint.activeCard;
                            DiscardPileController.instance.AddToDiscardPile(currentCard.cardSO);

                            // Doppelganger kartının animasyon ve yok olma işlemi
                            StartCoroutine(AbilityManager.instance.RemoveDoppelgangerAndPlaceField(currentCard, selectedPoint, this));
                        }
                        else
                        {
                            ReturnToHand();
                            UIController.instance.ShowEssenceWarning();
                        }
                    }

                    //Check for Locking Down
                    else if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint && selectedPoint.isLockedPoint)
                    {
                        LockedDownCheck(selectedPoint);
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }

                //Check for Efect card
                else if(hit.collider != null && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && cardKind == CardKind.Effect)
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint.activeCard != null && selectedPoint.isPlayerPoint && selectedPoint.isFieldPoint)
                    {
                        if (BattleController.instance.playerEssence >= essenceCost)
                        {
                            AbilityManager.instance.ActivateEffectAbility(this, selectedPoint.activeCard);

                            EffectImpactUsage();
                        }
                        else
                        {
                            ReturnToHand();
                            UIController.instance.ShowEssenceWarning();
                        }
                    }
                    else if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint && selectedPoint.isLockedPoint)
                    {
                        LockedDownCheck(selectedPoint);
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }

                // Impact Card Check
                else if (hit.collider != null && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && cardKind == CardKind.Impact)
                {
                    
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint.isImpactPoint)
                    {
                        if (BattleController.instance.playerEssence >= essenceCost)
                        {
                            AbilityManager.instance.ActivateImpactAbility(this);

                            EffectImpactUsage();
                        }
                        else
                        {
                            ReturnToHand();
                            UIController.instance.ShowEssenceWarning();
                        }
                    }
                    else if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint && selectedPoint.isLockedPoint)
                    {
                        LockedDownCheck(selectedPoint);
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        float currentRotateSpeed = isSelected || returningToHand ? selectedRotateSpeed : rotateSpeed; // Seçildiğinde veya ele dönerken farklı rotation hızı kullan
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, currentRotateSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);

        justPressed = false;
    }

    private void LockedDownCheck(CardPlacePoint selectedPoint)
    {
        selectedPoint.activeCard = this;
        assignedPlace = selectedPoint;

        MoveToPoint(selectedPoint.transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.identity);

        isSelected = false;
        returningToHand = false;
        isLocked = true;
        targetScale = originalScale;
        theHC.RemoveCardFromHand(this);
        theCol.enabled = true;
    }

    private void EffectImpactUsage()
    {
        if (isPlayer)
        {
            if (isGraveyard)
            {
                GraveyardPileController.instance.AddToGraveyardPile(cardSO);
            }
            else
            {
                DiscardPileController.instance.AddToDiscardPile(cardSO);
            }
        }

        StartCoroutine(WaitJumpAfterDeadCo());

        inPlayerHand = false;
        isSelected = false;
        returningToHand = false;
        targetScale = originalScale;
        theHC.RemoveCardFromHand(this);
        BattleController.instance.SpendPlayerEssence(essenceCost);
        isActive = true;
        MoonPhaseController.instance.CheckMoonPhase(this);
        theCol.enabled = true;
    }

    public void FieldUsage(CardPlacePoint selectedPoint)
    {
        if (BattleController.instance.playerEssence >= essenceCost)
        {
            selectedPoint.activeCard = this;
            assignedPlace = selectedPoint;

            MoveToPoint(selectedPoint.transform.position + new Vector3(0f, 0.75f, 0f), Quaternion.identity);

            inPlayerHand = false;
            isSelected = false;
            returningToHand = false;
            targetScale = originalScale;

            theHC.RemoveCardFromHand(this);

            AbilityManager.instance.ActivateAbility(this);

            if (instaKill == true)
            {
                StartCoroutine(AbilityManager.instance.QuickAttackCoroutine(this));
            }

            BattleController.instance.SpendPlayerEssence(essenceCost);
            isActive = true;
            MoonPhaseController.instance.CheckMoonPhase(this);
            theCol.enabled = true;
        }
        else
        {
            ReturnToHand();
            UIController.instance.ShowEssenceWarning();
        }
    }

    private void CheckForSuperEffectiveText()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, whatIsPlacement);

        if (hit.collider != null)
        {
            CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
            if (selectedPoint != null)
            {
                int index = System.Array.IndexOf(CardPointsController.instance.playerCardPoints, selectedPoint);
                if (index >= 0 && index < CardPointsController.instance.enemyCardPoints.Length)
                {
                    if (CardPointsController.instance.enemyCardPoints[index] != null && CardPointsController.instance.enemyCardPoints[index].activeCard != null)
                    {
                        float effectiveness = TypeEffectiveness.GetEffectiveness(cardType, CardPointsController.instance.enemyCardPoints[index].activeCard.cardType);
                        float result = attackPower * effectiveness;
                        float final = result - attackPower;
                        Debug.Log(effectiveness);
                        if (effectiveness == 2)
                        {
                            superEffectiveText.text = "+" + Mathf.RoundToInt(final).ToString();
                            superEffectiveText.gameObject.SetActive(true);
                        }
                        else if (effectiveness == .5f)
                        {
                            notEffectiveText.text = Mathf.RoundToInt(final).ToString();
                            notEffectiveText.gameObject.SetActive(true);
                        }
                        else
                        {
                            superEffectiveText.gameObject.SetActive(false);
                            notEffectiveText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        superEffectiveText.gameObject.SetActive(false);
                        notEffectiveText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    superEffectiveText.gameObject.SetActive(false);
                    notEffectiveText.gameObject.SetActive(false);
                }
            }
            else
            {
                superEffectiveText.gameObject.SetActive(false);
                notEffectiveText.gameObject.SetActive(false);
            }
        }
        else
        {
            superEffectiveText.gameObject.SetActive(false);
            notEffectiveText.gameObject.SetActive(false);
        }
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    private void OnMouseOver()
    {
        if ((inPlayerHand || isActive) && !isSelected && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false 
            && UIController.instance.graveyardPileOpen == false && UIController.instance.encyclopediaPanelOpen == false && UIController.instance.inventoryPanelOpen == false)
        {
            targetScale = hoverScale;

            if (inPlayerHand)
            {
                Vector3 hoverPosition;

                if (isLocked)
                {
                    hoverPosition = theHC.lockedPosition + new Vector3(0f, 1f, -2f);
                }
                else
                {
                    hoverPosition = theHC.playerCardPositions[handPosition] + new Vector3(0f, 1.5f, -2f);
                    theHC.SpreadCards(handPosition, 1f); 
                }
                MoveToPoint(hoverPosition, targetRot);
            }

            if (isActive)
            {
                CheckForSuperEffectiveText();
            }

            if (Time.timeScale != 0f && cardSO.abilities.Length > 0)
            {
                abilityDescription.SetActive(true);

                if (cardSO.abilities.Length > 1)
                {
                    abilityDescriptionToo.SetActive(true);
                }
            }
        }
    }

    private void OnMouseExit()
    {
        targetScale = originalScale;
        if ((inPlayerHand && !isActive) && !isSelected && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            if (isLocked)
            {
                targetScale = originalScale / 1.3f;
                MoveToPoint(theHC.lockedPosition + new Vector3(0f, .55f, -2f), targetRot);
            }
            else
            {
                MoveToPoint(theHC.playerCardPositions[handPosition] , targetRot);
            }

            abilityDescription.SetActive(false);
            abilityDescriptionToo.SetActive(false);

            theHC.ResetCardPositions();
        }
        else if (isActive && !isSelected && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            abilityDescription.SetActive(false);
            abilityDescriptionToo.SetActive(false);
        }
        superEffectiveText.gameObject.SetActive(false);
        notEffectiveText.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (inPlayerHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer && BattleController.instance.battleEnded == false && Time.timeScale != 0f
            && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false && UIController.instance.graveyardPileOpen == false)
        {
            isSelected = true;
            theCol.enabled = false;
            targetRot = Quaternion.identity;
            targetScale = selectedScale;

            justPressed = true;
            if (isLocked)
            {
                assignedPlace.activeCard = null;
                assignedPlace = null;
            }
        }
        abilityDescription.SetActive(false);
        abilityDescriptionToo.SetActive(false);
    }

    public void ReturnToHand()
    {
        isSelected = false;
        returningToHand = true;
        HandController.instance.ResetCardPositions();

        if (isLocked)
        {
            // assignedPlace'i boşalt
            if (assignedPlace != null)
            {
                assignedPlace.activeCard = null;
                assignedPlace = null;
            }

            isLocked = false;
            theHC.AddCardToHand(this);
            targetRot = theHC.playerCardRotations[handPosition];
            MoveToPoint(theHC.playerCardPositions[handPosition], targetRot);
        }
        else
        {
            targetRot = theHC.playerCardRotations[handPosition];
            MoveToPoint(theHC.playerCardPositions[handPosition], targetRot);
        }

        targetScale = originalScale;

        StartCoroutine(EnableColliderWithDelay());
    }

    private IEnumerator EnableColliderWithDelay()
    {
        yield return new WaitForSeconds(.2f);
        theCol.enabled = true;
    }

    public void DamageCard(int damageAmount)
    {
        int remainingDamage = damageAmount - currentHealth;
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            assignedPlace.activeCard = null;

            if (isPlayer)
            {
                BattleController.instance.DamagePlayer(remainingDamage);

                if (isGraveyard)
                {
                    GraveyardPileController.instance.AddToGraveyardPile(cardSO);
                }
                else
                {
                    DiscardPileController.instance.AddToDiscardPile(cardSO);
                }
            }
            else
            {
                BattleController.instance.DamageEnemy(remainingDamage);
            }

            StartCoroutine(WaitJumpAfterDeadCo());
        }

        StartCoroutine(FadeHealthArtCo());
        anim.SetTrigger("Hurt");

        UpdateCardDisplay();
    }

    private IEnumerator FadeHealthArtCo()
    {
        Color originalColor = healthArt.color; // Orijinal rengi kaydedin
        Color fadedColor = originalColor;
        fadedColor.a = 0.5f; // Hedef saydamlık (ör. 0.5)

        float duration = 0.2f; // Saydamlaşma süresi
        float elapsedTime = 0f;

        // Yavaşça saydamlaştır
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthArt.color = Color.Lerp(originalColor, fadedColor, elapsedTime / duration);
            yield return null; // Bir sonraki frame'i bekle
        }

        // Bekleme süresi (tam saydam durumda kalma süresi)
        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0f;

        // Yavaşça eski haline dön
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthArt.color = Color.Lerp(fadedColor, originalColor, elapsedTime / duration);
            yield return null;
        }

        // Son renk ayarını garantiye alın
        healthArt.color = originalColor;
    }

    public IEnumerator WaitJumpAfterDeadCo()
    {
        yield return new WaitForSeconds(.2f);

        anim.SetTrigger("Jump");

        yield return new WaitForSeconds(.5f);

        if (isGraveyard)
        {
            MoveToPoint(BattleController.instance.graveyardPoint.position, BattleController.instance.graveyardPoint.rotation);
        }
        else
        {
            MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);
        }

        // Kartı discardPoint'e taşıdıktan sonra boyut küçültme işlemi
        yield return StartCoroutine(ScaleDownCo());

        Destroy(gameObject);
    }

    public IEnumerator ScaleDownCo()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 0.3f;

        float duration = 0.5f; // Küçülme süresi
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Son boyutu ayarla
    }

    public void UpdateCardDisplay()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = essenceCost.ToString();
    }

    private void UpdateAbilityDescription()
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
}