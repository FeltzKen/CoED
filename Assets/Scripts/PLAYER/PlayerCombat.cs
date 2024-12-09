using System.Collections;
using System.Linq;
using CoED;
using UnityEngine;

namespace CoED
{
    // Handles the player's combat actions, including melee and ranged attacks, and integrates with the turn system.
    public class PlayerCombat : MonoBehaviour
    {
        public static PlayerCombat Instance { get; private set; }

        [Header("Combat Settings")]
        [SerializeField]
        private float attackRange = 1.5f;

        [SerializeField]
        private float projectileRange = 5f;

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
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //   PerformMeleeAttack();
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0f;
                }
            }
        }

        public void PerformMeleeAttack(Vector2Int targetPosition)
        {
            //Debug.Log($"PlayerCombat: Attempting melee attack at {targetPosition}.");
            Vector2 targetWorldPosition = new Vector2(targetPosition.x, targetPosition.y);

            // Use a small overlap circle for better enemy detection coverage
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
                    int damageDealt = Mathf.Max(playerStats.CurrentAttack, 1);
                    //    Debug.Log($"PlayerCombat: Melee attacked {enemyStats.name} at {targetPosition} for {damageDealt} damage.");
                    enemyStats.TakeDamage(damageDealt);

                    lastAttackTime = Time.time;
                    enemy.ResetEnemyAttackFlags();
                }
            }
        }
    }
}
