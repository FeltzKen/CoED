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
        private Enemy enemy;

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
            enemy = FindAnyObjectByType<Enemy>();
        }

        private void Update()
        {
            HandleCombatInput();
        }

        private void HandleCombatInput()
        {
            if (playerStats.CurrentHealth > 0 && Time.time >= lastAttackTime + attackCooldown)
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
                EnemyStats enemyStats = hitCollider.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    // ✅ Combine physical and elemental damage
                    Dictionary<DamageType, float> damageDealt = new Dictionary<DamageType, float>
                    {
                        { DamageType.Physical, playerStats.GetCurrentAttack() },
                        { DamageType.Fire, playerStats.GetCurrentBurnDamage() },
                        { DamageType.Poison, playerStats.GetCurrentPoisonDamage() },
                        { DamageType.Ice, playerStats.GetCurrentIceDamage() },
                        { DamageType.Lightning, playerStats.GetCurrentLightningDamage() },
                        { DamageType.Shadow, playerStats.GetCurrentShadowDamage() },
                        { DamageType.Arcane, playerStats.GetCurrentArcaneDamage() },
                        { DamageType.Holy, playerStats.GetCurrentHolyDamage() },
                        { DamageType.Bleed, playerStats.GetCurrentBleedDamage() },
                    };

                    // ✅ Combine status effects from equipment
                    List<StatusEffectType> successfulEffects = new List<StatusEffectType>();

                    foreach (var statusEffect in playerStats.inflictableStatusEffects)
                    {
                        if (Random.value <= PlayerStats.Instance.CurrentChanceToInflictStatusEffect)
                        {
                            successfulEffects.Add(statusEffect);
                        }
                    }

                    DamageInfo damageInfo = new DamageInfo(damageDealt, successfulEffects);
                    enemyStats.TakeDamage(damageInfo);
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
