using System.Collections.Generic;
using Unity.VisualScripting;
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
            foreach (var baseSpell in spellPrefabs)
            {
                GameObject spellWrapperObj = new GameObject($"{baseSpell.spellName}_Wrapper");
                PlayerSpellWrapper spellWrapper =
                    spellWrapperObj.AddComponent<PlayerSpellWrapper>();

                // Initialize wrapper with base spell
                spellWrapper.Initialize(baseSpell);

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
                playerStats.GetCurrentAttackRange(),
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
                _Enemy enemyComponent = currentTarget.GetComponent<_Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.SetHighlighted(false);
                }
            }

            currentTarget = target;
            _Enemy newTargetComponent = currentTarget.GetComponent<_Enemy>();
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
            if (playerStats.GetCurrentMagic() < spell.MagicCost)
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
            if (playerStats.GetCurrentHealth() == playerStats.GetCurrentMaxHealth())
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Already at full health",
                    transform,
                    Color.red
                );
                return;
            }
            playerStats.ConsumeMagic(spell.MagicCost);
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

            if (playerStats.GetCurrentMagic() < selectedSpell.MagicCost)
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

            ExecuteSpell(selectedSpell, targetPosition);

            PlayerUI.Instance.OnSpellCast(selectedSpell);
        }

        public void ExecuteSpell(PlayerSpellWrapper spell, Vector3 targetPosition)
        {
            switch (spell.Type)
            {
                case SpellType.Projectile:
                    if (spell.SpellName == "Lightning Bolt")
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
                    Debug.LogWarning($"Unsupported spell type: {spell.Type}");
                    break;
            }
        }

        private void CastLightningBolt(PlayerSpellWrapper spell, Vector3 targetPosition)
        {
            GameObject lightning = Instantiate(
                spell.SpellEffectPrefab,
                spellSpawnPoint.position,
                Quaternion.identity
            );
            LightningBoltController controller = lightning.GetComponent<LightningBoltController>();

            if (controller != null)
            {
                controller.CreateLightningBolt(
                    spellSpawnPoint.position,
                    targetPosition,
                    spell.Speed,
                    spell.Lifetime
                );

                foreach (var effect in spell.InflictedStatusEffectTypes)
                {
                    StatusEffectManager.Instance.AddStatusEffect(currentTarget, effect);
                }
            }
        }

        private void CastProjectileSpell(PlayerSpellWrapper spell, Vector3 targetPosition)
        {
            GameObject spellObject = Instantiate(
                spell.SpellEffectPrefab,
                spellSpawnPoint.position,
                Quaternion.identity
            );
            PlayerProjectile projectile = spellObject.GetComponent<PlayerProjectile>();

            if (projectile != null)
            {
                DamageInfo damageInfo = new DamageInfo(
                    spell.DamageTypes,
                    spell.InflictedStatusEffectTypes
                );

                projectile.Initialize(
                    damageInfo,
                    (targetPosition - spellSpawnPoint.position).normalized
                );

                projectile.lifetime = spell.Lifetime;
                projectile.collisionRadius = spell.CollisionRadius;
                projectile.speed = spell.Speed;
                projectile.SetTargetPosition(targetPosition);
            }
        }

        private void CastAoESpell(PlayerSpellWrapper spell, Vector3 targetPosition)
        {
            if (spell.SpellEffectPrefab != null)
            {
                Instantiate(spell.SpellEffectPrefab, targetPosition, Quaternion.identity);
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                targetPosition,
                spell.AreaOfEffect,
                LayerMask.GetMask("enemies")
            );

            foreach (var enemyCollider in hitEnemies)
            {
                _EnemyStats enemyStats = enemyCollider.GetComponent<_EnemyStats>();
                if (enemyStats != null)
                {
                    // ✅ Package dynamic damage and effects
                    DamageInfo damageInfo = new DamageInfo(
                        spell.DamageTypes,
                        spell.InflictedStatusEffectTypes
                    );
                    enemyStats.TakeDamage(damageInfo);
                }
            }
        }

        private void CastHealSpell(PlayerSpellWrapper spell)
        {
            playerStats.Heal(spell.DamageTypes[DamageType.Physical]);
            if (spell.SpellEffectPrefab != null)
            {
                Instantiate(spell.SpellEffectPrefab, transform.position, Quaternion.identity);
            }
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
