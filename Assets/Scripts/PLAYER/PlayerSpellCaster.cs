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
        private PlayerSpellWrapper selectedSpell; // Currently selected spell

        [SerializeField]
        private LayerMask enemyLayer; // Layer mask to identify enemies

        private GameObject currentTarget;
        private List<GameObject> enemiesInRange = new List<GameObject>();
        private int currentTargetIndex = 0;

        [Header("Status Effects")]
        [SerializeField]
        private StatusEffectIconLibrary statusEffectLibrary;
        private List<PlayerSpellWrapper> playerSpellsWrapper;

        [SerializeField]
        private List<PlayerSpell> spellPrefabs; // List of spell prefabs

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeWrappedSpells();
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

        private void InitializeWrappedSpells()
        {
            playerSpellsWrapper = new List<PlayerSpellWrapper>();
            foreach (var baseSpell in spellPrefabs) // Assuming spellPrefabs holds PlayerSpell ScriptableObjects
            {
                GameObject spellWrapperObj = new GameObject($"{baseSpell.spellName}_Wrapper");
                PlayerSpellWrapper spellWrapper =
                    spellWrapperObj.AddComponent<PlayerSpellWrapper>();
                spellWrapper.Initialize(baseSpell); // Initialize wrapper with the base spell

                playerSpellsWrapper.Add(spellWrapper);
            }
        }

        public List<PlayerSpellWrapper> GetKnownSpells()
        {
            return playerSpellsWrapper;
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

        public void SelectSpell(PlayerSpellWrapper spell)
        {
            selectedSpell = spell;
            FloatingTextManager.Instance.ShowFloatingText(
                $"{spell.SpellName} selected",
                transform,
                Color.yellow
            );
        }

        public void CastSelfTargetingSpell(PlayerSpellWrapper spell)
        {
            if (playerStats.CurrentMagic < spell.MagicCost)
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
            playerStats.ConsumeMagic(spell.MagicCost);
            ExecuteSpell(spell.BaseSpell, transform.position);

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

            if (playerStats.CurrentMagic < selectedSpell.MagicCost)
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

            playerStats.ConsumeMagic(selectedSpell.MagicCost);

            ExecuteSpell(selectedSpell.BaseSpell, targetPosition);

            PlayerUI.Instance.OnSpellCast(selectedSpell);
        }

        public void ExecuteSpell(PlayerSpell spell, Vector3 targetPosition)
        {
            switch (spell.type)
            {
                case SpellType.Projectile:
                    if (spell.spellName == "Lightning Bolt")
                    {
                        CastLightningBolt(spell, targetPosition);
                    }
                    else
                    {
                        CastProjectileSpell(spell, targetPosition);
                    }
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

        private void CastLightningBolt(PlayerSpell spell, Vector3 targetPosition)
        {
            GameObject lightning = Instantiate(
                spell.spellEffectPrefab,
                spellSpawnPoint.position,
                Quaternion.identity
            );
            LightningBoltController controller = lightning.GetComponent<LightningBoltController>();

            if (controller != null)
            {
                controller.CreateLightningBolt(
                    spellSpawnPoint.position,
                    targetPosition,
                    spell.damage,
                    spell.speed,
                    spell.lifetime
                );
                CreateStatusEffectsFromSpell(spell);
            }
        }

        private void CastProjectileSpell(PlayerSpell spell, Vector3 targetPosition)
        {
            GameObject spellObject = Instantiate(
                spell.spellEffectPrefab,
                spellSpawnPoint.position,
                Quaternion.identity
            );
            PlayerProjectile projectile = spellObject.GetComponent<PlayerProjectile>();

            if (projectile != null)
            {
                projectile.direction = (targetPosition - spellSpawnPoint.position).normalized;
                projectile.lifetime = spell.lifetime;
                projectile.collisionRadius = spell.collisionRadius;
                projectile.speed = spell.speed;
                projectile.damage = spell.damage;

                projectile.statusEffects = CreateStatusEffectsFromSpell(spell);
                projectile.SetTargetPosition(targetPosition);
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
                enemyStats.TakeDamage(spell.damage);
                if (enemyStats != null)
                {
                    foreach (var statusEffect in CreateStatusEffectsFromSpell(spell))
                    {
                        StatusEffectManager.Instance.AddStatusEffect(
                            enemyCollider.gameObject,
                            statusEffect
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

        public List<StatusEffect> CreateStatusEffectsFromSpell(PlayerSpell spell)
        {
            List<StatusEffect> statusEffects = new List<StatusEffect>();

            void AddEffect(StatusEffectType type)
            {
                GameObject prefab = statusEffectLibrary.GetEffectPrefab(type);
                if (prefab != null)
                {
                    StatusEffect effect = prefab.GetComponent<StatusEffect>();
                    if (effect != null)
                    {
                        statusEffects.Add(effect);
                    }
                }
            }

            if (spell.hasBurnEffect)
                AddEffect(StatusEffectType.Burn);
            if (spell.hasFreezeEffect)
                AddEffect(StatusEffectType.Freeze);
            if (spell.hasPoisonEffect)
                AddEffect(StatusEffectType.Poison);
            if (spell.hasStunEffect)
                AddEffect(StatusEffectType.Stun);
            if (spell.hasSlowEffect)
                AddEffect(StatusEffectType.Slow);
            if (spell.hasRegenEffect)
                AddEffect(StatusEffectType.Regen);
            if (spell.hasShieldEffect)
                AddEffect(StatusEffectType.Shield);
            if (spell.hasInvincibleEffect)
                AddEffect(StatusEffectType.Invincible);

            return statusEffects;
        }

        private bool IsSpellOnCooldown(PlayerSpellWrapper spell)
        {
            return PlayerUI.Instance.IsSpellOnCooldown(spell);
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    [System.Serializable]
    public class StatusEffectPrefab
    {
        public StatusEffectType effectType;
        public GameObject prefab;
    }
}
