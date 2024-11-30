using System;
using UnityEngine;
using CoED;

namespace CoED
{
    public class EnemyStats : MonoBehaviour
    {
        [Header("Experience System")]
        [SerializeField, Min(1)]
        public int availableExperienceLevel = 1;
        
      
        [Header("Base Stats")]
        [SerializeField, Min(0)]
        private int baseAttack {get;} = 10;

        [SerializeField, Min(0)]
        private int baseDefense = 5;
   
        [SerializeField, Min(0f)]
        private float baseAttackRange = 1.0f;
        [SerializeField, Min(0f)]
        private float baseDetectionRange = 5f;
        [SerializeField, Min(0f)]
        private float baseFireRate = 5f;

        [SerializeField, Min(0)]
        private int baseSpeed = 5;

        // Current Stats (after applying equipment)
        private int maxHealth = 1000;
        public int CurrentAttack { get; set; }
        public int CurrentHealth { get; set; }
        public float CurrentDefense { get; set; }
        public float CurrentAttackRange { get; set; }
        public float CurrentDetectionRange { get; set; }
        public float CurrentSpeed { get; set; }
        public float CurrentFireRate { get; set; }
        public EnemyUI enemyUI { get; set; }
        public event Action<int, int> OnHealthChanged;
        public event Action OnEnemyDeath;
        public int spawnFloor {get; set;} // Store the floor this enemy spawned on

        private void Awake()
        {
            enemyUI = GetComponentInChildren<EnemyUI>();
            if (enemyUI == null)
            {
                Debug.LogError("EnemyStats: EnemyUI component not found.");
            }
            Debug.Log("EnemyStats instance initialized for: " + gameObject.name);
        }

        private void Start()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {

            CalculateStats();
            CurrentHealth = maxHealth;
            enemyUI.SetHealthBarMax(maxHealth);
            UpdateHealthUI(enemyUI);
            
        }

        private void CalculateStats()
        {
            float floorMultiplier = 1 + (spawnFloor * 0.1f); // Example: each floor increases stats by 10%
            CurrentAttack = Mathf.RoundToInt(baseAttack * floorMultiplier);
            CurrentDefense = baseDefense * floorMultiplier;
            CurrentAttackRange = baseAttackRange * floorMultiplier;
            CurrentDetectionRange = baseDetectionRange * floorMultiplier;
            CurrentSpeed = baseSpeed * floorMultiplier;

            // Ensure current health does not exceed max health
            CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            int effectiveDamage = Mathf.Max(damage - (int)CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            UpdateHealthUI(enemyUI);

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;
            floatingTextManager?.ShowFloatingText(effectiveDamage.ToString(), transform.position, Color.red);

            Debug.Log($"EnemyStats: Took {effectiveDamage} damage. Current health: {CurrentHealth}/{maxHealth}");

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

            CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            UpdateHealthUI(enemyUI);

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;
            floatingTextManager?.ShowFloatingText($"+{amount}", transform.position, Color.green);

            Debug.Log($"EnemyStats: Healed {amount} health. Current health: {CurrentHealth}/{maxHealth}");
        }

        private void HandleDeath()
        {
            AwardExperienceToPlayer();
            Debug.Log("Enemy has died.");
            Destroy(gameObject);
        }


        private void UpdateHealthUI(EnemyUI enemyUI)
        {
            if (enemyUI != null)
            {
                enemyUI.UpdateHealthBar(CurrentHealth);
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
            int experiencePoints = baseExperience + (spawnFloor * scalingFactor) + UnityEngine.Random.Range(0, randomRange * spawnFloor);

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

