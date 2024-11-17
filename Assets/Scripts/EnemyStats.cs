using System;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class EnemyStats : MonoBehaviour
    {
        public static EnemyStats Instance { get; private set; }

      [SerializeField, Min(0)]
        private int maxHealth = 100;
      
        [Header("Base Stats")]
        [SerializeField, Min(0)]
        private int baseAttack {get;}= 10;

        [SerializeField, Min(0)]
        private int baseDefense = 5;

        [SerializeField, Min(0f)]
        private float baseAttackRange = 1f;
        private float baseDetectionRange = 5f;

        [SerializeField, Min(0)]
        private int baseSpeed = 5;

        // Currency variable
        [Header("Currency")]
        [SerializeField, Min(0)]
        private int moey = 0;

        // Current Stats (after applying equipment)
    private int CurrentMaxHealth;
    [field: SerializeField] public int CurrentAttack { get; set; }
    [field: SerializeField] public int CurrentMagic { get; set; }
    [field: SerializeField] public int CurrentHealth { get; set; }
    [field: SerializeField] public float CurrentDefense { get; set; }
    [field: SerializeField] public float CurrentAttackRange { get; set; }
    [field: SerializeField] public float CurrentProjectileRange { get; set; }
    [field: SerializeField] public float CurrentDetectionRange { get; set; }
    [field: SerializeField] public float CurrentSpeed { get; set; }
    [field: SerializeField] public float CurrentFireRate { get; set; }

        public event Action<int, int> OnHealthChanged;
        public event Action OnEnemyDeath;
        public int spawnFloor; // Store the floor this enemy spawned on

        private void Awake()
        {
            // Implement Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("EnemyStats instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {

            CalculateStats();
            CurrentHealth = CurrentMaxHealth;

            UIManager uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.SetHealthBarMax(CurrentMaxHealth);
                UpdateHealthUI(uiManager);
            }
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
            OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
            UpdateHealthUI(UIManager.Instance);

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;
            floatingTextManager?.ShowFloatingText(effectiveDamage.ToString(), transform.position, Color.red);

            Debug.Log($"EnemyStats: Took {effectiveDamage} damage. Current health: {CurrentHealth}/{CurrentMaxHealth}");

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

            CurrentHealth = Mathf.Min(CurrentHealth + amount, CurrentMaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
            UpdateHealthUI(UIManager.Instance);

            FloatingTextManager floatingTextManager = FloatingTextManager.Instance;
            floatingTextManager?.ShowFloatingText($"+{amount}", transform.position, Color.green);

            Debug.Log($"EnemyStats: Healed {amount} health. Current health: {CurrentHealth}/{CurrentMaxHealth}");
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

