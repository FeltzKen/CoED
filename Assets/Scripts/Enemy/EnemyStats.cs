using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class _EnemyStats : MonoBehaviour, IEntityStats
    {
        [Header("Assigned Monster Data")]
        public Monster monsterData; // from the new system

        private DamageType elementalBase = DamageType.Physical;

        private bool invincible = false;
        public bool Invincible => invincible;
        public bool Silenced { get; private set; }

        [Header("Dynamic Damage Types")]
        public Dictionary<DamageType, float> dynamicDamageTypes =
            new Dictionary<DamageType, float>();

        [SerializeField]
        public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
        public Dictionary<Stat, Coroutine> activeNullifications = new Dictionary<Stat, Coroutine>();

        // Current Stats
        private Dictionary<Stat, float> enemyStats = new Dictionary<Stat, float>()
        {
            { Stat.HP, 0f },
            { Stat.MaxHP, 0f },
            { Stat.Attack, 0f },
            { Stat.Intelligence, 0f },
            { Stat.Evasion, 0f },
            { Stat.Defense, 0f },
            { Stat.Dexterity, 0f },
            { Stat.Magic, 0f },
            { Stat.MaxMagic, 0f },
            { Stat.Accuracy, 0f },
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
            { Stat.ChanceToInflictStatusEffect, 0f },
            { Stat.StatusEffectDuration, 0f },
            { Stat.PatrolSpeed, 0f },
            { Stat.ChaseSpeed, 0f },
        };

        [SerializeField]
        public float GetEnemyHP() => enemyStats[Stat.HP];

        [SerializeField]
        public float GetEnemyMaxHP() => enemyStats[Stat.MaxHP];

        public float GetEnemyAttack() => enemyStats[Stat.Attack];

        public float GetEnemyIntelligence() => enemyStats[Stat.Intelligence];

        public float GetEnemyEvasion() => enemyStats[Stat.Evasion];

        public float GetEnemyDefense() => enemyStats[Stat.Defense];

        public float GetEnemyDexterity() => enemyStats[Stat.Dexterity];

        public float GetEnemyMagic() => enemyStats[Stat.Magic];

        public float GetEnemyMaxMagic() => enemyStats[Stat.MaxMagic];

        public float GetEnemyAccuracy() => enemyStats[Stat.Accuracy];

        public float GetEnemyFireRate() => enemyStats[Stat.FireRate];

        public float GetEnemyProjectileRange() => enemyStats[Stat.ProjectileRange];

        public float GetEnemyAttackRange() => enemyStats[Stat.AttackRange];

        public float GetEnemySpeed() => enemyStats[Stat.Speed];

        public float GetEnemyShield() => enemyStats[Stat.Shield];

        public float GetEnemyElementalDamage() => enemyStats[Stat.ElementalDamage];

        public float GetEnemyCritChance() => enemyStats[Stat.CritChance];

        public float GetEnemyCritDamage() => enemyStats[Stat.CritDamage];

        public float GetEnemyChanceToInflictStatusEffect() =>
            enemyStats[Stat.ChanceToInflictStatusEffect];

        public float GetEnemyStatusEffectDuration() => enemyStats[Stat.StatusEffectDuration];

        public float GetEnemyPatrolSpeed() => enemyStats[Stat.PatrolSpeed];

        public float GetEnemyChaseSpeed() => enemyStats[Stat.ChaseSpeed];

        [SerializeField]
        private int spawnFloor;
        public int spawnFloorLevel
        {
            get => spawnFloor;
            set => spawnFloor = value;
        }

        // If you want an "equipment tier" from floor
        public int EquipmentTier { get; private set; } = 1;

        // Factor for scaling stats
        public float ScaledFactor { get; private set; }

        private EnemyUI enemyUI;

        private void Start()
        {
            enemyUI = GetComponentInChildren<EnemyUI>();

            ApplyMonsterData(monsterData);

            // 3) floor-based logic:
            if (spawnFloor <= 0)
                spawnFloor = 1;
            EquipmentTier = Mathf.Clamp(Mathf.FloorToInt(spawnFloor / 3) + 1, 1, 3);

            // 4) calculate final stats (the hybrid approach: monster base + floor scaling)
            //CalculateMonsterScaledStats();
            InitializeUI();
        }

        private void InitializeUI()
        {
            if (enemyUI != null)
            {
                enemyUI.SetHealthBarMax(enemyStats[Stat.MaxHP]);
            }
            GetComponent<InspectorStatDisplay>().SetStats();
        }

        /// <summary>
        /// Copies the monster's base stats into local fields.
        /// </summary>
        private void ApplyMonsterData(Monster monster)
        {
            if (monster == null)
            {
                Debug.LogWarning("No monsterData assigned; using fallback stats.");
                return;
            }
            monsterData.monsterSprite = monster.monsterSprite;

            elementalBase = monster.damageType;
            enemyStats[Stat.MaxHP] = monster.monsterStats[Stat.MaxHP];
            enemyStats[Stat.Attack] = monster.monsterStats[Stat.Attack];
            enemyStats[Stat.Defense] = monster.monsterStats[Stat.Defense];
            enemyStats[Stat.Dexterity] = monster.monsterStats[Stat.Dexterity];
            enemyStats[Stat.ElementalDamage] = monster.monsterStats[Stat.ElementalDamage];
            enemyStats[Stat.AttackRange] = monster.monsterStats[Stat.AttackRange];
            enemyStats[Stat.ProjectileRange] = monster.monsterStats[Stat.ProjectileRange];
            enemyStats[Stat.ChanceToInflictStatusEffect] = monster.statusInflictionChance;
            enemyStats[Stat.Speed] = monster.monsterStats[Stat.Speed];
            enemyStats[Stat.Intelligence] = monster.monsterStats[Stat.Intelligence];
            enemyStats[Stat.Evasion] = monster.monsterStats[Stat.Evasion];
            enemyStats[Stat.CritChance] = monster.monsterStats[Stat.CritChance];
            enemyStats[Stat.CritDamage] = monster.monsterStats[Stat.CritDamage];
            enemyStats[Stat.FireRate] = monster.monsterStats[Stat.FireRate];
            enemyStats[Stat.Shield] = monster.monsterStats[Stat.Shield];
            enemyStats[Stat.Accuracy] = monster.monsterStats[Stat.Accuracy];
            enemyStats[Stat.StatusEffectDuration] = monster.monsterStats[Stat.StatusEffectDuration];
            enemyStats[Stat.PatrolSpeed] = monster.monsterStats[Stat.PatrolSpeed];
            enemyStats[Stat.ChaseSpeed] = monster.monsterStats[Stat.ChaseSpeed];

            monsterData.inflictedStatusEffect = monster.inflictedStatusEffect;
        }

        #region IEntity Implementation

        public void ModifyStat(Stat stat, float value)
        {
            if (enemyStats.ContainsKey(stat))
                enemyStats[stat] += value;
            else
                enemyStats[stat] = value;

            Debug.Log($"[EnemyStats] {stat} modified by {value}. New Value: {enemyStats[stat]}");
        }

        public void NullStat(Stat stat, float duration)
        {
            if (activeNullifications.ContainsKey(stat))
            {
                // If already nullifying, restart it
                StopCoroutine(activeNullifications[stat]);
            }

            activeNullifications[stat] = StartCoroutine(NullifyStatCoroutine(stat, duration)); // Default 5s duration
        }

        private IEnumerator NullifyStatCoroutine(Stat stat, float duration)
        {
            if (!enemyStats.ContainsKey(stat))
                yield break;

            float originalValue = enemyStats[stat];
            enemyStats[stat] = 0;
            Debug.Log($"[PlayerStats] {stat} nullified for {duration} seconds.");

            yield return new WaitForSeconds(duration);

            enemyStats[stat] = originalValue;
            activeNullifications.Remove(stat);
            Debug.Log($"[PlayerStats] {stat} restored to {originalValue}.");
        }

        public void Heal(float amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("EnemyStats: Heal amount must be positive.");
                return;
            }

            enemyStats[Stat.HP] = Mathf.Min(enemyStats[Stat.HP] + amount, enemyStats[Stat.MaxHP]);
            UpdateHealthUI();
            Debug.Log($"Healed {amount} => {enemyStats[Stat.HP]}/{enemyStats[Stat.MaxHP]}");
        }

        public void AddActiveStatusEffect(StatusEffectType effect)
        {
            if (!activeStatusEffects.Contains(effect))
            {
                activeStatusEffects.Add(effect);
            }
        }

        public void RemoveActiveStatusEffect(StatusEffectType effect)
        {
            if (activeStatusEffects.Contains(effect))
            {
                activeStatusEffects.Remove(effect);
            }
        }

        public void AddWeakness(Weaknesses weaknesses)
        {
            monsterData.weaknesses.Add(weaknesses);
        }

        public void RemoveWeakness(Weaknesses weaknesses)
        {
            monsterData.weaknesses.Remove(weaknesses);
        }

        public void AddImmunity(Immunities immunities)
        {
            monsterData.immunities.Add(immunities);
        }

        public void RemoveImmunity(Immunities immunities)
        {
            monsterData.immunities.Remove(immunities);
        }

        public void AddResistance(Resistances resistances)
        {
            monsterData.resistances.Add(resistances);
        }

        public void RemoveResistance(Resistances resistances)
        {
            monsterData.resistances.Remove(resistances);
        }

        public void AddShield(float shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("Shield value must be positive.");
                return;
            }
            shieldValue += shieldValue;
            enemyStats[Stat.Defense] += shieldValue;
        }

        public void RemoveShield(float shieldValue)
        {
            if (shieldValue <= 0)
            {
                Debug.LogWarning("Shield value must be positive.");
                return;
            }

            float effective = Mathf.Min(shieldValue, enemyStats[Stat.Shield]);
            enemyStats[Stat.Shield] -= effective;
            enemyStats[Stat.Defense] -= effective;
        }

        public bool HasStatusEffect(StatusEffectType effect)
        {
            return activeStatusEffects.Contains(effect);
        }

        public void IsSilenced(bool state)
        {
            Silenced = state;
        }

        public void TakeDamage(
            DamageInfo damageInfo,
            float statusChance = 0.05f,
            bool bypassInvincible = false
        )
        {
            if (!bypassInvincible && invincible)
            {
                Debug.Log($"{gameObject.name} is invincible.");
                return;
            }

            float totalDamage = 0f;
            foreach (var damageEntry in damageInfo.DamageAmounts)
            {
                DamageType type = damageEntry.Key;
                float amount = damageEntry.Value;

                if (monsterData.immunities.Contains((Immunities)type))
                {
                    Debug.Log($"{gameObject.name} is immune to {type} damage.");
                    FloatingTextManager.Instance.ShowFloatingText(
                        $"{type.ToString().ToUpper()} IMMUNE",
                        transform,
                        Color.cyan
                    );
                    continue;
                }

                if (monsterData.resistances.Contains((Resistances)type))
                {
                    amount *= 0.5f;
                }
                if (monsterData.weaknesses.Contains((Weaknesses)type))
                {
                    amount *= 1.5f;
                }
                totalDamage += amount;
            }

            float effectiveDamage = Mathf.Max(totalDamage - enemyStats[Stat.Defense], 1);
            enemyStats[Stat.HP] = Mathf.Max(
                enemyStats[Stat.HP] - Mathf.RoundToInt(effectiveDamage),
                0
            );

            // Status effects
            foreach (var effect in damageInfo.InflictedStatusEffects)
            {
                // If attacker has a certain chance to inflict
                // e.g. from playerStats or the skill used
                if (Random.value < PlayerStats.Instance.GetCurrentChanceToInflictStatusEffect())
                {
                    StatusEffectManager.Instance.AddStatusEffect(
                        gameObject,
                        effect,
                        enemyStats[Stat.StatusEffectDuration]
                    );
                }
            }

        #endregion
            #region End of IEntity Implementation
            #endregion
            UpdateHealthUI();
            FloatingTextManager.Instance.ShowFloatingText(
                effectiveDamage.ToString(),
                transform,
                Color.red
            );

            if (enemyStats[Stat.HP] <= 0)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            AwardExperienceToPlayer();
            Debug.Log($"Enemy {gameObject.name} has died.");

            // If there's an _Enemy script, call DropLoot()
            var enemyScript = GetComponent<_Enemy>();
            if (enemyScript != null)
            {
                enemyScript.DropLoot();
            }

            // Release the tile
            var nav = GetComponent<EnemyNavigator>();
            if (nav != null)
            {
                TileOccupancyManager.Instance.ReleaseAllTiles(nav.occupantID);
            }

            Destroy(gameObject);
        }

        public void SetInvincible(bool invincible)
        {
            this.invincible = invincible;
        }

        private void UpdateHealthUI()
        {
            var enemyUI = GetComponentInChildren<EnemyUI>();
            if (enemyUI != null)
            {
                enemyUI.UpdateHealthBar(enemyStats[Stat.HP], enemyStats[Stat.MaxHP]);
            }
        }

        private int CalculateExperiencePoints()
        {
            int baseExperience = Mathf.RoundToInt(ScaledFactor * 10);
            return baseExperience;
        }

        private void AwardExperienceToPlayer()
        {
            int xp = CalculateExperiencePoints();
            PlayerStats.Instance?.GainExperience(xp);
            Debug.Log($"Awarded {xp} XP to player.");
        }
    }
}
