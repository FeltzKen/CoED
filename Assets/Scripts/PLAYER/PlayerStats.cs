using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoED
{
    public class PlayerStats : MonoBehaviour, IEntityStats, IHasImmunities
    {
        public static PlayerStats Instance { get; private set; }
        private PlayerUI playerUI;

        #region Experience & Leveling
        public int level = 1;
        private int currentExp = 0;
        private int expToNextLevel = 100;
        private int maxLevel = 50;
        #endregion

        #region Core Resources & Floor
        public int currency = 275;
        public int currentFloor = 0;
        public int restInterval = 3;
        public int steps;
        public bool HasEnteredDungeon = false;
        #endregion

        #region Base Stats & Equipment Modifiers
        // These dictionaries hold the unmodified base stats, the bonuses from equipment, and the final computed stats.
        // It is assumed that baseStats never change once set (except on level-up) while equipmentStats is recalculated when gear is equipped/unequipped.
        public Dictionary<Stat, float> baseStats = new Dictionary<Stat, float>();

        // Equipment contributions are summed here.
        private Dictionary<Stat, float> equipmentStats = new Dictionary<Stat, float>()
        {
            // Initialize every stat to zero so additions work properly.
            { Stat.HP, 0f },
            { Stat.MaxHP, 0f },
            { Stat.Attack, 0f },
            { Stat.Intelligence, 0f },
            { Stat.Defense, 0f },
            { Stat.Dexterity, 0f },
            { Stat.Magic, 0f },
            { Stat.MaxMagic, 0f },
            { Stat.FireRate, 0f },
            { Stat.ProjectileRange, 0f },
            { Stat.AttackRange, 0f },
            { Stat.Speed, 0f },
            { Stat.Shield, 0f },
            { Stat.Stamina, 0f },
            { Stat.MaxStamina, 0f },
            { Stat.ElementalDamage, 0f },
            { Stat.CritChance, 0f },
            { Stat.CritDamage, 0f },
            { Stat.ChanceToInflict, 0f },
        };

        // Final computed stats = baseStats + level bonuses + equipmentStats.
        private Dictionary<Stat, float> playerStats = new Dictionary<Stat, float>();

        // For elemental damage modifiers from gear.
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
            { DamageType.Heal, 0f },
            { DamageType.Nature, 0f },
        };
        #endregion

        #region Resistances, Weaknesses, Immunities & Status Effects
        [Header("Elemental Attributes & Statuses")]
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public List<StatusEffectType> EquipmentEffects = new List<StatusEffectType>();
        public List<StatusEffectType> inflictableStatusEffects = new List<StatusEffectType>();
        public List<Weaknesses> activeWeaknesses = new List<Weaknesses>();
        public List<Resistances> activeResistances = new List<Resistances>();
        public List<Immunities> activeImmunities = new List<Immunities>();
        #endregion
        private Dictionary<Stat, Coroutine> activeNullifications =
            new Dictionary<Stat, Coroutine>();

        #region Other
        public bool invincible { get; private set; } = false;
        public bool silenced { get; private set; } = false;
        public event Action OnPlayerDeath;
        #endregion

        #region Unity Lifecycle
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
                return;
            }
        }

        private void Start()
        {
            // If a class was selected, override the default base stats.
            if (GameManager.SelectedClass != null)
            {
                // Replace the default baseStats with the selected class's stats.
                baseStats = new Dictionary<Stat, float>(GameManager.SelectedClass.BaseStats);
            }

            // Then, continue with the normal initialization.
            //CopyBaseToPlayerStats();
            //CalculateStats(refillResources: true);
        }

        #endregion

        #region Stat Getters (For UI Display)
        public float GetCurrentHealth() => playerStats[Stat.HP];

        public float GetCurrentMaxHealth() => playerStats[Stat.MaxHP];

        public float GetCurrentMagic() => playerStats[Stat.Magic];

        public float GetCurrentMaxMagic() => playerStats[Stat.MaxMagic];

        public float GetCurrentStamina() => playerStats[Stat.Stamina];

        public float GetCurrentMaxStamina() => playerStats[Stat.MaxStamina];

        public float GetCurrentAttack() => playerStats[Stat.Attack];

        public float GetCurrentDefense() => playerStats[Stat.Defense];

        public float GetCurrentProjectileRange() => playerStats[Stat.ProjectileRange];

        public float GetCurrentSpeed() => playerStats[Stat.Speed];

        public float GetCurrentFireRate() => playerStats[Stat.FireRate];

        public float GetCurrentDexterity() => playerStats[Stat.Dexterity];

        public float GetCurrentIntelligence() => playerStats[Stat.Intelligence];

        public float GetCurrentCritChance() => playerStats[Stat.CritChance];

        public float GetCurrentAttackRange() => playerStats[Stat.AttackRange];

        public float GetCurrentChanceToInflict() => playerStats[Stat.ChanceToInflict];

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

        public float GetCurrentStatusEffectDuration() => playerStats[Stat.StatusEffectDuration];
        #endregion

        #region Equipment Stats Management
        /// <summary>
        /// Adds the stat bonuses from an equipment item.
        /// </summary>
        public void AddEquipmentStats(Dictionary<Stat, float> statsToAdd)
        {
            foreach (var kvp in statsToAdd)
            {
                if (!equipmentStats.ContainsKey(kvp.Key))
                    equipmentStats[kvp.Key] = 0;
                equipmentStats[kvp.Key] += kvp.Value;
            }
        }

        /// <summary>
        /// Removes the stat bonuses from an equipment item.
        /// </summary>
        public void RemoveEquipmentStats(Dictionary<Stat, float> statsToRemove)
        {
            foreach (var kvp in statsToRemove)
            {
                if (equipmentStats.ContainsKey(kvp.Key))
                    equipmentStats[kvp.Key] -= kvp.Value;
            }
        }

        public void AddDamageModifier(DamageType type, float value)
        {
            if (!equipmentElementalDamage.ContainsKey(type))
                equipmentElementalDamage[type] = 0;
            equipmentElementalDamage[type] += value;
        }

        public void RemoveDamageModifier(DamageType type, float value)
        {
            if (equipmentElementalDamage.ContainsKey(type))
                equipmentElementalDamage[type] -= value;
        }

        #endregion

        #region Stat Calculation
        /// <summary>
        /// Copies baseStats into playerStats (for re-calculation).
        /// </summary>
        public void CopyBaseToPlayerStats()
        {
            playerStats = new Dictionary<Stat, float>(baseStats);
        }

        /// <summary>
        /// Recalculates final player stats by combining baseStats, level-based scaling, and equipmentStats.
        /// If refillResources is true, fills HP, Magic, and Stamina to their maximums.
        /// </summary>
        public void CalculateStats(bool refillResources = false)
        {
            // Start with a fresh copy of base stats.
            CopyBaseToPlayerStats();

            // Level-based bonuses (example formulas; adjust as needed)
            playerStats[Stat.MaxHP] += level * 20;
            playerStats[Stat.MaxMagic] += level * 10;
            playerStats[Stat.MaxStamina] += level * 5;
            playerStats[Stat.Attack] += level * 0.2f;
            playerStats[Stat.Defense] += level * 0.1f;
            playerStats[Stat.Speed] += level * 0.1f;
            playerStats[Stat.ProjectileRange] += level * 0.1f;
            playerStats[Stat.Dexterity] += level * 0.1f;
            playerStats[Stat.Intelligence] += level * 0.1f;
            playerStats[Stat.CritChance] += level * 0.1f;
            playerStats[Stat.ChanceToInflict] += level * 0.05f;

            // Add equipment bonuses
            foreach (var stat in equipmentStats)
            {
                if (playerStats.ContainsKey(stat.Key))
                    playerStats[stat.Key] += stat.Value;
                else
                    playerStats[stat.Key] = stat.Value;
            }

            if (refillResources)
            {
                playerStats[Stat.HP] = playerStats[Stat.MaxHP];
                playerStats[Stat.Magic] = playerStats[Stat.MaxMagic];
                playerStats[Stat.Stamina] = playerStats[Stat.MaxStamina];
            }

            // Update UI elements.
            InitializeUI();
            Debug.Log(
                $"Player stats: {string.Join(", ", playerStats.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}"
            );
        }
        #endregion

        #region UI Initialization & Updates
        private void InitializeUI()
        {
            if (playerUI == null)
                playerUI = FindAnyObjectByType<PlayerUI>();

            if (playerUI != null)
            {
                playerUI.SetHealthBarMax(GetCurrentMaxHealth());
                playerUI.SetMagicBarMax(GetCurrentMaxMagic());
                playerUI.SetStaminaBarMax(GetCurrentMaxStamina());
                playerUI.UpdateLevelDisplay();
                UpdateExperienceUI();
            }
        }

        private void UpdateExperienceUI()
        {
            if (playerUI != null)
            {
                playerUI.UpdateExperienceBar(currentExp, expToNextLevel);
            }
        }

        private void UpdateHealthUI()
        {
            if (playerUI != null)
                playerUI.UpdateHealthBar(GetCurrentHealth(), GetCurrentMaxHealth());
        }

        private void UpdateMagicUI()
        {
            if (playerUI != null)
                playerUI.UpdateMagicBar(GetCurrentMagic(), GetCurrentMaxMagic());
        }

        private void UpdateStaminaUI()
        {
            if (playerUI != null)
                playerUI.UpdateStaminaBar(GetCurrentStamina(), GetCurrentMaxStamina());
        }
        #endregion

        #region IEntity Implementation
        public void ModifyStat(Stat stat, float value)
        {
            if (playerStats.ContainsKey(stat))
                playerStats[stat] += value;
            else
                playerStats[stat] = value;

            Debug.Log($"[PlayerStats] {stat} modified by {value}. New Value: {playerStats[stat]}");
        }

        public void NullStat(Stat stat, float duration)
        {
            if (activeNullifications.ContainsKey(stat))
            {
                StopCoroutine(activeNullifications[stat]);
                activeNullifications.Remove(stat);
            }

            activeNullifications[stat] = StartCoroutine(NullifyStat(stat, duration));
        }

        private IEnumerator NullifyStat(Stat stat, float duration)
        {
            if (!playerStats.ContainsKey(stat))
                yield break;
            float originalValue = playerStats[stat];
            playerStats[stat] = 0;
            yield return new WaitForSeconds(duration);
            playerStats[stat] = originalValue;
            activeNullifications.Remove(stat);
        }

        public void AddResistance(Resistances res)
        {
            if (!activeResistances.Contains(res))
                activeResistances.Add(res);
        }

        public void RemoveResistance(Resistances res)
        {
            activeResistances.Remove(res);
        }

        public void AddWeakness(Weaknesses weak)
        {
            if (!activeWeaknesses.Contains(weak))
                activeWeaknesses.Add(weak);
        }

        public void RemoveWeakness(Weaknesses weak)
        {
            activeWeaknesses.Remove(weak);
        }

        public void AddImmunity(Immunities imm)
        {
            if (!activeImmunities.Contains(imm))
                activeImmunities.Add(imm);
        }

        public void RemoveImmunity(Immunities imm)
        {
            activeImmunities.Remove(imm);
        }

        public void AddActiveStatusEffect(StatusEffectType status)
        {
            if (!activeStatusEffects.Contains(status))
                activeStatusEffects.Add(status);
        }

        public void RemoveActiveStatusEffect(StatusEffectType status)
        {
            activeStatusEffects.Remove(status);
        }

        public void AddInflictableStatusEffect(StatusEffectType status)
        {
            if (!inflictableStatusEffects.Contains(status))
                inflictableStatusEffects.Add(status);

            return;
        }

        public void RemoveInflictableStatusEffect(StatusEffectType status)
        {
            inflictableStatusEffects.Remove(status);
        }

        public void AddShield(float shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("PlayerStats: Shield value must be positive.");
                return;
            }

            playerStats[Stat.Shield] += shieldValue;
            playerStats[Stat.Defense] += shieldValue; // Buff defense immediately.
            Debug.Log($"Shield added: {shieldValue}. Current Defense: {playerStats[Stat.Defense]}");
        }

        public void RemoveShield(float shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("PlayerStats: Shield value must be positive.");
                return;
            }

            float effectiveRemoval = Mathf.Min(playerStats[Stat.Shield], shieldValue);
            playerStats[Stat.Shield] -= effectiveRemoval;
            playerStats[Stat.Defense] -= effectiveRemoval;
            Debug.Log(
                $"Shield removed: {effectiveRemoval}. Current Defense: {playerStats[Stat.Defense]}"
            );
        }

        public void SetInvincible(bool value)
        {
            invincible = value;
        }

        public bool HasStatusEffect(StatusEffectType effect)
        {
            return activeStatusEffects.Contains(effect);
        }

        public void IsSilenced(bool state)
        {
            silenced = state;
        }

        public void TakeDamage(
            DamageInfo damageInfo,
            float statusChance = 0.05f,
            bool bypassInvincible = false,
            float effectDuration = 0f
        )
        {
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

                if (type != DamageType.Physical)
                {
                    if (activeImmunities.Contains((Immunities)type))
                    {
                        FloatingTextManager.Instance.ShowFloatingText(
                            $"{type.ToString().ToUpper()} IMMUNE",
                            transform,
                            Color.cyan
                        );
                        continue;
                    }
                    if (activeResistances.Contains((Resistances)type))
                    {
                        amount *= 0.5f;
                        Debug.Log($"Resisted {type}: {amount}");
                    }
                    if (activeWeaknesses.Contains((Weaknesses)type))
                    {
                        amount *= 1.5f;
                        Debug.Log($"Weakness to {type}: {amount}");
                    }
                }
                totalDamage += amount;
            }

            float effectiveDamage = Mathf.Max(totalDamage - playerStats[Stat.Defense], 1);
            playerStats[Stat.HP] = Mathf.Max(
                playerStats[Stat.HP] - Mathf.RoundToInt(effectiveDamage),
                0
            );

            // Inflict status effects if applicable.
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                if (!activeStatusEffects.Contains(effect) && Random.value < statusChance)
                {
                    StatusEffectManager.Instance.AddStatusEffect(
                        gameObject,
                        effect,
                        playerStats[Stat.StatusEffectDuration]
                    );
                }
            }

            UpdateHealthUI();
            FloatingTextManager.Instance.ShowFloatingText(
                effectiveDamage.ToString(),
                transform,
                Color.red
            );

            if (playerStats[Stat.HP] <= 0)
                HandleDeath();
        }

        public void TakeEffectDamage(DamageInfo damageInfo)
        {
            if (activeResistances.Contains((Resistances)damageInfo.DamageAmounts.Values.First()))
            {
                FloatingTextManager.Instance.ShowFloatingText("RESISTED", transform, Color.green);
                playerStats[Stat.HP] -= damageInfo.DamageAmounts.Values.Last() / 2;
                return;
            }

            if (activeWeaknesses.Contains((Weaknesses)damageInfo.DamageAmounts.Keys.First()))
            {
                FloatingTextManager.Instance.ShowFloatingText("WEAKNESS", transform, Color.red);
                playerStats[Stat.HP] -= damageInfo.DamageAmounts.Values.Last() * 2;
                return;
            }

            playerStats[Stat.HP] -= damageInfo.DamageAmounts.Values.Last();

            if (playerStats[Stat.HP] <= 0)
                HandleDeath();
        }

        #endregion

        public bool HasImmunity(Immunities immunity)
        {
            return activeImmunities.Contains(immunity);
        }

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

            CharacterClass playerClass = GameManager.SelectedClass;
            // Check if the player's class has any spell-learning levels defined.
            Debug.Log($"Player class: {playerClass.ClassName}");
            Debug.Log($"Player level: {level}");
            Debug.Log(
                $"Player can learn spells at levels: {string.Join(", ", playerClass.LevelToLearnSpells)}"
            );
            Debug.Log(
                $"Can learn spells at this level: {playerClass.LevelToLearnSpells.Contains(level)}"
            );
            if (playerClass.LevelToLearnSpells.Contains(level))
            {
                // Prompt the player to choose a new spell.
                PlayerSpellCaster.Instance.PromptSpellLearning();
                Debug.Log("Choose a new spell!");
            }

            // Level up any known spells (existing logic)
            LevelUpSpells();

            // Continue with updating stats, experience, UI, etc.
            expToNextLevel = Mathf.CeilToInt(expToNextLevel * 1.25f);
            UpdateHealthUI();
            UpdateExperienceUI();
            UpdateMagicUI();
            UpdateStaminaUI();
            CalculateStats(refillResources: true);
            PlayerUI.Instance.UpdateLevelDisplay();

            Debug.Log($"Leveled up to {level}! Next level at {expToNextLevel} EXP.");
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
                Debug.LogWarning("Insufficient currency.");
                return;
            }

            currency -= amount;
            FloatingTextManager.Instance.ShowFloatingText($"{amount} spent", transform, Color.red);
        }
        #endregion

        #region Damage & Status Effect Handling


        private void HandleDeath()
        {
            // Check for ReviveOnce effect.
            if (EquipmentEffects.Contains(StatusEffectType.ReviveOnce))
            {
                StatusEffectManager.Instance.RemoveEquipmentEffects(gameObject, EquipmentEffects);
                playerStats[Stat.HP] = Mathf.RoundToInt(playerStats[Stat.MaxHP] * 0.25f);
                Debug.Log("ReviveOnce triggered! Player revived at 25% HP.");
                FloatingTextManager.Instance.ShowFloatingText("Revived!", transform, Color.green);
                UpdateHealthUI();
                StatusEffectManager.Instance.RemoveAllStatusEffects(gameObject);
                StatusEffectManager.Instance.AddStatusEffect(
                    gameObject,
                    StatusEffectType.Invincible,
                    5f
                );

                StatusEffectManager.Instance.RemoveAllStatusEffects(gameObject);
                return;
            }

            FindAnyObjectByType<GameManager>()?.OnPlayerDeath();
            OnPlayerDeath?.Invoke();
        }
        #endregion

        #region Steps & Misc
        public void AddStep()
        {
            steps++;
            // Increment steps and apply occasional healing/magic refill.
            // (For example, heal 10 HP every 30 steps.)
            PlayerUI.Instance.UpdateStepCount();
        }

        public void DeductStamina(float amount)
        {
            playerStats[Stat.Stamina] = Mathf.Max(playerStats[Stat.Stamina] - amount, 0);
            UpdateStaminaUI();
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

        public void RefillStamina(float amount)
        {
            playerStats[Stat.Stamina] = Mathf.Min(
                playerStats[Stat.Stamina] + amount,
                playerStats[Stat.MaxStamina]
            );
            UpdateStaminaUI();
        }
        #endregion

        #region Temporary Boosts (Coroutines)
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

        public IEnumerator GainMaxHP(float amount, float duration)
        {
            playerStats[Stat.MaxHP] += amount;
            yield return new WaitForSeconds(duration);
            playerStats[Stat.MaxHP] -= amount;
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
        #endregion
    }
}
