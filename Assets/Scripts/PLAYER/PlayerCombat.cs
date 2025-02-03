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
                    // ✅ Calculate hit success
                    bool hitSuccess = BattleCalculations.IsAttackSuccessful(
                        playerStats.GetCurrentAccuracy(),
                        enemyStats.GetEnemyEvasion()
                    );

                    if (!hitSuccess)
                    {
                        Debug.Log("Player's attack missed!");
                        return;
                    }

                    // ✅ Determine if attack is critical
                    bool isCritical = BattleCalculations.IsCriticalHit(
                        playerStats.GetCurrentDexterity()
                    );

                    // ✅ Calculate physical damage
                    float physicalDamage = BattleCalculations.CalculateDamage(
                        playerStats.GetCurrentAttack(),
                        0, // No additional weapon power here (could be modified later)
                        enemyStats.GetEnemyDefense(),
                        isCritical
                    );

                    // ✅ Calculate elemental damage separately
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

                    // ✅ Apply status effects based on chance
                    List<StatusEffectType> successfulEffects = new List<StatusEffectType>();
                    foreach (var statusEffect in playerStats.inflictableStatusEffects)
                    {
                        if (
                            BattleCalculations.ApplyStatusEffect(
                                PlayerStats.Instance.GetCurrentChanceToInflictStatusEffect(),
                                playerStats.GetCurrentIntelligence()
                            )
                        )
                        {
                            successfulEffects.Add(statusEffect.Key);
                        }
                    }

                    // ✅ Send damage and effects to enemy
                    DamageInfo damageInfo = new DamageInfo(damageDealt, successfulEffects);
                    enemyStats.TakeDamage(damageInfo);

                    // ✅ Reset enemy attack flags after being hit
                    ResetEnemyAttackFlags();
                    lastAttackTime = Time.time;

                    Debug.Log(
                        $"Player dealt {physicalDamage} damage to {enemyStats.name} {(isCritical ? "(Critical Hit!)" : "")}."
                    );
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
