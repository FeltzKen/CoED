using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class Spell
    {
        public string SpellID { get; private set; }
        public string SpellName { get; private set; }
        public Sprite Icon { get; private set; }
        public int MagicCost { get; private set; }
        public int Damage { get; private set; }
        public float Lifetime { get; private set; }
        public float CollisionRadius { get; private set; }
        public float Speed { get; private set; }
        public float Cooldown { get; private set; }
        public int LevelUpThreshold { get; private set; }
        public DamageType DamageType { get; private set; }
        public float CriticalChance { get; private set; }
        public float AreaOfEffect { get; private set; }
        public SpellType Type { get; private set; }
        public List<StatusEffectType> GainableStatusEffects { get; private set; }
        public GameObject SpellEffectPrefab { get; private set; }
        public bool SelfTargeting { get; private set; }
        public bool CanChase { get; private set; }
        public bool IsInstant { get; private set; }
        public List<CharacterClass> LearnableByClasses { get; private set; }
        public DamageInfo DamageInfo { get; private set; }

        public Spell(
            string spellID,
            string spellName,
            Sprite icon,
            int magicCost,
            int damage,
            float lifetime,
            float collisionRadius,
            float speed,
            float cooldown,
            int levelUpThreshold,
            DamageType damageType,
            float criticalChance,
            float areaOfEffect,
            SpellType type,
            List<StatusEffectType> gainableStatusEffects,
            GameObject spellEffectPrefab,
            bool selfTargeting,
            bool canChase,
            List<CharacterClass> learnableByClasses
        )
        {
            SpellID = spellID;
            SpellName = spellName;
            Icon = icon;
            MagicCost = magicCost;
            Lifetime = lifetime;
            CollisionRadius = collisionRadius;
            Speed = speed;
            Cooldown = cooldown;
            LevelUpThreshold = levelUpThreshold;
            CriticalChance = criticalChance;
            AreaOfEffect = areaOfEffect;
            Type = type;
            SpellEffectPrefab = spellEffectPrefab;
            SelfTargeting = selfTargeting;
            CanChase = canChase;
            LearnableByClasses = learnableByClasses;
            DamageInfo = new DamageInfo(
                new Dictionary<DamageType, float> { { damageType, damage } },
                gainableStatusEffects
            );
        }
    }
}
