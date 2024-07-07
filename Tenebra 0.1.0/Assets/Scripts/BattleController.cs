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

    public enum TurnOrder { playerActive, playerCardAttacks, enemyActive, enemyCardAttacks}
    public TurnOrder currentPhase;

    public Transform discardPoint;

    public int playerHealth, enemyHealth;

    public bool battleEnded;

    public float resultScreenDelayTime = 1f;
    
    [Range(0f, 1f)]
    public float playerFirstChance = .5f;

    // Start is called before the first frame update
    void Start()
    {
        //playerEssence = startingEssenceAmount;
        //UIController.instance.SetPlayerEssenceText(playerEssence);
        FillPlayerEssence();
        FillEnemyEssence();

        UIController.instance.SetPlayerHealthText(playerHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);

        if(Random.value > playerFirstChance)
        {
            currentPhase = TurnOrder.playerCardAttacks;
            AdvanceTurn();
        }
        else
        {
            DeckController.instance.DrawMultipleCards(startingCardsAmount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTurn();
        }
    }

    public void SpendPlayerEssence(int amountToSpend)
    {
        playerEssence = playerEssence - amountToSpend;

        if(playerEssence < 0)
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
        UIController.instance.SetEnemyEssenceText(playerEssence);
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
                    UIController.instance.endTurnButton.SetActive(true);
                    UIController.instance.drawCardButton.GetComponent<Button>().interactable = true;

                    FillPlayerEssence();
                    HandController.instance.EmptyHand();
                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);

                    break;

                case TurnOrder.playerCardAttacks:
                    CardPointsController.instance.PlayerAttack();
                    break;

                case TurnOrder.enemyActive:
                    FillEnemyEssence();
                    EnemyController.instance.StartAction();
                    break;

                case TurnOrder.enemyCardAttacks:
                    CardPointsController.instance.EnemyAttack();
                    break;
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
        if(playerHealth > 0 || battleEnded == false)
        {
            playerHealth -= damageAmount;

            if(playerHealth <= 0)
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

                //End Battle
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

        if(enemyHealth <= 0)
        {
            UIController.instance.battleResultText.text = "YOU WON!";

            foreach(CardPlacePoint point in CardPointsController.instance.enemyCardPoints)
            {
                if(point.activeCard != null)
                {
                    point.activeCard.MoveToPoint(discardPoint.position, Quaternion.identity);
                }
            }
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
        }

        StartCoroutine(ShowResultsCo());
    }

    IEnumerator ShowResultsCo()
    {
        yield return new WaitForSeconds(resultScreenDelayTime);

        UIController.instance.battleEndedScreen.SetActive(true);
    }
}
