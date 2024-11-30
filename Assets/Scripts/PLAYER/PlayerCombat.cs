using System.Collections;
using System.Linq;
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
        private PlayerMagic playerMagic;
        private ProjectileManager projectileManager;
        private PlayerManager playerManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
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
            playerStats = GetComponent<PlayerStats>();
            playerMagic = GetComponent<PlayerMagic>();
            projectileManager = FindObjectsByType<ProjectileManager>(FindObjectsSortMode.None).FirstOrDefault();
            playerManager = PlayerManager.Instance;

            if (playerStats == null || playerMagic == null || projectileManager == null || playerManager == null)
            {
                Debug.LogError("PlayerCombat: Missing required components. Disabling PlayerCombat script.");
                enabled = false;
            }
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
                    AttemptRangedAttack(targetPosition);
                }
            }
        }

public void PerformMeleeAttack(Vector2Int targetPosition)
{
    //Debug.Log($"PlayerCombat: Attempting melee attack at {targetPosition}.");
    Vector2 targetWorldPosition = new Vector2(targetPosition.x, targetPosition.y);

    // Use a small overlap circle for better enemy detection coverage
    Collider2D hitCollider = Physics2D.OverlapCircle(targetWorldPosition, 0f, LayerMask.GetMask("enemies"));

    if (hitCollider != null && hitCollider.CompareTag("Enemy"))
    {
        EnemyStats enemyStats = hitCollider.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            int damageDealt = Mathf.Max(playerStats.CurrentAttack, 1);
            Debug.Log($"PlayerCombat: Melee attacked {enemyStats.name} at {targetPosition} for {damageDealt} damage.");
            enemyStats.TakeDamage(damageDealt);

            // Apply status effect to enemy
           // StatusEffect stunEffect = new StatusEffect("Stun", 3f, 0f, null);
           // enemyStats.GetComponent<StatusEffectManager>()?.AddStatusEffect(stunEffect);

            lastAttackTime = Time.time;
            PlayerManager.Instance.ResetEnemyAttackFlags();
        }
    }
    else
    {
        // Debug.Log($"PlayerCombat: No enemy at position {targetPosition} to attack.");
    }
}

        public void AttemptRangedAttack(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance > projectileRange)
            {
                targetPosition = transform.position + direction * projectileRange;
            }

            int spellCost = 20;
            int spellDamage = playerStats.CurrentAttack;

            if (playerMagic.CurrentMagic >= spellCost)
            {
                playerMagic.CurrentMagic -= spellCost;
                projectileManager.LaunchProjectile(transform.position, targetPosition);
                Debug.Log($"PlayerCombat: Cast spell towards {targetPosition} for {spellDamage} damage.");
                lastAttackTime = Time.time;
                PlayerManager.Instance.ResetEnemyAttackFlags();
            }
            else
            {
                // Debug.Log("PlayerCombat: Not enough magic to cast the spell.");
            }
        }
    }
}