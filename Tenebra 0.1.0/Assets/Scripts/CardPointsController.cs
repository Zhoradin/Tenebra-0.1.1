using System.Collections;
using System.Collections.Generic;
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

        for(int i = 0; i < playerCardPoints.Length; i++)
        {
            if(playerCardPoints[i].activeCard != null)
            {
                if (enemyCardPoints[i].activeCard != null && playerCardPoints[i].activeCard.direchHit == false)
                {
                    //Attack the enemy card
                    float effectiveness = TypeEffectiveness.GetEffectiveness(playerCardPoints[i].activeCard.cardType, enemyCardPoints[i].activeCard.cardType);
                    float damage = playerCardPoints[i].activeCard.attackPower * effectiveness;
                    Debug.Log("Effectiveness: " + effectiveness);
                    enemyCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                }
                else
                {
                    //Attack the enemy's overall health
                    BattleController.instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);

                    Card.instance.direchHit = false;
                }

                playerCardPoints[i].activeCard.anim.SetTrigger("Attack");

                yield return new WaitForSeconds(timeBetweenAttacks);
            }

            if(BattleController.instance.battleEnded == true)
            {
                i = playerCardPoints.Length;
            }
        }

        CheckAssignedCards();

        BattleController.instance.AdvanceTurn();
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
                if (playerCardPoints[i].activeCard != null && enemyCardPoints[i].activeCard.direchHit == false)
                {
                    //Attack the player card
                    float effectiveness = TypeEffectiveness.GetEffectiveness(enemyCardPoints[i].activeCard.cardType, playerCardPoints[i].activeCard.cardType);
                    float damage = enemyCardPoints[i].activeCard.attackPower * effectiveness;
                    Debug.Log("Effectiveness: " + effectiveness);
                    playerCardPoints[i].activeCard.DamageCard(Mathf.RoundToInt(damage));
                }
                else
                {
                    BattleController.instance.DamagePlayer(enemyCardPoints[i].activeCard.attackPower);
                }

                enemyCardPoints[i].activeCard.anim.SetTrigger("Enemy Attack");

                yield return new WaitForSeconds(timeBetweenAttacks);
            }

            if (BattleController.instance.battleEnded == true)
            {
                i = enemyCardPoints.Length;
            }
        }

        CheckAssignedCards();

        BattleController.instance.AdvanceTurn();
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
