using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Card Abilities/Heal")]
public class HealAbility : CardAbility
{
    public int healAmount;

    public override void ActivateAbility(Card card)
    {
        // Kartý iyileþtirme kodu
        card.currentHealth += healAmount;
        if (card.currentHealth > card.cardSO.currentHealth)
        {
            card.currentHealth = card.cardSO.currentHealth;
        }
        card.UpdateCardDisplay();
        Debug.Log(card.cardSO.cardName + " heals for " + healAmount);
    }
}