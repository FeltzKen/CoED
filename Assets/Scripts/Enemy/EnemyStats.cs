using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class _EnemyStats : MonoBehaviour
    {
        [Header("Assigned Monster Data")]
        [Tooltip("This enemy's monster data record.")]
        public Monster monsterData; // from the new system

        [Header("Base Stats")]
        [SerializeField, Min(1)]
        private int baseAttack = 10;

        [SerializeField]
        private DamageType elementalBase = DamageType.Physical;

        [SerializeField, Min(0)]
        private int elementalDamage;

        [SerializeField, Min(1)]
        private int baseDefense = 5;

        private int currentShieldValue = 0;

        [SerializeField, Min(100)]
        private int baseHealth = 100;

        [SerializeField, Min(1f)]
        private float baseAttackRange = 1.4f;

        [SerializeField, Min(1f)]
        private float baseFireRate = 1f;

        [SerializeField, Min(1f)]
        private float baseProjectileLifespan = 2f;

        [SerializeField, Min(0.5f)]
        private float projectileBaseAttackRange;

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
        public float PatrolSpeed;
        public float ChaseSpeed;
        public int CurrentAttack { get; set; }
        public int CurrentDefense { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public float CurrentAttackRange { get; set; }
        public float CurrentFireRate { get; set; }
        public float CurrentProjectileLifespan { get; set; }
        public float ProjectileCurrentAttackRange { get; set; }
        public float CurrentSpeed { get; set; } = 1f;

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
            CalculateStats();
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (enemyUI != null)
            {
                enemyUI.SetHealthBarMax(MaxHealth);
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

            baseHealth = monster.maxHP;
            baseAttack = monster.attack;
            baseDefense = monster.defense;
            elementalBase = monster.damageType;
            elementalDamage = 0;

            chanceToInflictStatusEffect = monster.statusInflictionChance;
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
        private void CalculateStats()
        {
            var dungeonSettings = FindAnyObjectByType<DungeonGenerator>().dungeonSettings;

            float floorMultiplier =
                1
                + (spawnFloor * dungeonSettings.difficultyLevel * 0.1f)
                + dungeonSettings.playerLevelFactor
                + dungeonSettings.floorDifficultyFactor;

            // random factor for variety
            ScaledFactor = floorMultiplier * Random.Range(0.9f, 1.5f);

            PatrolSpeed = Mathf.Lerp(1f, 3f, spawnFloor / 6f) + Random.Range(0f, 0.5f);
            ChaseSpeed = PatrolSpeed * 1.5f;

            MaxHealth = Mathf.RoundToInt(baseHealth * ScaledFactor);
            CurrentAttack = Mathf.RoundToInt(baseAttack * ScaledFactor);
            CurrentDefense = Mathf.RoundToInt(baseDefense * ScaledFactor);

            ProjectileCurrentAttackRange = projectileBaseAttackRange * ScaledFactor;
            CurrentAttackRange = baseAttackRange;
            CurrentFireRate = Mathf.Max(baseFireRate * ScaledFactor, 0.1f);
            CurrentProjectileLifespan = baseProjectileLifespan * ScaledFactor;

            CurrentHealth = MaxHealth;

            // Setup dynamicDamageTypes
            dynamicDamageTypes.Clear();
            dynamicDamageTypes.Add(DamageType.Physical, CurrentAttack);

            if (elementalBase != DamageType.Physical)
            {
                // e.g. half the scaled attack as elemental or something else
                float extraElemental =
                    (elementalDamage > 0)
                        ? (elementalDamage * ScaledFactor)
                        : (CurrentAttack * 0.5f);

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

            float effectiveDamage = Mathf.Max(totalDamage - CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - Mathf.RoundToInt(effectiveDamage), 0);

            // Status effects
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                // If attacker has a certain chance to inflict
                // e.g. from playerStats or the skill used
                if (Random.value < PlayerStats.Instance.chanceToInflictStatusEffect)
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

            if (CurrentHealth <= 0)
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

            CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
            UpdateHealthUI();
            Debug.Log($"Healed {amount} => {CurrentHealth}/{MaxHealth}");
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
            currentShieldValue += shieldValue;
            CurrentDefense += shieldValue;
        }

        public void RemoveShield(int shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("Shield value must be positive.");
                return;
            }

            int effective = Mathf.Min(currentShieldValue, shieldValue);
            currentShieldValue -= effective;
            CurrentDefense -= effective;
        }

        private void UpdateHealthUI()
        {
            var enemyUI = GetComponentInChildren<EnemyUI>();
            if (enemyUI != null)
            {
                enemyUI.UpdateHealthBar(CurrentHealth, MaxHealth);
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
