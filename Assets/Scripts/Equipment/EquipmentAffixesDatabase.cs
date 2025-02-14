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
            statModifiers = new Dictionary<Stat, float>
            {
                { Stat.Attack, 5 },
                { Stat.Defense, 5 },
                { Stat.MaxMagic, 5 },
                { Stat.MaxHP, 10 },
                { Stat.MaxStamina, 10 },
                { Stat.Intelligence, 5 },
                { Stat.Dexterity, 5 },
                { Stat.Speed, 5 },
                { Stat.CritChance, 5 },
                { Stat.CritDamage, 5 },
                { Stat.ProjectileRange, 5 },
                { Stat.AttackRange, 5 },
                { Stat.ElementalDamage, 5 },
                { Stat.ChanceToInflict, 5 },
                { Stat.StatusEffectDuration, 5 },
                { Stat.FireRate, 5 },
                { Stat.Shield, 5 },
            },
        },
        new EquipmentPrefixData
        {
            prefixName = "Cursed",
            description = "Adds random negative effects",
            statModifiers = new Dictionary<Stat, float>
            {
                { Stat.Attack, -5 },
                { Stat.Defense, -5 },
                { Stat.MaxMagic, -5 },
                { Stat.MaxHP, -10 },
                { Stat.MaxStamina, -10 },
                { Stat.Intelligence, -5 },
                { Stat.Dexterity, -5 },
                { Stat.Speed, -5 },
                { Stat.CritChance, -5 },
                { Stat.CritDamage, -5 },
                { Stat.ProjectileRange, -5 },
                { Stat.AttackRange, -5 },
                { Stat.ElementalDamage, -5 },
                { Stat.ChanceToInflict, -5 },
                { Stat.StatusEffectDuration, -5 },
                { Stat.FireRate, -5 },
                { Stat.Shield, -5 },
            },
            equipmentEffects = new List<StatusEffectType> { StatusEffectType.RandomDebuff },
        },
    };

    // 🔹 Basic Prefixes (Standard Positive Modifiers)
    public static List<EquipmentPrefixData> basicPrefixes = new List<EquipmentPrefixData>()
    {
        new EquipmentPrefixData
        {
            prefixName = "Fiery",
            description = "Adds fire damage",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 5 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Fire, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Freezing",
            description = "Adds ice damage",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 5 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Ice, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Shocking",
            description = "Adds lightning damage",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 5 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Lightning, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Venomous",
            description = "Adds poison damage",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 5 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Poison, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Swift",
            description = "Increases attack speed",
            statModifiers = new Dictionary<Stat, float> { { Stat.Speed, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Mighty",
            description = "Increases attack power",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Stalwart",
            description = "Increases defense",
            statModifiers = new Dictionary<Stat, float> { { Stat.Defense, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Mystic",
            description = "Increases MaxMagic power",
            statModifiers = new Dictionary<Stat, float> { { Stat.MaxMagic, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Vigilant",
            description = "Increases critical hit chance",
            statModifiers = new Dictionary<Stat, float> { { Stat.CritChance, 5 } },
        },
        new EquipmentPrefixData
        {
            prefixName = "Fortified",
            description = "Increases maximum health",
            statModifiers = new Dictionary<Stat, float> { { Stat.MaxHP, 50 } },
            equipmentEffects = new List<StatusEffectType> { StatusEffectType.Regen },
        },
    };

    // 🔹 Greater Prefixes (Enhanced Positive Modifiers)
    public static List<EquipmentPrefixData> greaterPrefixes = new List<EquipmentPrefixData>()
    {
        new EquipmentPrefixData
        {
            prefixName = "Blazing",
            description = "Adds stronger fire damage and burn effect",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 10 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Fire, 20 } },
            inflictedEffectsFromEquipment = new List<StatusEffectType> { StatusEffectType.Burn },
        },
        new EquipmentPrefixData
        {
            prefixName = "Frostforged",
            description = "Adds freezing ice damage and freeze effect",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 10 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Ice, 20 } },
            inflictedEffectsFromEquipment = new List<StatusEffectType> { StatusEffectType.Freeze },
        },
        new EquipmentPrefixData
        {
            prefixName = "Storming",
            description = "Enhances lightning damage and stuns",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 10 } },
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Lightning, 20 } },
            inflictedEffectsFromEquipment = new List<StatusEffectType> { StatusEffectType.Stun },
        },
        new EquipmentPrefixData
        {
            prefixName = "Titanic",
            description = "Massive strength boost",
            statModifiers = new Dictionary<Stat, float>
            {
                { Stat.Attack, 20 },
                { Stat.MaxStamina, 20 },
            },
        },
        new EquipmentPrefixData
        {
            prefixName = "Arcane",
            description = "Boosts floatelligence",
            statModifiers = new Dictionary<Stat, float>
            {
                { Stat.MaxMagic, 20 },
                { Stat.Intelligence, 9 },
            },
        },
        new EquipmentPrefixData
        {
            prefixName = "Guardian's",
            description = "Provides a shield",
            statModifiers = new Dictionary<Stat, float> { { Stat.Shield, 50 } },
            equipmentEffects = new List<StatusEffectType> { StatusEffectType.Shield },
        },
    };

    // 🔹 Basic Suffixes (Standard Utility Modifiers)
    public static List<EquipmentSuffixData> basicSuffixes = new List<EquipmentSuffixData>()
    {
        new EquipmentSuffixData
        {
            suffixName = "of the Bear",
            description = "Boosts strength",
            statModifiers = new Dictionary<Stat, float> { { Stat.Attack, 10 } },
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Wolf",
            description = "Boosts agility",
            statModifiers = new Dictionary<Stat, float> { { Stat.Dexterity, 10 } },
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Eagle",
            description = "Boosts crit chance",
            statModifiers = new Dictionary<Stat, float> { { Stat.CritChance, 0.1f } },
        },
        new EquipmentSuffixData
        {
            suffixName = "of Vitality",
            description = "Increases health regen",
            statModifiers = new Dictionary<Stat, float> { { Stat.MaxHP, 10 } },

            equipmentEffects = new List<StatusEffectType> { StatusEffectType.Regen },
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Wind",
            description = "Increases movement speed",
            statModifiers = new Dictionary<Stat, float> { { Stat.Speed, 10 } },
        },
    };

    // 🔹 Greater Suffixes (Enhanced Utility Modifiers)
    public static List<EquipmentSuffixData> greaterSuffixes = new List<EquipmentSuffixData>()
    {
        new EquipmentSuffixData
        {
            suffixName = "of the Colossus",
            description = "Massive health boost",
            statModifiers = new Dictionary<Stat, float> { { Stat.MaxHP, 100 } },
        },
        new EquipmentSuffixData
        {
            suffixName = "of Fury",
            description = "Increases attack speed",
            statModifiers = new Dictionary<Stat, float> { { Stat.Speed, 20 } },
        },
        new EquipmentSuffixData
        {
            suffixName = "of the Phoenix",
            description = "Revives once, if equipped, upon death",
            equipmentEffects = new List<StatusEffectType> { StatusEffectType.ReviveOnce },
            isOneTimeEffect = true,
        },
        new EquipmentSuffixData
        {
            suffixName = "of Shadows",
            description = "Increases stealth movement speed",
            statModifiers = new Dictionary<Stat, float> { { Stat.Speed, 20 } },
        },
        new EquipmentSuffixData
        {
            suffixName = "of Thorns",
            description = "Reflects a portion of damage back to attackers",
            equipmentEffects = new List<StatusEffectType> { StatusEffectType.DamageReflect },
        },
        new EquipmentSuffixData
        {
            suffixName = "of Absorption",
            description = "Grants a damage-absorbing shield",
            equipmentEffects = new List<StatusEffectType> { StatusEffectType.Shield },
            isOneTimeEffect = true,
        },
    };
}
