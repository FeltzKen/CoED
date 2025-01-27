using System.Collections.Generic;
using CoED;

public static class EquipmentAffixesDatabase
{
    // 🔹 Enchanted and Cursed prefixes
    public static List<EquipmentPrefixData> pre_prefixes = new List<EquipmentPrefixData>()
    {
        new EquipmentPrefixData
        {
            prefixName = "Enchanted",
            description = "Adds random positive effects",
            attackModifier = 5,
            defenseModifier = 5,
            magicModifier = 5,
            healthModifier = 10,
            staminaModifier = 10,
            intelligenceModifier = 5,
            dexterityModifier = 5,
            speedModifier = 5,
            critChanceModifier = 5,
        },
        new EquipmentPrefixData
        {
            prefixName = "Cursed",
            description = "Adds random negative effects",
            attackModifier = -5,
            defenseModifier = -5,
            magicModifier = -5,
            healthModifier = -10,
            staminaModifier = -10,
            intelligenceModifier = -5,
            dexterityModifier = -5,
            speedModifier = -5,
            critChanceModifier = -5,
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.RandomDebuff },
        },
    };

    // 🔹 Basic Prefixes (Standard Positive Modifiers)
    public static List<EquipmentPrefixData> basicPrefixes = new List<EquipmentPrefixData>()
    {
        new EquipmentPrefixData
        {
            prefixName = "Fiery",
            description = "Adds fire damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Fire, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Freezing",
            description = "Adds ice damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Ice, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Shocking",
            description = "Adds lightning damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Lightning, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Venomous",
            description = "Adds poison damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Poison, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Swift",
            description = "Increases attack speed",
            speedModifier = 10,
        },
        new EquipmentPrefixData
        {
            prefixName = "Mighty",
            description = "Increases attack power",
            attackModifier = 10,
        },
        new EquipmentPrefixData
        {
            prefixName = "Stalwart",
            description = "Increases defense",
            defenseModifier = 10,
        },
        new EquipmentPrefixData
        {
            prefixName = "Mystic",
            description = "Increases magic power",
            magicModifier = 10,
        },
        new EquipmentPrefixData
        {
            prefixName = "Vigilant",
            description = "Increases critical hit chance",
            critChanceModifier = 5,
        },
        new EquipmentPrefixData
        {
            prefixName = "Fortified",
            description = "Increases maximum health",
            healthModifier = 50,
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Regen },
        },
    };

    // 🔹 Greater Prefixes (Enhanced Positive Modifiers)
    public static List<EquipmentPrefixData> greaterPrefixes = new List<EquipmentPrefixData>()
    {
        new EquipmentPrefixData
        {
            prefixName = "Blazing",
            description = "Adds stronger fire damage and burn effect",
            attackModifier = 20,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Fire, 20 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Burn },
        },
        new EquipmentPrefixData
        {
            prefixName = "Frostforged",
            description = "Adds freezing ice damage and freeze effect",
            attackModifier = 20,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Ice, 20 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Freeze },
        },
        new EquipmentPrefixData
        {
            prefixName = "Storming",
            description = "Enhances lightning damage and stuns",
            attackModifier = 20,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Lightning, 20 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Stun },
        },
        new EquipmentPrefixData
        {
            prefixName = "Titanic",
            description = "Massive strength boost",
            attackModifier = 25,
            staminaModifier = 25,
        },
        new EquipmentPrefixData
        {
            prefixName = "Arcane",
            description = "Boosts intelligence",
            magicModifier = 25,
            intelligenceModifier = 25,
        },
        new EquipmentPrefixData
        {
            prefixName = "Guardian's",
            description = "Provides a shield",
            defenseModifier = 20,
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Shield },
        },
    };

    // 🔹 Basic Suffixes (Standard Utility Modifiers)
    public static List<EquipmentSuffixData> basicSuffixes = new List<EquipmentSuffixData>()
    {
        new EquipmentSuffixData
        {
            suffixName = "of the Bear",
            description = "Boosts strength",
            staminaBonus = 20,
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Wolf",
            description = "Boosts agility",
            dexterityBonus = 20,
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Eagle",
            description = "Boosts crit chance",
            critChanceBonus = 5,
        },
        new EquipmentSuffixData
        {
            suffixName = "of Vitality",
            description = "Increases health regen",
            healthBonus = 20,

            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Regen },
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Wind",
            description = "Increases movement speed",
            speedBonus = 10,
        },
    };

    // 🔹 Greater Suffixes (Enhanced Utility Modifiers)
    public static List<EquipmentSuffixData> greaterSuffixes = new List<EquipmentSuffixData>()
    {
        new EquipmentSuffixData
        {
            suffixName = "of the Colossus",
            description = "Massive health boost",
            healthBonus = 400,
        },
        new EquipmentSuffixData
        {
            suffixName = "of Fury",
            description = "Increases attack speed",
            speedBonus = 25,
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Phoenix",
            description = "Revives once upon death",
            equipmentEffects = new List<ActiveWhileEquipped> { ActiveWhileEquipped.ReviveOnce },
            isOneTimeEffect = true,
        },
        new EquipmentSuffixData
        {
            suffixName = "of Shadows",
            description = "Increases stealth movement speed",
            speedBonus = 30,
        },
        new EquipmentSuffixData
        {
            suffixName = "of Thorns",
            description = "Reflects a portion of damage back to attackers",
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.DamageReflect },
        },
        new EquipmentSuffixData
        {
            suffixName = "of Absorption",
            description = "Grants a damage-absorbing shield",
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Shield },
            isOneTimeEffect = true,
        },
    };
}
