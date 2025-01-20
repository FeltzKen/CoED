using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CoED
{
    public class EnemyStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField, Min(1)]
        private int baseAttack = 10;

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

        [Header("Resistances")]
        [Header("Elemental Attributes")]
        public List<Immunities> immunities = new List<Immunities>();

        [Header("Resistances")]
        public List<Resistances> resistances = new List<Resistances>();

        [Header("Weaknesses")]
        public List<Weaknesses> weaknesses = new List<Weaknesses>();

        [Header("Dynamic Damage Types")]
        public Dictionary<DamageType, float> dynamicDamageTypes =
            new Dictionary<DamageType, float>();

        [Header("Inflicted Status Effects")]
        public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();

        public float chanceToInflictStatusEffect = 0.1f;

        [SerializeField]
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();

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
        public float ScaledFactor { get; private set; }
        public float ProjectileCurrentAttackRange { get; set; }
        public float CurrentSpeed { get; set; } = 1f;
        private EnemyUI enemyUI { get; set; }
        private Enemy enemy { get; set; }
        public int spawnFloor { get; set; }

        private void Start()
        {
            CalculateStats();
        }

        private void InitializeUI()
        {
            enemy = GetComponent<Enemy>();
            enemyUI = GetComponent<EnemyUI>();
            if (enemyUI != null)
            {
                enemyUI.SetHealthBarMax(MaxHealth);
            }
        }

        private void CalculateStats()
        {
            var dungeonSettings = FindAnyObjectByType<DungeonGenerator>().dungeonSettings;
            float floorMultiplier =
                1
                + (spawnFloor * dungeonSettings.difficultyLevel * 0.1f)
                + dungeonSettings.playerLevelFactor
                + dungeonSettings.floorDifficultyFactor;

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

            InitializeUI();
        }

        /// <summary>
        /// Handles complex damage and applies status effects to enemies.
        /// </summary>
        /// <param name="damageInfo">Damage information, including damage types and status effects.</param>
        /// <param name="bypassInvincible">Whether to bypass invincibility.</param>
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
                float damageAmount = damageEntry.Value;

                // ✅ Check for Immunity
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

                // ✅ Apply Resistance
                if (resistances.Contains((Resistances)type))
                {
                    damageAmount *= 0.5f;
                }

                // ❌ Apply Weakness
                if (weaknesses.Contains((Weaknesses)type))
                {
                    damageAmount *= 1.5f;
                }

                totalDamage += damageAmount;
            }

            // ✅ Apply defense
            float effectiveDamage = Mathf.Max(totalDamage - CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - Mathf.RoundToInt(effectiveDamage), 0);

            // ✅ Apply all status effects dynamically
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                StatusEffectManager.Instance.AddStatusEffect(gameObject, effect);
            }

            // ✅ UI Updates
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

            Debug.Log(
                $"EnemyStats: Healed {amount} health. Current health: {CurrentHealth}/{MaxHealth}"
            );
        }

        private void HandleDeath()
        {
            AwardExperienceToPlayer();
            Debug.Log("Enemy has died.");
            Destroy(gameObject);
            enemy.DropLoot();
            TileOccupancyManager.Instance.ReleaseAllTiles(
                GetComponent<EnemyNavigator>().occupantID
            );
        }

        public void SetInvincible(bool invincible)
        {
            this.invincible = invincible;
        }

        public void AddShield(int shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("EnemyStats: Shield value must be positive.");
                return;
            }

            currentShieldValue += shieldValue;
            CurrentDefense += shieldValue;
            Debug.Log($"Shield added: {shieldValue}. Current defense: {CurrentDefense}");
        }

        public void RemoveShield(int shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("EnemyStats: Shield value must be positive.");
                return;
            }

            int effectiveShieldRemoval = Mathf.Min(currentShieldValue, shieldValue);
            currentShieldValue -= effectiveShieldRemoval;
            CurrentDefense -= effectiveShieldRemoval;

            Debug.Log(
                $"Shield removed: {effectiveShieldRemoval}. Current defense: {CurrentDefense}"
            );
        }

        private void UpdateHealthUI()
        {
            EnemyUI enemyUI = GetComponentInChildren<EnemyUI>();
            if (enemyUI == null)
            {
                Debug.LogError("EnemyStats: EnemyUI component not found.");
            }
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
            int experiencePoints = CalculateExperiencePoints();
            PlayerStats.Instance?.GainExperience(experiencePoints);
            Debug.Log($"EnemyStats: Awarded {experiencePoints} experience points to the player.");
        }
    }
}
