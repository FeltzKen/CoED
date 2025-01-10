using UnityEngine;

namespace CoED
{
    public class ConsumableItemWrapper : MonoBehaviour
    {
        [SerializeField]
        public Consumable consumableData;

        public string ItemName => consumableData.ItemName;
        public Sprite Icon => consumableData.Icon;

        public void Initialize(Consumable consumable)
        {
            consumableData = consumable;

            // Copy boosts to wrapper instance
            consumableData.attackBoost = consumable.AttackBoost;
            consumableData.defenseBoost = consumable.DefenseBoost;
            consumableData.speedBoost = consumable.speedBoost;
            consumableData.healthBoost = consumable.HealthBoost;
            consumableData.magicBoost = consumable.magicBoost;
            consumableData.staminaBoost = consumable.staminaBoost;
        }

        public string GetDescription()
        {
            return consumableData.Description;
        }

        public void Consume(GameObject entity)
        {
            if (consumableData != null)
            {
                consumableData.Consume(entity);
            }
            else
            {
                Debug.LogWarning("ConsumableItemWrapper: Consumable data is not initialized.");
            }
        }

        public void ApplyStatModifiers(float scaledFactor)
        {
            // Apply scaled boosts
            consumableData.attackBoost += scaledFactor;
            consumableData.defenseBoost += scaledFactor;
            consumableData.speedBoost += scaledFactor;
            consumableData.healthBoost += scaledFactor;
            consumableData.magicBoost += scaledFactor;
            consumableData.staminaBoost += scaledFactor;
        }
        // Additional functionality if needed (e.g., modifying boosts at runtime)
    }
}
