using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class ConsumablesDatabase
    {
        public static List<ConsumableItem> consumables = new List<ConsumableItem>()
        {
            // Health Potion: restores 50 health (assumed to boost HP by 50)
            new ConsumableItem(
                itemID: "healthPotion001",
                itemName: "Health Potion",
                description: "Restores health",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/HealthPotion"),
                hasDuration: false,
                duration: 0f,
                statOverrides: new Dictionary<Stat, float> { { Stat.HP, 50f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Mana Potion: restores 50 mana (boost to MaxMagic)
            new ConsumableItem(
                itemID: "manaPotion001",
                itemName: "Mana Potion",
                description: "Restores mana",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/ManaPotion"),
                hasDuration: false,
                duration: 0f,
                statOverrides: new Dictionary<Stat, float> { { Stat.MaxMagic, 50f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Stamina Potion: restores 50 stamina (boost to MaxStamina)
            new ConsumableItem(
                itemID: "staminaPotion001",
                itemName: "Stamina Potion",
                description: "Restores stamina",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/StaminaPotion"),
                hasDuration: false,
                duration: 0f,
                statOverrides: new Dictionary<Stat, float> { { Stat.MaxStamina, 50f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Strength Potion: temporarily increases attack by 20 for 20 seconds.
            new ConsumableItem(
                itemID: "strengthPotion001",
                itemName: "Strength Potion",
                description: "Increases strength temporarily",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/StrengthPotion"),
                hasDuration: true,
                duration: 20f,
                statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 20f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Dexterity Potion: temporarily increases dexterity by 20 for 20 seconds.
            new ConsumableItem(
                itemID: "dexterityPotion001",
                itemName: "Dexterity Potion",
                description: "Increases dexterity temporarily",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/DexterityPotion"),
                hasDuration: true,
                duration: 20f,
                statOverrides: new Dictionary<Stat, float> { { Stat.Dexterity, 20f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Intelligence Potion: temporarily increases intelligence by 20 for 20 seconds.
            new ConsumableItem(
                itemID: "intelligencePotion001",
                itemName: "Intelligence Potion",
                description: "Increases intelligence temporarily",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/IntelligencePotion"),
                hasDuration: true,
                duration: 20f,
                statOverrides: new Dictionary<Stat, float> { { Stat.Intelligence, 20f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Speed Potion: temporarily increases speed by 20 for 20 seconds.
            new ConsumableItem(
                itemID: "speedPotion001",
                itemName: "Speed Potion",
                description: "Increases speed temporarily",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/SpeedPotion"),
                hasDuration: true,
                duration: 20f,
                statOverrides: new Dictionary<Stat, float> { { Stat.Speed, 20f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Critical Hit Potion: temporarily increases critical hit chance by 0.2 for 60 seconds.
            new ConsumableItem(
                itemID: "critHitPotion001",
                itemName: "Critical Hit Potion",
                description: "Increases critical hit chance temporarily",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/CriticalHitPotion"),
                hasDuration: true,
                duration: 60f,
                statOverrides: new Dictionary<Stat, float> { { Stat.CritChance, 0.2f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Defense Potion: temporarily increases defense by 20 for 90 seconds.
            new ConsumableItem(
                itemID: "defensePotion001",
                itemName: "Defense Potion",
                description: "Increases defense temporarily",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/DefensePotion"),
                hasDuration: true,
                duration: 90f,
                statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 20f } },
                addedEffects: null,
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 0
            ),
            // Health Regeneration Potion: regenerates health over time (applies a Regen status effect) for 120 seconds.
            new ConsumableItem(
                itemID: "healthRegenPotion001",
                itemName: "Health Regeneration Potion",
                description: "Regenerates health over time",
                baseSprite: Resources.Load<Sprite>(
                    "Sprites/Items/Consumables/HealthRegenerationPotion"
                ),
                hasDuration: true,
                duration: 120f,
                statOverrides: null,
                addedEffects: new List<StatusEffectType> { StatusEffectType.Regen },
                removedEffects: null,
                canHaveAffixes: true,
                amountPerInterval: 20
            ),
            // Remove Poison Potion: instantly removes Poison status effect.
            new ConsumableItem(
                itemID: "removePoisonPotion001",
                itemName: "Remove Poison Potion",
                description: "Removes poison status effect",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/RemovePoison"),
                hasDuration: false,
                duration: 0f,
                statOverrides: null,
                addedEffects: null,
                removedEffects: new List<StatusEffectType> { StatusEffectType.Poison },
                canHaveAffixes: false,
                amountPerInterval: 0
            ),
            // Remove Curse Potion: instantly removes Curse status effect.
            new ConsumableItem(
                itemID: "removeCursePotion001",
                itemName: "Remove Curse Potion",
                description: "Removes curse status effect",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveCurse"),
                hasDuration: false,
                duration: 0f,
                statOverrides: null,
                addedEffects: null,
                removedEffects: new List<StatusEffectType> { StatusEffectType.Curse },
                canHaveAffixes: false,
                amountPerInterval: 0
            ),
            // Remove Silence Potion: instantly removes Silence status effect.
            new ConsumableItem(
                itemID: "removeSilencePotion001",
                itemName: "Remove Silence Potion",
                description: "Removes silence status effect",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveSilence"),
                hasDuration: false,
                duration: 0f,
                statOverrides: null,
                addedEffects: null,
                removedEffects: new List<StatusEffectType> { StatusEffectType.Silence },
                canHaveAffixes: false,
                amountPerInterval: 0
            ),
            // Remove Slow Potion: instantly removes Slow status effect.
            new ConsumableItem(
                itemID: "removeSlowPotion001",
                itemName: "Remove Slow Potion",
                description: "Removes slow status effect",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveSlow"),
                hasDuration: false,
                duration: 0f,
                statOverrides: null,
                addedEffects: null,
                removedEffects: new List<StatusEffectType> { StatusEffectType.Slow },
                canHaveAffixes: false,
                amountPerInterval: 0
            ),
            // Remove Burn Potion: instantly removes Burn status effect.
            new ConsumableItem(
                itemID: "removeBurnPotion001",
                itemName: "Remove Burn Potion",
                description: "Removes burn status effect",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/RemoveBurn"),
                hasDuration: false,
                duration: 0f,
                statOverrides: null,
                addedEffects: null,
                removedEffects: new List<StatusEffectType> { StatusEffectType.Burn },
                canHaveAffixes: false,
                amountPerInterval: 0
            ),
            // Heal Bleeding Wounds: instantly removes Bleed status effect.
            new ConsumableItem(
                itemID: "healBleedPotion001",
                itemName: "Heal Bleeding Wounds",
                description: "Removes bleed status effect",
                baseSprite: Resources.Load<Sprite>("Sprites/Items/Consumables/HealBleedingWounds"),
                hasDuration: false,
                duration: 0f,
                statOverrides: null,
                addedEffects: null,
                removedEffects: new List<StatusEffectType> { StatusEffectType.Bleed },
                canHaveAffixes: false,
                amountPerInterval: 0
            ),
        };
    }
}
