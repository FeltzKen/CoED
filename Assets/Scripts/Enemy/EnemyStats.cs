using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace CoED
{
    public class _EnemyStats : MonoBehaviour
    {
        [Header("Assigned Monster Data")]
        public Monster monsterData; // from the new system

        private DamageType elementalBase = DamageType.Physical;

        private bool invincible = false;
        public bool Invincible => invincible;

        [Header("Resistances/Weaknesses/Immunities")]
        public List<Immunities> immunities = new List<Immunities>();
        public List<Resistances> resistances = new List<Resistances>();
        public List<Weaknesses> weaknesses = new List<Weaknesses>();

        [Header("Dynamic Damage Types")]
        public Dictionary<DamageType, float> dynamicDamageTypes =
            new Dictionary<DamageType, float>();

        [Header("Inflicted Status Effects")]
        public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();
        public float chanceToInflictStatusEffect = 0.1f;

        [SerializeField]
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public Sprite sprite;

        // Current Stats
        private Dictionary<Stat, float> enemyStats = new Dictionary<Stat, float>()
        {
            { Stat.HP, 0f },
            { Stat.MaxHP, 500f },
            { Stat.Attack, 10f },
            { Stat.Intelligence, 5f },
            { Stat.Evasion, 0f },
            { Stat.Defense, 5f },
            { Stat.Dexterity, 5f },
            { Stat.Magic, 0f },
            { Stat.MaxMagic, 100f },
            { Stat.Accuracy, 5f },
            { Stat.FireRate, 1f },
            { Stat.ProjectileRange, 5f },
            { Stat.AttackRange, 3f },
            { Stat.Speed, 5f },
            { Stat.Shield, 0f },
            { Stat.Stamina, 0f },
            { Stat.MaxStamina, 100f },
            { Stat.ElementalDamage, 5f },
            { Stat.CritChance, 5f },
            { Stat.CritDamage, 1.5f },
            { Stat.ChanceToInflictStatusEffect, 0.05f },
            { Stat.StatusEffectDuration, 5f },
            { Stat.PatrolSpeed, 1f },
            { Stat.ChaseSpeed, 1f },
        };

        [SerializeField]
        public float GetEnemyHP() => enemyStats[Stat.HP];

        [SerializeField]
        public float GetEnemyMaxHP() => enemyStats[Stat.MaxHP];

        public float GetEnemyAttack() => enemyStats[Stat.Attack];

        public float GetEnemyIntelligence() => enemyStats[Stat.Intelligence];

        public float GetEnemyEvasion() => enemyStats[Stat.Evasion];

        public float GetEnemyDefense() => enemyStats[Stat.Defense];

        public float GetEnemyDexterity() => enemyStats[Stat.Dexterity];

        public float GetEnemyMagic() => enemyStats[Stat.Magic];

        public float GetEnemyMaxMagic() => enemyStats[Stat.MaxMagic];

        public float GetEnemyAccuracy() => enemyStats[Stat.Accuracy];

        public float GetEnemyFireRate() => enemyStats[Stat.FireRate];

        public float GetEnemyProjectileRange() => enemyStats[Stat.ProjectileRange];

        public float GetEnemyAttackRange() => enemyStats[Stat.AttackRange];

        public float GetEnemySpeed() => enemyStats[Stat.Speed];

        public float GetEnemyShield() => enemyStats[Stat.Shield];

        public float GetEnemyElementalDamage() => enemyStats[Stat.ElementalDamage];

        public float GetEnemyCritChance() => enemyStats[Stat.CritChance];

        public float GetEnemyCritDamage() => enemyStats[Stat.CritDamage];

        public float GetEnemyChanceToInflictStatusEffect() =>
            enemyStats[Stat.ChanceToInflictStatusEffect];

        public float GetEnemyStatusEffectDuration() => enemyStats[Stat.StatusEffectDuration];

        public float GetEnemyPatrolSpeed() => enemyStats[Stat.PatrolSpeed];

        public float GetEnemyChaseSpeed() => enemyStats[Stat.ChaseSpeed];

        [SerializeField]
        private int spawnFloor;
        public int spawnFloorLevel
        {
            get => spawnFloor;
            set => spawnFloor = value;
        }

        // If you want an "equipment tier" from floor
        public int EquipmentTier { get; private set; } = 1;

        // Factor for scaling stats
        public float ScaledFactor { get; private set; }

        private EnemyUI enemyUI;
        private _Enemy enemy;

        private void Start()
        {
            enemy = GetComponent<_Enemy>();
            enemyUI = GetComponentInChildren<EnemyUI>();

            ApplyMonsterData(monsterData);

            // 3) floor-based logic:
            if (spawnFloor <= 0)
                spawnFloor = 1;
            EquipmentTier = Mathf.Clamp(Mathf.FloorToInt(spawnFloor / 3) + 1, 1, 3);

            // 4) calculate final stats (the hybrid approach: monster base + floor scaling)
            CalculateMonsterScaledStats();
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (enemyUI != null)
            {
                enemyUI.SetHealthBarMax(enemyStats[Stat.MaxHP]);
            }
        }

        /// <summary>
        /// Copies the monster's base stats into local fields.
        /// </summary>
        private void ApplyMonsterData(Monster monster)
        {
            if (monster == null)
            {
                Debug.LogWarning("No monsterData assigned; using fallback stats.");
                return;
            }
            sprite = monster.monsterSprite;

            elementalBase = monster.damageType;
            enemyStats[Stat.MaxHP] = monster.monsterStats[Stat.MaxHP];
            enemyStats[Stat.Attack] = monster.monsterStats[Stat.Attack];
            enemyStats[Stat.Defense] = monster.monsterStats[Stat.Defense];
            enemyStats[Stat.Dexterity] = monster.monsterStats[Stat.Dexterity];
            enemyStats[Stat.ElementalDamage] = monster.monsterStats[Stat.ElementalDamage];
            enemyStats[Stat.AttackRange] = monster.monsterStats[Stat.AttackRange];
            enemyStats[Stat.ProjectileRange] = monster.monsterStats[Stat.ProjectileRange];
            enemyStats[Stat.ChanceToInflictStatusEffect] = monster.statusInflictionChance;
            enemyStats[Stat.Speed] = monster.monsterStats[Stat.Speed];
            enemyStats[Stat.Intelligence] = monster.monsterStats[Stat.Intelligence];
            enemyStats[Stat.Evasion] = monster.monsterStats[Stat.Evasion];
            enemyStats[Stat.CritChance] = monster.monsterStats[Stat.CritChance];
            enemyStats[Stat.CritDamage] = monster.monsterStats[Stat.CritDamage];
            enemyStats[Stat.FireRate] = monster.monsterStats[Stat.FireRate];
            enemyStats[Stat.Shield] = monster.monsterStats[Stat.Shield];
            enemyStats[Stat.Accuracy] = monster.monsterStats[Stat.Accuracy];
            enemyStats[Stat.StatusEffectDuration] = monster.monsterStats[Stat.StatusEffectDuration];
            enemyStats[Stat.PatrolSpeed] = monster.monsterStats[Stat.PatrolSpeed];

            inflictedStatusEffects.Clear();
            if (monster.inflictedStatusEffect != StatusEffectType.None)
            {
                inflictedStatusEffects.Add(monster.inflictedStatusEffect);
            }
            if (monster.resistance != Resistances.None)
            {
                resistances.Add(monster.resistance);
            }
            {
                resistances.Add(monster.resistance);
            }
            if (monster.weakness != Weaknesses.None)
            {
                weaknesses.Add(monster.weakness);
            }
        }

        /// <summary>
        /// Applies floor-based scaling so that monsters can appear on deeper floors
        /// while still retaining their base identity from monsterData.
        /// </summary>
        private void CalculateMonsterScaledStats()
        {
            var dungeonSettings = FindAnyObjectByType<DungeonGenerator>().dungeonSettings;

            float floorMultiplier =
                1
                + (spawnFloor * dungeonSettings.difficultyLevel * 0.1f)
                + dungeonSettings.playerLevelFactor
                + dungeonSettings.floorDifficultyFactor;

            // random factor for variety
            ScaledFactor = floorMultiplier * Random.Range(0.9f, 1.5f);

            enemyStats[Stat.PatrolSpeed] =
                Mathf.Lerp(1f, 3f, spawnFloor / 6f) + Random.Range(0f, 0.5f);
            enemyStats[Stat.ChaseSpeed] = enemyStats[Stat.PatrolSpeed] * 1.5f;

            enemyStats[Stat.MaxHP] = Mathf.RoundToInt(enemyStats[Stat.MaxHP] * ScaledFactor);
            enemyStats[Stat.Attack] = Mathf.RoundToInt(enemyStats[Stat.Attack] * ScaledFactor);
            enemyStats[Stat.Defense] = Mathf.RoundToInt(enemyStats[Stat.Defense] * ScaledFactor);
            enemyStats[Stat.Dexterity] = Mathf.RoundToInt(
                enemyStats[Stat.Dexterity] * ScaledFactor
            );
            enemyStats[Stat.ElementalDamage] = Mathf.RoundToInt(
                enemyStats[Stat.ElementalDamage] * ScaledFactor
            );
            enemyStats[Stat.AttackRange] = Mathf.RoundToInt(
                enemyStats[Stat.AttackRange] * ScaledFactor
            );
            enemyStats[Stat.ProjectileRange] = Mathf.RoundToInt(
                enemyStats[Stat.ProjectileRange] * ScaledFactor
            );
            enemyStats[Stat.ChanceToInflictStatusEffect] = Mathf.RoundToInt(
                enemyStats[Stat.ChanceToInflictStatusEffect] * ScaledFactor
            );
            enemyStats[Stat.Speed] = Mathf.RoundToInt(enemyStats[Stat.Speed] * ScaledFactor);
            enemyStats[Stat.Intelligence] = Mathf.RoundToInt(
                enemyStats[Stat.Intelligence] * ScaledFactor
            );
            enemyStats[Stat.Evasion] = Mathf.RoundToInt(enemyStats[Stat.Evasion] * ScaledFactor);
            enemyStats[Stat.CritChance] = Mathf.RoundToInt(
                enemyStats[Stat.CritChance] * ScaledFactor
            );
            enemyStats[Stat.CritDamage] = Mathf.RoundToInt(
                enemyStats[Stat.CritDamage] * ScaledFactor
            );
            enemyStats[Stat.FireRate] = Mathf.RoundToInt(enemyStats[Stat.FireRate] * ScaledFactor);
            enemyStats[Stat.Shield] = Mathf.RoundToInt(enemyStats[Stat.Shield] * ScaledFactor);
            enemyStats[Stat.Accuracy] = Mathf.RoundToInt(enemyStats[Stat.Accuracy] * ScaledFactor);
            enemyStats[Stat.StatusEffectDuration] = Mathf.RoundToInt(
                enemyStats[Stat.StatusEffectDuration] * ScaledFactor
            );

            enemyStats[Stat.HP] = enemyStats[Stat.MaxHP];

            // Setup dynamicDamageTypes
            dynamicDamageTypes.Clear();
            dynamicDamageTypes.Add(DamageType.Physical, enemyStats[Stat.Attack]);

            if (elementalBase != DamageType.Physical)
            {
                // e.g. half the scaled attack as elemental or something else
                float extraElemental =
                    (enemyStats[Stat.ElementalDamage] > 0)
                        ? (enemyStats[Stat.ElementalDamage] * ScaledFactor)
                        : (enemyStats[Stat.Attack] * 0.5f);

                dynamicDamageTypes.Add(elementalBase, extraElemental);
            }
        }

        public void TakeDamage(DamageInfo damageInfo, bool bypassInvincible = false)
        {
            if (!bypassInvincible && invincible)
            {
                Debug.Log($"{gameObject.name} is invincible.");
                return;
            }

            float totalDamage = 0f;
            foreach (var damageEntry in damageInfo.DamageAmounts)
            {
                DamageType type = damageEntry.Key;
                float amount = damageEntry.Value;

                if (immunities.Contains((Immunities)type))
                {
                    Debug.Log($"{gameObject.name} is immune to {type} damage.");
                    FloatingTextManager.Instance.ShowFloatingText(
                        $"{type.ToString().ToUpper()} IMMUNE",
                        transform,
                        Color.cyan
                    );
                    continue;
                }

                if (resistances.Contains((Resistances)type))
                {
                    amount *= 0.5f;
                }
                if (weaknesses.Contains((Weaknesses)type))
                {
                    amount *= 1.5f;
                }
                totalDamage += amount;
            }

            float effectiveDamage = Mathf.Max(totalDamage - enemyStats[Stat.Defense], 1);
            enemyStats[Stat.HP] = Mathf.Max(
                enemyStats[Stat.HP] - Mathf.RoundToInt(effectiveDamage),
                0
            );

            // Status effects
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                // If attacker has a certain chance to inflict
                // e.g. from playerStats or the skill used
                if (Random.value < PlayerStats.Instance.GetCurrentChanceToInflictStatusEffect())
                {
                    StatusEffectManager.Instance.AddStatusEffect(gameObject, effect);
                }
            }

            UpdateHealthUI();
            FloatingTextManager.Instance.ShowFloatingText(
                effectiveDamage.ToString(),
                transform,
                Color.red
            );

            if (enemyStats[Stat.HP] <= 0)
            {
                HandleDeath();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("EnemyStats: Heal amount must be positive.");
                return;
            }

            enemyStats[Stat.HP] = Mathf.Min(enemyStats[Stat.HP] + amount, enemyStats[Stat.MaxHP]);
            UpdateHealthUI();
            Debug.Log($"Healed {amount} => {enemyStats[Stat.HP]}/{enemyStats[Stat.MaxHP]}");
        }

        private void HandleDeath()
        {
            AwardExperienceToPlayer();
            Debug.Log($"Enemy {gameObject.name} has died.");

            // If there's an _Enemy script, call DropLoot()
            var enemyScript = GetComponent<_Enemy>();
            if (enemyScript != null)
            {
                enemyScript.DropLoot();
            }

            // Release the tile
            var nav = GetComponent<EnemyNavigator>();
            if (nav != null)
            {
                TileOccupancyManager.Instance.ReleaseAllTiles(nav.occupantID);
            }

            Destroy(gameObject);
        }

        public void SetInvincible(bool invincible)
        {
            this.invincible = invincible;
        }

        public void AddShield(int shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("Shield value must be positive.");
                return;
            }
            shieldValue += shieldValue;
            enemyStats[Stat.Defense] += shieldValue;
        }

        public void RemoveShield(int shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("Shield value must be positive.");
                return;
            }

            float effective = Mathf.Min(shieldValue, enemyStats[Stat.Shield]);
            enemyStats[Stat.Shield] -= effective;
            enemyStats[Stat.Defense] -= effective;
        }

        private void UpdateHealthUI()
        {
            var enemyUI = GetComponentInChildren<EnemyUI>();
            if (enemyUI != null)
            {
                enemyUI.UpdateHealthBar(enemyStats[Stat.HP], enemyStats[Stat.MaxHP]);
            }
        }

        private int CalculateExperiencePoints()
        {
            int baseExperience = Mathf.RoundToInt(ScaledFactor * 10);
            return baseExperience;
        }

        private void AwardExperienceToPlayer()
        {
            int xp = CalculateExperiencePoints();
            PlayerStats.Instance?.GainExperience(xp);
            Debug.Log($"Awarded {xp} XP to player.");
        }
    }
}
