using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerSpellCaster : MonoBehaviour
    {
        // Singleton Instance
        public static PlayerSpellCaster Instance { get; private set; }

        [SerializeField]
        private Transform spellSpawnPoint; // Point where spells are spawned
        private PlayerStats playerStats; // Reference to the PlayerStats component
        private PlayerSpell selectedSpell; // Currently selected spell

        [SerializeField]
        private LayerMask enemyLayer; // Layer mask to identify enemies

        private GameObject currentTarget;
        private List<GameObject> enemiesInRange = new List<GameObject>();
        private int currentTargetIndex = 0;

        /// <summary>
        /// Initializes the Singleton instance and essential components.
        /// </summary>
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
                Debug.LogWarning(
                    "PlayerSpellCaster instance already exists. Destroying duplicate."
                );
                return;
            }

            playerStats = GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                playerStats = PlayerStats.Instance;
                if (playerStats == null)
                {
                    Debug.LogError("PlayerStats instance not found.");
                    enabled = false;
                    return;
                }
            }
        }

        private void Update()
        {
            UpdateEnemiesInRange();
            HandleTargetingInput();
            // Detect right mouse button click
            if (Input.GetMouseButton(1)) // 1 corresponds to the right mouse button
            {
                //Debug.Log("Right mouse button clicked.");
                CastSelectedSpell(GetMouseWorldPosition());
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CastSelectedSpell(currentTarget.transform.position);
            }
        }

        private void UpdateEnemiesInRange()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                transform.position,
                playerStats.targetingRange,
                enemyLayer
            );
            enemiesInRange.Clear();

            foreach (Collider2D collider in hitColliders)
            {
                if (collider.gameObject != this.gameObject)
                {
                    enemiesInRange.Add(collider.gameObject);
                }
            }

            // Auto-target the first enemy if no target is selected
            if (currentTarget == null && enemiesInRange.Count > 0)
            {
                SetTarget(enemiesInRange[0]);
            }
        }

        private void HandleTargetingInput()
        {
            // Mouse Click Targeting
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hitCollider = Physics2D.OverlapCircle(mouseWorldPos, 1f, enemyLayer);

                if (hitCollider != null)
                {
                    SetTarget(hitCollider.gameObject);
                }
            }

            // Toggle Targeting
            if (Input.GetKeyDown(KeyCode.T) && enemiesInRange.Count > 0)
            {
                ToggleTarget();
            }
        }

        private void SetTarget(GameObject target)
        {
            // Remove highlight from previous target
            if (currentTarget != null)
            {
                Enemy enemyComponent = currentTarget.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.SetHighlighted(false);
                }
            }

            // Set and highlight the new target
            currentTarget = target;
            Enemy newTargetComponent = currentTarget.GetComponent<Enemy>();
            if (newTargetComponent != null)
            {
                newTargetComponent.SetHighlighted(true);
            }
            Debug.Log($"Enemies in range: {enemiesInRange.Count}");
            Debug.Log($"Current target: {currentTarget?.name ?? "None"}");
        }

        private void ToggleTarget()
        {
            if (currentTarget == null)
                return;

            currentTargetIndex = enemiesInRange.IndexOf(currentTarget);
            currentTargetIndex = (currentTargetIndex + 1) % enemiesInRange.Count;
            SetTarget(enemiesInRange[currentTargetIndex]);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize targeting range in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, playerStats.targetingRange);
        }

        public void SelectSpell(PlayerSpell spell)
        {
            selectedSpell = spell;
            Debug.Log($"Selected Spell: {spell.spellName}");
        }

        public void CastSelfTargetingSpell(PlayerSpell spell)
        {
            if (playerStats.CurrentMagic < spell.magicCost)
            {
                Debug.Log("Not enough magic to cast the spell.");
                return;
            }

            if (IsSpellOnCooldown(spell))
            {
                Debug.Log("Spell is on cooldown.");
                return;
            }
            if (playerStats.CurrentHealth == playerStats.MaxHealth)
            {
                Debug.Log("Player is already at full health.");
                return;
            }
            // Consume mana and execute spell
            playerStats.ConsumeMagic(spell.magicCost);
            ExecuteSpell(spell, transform.position);

            // Start cooldown
            PlayerUI.Instance.OnSpellCast(spell);
        }

        /// <summary>
        /// Casts the currently selected spell, handling cooldowns and UI updates.
        /// </summary>
        private void CastSelectedSpell(Vector3 targetPosition)
        {
            if (selectedSpell == null)
            {
                Debug.LogWarning("No spell selected to cast.");
                return;
            }

            if (playerStats.CurrentMagic < selectedSpell.magicCost)
            {
                Debug.Log("Not enough magic to cast the spell.");
                return;
            }

            if (IsSpellOnCooldown(selectedSpell))
            {
                Debug.Log("Spell is on cooldown.");
                return;
            }

            // Consume mana and set cooldown
            playerStats.ConsumeMagic(selectedSpell.magicCost);

            // Execute the spell's effect
            ExecuteSpell(selectedSpell, targetPosition);

            // Notify PlayerUI to start cooldown and fade effects
            PlayerUI.Instance.OnSpellCast(selectedSpell);
        }

        /// <summary>
        /// Checks if the selected spell is on cooldown.
        /// </summary>
        /// <param name="spell">The spell to check.</param>
        /// <returns>True if the spell is on cooldown, false otherwise.</returns>
        private bool IsSpellOnCooldown(PlayerSpell spell)
        {
            return PlayerUI.Instance.IsSpellOnCooldown(spell);
        }

        /// <summary>
        /// Executes the effect of the cast spell based on its type.
        /// </summary>
        /// <param name="spell">The spell to execute.</param>
        /// <param name="targetPosition">Position where the spell is targeted.</param>
        public void ExecuteSpell(PlayerSpell spell, Vector3 targetPosition)
        {
            switch (spell.type)
            {
                case SpellType.Projectile:
                    CastProjectileSpell(spell, targetPosition);
                    break;
                case SpellType.AoE:
                    CastAoESpell(spell, targetPosition);
                    break;
                case SpellType.Heal:
                    CastHealSpell(spell);
                    break;
                default:
                    Debug.LogWarning($"Unsupported spell type: {spell.type}");
                    break;
            }
        }

        /// <summary>
        /// Casts a projectile-type spell.
        /// </summary>
        /// <param name="spell">The projectile spell to cast.</param>
        /// <param name="targetPosition">Target position of the projectile.</param>
        private void CastProjectileSpell(PlayerSpell spell, Vector3 targetPosition)
        {
            if (spell.spellEffectPrefab == null)
            {
                Debug.LogWarning($"No spellEffectPrefab assigned for {spell.spellName}.");
                return;
            }

            GameObject spellObject = Instantiate(
                spell.spellEffectPrefab,
                spellSpawnPoint.position,
                Quaternion.identity
            );
            PlayerProjectile projectile = spellObject.GetComponent<PlayerProjectile>();

            if (projectile != null)
            {
                Vector2 direction = (targetPosition - spellSpawnPoint.position).normalized;
                projectile.lifetime = spell.lifetime;
                projectile.collisionRadius = spell.collisionRadius;
                projectile.direction = direction;
                projectile.speed = spell.speed; // Ensure the speed is set
                projectile.damage = spell.damage;
                projectile.SetTargetPosition(targetPosition);
            }
            else
            {
                Debug.LogWarning(
                    $"The prefab for {spell.spellName} does not have a PlayerProjectile component."
                );
            }
        }

        private void CastAoESpell(PlayerSpell spell, Vector3 targetPosition)
        {
            // Instantiate AoE effect
            if (spell.spellEffectPrefab != null)
            {
                Instantiate(spell.spellEffectPrefab, targetPosition, Quaternion.identity);
            }

            // Apply effects to enemies within area
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                targetPosition,
                spell.areaOfEffect,
                LayerMask.GetMask("enemies")
            );
            foreach (var enemyCollider in hitEnemies)
            {
                EnemyStats enemyStats = enemyCollider.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(spell.damage);
                    ///
                    if (spell.hasBurnEffect)
                    {
                        enemyStats.ApplyStatusEffect(
                            StatusEffectType.Burn,
                            spell.burnDamage,
                            spell.burnDuration
                        );
                    }

                    if (spell.hasFreezeEffect)
                    {
                        enemyStats.ApplyStatusEffect(
                            StatusEffectType.Freeze,
                            0,
                            spell.freezeDuration
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Casts a healing-type spell.
        /// </summary>
        /// <param name="spell">The healing spell to cast.</param>
        private void CastHealSpell(PlayerSpell spell)
        {
            playerStats.Heal(spell.damage);
            if (spell.spellEffectPrefab != null)
            {
                Instantiate(spell.spellEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Retrieves the mouse position in the world space.
        /// </summary>
        /// <returns>World position of the mouse.</returns>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}
