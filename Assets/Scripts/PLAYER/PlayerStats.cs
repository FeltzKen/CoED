using System;
using UnityEngine;
using CoED;
using Unity.VisualScripting;

namespace CoED
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Leveling System")]
        [SerializeField, Min(1)]
        public int level = 1;


        [SerializeField, Min(0)]
        private int currentExp = 0;

        [SerializeField, Min(1)]
        private int ExpToNextLevel = 100;

        [SerializeField, Min(1)]
        private int maxLevel = 50;

        [Header("Health")]
        [SerializeField, Min(0)]
        private int baseHealth = 1000;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        private int equipmentHealth = 0;

        [Header("Stamina")]
        [SerializeField, Min(0)]
        public float baseStamina = 100f;
        public float CurrentStamina { get; set; }
        public float MaxStamina { get; set; }

        [Header("Magic")]
        [SerializeField, Min(0)]
        private int baseMagic = 50;
        public int CurrentMagic { get; set; }
        public int MaxMagic { get; set; }

        [Header("Attack")]
        [SerializeField, Min(0)]
        private int baseAttack = 10;
        public int CurrentAttack { get; set; }
        private int equipmentAttack = 0;

        [Header("Defense")]
        [SerializeField, Min(0)]
        private int baseDefense = 5;
        public int CurrentDefense { get; set; }
        private int equipmentDefense = 0;

        [Header("Range")]
        [SerializeField, Min(0f)]
        private float baseRange = 5f;
        public float CurrentRange { get; set; }

        [Header("Speed")]
        [SerializeField, Min(0)]
        private float baseSpeed = 5;
        public float CurrentSpeed { get; set; }

        [Header("Fire Rate")]
        [SerializeField, Min(0f)]
        private float baseFireRate = 1f;
        public float CurrentFireRate { get; set; }

        [Header("Projectile Lifespan")]
        [SerializeField, Min(0f)]
        private float baseProjectileLifespan = 2f;
        public float CurrentProjectileLifespan { get; set; }

        [Header("Currency")]
        [SerializeField, Min(0)]
        private int currency = 0;

        public int currentFloor = 1;
        private PlayerUI playerUI;
        // Events
        public event Action OnLevelUp;
        public event Action<int, int> OnExperienceChanged;
        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnMagicChanged;
        public event Action<int, int> OnStaminaChanged;
        public event Action OnPlayerDeath;

        private void Awake()
        {
            PlayerUI playerUI = FindAnyObjectByType<PlayerUI>();

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
        private void InitializeUI()
        {
            if (playerUI != null)
            {
                playerUI.SetHealthBarMax(MaxHealth);

                UpdateExperienceUI(playerUI);
                
                playerUI.SetMagicBarMax(MaxMagic);

                
                playerUI.SetStaminaBarMax((int)MaxStamina);                
                
                UpdateLevelDisplay(playerUI);
                OnLevelUp?.Invoke();


            }
        }

        private void Start()
        {
            InitializeStats();
            InitializeUI();
        }

        private void InitializeStats()
        {
            CalculateStats();
            CurrentStamina = MaxStamina;
            CurrentHealth = MaxHealth;

            if (playerUI != null)
            {
                playerUI.SetHealthBarMax(MaxHealth);
                UpdateHealthUI(playerUI);
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
            MaxHealth = baseHealth + level * 20 + equipmentHealth;
            MaxStamina = baseStamina;
            MaxMagic = baseMagic;

            // Ensure current health does not exceed max health
            CurrentHealth = baseHealth + level * 20 + equipmentHealth;
        }

        public void SetEquipmentStats(int attack, int defense, int health)
        {
            equipmentAttack = Mathf.Max(attack, 0);
            equipmentDefense = Mathf.Max(defense, 0);
            equipmentHealth = Mathf.Max(health, 0);
            CalculateStats();
            UpdateHealthUI(playerUI);
        }

        public void GainExperience(int amount)
        {
            if (level >= maxLevel)
            {
                Debug.Log("PlayerStats: Maximum level reached.");
                return;
            }
            UpdateExperienceUI(FindAnyObjectByType<PlayerUI>());
            currentExp += Mathf.Max(amount, 0);
            OnExperienceChanged?.Invoke(currentExp, ExpToNextLevel);
            Debug.Log($"PlayerStats: Gained {amount} experience. Total experience: {currentExp}/{ExpToNextLevel}");

            while (currentExp >= ExpToNextLevel && level < maxLevel)
            {
                currentExp -= ExpToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            ExpToNextLevel = Mathf.CeilToInt(ExpToNextLevel * 1.25f);
            CalculateStats();

            if (playerUI != null)
            {
                playerUI.UpdateLevelDisplay();
                OnLevelUp?.Invoke();
                UpdateHealthUI(playerUI);
                UpdateExperienceUI(playerUI);
                UpdateLevelDisplay(playerUI);
            }

            Debug.Log($"PlayerStats: Leveled up to level {level}! Next level at {ExpToNextLevel} experience.");
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

        public void SpendCurrency(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Spend amount must be positive.");
                return;
            }

            if (currency < amount)
            {
                Debug.LogWarning("PlayerStats: Insufficient currency.");
                return;
            }

            currency -= amount;
            Debug.Log($"PlayerStats: Spent {amount} currency. Total currency: {currency}");
        }

        public void RefillMagicPartial(int amount){
            CurrentMagic += amount;
            OnMagicChanged?.Invoke(CurrentMagic, MaxMagic);
        }
        public void RefillMagicFull(){
            CurrentMagic = MaxMagic;
            OnMagicChanged?.Invoke(CurrentMagic, MaxMagic);
        }
        public void UseMagic(int amount){
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Magic amount must be positive.");
                return;
            }

            if (CurrentMagic < amount)
            {
                Debug.LogWarning("PlayerStats: Insufficient magic.");
                return;
            }

            CurrentMagic -= amount;
            OnMagicChanged?.Invoke(CurrentMagic, MaxMagic);
        }
        public void RefillStamina(){
            CurrentStamina = MaxStamina;
            OnStaminaChanged?.Invoke((int)CurrentStamina, (int)MaxStamina);
        }
        public void UseStamina(float amount){
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Stamina amount must be positive.");
                return;
            }

            if (CurrentStamina < amount)
            {
                Debug.LogWarning("PlayerStats: Insufficient stamina.");
                return;
            }

            CurrentStamina -= amount;
            OnStaminaChanged?.Invoke((int)CurrentStamina, (int)MaxStamina);
        }

        public void TakeDamage(int damage)
        {
            int effectiveDamage = Mathf.Max(damage - CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            floatingTextManager?.ShowFloatingText(effectiveDamage.ToString(), transform.position, Color.red);

            Debug.Log($"PlayerStats: Took {effectiveDamage} damage. Current health: {CurrentHealth}/{MaxHealth}");

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

            CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            floatingTextManager?.ShowFloatingText($"+{amount}", transform.position, Color.green);

            Debug.Log($"PlayerStats: Healed {amount} health. Current health: {CurrentHealth}/{MaxHealth}");
        }

        private void HandleDeath()
        {
            Debug.Log("PlayerStats: Player has died.");
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager?.OnPlayerDeath();
            OnPlayerDeath?.Invoke();
        }

        private void UpdateExperienceUI(PlayerUI playerUI)
        {
            if (playerUI != null)
            {
                playerUI.UpdateExperienceBar(currentExp, ExpToNextLevel);
            }
        }
        private void UpdateMagicUI(PlayerUI playerUI){
            if (playerUI != null)
            {
                playerUI.UpdateMagicBar(CurrentMagic, MaxMagic);
            }
        }

        private void UpdateHealthUI(PlayerUI playerUI)
        {
            if (playerUI != null)
            {
                playerUI.UpdateHealthBar(CurrentHealth, MaxHealth);
            }
        }
        private void UpdateStaminaUI(PlayerUI playerUI)
        {
            if (playerUI != null)
            {
                playerUI.UpdateStaminaBar((int)CurrentStamina, (int)MaxStamina);
            }
        }
        private void UpdateLevelDisplay(PlayerUI playerUI)
        {
            if (playerUI != null)
            {
                playerUI.UpdateLevelDisplay();
            }

        }
        
        public void UpdateLevelDisplay(int level)
        {
            if (playerUI != null)
            {
                playerUI.UpdateLevelDisplay();
            }
            // Update level display logic
        }
        public int GetLevel() => level;
        public int GetExperience() => currentExp;
        public int GetExperienceToNextLevel() => ExpToNextLevel;

        public int GetCurrentFloor() => currentFloor;
    }
}


