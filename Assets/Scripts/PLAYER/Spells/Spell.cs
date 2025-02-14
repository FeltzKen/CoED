using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class Spell
    {
        public string SpellID { get; private set; }
        public string SpellName { get; private set; }
        public Sprite Icon { get; private set; }
        public int SpellLevel { get; private set; } = 1;
        public float StatusEffectDuration { get; private set; }
        public int MagicCost { get; private set; }
        public float Lifetime { get; private set; }
        public float CollisionRadius { get; private set; }
        public float Speed { get; private set; }
        public float Cooldown { get; private set; }
        public int LevelUpThreshold { get; private set; }
        public Dictionary<DamageType, float> DamageTypes { get; private set; } = new();
        public float CriticalChance { get; private set; }
        public float AreaOfEffect { get; private set; }
        public SpellType Type { get; private set; }
        public List<StatusEffectType> SpellStatusEffects { get; private set; }
        public GameObject SpellEffectPrefab { get; private set; }
        public bool SelfTargeting { get; private set; }
        public bool CanChase { get; private set; }
        public bool IsInstant { get; private set; }
        public List<CharacterClass> LearnableByClasses { get; private set; }
        public DamageInfo DamageInfo { get; private set; }

        public event System.Action<Spell> OnSpellLeveledUp;

        public Spell(
            string spellID,
            string spellName,
            Sprite icon,
            float statusEffectDuration,
            int magicCost,
            float lifetime,
            float collisionRadius,
            float speed,
            float cooldown,
            int levelUpThreshold,
            Dictionary<DamageType, float> damageTypes,
            float criticalChance,
            float areaOfEffect,
            SpellType type,
            List<StatusEffectType> spellDataStatusEffects,
            GameObject spellEffectPrefab,
            bool selfTargeting,
            bool canChase,
            List<CharacterClass> learnableByClasses
        )
        {
            SpellID = spellID;
            SpellName = spellName;
            Icon = icon;
            StatusEffectDuration = statusEffectDuration;
            MagicCost = magicCost;
            Lifetime = lifetime;
            CollisionRadius = collisionRadius;
            Speed = speed;
            Cooldown = cooldown;
            LevelUpThreshold = levelUpThreshold;
            DamageTypes = damageTypes;
            CriticalChance = criticalChance;
            AreaOfEffect = areaOfEffect;
            Type = type;
            SpellEffectPrefab = spellEffectPrefab;
            SelfTargeting = selfTargeting;
            CanChase = canChase;
            LearnableByClasses = learnableByClasses;
            SpellStatusEffects = spellDataStatusEffects;
        }

        public Spell Clone()
        {
            return new Spell(
                SpellID,
                SpellName,
                Icon,
                StatusEffectDuration,
                MagicCost,
                Lifetime,
                CollisionRadius,
                Speed,
                Cooldown,
                LevelUpThreshold,
                DamageTypes,
                CriticalChance,
                AreaOfEffect,
                Type,
                SpellStatusEffects,
                SpellEffectPrefab,
                SelfTargeting,
                CanChase,
                LearnableByClasses
            );
        }

        public void LevelUp()
        {
            SpellLevel++;
            // Optionally update internal stats based on the new level.
            // (For example, increase damage, reduce cooldown, etc.)

            // Raise the event so that the UI can display a modification panel.
            OnSpellLeveledUp?.Invoke(this);
            Debug.Log($"{SpellName} leveled up to level {SpellLevel}.");
        }
    }
}
