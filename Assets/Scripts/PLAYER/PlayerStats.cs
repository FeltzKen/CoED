using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoED
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }
        private PlayerUI playerUI;

        // Experience & Leveling
        public int level = 1;
        private int currentExp = 0;
        private int expToNextLevel = 100;
        private int maxLevel = 50;

        public int stepCounter { get; private set; } = 0;
        public float restInterval = 3f;
        private float originalRestInterval; // To avoid repeatedly dividing by level

        private int currency = 275;
        public int currentFloor = 0;

        public bool HasEnteredDungeon { get; set; } = false;
        private bool invincible = false;
        public bool Invincible => invincible;
        private Dictionary<Stat, float> equipmentStats = new Dictionary<Stat, float>()
        {
            { Stat.HP, 0f },
            { Stat.Attack, 0f },
            { Stat.Intelligence, 0f },
            { Stat.Evasion, 0f },
            { Stat.Defense, 0f },
            { Stat.Dexterity, 0f },
            { Stat.Magic, 0f },
            { Stat.Accuracy, 0f },
            { Stat.FireRate, 0f },
            { Stat.ProjectileRange, 0f },
            { Stat.AttackRange, 0f },
            { Stat.Speed, 0f },
            { Stat.Shield, 0f },
            { Stat.Stamina, 0f },
            { Stat.ElementalDamage, 0f },
            { Stat.CritChance, 0f },
            { Stat.CritDamage, 0f },
            { Stat.ChanceToInflictStatusEffect, 0f },
        };
        private Dictionary<Stat, float> playerStats = new Dictionary<Stat, float>()
        {
            { Stat.HP, 0f },
            { Stat.MaxHP, 500f },
            { Stat.Attack, 10f },
            { Stat.Intelligence, 5f },
            { Stat.Evasion, 0f },
            { Stat.Defense, 5f },
            { Stat.Dexterity, 5f },
            { Stat.Magic, 0f },
            { Stat.MaxMagic, 100f },
            { Stat.Accuracy, 5f },
            { Stat.FireRate, 1f },
            { Stat.ProjectileRange, 5f },
            { Stat.AttackRange, 3f },
            { Stat.Speed, 5f },
            { Stat.Shield, 0f },
            { Stat.Stamina, 0f },
            { Stat.MaxStamina, 100f },
            { Stat.ElementalDamage, 5f },
            { Stat.CritChance, 5f },
            { Stat.CritDamage, 1.5f },
            { Stat.ChanceToInflictStatusEffect, 0.05f },
        };

        private Dictionary<DamageType, float> equipmentElementalDamage = new Dictionary<
            DamageType,
            float
        >()
        {
            { DamageType.Physical, 0f },
            { DamageType.Fire, 0f },
            { DamageType.Ice, 0f },
            { DamageType.Lightning, 0f },
            { DamageType.Poison, 0f },
            { DamageType.Arcane, 0f },
            { DamageType.Bleed, 0f },
            { DamageType.Holy, 0f },
            { DamageType.Shadow, 0f },
        };

        // Resistances, Weaknesses, Immunities
        [Header("Elemental Attributes & Statuses")]
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public List<ActiveWhileEquipped> EquippedmentEffects = new List<ActiveWhileEquipped>();
        public List<StatusEffectType> inflictableStatusEffects = new List<StatusEffectType>();
        public List<Weaknesses> activeWeaknesses = new List<Weaknesses>();
        public List<Resistances> activeResistances = new List<Resistances>();
        public List<Immunities> activeImmunities = new List<Immunities>();

        // Death event
        public event Action OnPlayerDeath;

        #region Public Stat Getters
        public float GetCurrentHealth() => playerStats[Stat.HP];

        public float GetCurrentMaxHealth() => playerStats[Stat.MaxHP];

        public float GetCurrentMagic() => playerStats[Stat.Magic];

        public float GetCurrentMaxMagic() => playerStats[Stat.MaxMagic];

        public float GetCurrentAccuracy() => playerStats[Stat.Accuracy];

        public float GetCurrentEvasion() => playerStats[Stat.Evasion];

        public float GetCurrentStamina() =>
            playerStats[Stat.Stamina] + equipmentStats[Stat.Stamina];

        public float GetCurrentMaxStamina() => playerStats[Stat.MaxStamina];

        public float GetCurrentAttack() => playerStats[Stat.Attack];

        public float GetCurrentDefense() =>
            playerStats[Stat.Defense] + equipmentStats[Stat.Defense];

        public float GetCurrentProjectileRange() => playerStats[Stat.ProjectileRange];

        public float GetCurrentSpeed() => playerStats[Stat.Speed];

        public float GetCurrentFireRate() => playerStats[Stat.FireRate];

        public float GetCurrentProjectileLifespan() => 5f;

        public float GetCurrentDexterity() => playerStats[Stat.Dexterity];

        public float GetCurrentIntelligence() => playerStats[Stat.Intelligence];

        public float GetCurrentCritChance() => playerStats[Stat.CritChance];

        public float GetCurrentAttackRange() => playerStats[Stat.AttackRange];

        public float GetCurrentChanceToInflictStatusEffect() =>
            playerStats[Stat.ChanceToInflictStatusEffect];

        public float GetCurrentBurnDamage() => equipmentElementalDamage[DamageType.Fire];

        public float GetCurrentIceDamage() => equipmentElementalDamage[DamageType.Ice];

        public float GetCurrentLightningDamage() => equipmentElementalDamage[DamageType.Lightning];

        public float GetCurrentPoisonDamage() => equipmentElementalDamage[DamageType.Poison];

        public float GetCurrentArcaneDamage() => equipmentElementalDamage[DamageType.Arcane];

        public float GetCurrentBleedDamage() => equipmentElementalDamage[DamageType.Bleed];

        public float GetCurrentHolyDamage() => equipmentElementalDamage[DamageType.Holy];

        public float GetCurrentShadowDamage() => equipmentElementalDamage[DamageType.Shadow];

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
                playerUI.SetHealthBarMax(GetCurrentHealth());
                UpdateExperienceUI();
                playerUI.SetMagicBarMax(GetCurrentMagic());
                playerUI.SetStaminaBarMax(GetCurrentStamina());
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
            // Reset base stats before calculations
            playerStats[Stat.MaxHP] =
                playerStats[Stat.MaxHP] + (level * 20) + equipmentStats[Stat.HP];

            playerStats[Stat.MaxMagic] =
                playerStats[Stat.MaxMagic] + (level * 10) + equipmentStats[Stat.Magic];

            playerStats[Stat.MaxStamina] =
                playerStats[Stat.MaxStamina] + (level * 5) + equipmentStats[Stat.Stamina];

            playerStats[Stat.Attack] =
                playerStats[Stat.Attack] + (level * 0.2f) + equipmentStats[Stat.Attack];

            playerStats[Stat.Defense] =
                playerStats[Stat.Defense]
                + (level * 0.1f)
                + equipmentStats[Stat.Defense]
                + playerStats[Stat.Shield];

            playerStats[Stat.Speed] =
                playerStats[Stat.Speed] + (level * 0.1f) + equipmentStats[Stat.Speed];

            playerStats[Stat.FireRate] = Mathf.Max(
                0.2f,
                1f / (1f + (level * 0.05f)) + equipmentStats[Stat.FireRate]
            );

            playerStats[Stat.ProjectileRange] =
                playerStats[Stat.ProjectileRange]
                + (level * 0.1f)
                + equipmentStats[Stat.ProjectileRange];

            playerStats[Stat.Dexterity] =
                playerStats[Stat.Dexterity] + (level * 0.1f) + equipmentStats[Stat.Dexterity];

            playerStats[Stat.Intelligence] =
                playerStats[Stat.Intelligence] + (level * 0.1f) + equipmentStats[Stat.Intelligence];

            playerStats[Stat.CritChance] =
                playerStats[Stat.CritChance] + (level * 0.1f) + equipmentStats[Stat.CritChance];

            playerStats[Stat.ChanceToInflictStatusEffect] =
                playerStats[Stat.ChanceToInflictStatusEffect]
                + (level * 0.05f)
                + equipmentStats[Stat.ChanceToInflictStatusEffect];

            // Ensure Rest Interval scales with level
            restInterval = originalRestInterval / Mathf.Max(level, 1);

            // Refill HP, Magic, and Stamina if requested
            if (refillResources)
            {
                playerStats[Stat.HP] = playerStats[Stat.MaxHP];
                playerStats[Stat.Magic] = playerStats[Stat.MaxMagic];
                playerStats[Stat.Stamina] = playerStats[Stat.MaxStamina];
            }

            InitializeUI();
            Debug.Log(
                $"Player stats: {string.Join(", ", playerStats.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}"
            );
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
                totalCritDamage = 0,
                totalAccuracy = 0,
                totalFireRate = 0,
                totalProjectileRange = 0,
                totalAttackRange = 0,
                totalElementalDamage = 0,
                totalChanceToInflictStatusEffect = 0,
                totalStatusEffectDuration = 0,
                totalShield = 0,
                totalEvasion = 0,
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
                totalAttack += Mathf.RoundToInt(item.equipmentStats[Stat.Attack]);
                totalDefense += Mathf.RoundToInt(item.equipmentStats[Stat.Defense]);
                totalHealth += Mathf.RoundToInt(item.equipmentStats[Stat.MaxHP]);
                totalMagic += Mathf.RoundToInt(item.equipmentStats[Stat.MaxMagic]);
                totalSpeed += Mathf.RoundToInt(item.equipmentStats[Stat.Speed]);
                totalDexterity += Mathf.RoundToInt(item.equipmentStats[Stat.Dexterity]);
                totalIntelligence += Mathf.RoundToInt(item.equipmentStats[Stat.Intelligence]);
                totalCritChance += Mathf.RoundToInt(item.equipmentStats[Stat.CritChance]);
                totalCritDamage += Mathf.RoundToInt(item.equipmentStats[Stat.CritDamage]);

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
            equipmentStats[Stat.Attack] = totalAttack;
            equipmentStats[Stat.Defense] = totalDefense;
            equipmentStats[Stat.MaxHP] = totalHealth;
            equipmentStats[Stat.MaxMagic] = totalMagic;
            equipmentStats[Stat.Speed] = totalSpeed;
            equipmentStats[Stat.Dexterity] = totalDexterity;
            equipmentStats[Stat.Intelligence] = totalIntelligence;
            equipmentStats[Stat.CritChance] = totalCritChance;
            equipmentStats[Stat.CritDamage] = totalCritDamage;
            equipmentStats[Stat.Accuracy] = totalAccuracy;
            equipmentStats[Stat.FireRate] = totalFireRate;
            equipmentStats[Stat.ProjectileRange] = totalProjectileRange;
            equipmentStats[Stat.AttackRange] = totalAttackRange;
            equipmentStats[Stat.ElementalDamage] = totalElementalDamage;
            equipmentStats[Stat.ChanceToInflictStatusEffect] = totalChanceToInflictStatusEffect;
            equipmentStats[Stat.StatusEffectDuration] = totalStatusEffectDuration;
            equipmentStats[Stat.Shield] = totalShield;
            equipmentStats[Stat.Evasion] = totalEvasion;

            equipmentElementalDamage[DamageType.Fire] = totalBurnDamage;
            equipmentElementalDamage[DamageType.Ice] = totalIceDamage;
            equipmentElementalDamage[DamageType.Lightning] = totalLightningDamage;
            equipmentElementalDamage[DamageType.Poison] = totalPoisonDamage;
            equipmentElementalDamage[DamageType.Arcane] = totalArcaneDamage;
            equipmentElementalDamage[DamageType.Bleed] = totalBleedDamage;
            equipmentElementalDamage[DamageType.Holy] = totalHolyDamage;
            equipmentElementalDamage[DamageType.Shadow] = totalShadowDamage;

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

            playerStats[Stat.Shield] += shieldValue;
            playerStats[Stat.Defense] += shieldValue; // Immediately buff defense
            Debug.Log($"Shield added: {shieldValue}. Current defense: {playerStats[Stat.Defense]}");
        }

        public void RemoveShield(float shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("PlayerStats: Shield value must be positive.");
                return;
            }

            float effectiveShieldRemoval = Mathf.Min(playerStats[Stat.Shield], shieldValue);
            playerStats[Stat.Shield] -= effectiveShieldRemoval;
            playerStats[Stat.Shield] -= effectiveShieldRemoval;

            Debug.Log(
                $"Shield removed: {effectiveShieldRemoval}. Current defense: {playerStats[Stat.Shield]}"
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
                    spell.LevelUpThreshold > 0
                    && level % spell.LevelUpThreshold == 0
                    && spell.SpellLevel < level / spell.LevelUpThreshold
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
        public void TakeDamage(
            DamageInfo damageInfo,
            float chanceToInflictStatusEffect = 0.05f,
            bool bypassInvincible = false
        )
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
            float effectiveDamage = Mathf.Max(totalDamage - playerStats[Stat.Defense], 1);

            // 4) Subtract HP
            playerStats[Stat.HP] = Mathf.Max(
                playerStats[Stat.HP] - Mathf.RoundToInt(effectiveDamage),
                0
            );

            // 5) Inflict any status effects
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                // If the effect is something the player can “inflict,” skip or handle differently;
                // but typically, we just add the effect to the player
                if (!inflictableStatusEffects.Contains(effect))
                {
                    if (Random.value < chanceToInflictStatusEffect)
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
            if (playerStats[Stat.HP] <= 0)
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
            if (EquippedmentEffects.Contains(ActiveWhileEquipped.ReviveOnce))
            {
                // Remove the effect from the manager so it doesn't trigger again
                StatusEffectManager.Instance.RemoveEquipmentEffects(ActiveWhileEquipped.ReviveOnce);

                // Restore partial HP
                playerStats[Stat.HP] = Mathf.RoundToInt(playerStats[Stat.MaxHP] * 0.25f);
                Debug.Log("ReviveOnce triggered! Player revived at 25% HP.");
                FloatingTextManager.Instance.ShowFloatingText("Revived!", transform, Color.green);
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
            playerStats[Stat.HP] = Mathf.Min(
                playerStats[Stat.HP] + amount,
                playerStats[Stat.MaxHP]
            );
            UpdateHealthUI();
        }

        public void ConsumeMagic(float amount)
        {
            playerStats[Stat.Magic] = Mathf.Max(playerStats[Stat.Magic] - amount, 0);
            UpdateMagicUI();
        }

        public void RefillMagic(float amount)
        {
            playerStats[Stat.Magic] = Mathf.Min(
                playerStats[Stat.Magic] + amount,
                playerStats[Stat.MaxMagic]
            );
            UpdateMagicUI();
        }

        public void IncreaseMaxMagic(float amount)
        {
            playerStats[Stat.MaxMagic] += Mathf.RoundToInt(amount);
            UpdateMagicUI();
        }

        public void RefillStamina(float amount)
        {
            playerStats[Stat.Stamina] = Mathf.Min(
                playerStats[Stat.Stamina] + amount,
                playerStats[Stat.MaxStamina]
            );
            UpdateStaminaUI();
        }

        public void DecreaseStamina(float amount)
        {
            playerStats[Stat.Stamina] = Mathf.RoundToInt(amount);
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

        public IEnumerator GainAttack(float amount, float duration)
        {
            playerStats[Stat.Attack] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.Attack] -= amount;
        }

        public IEnumerator GainDefense(float amount, float duration)
        {
            playerStats[Stat.Defense] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.Defense] -= amount;
        }

        public IEnumerator GainSpeed(float amount, float duration)
        {
            playerStats[Stat.Speed] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.Speed] -= amount;
        }

        public IEnumerator GainDexterity(float amount, float duration)
        {
            playerStats[Stat.Dexterity] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.Dexterity] -= amount;
        }

        public IEnumerator GainIntelligence(float amount, float duration)
        {
            playerStats[Stat.Intelligence] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.Intelligence] -= amount;
        }

        public IEnumerator GainCritChance(float amount, float duration)
        {
            playerStats[Stat.CritChance] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.CritChance] -= amount;
        }

        private void UpdateMagicUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateMagicBar(playerStats[Stat.Magic], playerStats[Stat.MaxMagic]);
            }
        }

        private void UpdateHealthUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateHealthBar(playerStats[Stat.HP], playerStats[Stat.MaxHP]);
            }
        }

        private void UpdateStaminaUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateStaminaBar(playerStats[Stat.Stamina], playerStats[Stat.MaxStamina]);
            }
        }
        #endregion
    }
}
