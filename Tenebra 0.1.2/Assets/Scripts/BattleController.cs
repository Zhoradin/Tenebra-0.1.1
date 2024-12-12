using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    private void Awake()
    {
        instance = this;
    }

    public int startingEssenceAmount = 4, maxEssence = 12;
    public int playerEssence, enemyEssence;

    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 2;

    public enum TurnOrder { playerActive, playerCardAttacks, enemyActive, enemyCardAttacks }
    public TurnOrder currentPhase;
    public MoonPhase currentMoonPhase;
    public int moonPhaseCount = 0;

    public Transform discardPoint, enemyDiscardPoint, graveyardPoint;

    public int playerHealth, enemyHealth;

    public bool battleEnded;

    public float resultScreenDelayTime = 1f;

    private List<Card> lostSideCards = new List<Card>();
    
    public int turnCount = 0;

    [Range(0f, 1f)]
    public float playerFirstChance = .5f;

    [HideInInspector]
    public CardPlacePoint[] playerCardPoints, enemyCardPoints;

    void Start()
    {
        if (DataCarrier.instance.enemy != null)
        {
            AudioManager.instance.PullBattleMusic();
        }

        // Verileri DataCarrier'dan al
        if (DataCarrier.instance != null)
        {
            playerHealth = DataCarrier.instance.playerHealth;
            playerEssence = DataCarrier.instance.playerEssence;
        }

        FillPlayerEssence();
        FillEnemyEssence();

        if (currentPhase == TurnOrder.enemyActive)
        {
            DeckController.instance.DrawMultipleCards(startingCardsAmount);
        }

        UIController.instance.SetPlayerHealthText(playerHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);

        if (Random.value > playerFirstChance)
        {
            currentPhase = TurnOrder.playerCardAttacks;
            AdvanceTurn();
        }
        else
        {
            currentPhase = TurnOrder.enemyCardAttacks;
            AdvanceTurn();
        }
    }

    public void SetEnemyHealth()
    {
        enemyHealth = EnemyController.instance.enemyHealth;
        UIController.instance.SetEnemyHealthText(EnemyController.instance.enemyHealth);
    }

    public void SetEnemyEssence()
    {
        enemyEssence = EnemyController.instance.enemyEssence;
        UIController.instance.SetEnemyEssenceText(EnemyController.instance.enemyEssence);
    }

    public void SpendPlayerEssence(int amountToSpend)
    {
        playerEssence = playerEssence - amountToSpend;

        if (playerEssence < 0)
        {
            playerEssence = 0;
        }

        UIController.instance.SetPlayerEssenceText(playerEssence);
    }

    public void FillPlayerEssence()
    {
        playerEssence = startingEssenceAmount;
        UIController.instance.SetPlayerEssenceText(playerEssence);
    }

    public void SpendEnemyEssence(int amountToSpend)
    {
        enemyEssence = enemyEssence - amountToSpend;

        if (enemyEssence < 0)
        {
            enemyEssence = 0;
        }

        UIController.instance.SetEnemyEssenceText(enemyEssence);
    }

    public void FillEnemyEssence()
    {
        enemyEssence = startingEssenceAmount;
        UIController.instance.SetEnemyEssenceText(enemyEssence);
    }

    public void PlayerGainEssence(int essenceAmount)
    {
        playerEssence += essenceAmount;
        UIController.instance.SetPlayerEssenceText(playerEssence);
    }

    public void EnemyGainEssence(int essenceAmount)
    {
        Debug.Log("arttı");
        enemyEssence += essenceAmount;
        UIController.instance.SetEnemyEssenceText(enemyEssence);
    }

    public void AdvanceTurn()
    {
        if (battleEnded == false)
        {
            currentPhase++;
            if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
            {
                currentPhase = 0;
            }

            switch (currentPhase)
            {
                case TurnOrder.playerActive:

                    MoonPhaseController.instance.AdvanceMoonPhase();
                    moonPhaseCount++;
                    if (moonPhaseCount > 15)
                    {
                        moonPhaseCount = 0;
                        currentMoonPhase = MoonPhase.NewMoon;
                    }
                    MoonPhaseController.instance.UpdateMoonPhase();
                    if ((int)currentMoonPhase >= System.Enum.GetValues(typeof(MoonPhase)).Length)
                    {
                        currentMoonPhase = 0;
                    }
                    UIController.instance.endTurnButton.SetActive(true);
                    UIController.instance.drawCardButton.GetComponent<Button>().interactable = true;

                    FillPlayerEssence();

                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);
                    if (EnemyController.instance.isStarting == false)
                    {
                        EnemyController.instance.SetupHand();
                    }
                    CheckMoonPhaseForAllCards(CardPointsController.instance.playerCardPoints);
                    CheckMoonPhaseForAllCards(CardPointsController.instance.enemyCardPoints);
                    PlayerAbilityControl();
                    turnCount++;

                    StartCoroutine(WaitForButtonAvailabilityCo());
                    break;

                case TurnOrder.playerCardAttacks:

                    HandController.instance.EmptyHand();
                    UIController.instance.drawCardButton.GetComponent<Button>().interactable = false;
                    UIController.instance.endTurnButton.GetComponent<Button>().interactable = false;
                    if (turnCount >= 2)
                    {
                        CardPointsController.instance.PlayerAttack();
                    }
                    else
                    {
                        AdvanceTurn();
                    }
                    break;

                case TurnOrder.enemyActive:

                    UIController.instance.drawCardButton.GetComponent<Button>().interactable = false;
                    UIController.instance.endTurnButton.GetComponent<Button>().interactable = false;
                    FillEnemyEssence();
                    StartCoroutine(WaitForEnemyHandCo());
                    break;

                case TurnOrder.enemyCardAttacks:
                    EnemyHandController.instance.EmptyHand();
                    EnemyController.instance.isStarting = false;
                    if (turnCount >= 2)
                    {
                        CardPointsController.instance.EnemyAttack();
                    }
                    else
                    {
                        AdvanceTurn();
                    }
                    break;
            }

            if (currentPhase == TurnOrder.enemyActive || currentPhase == TurnOrder.playerActive)
            {
                if (turnCount < 2)
                {
                    turnCount++;
                }
            }
        }
    }
    public void CheckMoonPhaseForAllCards(CardPlacePoint[] cardPoints)
    {
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null)
            {
                MoonPhaseController.instance.CheckMoonPhase(point.activeCard);
            }
        }
    }

    public void PlayerAbilityControl()
    {
        AbilityManager.instance.ApplyBenevolenceEffect(CardPointsController.instance.playerCardPoints);
        AbilityManager.instance.ApplyDuality(CardPointsController.instance.playerCardPoints);
        AbilityManager.instance.ApplyDuality(CardPointsController.instance.enemyCardPoints);
        AbilityManager.instance.MetamorphoseCard();
        AbilityManager.instance.ProcessDecayDamage();
        AbilityManager.instance.CheckStun(CardPointsController.instance.enemyCardPoints);
        AbilityManager.instance.ApplyHarvesterAbility(CardPointsController.instance.playerCardPoints);
        AbilityManager.instance.ApplyGrowthAbility(CardPointsController.instance.playerCardPoints);
        AbilityManager.instance.CheckSwitch(CardPointsController.instance.playerCardPoints);
    }

    public void EnemyAbilityControl()
    {
        AbilityManager.instance.ApplyBenevolenceEffect(CardPointsController.instance.enemyCardPoints);
        AbilityManager.instance.MetamorphoseCard();
        AbilityManager.instance.ProcessDecayDamage();
        AbilityManager.instance.CheckStun(CardPointsController.instance.playerCardPoints);
        AbilityManager.instance.ApplyDuality(CardPointsController.instance.playerCardPoints);
        AbilityManager.instance.ApplyDuality(CardPointsController.instance.enemyCardPoints);
        AbilityManager.instance.ApplyGrowthAbility(CardPointsController.instance.enemyCardPoints);
        AbilityManager.instance.ApplyHarvesterAbility(CardPointsController.instance.enemyCardPoints);
        AbilityManager.instance.CheckSwitch(CardPointsController.instance.enemyCardPoints);
    }

    public IEnumerator WaitForButtonAvailabilityCo()
    {
        yield return new WaitForSeconds(1.5f);
        UIController.instance.drawCardButton.GetComponent<Button>().interactable = true;
        UIController.instance.endTurnButton.GetComponent<Button>().interactable = true;
        UIController.instance.isEndTurnKeyActive = true;
    }

    public IEnumerator WaitForEnemyHandCo()
    {
        yield return new WaitForSeconds(1f);
        EnemyController.instance.StartAction();
        CheckMoonPhaseForAllCards(enemyCardPoints);
        EnemyAbilityControl();
    }

    public void EndPlayerTurn()
    {
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.GetComponent<Button>().interactable = false;

        AdvanceTurn();
    }

    public void DamagePlayer(int damageAmount)
    {
        if (playerHealth > 0 || battleEnded == false)
        {
            playerHealth -= damageAmount;

            if (playerHealth <= 0)
            {
                playerHealth = 0;

                EndBattle();
            }

            UIController.instance.SetPlayerHealthText(playerHealth);

            UIDamageIndicator damageClone = Instantiate(UIController.instance.playerDamage, UIController.instance.playerDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);

            AudioManager.instance.PlaySFX(5);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 || battleEnded == false)
        {
            enemyHealth -= damageAmount;

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;

                EndBattle();
            }

            UIController.instance.SetEnemyHealthText(enemyHealth);

            UIDamageIndicator damageClone = Instantiate(UIController.instance.enemyDamage, UIController.instance.enemyDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);

            AudioManager.instance.PlaySFX(6);
        }
    }

    void EndBattle()
    {
        battleEnded = true;

        HandController.instance.EmptyHand();

        if (enemyHealth <= 0)
        {
            DataCarrier.instance.playerCoin += DataCarrier.instance.enemy.gainedCoin;
            UIController.instance.UpdateCoinAmountText();

            UIController.instance.battleResultText.text = "YOU WON!";

            foreach (CardPlacePoint point in CardPointsController.instance.enemyCardPoints)
            {
                lostSideCards.Clear();
                if (point.activeCard != null)
                {
                    point.activeCard.MoveToPoint(enemyDiscardPoint.position, Quaternion.identity);
                    lostSideCards.Add(point.activeCard);
                }
            }
            StartCoroutine(DestroyLostSideCardsCo());
            CardSelectController.instance.ShowRandomCards();
        }
        else
        {
            UIController.instance.battleResultText.text = "YOU LOST!";

            foreach (CardPlacePoint point in CardPointsController.instance.playerCardPoints)
            {
                if (point.activeCard != null)
                {
                    point.activeCard.MoveToPoint(discardPoint.position, Quaternion.identity);
                }
            }

            StartCoroutine(ShowResultsCo());
        }
    }

    private IEnumerator DestroyLostSideCardsCo()
    {
        yield return new WaitForSeconds(1f);
        foreach (Card card in lostSideCards)
        {
            StartCoroutine(card.ScaleDownCo());
        }
    }

    public IEnumerator ShowResultsCo()
    {
        yield return StartCoroutine(CardSelectController.instance.SlideMenuToOriginalPosition());

        yield return new WaitForSeconds(resultScreenDelayTime);

        if (UIController.instance.drawPilePanel.activeInHierarchy == true)
        {
            UIController.instance.OpenDrawPile();
        }

        if (UIController.instance.discardPilePanel.activeInHierarchy == true)
        {
            UIController.instance.OpenDiscardPile();
        }

        UIController.instance.resultScreen.SetActive(true);

        // Verileri DataCarrier'a kaydet
        if (DataCarrier.instance != null)
        {
            DataCarrier.instance.UpdatePlayerEssence(playerEssence);
            DataCarrier.instance.UpdatePlayerHealth(playerHealth);
        }
    }

    public void SetPlayerHealth(int health)
    {
        DataCarrier.instance.UpdatePlayerHealth(health);
    }

    public void SetPlayerEssence(int essence)
    {
        DataCarrier.instance.UpdatePlayerEssence(essence);
    }

    public void UpdateDataCarrier()
    {
        if (DataCarrier.instance != null)
        {
            DataCarrier.instance.UpdatePlayerHealth(playerHealth);
            DataCarrier.instance.UpdatePlayerEssence(playerEssence);
        }
    }

    public void LoadData(PlayerData data)
    {
        // Mevcut health ve essence'ı güncelle
        SetPlayerHealth(data.health);
        SetPlayerEssence(data.essence);
    }

    public void SaveData(PlayerData data)
    {
        // DataCarrier'dan health ve essence'ı al
        data.health = DataCarrier.instance.playerHealth;
        data.essence = DataCarrier.instance.playerEssence;
    }
}
