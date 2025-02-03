using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class StatusEffect : MonoBehaviour
    {
        [Header("General Effect Info")]
        public StatusEffectType effectType;
        public ActiveWhileEquipped equipmentEffects;
        public string effectName;
        public bool hasDuration = true;

        [Min(0.1f)]
        public float duration = 5f;

        [Header("Periodic Tick Settings")]
        [Tooltip("For over-time effects (DoT, HoT).")]
        public bool hasPeriodicTick = false;

        [Min(0.1f)]
        public float tickInterval = 1f;
        private float tickTimer;

        public float tickDamageOrHeal = 5f;

        private bool addedShield = false;
        private bool wasInvincible = false;

        public void ApplyToEntity(IEntityStats entityStats, float effectValue)
        {
            switch (effectType)
            {
                // Debuffs
                case StatusEffectType.Burn:
                case StatusEffectType.Poison:
                case StatusEffectType.Bleed:
                    hasPeriodicTick = true;
                    break;

                case StatusEffectType.Slow:
                    entityStats.ModifyStat(Stat.Speed, effectValue);
                    break;

                case StatusEffectType.Stun:
                case StatusEffectType.Paralyze:
                case StatusEffectType.Freeze:
                    entityStats.ModifyStat(Stat.Speed, effectValue);
                    break;

                case StatusEffectType.Curse:
                    entityStats.ModifyStat(Stat.Intelligence, effectValue);
                    break;

                case StatusEffectType.Blindness:
                    entityStats.ModifyStat(Stat.Accuracy, effectValue);
                    break;

                case StatusEffectType.Silence:
                    // block entity from casting spells
                    entityStats.IsSilenced(true);
                    break;

                case StatusEffectType.Fear:
                    entityStats.ModifyStat(Stat.Evasion, effectValue);
                    break;

                case StatusEffectType.Sleep:
                    entityStats.NullStat(Stat.Speed, effectValue);
                    entityStats.NullStat(Stat.Evasion, effectValue);
                    entityStats.NullStat(Stat.Dexterity, effectValue);
                    break;

                case StatusEffectType.Root:
                    entityStats.ModifyStat(Stat.Speed, effectValue);
                    break;

                // Buffs
                case StatusEffectType.Regen:
                    hasPeriodicTick = true;
                    break;

                case StatusEffectType.StealHealth:
                    hasPeriodicTick = true;
                    break;

                case StatusEffectType.Shield:
                    entityStats.ModifyStat(Stat.Shield, effectValue);
                    addedShield = true;
                    break;

                case StatusEffectType.Invincible:
                    entityStats.SetInvincible(true);
                    wasInvincible = true;
                    break;

                case StatusEffectType.DamageAbsorb:
                    entityStats.AddActiveStatusEffect(StatusEffectType.DamageAbsorb);
                    break;

                case StatusEffectType.DamageReduction:
                    entityStats.AddActiveStatusEffect(StatusEffectType.DamageReduction);
                    break;

                case StatusEffectType.DamageIncrease:
                    entityStats.AddActiveStatusEffect(StatusEffectType.DamageIncrease);
                    break;

                case StatusEffectType.AttackSpeedIncrease:
                    entityStats.ModifyStat(Stat.FireRate, effectValue);
                    break;

                case StatusEffectType.MovementSpeedIncrease:
                    entityStats.ModifyStat(Stat.Speed, effectValue);
                    break;

                case StatusEffectType.EvasionIncrease:
                    entityStats.ModifyStat(Stat.Evasion, effectValue);
                    break;

                case StatusEffectType.AccuracyIncrease:
                    entityStats.ModifyStat(Stat.Accuracy, effectValue);
                    break;

                case StatusEffectType.DamageReflect:
                    entityStats.AddActiveStatusEffect(StatusEffectType.DamageReflect);
                    break;
            }
        }

        private void Update()
        {
            if (!hasPeriodicTick)
                return;

            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0f)
            {
                tickTimer = tickInterval;
                ApplyPeriodicEffect();
            }
        }

        private void ApplyPeriodicEffect()
        {
            var entityStats = GetComponentInParent<IEntityStats>();
            if (entityStats == null)
                return;

            switch (effectType)
            {
                case StatusEffectType.Burn:
                    entityStats.TakeDamage(
                        new DamageInfo(
                            new Dictionary<DamageType, float>
                            {
                                { DamageType.Fire, tickDamageOrHeal },
                            },
                            null
                        )
                    );
                    break;

                case StatusEffectType.Poison:
                    entityStats.TakeDamage(
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
                    entityStats.TakeDamage(
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
            }
        }

        private void OnDestroy()
        {
            var entityStats = GetComponentInParent<IEntityStats>();
            if (entityStats == null)
                return;

            if (wasInvincible)
            {
                entityStats.SetInvincible(false);
            }
            if (addedShield)
            {
                entityStats.ModifyStat(Stat.Shield, -10);
            }
        }
    }
}
