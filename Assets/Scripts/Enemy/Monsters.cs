using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    [System.Serializable]
    public class Monster
    {
        // The full dictionary of stats. This is always initialized with every key.
        public Dictionary<Stat, float> monsterStats;

        public string name;
        public string description;
        public int level;
        public DamageType damageType;
        public float statusInflictionChance; // base chance
        public List<StatusEffectType> inflictedStatusEffect = new List<StatusEffectType>();
        public List<Immunities> immunities;
        public List<Resistances> resistances;
        public List<Weaknesses> weaknesses;
        public Sprite monsterSprite;

        /// <summary>
        /// Default constructor (required for serialization).
        /// </summary>
        public Monster()
        {
            InitializeDefaultStats();
            immunities = new List<Immunities>();
            resistances = new List<Resistances>();
            weaknesses = new List<Weaknesses>();
        }

        /// <summary>
        /// New constructor that initializes a monster with all properties.
        /// It builds a full stat dictionary with default values, then merges in the provided overrides.
        /// </summary>
        public Monster(
            string monsterName,
            string description,
            int level,
            DamageType damageType,
            float statusInflictionChance,
            List<StatusEffectType> inflictedStatusEffect,
            Dictionary<Stat, float> statOverrides,
            List<Immunities> immunities = null,
            List<Resistances> resistances = null,
            List<Weaknesses> weaknesses = null,
            Sprite monsterSprite = null
        )
        {
            InitializeDefaultStats();
            this.name = monsterName;
            this.description = description;
            this.level = level;
            this.damageType = damageType;
            this.statusInflictionChance = statusInflictionChance;
            this.inflictedStatusEffect = inflictedStatusEffect;

            // Merge in provided stat overrides.
            if (statOverrides != null)
            {
                foreach (var kvp in statOverrides)
                {
                    // Overwrite the default value for the given key.
                    this.monsterStats[kvp.Key] = kvp.Value;
                }
            }

            this.immunities =
                immunities != null ? new List<Immunities>(immunities) : new List<Immunities>();
            this.resistances =
                resistances != null ? new List<Resistances>(resistances) : new List<Resistances>();
            this.weaknesses =
                weaknesses != null ? new List<Weaknesses>(weaknesses) : new List<Weaknesses>();
            this.monsterSprite = monsterSprite;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Monster(Monster other)
        {
            InitializeDefaultStats(); // create a full dictionary
            this.name = other.name;
            this.description = other.description;
            this.level = other.level;
            this.damageType = other.damageType;
            this.statusInflictionChance = other.statusInflictionChance;
            this.inflictedStatusEffect = other.inflictedStatusEffect;
            // Create a deep copy of the full dictionary.
            this.monsterStats = new Dictionary<Stat, float>(other.monsterStats);
            this.immunities = new List<Immunities>(other.immunities);
            this.resistances = new List<Resistances>(other.resistances);
            this.weaknesses = new List<Weaknesses>(other.weaknesses);
            this.monsterSprite = other.monsterSprite;
        }

        /// <summary>
        /// Initializes the monsterStats dictionary with default values for all stats.
        /// </summary>
        private void InitializeDefaultStats()
        {
            monsterStats = new Dictionary<Stat, float>()
            {
                { Stat.HP, 0 },
                { Stat.MaxHP, 0 },
                { Stat.Attack, 0 },
                { Stat.Intelligence, 0 },
                { Stat.Evasion, 0 },
                { Stat.Defense, 0 },
                { Stat.Dexterity, 0 },
                { Stat.Accuracy, 0 },
                { Stat.Magic, 0 },
                { Stat.MaxMagic, 0 },
                { Stat.Stamina, 0 },
                { Stat.MaxStamina, 0 },
                { Stat.Shield, 0 },
                { Stat.FireRate, 0 },
                { Stat.CritChance, 0 },
                { Stat.CritDamage, 0 },
                { Stat.ProjectileRange, 0 },
                { Stat.AttackRange, 0 },
                { Stat.Speed, 0 },
                { Stat.ElementalDamage, 0 },
                { Stat.ChanceToInflictStatusEffect, 0 },
                { Stat.StatusEffectDuration, 0 },
                { Stat.PatrolSpeed, 0 },
                { Stat.ChaseSpeed, 0 },
            };
        }
    }
}
