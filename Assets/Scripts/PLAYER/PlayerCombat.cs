using System.Collections;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    // Handles the player's combat actions, including melee and ranged attacks, and integrates with the turn system.
    public class PlayerCombat : MonoBehaviour
    {
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

        private void Start()
        {
            playerStats = GetComponent<PlayerStats>();
            playerMagic = GetComponent<PlayerMagic>();
            projectileManager = FindAnyObjectByType<ProjectileManager>();
            playerManager = PlayerManager.Instance;

            if (playerStats == null || playerMagic == null || projectileManager == null || playerManager == null)
            {
                Debug.LogError("PlayerCombat: Missing required components. Disabling PlayerCombat script.");
                enabled = false;
            }
        }

        private void HandleCombatInput()
        {
            if (playerStats.CurrentHealth > 0 && Time.time >= lastAttackTime + attackCooldown)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    playerManager.CommitCombatAction(true, Vector3.zero); // Melee attack
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0f;
                    playerManager.CommitCombatAction(false, targetPosition); // Ranged attack
                }
            }
        }

        public void PerformMeleeAttack()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            bool hitAny = false;

            foreach (var enemyCollider in hitEnemies)
            {
                if (enemyCollider.CompareTag("Enemy"))
                {
                    EnemyStats enemyStats = enemyCollider.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        int damageDealt = (int)Mathf.Max(playerStats.CurrentAttack - enemyStats.CurrentDefense, 1);
                        enemyStats.TakeDamage(damageDealt);
                        Debug.Log($"PlayerCombat: Melee attacked {enemyStats.name} for {damageDealt} damage.");
                        hitAny = true;

                        // Apply status effect to enemy using the constructor
                        StatusEffect stunEffect = new StatusEffect("Stun", 3f, 0f, null);
                        enemyStats.GetComponent<StatusEffectManager>()?.AddStatusEffect(stunEffect);
                    }
                }
            }

            if (hitAny)
            {
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.Log("PlayerCombat: No enemies in melee range.");
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
                projectileManager.LaunchProjectile(transform.position, targetPosition, spellDamage, false);
                Debug.Log($"PlayerCombat: Cast spell towards {targetPosition} for {spellDamage} damage.");
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.Log("PlayerCombat: Not enough magic to cast the spell.");
            }
        }
    }
}

/*
Changes made:
1. Removed the `CanAct()` method since it was a leftover from when this script implemented `IActor`. Action eligibility is now checked directly within input methods.
2. Removed any direct interaction with turn management. Turn registration is now solely handled by `PlayerManager`.
3. Kept cooldown checks and health conditions as internal checks before executing actions to ensure consistency without unnecessary redundancy.
*/
