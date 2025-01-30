using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class ConsumableItemGenerator
    {
        /// <summary>
        /// Generates a random consumable item based on tier and available affixes.
        /// </summary>
        /// <param name="tier">Tier of the consumable (1, 2, or 3).</param>
        /// <returns>A fully configured ConsumableItem object.</returns>
        public static ConsumableItem GenerateRandomConsumable()
        {
            // Get a base consumable from the database
            ConsumableItem baseConsumable = GetBaseConsumable();
            ConsumableItem generatedConsumable = new ConsumableItem();
            if (baseConsumable == null)
            {
                Debug.LogError("Failed to retrieve a base consumable item.");
                return null;
            }

            InitializeItem(generatedConsumable, baseConsumable); // Clone the base consumable
            // Clone the base consumable

            if (!generatedConsumable.canHaveAffixes)
                return generatedConsumable;

            // Apply Suffix
            ConsumableSuffixData suffix = RollSuffix();
            if (suffix != null)
            {
                generatedConsumable.ApplySuffix(suffix);
                generatedConsumable.name = generatedConsumable.name + " " + suffix.suffixName;
            }

            // Apply Prefix
            List<ConsumablePrefixData> prefixes = ConsumableAffixesDatabase.prefixes;
            ConsumablePrefixData prefix = prefixes[Random.Range(0, prefixes.Count)];
            generatedConsumable.ApplyPrefix(prefix);
            generatedConsumable.name = prefix.prefixName + " " + generatedConsumable.name;

            return generatedConsumable;
        }

        // Apply affix data to the consumable item
        private static void InitializeItem(
            ConsumableItem generatedConsumable,
            ConsumableItem baseConsumable
        )
        {
            generatedConsumable.Initialize(
                baseConsumable.name,
                baseConsumable.description,
                baseConsumable.icon,
                baseConsumable.duration,
                baseConsumable.attackBoost,
                baseConsumable.defenseBoost,
                baseConsumable.speedBoost,
                baseConsumable.healthBoost,
                baseConsumable.magicBoost,
                baseConsumable.staminaBoost,
                baseConsumable.dexterityBoost,
                baseConsumable.intelligenceBoost,
                baseConsumable.critChanceBoost,
                baseConsumable.addedEffects,
                baseConsumable.removedEffects,
                baseConsumable.canHaveAffixes,
                baseConsumable.amountPerInterval,
                baseConsumable.price
            );
        }

        /// <summary>
        /// Retrieves a base consumable item based on the tier.
        /// </summary>
        private static ConsumableItem GetBaseConsumable()
        {
            List<ConsumableItem> availableItems = ConsumablesDatabase.consumables;
            if (availableItems == null || availableItems.Count == 0)
            {
                Debug.LogError($"No consumable items found.");
                return null;
            }
            return availableItems[Random.Range(0, availableItems.Count)];
        }

        /// <summary>
        /// Rolls for a suffix based on the tier.
        /// </summary>
        private static ConsumableSuffixData RollSuffix()
        {
            List<ConsumableSuffixData> suffixes = ConsumableAffixesDatabase.suffixes;
            if (suffixes == null || suffixes.Count == 0)
                return null;

            if (Random.value < 0.3f) // 30% chance to apply a suffix
                return suffixes[Random.Range(0, suffixes.Count)];
            return null;
        }
    }
}
