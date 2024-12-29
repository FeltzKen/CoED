using System;
using System.Collections.Generic;
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

        [SerializeField, Min(0f)]
        private float baseAttackRange = 1.0f;

        [SerializeField, Min(0f)]
        private float baseFireRate = 1f;

        [SerializeField, Min(0f)]
        private float baseProjectileLifespan = 2f;

        private List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

        // Current Stats
        public float PatrolSpeed;
        public float ChaseSpeed;
        public float CurrentAttack { get; set; }
        public float CurrentDefense { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public float CurrentAttackRange { get; set; }
        public float CurrentFireRate { get; set; }
        public float CurrentProjectileLifespan { get; set; }
        public float ScaledFactor { get; private set; }
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
            float floorMultiplier = 1 + (spawnFloor * 0.5f);
            ScaledFactor = floorMultiplier * UnityEngine.Random.Range(0.9f, 1.1f);

            PatrolSpeed = Mathf.Lerp(1f, 3f, spawnFloor / 6f) + UnityEngine.Random.Range(0f, 0.5f);
            ChaseSpeed = PatrolSpeed * 1.5f;

            MaxHealth = Mathf.RoundToInt(baseHealth * ScaledFactor);
            CurrentAttack = Mathf.RoundToInt(baseAttack * ScaledFactor);
            CurrentDefense = Mathf.RoundToInt(baseDefense * ScaledFactor);
            CurrentAttackRange = baseAttackRange * ScaledFactor;
            CurrentFireRate = Mathf.Max(baseFireRate * ScaledFactor, 0.1f);
            CurrentProjectileLifespan = baseProjectileLifespan * ScaledFactor;
            CurrentHealth = MaxHealth;

            InitializeUI();
        }

        public void TakeDamage(float damage)
        {
            float effectiveDamage = Mathf.Max(damage - CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            FloatingTextManager.Instance.ShowFloatingText(
                effectiveDamage.ToString(),
                transform,
                Color.red
            );

            UpdateHealthUI();

            if (CurrentHealth <= 0)
            {
                HandleDeath();
            }
        }

        public void Heal(float amount)
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

        public void ApplyStatusEffect(
            StatusEffectType effectType,
            float damagePerSecond,
            float duration
        )
        {
            GameObject effectObject = new GameObject($"{effectType}Effect");
            effectObject.transform.parent = transform;
            StatusEffect statusEffect = effectObject.AddComponent<StatusEffect>();
            statusEffect.effectType = effectType;
            statusEffect.damagePerSecond = damagePerSecond;
            statusEffect.duration = duration;
            activeStatusEffects.Add(statusEffect);
        }

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
