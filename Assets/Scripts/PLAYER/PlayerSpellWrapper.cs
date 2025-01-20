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
        public GameObject SpellEffectPrefabs => BaseSpell.spellEffectPrefab;
        public SpellType Type => BaseSpell.type;
        public bool IsInstant => BaseSpell.isInstant;
        public bool CanChase => BaseSpell.canChase;
        public bool SelfTargeting => BaseSpell.selfTargeting;
        public int LevelUpThreshold => BaseSpell.levelUpThreshold;
        public float AreaOfEffect => BaseSpell.areaOfEffect;

        [Header("Dynamic Damage and Status Effects")]
        public DamageType damageType;
        public Dictionary<DamageType, float> DamageTypes { get; private set; } = new();
        public List<StatusEffectType> InflictedStatusEffectTypes { get; private set; } = new();
        public List<StatusEffect> InflictedStatusEffects { get; private set; } = new();

        public void Initialize(PlayerSpell baseSpell)
        {
            BaseSpell = baseSpell;
            MagicCost = baseSpell.magicCost;
            Lifetime = baseSpell.lifetime;
            CollisionRadius = baseSpell.collisionRadius;
            Speed = baseSpell.speed;
            Cooldown = baseSpell.cooldown;
            SpellLevel = 1;

            // Initialize damage types
            DamageTypes[damageType] = baseSpell.damage;

            // Initialize status effects
            InflictedStatusEffectTypes = new List<StatusEffectType>(
                baseSpell.inflictedStatusEffectTypes
            );
        }

        public void LevelUp()
        {
            SpellLevel++;
            foreach (var key in DamageTypes.Keys)
            {
                DamageTypes[key] += 5; // Increase all damage types by 5
            }
            Cooldown *= 0.9f;
            MagicCost += 1;

            Debug.Log($"{SpellName} leveled up to Level {SpellLevel}!");
        }

        public void AddDamageType(DamageType type, float amount)
        {
            if (DamageTypes.ContainsKey(type))
                DamageTypes[type] += amount;
            else
                DamageTypes.Add(type, amount);
        }

        public void AddStatusEffect(StatusEffectType effect)
        {
            if (!InflictedStatusEffectTypes.Contains(effect))
                InflictedStatusEffectTypes.Add(effect);
        }
    }
}
