using System;
using UnityEngine;
using CoED;

namespace CoED
{
    public class EnemyStats : MonoBehaviour
    {

      
        [Header("Base Stats")]
        [SerializeField, Min(0)]
        private int baseAttack {get;}= 10;

        [SerializeField, Min(0)]
        private int baseDefense = 5;
   
        [SerializeField, Min(0f)]
        private float baseProjectileRange = 1f;
        [SerializeField, Min(0f)]
        private float baseDetectionRange = 5f;
        [SerializeField, Min(0f)]
        private float baseFireRate = 5f;

        [SerializeField, Min(0)]
        private int baseSpeed = 5;

        // Current Stats (after applying equipment)
        private int maxHealth = 100;
        public int CurrentAttack { get; set; }
        public int CurrentHealth { get; set; }
        public float CurrentDefense { get; set; }
        public float CurrentProjectileRange { get; set; }
        public float CurrentDetectionRange { get; set; }
        public float CurrentSpeed { get; set; }
        public float CurrentFireRate { get; set; }

        public event Action<int, int> OnHealthChanged;
        public event Action OnEnemyDeath;
        public int spawnFloor {get; set;} // Store the floor this enemy spawned on

        private void Awake()
        {
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

            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.SetHealthBarMax(maxHealth);
                UpdateHealthUI(uiManager);
            }
        }

        private void CalculateStats()
        {
            float floorMultiplier = 1 + (spawnFloor * 0.1f); // Example: each floor increases stats by 10%

            CurrentAttack = Mathf.RoundToInt(baseAttack * floorMultiplier);
            CurrentDefense = baseDefense * floorMultiplier;
            CurrentProjectileRange = baseProjectileRange * floorMultiplier;
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
            UpdateHealthUI(UIManager.Instance);

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
            UpdateHealthUI(UIManager.Instance);

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;
            floatingTextManager?.ShowFloatingText($"+{amount}", transform.position, Color.green);

            Debug.Log($"EnemyStats: Healed {amount} health. Current health: {CurrentHealth}/{maxHealth}");
        }

        private void HandleDeath()
        {
            IActor actor = GetComponent<IActor>();
            if (actor != null)
            {
                Debug.Log("Enemy has died.");
                TurnManager.Instance.RemoveActor(actor); // Remove from TurnManager
                Destroy(gameObject);
            }
        }


        private void UpdateHealthUI(UIManager uiManager)
        {
            if (uiManager != null)
            {
                uiManager.UpdateHealthBar(CurrentHealth);
            }
        }
    }
}

