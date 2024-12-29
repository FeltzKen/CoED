using CoED;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int magicBoost;
    public int staminaBoost;

    public void Consume(PlayerStats playerStats)
    {
        playerStats.Heal(healthBoost);
        playerStats.RefillMagic(magicBoost);
        playerStats.GainStamina(staminaBoost);
    }
}
