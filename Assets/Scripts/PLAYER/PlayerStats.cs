using System;
using System.Collections.Generic;
using System.Linq;
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
        private int currentExp = 0;

        [SerializeField, Min(1)]
        private int expToNextLevel = 100;

        [SerializeField, Min(1)]
        private int maxLevel = 50;

        // Steps & resting mechanics
        public int stepCounter { get; private set; } = 0;
        public float restInterval = 3f;
        private float originalRestInterval; // To avoid repeatedly dividing by level

        [Header("Health")]
        [SerializeField, Min(0)]
        private float baseHealth = 1000f;
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        private float equipmentHealth = 0f;

        private float currentShieldValue = 0f;
        private bool invincible = false;
        public bool Invincible => invincible;

        [Header("Stamina")]
        [SerializeField, Min(0)]
        private int baseStamina = 100;
        public int CurrentStamina { get; private set; }
        public int MaxStamina { get; private set; }

        [Header("Magic")]
        [SerializeField, Min(0)]
        private int baseMagic = 100;
        public float CurrentMagic { get; private set; }
        public int MaxMagic { get; private set; }
        private float equipmentMagic = 0f;

        [Header("Attack")]
        [SerializeField, Min(0)]
        private int baseAttack = 10;
        public float CurrentAttack { get; private set; }
        private float equipmentAttack = 0f;

        [SerializeField]
        public float AttackRange = 3f;

        [Header("Defense")]
        [SerializeField, Min(0)]
        private int baseDefense = 5;
        public float CurrentDefense { get; private set; }
        private float equipmentDefense = 0f;

        [Header("Range")]
        [SerializeField, Min(0f)]
        private float baseProjectileRange = 5f;
        public float CurrentProjectileRange { get; private set; }
        public float targetingRange = 0f;
        private float equipmentRange = 0f;

        [Header("Dexterity")]
        [SerializeField, Min(0)]
        private int baseDexterity = 5;
        public float CurrentDexterity { get; private set; }
        private float equipmentDexterity = 0f;

        [Header("Intelligence")]
        [SerializeField, Min(0)]
        private int baseIntelligence = 5;
        public float CurrentIntelligence { get; private set; }
        private float equipmentIntelligence = 0f;

        [Header("Crit Chance")]
        [SerializeField, Min(0)]
        private int baseCritChance = 5;
        public float CurrentCritChance { get; private set; }
        private float equipmentCritChance = 0f;

        [Header("Speed")]
        [SerializeField, Min(0)]
        private int baseSpeed = 5;
        public float CurrentSpeed { get; private set; }
        private float equipmentSpeed = 0f;

        [Header("Equipment Elemental Damage (Read-Only Summary)")]
        [SerializeField]
        private Dictionary<DamageType, float> equipmentElementalDamage =
            new Dictionary<DamageType, float>();
        private float burnDamage = 0f;
        private float iceDamage = 0f;
        private float lightningDamage = 0f;
        private float poisonDamage = 0f;
        private float arcaneDamage = 0f;
        private float bleedDamage = 0f;
        private float holyDamage = 0f;
        private float shadowDamage = 0f;

        [Header("Fire Rate")]
        [SerializeField, Min(0f)]
        private float baseFireRate = 1f;
        public float CurrentFireRate { get; private set; }

        [Header("Projectile Lifespan")]
        [SerializeField, Min(0f)]
        private float baseProjectileLifespan = 2f;
        public float CurrentProjectileLifespan { get; private set; }

        [Header("Dungeon & UI")]
        public bool HasEnteredDungeon { get; set; } = false;

        [SerializeField, Min(0)]
        private int currency = 0;
        public int currentFloor = 0;
        private PlayerUI playerUI;

        // Resistances, Weaknesses, Immunities
        [Header("Elemental Attributes & Statuses")]
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public List<StatusEffectType> inflictableStatusEffects = new List<StatusEffectType>();
        public List<Weaknesses> activeWeaknesses = new List<Weaknesses>();
        public List<Resistances> activeResistances = new List<Resistances>();
        public List<Immunities> activeImmunities = new List<Immunities>();
        private float chanceToInflictStatusEffect = 0.1f;
        public float CurrentChanceToInflictStatusEffect { get; private set; }

        // Death event
        public event Action OnPlayerDeath;

        #region Public Stat Getters
        public float GetCurrentHealth() => CurrentHealth;

        public float GetCurrentMagic() => CurrentMagic;

        public float GetCurrentStamina() => CurrentStamina;

        public float GetCurrentAttack() => CurrentAttack;

        public float GetCurrentDefense() => CurrentDefense;

        public float GetCurrentProjectileRange() => CurrentProjectileRange;

        public float GetCurrentSpeed() => CurrentSpeed;

        public float GetCurrentFireRate() => CurrentFireRate;

        public float GetCurrentProjectileLifespan() => CurrentProjectileLifespan;

        public float GetCurrentDexterity() => CurrentDexterity;

        public float GetCurrentIntelligence() => CurrentIntelligence;

        public float GetCurrentCritChance() => CurrentCritChance;

        public float GetCurrentBurnDamage() => burnDamage;

        public float GetCurrentIceDamage() => iceDamage;

        public float GetCurrentLightningDamage() => lightningDamage;

        public float GetCurrentPoisonDamage() => poisonDamage;

        public float GetCurrentArcaneDamage() => arcaneDamage;

        public float GetCurrentBleedDamage() => bleedDamage;

        public float GetCurrentHolyDamage() => holyDamage;

        public float GetCurrentShadowDamage() => shadowDamage;

        public string GetStatusEffects() => string.Join(", ", activeStatusEffects);

        public string GetWeaknesses() => string.Join(", ", activeWeaknesses);

        public string GetResistances() => string.Join(", ", activeResistances);
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // Store original rest interval so we don’t keep dividing by level
                originalRestInterval = restInterval;
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
            CalculateStats(refillResources: true);
        }

        #endregion

        #region Initialization / UI
        private void InitializeUI()
        {
            // Attempt to find the UI if not already assigned
            if (playerUI == null)
                playerUI = FindAnyObjectByType<PlayerUI>();

            if (playerUI != null)
            {
                playerUI.SetHealthBarMax(CurrentHealth);
                UpdateExperienceUI();
                playerUI.SetMagicBarMax(CurrentMagic);
                playerUI.SetStaminaBarMax(CurrentStamina);
                playerUI.UpdateLevelDisplay();
            }
        }

        #endregion

        #region Stat Calculation
        /// <summary>
        /// Recalculate all derived stats based on base stats, level, equipment, etc.
        /// </summary>
        /// <param name="refillResources">If true, refills Health, Magic, and Stamina to max.</param>
        public void CalculateStats(bool refillResources = true)
        {
            // Attack, scaled with level & equipment
            CurrentAttack =
                baseAttack + Mathf.RoundToInt((baseAttack * level * 0.2f) + equipmentAttack);

            // Stamina & Health & Magic
            MaxStamina = baseStamina + level * 5;
            MaxHealth = Mathf.RoundToInt(baseHealth + level * 20 + equipmentHealth);
            MaxMagic = Mathf.RoundToInt(baseMagic + level * 10 + equipmentMagic);

            // Defense (inc. shield)
            CurrentDefense =
                baseDefense
                + Mathf.RoundToInt((baseDefense * level * 0.1f) + equipmentDefense)
                + currentShieldValue;

            // Projectile range
            CurrentProjectileRange = baseProjectileRange + level * 0.1f + equipmentRange;

            // Speed
            CurrentSpeed = baseSpeed + level * 0.05f + equipmentSpeed;

            // FireRate (higher level => slower is subtracted, so net is "faster"?)
            CurrentFireRate = Mathf.Max(0.1f, baseFireRate - level * 0.02f);

            // Lifespan
            CurrentProjectileLifespan = baseProjectileLifespan + level * 0.05f;

            // Dexterity & Intelligence
            CurrentDexterity = baseDexterity + level * 0.1f + equipmentDexterity;
            CurrentIntelligence = baseIntelligence + level * 0.1f + equipmentIntelligence;

            // Crit Chance
            CurrentCritChance = baseCritChance + level * 0.1f + equipmentCritChance;
            CurrentChanceToInflictStatusEffect = chanceToInflictStatusEffect + level * 0.1f;
            // Avoid repeatedly dividing restInterval by level
            // Instead, we do a base calculation from the original restInterval if you want it to get faster
            restInterval = originalRestInterval / Mathf.Max(level, 1);

            // Refill HP/MP/Stamina if needed
            if (refillResources)
            {
                CurrentHealth = MaxHealth;
                CurrentMagic = MaxMagic;
                CurrentStamina = MaxStamina;
            }

            InitializeUI();
        }

        /// <summary>
        /// Collates equipment stats from all equipped items and recalculates final stats.
        /// </summary>
        public void SetEquipmentStats(List<Equipment> equippedItems)
        {
            // We'll sum up each stat from the items
            int totalAttack = 0,
                totalDefense = 0,
                totalHealth = 0,
                totalMagic = 0,
                totalSpeed = 0,
                totalDexterity = 0,
                totalIntelligence = 0,
                totalCritChance = 0,
                totalBurnDamage = 0,
                totalIceDamage = 0,
                totalLightningDamage = 0,
                totalPoisonDamage = 0,
                totalArcaneDamage = 0,
                totalBleedDamage = 0,
                totalHolyDamage = 0,
                totalShadowDamage = 0;

            foreach (var item in equippedItems)
            {
                totalAttack += Mathf.RoundToInt(item.attack);
                totalDefense += Mathf.RoundToInt(item.defense);
                totalHealth += Mathf.RoundToInt(item.health);
                totalMagic += Mathf.RoundToInt(item.magic);
                totalSpeed += Mathf.RoundToInt(item.speed);
                totalDexterity += Mathf.RoundToInt(item.dexterity);
                totalIntelligence += Mathf.RoundToInt(item.intelligence);
                totalCritChance += Mathf.RoundToInt(item.critChance);

                totalBurnDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Fire, 0)
                );
                totalIceDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Ice, 0)
                );
                totalLightningDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Lightning, 0)
                );
                totalPoisonDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Poison, 0)
                );
                totalArcaneDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Arcane, 0)
                );
                totalBleedDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Bleed, 0)
                );
                totalHolyDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Holy, 0)
                );
                totalShadowDamage += Mathf.RoundToInt(
                    item.damageModifiers.GetValueOrDefault(DamageType.Shadow, 0)
                );
            }

            // Store these in our local "equipment" variables
            equipmentAttack = totalAttack;
            equipmentDefense = totalDefense;
            equipmentHealth = totalHealth;
            equipmentMagic = totalMagic;
            equipmentSpeed = totalSpeed;
            equipmentDexterity = totalDexterity;
            equipmentIntelligence = totalIntelligence;
            equipmentCritChance = totalCritChance;

            burnDamage = totalBurnDamage;
            iceDamage = totalIceDamage;
            lightningDamage = totalLightningDamage;
            poisonDamage = totalPoisonDamage;
            arcaneDamage = totalArcaneDamage;
            bleedDamage = totalBleedDamage;
            holyDamage = totalHolyDamage;
            shadowDamage = totalShadowDamage;

            // Recalculate final stats without refilling resources
            CalculateStats(refillResources: false);
        }

        #endregion

        #region Shields & Invincibility
        public void AddShield(int shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("PlayerStats: Shield value must be positive.");
                return;
            }

            currentShieldValue += shieldValue;
            CurrentDefense += shieldValue; // Immediately buff defense
            Debug.Log($"Shield added: {shieldValue}. Current defense: {CurrentDefense}");
        }

        public void RemoveShield(float shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("PlayerStats: Shield value must be positive.");
                return;
            }

            float effectiveShieldRemoval = Mathf.Min(currentShieldValue, shieldValue);
            currentShieldValue -= effectiveShieldRemoval;
            CurrentDefense -= effectiveShieldRemoval;

            Debug.Log(
                $"Shield removed: {effectiveShieldRemoval}. Current defense: {CurrentDefense}"
            );
        }

        public void SetInvincible(bool value)
        {
            invincible = value;
        }
        #endregion

        #region XP & Leveling
        public void GainExperience(int amount)
        {
            if (level >= maxLevel)
                return;

            currentExp += Mathf.Max(amount, 0);
            UpdateExperienceUI();

            while (currentExp >= expToNextLevel && level < maxLevel)
            {
                currentExp -= expToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            LevelUpSpells();

            if (playerUI == null)
                playerUI = FindAnyObjectByType<PlayerUI>();

            if (playerUI != null)
            {
                playerUI.UpdateLevelDisplay();
                expToNextLevel = Mathf.CeilToInt(expToNextLevel * 1.25f);

                UpdateHealthUI();
                UpdateExperienceUI();
                UpdateMagicUI();
                UpdateStaminaUI();

                CalculateStats(refillResources: true);

                // Possibly spawn a boss every 2 levels
                if (level % 2 == 0)
                {
                    var dungeonSpawner = FindAnyObjectByType<DungeonSpawner>();
                    dungeonSpawner?.SpawnBossOnFloor(currentFloor);
                }
            }

            Debug.Log(
                $"PlayerStats: Leveled up to level {level}! Next level at {expToNextLevel} EXP."
            );
        }

        private void LevelUpSpells()
        {
            var spells = PlayerSpellCaster.Instance.GetKnownSpells();
            foreach (var spell in spells)
            {
                if (
                    spell.BaseSpell.levelUpThreshold > 0
                    && level % spell.BaseSpell.levelUpThreshold == 0
                    && spell.BaseSpell.spellLevel < level / spell.BaseSpell.levelUpThreshold
                )
                {
                    spell.LevelUp();
                }
            }
        }
        #endregion

        #region Currency
        public void GainCurrency(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("PlayerStats: Gain amount must be positive.");
                return;
            }
            currency += amount;
        }

        public int GetCurrency() => currency;

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
        #endregion

        #region Damage / Status Effect Handling
        /// <summary>
        /// Handles complex damage calculations and dynamic status effects.
        /// </summary>
        /// <param name="damageInfo">Damage package with typed damage and inflicted statuses.</param>
        /// <param name="bypassInvincible">If true, ignore the invincible flag.</param>
        public void TakeDamage(DamageInfo damageInfo, bool bypassInvincible = false)
        {
            // 1) Invincibility check
            if (!bypassInvincible && invincible)
            {
                Debug.Log("Player is invincible. No damage taken.");
                return;
            }

            float totalDamage = 0f;

            foreach (var damageEntry in damageInfo.DamageAmounts)
            {
                DamageType type = damageEntry.Key;
                float amount = damageEntry.Value;

                if (type == DamageType.Physical)
                {
                    // Physical always applies
                    totalDamage += amount;
                }
                else
                {
                    // Immunity check
                    if (activeImmunities.Contains((Immunities)type))
                    {
                        Debug.Log($"Player is immune to {type} damage.");
                        FloatingTextManager.Instance.ShowFloatingText(
                            $"{type.ToString().ToUpper()} IMMUNE",
                            transform,
                            Color.cyan
                        );
                        continue;
                    }
                    // Resistance (halve damage)
                    if (activeResistances.Contains((Resistances)type))
                    {
                        amount *= 0.5f;
                        Debug.Log($"Player resisted {type}. Reduced damage => {amount}");
                    }
                    // Weakness (increase damage by 50%)
                    if (activeWeaknesses.Contains((Weaknesses)type))
                    {
                        amount *= 1.5f;
                        Debug.Log($"Player is weak to {type}. Increased damage => {amount}");
                    }

                    totalDamage += amount;
                }
            }

            // 2) Reflect check (if we want immediate reflection)
            // If the player has "DamageReflect" in activeStatusEffects, do partial reflection
            if (activeStatusEffects.Contains(StatusEffectType.DamageReflect))
            {
                float reflectAmount = totalDamage * 0.15f; // 15% reflection example
                // We would need an "attacker" reference to actually damage them
                // For now, we just log it
                Debug.Log($"Reflected {reflectAmount} damage back to attacker!");
            }

            // 3) Defense application, never below 1
            float effectiveDamage = Mathf.Max(totalDamage - CurrentDefense, 1);

            // 4) Subtract HP
            CurrentHealth = Mathf.Max(CurrentHealth - Mathf.RoundToInt(effectiveDamage), 0);

            // 5) Inflict any status effects
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                // If the effect is something the player can “inflict,” skip or handle differently;
                // but typically, we just add the effect to the player
                if (!inflictableStatusEffects.Contains(effect))
                {
                    StatusEffectManager.Instance.AddStatusEffect(gameObject, effect);
                }
            }

            // 6) UI feedback
            UpdateHealthUI();
            FloatingTextManager.Instance.ShowFloatingText(
                effectiveDamage.ToString(),
                transform,
                Color.red
            );

            // 7) Check death
            if (CurrentHealth <= 0)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// Called whenever the player's Health hits 0 or below.
        /// </summary>
        private void HandleDeath()
        {
            // Check for "ReviveOnce"
            if (activeStatusEffects.Contains(StatusEffectType.ReviveOnce))
            {
                // Remove the effect from the manager so it doesn't trigger again
                StatusEffectManager.Instance.RemoveSpecificEffect(
                    gameObject,
                    StatusEffectType.ReviveOnce
                );

                // Restore partial HP
                CurrentHealth = Mathf.RoundToInt(MaxHealth * 0.25f);
                Debug.Log("ReviveOnce triggered! Player revived at 25% HP.");
                return;
            }

            // Normal death flow
            var gameManager = FindAnyObjectByType<GameManager>();
            gameManager?.OnPlayerDeath();
            OnPlayerDeath?.Invoke();
        }
        #endregion

        #region Steps & Misc
        public void AddStep()
        {
            stepCounter++;
            // Example: heal every 30 steps
            if (stepCounter % 30 == 0)
            {
                Heal(10);
            }
            // Example: restore magic every 100 steps
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
                Debug.LogWarning("Heal amount must be positive.");
                return;
            }
            CurrentHealth = Mathf.Min(CurrentHealth + Mathf.RoundToInt(amount), MaxHealth);
            UpdateHealthUI();
            Debug.Log($"Player healed {amount}. Current: {CurrentHealth}/{MaxHealth}");
        }

        public void ConsumeMagic(float amount)
        {
            CurrentMagic = Mathf.Max(CurrentMagic - amount, 0);
            UpdateMagicUI();
        }

        public void RefillMagic(float amount)
        {
            CurrentMagic = Mathf.Min(CurrentMagic + amount, MaxMagic);
            UpdateMagicUI();
        }

        public void IncreaseMaxMagic(float amount)
        {
            MaxMagic += Mathf.RoundToInt(amount);
            UpdateMagicUI();
        }

        public void GainStamina(float amount)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + Mathf.RoundToInt(amount), 0, MaxStamina);
            UpdateStaminaUI();
        }

        public void DecreaseStamina(float amount)
        {
            CurrentStamina = Mathf.RoundToInt(amount);
            UpdateStaminaUI();
        }

        public int GetCurrentFloor() => currentFloor;
        #endregion

        #region UI Updates
        private void UpdateExperienceUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateExperienceBar(currentExp, expToNextLevel);
            }
        }

        private void UpdateMagicUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateMagicBar(CurrentMagic, MaxMagic);
            }
        }

        private void UpdateHealthUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateHealthBar(CurrentHealth, MaxHealth);
            }
        }

        private void UpdateStaminaUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateStaminaBar(CurrentStamina, MaxStamina);
            }
        }
        #endregion
    }
}
