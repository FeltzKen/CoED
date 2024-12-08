using System;
using UnityEngine;

namespace CoED
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Leveling System")]
        [SerializeField, Min(1)]
        public int level = 1;

        [SerializeField, Min(0)]
        private int CurrentExp = 0;

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
        public int baseStamina = 100;
        public int CurrentStamina { get; set; }
        public int MaxStamina { get; set; }

        [Header("Magic")]
        [SerializeField, Min(0)]
        private int baseMagic = 50;
        public int CurrentMagic { get; set; }
        public int MaxMagic { get; set; }
        public int equipmentMagic = 0;

        [Header("Attack")]
        [SerializeField, Min(0)]
        private int baseAttack = 10;
        public int CurrentAttack { get; set; }
        private int equipmentAttack = 0;

        [SerializeField]
        public float AttackRange = 3f;

        [Header("Defense")]
        [SerializeField, Min(0)]
        private int baseDefense = 5;
        public int CurrentDefense { get; set; }
        private int equipmentDefense = 0;

        [Header("Range")]
        [SerializeField, Min(0f)]
        private float baseProjectileRange = 5f;
        public float CurrentProjectileRange { get; set; }
        private float equipmentRange = 0;

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

        public bool HasEnteredDungeon { get; set; } = false;
        public bool transitioningComplete;

        [Header("Currency")]
        [SerializeField, Min(0)]
        private int currency = 0;

        [SerializeField]
        public int currentFloor = 1;
        private PlayerUI playerUI;

        // Events
        //  public event Action OnLevelUp;
        // public event Action<int, int> OnExperienceChanged;
        //  public event Action<int, int> OnHealthChanged;
        // public event Action<int, int> OnMagicChanged;
        // public event Action<int, int> OnStaminaChanged;
        public event Action OnPlayerDeath;

        private void Awake()
        {
            //_ = FindAnyObjectByType<PlayerUI>();

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
            CalculateStats();
        }

        private void InitializeUI()
        {
            playerUI = FindAnyObjectByType<PlayerUI>();
            if (playerUI != null)
            {
                playerUI.SetHealthBarMax(MaxHealth);
                UpdateExperienceUI(playerUI);
                playerUI.SetMagicBarMax(MaxMagic);
                playerUI.SetStaminaBarMax(MaxStamina);
                playerUI.UpdateLevelDisplay();
            }
        }

        private void CalculateStats()
        {
            MaxStamina = baseStamina + Mathf.RoundToInt(level * 5);
            MaxHealth = baseHealth + level * 20 + equipmentHealth;
            MaxMagic = baseMagic + level * 10 + equipmentMagic;
            CurrentAttack =
                baseAttack + Mathf.RoundToInt(baseAttack * level * 0.2f) + equipmentAttack;
            CurrentDefense =
                baseDefense + Mathf.RoundToInt(baseDefense * level * 0.15f) + equipmentDefense;
            CurrentProjectileRange = baseProjectileRange + level * 0.1f + equipmentRange;
            CurrentSpeed = baseSpeed + level * 0.05f;
            CurrentFireRate = Mathf.Max(baseFireRate - level * 0.02f, 0.1f);
            CurrentProjectileLifespan = baseProjectileLifespan + level * 0.05f;
            CurrentHealth = MaxHealth;
            CurrentMagic = MaxMagic;
            CurrentStamina = MaxStamina;

            InitializeUI();
        }

        public void SetEquipmentStats(int attack, int defense, int health)
        {
            equipmentAttack = Mathf.Max(attack, 0);
            equipmentDefense = Mathf.Max(defense, 0);
            equipmentHealth = Mathf.Max(health, 0);
            CalculateStats();
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());
        }

        public void GainExperience(int amount)
        {
            if (level >= maxLevel)
            {
                // Debug.Log("PlayerStats: Maximum level reached.");
                return;
            }
            CurrentExp += Mathf.Max(amount, 0);
            //OnExperienceChanged?.Invoke(CurrentExp, ExpToNextLevel);
            // Debug.Log($"PlayerStats: Gained {amount} experience. Total experience: {CurrentExp}/{ExpToNextLevel}");
            UpdateExperienceUI(FindAnyObjectByType<PlayerUI>());

            while (CurrentExp >= ExpToNextLevel && level < maxLevel)
            {
                CurrentExp -= ExpToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            ExpToNextLevel = Mathf.CeilToInt(ExpToNextLevel * 1.25f);
            CalculateStats();

            //  OnLevelUp?.Invoke();
            if (playerUI != null)
            {
                playerUI = FindAnyObjectByType<PlayerUI>();
                playerUI.UpdateLevelDisplay();
                UpdateHealthUI(playerUI);
                UpdateExperienceUI(playerUI);
                UpdateMagicUI(playerUI);
                UpdateStaminaUI(playerUI);
                //check if player's level is a multiple of 2 and run spawn boss function in dungeon spawner
                if (level % 2 == 0)
                {
                    DungeonSpawner dungeonSpawner = FindAnyObjectByType<DungeonSpawner>();
                    dungeonSpawner.SpawnBossOnFloor(currentFloor);
                }
            }

            Debug.Log(
                $"PlayerStats: Leveled up to level {level}! Next level at {ExpToNextLevel} experience."
            );
        }

        public void GainCurrency(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Gain amount must be positive.");
                return;
            }

            currency += amount;
            // Debug.Log($"PlayerStats: Gained {amount} currency. Total currency: {currency}");
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
            // Debug.Log($"PlayerStats: Spent {amount} currency. Total currency: {currency}");
        }

        public void TakeDamage(int damage)
        {
            int effectiveDamage = Mathf.Max(damage - CurrentDefense, 1);
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            //OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            floatingTextManager?.ShowFloatingText(effectiveDamage.ToString(), transform, Color.red);

            //  Debug.Log($"PlayerStats: Took {effectiveDamage} damage. Current health: {CurrentHealth}/{MaxHealth}");

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
            //OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            // floatingTextManager?.ShowFloatingText($"+{amount}", transform, Color.green);

            Debug.Log(
                $"PlayerStats: Healed {amount} health. Current health: {CurrentHealth}/{MaxHealth}"
            );
        }

        public void GainMagic(int amount)
        {
            CurrentMagic = Mathf.Clamp(CurrentMagic + amount, 0, MaxMagic);
            UpdateMagicUI(FindAnyObjectByType<PlayerUI>());
            // Update magic UI
        }

        public void GainStamina(int amount)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + amount, 0, MaxStamina);
            UpdateStaminaUI(FindAnyObjectByType<PlayerUI>());
            // Update stamina UI
        }

        private void UpdateExperienceUI(PlayerUI playerUI)
        {
            if (playerUI != null)
            {
                playerUI.UpdateExperienceBar(CurrentExp, ExpToNextLevel);
            }
        }

        private void UpdateMagicUI(PlayerUI playerUI)
        {
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

        public int GetCurrentFloor()
        {
            return currentFloor;
        }

        private void HandleDeath()
        {
            // Debug.Log("PlayerStats: Player has died.");
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager?.OnPlayerDeath();
            OnPlayerDeath?.Invoke();
        }
    }
}
