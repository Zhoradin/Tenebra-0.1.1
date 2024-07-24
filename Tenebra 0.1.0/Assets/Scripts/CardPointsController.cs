using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{
    public static CardPointsController instance;

    private void Awake()
    {
        instance = this;
    }

    public CardPlacePoint[] playerCardPoints, enemyCardPoints;

    public float timeBetweenAttacks = .25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCo());
    }

    IEnumerator PlayerAttackCo()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard != null)
            {
                int attackCount = playerCardPoints[i].activeCard.doubleTap ? 2 : 1;

                for (int j = 0; j < attackCount; j++)
                {
                    if (enemyCardPoints[i].activeCard != null && playerCardPoints[i].activeCard.direchHit == false)
                    {
                        // Attack the enemy card
                        float effectiveness = TypeEffectiveness.GetEffectiveness(playerCardPoints[i].activeCard.cardType, enemyCardPoints[i].activeCard.cardType);
                        float damage = playerCardPoints[i].activeCard.attackPower * effectiveness;
                        Debug.Log("Effectiveness: " + effectiveness);
                        enemyCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        if(enemyCardPoints[i].activeCard != null && enemyCardPoints[i].activeCard.mend == true)
                        {
                            enemyCardPoints[i].activeCard.currentHealth += Mathf.RoundToInt(damage/2);
                            enemyCardPoints[i].activeCard.UpdateCardDisplay();
                        }
                        if (enemyCardPoints[i].activeCard != null && enemyCardPoints[i].activeCard.leech == true)
                        {
                            BattleController.instance.enemyHealth += Mathf.RoundToInt(damage);
                            UIController.instance.SetEnemyHealthText(BattleController.instance.enemyHealth);
                            Debug.Log(BattleController.instance.enemyHealth);
                        }
                        BattleController.instance.SetupActiveCards();

                    List<int> targets = new List<int>();
                    // Add the primary target
                    if (enemyCardPoints[i].activeCard != null)
                    {
                        targets.Add(i);
                    }

                    // Add adjacent targets if multipleHit is true
                    if (playerCardPoints[i].activeCard.multipleHit)
                    {
                        if (i - 1 >= 0)
                        {
                            if (enemyCardPoints[i - 1].activeCard != null)
                            {
                                targets.Add(i - 1);
                            }
                            else
                            {
                                BattleController.instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);
                                playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
                                yield return new WaitForSeconds(0.5f);
                            }
                        }

                        if (i + 1 < enemyCardPoints.Length)
                        {
                            if (enemyCardPoints[i + 1].activeCard != null)
                            {
                                targets.Add(i + 1);
                            }
                            else
                            {
                                BattleController.instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);
                                playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
                                yield return new WaitForSeconds(0.5f);
                            }
                        }
                    }

                    foreach (int targetIndex in targets)
                    {
                        // Attack the enemy card
                        float effectiveness = TypeEffectiveness.GetEffectiveness(playerCardPoints[i].activeCard.cardType, enemyCardPoints[targetIndex].activeCard.cardType);
                        float damage = playerCardPoints[i].activeCard.attackPower * effectiveness;
                        Debug.Log("Effectiveness: " + effectiveness);

                        if (enemyCardPoints[targetIndex].activeCard.cardSO.moonPhase == BattleController.instance.currentMoonPhase && enemyCardPoints[targetIndex].activeCard.cardSO.moonPhase == MoonPhase.FullMoon)
                        {
                            enemyCardPoints[targetIndex].activeCard.DamageCard(0);
                            playerCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        }
                        else
                        {
                            enemyCardPoints[targetIndex].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        }
                        BattleController.instance.SetupActiveCards();

                        if (playerCardPoints[i].activeCard != null)
                        {
                            if (playerCardPoints[i].activeCard.cardSO.moonPhase == BattleController.instance.currentMoonPhase)
                            {
                                if (playerCardPoints[i].activeCard.cardSO.moonPhase == MoonPhase.WaningCrescent)
                                {
                                    playerCardPoints[i].activeCard.StealHealth(1);
                                }
                                else if (playerCardPoints[i].activeCard.cardSO.moonPhase == MoonPhase.FirstQuarter)
                                {
                                    enemyCardPoints[targetIndex].activeCard.currentHealth = 0;
                                }
                            }
                        }

                        playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (enemyCardPoints[i].activeCard == null)
                    {
                        // Attack the enemy's overall health
                        BattleController.instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);
                        playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
                        yield return new WaitForSeconds(0.5f);
                        playerCardPoints[i].activeCard.directHit = false;
                    }

                    if (playerCardPoints[i].activeCard != null && !playerCardPoints[i].activeCard.multipleHit)
                    {
                        playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
                        yield return new WaitForSeconds(0.5f);
                    }

                    yield return new WaitForSeconds(timeBetweenAttacks);
                }
            }

            if (BattleController.instance.battleEnded == true)
            {
                break;
            }
        }

        CheckAssignedCards();

        BattleController.instance.AdvanceTurn();
    }


    public void PlayerSingleCardAttack(Card card)
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (playerCardPoints[i].activeCard == card)
            {
                if (enemyCardPoints[i].activeCard != null && card.directHit == false)
                {
                    if(card.instaKill == true)
                    {
                        enemyCardPoints[i].activeCard.DamageCard(100);
                    }
                    else
                    {
                        // Attack the enemy card
                        float effectiveness = TypeEffectiveness.GetEffectiveness(card.cardType, enemyCardPoints[i].activeCard.cardType);
                        float damage = card.attackPower * effectiveness;
                        Debug.Log("Effectiveness: " + effectiveness);
                        enemyCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        BattleController.instance.SetupActiveCards();
                    }
                } 
                else
                {
                    // Attack the enemy's overall health
                    BattleController.instance.DamageEnemy(card.attackPower);
                    card.directHit = false;
                }

                card.anim.SetTrigger("Attack");

                break;
            }
        }
    }

    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCo());
    }

    IEnumerator EnemyAttackCo()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        for (int i = 0; i < enemyCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard != null)
            {
                int attackCount = enemyCardPoints[i].activeCard.doubleTap ? 2 : 1;

                for (int j = 0; j < attackCount; j++)
                {
                    if (playerCardPoints[i].activeCard != null && enemyCardPoints[i].activeCard.directHit == false)
                    {
                        // Attack the player card
                        float effectiveness = TypeEffectiveness.GetEffectiveness(enemyCardPoints[i].activeCard.cardType, playerCardPoints[i].activeCard.cardType);
                        float damage = enemyCardPoints[i].activeCard.attackPower * effectiveness;

                        Debug.Log("Effectiveness: " + effectiveness);
                        playerCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        if(playerCardPoints[i].activeCard != null && playerCardPoints[i].activeCard.mend == true)
                        {
                            playerCardPoints[i].activeCard.currentHealth += Mathf.RoundToInt(damage/2);
                            playerCardPoints[i].activeCard.UpdateCardDisplay();
                        }

                         if (playerCardPoints[i].activeCard != null && playerCardPoints[i].activeCard.leech == true)
                        {
                            BattleController.instance.playerHealth += Mathf.RoundToInt(damage);
                            UIController.instance.SetPlayerHealthText(BattleController.instance.playerHealth);
                            Debug.Log(BattleController.instance.playerHealth);
                        }
                        BattleController.instance.SetupActiveCards();
                        if (enemyCardPoints[i].activeCard.cardSO.moonPhase == BattleController.instance.currentMoonPhase)

                        //Debug.Log("Effectiveness: " + effectiveness);
                        if (playerCardPoints[i].activeCard.cardSO.moonPhase == BattleController.instance.currentMoonPhase && playerCardPoints[i].activeCard.cardSO.moonPhase == MoonPhase.FullMoon)

                        {
                            playerCardPoints[i].activeCard.DamageCard(0);
                            enemyCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        }
                        else
                        {
                            playerCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        }
                        BattleController.instance.SetupActiveCards();

                        if (enemyCardPoints[i].activeCard != null)
                        {
                            if (enemyCardPoints[i].activeCard.cardSO.moonPhase == BattleController.instance.currentMoonPhase)
                            {
                                if (enemyCardPoints[i].activeCard.cardSO.moonPhase == MoonPhase.WaningCrescent)
                                {
                                    enemyCardPoints[i].activeCard.StealHealth(1);
                                }
                                else if (enemyCardPoints[i].activeCard.cardSO.moonPhase == MoonPhase.FirstQuarter)
                                {
                                    playerCardPoints[i].activeCard.currentHealth = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("attacking overall health");
                        // Attack the player's overall health
                        BattleController.instance.DamagePlayer(enemyCardPoints[i].activeCard.attackPower);
                        enemyCardPoints[i].activeCard.directHit = false;
                    }
                    if (enemyCardPoints[i].activeCard != null)
                    {
                        enemyCardPoints[i].activeCard.anim.SetTrigger("Enemy Attack");
                    }

                    yield return new WaitForSeconds(timeBetweenAttacks);
                }
            }

            if (BattleController.instance.battleEnded == true)
            {
                i = enemyCardPoints.Length;
            }
        }

        CheckAssignedCards();

        BattleController.instance.AdvanceTurn();
    }


    public void EnemySingleCardAttack(Card card)
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard == card)
            {
                if (playerCardPoints[i].activeCard != null && card.directHit == false)
                {
                    if (card.instaKill == true)
                    {
                        playerCardPoints[i].activeCard.DamageCard(100);
                    }
                    else
                    {
                        // Attack the enemy card
                        float effectiveness = TypeEffectiveness.GetEffectiveness(card.cardType, playerCardPoints[i].activeCard.cardType);
                        float damage = card.attackPower * effectiveness;
                        Debug.Log("Effectiveness: " + effectiveness);
                        playerCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                        BattleController.instance.SetupActiveCards();
                    }
                }
                else
                {
                    // Attack the enemy's overall health
                    BattleController.instance.DamagePlayer(card.attackPower);
                    card.directHit = false;
                }

                card.anim.SetTrigger("Enemy Attack");

                break;
            }
        }
    }

    public void CheckAssignedCards()
    {
        foreach(CardPlacePoint point in enemyCardPoints)
        {
            if(point.activeCard != null)
            {
                if (point.activeCard.currentHealth <= 0)
                {
                    point.activeCard = null;
                }
            }      
        }

        foreach (CardPlacePoint point in playerCardPoints)
        {
            if (point.activeCard != null)
            {
                if (point.activeCard.currentHealth <= 0)
                {
                    point.activeCard = null;
                }
            }
        }
    }
}
