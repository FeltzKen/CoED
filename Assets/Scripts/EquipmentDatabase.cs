using System.Collections.Generic;
using UnityEngine;

public static class EquipmentDatabase
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
            statusEffect = StatusEffectType.RandomBuff,
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
            statusEffect = StatusEffectType.RandomDebuff,
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
            statusEffect = StatusEffectType.Burn,
        },
        new PrefixData
        {
            prefixName = "Freezing",
            description = "Adds ice damage",
            attackModifier = 5,
            statusEffect = StatusEffectType.Slow,
        },
        new PrefixData
        {
            prefixName = "Shocking",
            description = "Adds lightning damage",
            attackModifier = 5,
            statusEffect = StatusEffectType.Stun,
        },
        new PrefixData
        {
            prefixName = "Venomous",
            description = "Adds poison damage",
            attackModifier = 5,
            statusEffect = StatusEffectType.Poison,
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
        },
    };

    // 🔹 Greater Prefixes (Enhanced Positive Modifiers)
    public static List<PrefixData> greaterPrefixes = new List<PrefixData>()
    {
        new PrefixData
        {
            prefixName = "Blazing",
            description = "Adds stronger fire damage",
            attackModifier = 20,
            statusEffect = StatusEffectType.Burn,
        },
        new PrefixData
        {
            prefixName = "Frostforged",
            description = "Adds freezing ice damage",
            attackModifier = 20,
            statusEffect = StatusEffectType.Freeze,
        },
        new PrefixData
        {
            prefixName = "Storming",
            description = "Enhances lightning damage",
            attackModifier = 20,
            statusEffect = StatusEffectType.Stun,
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
            description = "Provides a shield on crit",
            defenseModifier = 20,
            statusEffect = StatusEffectType.Shield,
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
            statusEffect = StatusEffectType.Rebirth,
        },
        new SuffixData
        {
            suffixName = "of Shadows",
            description = "Increases stealth movement speed",
            speedBonus = 30,
        },
    };

    // 🔹 Cursed Prefixes (Negative Modifiers)
    public static List<PrefixData> cursedPrefixes = new List<PrefixData>()
    {
        new PrefixData
        {
            prefixName = "Corrupted",
            description = "Applies random negative effects",
            statusEffect = StatusEffectType.RandomDebuff,
        },
        new PrefixData
        {
            prefixName = "Draining",
            description = "Gradually drains health",
            healthModifier = -5,
        },
        new PrefixData
        {
            prefixName = "Fragile",
            description = "Reduces durability",
            defenseModifier = -20,
        },
        new PrefixData
        {
            prefixName = "Rusty",
            description = "Decreases defense",
            defenseModifier = -15,
        },
        new PrefixData
        {
            prefixName = "Heavy",
            description = "Slows movement",
            speedModifier = -10,
        },
    };

    // 🔹 Greater Cursed Prefixes (Severe Negative Modifiers)
    public static List<PrefixData> greaterCursedPrefixes = new List<PrefixData>()
    {
        new PrefixData
        {
            prefixName = "Doomed",
            description = "Applies random negative effects",
            statusEffect = StatusEffectType.RandomDebuff,
        },
        new PrefixData
        {
            prefixName = "Withering",
            description = "Gradually drains health",
            healthModifier = -5,
        },
        new PrefixData
        {
            prefixName = "Sluggish",
            description = "Heavily reduces movement speed",
            speedModifier = -40,
        },
        new PrefixData
        {
            prefixName = "Backfiring",
            description = "Chance to damage the user",
            attackModifier = -10,
        },
    };
}
