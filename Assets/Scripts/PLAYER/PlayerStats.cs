using System;
using UnityEngine;
using CoED;

namespace CoED
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Leveling System")]
        [SerializeField, Min(1)]
        public int level = 1;

        [SerializeField, Min(0)]
        private int experience = 0;

        [SerializeField, Min(1)]
        private int experienceToNextLevel = 100;

        [SerializeField, Min(1)]
        private int maxLevel = 50;

        [SerializeField, Min(0)]
        public float maxStamina { get; private set; } = 100f;

        [SerializeField, Min(0)]
        private int maxHealth = 100;
        public float CurrentStamina { get; set; }

        [Header("Base Stats")]
        [SerializeField, Min(0)]
        private int baseAttack = 10;

        [SerializeField, Min(0)]
        private int baseDefense = 5;

        [SerializeField, Min(0)]
        private int baseMagic = 5;

        [SerializeField, Min(0f)]
        private float baseRange = 5f;

        [SerializeField, Min(0)]
        private int baseSpeed = 5;

        [SerializeField, Min(0f)]
        private float baseFireRate = 1f;

        [SerializeField, Min(0f)]
        private float baseProjectileLifespan = 2f;

        // Currency variable
        [Header("Currency")]
        [SerializeField, Min(0)]
        private int currency = 0;

        // Current Stats (after applying equipment)
        public int CurrentAttack { get; set; }
        public float CurrentDefense { get; set; }
        public int CurrentMagic { get; set; }
        public int CurrentMaxHealth { get; set; }
        public float CurrentRange { get; set; }
        public float CurrentSpeed { get; set; }
        public float CurrentFireRate { get; set; }
        public float CurrentProjectileLifespan { get; set; }
        public int CurrentHealth { get; set; }

        // Equipment Stats (set by EquipmentManager)
        private int equipmentAttack = 0;
        private int equipmentDefense = 0;
        private int equipmentHealth = 0;

        // Events
        public event Action OnLevelUp;
        public event Action<int, int> OnExperienceChanged;
        public event Action<int, int> OnHealthChanged;
        public event Action OnPlayerDeath;

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
                Debug.LogWarning("PlayerStats instance already exists. Destroying duplicate.");
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
            CurrentStamina = maxStamina;
            CurrentHealth = CurrentMaxHealth;

            UIManager uiManager = FindAnyObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.SetHealthBarMax(CurrentMaxHealth);
                UpdateHealthUI(uiManager);
            }
        }

        private void CalculateStats()
        {
            CurrentAttack = baseAttack + Mathf.RoundToInt(baseAttack * level * 0.2f) + equipmentAttack;
            CurrentDefense = baseDefense + Mathf.RoundToInt(baseDefense * level * 0.15f) + equipmentDefense;
            CurrentRange = baseRange + level * 0.1f;
            CurrentSpeed = baseSpeed + level * 0.05f;
            CurrentFireRate = Mathf.Max(baseFireRate - level * 0.02f, 0.1f);
            CurrentProjectileLifespan = baseProjectileLifespan + level * 0.05f;
            CurrentMaxHealth = maxHealth + level * 20 + equipmentHealth;

            // Ensure current health does not exceed max health
            CurrentHealth = Mathf.Min(CurrentHealth, CurrentMaxHealth);
        }

        public void SetEquipmentStats(int attack, int defense, int health)
        {
            equipmentAttack = Mathf.Max(attack, 0);
            equipmentDefense = Mathf.Max(defense, 0);
            equipmentHealth = Mathf.Max(health, 0);
            CalculateStats();
            UpdateHealthUI(FindAnyObjectByType<UIManager>());
        }

        public void GainExperience(int amount)
        {
            if (level >= maxLevel)
            {
                Debug.Log("PlayerStats: Maximum level reached.");
                return;
            }

            experience += Mathf.Max(amount, 0);
            OnExperienceChanged?.Invoke(experience, experienceToNextLevel);
            Debug.Log($"PlayerStats: Gained {amount} experience. Total experience: {experience}/{experienceToNextLevel}");

            while (experience >= experienceToNextLevel && level < maxLevel)
            {
                experience -= experienceToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            experienceToNextLevel = Mathf.CeilToInt(experienceToNextLevel * 1.25f);
            CalculateStats();

            OnLevelUp?.Invoke();
            UIManager uiManager = FindAnyObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.UpdateLevelDisplay(level);
                UpdateHealthUI(uiManager);
            }

            Debug.Log($"PlayerStats: Leveled up to level {level}! Next level at {experienceToNextLevel} experience.");
        }

        public void GainCurrency(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Gain amount must be positive.");
                return;
            }

            currency += amount;
            Debug.Log($"PlayerStats: Gained {amount} currency. Total currency: {currency}");
        }

        public int GetCurrency()
        {
            return currency;
        }

        public void TakeDamage(int damage)
        {
            int effectiveDamage = Mathf.Max(damage - (int)CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
            UpdateHealthUI(FindAnyObjectByType<UIManager>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            floatingTextManager?.ShowFloatingText(effectiveDamage.ToString(), transform.position, Color.red);

            Debug.Log($"PlayerStats: Took {effectiveDamage} damage. Current health: {CurrentHealth}/{CurrentMaxHealth}");

            if (CurrentHealth <= 0)
            {
                HandleDeath();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Heal amount must be positive.");
                return;
            }

            CurrentHealth = Mathf.Min(CurrentHealth + amount, CurrentMaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, CurrentMaxHealth);
            UpdateHealthUI(FindAnyObjectByType<UIManager>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            floatingTextManager?.ShowFloatingText($"+{amount}", transform.position, Color.green);

            Debug.Log($"PlayerStats: Healed {amount} health. Current health: {CurrentHealth}/{CurrentMaxHealth}");
        }

        private void HandleDeath()
        {
            Debug.Log("PlayerStats: Player has died.");
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager?.OnPlayerDeath();
            OnPlayerDeath?.Invoke();
        }

        private void UpdateHealthUI(UIManager uiManager)
        {
            if (uiManager != null)
            {
                uiManager.UpdateHealthBar(CurrentHealth);
            }
        }

        public int GetLevel() => level;
        public int GetExperience() => experience;
        public int GetExperienceToNextLevel() => experienceToNextLevel;
    }
}

/*
Changes made:
1. Removed any direct references to managing turns, as action registration should happen through PlayerManager.
2. Cleaned up methods to focus on health, stamina, experience, and level management, allowing PlayerManager to decide when to register actions with TurnManager.
3. Removed unnecessary turn-specific logic or left-over remnants that tried to register actions, ensuring consistency with the centralized approach.
*/
