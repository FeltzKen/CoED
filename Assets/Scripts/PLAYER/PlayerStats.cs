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
        public int stepCounter { get; private set; } = 0;
        public float restInterval = 3f;

        [Header("Health")]
        [SerializeField, Min(0)]
        private int baseHealth = 1000;
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        private float equipmentHealth = 0;

        [Header("Stamina")]
        [SerializeField, Min(0)]
        public int baseStamina = 100;
        public float CurrentStamina { get; set; }
        public float MaxStamina { get; set; }

        [Header("Magic")]
        [SerializeField, Min(0)]
        private int baseMagic = 100;
        public float CurrentMagic { get; set; }
        public float MaxMagic { get; set; }
        public float equipmentMagic = 0;

        [Header("Attack")]
        [SerializeField, Min(0)]
        private int baseAttack = 10;
        public float CurrentAttack { get; set; }
        private float equipmentAttack = 0;

        [SerializeField]
        public float AttackRange = 3f;

        [Header("Defense")]
        [SerializeField, Min(0)]
        private int baseDefense = 5;
        public float CurrentDefense { get; set; }
        private float equipmentDefense = 0;

        [Header("Range")]
        [SerializeField, Min(0f)]
        private float baseProjectileRange = 5f;
        public float CurrentProjectileRange { get; set; }
        public float targetingRange = 0f;
        private float equipmentRange = 0;

        [Header("Speed")]
        [SerializeField, Min(0)]
        private int baseSpeed = 5;
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

        [Header("Currency")]
        [SerializeField, Min(0)]
        private int currency = 0;

        [SerializeField]
        public int currentFloor = 1;
        private PlayerUI playerUI;
        public event Action OnPlayerDeath;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerStats instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy called.");
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
            MaxStamina = baseStamina + level * 5;
            MaxHealth = baseHealth + level * 20 + equipmentHealth;
            MaxMagic = baseMagic + level * 10 + equipmentMagic;
            CurrentAttack = baseAttack + (baseAttack * level * 0.2f) + equipmentAttack;
            CurrentDefense = baseDefense + (baseDefense * level * 0.15f) + equipmentDefense;
            CurrentProjectileRange = baseProjectileRange + level * 0.1f + equipmentRange;
            CurrentSpeed = baseSpeed + level * 0.05f;
            CurrentFireRate = baseFireRate - level * 0.02f;
            CurrentProjectileLifespan = baseProjectileLifespan + level * 0.05f;
            CurrentHealth = MaxHealth;
            CurrentMagic = MaxMagic;
            CurrentStamina = MaxStamina;
            restInterval -= 1 / level;

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
                return; // set experience bar color to black or something
            }
            CurrentExp += Mathf.Max(amount, 0);
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
            if (playerUI != null)
            {
                playerUI = FindAnyObjectByType<PlayerUI>();
                playerUI.UpdateLevelDisplay();
                UpdateHealthUI(playerUI);
                UpdateExperienceUI(playerUI);
                UpdateMagicUI(playerUI);
                UpdateStaminaUI(playerUI);
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
            FloatingTextManager.Instance.ShowFloatingText($"{amount} spent", transform, Color.red);
        }

        public void TakeDamage(float damage)
        {
            float effectiveDamage = damage - CurrentDefense;
            CurrentHealth = Mathf.Max(CurrentHealth - effectiveDamage, 0);
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());

            FloatingTextManager floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            floatingTextManager?.ShowFloatingText(effectiveDamage.ToString(), transform, Color.red);

            if (CurrentHealth <= 0)
            {
                HandleDeath();
            }
        }

        public void AddStep()
        {
            stepCounter++;
            if (stepCounter % 30 == 0)
            {
                Heal(10);
            }
            if (stepCounter % 100 == 0)
            {
                RefillMagic(10);
            }

            PlayerUI.Instance.UpdateStepCount();
        }

        public void Heal(float amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Heal amount must be positive.");
                return;
            }

            CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
            UpdateHealthUI(FindAnyObjectByType<PlayerUI>());

            Debug.Log(
                $"PlayerStats: Healed {amount} health. Current health: {CurrentHealth}/{MaxHealth}"
            );
        }

        public void ConsumeMagic(float amount)
        {
            CurrentMagic = Mathf.Max(CurrentMagic - amount, 0);
            PlayerUI.Instance.UpdateMagicBar(CurrentMagic, MaxMagic);
        }

        public void RefillMagic(int amount)
        {
            CurrentMagic = Mathf.Min(CurrentMagic + amount, MaxMagic);
            PlayerUI.Instance.UpdateMagicBar(CurrentMagic, MaxMagic);
        }

        public void IncreaseMaxMagic(float amount)
        {
            MaxMagic += amount;
            UpdateMagicUI(FindAnyObjectByType<PlayerUI>());
        }

        public void GainStamina(float amount)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + amount, 0, MaxStamina);
            UpdateStaminaUI(FindAnyObjectByType<PlayerUI>());
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
                playerUI.UpdateStaminaBar(CurrentStamina, MaxStamina);
            }
        }

        public int GetCurrentFloor()
        {
            return currentFloor;
        }

        private void HandleDeath()
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            gameManager?.OnPlayerDeath();
            OnPlayerDeath?.Invoke();
        }
    }
}
