using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    [System.Serializable]
    public class Equipment
    {
        public string itemID;
        public PrefixData prePrefix = null;
        public PrefixData prefix = null;
        public SuffixData suffix = null;
        public string itemName;
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

        // Elemental Damage
        public Dictionary<DamageType, int> damageModifiers = new Dictionary<DamageType, int>();

        // Player's active effects
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();

        // Effects this equipment inflicts on others
        public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();
        public List<Resistances> resistanceEffects = new List<Resistances>();
        public List<Weaknesses> weaknessEffects = new List<Weaknesses>();
        public bool isOneTimeEffect;
        public bool effectUsed;
        public bool hasBeenRevealed;
        public Sprite prefixSprite;
        public Sprite suffixSprite;
        public Sprite baseSprite;
        public Rarity rarity;

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

        public float GetTotalAttackModifier() => attack;

        public float GetTotalDefenseModifier() => defense;

        public float GetTotalMagicModifier() => magic;

        public float GetTotalHealthModifier() => health;

        public float GetTotalStaminaModifier() => stamina;

        public float GetTotalIntelligenceModifier() => intelligence;

        public float GetTotalDexterityModifier() => dexterity;

        public float GetTotalSpeedModifier() => speed;

        public float GetTotalCritChanceModifier() => critChance;

        public float GetTotalBurnDamage() => damageModifiers.GetValueOrDefault(DamageType.Fire, 0);

        public float GetTotalIceDamage() => damageModifiers.GetValueOrDefault(DamageType.Ice, 0);

        public float GetTotalLightningDamage() =>
            damageModifiers.GetValueOrDefault(DamageType.Lightning, 0);

        public float GetTotalPoisonDamage() =>
            damageModifiers.GetValueOrDefault(DamageType.Poison, 0);

        public float GetTotalArcane() => damageModifiers.GetValueOrDefault(DamageType.Arcane, 0);

        public float GetTotalHoly() => damageModifiers.GetValueOrDefault(DamageType.Holy, 0);

        public float GetTotalShadow() => damageModifiers.GetValueOrDefault(DamageType.Shadow, 0);

        public float GetTotalBleed() => damageModifiers.GetValueOrDefault(DamageType.Bleed, 0);

        // Trigger One-Time Special Effects
        public void TriggerStatusEffects()
        {
            // Prevent repeated activation of one-time effects
            if (isOneTimeEffect && effectUsed)
                return;

            // Apply status effects to the player
            foreach (var effect in activeStatusEffects)
            {
                ApplyStatusEffectToPlayer(effect);
            }

            // Apply resistances to the player's resistance list
            foreach (var resistance in resistanceEffects)
            {
                if (!PlayerStats.Instance.activeResistances.Contains(resistance))
                {
                    PlayerStats.Instance.activeResistances.Add(resistance);
                    Debug.Log($"{itemName} granted resistance to {resistance}");
                }
            }

            // Apply weaknesses to the player's weakness list
            foreach (var weakness in weaknessEffects)
            {
                if (!PlayerStats.Instance.activeWeaknesses.Contains(weakness))
                {
                    PlayerStats.Instance.activeWeaknesses.Add(weakness);
                    Debug.Log($"{itemName} added weakness to {weakness}");
                }
            }

            // Mark one-time effects as used
            if (isOneTimeEffect)
                effectUsed = true;
        }

        private void ApplyStatusEffectToPlayer(StatusEffectType effect)
        {
            switch (effect)
            {
                case StatusEffectType.Regen:
                    //PlayerStats.Instance.StartRegen(5, 10); // Example: 5 HP/sec for 10 sec
                    Debug.Log($"{itemName} applied Regen effect.");
                    break;

                case StatusEffectType.Burn:
                    //PlayerStats.Instance.TakeDamageOverTime(3, 5); // 3 damage/sec for 5 sec
                    Debug.Log($"{itemName} applied Burn effect.");
                    break;

                case StatusEffectType.Shield:
                    PlayerStats.Instance.AddShield(10);
                    Debug.Log($"{itemName} granted Absorb Shield.");
                    break;

                case StatusEffectType.ReviveOnce:
                    if (!effectUsed)
                    {
                        //PlayerStats.Instance.Heal(PlayerStats.Instance.MaxHealth * 0.25f);
                        //effectUsed = true;
                        Debug.Log($"{itemName} revived the player.");
                    }
                    break;

                case StatusEffectType.DamageReflect:
                    //PlayerStats.Instance.ApplyDamageReflect(0.15f); // Reflect 15% damage
                    Debug.Log($"{itemName} enabled damage reflection.");
                    break;

                default:
                    Debug.LogWarning($"{itemName} has an unhandled effect: {effect}");
                    break;
            }
        }

        public void ApplyAffixes()
        {
            // Apply Pre-Prefix Modifiers (Enchanted/Cursed)
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

                foreach (var modifier in prePrefix.damageModifiers)
                {
                    if (damageModifiers.ContainsKey(modifier.Key))
                        damageModifiers[modifier.Key] += modifier.Value;
                    else
                        damageModifiers.Add(modifier.Key, modifier.Value);
                }

                // Apply status effects from pre-prefix
                if (prePrefix.activeStatusEffects != null)
                {
                    foreach (var effect in prePrefix.activeStatusEffects)
                    {
                        if (!activeStatusEffects.Contains(effect))
                            activeStatusEffects.Add(effect);
                    }
                }
            }

            // Apply Prefix Modifiers
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
                attack += suffix.attackBonus;
                defense += suffix.defenseBonus;
                magic += suffix.magicBonus;
                health += suffix.healthBonus;
                stamina += suffix.staminaBonus;
                intelligence += suffix.intelligenceBonus;
                dexterity += suffix.dexterityBonus;
                speed += suffix.speedBonus;
                critChance += suffix.critChanceBonus;

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

            // Format Item Name: [PrePrefix] [Prefix] [BaseName] [Suffix]
            itemName =
                $"{(prePrefix != null ? prePrefix.prefixName + " " : "")}"
                + $"{(prefix != null ? prefix.prefixName + " " : "")}"
                + $"{itemName}"
                + $"{(suffix != null ? " " + suffix.suffixName : "")}";
        }

        // Generate Item Description
        public string GetItemDescription()
        {
            string description = $"{itemName} - {rarity}\n";

            if (attack != 0)
                description += $"Attack: +{attack}\n";
            if (defense != 0)
                description += $"Defense: +{defense}\n";
            if (magic != 0)
                description += $"Magic: +{magic}\n";
            if (health != 0)
                description += $"Health: +{health}\n";
            if (stamina != 0)
                description += $"Stamina: +{stamina}\n";
            foreach (var modifier in damageModifiers)
            {
                description += $"{modifier.Key}: +{modifier.Value}\n";
            }

            if (activeStatusEffects != null && activeStatusEffects.Count > 0)
            {
                foreach (var effect in activeStatusEffects)
                {
                    description += $"Status Effect: {effect}\n";
                }
            }
            if (resistanceEffects != null && resistanceEffects.Count > 0)
            {
                foreach (var effect in resistanceEffects)
                {
                    description += $"Resistance: {effect}\n";
                }
            }
            if (weaknessEffects != null && weaknessEffects.Count > 0)
            {
                foreach (var effect in weaknessEffects)
                {
                    description += $"Weakness: {effect}\n";
                }
            }

            return description;
        }
    }
}
