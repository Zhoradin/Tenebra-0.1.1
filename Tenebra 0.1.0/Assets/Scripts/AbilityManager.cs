using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public int turnCount = 0;
    public int metamorphoseTurnCount;
    private int heldCardCount;
    private int playerSnowballAmount, enemySnowballAmount = 0;
    private int tempHealth;

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
                case CardAbilitySO.AbilityType.Benevolence:
                    Benevolence(card);
                    break;
                case CardAbilitySO.AbilityType.Snowball:
                    Snowball(card);
                    break;
                case CardAbilitySO.AbilityType.Duality:
                    Duality(card);
                    break;
                case CardAbilitySO.AbilityType.Doppelganger:
                    break;
                case CardAbilitySO.AbilityType.Stun:
                    Stun(card);
                    break;
                case CardAbilitySO.AbilityType.HealBlock:
                    HealBlock(card);
                    break;
                case CardAbilitySO.AbilityType.Mirror:
                    Mirror(card);
                    break;
                case CardAbilitySO.AbilityType.Harvester:
                    Harvester(card);
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
            }
        }
    }

    public void ActivateImpactAbility(Card playedCard)
    {
        foreach (CardAbilitySO ability in playedCard.cardSO.abilities)
        {
            switch (ability.abilityType)
            {
                case CardAbilitySO.AbilityType.Revelation:
                    Revelation(playedCard);
                    break;
                case CardAbilitySO.AbilityType.Resurrect:
                    Resurrect(playedCard);
                    break;
                case CardAbilitySO.AbilityType.Gratis:
                    Gratis(playedCard);
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

    public void Mend(Card card)
    {
        card.mend = true;
    }

    public void Leech(Card card)
    {
        card.leech = true;
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
                if (point.activeCard.isPlayer)
                {
                    if (CanHeal(CardPointsController.instance.enemyCardPoints))
                    {
                        point.activeCard.currentHealth += point.activeCard.cardSO.abilities[0].value;
                        point.activeCard.UpdateCardDisplay(); // Kart görselini güncelle
                    }
                }
                else
                {
                    if (CanHeal(CardPointsController.instance.playerCardPoints))
                    {
                        point.activeCard.currentHealth += point.activeCard.cardSO.abilities[0].value;
                        point.activeCard.UpdateCardDisplay(); // Kart görselini güncelle
                    }
                } 
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

    public void Benevolence(Card card)
    {
        card.benevolence = true;
    }

    public void ApplyBenevolenceEffect(CardPlacePoint[] cardPoints)
    {
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.benevolence)
            {
                foreach (var targetPoint in cardPoints)
                {
                    if (targetPoint.activeCard != null)
                    {
                        if (targetPoint.activeCard.isPlayer)
                        {
                            if (CanHeal(CardPointsController.instance.enemyCardPoints))
                            {
                                targetPoint.activeCard.currentHealth += 1; // Can deðerini artýr
                                targetPoint.activeCard.UpdateCardDisplay();
                            }
                        }
                        else
                        {
                            if (CanHeal(CardPointsController.instance.playerCardPoints))
                            {
                                targetPoint.activeCard.currentHealth += 1; // Can deðerini artýr
                                targetPoint.activeCard.UpdateCardDisplay();
                            }
                        }   
                    }
                }
            }
        }
    }

    public void Snowball(Card card)
    {
        if (card.isPlayer)
        {
            card.snowball = true;
            card.attackPower += playerSnowballAmount;
            card.UpdateCardDisplay();
            playerSnowballAmount++;
        }
        else
        {
            card.snowball = true;
            card.attackPower += enemySnowballAmount;
            card.UpdateCardDisplay();
            enemySnowballAmount++;
        }  
    }

    public void Duality(Card card)
    {
        card.duality = true;
    }

    public void ApplyDuality(CardPlacePoint[] cardPoints)
    {
        /*foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.duality)
            {
                foreach (var targetPoint in cardPoints)
                {
                    if(targetPoint.activeCard != null)
                    {
                        tempHealth = targetPoint.activeCard.currentHealth;
                        targetPoint.activeCard.currentHealth = targetPoint.activeCard.attackPower;
                        targetPoint.activeCard.attackPower = tempHealth;
                        targetPoint.activeCard.UpdateCardDisplay();
                    }
                }
            }
        }*/

        for (int i = 0; i < cardPoints.Length; i++)
        {
            if(cardPoints[i].activeCard != null && cardPoints[i].activeCard.duality)
            {
                tempHealth = cardPoints[i].activeCard.currentHealth;
                cardPoints[i].activeCard.currentHealth = cardPoints[i].activeCard.attackPower;
                cardPoints[i].activeCard.attackPower = tempHealth;
                cardPoints[i].activeCard.UpdateCardDisplay();
            }
        }
    }

    public void HealingTouch(Card playedCard, Card effectedCard)
    {
        if (playedCard.isPlayer)
        {
            if (CanHeal(CardPointsController.instance.enemyCardPoints))
            {
                effectedCard.currentHealth += playedCard.cardSO.abilities[0].value;
                effectedCard.UpdateCardDisplay();
            }
        }
        else
        {
            if (CanHeal(CardPointsController.instance.playerCardPoints))
            {
                effectedCard.currentHealth += playedCard.cardSO.abilities[0].value;
                effectedCard.UpdateCardDisplay();
            }
        }   
    }

    public void Revelation(Card card)
    {
        card.revelation = true;
        DeckController.instance.DrawMultipleCards(2);
    }

    public void Resurrect(Card card)
    {
        DrawPileController.instance.drawPile.AddRange(GraveyardPileController.instance.graveyardPile);
        DrawPileController.instance.CreateDrawPileCardSlots();
        GraveyardPileController.instance.graveyardPile.Clear();
        GraveyardPileController.instance.CreateGraveyardPileCardSlots();
    }

    public void Gratis(Card card)
    {
        card.gratis = true; // Bu kart artýk Gratis oldu
        int randomNumber;

        // Eðer elde sadece Gratis kartý varsa random kart seçmeden iþlemi durdur
        if (HandController.instance.heldCards.Count == 1)
        {
            Debug.LogWarning("Only Gratis card in hand. No other card to apply Gratis effect.");
            return;
        }

        // Random bir kart seç ve Gratis kartýný dýþla
        do
        {
            randomNumber = Random.Range(0, HandController.instance.heldCards.Count);
        }
        while (HandController.instance.heldCards[randomNumber].gratis); // Gratis olmayan bir kart bulana kadar döngü

        // Seçilen kartýn essence cost'unu 0 yap
        Card selectedCard = HandController.instance.heldCards[randomNumber];
        selectedCard.essenceCost = 0;
        selectedCard.costText.color = Color.green;
        selectedCard.UpdateCardDisplay();

        //Debug.Log($"Gratis applied to: {selectedCard.cardSO.name}");
    }

    public void Stun(Card card)
    {
        card.stun = true;
        ApplyStun(card, card.assignedPlace.oppositeCardPlacePoint.activeCard);
    }

    public void ApplyStun(Card attackerCard, Card defenderCard)
    {
        if(attackerCard.assignedPlace != null)
        {
            if(attackerCard.assignedPlace.oppositeCardPlacePoint.activeCard != null)
            {
                defenderCard.stunned = true;
                defenderCard.stunImage.gameObject.SetActive(true);
            }
        }
    }

    public void CheckStun(CardPlacePoint[] cardPoints)
    {
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.duality)
            {
                foreach (var targetPoint in cardPoints)
                {
                    if (targetPoint.activeCard != null)
                    {
                        targetPoint.activeCard.stunned = false;
                        targetPoint.activeCard.stunImage.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void HealBlock(Card card)
    {
        card.healBlock = true;
    }

    public bool CanHeal(CardPlacePoint[] cardPoints)
    {
        foreach (CardPlacePoint point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.healBlock)
            {
                return false;
            }
        }
        return true;
    }

    public void Mirror(Card card)
    {
        card.mirror = true;
    }

    public void Harvester(Card card)
    {
        card.harvester = true;
    }

    public void ApplyHarvesterAbility(CardPlacePoint[] cardPoints)
    {
        foreach (CardPlacePoint point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.harvester)
            {
                if (point.activeCard.assignedPlace.oppositeCardPlacePoint.activeCard)
                {
                    if (point.activeCard.isPlayer)
                    {
                        if (CanHeal(CardPointsController.instance.enemyCardPoints))
                        {
                            point.activeCard.currentHealth += 1;
                            point.activeCard.UpdateCardDisplay();
                            point.activeCard.assignedPlace.oppositeCardPlacePoint.activeCard.currentHealth -= 1;
                            point.activeCard.assignedPlace.oppositeCardPlacePoint.activeCard.UpdateCardDisplay();
                        }
                    }
                    else
                    {
                        if (CanHeal(CardPointsController.instance.playerCardPoints))
                        {
                            point.activeCard.currentHealth += 1;
                            point.activeCard.UpdateCardDisplay();
                            point.activeCard.assignedPlace.oppositeCardPlacePoint.activeCard.currentHealth -= 1;
                            point.activeCard.assignedPlace.oppositeCardPlacePoint.activeCard.UpdateCardDisplay();
                        }
                    }
                    
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
