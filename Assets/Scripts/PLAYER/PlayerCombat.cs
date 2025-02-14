using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class PlayerCombat : MonoBehaviour
    {
        public static PlayerCombat Instance { get; private set; }

        [SerializeField]
        private float attackCooldown = 1f;

        private float lastAttackTime = 0f;
        private PlayerStats playerStats;

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
                Debug.LogWarning("PlayerCombat instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            playerStats = PlayerStats.Instance;

            if (playerStats == null)
            {
                Debug.LogError(
                    "PlayerCombat: Missing required components. Disabling PlayerCombat script."
                );
                enabled = false;
            }
        }

        private void Update()
        {
            HandleCombatInput();
        }

        private void HandleCombatInput()
        {
            if (playerStats.HasEnteredDungeon == false)
                return;
            if (playerStats.GetCurrentHealth() > 0 && Time.time >= lastAttackTime + attackCooldown)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0f;
                }
            }
        }

        public void PerformMeleeAttack(Vector2Int targetPosition)
        {
            // Get the enemy collider at the target grid position.
            Vector2 targetWorldPosition = new Vector2(targetPosition.x, targetPosition.y);
            Collider2D hitCollider = Physics2D.OverlapCircle(
                targetWorldPosition,
                0f,
                LayerMask.GetMask("enemies")
            );

            if (hitCollider != null && hitCollider.CompareTag("Enemy"))
            {
                _EnemyStats enemyStats = hitCollider.GetComponent<_EnemyStats>();
                if (enemyStats != null)
                {
                    // Use Dexterity for hit/dodge.
                    bool hitSuccess = BattleCalculations.IsAttackSuccessful(
                        playerStats.GetCurrentDexterity(),
                        enemyStats.GetEnemyDexterity()
                    );

                    if (!hitSuccess)
                    {
                        Debug.Log("Player's attack missed!");
                        return;
                    }

                    // Determine a critical hit using the player's CritChance stat.
                    bool isCritical = BattleCalculations.IsCriticalHit(
                        playerStats.GetCurrentCritChance()
                    );

                    // Calculate the base physical damage.
                    float physicalDamage = BattleCalculations.CalculateDamage(
                        playerStats.GetCurrentAttack(),
                        0, // weapon power can be added here if needed
                        enemyStats.GetEnemyDefense(),
                        isCritical,
                        1.5f // hradcoded crit damage multiplier might be replaced with a stat
                    );

                    // Calculate elemental damage (example uses player's elemental damage bonuses).
                    Dictionary<DamageType, float> damageDealt = new Dictionary<DamageType, float>
                    {
                        { DamageType.Physical, physicalDamage },
                        { DamageType.Fire, playerStats.GetCurrentBurnDamage() },
                        { DamageType.Poison, playerStats.GetCurrentPoisonDamage() },
                        { DamageType.Ice, playerStats.GetCurrentIceDamage() },
                        { DamageType.Lightning, playerStats.GetCurrentLightningDamage() },
                        { DamageType.Shadow, playerStats.GetCurrentShadowDamage() },
                        { DamageType.Arcane, playerStats.GetCurrentArcaneDamage() },
                        { DamageType.Holy, playerStats.GetCurrentHolyDamage() },
                        { DamageType.Bleed, playerStats.GetCurrentBleedDamage() },
                    };

                    // Decide which status effects (if any) should be applied.
                    List<StatusEffectType> successfulEffects = new List<StatusEffectType>();
                    foreach (var effect in playerStats.inflictableStatusEffects)
                    {
                        if (
                            BattleCalculations.ShouldApplyStatusEffect(
                                playerStats.GetCurrentChanceToInflict(),
                                playerStats.GetCurrentIntelligence()
                            )
                        )
                        {
                            successfulEffects.Add(effect);
                        }
                    }
                    // (Optionally, include additional status effects from equipped items.)
                    foreach (var effect in playerStats.EquipmentEffects)
                    {
                        if (
                            BattleCalculations.ShouldApplyStatusEffect(
                                playerStats.GetCurrentChanceToInflict(),
                                playerStats.GetCurrentIntelligence()
                            )
                        )
                        {
                            successfulEffects.Add(effect);
                        }
                    }
                    // Package damage and status effects together.
                    DamageInfo damageInfo = new DamageInfo(damageDealt, successfulEffects);

                    // Apply the damage to the enemy.
                    enemyStats.TakeDamage(damageInfo);

                    // Reset enemy attack flags and update the last attack time.
                    ResetEnemyAttackFlags();
                    lastAttackTime = Time.time;
                }
            }
        }

        public void ResetEnemyAttackFlags()
        {
            foreach (var enemy in FindObjectsByType<EnemyBrain>(FindObjectsSortMode.None))
            {
                enemy.CanAttackPlayer = true;
            }
        }
    }
}
