using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    /// <summary>
    /// A consumable item whose stat-boosts, affixes, and effects are stored and applied in a manner
    /// similar to Equipment.
    /// </summary>
    [Serializable]
    public class ConsumableItem : IShopItem
    {
        // ------------------------------
        // Basic Fields
        // ------------------------------
        public string itemID;
        public string itemName;
        public string description;
        public Sprite baseSprite;

        // Duration-related fields
        public bool hasDuration = true;
        public float duration;
        public int floorNumber;

        // ------------------------------
        // Stat Modifiers
        // ------------------------------
        // For consistency with Equipment, we store all stat boosts in a dictionary.
        // (Keys correspond to the Stat enum values used for equipment.)
        public Dictionary<Stat, float> consumableStats = new Dictionary<Stat, float>
        {
            { Stat.Attack, 0f },
            { Stat.Defense, 0f },
            { Stat.Speed, 0f },
            { Stat.HP, 0f },
            { Stat.Magic, 0f },
            { Stat.Stamina, 0f },
            { Stat.Dexterity, 0f },
            { Stat.Intelligence, 0f },
            { Stat.CritChance, 0f },
            { Stat.Shield, 0f },
            { Stat.FireRate, 0f },
            { Stat.CritDamage, 0f },
            { Stat.ProjectileRange, 0f },
            { Stat.AttackRange, 0f },
            { Stat.ElementalDamage, 0f },
            { Stat.ChanceToInflict, 0f },
            { Stat.StatusEffectDuration, 0f },
        };

        // ------------------------------
        // Affix Data
        // ------------------------------
        public ConsumablePrefixData prefix;
        public ConsumableSuffixData suffix;

        // ------------------------------
        // Effects
        // ------------------------------
        public List<StatusEffectType> addedEffects = new List<StatusEffectType>();
        public List<StatusEffectType> removedEffects = new List<StatusEffectType>();

        // ------------------------------
        // Pricing & Other Fields
        // ------------------------------
        public bool canHaveAffixes = true;
        public int amountPerInterval = 0;
        public int price;
        public int suffixPriceIncrease;
        public float prefixPriceMultiplier = 1f;

        // ------------------------------
        // Constructors
        // ------------------------------

        /// <summary>
        /// Default constructor (required for serialization).
        /// </summary>
        public ConsumableItem() { }

        /// <summary>
        /// Constructor that initializes a consumable item. Stat overrides are applied to the consumableStats dictionary.
        /// </summary>
        public ConsumableItem(
            string itemID,
            string itemName,
            string description,
            Sprite baseSprite,
            bool hasDuration,
            float duration,
            Dictionary<Stat, float> statOverrides = null,
            List<StatusEffectType> addedEffects = null,
            List<StatusEffectType> removedEffects = null,
            bool canHaveAffixes = true,
            int amountPerInterval = 0
        )
        {
            this.itemID = itemID;
            this.itemName = itemName;
            this.description = description;
            this.baseSprite = baseSprite;
            this.hasDuration = hasDuration;
            this.duration = duration;

            if (statOverrides != null)
            {
                foreach (var pair in statOverrides)
                {
                    if (consumableStats.ContainsKey(pair.Key))
                        consumableStats[pair.Key] = pair.Value;
                }
            }

            this.addedEffects = addedEffects ?? new List<StatusEffectType>();
            this.removedEffects = removedEffects ?? new List<StatusEffectType>();
            this.canHaveAffixes = canHaveAffixes;
            this.amountPerInterval = amountPerInterval;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public ConsumableItem(ConsumableItem other)
        {
            this.itemID = other.itemID;
            this.itemName = other.itemName;
            this.description = other.description;
            this.baseSprite = other.baseSprite;
            this.hasDuration = other.hasDuration;
            this.duration = other.duration;
            this.consumableStats = new Dictionary<Stat, float>(other.consumableStats);
            this.prefix = other.prefix;
            this.suffix = other.suffix;
            this.addedEffects = new List<StatusEffectType>(other.addedEffects);
            this.removedEffects = new List<StatusEffectType>(other.removedEffects);
            this.canHaveAffixes = other.canHaveAffixes;
            this.amountPerInterval = other.amountPerInterval;
            this.suffixPriceIncrease = other.suffixPriceIncrease;
            this.prefixPriceMultiplier = other.prefixPriceMultiplier;
        }

        // ------------------------------
        // IShopItem Interface Methods
        // ------------------------------
        public string GetName() => itemName;

        public Sprite GetSprite() => baseSprite;

        /// <summary>
        /// Returns a description that includes affix names (if applied) and the base description.
        /// </summary>
        public string GetDescription()
        {
            string desc = itemName;
            if (prefix != null)
                desc = prefix.prefixName + " " + desc;
            if (suffix != null)
                desc += " " + suffix.suffixName;
            desc += "\n" + description;
            return desc;
        }

        // ------------------------------
        // Affix Logic
        // ------------------------------

        /// <summary>
        /// Applies affix stat modifiers and effects from both prefix and suffix.
        /// Updates the itemName accordingly.
        /// </summary>
        public void ApplyAffixes()
        {
            // 1) Apply prefix modifiers.
            if (prefix != null)
            {
                ApplyStats(prefix.statModifiers);

                if (prefix.addedEffects != null)
                {
                    foreach (var effect in prefix.addedEffects)
                    {
                        if (!addedEffects.Contains(effect))
                            addedEffects.Add(effect);
                    }
                }
            }

            // 2) Apply suffix modifiers.
            if (suffix != null)
            {
                ApplyStats(suffix.statModifiers);

                if (suffix.addedEffects != null)
                {
                    foreach (var effect in suffix.addedEffects)
                    {
                        if (!addedEffects.Contains(effect))
                            addedEffects.Add(effect);
                    }
                }
            }

            // 3) Rename the consumable to include affix names.
            string baseName = itemName;
            itemName =
                (prefix != null ? prefix.prefixName + " " : "")
                + baseName
                + (suffix != null ? " " + suffix.suffixName : "");
        }

        /// <summary>
        /// Adds the provided stat modifiers to consumableStats.
        /// </summary>
        private void ApplyStats(Dictionary<Stat, float> modifiers)
        {
            if (modifiers == null)
                return;

            foreach (var kvp in modifiers)
            {
                if (!consumableStats.ContainsKey(kvp.Key))
                    consumableStats[kvp.Key] = 0f;

                consumableStats[kvp.Key] += kvp.Value;
            }
        }

        // ------------------------------
        // Pricing
        // ------------------------------

        /// <summary>
        /// Calculates the price based on base price, any suffix price increase, and prefix price multiplier.
        /// </summary>
        public int GetPrice()
        {
            return (int)((price + suffixPriceIncrease) * prefixPriceMultiplier);
        }

        // ------------------------------
        // Consumption Logic
        // ------------------------------

        /// <summary>
        /// When consumed, this method applies the stat boosts and status effects to the player.
        /// (It assumes that PlayerStats and StatusEffectManager expose methods similar to those used for Equipment.)
        /// </summary>
        public void Consume()
        {
            var playerStats = PlayerStats.Instance;
            if (playerStats != null)
            {
                if (consumableStats[Stat.HP] > 0)
                    playerStats.Heal(consumableStats[Stat.HP]);

                if (consumableStats[Stat.Dexterity] > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainDexterity(consumableStats[Stat.Dexterity], duration)
                    );

                if (consumableStats[Stat.Intelligence] > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainIntelligence(consumableStats[Stat.Intelligence], duration)
                    );

                if (consumableStats[Stat.CritChance] > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainCritChance(consumableStats[Stat.CritChance] / 100, duration)
                    );

                if (consumableStats[Stat.Attack] > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainAttack(consumableStats[Stat.Attack], duration)
                    );

                if (consumableStats[Stat.Defense] > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainDefense(consumableStats[Stat.Defense], duration)
                    );

                if (consumableStats[Stat.Speed] > 0)
                    playerStats.StartCoroutine(
                        playerStats.GainSpeed(consumableStats[Stat.Speed], duration)
                    );
            }

            // Apply status effects.
            foreach (var effect in addedEffects)
                StatusEffectManager.Instance.AddStatusEffect(
                    playerStats.gameObject,
                    effect,
                    consumableStats[Stat.StatusEffectDuration]
                );

            foreach (var effect in removedEffects)
                StatusEffectManager.Instance.RemoveStatusEffect(playerStats.gameObject, effect);
        }
    }
}
