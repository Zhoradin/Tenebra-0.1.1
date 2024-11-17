using System.Collections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public int turnCount = 0;
    public int metamorphoseTurnCount;

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
                    case CardAbilitySO.AbilityType.Metamorphosis:
                        Metamorphosis(card);
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
        card.revelation = true;
    }

    public void Metamorphosis(Card card)
    {
        card.metamorphosis = true;
        card.metamorphosisTurnCount = BattleController.instance.turnCount; // Kartýn dönüþüm zamanýný kaydet
    }

    public void MetamorphoseCard()
    {
        foreach (var point in CardPointsController.instance.playerCardPoints)
        {
            if (point.activeCard != null)
            {
                var card = point.activeCard;
                if (card.metamorphosis && BattleController.instance.turnCount >= card.metamorphosisTurnCount + 2) // Her kartýn kendi dönüþüm zamanýný kontrol et
                {
                    card.characterArt.sprite = card.cardSO.changedCharacterSprite; // Görseli deðiþtir
                    card.currentHealth = card.cardSO.changedHealth;
                    card.attackPower = card.cardSO.changedAttackPower;
                    card.UpdateCardDisplay();
                    card.metamorphosis = false; // Ýþlem tamamlandýðý için kapat
                }
            }
        }
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
