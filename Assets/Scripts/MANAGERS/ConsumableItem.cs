using UnityEngine;
using CoED;
[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int magicBoost;
    public int staminaBoost;

    // Method to consume the item and apply its effects
    public void Consume(PlayerStats playerStats)
    {
        playerStats.Heal(healthBoost);
        playerStats.GainMagic(magicBoost);
        playerStats.GainStamina(staminaBoost);
    }
}