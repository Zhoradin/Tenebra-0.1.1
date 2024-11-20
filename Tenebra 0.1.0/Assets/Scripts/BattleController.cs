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

    public Transform discardPoint, graveyardPoint;

    public int playerHealth, enemyHealth;

    public bool battleEnded;

    public float resultScreenDelayTime = 1f;

    
    public int turnCount = 0;

    [Range(0f, 1f)]
    public float playerFirstChance = .5f;

    [HideInInspector]
    public CardPlacePoint[] playerCardPoints, enemyCardPoints;

    void Start()
    {
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

        SetupActiveCards();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
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

    public void SetupActiveCards()
    {
        playerCardPoints = CardPointsController.instance.playerCardPoints;
        enemyCardPoints = CardPointsController.instance.enemyCardPoints;
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
                    HandController.instance.EmptyHand();
                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);
                    // Check moon phase for player cards
                    CheckMoonPhaseForAllCards(playerCardPoints);
                    CheckMoonPhaseForAllCards(enemyCardPoints);
                    turnCount++;
                    AbilityManager.instance.MetamorphoseCard();
                    AbilityManager.instance.ProcessDecayDamage();

                    // Growth yeteneği için kontrol (oyuncu kartları)
                    AbilityManager.instance.ApplyGrowthAbility(CardPointsController.instance.playerCardPoints);
                    break;

                case TurnOrder.playerCardAttacks:
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
                    FillEnemyEssence();
                    EnemyController.instance.StartAction();
                    // Check moon phase for enemy cards
                    CheckMoonPhaseForAllCards(enemyCardPoints);
                    AbilityManager.instance.MetamorphoseCard();
                    AbilityManager.instance.ProcessDecayDamage();

                    // Growth yeteneği için kontrol (düşman kartları)
                    AbilityManager.instance.ApplyGrowthAbility(CardPointsController.instance.enemyCardPoints);
                    break;

                case TurnOrder.enemyCardAttacks:
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

                //End Battle
                EndBattle();
            }

            UIController.instance.SetPlayerHealthText(playerHealth);

            UIDamageIndicator damageClone = Instantiate(UIController.instance.playerDamage, UIController.instance.playerDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
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
                if (point.activeCard != null)
                {
                    point.activeCard.MoveToPoint(discardPoint.position, Quaternion.identity);
                }
            }

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
