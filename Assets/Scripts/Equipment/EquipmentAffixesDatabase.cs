using System.Collections.Generic;
using CoED;

public static class EquipmentAffixesDatabase
{
    // 🔹 Enchanted and Cursed prefixes
    public static List<PrefixData> pre_prefixes = new List<PrefixData>()
    {
        new PrefixData
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
        new PrefixData
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
    public static List<PrefixData> basicPrefixes = new List<PrefixData>()
    {
        new PrefixData
        {
            prefixName = "Fiery",
            description = "Adds fire damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Fire, 5 } },
        },
        new PrefixData
        {
            prefixName = "Freezing",
            description = "Adds ice damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Ice, 5 } },
        },
        new PrefixData
        {
            prefixName = "Shocking",
            description = "Adds lightning damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Lightning, 5 } },
        },
        new PrefixData
        {
            prefixName = "Venomous",
            description = "Adds poison damage",
            attackModifier = 5,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Poison, 5 } },
        },
        new PrefixData
        {
            prefixName = "Swift",
            description = "Increases attack speed",
            speedModifier = 10,
        },
        new PrefixData
        {
            prefixName = "Mighty",
            description = "Increases attack power",
            attackModifier = 10,
        },
        new PrefixData
        {
            prefixName = "Stalwart",
            description = "Increases defense",
            defenseModifier = 10,
        },
        new PrefixData
        {
            prefixName = "Mystic",
            description = "Increases magic power",
            magicModifier = 10,
        },
        new PrefixData
        {
            prefixName = "Vigilant",
            description = "Increases critical hit chance",
            critChanceModifier = 5,
        },
        new PrefixData
        {
            prefixName = "Fortified",
            description = "Increases maximum health",
            healthModifier = 50,
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Regen },
        },
    };

    // 🔹 Greater Prefixes (Enhanced Positive Modifiers)
    public static List<PrefixData> greaterPrefixes = new List<PrefixData>()
    {
        new PrefixData
        {
            prefixName = "Blazing",
            description = "Adds stronger fire damage and burn effect",
            attackModifier = 20,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Fire, 20 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Burn },
        },
        new PrefixData
        {
            prefixName = "Frostforged",
            description = "Adds freezing ice damage and freeze effect",
            attackModifier = 20,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Ice, 20 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Freeze },
        },
        new PrefixData
        {
            prefixName = "Storming",
            description = "Enhances lightning damage and stuns",
            attackModifier = 20,
            damageModifiers = new Dictionary<DamageType, int> { { DamageType.Lightning, 20 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Stun },
        },
        new PrefixData
        {
            prefixName = "Titanic",
            description = "Massive strength boost",
            attackModifier = 25,
            staminaModifier = 25,
        },
        new PrefixData
        {
            prefixName = "Arcane",
            description = "Boosts intelligence",
            magicModifier = 25,
            intelligenceModifier = 25,
        },
        new PrefixData
        {
            prefixName = "Guardian's",
            description = "Provides a shield",
            defenseModifier = 20,
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Shield },
        },
    };

    // 🔹 Basic Suffixes (Standard Utility Modifiers)
    public static List<SuffixData> basicSuffixes = new List<SuffixData>()
    {
        new SuffixData
        {
            suffixName = "of the Bear",
            description = "Boosts strength",
            staminaBonus = 20,
        },
        new SuffixData
        {
            suffixName = "of the Wolf",
            description = "Boosts agility",
            dexterityBonus = 20,
        },
        new SuffixData
        {
            suffixName = "of the Eagle",
            description = "Boosts crit chance",
            critChanceBonus = 5,
        },
        new SuffixData
        {
            suffixName = "of Vitality",
            description = "Increases health regen",
            healthBonus = 20,

            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Regen },
        },
        new SuffixData
        {
            suffixName = "of the Wind",
            description = "Increases movement speed",
            speedBonus = 10,
        },
    };

    // 🔹 Greater Suffixes (Enhanced Utility Modifiers)
    public static List<SuffixData> greaterSuffixes = new List<SuffixData>()
    {
        new SuffixData
        {
            suffixName = "of the Colossus",
            description = "Massive health boost",
            healthBonus = 400,
        },
        new SuffixData
        {
            suffixName = "of Fury",
            description = "Increases attack speed",
            speedBonus = 25,
        },
        new SuffixData
        {
            suffixName = "of the Phoenix",
            description = "Revives once upon death",
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Rebirth },
            isOneTimeEffect = true,
        },
        new SuffixData
        {
            suffixName = "of Shadows",
            description = "Increases stealth movement speed",
            speedBonus = 30,
        },
        new SuffixData
        {
            suffixName = "of Thorns",
            description = "Reflects a portion of damage back to attackers",
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.DamageReflect },
        },
        new SuffixData
        {
            suffixName = "of Absorption",
            description = "Grants a damage-absorbing shield",
            activeStatusEffects = new List<StatusEffectType> { StatusEffectType.Shield },
            isOneTimeEffect = true,
        },
    };
}
