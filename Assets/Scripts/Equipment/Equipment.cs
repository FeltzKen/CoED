using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    /// <summary>
    /// Example "Equipment" class that shows how to
    /// 1) Provide default stats.
    /// 2) Override them in a constructor.
    /// 3) Keep the affix logic (prefix/suffix) intact.
    /// </summary>
    [Serializable]
    public class Equipment : IShopItem
    {
        // ------------------------------
        // Fields
        // ------------------------------
        public string itemID;
        public string itemName;
        public EquipmentSlot slot;
        public Rarity rarity;
        public Sprite baseSprite;

        // If you want them optional:
        public EquipmentPrefixData prePrefix;
        public EquipmentPrefixData prefix;
        public EquipmentSuffixData suffix;

        // Example: used to store "Enchanted" or "Cursed" from prePrefix
        public string hiddenNameData;
        public bool isEnchantedOrCursed;
        public bool isOneTimeEffect;
        public bool effectUsed;
        public bool hasBeenRevealed;

        // For consistency, still keep a dictionary with default 0f for every Stat you care about.
        // This ensures every new Equipment object starts with a "full set" of stats at zero.
        public Dictionary<Stat, float> equipmentStats = new Dictionary<Stat, float>
        {
            { Stat.Attack, 0f },
            { Stat.Defense, 0f },
            { Stat.Intelligence, 0f },
            { Stat.Dexterity, 0f },
            { Stat.Speed, 0f },
            { Stat.CritChance, 0f },
            { Stat.CritDamage, 0f },
            { Stat.ProjectileRange, 0f },
            { Stat.AttackRange, 0f },
            { Stat.ElementalDamage, 0f },
            { Stat.ChanceToInflictStatusEffect, 0f },
            { Stat.StatusEffectDuration, 0f },
            { Stat.FireRate, 0f },
            { Stat.Shield, 0f },
            { Stat.Accuracy, 0f },
            { Stat.MaxHP, 0f },
            { Stat.MaxMagic, 0f },
            { Stat.MaxStamina, 0f },
            { Stat.Evasion, 0f },
        };

        // Elemental damage
        public Dictionary<DamageType, float> damageModifiers = new Dictionary<DamageType, float>();
        public Dictionary<DamageType, float> enchantedOrCursedModifiers =
            new Dictionary<DamageType, float>();

        // Status effects
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public List<StatusEffectType> hiddenStatusEffects = new List<StatusEffectType>();
        public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();
        public List<Resistances> resistanceEffects = new List<Resistances>();
        public List<Weaknesses> weaknessEffects = new List<Weaknesses>();
        public List<Immunities> immunityEffects = new List<Immunities>();

        // ------------------------------
        // Constructors
        // ------------------------------

        /// <summary>
        /// Default constructor (required for serialization).
        /// </summary>
        public Equipment()
        {
            // If you want special logic whenever you create a new Equipment,
            // you can put it here.
        }

        /// <summary>
        /// Constructor that lets you override any stats you want in a dictionary.
        /// This is often more flexible than a 15-argument constructor.
        /// </summary>
        public Equipment(
            string itemID,
            string itemName,
            EquipmentSlot slot,
            Rarity rarity,
            Sprite baseSprite = null,
            bool isOneTimeEffect = false,
            Dictionary<Stat, float> statOverrides = null
        )
        {
            this.itemID = itemID;
            this.itemName = itemName;
            this.slot = slot;
            this.rarity = rarity;
            this.baseSprite = baseSprite;

            // We already have default 0.0f for each Stat in equipmentStats.
            // But if the caller wants to override certain stats:
            if (statOverrides != null)
            {
                foreach (var pair in statOverrides)
                {
                    // Make sure the key exists (it does, but just in case):
                    if (!equipmentStats.ContainsKey(pair.Key))
                        equipmentStats[pair.Key] = 0f;

                    // Override with the new value
                    equipmentStats[pair.Key] = pair.Value;
                }
            }
        }

        public Equipment(Equipment other)
        {
            // Basic fields
            this.itemID = other.itemID;
            this.itemName = other.itemName;
            this.slot = other.slot;
            this.rarity = other.rarity;
            this.baseSprite = other.baseSprite;

            // Dictionaries: clone them so we don't share references
            this.equipmentStats = new Dictionary<Stat, float>(other.equipmentStats);
            this.damageModifiers = new Dictionary<DamageType, float>(other.damageModifiers);
            this.enchantedOrCursedModifiers = new Dictionary<DamageType, float>(
                other.enchantedOrCursedModifiers
            );

            // Copy status effect lists
            this.activeStatusEffects = new List<StatusEffectType>(other.activeStatusEffects);
            this.hiddenStatusEffects = new List<StatusEffectType>(other.hiddenStatusEffects);
            this.inflictedStatusEffects = new List<StatusEffectType>(other.inflictedStatusEffects);
            this.resistanceEffects = new List<Resistances>(other.resistanceEffects);
            this.weaknessEffects = new List<Weaknesses>(other.weaknessEffects);
            this.immunityEffects = new List<Immunities>(other.immunityEffects);

            // Other booleans
            this.isOneTimeEffect = other.isOneTimeEffect;
            this.effectUsed = other.effectUsed;
            this.hasBeenRevealed = other.hasBeenRevealed;
            this.isEnchantedOrCursed = other.isEnchantedOrCursed;
            this.hiddenNameData = other.hiddenNameData;

            // Affix references
            this.prePrefix = other.prePrefix; // We can just copy the references if they are scriptable objects / data classes
            this.prefix = other.prefix;
            this.suffix = other.suffix;
        }

        // ------------------------------
        // Interface & Utility Methods
        // ------------------------------

        public string GetName() => itemName;

        public Sprite GetSprite() => baseSprite;

        /// <summary>
        /// Example "describe" function: you could expand or customize as needed.
        /// </summary>
        public string GetDescription()
        {
            string desc = itemName;

            if (prePrefix != null)
                desc = prePrefix.prefixName + " " + desc;
            if (suffix != null)
                desc += " " + suffix.suffixName;

            // Add linebreak to show final "itemName" again, or whatever you prefer
            desc += "\n" + itemName;
            return desc;
        }

        // ------------------------------
        // Example Affix Logic
        // ------------------------------

        /// <summary>
        /// Called after you set up "prefix"/"suffix"/"prePrefix"
        /// to finalize stats and naming.
        /// </summary>
        public void ApplyAffixes()
        {
            // 1) Pre-prefix logic (Enchanted or Cursed)
            if (prePrefix != null)
            {
                // Stash the "Enchanted"/"Cursed" for reveal
                hiddenNameData = prePrefix.prefixName;

                // Copy damage modifiers from prePrefix into "enchantedOrCursedModifiers"
                foreach (var modifier in prePrefix.damageModifiers)
                {
                    if (enchantedOrCursedModifiers.ContainsKey(modifier.Key))
                        enchantedOrCursedModifiers[modifier.Key] += modifier.Value;
                    else
                        enchantedOrCursedModifiers[modifier.Key] = modifier.Value;
                }

                // Add any hidden status effects
                if (prePrefix.activeStatusEffects != null)
                {
                    foreach (var effect in prePrefix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            hiddenStatusEffects.Add(effect);
                    }
                }
            }

            // 2) Prefix
            if (prefix != null)
            {
                // Add stats from prefix
                ApplyStats(prefix.statModifiers);

                // Merge damage modifiers from prefix
                MergeDamageModifiers(prefix.damageModifiers, damageModifiers);

                // Add active status effects from prefix
                if (prefix.activeStatusEffects != null)
                {
                    foreach (var effect in prefix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            activeStatusEffects.Add(effect);
                    }
                }
            }

            // 3) Suffix
            if (suffix != null)
            {
                ApplyStats(suffix.statModifiers);
                MergeDamageModifiers(suffix.damageModifiers, damageModifiers);

                if (suffix.activeStatusEffects != null)
                {
                    foreach (var effect in suffix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            activeStatusEffects.Add(effect);
                    }
                }
            }

            // 4) Rename the item based on prefix / suffix
            //    e.g. "Fiery Iron Sword of the Bear"
            string baseName = itemName;
            itemName =
                (prefix != null ? prefix.prefixName + " " : "")
                + baseName
                + (suffix != null ? " " + suffix.suffixName : "");
        }

        /// <summary>
        /// Reveals "Enchanted" or "Cursed" if the item had a prePrefix.
        /// Typically called when you actually equip or identify the item.
        /// </summary>
        public void RevealHiddenAttributes(bool shopRequest = false)
        {
            if (hasBeenRevealed || prePrefix == null)
                return;

            // Move the prePrefix damage modifiers into the real dictionary
            if (enchantedOrCursedModifiers.Count > 0)
            {
                foreach (var kvp in enchantedOrCursedModifiers)
                {
                    damageModifiers[kvp.Key] = kvp.Value;
                }
            }

            // Apply the prePrefix stats onto the final equipmentStats
            ApplyStats(prePrefix.statModifiers);

            // Add the prePrefix name to the front: "Cursed Iron Sword"
            itemName = prePrefix.prefixName + " " + itemName;

            // Transfer hidden status effects
            if (hiddenStatusEffects.Count > 0)
            {
                foreach (var effect in hiddenStatusEffects)
                {
                    if (!activeStatusEffects.Contains(effect))
                        activeStatusEffects.Add(effect);
                }
            }

            // Optionally show floating text (depends on your game’s UI code)
            if (!shopRequest)
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    $"The equipped item is {hiddenNameData}!",
                    // Where to show it
                    PlayerStats.Instance.transform,
                    // Color based on curse or enchant
                    hiddenNameData == "Cursed"
                        ? Color.red
                        : Color.green
                );
            }

            hasBeenRevealed = true;
        }

        // ------------------------------
        // Private Helpers
        // ------------------------------

        /// <summary>
        /// Applies the given dictionary of stat modifiers to equipmentStats.
        /// </summary>
        private void ApplyStats(Dictionary<Stat, float> modifiers)
        {
            if (modifiers == null)
                return;

            foreach (var kvp in modifiers)
            {
                if (!equipmentStats.ContainsKey(kvp.Key))
                    equipmentStats[kvp.Key] = 0f;

                equipmentStats[kvp.Key] += kvp.Value;
            }
        }

        /// <summary>
        /// Merges the 'source' damage modifiers into 'target' dictionary.
        /// </summary>
        private void MergeDamageModifiers(
            Dictionary<DamageType, float> source,
            Dictionary<DamageType, float> target
        )
        {
            if (source == null)
                return;

            foreach (var kvp in source)
            {
                if (target.ContainsKey(kvp.Key))
                    target[kvp.Key] += kvp.Value;
                else
                    target[kvp.Key] = kvp.Value;
            }
        }
    }
}
