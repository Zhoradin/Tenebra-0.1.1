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
                    List<int> targets = GetTargets(i, playerCardPoints, enemyCardPoints, playerCardPoints[i].activeCard.multipleHit);
                    foreach (int targetIndex in targets)
                    {
                        yield return PerformAttack(playerCardPoints[i], enemyCardPoints[targetIndex], targetIndex);
                    }

                    if (targets.Count == 0)
                    {
                        float damage = playerCardPoints[i].activeCard.attackPower;
                        BattleController.instance.DamageEnemy(Mathf.RoundToInt(damage));
                        playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
                        yield return new WaitForSeconds(0.5f);
                    }

                    yield return new WaitForSeconds(timeBetweenAttacks);
                }
            }

            if (BattleController.instance.battleEnded)
            {
                break;
            }
        }

        CheckAssignedCards();
        BattleController.instance.AdvanceTurn();
    }

    List<int> GetTargets(int index, CardPlacePoint[] attackingPoints, CardPlacePoint[] defendingPoints, bool multipleHit)
    {
        List<int> targets = new List<int>();

        // Guardian özelliði kontrolü
        int guardianIndex = -1;
        for (int i = 0; i < defendingPoints.Length; i++)
        {
            if (defendingPoints[i].activeCard != null && defendingPoints[i].activeCard.guardian)
            {
                guardianIndex = i;
                break;
            }
        }

        // Eðer Guardian varsa
        if (guardianIndex != -1)
        {
            if (multipleHit)
            {
                // Multiple hit özelliði aktifse Guardian üç kez hedeflenir
                targets.Add(guardianIndex);
                targets.Add(guardianIndex);
                targets.Add(guardianIndex);
            }
            else
            {
                // Normal saldýrý için Guardian bir kez hedeflenir
                targets.Add(guardianIndex);
            }
            return targets;
        }

        // Guardian yoksa hedefleme normal þekilde devam eder
        if (multipleHit)
        {
            // Önce sol slot
            if (index - 1 >= 0)
            {
                targets.Add(index - 1);
            }
        }

        // Ana hedef (karþýdaki slot)
        targets.Add(index);

        if (multipleHit)
        {
            // Son olarak sað slot
            if (index + 1 < defendingPoints.Length)
            {
                targets.Add(index + 1);
            }
        }

        return targets;
    }

    IEnumerator PerformAttack(CardPlacePoint attacker, CardPlacePoint defender, int targetIndex)
    {
        float damage = attacker.activeCard.attackPower;

        if (defender.activeCard == null)
        {
            // Slot boþ, ana düþmana hasar ver
            if (defender == playerCardPoints[targetIndex])
            {
                BattleController.instance.DamagePlayer(Mathf.RoundToInt(damage));
            }
            else if (defender == enemyCardPoints[targetIndex])
            {
                BattleController.instance.DamageEnemy(Mathf.RoundToInt(damage));
            }
        }
        else
        {
            // Slot dolu, karta hasar ver
            float effectiveness = TypeEffectiveness.GetEffectiveness(attacker.activeCard.cardType, defender.activeCard.cardType);
            damage *= effectiveness;

            defender.activeCard.DamageCard(Mathf.RoundToInt(damage));

            // Decay özelliðini tetikleyin
            if (attacker.activeCard.decay)
            {
                AbilityManager.instance.DecayCard(attacker.activeCard, defender.activeCard);
            }

            if (defender.activeCard != null)
            {
                if (defender.activeCard.mend)
                {
                    defender.activeCard.currentHealth += Mathf.RoundToInt(damage / 2);
                    defender.activeCard.UpdateCardDisplay();
                }

                if (defender.activeCard.leech)
                {
                    BattleController.instance.enemyHealth += Mathf.RoundToInt(damage);
                    UIController.instance.SetEnemyHealthText(BattleController.instance.enemyHealth);
                }
            }
        }

        // Saldýrý animasyonu
        if (attacker.activeCard != null)
        {
            if (attacker.activeCard.isPlayer)
            {
                attacker.activeCard.anim.SetTrigger("Attack");
            }
            else
            {
                attacker.activeCard.anim.SetTrigger("Enemy Attack");
            }
        }

        yield return new WaitForSeconds(0.5f);
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
                    List<int> targets = GetTargets(i, enemyCardPoints, playerCardPoints, enemyCardPoints[i].activeCard.multipleHit);
                    foreach (int targetIndex in targets)
                    {
                        yield return PerformAttack(enemyCardPoints[i], playerCardPoints[targetIndex], targetIndex);
                    }

                    if (targets.Count == 0)
                    {
                        float damage = enemyCardPoints[i].activeCard.attackPower;
                        BattleController.instance.DamagePlayer(Mathf.RoundToInt(damage));
                        enemyCardPoints[i].activeCard.anim.SetTrigger("Enemy Attack");
                        yield return new WaitForSeconds(0.5f);
                    }

                    yield return new WaitForSeconds(timeBetweenAttacks);
                }
            }

            if (BattleController.instance.battleEnded)
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
                PerformSingleCardAttack(card, enemyCardPoints[i], playerCardPoints[i].activeCard != null && card.directHit == false);
                break;
            }
        }
    }

    public void EnemySingleCardAttack(Card card)
    {
        for (int i = 0; i < playerCardPoints.Length; i++)
        {
            if (enemyCardPoints[i].activeCard == card)
            {
                PerformSingleCardAttack(card, playerCardPoints[i], playerCardPoints[i].activeCard != null && card.directHit == false);
                break;
            }
        }
    }

    void PerformSingleCardAttack(Card attacker, CardPlacePoint defender, bool attackCard)
    {
        if (attackCard)
        {
            if(defender.activeCard != null)
            {
                if (attacker.instaKill)
                {
                    defender.activeCard.DamageCard(100);
                }
                else
                {
                    float effectiveness = TypeEffectiveness.GetEffectiveness(attacker.cardType, defender.activeCard.cardType);
                    float damage = attacker.attackPower * effectiveness;
                    Debug.Log("Effectiveness: " + effectiveness);
                    defender.activeCard.DamageCard(Mathf.RoundToInt(damage));
                    BattleController.instance.SetupActiveCards();
                }
            }
            else
            {
                BattleController.instance.DamagePlayer(attacker.attackPower);
                attacker.directHit = false;
            }
        }
        else
        {
            BattleController.instance.DamagePlayer(attacker.attackPower);
            attacker.directHit = false;
        }

        attacker.anim.SetTrigger("Attack");
    }

    public void CheckAssignedCards()
    {
        CheckCards(enemyCardPoints);
        CheckCards(playerCardPoints);
    }

    void CheckCards(CardPlacePoint[] points)
    {
        foreach (CardPlacePoint point in points)
        {
            if (point.activeCard != null && point.activeCard.currentHealth <= 0)
            {
                point.activeCard = null;
            }
        }
    }
}
