using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class StatusEffectLibrary
    {
        public static List<StatusEffect> statusEffects = new List<StatusEffect>
        {
            new StatusEffect
            {
                effectType = StatusEffectType.Burn,
                effectName = "Burn",
                hasDuration = true,
                hasPeriodicTick = true,
                tickInterval = 7f,
                tickDamageOrHeal = 20f,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Burn"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Slow,
                effectName = "Slow",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Slow"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Stun,
                effectName = "Stun",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Stun"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            // ... Add other effects as needed.
            new StatusEffect
            {
                effectType = StatusEffectType.Freeze,
                effectName = "Freeze",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Freeze"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Paralyze,
                effectName = "Paralyze",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Paralyze"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Poison,
                effectName = "Poison",
                hasDuration = true,
                hasPeriodicTick = true,
                tickInterval = 1f,
                tickDamageOrHeal = 5f,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Poison"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.PoisonAura,
                effectName = "Poison Aura",
                hasDuration = true,
                hasPeriodicTick = true,
                tickInterval = 1f,
                tickDamageOrHeal = 5f,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/PoisonAura"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Invincible,
                effectName = "Invincible",
                hasDuration = true,
                duration = 5f,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Invincible"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Bleed,
                effectName = "Bleed",
                hasDuration = true,
                hasPeriodicTick = true,
                tickInterval = 1f,
                tickDamageOrHeal = 5f,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Bleed"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Curse,
                effectName = "Curse",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Curse"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Blindness,
                effectName = "Blindness",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Blindness"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Silence,
                effectName = "Silence",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Silence"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Fear,
                effectName = "Fear",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Fear"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Confusion,
                effectName = "Confusion",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Confusion"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Sleep,
                effectName = "Sleep",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Sleep"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Petrify,
                effectName = "Petrify",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Petrify"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Root,
                effectName = "Root",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Root"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Berserk,
                effectName = "Berserk",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Berserk"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            // buffs
            new StatusEffect
            {
                effectType = StatusEffectType.Regen,
                effectName = "Regen",
                hasDuration = true,
                hasPeriodicTick = true,
                tickInterval = 1f,
                tickDamageOrHeal = 5f,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Regen"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.StealHealth,
                effectName = "Steal Health",
                hasDuration = true,
                hasPeriodicTick = true,
                tickInterval = 1f,
                tickDamageOrHeal = 5f,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/StealHealth"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Charm,
                effectName = "Charm",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Charm"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.Shield,
                effectName = "Shield",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/Shield"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.DamageReflect,
                effectName = "Damage Reflect",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/DamageReflect"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.DamageAbsorb,
                effectName = "Damage Absorb",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/DamageAbsorb"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.DamageReduction,
                effectName = "Damage Reduction",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/DamageReduction"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.DamageIncrease,
                effectName = "Damage Increase",
                hasDuration = true,
                duration = 5f,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/DamageIncrease"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.AttackSpeedIncrease,
                effectName = "Attack Speed Increase",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/AttackSpeedIncrease"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.MovementSpeedIncrease,
                effectName = "Movement Speed Increase",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/MoveSpeedIncrease"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.EvasionIncrease,
                effectName = "Evasion Increase",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/EvasionIncrease"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.DefenseIncrease,
                effectName = "Defense Increase",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/DefenseIncrease"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
            new StatusEffect
            {
                effectType = StatusEffectType.AccuracyIncrease,
                effectName = "Accuracy Increase",
                hasDuration = true,
                hasPeriodicTick = false,
                effectSprite = Resources.Load<Sprite>("Sprites/StatusEffects/AccuracyIncrease"),
                effectIconPrefab = Resources.Load<GameObject>(
                    "Prefabs/EnemyStatusEffectIconPrefab"
                ),
            },
        };

        /// <summary>
        /// Retrieves a status effect definition for the given type.
        /// </summary>
        public static StatusEffect GetDefinition(StatusEffectType effectType)
        {
            foreach (StatusEffect effect in statusEffects)
            {
                if (effect.effectType == effectType)
                    return effect;
            }
            return null;
        }
    }
}
