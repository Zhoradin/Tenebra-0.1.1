using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public int turnCount = 0;
    public int metamorphoseTurnCount;
    private int heldCardCount;

    private List<DecayedCard> decayedCards = new List<DecayedCard>();

    private class DecayedCard
    {
        public Card card;         // Decayed olan kart
        public Card sourceCard;   // Decayed yapan kart
        public int turnCounter;   // Decay hasarýnýn artýþýný takip
    }

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
                case CardAbilitySO.AbilityType.Scattershot:
                    Scattershot(card);
                    break;
                case CardAbilitySO.AbilityType.Growth:
                    Growth(card);
                    break;
                case CardAbilitySO.AbilityType.Decay:
                    Decay(card);
                    break;
                case CardAbilitySO.AbilityType.Guardian:
                    Guardian(card);
                    break;
                case CardAbilitySO.AbilityType.Reckoning:
                    Reckoning(card);
                    break;
            }
        }
        CheckPrimalPactInteractions(card);
    }

    public void ActivateEffectAbility(Card playedCard, Card effectedCard)
    {
        foreach (CardAbilitySO ability in playedCard.cardSO.abilities)
        {
            switch (ability.abilityType)
            {
                case CardAbilitySO.AbilityType.HealingTouch:
                    HealingTouch(playedCard, effectedCard);
                    break;
                case CardAbilitySO.AbilityType.Revelation:
                    break;
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

    public void PrimalPact(Card card)
    {
        card.primalPact = true;
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

    public void Scattershot(Card card)
    {
        card.scattershot = true;
        card.multipleHit = true;
    }

    public void Growth(Card card)
    {
        card.growth = true;
    }

    public void ApplyGrowthAbility(CardPlacePoint[] cardPoints)
    {
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.growth)
            {
                point.activeCard.currentHealth += point.activeCard.cardSO.abilities[0].value;
                point.activeCard.UpdateCardDisplay(); // Kart görselini güncelle
            }
        }
    }

    public void Decay(Card card)
    {
        card.decay = true;
    }

    public void DecayCard(Card attackingCard, Card defendingCard)
    {
        if(defendingCard != null)
        {
            defendingCard.decayed = true;
        }

        DecayedCard decayedCard = new DecayedCard
        {
            card = defendingCard,
            sourceCard = attackingCard,
            turnCounter = 0
        };

        decayedCards.Add(decayedCard);
    }

    public void ProcessDecayDamage()
    {
        for (int i = decayedCards.Count - 1; i >= 0; i--)
        {
            DecayedCard decayedCard = decayedCards[i];

            // Decayed yapan kart öldüyse hasarý durdur
            if (decayedCard.sourceCard == null || !decayedCard.sourceCard.isActive)
            {
                decayedCards.RemoveAt(i);
                continue;
            }

            // Aktif faz kontrolü
            if (decayedCard.card != null && decayedCard.card.isActive)
            {
                bool isPlayerTurn = BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive;

                // Faz uygun deðilse iþlemi atla
                if ((decayedCard.card.isPlayer && !isPlayerTurn) || (!decayedCard.card.isPlayer && isPlayerTurn))
                {
                    continue;
                }

                // Ýlk turda hasar vermeme kontrolü
                if (decayedCard.turnCounter > 0)
                {
                    int decayDamage = decayedCard.turnCounter;
                    decayedCard.card.DamageCard(decayDamage);

                    // Hasar sonrasý kart öldüyse listeden kaldýr
                    if (!decayedCard.card.isActive)
                    {
                        decayedCards.RemoveAt(i);
                        continue;
                    }
                }

                // Her tur sonunda turnCounter artýrýlýr
                decayedCard.turnCounter++;
            }
        }
    }

    public void Guardian(Card card)
    {
        card.guardian = true;
    }

    public void Reckoning(Card card)
    {
        card.reckoning = true;
        heldCardCount = HandController.instance.heldCards.Count;
        card.attackPower += heldCardCount;
        HandController.instance.EmptyHand();
        card.UpdateCardDisplay();
    }

    public void HealingTouch(Card playedCard, Card effectedCard)
    {
        effectedCard.currentHealth += playedCard.cardSO.abilities[0].value;
        effectedCard.UpdateCardDisplay();
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
