using System;
using System.Collections.Generic;
using CoED;
using UnityEngine;

namespace CoED
{
    public class EnemyStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField, Min(0)]
        private int baseAttack = 10;

        [SerializeField, Min(0)]
        private int baseDefense = 5;

        [SerializeField, Min(0)]
        private int baseHealth = 100;

        [SerializeField, Min(0)]
        private int baseSpeed = 5;

        [SerializeField, Min(0f)]
        private float baseAttackRange = 1.0f;

        [SerializeField, Min(0f)]
        private float baseDetectionRange = 5f;

        [SerializeField, Min(0f)]
        private float baseFireRate = 1f;

        [SerializeField, Min(0f)]
        private float baseProjectileLifespan = 2f;

        private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

        // Current Stats
        public int CurrentAttack { get; set; }
        public int CurrentDefense { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentSpeed { get; set; }
        public float CurrentAttackRange { get; set; }
        public float CurrentDetectionRange { get; set; }
        public float CurrentFireRate { get; set; }
        public float CurrentProjectileLifespan { get; set; }

        private EnemyUI enemyUI { get; set; }
        private Enemy enemy { get; set; }

        // public event Action <int> OnHealthChanged;
        public event Action OnEnemyDeath;
        public int spawnFloor { get; set; } // Store the floor this enemy spawned on

        private void Awake() { }

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
            float floorMultiplier = 1 + (spawnFloor * 0.1f); // Example: each floor increases stats by 10%

            MaxHealth = Mathf.RoundToInt(baseHealth * floorMultiplier);
            CurrentAttack = Mathf.RoundToInt(baseAttack * floorMultiplier);
            CurrentDefense = Mathf.RoundToInt(baseDefense * floorMultiplier);
            CurrentSpeed = Mathf.RoundToInt(baseSpeed * floorMultiplier);
            CurrentAttackRange = baseAttackRange * floorMultiplier;
            CurrentDetectionRange = baseDetectionRange * floorMultiplier;
            CurrentFireRate = Mathf.Max(baseFireRate - spawnFloor * 0.02f, 0.1f);
            CurrentProjectileLifespan = baseProjectileLifespan + spawnFloor * 0.05f;
            CurrentHealth = MaxHealth;

            InitializeUI();
        }

        public void TakeDamage(int damage)
        {
            int effectiveDamage = Mathf.Max(damage - CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            // OnHealthChanged?.Invoke(CurrentHealth);
            FloatingTextManager.Instance.ShowFloatingText(
                effectiveDamage.ToString(),
                transform,
                Color.red
            );

            UpdateHealthUI();

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;

            // Debug.Log($"EnemyStats: Took {effectiveDamage} damage. Current health: {CurrentHealth}/{MaxHealth}");

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
            //OnHealthChanged?.Invoke(CurrentHealth);
            UpdateHealthUI();

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;
            //  floatingTextManager?.ShowFloatingText($"+{amount}", transform, Color.green);

            Debug.Log(
                $"EnemyStats: Healed {amount} health. Current health: {CurrentHealth}/{MaxHealth}"
            );
        }

        public void ApplyStatusEffect(
            StatusEffectType effectType,
            float damagePerSecond,
            float duration
        )
        {
            GameObject effectObject = new GameObject($"{effectType}Effect");
            effectObject.transform.parent = this.transform;
            StatusEffect statusEffect = effectObject.AddComponent<StatusEffect>();
            statusEffect.effectType = effectType;
            statusEffect.damagePerSecond = damagePerSecond;
            statusEffect.duration = duration;
            activeStatusEffects.Add(statusEffect);
        }

        public void ResetSpeed()
        {
            // Reset speed to base or modified speed
            CurrentSpeed = baseSpeed; // Assuming baseSpeed is defined
        }

        // Optionally, handle removing or managing multiple effects
        private void HandleDeath()
        {
            AwardExperienceToPlayer();
            Debug.Log("Enemy has died.");
            Destroy(gameObject);
            enemy.DropLoot();
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
            // Base experience points
            int baseExperience = 10;

            // Scaling factor for experience points per floor
            int scalingFactor = 5;

            // Random range factor to add variability
            int randomRange = 3;

            // Calculate experience points with some randomness
            int experiencePoints =
                baseExperience
                + (spawnFloor * scalingFactor)
                + UnityEngine.Random.Range(baseExperience * spawnFloor, randomRange * spawnFloor);

            return experiencePoints;
        }

        private void AwardExperienceToPlayer()
        {
            int experiencePoints = CalculateExperiencePoints();
            PlayerStats.Instance?.GainExperience(experiencePoints);
            Debug.Log($"EnemyStats: Awarded {experiencePoints} experience points to the player.");
        }
    }
}
