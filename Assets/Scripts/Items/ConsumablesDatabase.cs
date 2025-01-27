using System.Collections.Generic;
using CoED;
using Mono.Cecil;
using UnityEngine;

namespace CoED
{
    public static class ConsumablesDatabase
    {
        public static List<ConsumableItem> consumables = new List<ConsumableItem>()
        {
            new ConsumableItem
            {
                name = "Health Potion",
                description = "Restores health",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/HealthPotion"),
                healthBoost = 50,
            },
            new ConsumableItem
            {
                name = "Mana Potion",
                description = "Restores mana",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/ManaPotion"),
                magicBoost = 50,
            },
            new ConsumableItem
            {
                name = "Stamina Potion",
                description = "Restores stamina",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/StaminaPotion"),
                staminaBoost = 50,
            },
            new ConsumableItem
            {
                name = "Strength Potion",
                description = "Increases strength temporarily",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/StrengthPotion"),
                attackBoost = 20,
                hasDuration = true,
                duration = 20,
            },
            new ConsumableItem
            {
                name = "Dexterity Potion",
                description = "Increases dexterity temporarily",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/DexterityPotion"),
                dexterityBoost = 20,
                hasDuration = true,
                duration = 20,
            },
            new ConsumableItem
            {
                name = "Intelligence Potion",
                description = "Increases intelligence temporarily",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/IntelligencePotion"),
                intelligenceBoost = 20,
            },
            new ConsumableItem
            {
                name = "Speed Potion",
                description = "Increases speed temporarily",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/SpeedPotion"),
                speedBoost = 20,
                hasDuration = true,
                duration = 20,
            },
            new ConsumableItem
            {
                name = "Critical Hit Potion",
                description = "Increases critical hit chance temporarily",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/CriticalHitPotion"),
                critChanceBoost = 20,
                hasDuration = true,
                duration = 60,
            },
            new ConsumableItem
            {
                name = "Defense Potion",
                description = "Increases defense temporarily",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/DefensePotion"),
                defenseBoost = 20,
                hasDuration = true,
                duration = 90,
            },
            new ConsumableItem
            {
                name = "Health Regeneration Potion",
                description = "Regenerates health over time",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/HealthRegenerationPotion"),
                addedEffects = new List<StatusEffectType> { StatusEffectType.Regen },
                amountPerInterval = 20,
                hasDuration = true,
                duration = 120,
            },
            new ConsumableItem
            {
                name = "Remove Poison Potion",
                description = "Removes poison status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/RemovePoison"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Poison },
                canHaveAffixes = false,
            },
            new ConsumableItem
            {
                name = "Remove Curse Potion",
                description = "Removes curse status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveCurse"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Curse },
                canHaveAffixes = false,
            },
            new ConsumableItem
            {
                name = "Remove Silence Potion",
                description = "Removes silence status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveSilence"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Silence },
                canHaveAffixes = false,
            },
            new ConsumableItem
            {
                name = "Remove Slow Potion",
                description = "Removes slow status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveSlow"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Slow },
                canHaveAffixes = false,
            },
            new ConsumableItem
            {
                name = "Remove Freeze Potion",
                description = "Removes freeze status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveFreeze"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Freeze },
                canHaveAffixes = false,
            },
            new ConsumableItem
            {
                name = "Remove Burn Potion",
                description = "Removes burn status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveBurn"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Burn },
                canHaveAffixes = false,
            },
            new ConsumableItem
            {
                name = "Heal Bleeding Wounds",
                description = "Removes bleed status effect",
                icon = Resources.Load<Sprite>("Sprites/Items/Consumables/HealBleedingWounds"),
                removedEffects = new List<StatusEffectType> { StatusEffectType.Bleed },
                canHaveAffixes = false,
            },
        };
    }
}
