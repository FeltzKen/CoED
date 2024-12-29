using System.Collections.Generic;
using UnityEngine;

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
            if (Input.GetMouseButton(1))
            {
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
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hitCollider = Physics2D.OverlapCircle(mouseWorldPos, 1f, enemyLayer);

                if (hitCollider != null)
                {
                    SetTarget(hitCollider.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.T) && enemiesInRange.Count > 0)
            {
                ToggleTarget();
            }
        }

        private void SetTarget(GameObject target)
        {
            if (currentTarget != null)
            {
                Enemy enemyComponent = currentTarget.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.SetHighlighted(false);
                }
            }

            currentTarget = target;
            Enemy newTargetComponent = currentTarget.GetComponent<Enemy>();
            if (newTargetComponent != null)
            {
                newTargetComponent.SetHighlighted(true);
            }
        }

        private void ToggleTarget()
        {
            if (currentTarget == null)
                return;

            currentTargetIndex = enemiesInRange.IndexOf(currentTarget);
            currentTargetIndex = (currentTargetIndex + 1) % enemiesInRange.Count;
            SetTarget(enemiesInRange[currentTargetIndex]);
        }

        public void SelectSpell(PlayerSpell spell)
        {
            selectedSpell = spell;
            FloatingTextManager.Instance.ShowFloatingText(
                $"{spell.spellName} selected",
                transform,
                Color.yellow
            );
        }

        public void CastSelfTargetingSpell(PlayerSpell spell)
        {
            if (playerStats.CurrentMagic < spell.magicCost)
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Not enough magic",
                    transform,
                    Color.red
                );
                return;
            }

            if (IsSpellOnCooldown(spell))
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Spell is on cooldown",
                    transform,
                    Color.red
                );
                return;
            }
            if (playerStats.CurrentHealth == playerStats.MaxHealth)
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Already at full health",
                    transform,
                    Color.red
                );
                return;
            }
            playerStats.ConsumeMagic(spell.magicCost);
            ExecuteSpell(spell, transform.position);

            PlayerUI.Instance.OnSpellCast(spell);
        }

        private void CastSelectedSpell(Vector3 targetPosition)
        {
            if (selectedSpell == null)
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "No spell selected",
                    transform,
                    Color.red
                );
                return;
            }

            if (playerStats.CurrentMagic < selectedSpell.magicCost)
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Not enough magic",
                    transform,
                    Color.red
                );
                return;
            }

            if (IsSpellOnCooldown(selectedSpell))
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Spell is on cooldown",
                    transform,
                    Color.red
                );
                return;
            }

            playerStats.ConsumeMagic(selectedSpell.magicCost);

            ExecuteSpell(selectedSpell, targetPosition);

            PlayerUI.Instance.OnSpellCast(selectedSpell);
        }

        private bool IsSpellOnCooldown(PlayerSpell spell)
        {
            return PlayerUI.Instance.IsSpellOnCooldown(spell);
        }

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
                case SpellType.Buff:
                    Debug.LogWarning("Buff spells are not implemented yet.");
                    break;
                case SpellType.Debuff:
                    Debug.LogWarning("Debuff spells are not implemented yet.");
                    break;
                default:
                    Debug.LogWarning($"Unsupported spell type: {spell.type}");
                    break;
            }
        }

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

        private void CastHealSpell(PlayerSpell spell)
        {
            playerStats.Heal(spell.damage);
            if (spell.spellEffectPrefab != null)
            {
                Instantiate(spell.spellEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}
