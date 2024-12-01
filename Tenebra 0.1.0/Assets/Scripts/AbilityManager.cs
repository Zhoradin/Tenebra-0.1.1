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
        public int turnCounter;   // Decay hasar�n�n art���n� takip
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
                    card.directHit = true;
                    break;
                case CardAbilitySO.AbilityType.DoubleTap:
                    card.doubleTap = true;
                    break;
                case CardAbilitySO.AbilityType.QuickAttack:
                    StartCoroutine(QuickAttackCoroutine(card));
                    break;
                case CardAbilitySO.AbilityType.Mend:
                    card.mend = true;
                    break;
                case CardAbilitySO.AbilityType.Leech:
                    card.leech = true;
                    break;
                case CardAbilitySO.AbilityType.Metamorphosis:
                    Metamorphosis(card);
                    break;
                case CardAbilitySO.AbilityType.PrimalPact:
                    card.primalPact = true;
                    break;
                case CardAbilitySO.AbilityType.Scattershot:
                    Scattershot(card);
                    break;
                case CardAbilitySO.AbilityType.Growth:
                    card.growth = true;
                    break;
                case CardAbilitySO.AbilityType.Decay:
                    card.decay = true;
                    break;
                case CardAbilitySO.AbilityType.Guardian:
                    card.guardian = true;
                    break;
                case CardAbilitySO.AbilityType.Reckoning:
                    Reckoning(card);
                    break;
                case CardAbilitySO.AbilityType.Benevolence:
                    card.benevolence = true;
                    break;
                case CardAbilitySO.AbilityType.Snowball:
                    Snowball(card);
                    break;
                case CardAbilitySO.AbilityType.Duality:
                    card.duality = true;
                    break;
                case CardAbilitySO.AbilityType.Doppelganger:
                    break;
                case CardAbilitySO.AbilityType.Stun:
                    Stun(card);
                    break;
                case CardAbilitySO.AbilityType.HealBlock:
                    card.healBlock = true;
                    break;
                case CardAbilitySO.AbilityType.Mirror:
                    card.mirror = true;
                    break;
                case CardAbilitySO.AbilityType.Harvester:
                    card.harvester = true;
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

    public void Metamorphosis(Card card)
    {
        card.metamorphosis = true;
        card.metamorphosisTurnCount = BattleController.instance.turnCount; // Save the turn count of the card
    }

    public void MetamorphoseCard()
    {
        CardPlacePoint[] cardPoints = null;

        // Metamorphose the cards that are available
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
            if (point.activeCard != null && CanMetamorphose(point.activeCard))
            {
                TransformCard(point.activeCard);
            }
        }
    }

    public bool CanMetamorphose(Card card)
    {
        return card.metamorphosis && !card.isTransformed && (BattleController.instance.turnCount - card.metamorphosisTurnCount) >= 2; // Wait 2 turns to metamorphose
    }

    private void CheckPrimalPactInteractions(Card card)
    {
        if (!card.primalPact || card.isTransformed) return;

        // Check the all cards on the field
        var cardPoints = card.isPlayer
            ? CardPointsController.instance.playerCardPoints
            : CardPointsController.instance.enemyCardPoints;

        int primalPactCount = 0;

        // Check the amount of Primal Pact cards on the field
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.primalPact)
            {
                primalPactCount++;
            }
        }

        // If there are more than 1 Primal Pact cards on the field, transform the card
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
        card.isTransformed = true;
    }

    public void Scattershot(Card card)
    {
        card.scattershot = true;
        card.multipleHit = true;
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
                        point.activeCard.UpdateCardDisplay();
                    }
                }
                else
                {
                    if (CanHeal(CardPointsController.instance.playerCardPoints))
                    {
                        point.activeCard.currentHealth += point.activeCard.cardSO.abilities[0].value;
                        point.activeCard.UpdateCardDisplay();
                    }
                } 
            }
        }
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

            // Decayed yapan kart �ld�yse hasar� durdur
            if (decayedCard.sourceCard == null || !decayedCard.sourceCard.isActive)
            {
                decayedCards.RemoveAt(i);
                continue;
            }

            // Aktif faz kontrol�
            if (decayedCard.card != null && decayedCard.card.isActive)
            {
                bool isPlayerTurn = BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive;

                // Faz uygun de�ilse i�lemi atla
                if ((decayedCard.card.isPlayer && !isPlayerTurn) || (!decayedCard.card.isPlayer && isPlayerTurn))
                {
                    continue;
                }

                // �lk turda hasar vermeme kontrol�
                if (decayedCard.turnCounter > 0)
                {
                    int decayDamage = decayedCard.turnCounter;
                    decayedCard.card.DamageCard(decayDamage);

                    // Hasar sonras� kart �ld�yse listeden kald�r
                    if (!decayedCard.card.isActive)
                    {
                        decayedCards.RemoveAt(i);
                        continue;
                    }
                }

                // Her tur sonunda turnCounter art�r�l�r
                decayedCard.turnCounter++;
            }
        }
    }

    public void Reckoning(Card card)
    {
        heldCardCount = HandController.instance.heldCards.Count;
        card.attackPower += heldCardCount;
        HandController.instance.EmptyHand();
        card.UpdateCardDisplay();
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
                                targetPoint.activeCard.currentHealth += 1; // Can de�erini art�r
                                targetPoint.activeCard.UpdateCardDisplay();
                            }
                        }
                        else
                        {
                            if (CanHeal(CardPointsController.instance.playerCardPoints))
                            {
                                targetPoint.activeCard.currentHealth += 1; // Can de�erini art�r
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

    public void ApplyDuality(CardPlacePoint[] cardPoints)
    {
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
        card.gratis = true;
        int randomNumber;

        // E�er elde sadece Gratis kart� varsa random kart se�meden i�lemi durdur
        if (HandController.instance.heldCards.Count == 1)
        {
            Debug.LogWarning("Only Gratis card in hand. No other card to apply Gratis effect.");
            return;
        }

        // Random bir kart se� ve Gratis kart�n� d��la
        do
        {
            randomNumber = Random.Range(0, HandController.instance.heldCards.Count);
        }
        while (HandController.instance.heldCards[randomNumber].gratis); // Gratis olmayan bir kart bulana kadar d�ng�

        // Se�ilen kart�n essence cost'unu 0 yap
        Card selectedCard = HandController.instance.heldCards[randomNumber];
        selectedCard.essenceCost = 0;
        selectedCard.costText.color = Color.green;
        selectedCard.UpdateCardDisplay();
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
        yield return new WaitForSeconds(0.5f);
        if (card.isPlayer)
        {
            CardPointsController.instance.PlayerSingleCardAttack(card);
        }
        else
        {
            CardPointsController.instance.EnemySingleCardAttack(card);
        }
    }

    public IEnumerator RemoveDoppelgangerAndPlaceField(Card doppelgangerCard, CardPlacePoint selectedPoint, Card playedCard)
    {
        doppelgangerCard.theCol.enabled = false;
        playedCard.theCol.enabled = false;
        // Yeni Field kart�n� yerle�tir
        playedCard.FieldUsage(selectedPoint);

        // Doppelganger yok olma animasyonu
        doppelgangerCard.StartCoroutine(doppelgangerCard.WaitJumpAfterDeadCo());

        // Doppelganger'�n tamamen yok olmas�n� bekle
        yield return new WaitForSeconds(0.7f); // Animasyon s�relerine g�re ayarlay�n

        // Sahadaki referans� s�f�rla
        selectedPoint.activeCard = null;

        playedCard.theCol.enabled = true;
    }
}
