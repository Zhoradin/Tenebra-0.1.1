using System.Collections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void ActivateAbility(Card card)
    {
        if (card.abilityDescription.activeSelf)
        {
            card.abilityDescription.SetActive(false);
        }
        if (card.abilityDescriptionToo.activeSelf)
        {
            card.abilityDescriptionToo.SetActive(false);
        }

        foreach (CardAbilitySO ability in card.cardSO.abilities)
        {
            if(card.cardKind == CardKind.Field)
            {
                switch (ability.abilityType)
                {
                    case CardAbilitySO.AbilityType.Heal:
                        Heal(card, ability.value);
                        break;
                    case CardAbilitySO.AbilityType.DirectHit:
                        DirectHit(card);
                        break;
                    case CardAbilitySO.AbilityType.DoubleTap:
                        DoubleTap(card);
                        break;
                    case CardAbilitySO.AbilityType.QuickAttack:
                        StartCoroutine(QuickAttackCoroutine(card));
                        break;
                    case CardAbilitySO.AbilityType.GlassCannon:
                        GlassCannon(card);
                        break;
                    case CardAbilitySO.AbilityType.Mend:
                        Mend(card);
                        break;
                    case CardAbilitySO.AbilityType.Leech:
                        Leech(card);
                        break;
                }
            }
            else if (card.cardKind == CardKind.Efect)
            {
                switch (ability.abilityType)
                {
                    case CardAbilitySO.AbilityType.Revelation:
                        Revelation(card);
                        break;
                }
            }
        }
    }

    public void Heal(Card card, int healAmount)
    {
        card.currentHealth += healAmount;
        card.UpdateCardDisplay();
    }

    private void DirectHit(Card card)
    {
        card.directHit = true;
    }

    private void DoubleTap(Card card)
    {
        card.doubleTap = true;
    }

    private void GlassCannon(Card card)
    {
        card.glassCannon = true;
    }

    private void Mend(Card card)
    {
        card.mend = true;
    }

    private void Leech(Card card)
    {
        card.leech = true;
    }

    public void Revelation(Card card)
    {
        card.leech = true;
    }

    public IEnumerator QuickAttackCoroutine(Card card)
    {
        card.quickAttack = true;
        yield return new WaitForSeconds(0.5f);
        if (card.isPlayer)
        {
            CardPointsController.instance.PlayerSingleCardAttack(card);
        }
        else
        {
            CardPointsController.instance.EnemySingleCardAttack(card);
        }

        card.quickAttack = false;
    }
}
