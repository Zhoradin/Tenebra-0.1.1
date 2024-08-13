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

    public bool isPlayer;

    public int currentHealth, attackPower, essenceCost;

    private int originalHealth, originalAttack, originalEssence;

    public TMP_Text healthText, attackText, costText, nameText, descriptionText, abilityDescriptionText, abilityDescriptionTextToo, superEffectiveText, notEffectiveText;

    public Image characterArt, bgArt, moonPhaseArt;

    public GameObject abilityDescription, abilityDescriptionToo;

    public CardType cardType;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public float selectedRotateSpeed = 720f;
    public float scaleSpeed = 5f;

    public bool inHand;
    public bool isActive;
    public int handPosition;

    private HandController theHC;

    private bool isSelected;
    private bool returningToHand;
    private Collider2D theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;

    public Animator anim;

    private Vector3 originalScale;
    private Vector3 targetScale;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

    [HideInInspector]
    public bool directHit, doubleTap, quickAttack, glassCannon, instaKill, multipleHit, mend, leech = false;

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

        /*healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = essenceCost.ToString();*/
        UpdateCardDisplay();

        nameText.text = cardSO.cardName;
        descriptionText.text = cardSO.cardDescription;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;

        cardType = cardSO.cardType;

        moonPhaseArt.sprite = cardSO.moonPhaseSprite;

        UpdateAbilityDescription();

        CheckMoonPhase();
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

            CheckForSuperEffectiveText();

            if (Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
            {
                ReturnToHand();
            }

            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.battleEnded == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, whatIsPlacement);

                if (hit.collider != null && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();
                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        if (BattleController.instance.playerEssence >= essenceCost)
                        {
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            MoveToPoint(selectedPoint.transform.position + new Vector3(0f, 0.75f, 0f), Quaternion.identity);

                            inHand = false;

                            isSelected = false;
                            returningToHand = false;

                            targetScale = originalScale;

                            theHC.RemoveCardFromHand(this);

                            ActivateAbility();

                            if (instaKill == true)
                            {
                                StartCoroutine(QuickAttackCoroutine());
                            }

                            BattleController.instance.SpendPlayerEssence(essenceCost);

                            isActive = true;

                            CheckMoonPhase();

                            theCol.enabled = true;
                        }
                        else
                        {
                            ReturnToHand();

                            UIController.instance.ShowEssenceWarning();
                        }
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
        if ((inHand || isActive) && !isSelected && isPlayer && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            targetScale = hoverScale;
            if (inHand)
            {
                Vector3 hoverPosition = theHC.cardPositions[handPosition] + new Vector3(0f, 1f, -2f);
                MoveToPoint(hoverPosition, targetRot);
                theHC.SpreadCards(handPosition, 1f);
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
        if ((inHand && !isActive) && !isSelected && isPlayer && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            targetScale = originalScale;
            MoveToPoint(theHC.cardPositions[handPosition], targetRot);

            abilityDescription.SetActive(false);
            abilityDescriptionToo.SetActive(false);

            theHC.ResetCardPositions();
        }
        else if (isActive && !isSelected && isPlayer && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            targetScale = originalScale;

            abilityDescription.SetActive(false);
            abilityDescriptionToo.SetActive(false);
        }

        superEffectiveText.gameObject.SetActive(false);
        notEffectiveText.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer && BattleController.instance.battleEnded == false && Time.timeScale != 0f && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            isSelected = true;
            theCol.enabled = false;
            targetRot = Quaternion.identity;
            targetScale = selectedScale;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        returningToHand = true;
        theCol.enabled = true;
        targetRot = theHC.cardRotations[handPosition];
        MoveToPoint(theHC.cardPositions[handPosition], targetRot);
        targetScale = originalScale;
    }

    public void DamageCard(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            assignedPlace.activeCard = null;

            if (isPlayer)
            {
                DiscardPileController.instance.AddToDiscardPile(cardSO);
            }

            StartCoroutine(WaitJumpAfterDeadCo());
        }

        anim.SetTrigger("Hurt");

        UpdateCardDisplay();
    }


    IEnumerator WaitJumpAfterDeadCo()
    {
        yield return new WaitForSeconds(.2f);

        anim.SetTrigger("Jump");



        yield return new WaitForSeconds(.5f);

        MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);

        Destroy(gameObject, 5f);
    }

    public void UpdateCardDisplay()
    {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = essenceCost.ToString();
    }

    public void ActivateAbility()
    {
        if (abilityDescription.activeSelf == true)
        {
            abilityDescription.SetActive(false);
        }
        if (abilityDescriptionToo.activeSelf == true)
        {
            abilityDescriptionToo.SetActive(false);
        }

        foreach (CardAbilitySO ability in cardSO.abilities)
        {
            switch (ability.abilityType)
            {
                case CardAbilitySO.AbilityType.Heal:
                    Heal(ability.value);
                    break;
                case CardAbilitySO.AbilityType.DirectHit:
                    DirectHit();
                    break;
                case CardAbilitySO.AbilityType.DoubleTap:
                    DoubleTap();
                    break;
                case CardAbilitySO.AbilityType.QuickAttack:
                    StartCoroutine(QuickAttackCoroutine());
                    break;
                case CardAbilitySO.AbilityType.GlassCannon:
                    GlassCannon();
                    break;
                case CardAbilitySO.AbilityType.Mend:
                    Mend();
                    break;
                case CardAbilitySO.AbilityType.Leech:
                    Leech();
                    break;
            }
        }
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


    private void Heal(int healAmount)
    {
        currentHealth += healAmount;

        UpdateCardDisplay();
    }


    private void DirectHit()
    {
        directHit = true;
    }

    private void DoubleTap()
    {
        doubleTap = true;
    }

    private void GlassCannon()
    {
        glassCannon = true;
    }

    private void Mend()
    {
        mend = true;
    }

    private void Leech()
    {
        leech = true;
    }

    public IEnumerator QuickAttackCoroutine()
    {
        quickAttack = true;
        yield return new WaitForSeconds(0.5f);
        if (isPlayer)
        {
            CardPointsController.instance.PlayerSingleCardAttack(this);
        }
        else
        {
            CardPointsController.instance.EnemySingleCardAttack(this);
        }
        
        quickAttack = false;
    }

    public void CheckMoonPhase()
    {
        if (cardSO.moonPhase == BattleController.instance.currentMoonPhase)
        {
            switch (cardSO.moonPhase)
            {
                case MoonPhase.NewMoon:
                    // No effect for NewMoon
                    break;

                case MoonPhase.WaxingCrescent:
                    // Increase health and attackPower by .33
                    currentHealth += Mathf.RoundToInt(currentHealth * .33f);
                    attackPower += Mathf.RoundToInt(attackPower * .33f);
                    UpdateCardDisplay();
                    break;

                case MoonPhase.FirstQuarter:
                    // Insta kill
                    instaKill = true;
                    break;

                case MoonPhase.WaxingGibbous:
                    // Decrease essence cost to half
                    essenceCost /= 2;
                    UpdateCardDisplay();
                    break;

                case MoonPhase.FullMoon:
                    // Reflect the damage
                    break;

                case MoonPhase.WaningGibbous:
                    // Increase overall essence by 1
                    if (isActive)
                    {
                        if (isPlayer)
                        {
                            BattleController.instance.PlayerGainEssence(1);
                        }
                        else
                        {
                            BattleController.instance.EnemyGainEssence(1);
                        }
                    }
                    break;

                case MoonPhase.LastQuarter:
                    // Attack 3 opponents
                    multipleHit = true;
                    break;

                case MoonPhase.WaningCrescent:
                    // Steal 1 health
                    break;

                default:
                    Debug.Log("Eşleşen bir moon phase yok.");
                    break;
            }
        }
        else
        {
            switch (cardSO.moonPhase)
            {
                case MoonPhase.NewMoon:
                    // No effect for NewMoon
                    break;

                case MoonPhase.WaxingCrescent:
                    // Convert health and attack to its original
                    currentHealth = originalHealth;
                    attackPower = originalAttack;
                    UpdateCardDisplay();
                    break;

                case MoonPhase.FirstQuarter:
                    // No effect
                    instaKill = false;
                    break;

                case MoonPhase.WaxingGibbous:
                    essenceCost = originalEssence;
                    UpdateCardDisplay();
                    break;

                case MoonPhase.FullMoon:
                    // No effect
                    break;

                case MoonPhase.WaningGibbous:
                    // No effect
                    break;

                case MoonPhase.LastQuarter:
                    multipleHit = false;
                    break;

                case MoonPhase.WaningCrescent:
                    // No effect
                    break;

                default:
                    Debug.Log("Eşleşen bir moon phase yok.");
                    break;
            }
        }
    }

    public void StealHealth(int stealAmount)
    {
        Heal(stealAmount);
    }
}