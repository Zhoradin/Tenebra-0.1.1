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
                case CardAbilitySO.AbilityType.Dreamweaving:
                    card.dreamweaving = true;
                    break;
                case CardAbilitySO.AbilityType.Bulwark:
                    Bulwark(card);
                    break;
                case CardAbilitySO.AbilityType.SoulSucking:
                    SoulSucking(card);
                    break;
                case CardAbilitySO.AbilityType.Switch:
                    card.switchAbility = true;
                    break;
                case CardAbilitySO.AbilityType.Armor:
                    ActivateArmor(card);
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
                case CardAbilitySO.AbilityType.Armor:
                    ActivateArmor(playedCard);
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

    public void Reckoning(Card card)
    {
        heldCardCount = HandController.instance.playerHeldCards.Count;
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

        // Eðer elde sadece Gratis kartý varsa random kart seçmeden iþlemi durdur
        if (HandController.instance.playerHeldCards.Count == 1)
        {
            Debug.LogWarning("Only Gratis card in hand. No other card to apply Gratis effect.");
            return;
        }

        // Random bir kart seç ve Gratis kartýný dýþla
        do
        {
            randomNumber = Random.Range(0, HandController.instance.playerHeldCards.Count);
        }
        while (HandController.instance.playerHeldCards[randomNumber].gratis); // Gratis olmayan bir kart bulana kadar döngü

        // Seçilen kartýn essence cost'unu 0 yap
        Card selectedCard = HandController.instance.playerHeldCards[randomNumber];
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
        // Yeni Field kartýný yerleþtir
        playedCard.FieldUsage(selectedPoint);

        // Doppelganger yok olma animasyonu
        doppelgangerCard.StartCoroutine(doppelgangerCard.WaitJumpAfterDeadCo());

        // Doppelganger'ýn tamamen yok olmasýný bekle
        yield return new WaitForSeconds(0.7f); // Animasyon sürelerine göre ayarlayýn

        // Sahadaki referansý sýfýrla
        selectedPoint.activeCard = null;

        playedCard.theCol.enabled = true;
    }

    public IEnumerator ShowEvadedText(Card card)
    {
        card.evadedText.gameObject.SetActive(true);
        Vector3 startPosition = card.evadedText.transform.position;

        float duration = 1.5f; // Animasyon süresi
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Text yukarý hareket ediyor
            card.evadedText.transform.position = startPosition + Vector3.up * (elapsed * 0.5f); // .5 birim/sn yukarý çýkma hýzý

            // Text'in opaklýðýný azaltma
            Color textColor = card.evadedText.color;
            textColor.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            card.evadedText.color = textColor;

            yield return null;
        }

        card.evadedText.gameObject.SetActive(false);
        card.evadedText.transform.position = startPosition; // Pozisyonu sýfýrla
        card.evadedText.color = new Color(card.evadedText.color.r, card.evadedText.color.g, card.evadedText.color.b, 1f); // Opaklýðý sýfýrla
    }

    public void Bulwark(Card card)
    {
        card.bulwark = true;
        card.bulwarkHealth = card.cardSO.abilities[0].value;
        card.bulwarkText.text = card.bulwarkHealth.ToString();
        card.bulwarkText.gameObject.SetActive(true);
        card.bulwarkImage.gameObject.SetActive(true);
    }

    public IEnumerator DestroyBulwarkCo(Card card)
    {
        float duration = 1f; // Saydamlýk azaltma süresi
        float elapsed = 0f;

        // Baþlangýç renklerini al
        Color textColor = card.bulwarkText.color;
        Color imageColor = card.bulwarkImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            // Saydamlýðý azalt
            textColor.a = alpha;
            imageColor.a = alpha;

            card.bulwarkText.color = textColor;
            card.bulwarkImage.color = imageColor;

            yield return null; // Bir sonraki frame'i bekle
        }

        // Son olarak SetActive(false) yap
        card.bulwark = false;
        card.bulwarkText.gameObject.SetActive(false);
        card.bulwarkImage.gameObject.SetActive(false);
    }

    public void SoulSucking(Card card)
    {
        if (card.assignedPlace.oppositeCardPlacePoint.activeCard != null)
        {
            if (card.isPlayer)
            {
                if (card.assignedPlace.oppositeCardPlacePoint.activeCard.currentHealth >= 2)
                {
                    card.currentHealth += 2;
                    card.UpdateCardDisplay();
                    card.assignedPlace.oppositeCardPlacePoint.activeCard.DamageCard(2);
                }
                else
                {
                    card.currentHealth++;
                    card.UpdateCardDisplay();
                    card.assignedPlace.oppositeCardPlacePoint.activeCard.DamageCard(1);
                }
            }
            else
            {
                if (card.assignedPlace.oppositeCardPlacePoint.activeCard.currentHealth >= 2)
                {
                    card.currentHealth += 2;
                    card.UpdateCardDisplay();
                    card.assignedPlace.oppositeCardPlacePoint.activeCard.DamageCard(2);
                }
                else
                {
                    card.currentHealth++;
                    card.UpdateCardDisplay();
                    card.assignedPlace.oppositeCardPlacePoint.activeCard.DamageCard(1);
                }
            }
        }
    }

    public void CheckSwitch(CardPlacePoint[] cardPoints)
    {
        foreach (var point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard.switchAbility)
            {
                point.activeCard.attackPower = point.activeCard.cardSO.changedAttackPower;
                point.activeCard.UpdateCardDisplay();
            }
            else if (point.activeCard != null && point.activeCard.switchedAbility)
            {
                point.activeCard.attackPower = point.activeCard.cardSO.attackPower;
                point.activeCard.UpdateCardDisplay();
            }
        }
    }

    public void ActivateArmor(Card card)
    {
        if (UIController.instance.armorAmount == 0)
        {
            Debug.Log("deneme");
            UIController.instance.armorText.color = new Color(UIController.instance.armorText.color.r, UIController.instance.armorText.color.g, UIController.instance.armorText.color.b, 0);
            UIController.instance.armorImage.color = new Color(UIController.instance.armorImage.color.r, UIController.instance.armorImage.color.g, UIController.instance.armorImage.color.b, 0);

            UIController.instance.armorText.gameObject.SetActive(true);
            UIController.instance.armorImage.gameObject.SetActive(true);

            StartCoroutine(ShowArmorCo()); // Saydamlýk artýrma coroutine'ini çaðýr
        }

        UIController.instance.armorAmount += card.cardSO.abilities[0].value;
        UIController.instance.armorText.text = UIController.instance.armorAmount.ToString();
    }

    private IEnumerator ShowArmorCo()
    {
        float duration = 1f; // Saydamlýk artýrma süresi
        float elapsed = 0f;

        // Baþlangýç renklerini al
        Color textColor = UIController.instance.armorText.color;
        Color imageColor = UIController.instance.armorImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);

            // Saydamlýðý artýr
            textColor.a = alpha;
            imageColor.a = alpha;

            UIController.instance.armorText.color = textColor;
            UIController.instance.armorImage.color = imageColor;

            yield return null; // Bir sonraki frame'i bekle
        }

        // Opaklýk 1'e ulaþtýktan sonra renklerin alpha deðerini tam olarak 1 yap
        textColor.a = 1f;
        imageColor.a = 1f;

        UIController.instance.armorText.color = textColor;
        UIController.instance.armorImage.color = imageColor;
    }

    public IEnumerator DestroyArmorCo()
    {
        UIController.instance.armorAmount = 0;
        float duration = 1f; // Saydamlýk azaltma süresi
        float elapsed = 0f;

        // Baþlangýç renklerini al
        Color textColor = UIController.instance.armorText.color;
        Color imageColor = UIController.instance.armorImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            // Saydamlýðý azalt
            textColor.a = alpha;
            imageColor.a = alpha;

            UIController.instance.armorText.color = textColor;
            UIController.instance.armorImage.color = imageColor;

            yield return null; // Bir sonraki frame'i bekle
        }
        UIController.instance.armorImage.gameObject.SetActive(false);
        UIController.instance.armorText.gameObject.SetActive(false);
    }
}
