using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class PlayerSpellWrapper : MonoBehaviour
    {
        public PlayerSpell BaseSpell { get; private set; }
        public Sprite Icon => BaseSpell.icon;
        public string SpellName => BaseSpell.spellName;
        public int MagicCost { get; private set; }
        public float Lifetime { get; private set; }
        public float CollisionRadius { get; private set; }
        public float Speed { get; private set; }
        public float Cooldown { get; private set; }
        public int SpellLevel { get; private set; }
        private float boostPerLevel = 10f;
        public float CriticalChance { get; private set; }
        public GameObject SpellEffectPrefab => BaseSpell.spellEffectPrefab;
        public SpellType Type => BaseSpell.type;
        public bool IsInstant => BaseSpell.isInstant;
        public bool CanChase => BaseSpell.canChase;
        public bool SelfTargeting => BaseSpell.selfTargeting;
        public int LevelUpThreshold => BaseSpell.levelUpThreshold;
        public float AreaOfEffect;

        [Header("Dynamic Damage and Status Effects")]
        public Dictionary<DamageType, float> DamageTypes { get; private set; } = new();
        public List<StatusEffectType> InflictedStatusEffectTypes { get; private set; } = new();
        private List<StatusEffectType> GainableStatusEffects;
        private Dictionary<int, Action<PlayerSpellWrapper>> levelUpgrades;

        public void Initialize(PlayerSpell baseSpell)
        {
            BaseSpell = baseSpell;
            SpellLevel = 1;
            CriticalChance = baseSpell.criticalChance;
            DamageTypes[baseSpell.damageType] = baseSpell.damage;
            GainableStatusEffects = new List<StatusEffectType>(baseSpell.gainableStatusEffects);
            MagicCost = baseSpell.magicCost;
            Lifetime = baseSpell.lifetime;
            CollisionRadius = baseSpell.collisionRadius;
            Speed = baseSpell.speed;
            Cooldown = baseSpell.cooldown;
            AreaOfEffect = baseSpell.areaOfEffect;

            InitializeUpgrades();
        }

        private void InitializeUpgrades()
        {
            levelUpgrades = new Dictionary<int, Action<PlayerSpellWrapper>>
            {
                {
                    2,
                    wrapper =>
                    {
                        wrapper.IncreaseDamage(boostPerLevel);
                        if (GainableStatusEffects.Count > 0)
                            wrapper.InflictedStatusEffectTypes.Add(GainableStatusEffects[0]);
                        wrapper.IncreaseLifetime(0.2f);
                    }
                },
                {
                    3,
                    wrapper =>
                    {
                        wrapper.IncreaseCritChance(0.02f);
                        wrapper.IncreaseAoE(1.5f);
                        wrapper.DecreaseCooldown(0.5f);
                    }
                },
                {
                    4,
                    wrapper =>
                    {
                        wrapper.IncreaseLifetime(0.2f);
                        wrapper.IncreaseCritChance(0.02f);
                        wrapper.IncreaseDamage(boostPerLevel);
                    }
                },
                {
                    5,
                    wrapper =>
                    {
                        wrapper.IncreaseAoE(1.5f);
                        wrapper.DecreaseMagicCost(2);
                        wrapper.IncreaseSpeed(0.5f);
                    }
                },
                {
                    6,
                    wrapper =>
                    {
                        if (GainableStatusEffects.Count > 1)
                            wrapper.AddStatusEffect(GainableStatusEffects[1]);
                        wrapper.DecreaseCooldown(0.5f);
                    }
                },
                {
                    7,
                    wrapper =>
                    {
                        wrapper.IncreaseDamage(boostPerLevel);
                        wrapper.DecreaseMagicCost(2);
                    }
                },
                {
                    8,
                    wrapper =>
                    {
                        wrapper.IncreaseAoE(1.5f);
                        wrapper.IncreaseSpeed(0.5f);
                    }
                },
                {
                    9,
                    wrapper =>
                    {
                        wrapper.IncreaseCritChance(0.02f);
                        wrapper.DecreaseMagicCost(2);
                    }
                },
                {
                    10,
                    wrapper =>
                    {
                        if (GainableStatusEffects.Count > 2)
                            wrapper.AddStatusEffect(GainableStatusEffects[2]);
                        wrapper.IncreaseDamage(boostPerLevel);
                    }
                },
                {
                    11,
                    wrapper =>
                    {
                        wrapper.IncreaseAoE(1.5f);
                        wrapper.IncreaseSpeed(0.5f);
                    }
                },
            };
        }

        public void LevelUp()
        {
            SpellLevel++;

            // Apply upgrades for the new level
            if (levelUpgrades.TryGetValue(SpellLevel, out var upgrade))
            {
                upgrade(this);
                Debug.Log($"{SpellName} received a level {SpellLevel} upgrade!");
            }
            else
            {
                Debug.Log($"{SpellName} leveled up to {SpellLevel}, but no upgrades available.");
            }
        }

        public void IncreaseDamage(float amount)
        {
            foreach (var key in DamageTypes.Keys)
            {
                DamageTypes[key] += amount;
            }
            Debug.Log($"{SpellName} damage increased by {amount}.");
        }

        public void IncreaseAoE(float amount)
        {
            AreaOfEffect *= amount;
            Debug.Log($"{SpellName} AoE increased by {amount}.");
        }

        public void AddStatusEffect(StatusEffectType effect)
        {
            if (!InflictedStatusEffectTypes.Contains(effect))
            {
                InflictedStatusEffectTypes.Add(effect);
                Debug.Log($"{SpellName} gained status effect: {effect}.");
            }
        }

        public void IncreaseSpeed(float amount)
        {
            Speed += amount;
            Debug.Log($"{SpellName} speed increased by {amount}.");
        }

        public void DecreaseCooldown(float amount)
        {
            Cooldown -= amount;
            Debug.Log($"{SpellName} cooldown decreased by {amount}.");
        }

        public void DecreaseMagicCost(int amount)
        {
            MagicCost -= amount;
            Debug.Log($"{SpellName} magic cost decreased by {amount}.");
        }

        public void IncreaseLifetime(float amount)
        {
            Lifetime += amount;
            Debug.Log($"{SpellName} lifetime increased by {amount}.");
        }

        public void IncreaseCritChance(float amount)
        {
            CriticalChance = Mathf.Min(CriticalChance + amount, 0.3f); // Cap at 30%
            Debug.Log($"{SpellName} critical chance increased by {amount * 100}%.");
        }
    }
}
