using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class PlayerSpellCaster : MonoBehaviour
    {
        public static PlayerSpellCaster Instance { get; private set; }

        [Header("Spell Settings")]
        [SerializeField]
        private List<PlayerSpell> availableSpells;

        [SerializeField]
        private Transform spellSpawnPoint;

        private PlayerStats playerStats;
        private float[] spellCooldownTimers;
        private PlayerSpell selectedSpell;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Uncomment if you want this instance to persist across scenes
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
                // Fallback to Singleton if not found on the same GameObject
                playerStats = PlayerStats.Instance;
                if (playerStats == null)
                {
                    Debug.LogError("PlayerStats instance not found.");
                    enabled = false;
                    return;
                }
            }

            if (availableSpells == null || availableSpells.Count == 0)
            {
                Debug.LogWarning("No spells assigned to PlayerSpellCaster.");
            }

            spellCooldownTimers = new float[availableSpells.Count];
        }

        private void OnDestroy()
        {
            Debug.Log("PlayerSpellCaster: OnDestroy called.");
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && selectedSpell != null)
            {
                CastSelectedSpell();
            }

            UpdateCooldownTimers();
        }

        public List<PlayerSpell> AvailableSpells => availableSpells;

        public void SelectSpell(PlayerSpell spell)
        {
            selectedSpell = spell;
            Debug.Log($"Selected Spell: {spell.spellName}");
        }

        private void CastSelectedSpell()
        {
            Debug.Log("Casting selected spell.");
            int spellIndex = availableSpells.IndexOf(selectedSpell);
            if (spellIndex == -1)
                return;

            if (spellCooldownTimers[spellIndex] > 0f)
            {
                Debug.Log($"SpellCaster: {selectedSpell.spellName} is on cooldown.");
                return;
            }

            if (playerStats.CurrentMagic < selectedSpell.magicCost)
            {
                Debug.Log($"SpellCaster: Not enough magic to cast {selectedSpell.spellName}.");
                return;
            }

            playerStats.ConsumeMagic(selectedSpell.magicCost);
            spellCooldownTimers[spellIndex] = selectedSpell.cooldown;

            Vector3 targetPosition = GetMouseWorldPosition();
            ExecuteSpell(selectedSpell, targetPosition);
        }

        private void ExecuteSpell(PlayerSpell spell, Vector3 targetPosition)
        {
            Debug.Log($"Casting {spell.spellName} at {targetPosition}.");
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

            selectedSpell = null; // Reset after casting
        }

        private void CastInstantSpell(PlayerSpell spell, Vector3 targetPosition)
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(
                targetPosition,
                LayerMask.GetMask("Enemy")
            );
            if (hitCollider != null)
            {
                EnemyStats enemyStats = hitCollider.GetComponent<EnemyStats>();
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

            // Instantiate instant spell effect
            if (spell.spellEffectPrefab != null)
            {
                Instantiate(spell.spellEffectPrefab, targetPosition, Quaternion.identity);
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
                projectile.direction = direction;
                projectile.damage = spell.damage;

                // Set the target position for the projectile
                projectile.SetTargetPosition(targetPosition);

                // Handle chasing ability
                if (spell.canChase)
                {
                    // Implement chasing logic if needed
                    // projectile.SetChaseMode(true);
                }
            }
            else
            {
                Debug.LogWarning(
                    $"The spellEffectPrefab for {spell.spellName} does not have a PlayerProjectile component."
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
                LayerMask.GetMask("Enemy")
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

        private void UpdateCooldownTimers()
        {
            for (int i = 0; i < spellCooldownTimers.Length; i++)
            {
                if (spellCooldownTimers[i] > 0f)
                {
                    spellCooldownTimers[i] -= Time.deltaTime;
                    if (spellCooldownTimers[i] < 0f)
                        spellCooldownTimers[i] = 0f;
                }
            }
        }
    }
}
