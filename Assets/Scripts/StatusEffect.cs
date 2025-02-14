using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class StatusEffect
    {
        // Basic effect data.
        public StatusEffectType effectType;
        public string effectName;
        public bool hasDuration;
        public float duration; // Total remaining duration (seconds)
        public bool hasPeriodicTick;
        public float tickInterval; // How often to tick (seconds)
        public float tickDamageOrHeal; // Amount to apply per tick
        public Sprite effectSprite; // For UI display, etc.
        public GameObject effectIconPrefab; // For visual effects, etc.

        // Internal state (not in the definition).
        private float tickTimer;

        // Optional flags for modifications that must be undone later.
        public bool addedShield;
        public bool wasInvincible;

        /// <summary>
        /// Initializes the effect instance. Call this after cloning.
        /// </summary>
        /// <param name="customDuration">Custom duration to use (can override the base value).</param>
        public void Initialize(
            float newTickDamageOrHeal,
            GameObject effectIconPrefab,
            float tickInterval = 1f,
            bool isPeriodic = false,
            bool hasDuration = true,
            float customDuration = 0f
        )
        {
            tickDamageOrHeal = newTickDamageOrHeal;
            tickTimer = tickInterval;
            this.hasDuration = hasDuration;
            hasPeriodicTick = isPeriodic;
            if (customDuration > 0f)
                duration = customDuration;
            this.effectIconPrefab = effectIconPrefab;
        }

        /// <summary>
        /// Called each frame by the manager to update the effect.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        /// <param name="entityStats">The stats of the affected entity.</param>
        public void UpdateEffect(float deltaTime, IEntityStats entityStats)
        {
            if (hasDuration)
                duration -= deltaTime;

            if (hasPeriodicTick)
            {
                tickTimer -= deltaTime;
                if (tickTimer <= 0f)
                {
                    tickTimer = tickInterval;
                    ApplyPeriodicEffect(entityStats);
                }
            }
        }

        /// <summary>
        /// Immediately applies the effect to the entity (for effects that have an immediate impact).
        /// </summary>
        public void ApplyToEntity(
            IEntityStats entityStats,
            float effectValue,
            float effectDuration = 0f
        )
        {
            switch (effectType)
            {
                case StatusEffectType.Burn:
                    entityStats.TakeEffectDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float> { { DamageType.Fire, effectValue } },
                            null
                        )
                    );
                    break;
                case StatusEffectType.Poison:
                    entityStats.TakeEffectDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float>
                            {
                                { DamageType.Poison, effectValue },
                            },
                            null
                        )
                    );
                    break;
                case StatusEffectType.Slow:
                    entityStats.ModifyStat(Stat.Speed, effectValue);
                    break;
                case StatusEffectType.Stun:
                    entityStats.NullStat(Stat.Speed, effectDuration);
                    break;
                case StatusEffectType.Paralyze:
                case StatusEffectType.Freeze:
                    entityStats.NullStat(Stat.Speed, effectDuration);
                    break;
                case StatusEffectType.Curse:
                    entityStats.ModifyStat(Stat.Intelligence, effectValue);
                    entityStats.TakeDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float>
                            {
                                { DamageType.Shadow, effectValue },
                            },
                            null
                        )
                    );
                    break;
                case StatusEffectType.Blindness:
                    // will add method to block player's ability to cast spells
                    entityStats.ModifyStat(Stat.Dexterity, effectValue);
                    break;
                case StatusEffectType.Silence:
                    entityStats.IsSilenced(true);
                    break;
                case StatusEffectType.Fear:
                    break;
                case StatusEffectType.Sleep:
                    entityStats.NullStat(Stat.Speed, effectDuration);
                    entityStats.NullStat(Stat.Dexterity, effectDuration);
                    break;
                case StatusEffectType.Root:
                    entityStats.ModifyStat(Stat.Speed, effectValue);
                    break;
                // Buffs:
                case StatusEffectType.Shield:
                    entityStats.ModifyStat(Stat.Shield, effectValue);
                    addedShield = true;
                    break;
                case StatusEffectType.Invincible:
                    entityStats.SetInvincible(true);
                    wasInvincible = true;
                    break;
                // … add more cases as needed.
                default:
                    break;
            }
        }

        /// <summary>
        /// Applies the periodic tick effect (damage/heal) to the entity.
        /// </summary>
        public void ApplyPeriodicEffect(IEntityStats entityStats)
        {
            Debug.Log($"Applying periodic effect: {effectType} - {tickDamageOrHeal}");
            switch (effectType)
            {
                case StatusEffectType.Burn:
                    entityStats.TakeEffectDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float>
                            {
                                { DamageType.Fire, tickDamageOrHeal },
                            },
                            null
                        )
                    );
                    Debug.Log(
                        $"[Burn Tick] Applying {tickDamageOrHeal} fire damage to {entityStats}."
                    );

                    break;
                case StatusEffectType.Poison:
                    entityStats.TakeEffectDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float>
                            {
                                { DamageType.Poison, tickDamageOrHeal },
                            },
                            null
                        )
                    );
                    break;
                case StatusEffectType.Bleed:
                    entityStats.TakeEffectDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float>
                            {
                                { DamageType.Physical, tickDamageOrHeal },
                            },
                            null
                        )
                    );
                    break;
                case StatusEffectType.Regen:
                    entityStats.Heal(tickDamageOrHeal);
                    break;
                case StatusEffectType.StealHealth:
                    entityStats.Heal(tickDamageOrHeal);
                    break;
                // Add additional periodic behaviors as needed.
                default:
                    break;
            }
        }

        /// <summary>
        /// Cleans up any persistent modifications when the effect expires.
        /// </summary>
        public void Cleanup(IEntityStats entityStats)
        {
            if (wasInvincible)
                entityStats.SetInvincible(false);
            if (addedShield)
                entityStats.ModifyStat(Stat.Shield, -10);
            // Add additional cleanup as needed.
        }

        /// <summary>
        /// Creates a new copy of this StatusEffect instance.
        /// (This acts as a “constructor” that clones the definition.)
        /// </summary>
        public StatusEffect Clone()
        {
            return new StatusEffect
            {
                effectType = this.effectType,
                effectName = this.effectName,
                hasDuration = this.hasDuration,
                effectIconPrefab = this.effectIconPrefab,
                duration = this.duration, // base duration; this will be overridden by Initialize().
                hasPeriodicTick = this.hasPeriodicTick,
                tickInterval = this.tickInterval,
                tickDamageOrHeal = this.tickDamageOrHeal,
                effectSprite = this.effectSprite,
                // Internal state is not cloned.
                tickTimer = this.tickInterval,
                addedShield = false,
                wasInvincible = false,
            };
        }
    }
}
