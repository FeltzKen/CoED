using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CoED
{
    [System.Serializable]
    public class Equipment : IShopItem
    {
        public string itemID;
        public EquipmentPrefixData prePrefix = null;
        public EquipmentPrefixData prefix = null;
        public EquipmentSuffixData suffix = null;
        public string itemName;
        public string hiddenNameData;
        public EquipmentSlot slot;
        public int attack;
        public int defense;
        public int magic;
        public int health;
        public int stamina;
        public int intelligence;
        public int dexterity;
        public int speed;
        public int critChance;
        public bool isEnchantedOrCursed;

        public string GetName() => itemName;

        // Elemental Damage
        public Dictionary<DamageType, int> damageModifiers = new Dictionary<DamageType, int>();
        public Dictionary<DamageType, int> enchantedOrCursedModifiers =
            new Dictionary<DamageType, int>();

        // Player's active effects
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public List<StatusEffectType> hiddenStatusEffects = new List<StatusEffectType>();

        // Effects this equipment inflicts on others
        public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();
        public List<Resistances> resistanceEffects = new List<Resistances>();
        public List<Weaknesses> weaknessEffects = new List<Weaknesses>();
        public List<Immunities> immunityEffects = new List<Immunities>();
        public bool isOneTimeEffect;
        public bool effectUsed;
        public bool hasBeenRevealed;
        public Sprite baseSprite;
        public Rarity rarity;

        public Sprite GetSprite() => baseSprite;

        // Initialize default stats to prevent unintentional nulls
        public void InitializeEquipment(
            string id,
            string name,
            EquipmentSlot slot,
            Rarity rarity,
            Sprite baseSprite,
            int attack,
            int defense,
            int magic,
            int health,
            int stamina,
            int intelligence,
            int dexterity,
            int speed,
            int critChance,
            Dictionary<DamageType, int> damageModifiers,
            List<Resistances> resistanceEffects,
            List<Weaknesses> weaknessEffects,
            bool isOneTimeEffect,
            bool effectUsed,
            List<StatusEffectType> activeEffects,
            List<StatusEffectType> inflictedEffects
        )
        {
            itemID = id;
            itemName = name;
            this.slot = slot;
            this.rarity = rarity;
            this.baseSprite = baseSprite;
            this.attack = attack;
            this.defense = defense;
            this.magic = magic;
            this.health = health;
            this.stamina = stamina;
            this.intelligence = intelligence;
            this.dexterity = dexterity;
            this.speed = speed;
            this.critChance = critChance;
            this.damageModifiers = damageModifiers;
            this.activeStatusEffects = activeEffects;
            this.inflictedStatusEffects = inflictedEffects;
            this.resistanceEffects = resistanceEffects;
            this.weaknessEffects = weaknessEffects;
            this.isOneTimeEffect = isOneTimeEffect;
            this.effectUsed = effectUsed;
        }

        public void RevealHiddenAttributes(bool shopRequest = false)
        {
            if (hasBeenRevealed || prePrefix == null)
                return;

            // Apply Enchanted/Cursed Modifiers
            if (enchantedOrCursedModifiers.Count > 0)
            {
                foreach (var modifier in enchantedOrCursedModifiers)
                {
                    damageModifiers[modifier.Key] = modifier.Value;
                }
            }
            // Apply hidden stats
            ApplyStats(prePrefix, null, null);

            // Apply hidden name data
            itemName = prePrefix.prefixName + " " + itemName;

            // Apply hidden status effects
            if (hiddenStatusEffects.Count > 0)
            {
                foreach (var effect in hiddenStatusEffects)
                {
                    activeStatusEffects.Add(effect);
                }
            }
            if (!shopRequest)
                FloatingTextManager.Instance.ShowFloatingText(
                    $"The equipped item is {hiddenNameData}!",
                    PlayerStats.Instance.transform,
                    hiddenNameData == "Cursed" ? Color.red : Color.green
                );
            hasBeenRevealed = true;
        }

        public void ApplyAffixes()
        {
            // Apply Pre-Prefix Modifiers (Enchanted/Cursed)
            if (prePrefix != null)
            {
                hiddenNameData = prePrefix.prefixName;
                foreach (var modifier in prePrefix.damageModifiers)
                {
                    if (damageModifiers.ContainsKey(modifier.Key))
                        enchantedOrCursedModifiers[modifier.Key] += modifier.Value;
                    else
                        enchantedOrCursedModifiers.Add(modifier.Key, modifier.Value);
                }
                // Apply status effects from pre-prefix
                if (prePrefix.activeStatusEffects != null)
                {
                    foreach (var effect in prePrefix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            hiddenStatusEffects.Add(effect);
                    }
                }
            }

            // Apply Prefix Modifiers
            if (prefix != null)
            {
                ApplyStats(null, prefix, null);

                foreach (var modifier in prefix.damageModifiers)
                {
                    if (damageModifiers.ContainsKey(modifier.Key))
                        damageModifiers[modifier.Key] += modifier.Value;
                    else
                        damageModifiers.Add(modifier.Key, modifier.Value);
                }

                // Apply status effects from prefix
                if (prefix.activeStatusEffects != null)
                {
                    foreach (var effect in prefix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            activeStatusEffects.Add(effect);
                    }
                }
            }

            // Apply Suffix Modifiers
            if (suffix != null)
            {
                ApplyStats(null, null, suffix);

                foreach (var modifier in suffix.damageModifiers)
                {
                    if (damageModifiers.ContainsKey(modifier.Key))
                        damageModifiers[modifier.Key] += modifier.Value;
                    else
                        damageModifiers.Add(modifier.Key, modifier.Value);
                }

                // Apply status effects from suffix
                if (suffix.activeStatusEffects != null)
                {
                    foreach (var effect in suffix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            activeStatusEffects.Add(effect);
                    }
                }
            }

            // Format Item Name: [Prefix] [BaseName] [Suffix]
            itemName =
                $"{(prefix != null ? prefix.prefixName + " " : "")}"
                + $"{itemName}"
                + $"{(suffix != null ? " " + suffix.suffixName : "")}";
        }

        public string GetDescription()
        {
            string desc = "";
            if (prePrefix != null)
                desc = prePrefix.prefixName + " " + itemName;
            if (suffix != null)
                desc = desc + " " + suffix.suffixName;
            desc = desc + "\n" + itemName;
            return desc;
        }

        public int GetPrice()
        {
            int basePrice =
                attack
                + defense
                + magic
                + health
                + stamina
                + intelligence
                + dexterity
                + speed
                + critChance;
            int affixMultiplier = prePrefix != null || suffix != null ? 2 : 1;
            return basePrice * affixMultiplier;
        }

        private void ApplyStats(
            EquipmentPrefixData prePrefix = null,
            EquipmentPrefixData prefix = null,
            EquipmentSuffixData suffix = null
        )
        {
            if (prePrefix != null)
            {
                attack += prePrefix.attackModifier;
                defense += prePrefix.defenseModifier;
                magic += prePrefix.magicModifier;
                health += prePrefix.healthModifier;
                stamina += prePrefix.staminaModifier;
                intelligence += prePrefix.intelligenceModifier;
                dexterity += prePrefix.dexterityModifier;
                speed += prePrefix.speedModifier;
                critChance += prePrefix.critChanceModifier;
            }

            if (prefix != null)
            {
                attack += prefix.attackModifier;
                defense += prefix.defenseModifier;
                magic += prefix.magicModifier;
                health += prefix.healthModifier;
                stamina += prefix.staminaModifier;
                intelligence += prefix.intelligenceModifier;
                dexterity += prefix.dexterityModifier;
                speed += prefix.speedModifier;
                critChance += prefix.critChanceModifier;
            }

            if (suffix != null)
            {
                attack += suffix.attackBonus;
                defense += suffix.defenseBonus;
                magic += suffix.magicBonus;
                health += suffix.healthBonus;
                stamina += suffix.staminaBonus;
                intelligence += suffix.intelligenceBonus;
                dexterity += suffix.dexterityBonus;
                speed += suffix.speedBonus;
                critChance += suffix.critChanceBonus;
            }
        }
    }
}
