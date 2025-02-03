using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class ConsumableItemGenerator
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
            if (baseConsumable == null)
            {
                Debug.LogError("Failed to retrieve a base consumable item.");
                return null;
            }

            // Create a new consumable instance using the new constructor (or copy constructor)
            ConsumableItem generatedConsumable = InitializeItem(baseConsumable);

            // Only apply affixes if the item can have them.
            if (generatedConsumable.canHaveAffixes)
            {
                // Roll for a suffix (30% chance)
                ConsumableSuffixData suffix = RollSuffix();
                if (suffix != null)
                {
                    generatedConsumable.suffix = suffix;
                }
                // Roll for a prefix (50% chance)
                ConsumablePrefixData prefix = RollPrefix();
                if (prefix != null)
                {
                    generatedConsumable.prefix = prefix;
                }
                // Now update the consumable's stats and name based on the applied affixes.
                generatedConsumable.ApplyAffixes();
            }
            return generatedConsumable;
        }

        /// <summary>
        /// Creates a new ConsumableItem by cloning the base consumable using the new constructor.
        /// </summary>
        private static ConsumableItem InitializeItem(ConsumableItem baseConsumable)
        {
            ConsumableItem newItem = new ConsumableItem(
                baseConsumable.itemID,
                baseConsumable.itemName,
                baseConsumable.description,
                baseConsumable.baseSprite,
                baseConsumable.hasDuration,
                baseConsumable.duration,
                new Dictionary<Stat, float>(baseConsumable.consumableStats),
                new List<StatusEffectType>(baseConsumable.addedEffects),
                new List<StatusEffectType>(baseConsumable.removedEffects),
                baseConsumable.canHaveAffixes,
                baseConsumable.amountPerInterval
            );
            // Copy pricing fields
            newItem.price = baseConsumable.price;
            newItem.suffixPriceIncrease = baseConsumable.suffixPriceIncrease;
            newItem.prefixPriceMultiplier = baseConsumable.prefixPriceMultiplier;
            return newItem;
        }

        /// <summary>
        /// Retrieves a base consumable item from the database at random.
        /// </summary>
        private static ConsumableItem GetBaseConsumable()
        {
            List<ConsumableItem> availableItems = ConsumablesDatabase.consumables;
            if (availableItems == null || availableItems.Count == 0)
            {
                Debug.LogError("No consumable items found.");
                return null;
            }
            return availableItems[Random.Range(0, availableItems.Count)];
        }

        /// <summary>
        /// Rolls for a suffix with a 30% chance.
        /// </summary>
        private static ConsumableSuffixData RollSuffix()
        {
            List<ConsumableSuffixData> suffixes = ConsumableAffixesDatabase.suffixes;
            if (suffixes == null || suffixes.Count == 0)
                return null;
            if (Random.value < 0.3f)
                return suffixes[Random.Range(0, suffixes.Count)];
            return null;
        }

        /// <summary>
        /// Rolls for a prefix with a 50% chance.
        /// </summary>
        private static ConsumablePrefixData RollPrefix()
        {
            List<ConsumablePrefixData> prefixes = ConsumableAffixesDatabase.prefixes;
            if (prefixes == null || prefixes.Count == 0)
                return null;
            if (Random.value < 0.5f)
                return prefixes[Random.Range(0, prefixes.Count)];
            return null;
        }
    }
}
