using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class PricingLibrary
    {
        private static Dictionary<string, int> ValuePerStat = new Dictionary<string, int>
        {
            { "Attack", 2 },
            { "Defense", 3 },
            { "MaxMagic", 2 },
            { "MaxHP", 3 },
            { "MaxStamina", 1 },
            { "Intelligence", 2 },
            { "Dexterity", 3 },
            { "Speed", 2 },
            { "CritChance", 3 },
            { "CritDamage", 2 },
            { "ProjectileRange", 2 },
            { "AttackRange", 3 },
            { "ElementalDamage", 2 },
            { "ChanceToInflictStatusEffect", 3 },
            { "StatusEffectDuration", 2 },
            { "FireRate", 2 },
            { "Shield", 2 },
            { "Accuracy", 3 },
            { "Evasion", 3 },
        };

        private static Dictionary<StatusEffectType, int> statusEffectValues = new Dictionary<
            StatusEffectType,
            int
        >
        {
            { StatusEffectType.Burn, 10 },
            { StatusEffectType.Freeze, 12 },
            { StatusEffectType.Poison, 15 },
            { StatusEffectType.Paralyze, 20 },
            { StatusEffectType.Stun, 15 },
            { StatusEffectType.Bleed, 20 },
            { StatusEffectType.Curse, 50 },
        };

        private static Dictionary<Resistances, int> resistanceValues = new Dictionary<
            Resistances,
            int
        >
        {
            { Resistances.Physical, 30 },
            { Resistances.Fire, 40 },
            { Resistances.Ice, 50 },
            { Resistances.Poison, 50 },
            { Resistances.Lightning, 40 },
            { Resistances.Shadow, 50 },
            { Resistances.Holy, 55 },
        };

        private static Dictionary<Immunities, int> immunityValues = new Dictionary<Immunities, int>
        {
            { Immunities.Physical, 60 },
            { Immunities.Fire, 80 },
            { Immunities.Ice, 100 },
            { Immunities.Poison, 100 },
            { Immunities.Lightning, 80 },
            { Immunities.Shadow, 100 },
            { Immunities.Holy, 110 },
        };

        private static Dictionary<StatusEffectType, int> removeStatusEffectValues = new Dictionary<
            StatusEffectType,
            int
        >
        {
            { StatusEffectType.Burn, 20 },
            { StatusEffectType.Freeze, 20 },
            { StatusEffectType.Poison, 25 },
            { StatusEffectType.Paralyze, 15 },
            { StatusEffectType.Stun, 20 },
            { StatusEffectType.Bleed, 25 },
            { StatusEffectType.Curse, 50 },
        };

        public static float CalculateEquipmentPrice(Equipment equipment)
        {
            float totalPrice = 0;

            totalPrice += equipment.equipmentStats[Stat.Attack] * ValuePerStat["Attack"];
            totalPrice += equipment.equipmentStats[Stat.Defense] * ValuePerStat["Defense"];
            totalPrice += equipment.equipmentStats[Stat.MaxMagic] * ValuePerStat["MaxMagic"];
            totalPrice += equipment.equipmentStats[Stat.MaxHP] * ValuePerStat["MaxHP"];
            totalPrice += equipment.equipmentStats[Stat.MaxStamina] * ValuePerStat["MaxStamina"];
            totalPrice += equipment.equipmentStats[Stat.Intelligence] * ValuePerStat["Intelligence"];
            totalPrice += equipment.equipmentStats[Stat.Dexterity] * ValuePerStat["Dexterity"];
            totalPrice += equipment.equipmentStats[Stat.Speed] * ValuePerStat["Speed"];
            totalPrice += equipment.equipmentStats[Stat.CritChance] * ValuePerStat["CritChance"];
            totalPrice += equipment.equipmentStats[Stat.CritDamage] * ValuePerStat["CritDamage"];
            totalPrice +=
                equipment.equipmentStats[Stat.ProjectileRange] * ValuePerStat["ProjectileRange"];
            totalPrice += equipment.equipmentStats[Stat.AttackRange] * ValuePerStat["AttackRange"];
            totalPrice +=
                equipment.equipmentStats[Stat.ElementalDamage] * ValuePerStat["ElementalDamage"];
            totalPrice +=
                equipment.equipmentStats[Stat.ChanceToInflictStatusEffect]
                * ValuePerStat["ChanceToInflictStatusEffect"];
            totalPrice +=
                equipment.equipmentStats[Stat.StatusEffectDuration]
                * ValuePerStat["StatusEffectDuration"];
            totalPrice += equipment.equipmentStats[Stat.FireRate] * ValuePerStat["FireRate"];
            totalPrice += equipment.equipmentStats[Stat.Shield] * ValuePerStat["Shield"];
            totalPrice += equipment.equipmentStats[Stat.Accuracy] * ValuePerStat["Accuracy"];
            totalPrice += equipment.equipmentStats[Stat.Evasion] * ValuePerStat["Evasion"];

            foreach (var status in equipment.inflictedStatusEffects)
            {
                if (statusEffectValues.ContainsKey(status))
                    totalPrice += statusEffectValues[status];
            }

            foreach (var resistance in equipment.resistanceEffects)
            {
                if (resistanceValues.ContainsKey(resistance))
                    totalPrice += resistanceValues[resistance];
            }

            foreach (var immunity in equipment.immunityEffects)
            {
                if (immunityValues.ContainsKey(immunity))
                    totalPrice += immunityValues[immunity];
            }

            return Mathf.Max(1, totalPrice); // Ensure minimum price of 1
        }

        public static int CalculateConsumablePrice(ConsumableItem consumable)
        {
            int totalPrice = 0;

            totalPrice += (int)consumable.attackBoost * ValuePerStat["Attack"];
            totalPrice += (int)consumable.defenseBoost * ValuePerStat["Defense"];
            totalPrice += (int)consumable.magicBoost * ValuePerStat["MaxMagic"];
            totalPrice += (int)consumable.healthBoost * ValuePerStat["MaxHP"];
            totalPrice += (int)consumable.staminaBoost * ValuePerStat["MaxStamina"];
            totalPrice += (int)consumable.intelligenceBoost * ValuePerStat["Intelligence"];
            totalPrice += (int)consumable.dexterityBoost * ValuePerStat["Dexterity"];
            totalPrice += (int)consumable.speedBoost * ValuePerStat["Speed"];
            totalPrice += (int)consumable.critChanceBoost * ValuePerStat["CritChance"];
            totalPrice += (int)consumable.duration;
            foreach (var effect in consumable.addedEffects)
            {
                if (statusEffectValues.ContainsKey(effect))
                    totalPrice += statusEffectValues[effect];
            }
            foreach (var effect in consumable.removedEffects)
            {
                if (removeStatusEffectValues.ContainsKey(effect))
                    totalPrice += removeStatusEffectValues[effect];
            }

            return Mathf.Max(1, totalPrice); // Ensure minimum price of 1
        }
    }
}
