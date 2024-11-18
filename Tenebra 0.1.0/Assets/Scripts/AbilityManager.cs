using System.Collections;
using System.Linq;
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
                    case CardAbilitySO.AbilityType.PrimalPact:
                        PrimalPact(card);
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
        CheckPrimalPactInteractions(card);
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

    public void PrimalPact(Card card)
    {
        card.primalPact = true;
    }

    public void MetamorphoseCard()
    {
        CardPlacePoint[] cardPoints = null;

        // Þu anki tura göre metamorphose iþlemini uygun kartlar için yap
        if (BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
        {
            cardPoints = CardPointsController.instance.playerCardPoints;
        }
        else if (BattleController.instance.currentPhase == BattleController.TurnOrder.enemyActive)
        {
            cardPoints = CardPointsController.instance.enemyCardPoints;
        }

        if (cardPoints == null) return;

        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.CanMetamorphose())
            {
                TransformCard(point.activeCard);
            }
        }
    }

    private void CheckPrimalPactInteractions(Card card)
    {
        if (!card.primalPact || card.isTransformed) return;

        // Ýlgili kart türü (oyuncu veya düþman) için sahadaki tüm kartlarý kontrol et
        var cardPoints = card.isPlayer
            ? CardPointsController.instance.playerCardPoints
            : CardPointsController.instance.enemyCardPoints;

        int primalPactCount = 0;

        // Sahadaki `PrimalPact` kartlarýnýn sayýsýný kontrol et
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.primalPact)
            {
                primalPactCount++;
            }
        }

        // Eðer sahada birden fazla `PrimalPact` kartý varsa dönüþümü baþlat
        if (primalPactCount > 1)
        {
            foreach (var point in cardPoints)
            {
                if (point.activeCard != null && point.activeCard.primalPact && !point.activeCard.isTransformed)
                {
                    TransformCard(point.activeCard);
                }
            }
        }
    }

    private void TransformCard(Card card)
    {
        card.characterArt.sprite = card.cardSO.changedCharacterSprite;
        card.currentHealth = card.cardSO.changedHealth;
        card.attackPower = card.cardSO.changedAttackPower;
        card.UpdateCardDisplay();
        card.isTransformed = true; // Kart dönüþmüþ olarak iþaretleniyor
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
