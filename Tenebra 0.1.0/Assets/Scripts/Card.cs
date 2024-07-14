using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public TMP_Text healthText, attackText, costText, nameText, descriptionText, abilityDescriptionText;

    public Image characterArt, bgArt;

    public GameObject abilityDescription, abilityDescriptionToo;

    public CardType cardType;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public float selectedRotateSpeed = 720f; // Seçildiðinde kullanýlacak olan rotation hýzý
    public float scaleSpeed = 5f; // Ölçek deðiþtirme hýzý

    public bool inHand;
    public int handPosition;

    private HandController theHC;

    private bool isSelected;
    private bool returningToHand; // Ele dönerken farklý hýz kullanýmý için
    private Collider2D theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private bool justPressed;

    public CardPlacePoint assignedPlace;

    public Animator anim;

    private Vector3 originalScale;
    private Vector3 targetScale;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f); // Kartýn üzerine gelindiðinde büyüme oraný
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f); // Kart seçildiðinde büyüme oraný

    public bool direchHit = false;

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
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        essenceCost = cardSO.essenceCost;

        /*healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = essenceCost.ToString();*/
        UpdateCardDisplay();

        nameText.text = cardSO.cardName;
        descriptionText.text = cardSO.cardDescription;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;

        cardType = cardSO.cardType;

        UpdateAbilityDescription();
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
            MoveToPoint(worldPosition + new Vector3(0f, 2f, -4f), Quaternion.identity); // Hareket ederken rotasyonu sýfýrlar

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

                            if (abilityDescription.activeSelf == true)
                            {
                                abilityDescription.SetActive(false);
                            }

                            PlayCard();

                            BattleController.instance.SpendPlayerEssence(essenceCost);
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
        float currentRotateSpeed = isSelected || returningToHand ? selectedRotateSpeed : rotateSpeed; // Seçildiðinde veya ele dönerken farklý rotation hýzý kullan
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, currentRotateSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);

        justPressed = false;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    private void OnMouseOver()
    {
        if (inHand && !isSelected && isPlayer && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            targetScale = hoverScale;
            Vector3 hoverPosition = theHC.cardPositions[handPosition] + new Vector3(0f, 1f, -2f);
            MoveToPoint(hoverPosition, targetRot); // Mevcut rotasyonu kullanarak pozisyonu deðiþtir

            if (Time.timeScale != 0f && cardSO.abilities.Length > 0)
            {
                // Açýklama metnini güncelleyerek göster
                abilityDescription.SetActive(true);
                abilityDescriptionText.text = abilityDescriptionText.text;
            }
        }
    }

    private void OnMouseExit()
    {
        if (inHand && !isSelected && isPlayer && BattleController.instance.battleEnded == false && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            targetScale = originalScale;
            MoveToPoint(theHC.cardPositions[handPosition], targetRot);

            // Açýklama metnini gizle
            abilityDescription.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer && BattleController.instance.battleEnded == false && Time.timeScale != 0f && UIController.instance.drawPileOpen == false && UIController.instance.discardPileOpen == false)
        {
            isSelected = true;
            theCol.enabled = false;
            targetRot = Quaternion.identity; // Seçildiðinde hedef rotasyonu sýfýrla
            targetScale = selectedScale;

            justPressed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        returningToHand = true; // Ele dönerken durumu ayarlayýn
        theCol.enabled = true;
        targetRot = theHC.cardRotations[handPosition]; // El pozisyonundaki rotasyonu geri yükle
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

            DiscardPileController.instance.AddToDiscardPile(cardSO);

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

    public void PlayCard()
    {
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
            }
        }
    }

    private void UpdateAbilityDescription()
    {
        string abilityDesc = "";
        foreach (CardAbilitySO ability in cardSO.abilities)
        {
            abilityDesc += ability.abilityType + "\n" + ability.description;
        }
        abilityDescriptionText.text = abilityDesc;
    }

    private void Heal(int healAmount)
    {
        currentHealth += healAmount;

        UpdateCardDisplay();
    }


    private void DirectHit()
    {
        direchHit = true;
    }
}