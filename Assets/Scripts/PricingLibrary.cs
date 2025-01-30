using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class PricingLibrary
    {
        private static Dictionary<string, int> statValues = new Dictionary<string, int>
        {
            { "Attack", 5 },
            { "Defense", 3 },
            { "Magic", 6 },
            { "Health", 2 },
            { "Stamina", 1 },
            { "Intelligence", 8 },
            { "Dexterity", 3 },
            { "Speed", 4 },
            { "CritChance", 10 },
        };

        private static Dictionary<StatusEffectType, int> statusEffectValues = new Dictionary<
            StatusEffectType,
            int
        >
        {
            { StatusEffectType.Burn, 30 },
            { StatusEffectType.Freeze, 35 },
            { StatusEffectType.Poison, 25 },
            { StatusEffectType.Paralyze, 40 },
            { StatusEffectType.Stun, 50 },
            { StatusEffectType.Bleed, 20 },
            { StatusEffectType.Curse, 45 },
        };

        private static Dictionary<Resistances, int> resistanceValues = new Dictionary<
            Resistances,
            int
        >
        {
            { Resistances.Physical, 50 },
            { Resistances.Fire, 50 },
            { Resistances.Ice, 50 },
            { Resistances.Poison, 45 },
            { Resistances.Lightning, 60 },
            { Resistances.Shadow, 55 },
            { Resistances.Holy, 65 },
        };

        private static Dictionary<Immunities, int> immunityValues = new Dictionary<Immunities, int>
        {
            { Immunities.Physical, 100 },
            { Immunities.Fire, 100 },
            { Immunities.Ice, 100 },
            { Immunities.Poison, 90 },
            { Immunities.Lightning, 120 },
            { Immunities.Shadow, 110 },
            { Immunities.Holy, 130 },
        };

        private static Dictionary<StatusEffectType, int> removeStatusEffectValues = new Dictionary<
            StatusEffectType,
            int
        >
        {
            { StatusEffectType.Burn, 10 },
            { StatusEffectType.Freeze, 15 },
            { StatusEffectType.Poison, 5 },
            { StatusEffectType.Paralyze, 20 },
            { StatusEffectType.Stun, 30 },
            { StatusEffectType.Bleed, 0 },
            { StatusEffectType.Curse, 25 },
        };

        public static int CalculateEquipmentPrice(Equipment equipment)
        {
            int totalPrice = 0;

            totalPrice += equipment.attack * statValues["Attack"];
            totalPrice += equipment.defense * statValues["Defense"];
            totalPrice += equipment.magic * statValues["Magic"];
            totalPrice += equipment.health * statValues["Health"];
            totalPrice += equipment.stamina * statValues["Stamina"];
            totalPrice += equipment.intelligence * statValues["Intelligence"];
            totalPrice += equipment.dexterity * statValues["Dexterity"];
            totalPrice += equipment.speed * statValues["Speed"];
            totalPrice += equipment.critChance * statValues["CritChance"];

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

            totalPrice += (int)consumable.attackBoost * statValues["Attack"];
            totalPrice += (int)consumable.defenseBoost * statValues["Defense"];
            totalPrice += (int)consumable.magicBoost * statValues["Magic"];
            totalPrice += (int)consumable.healthBoost * statValues["Health"];
            totalPrice += (int)consumable.staminaBoost * statValues["Stamina"];
            totalPrice += (int)consumable.intelligenceBoost * statValues["Intelligence"];
            totalPrice += (int)consumable.dexterityBoost * statValues["Dexterity"];
            totalPrice += (int)consumable.speedBoost * statValues["Speed"];
            totalPrice += (int)consumable.critChanceBoost * statValues["CritChance"];
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
