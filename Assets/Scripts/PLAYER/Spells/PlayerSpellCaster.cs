using System.Collections.Generic;
using System.Linq;
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
        private Spell selectedSpell; // Currently selected spell

        [SerializeField]
        private LayerMask enemyLayer; // Layer mask to identify enemies

        private GameObject currentTarget;
        private List<GameObject> enemiesInRange = new List<GameObject>();
        private int currentTargetIndex = 0;
        private List<Spell> knownSpells = new List<Spell>();

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

        /// <summary>
        /// Prompts the player to choose a new spell.
        /// Filters the global spell database for spells learnable by the player's class that aren’t already known.
        /// </summary>
        public void PromptSpellLearning()
        {
            // Get the player's class from the GameManager.
            CharacterClass playerClass = GameManager.SelectedClass;

            // If the player's class doesn't support spell learning, simply exit.
            if (
                playerClass == null
                || playerClass.LevelToLearnSpells == null
                || playerClass.LevelToLearnSpells.Count == 0
            )
            {
                Debug.Log("This class does not learn spells.");
                return;
            }

            // Filter available spells based on the player's class.
            List<Spell> availableSpells = new List<Spell>(SpellsDatabase.Spells);
            foreach (Spell spell in SpellsDatabase.Spells)
            {
                if (spell.LearnableByClasses.Contains(playerClass) && !knownSpells.Contains(spell))
                {
                    availableSpells.Add(spell);
                }
            }

            if (availableSpells.Count == 0)
            {
                Debug.Log("No new spells available to learn.");
                return;
            }

            // Show the Spell Learning UI panel (this panel is assumed to let the player choose from availableSpells).
            SpellLearningPanel.Instance.Show(availableSpells, OnSpellChosen);
        }

        /// <summary>
        /// Callback for when a new spell is chosen by the player.
        /// Clones the spell from the database and adds it to the known spells.
        /// </summary>
        /// <param name="selectedSpell">The spell selected by the player.</param>
        private void OnSpellChosen(Spell selectedSpell)
        {
            // Clone the spell so that modifications are unique to this instance.
            Spell newSpell = selectedSpell.Clone();
            knownSpells.Add(newSpell);

            // Update the spells panel UI.
            PlayerUI.Instance.AddSpell(newSpell);

            Debug.Log($"New spell learned: {newSpell.SpellName}");
            Debug.Log(
                $"Stats of learned spell:\nDamageTypes: {newSpell.DamageTypes},\nMagicCost: {newSpell.MagicCost},\nLifetime: {newSpell.Lifetime},\nCollisionRadius: {newSpell.CollisionRadius},\nSpeed: {newSpell.Speed},\nCooldown: {newSpell.Cooldown},\nLevelUpThreshold: {newSpell.LevelUpThreshold},\nCriticalChance: {newSpell.CriticalChance},\nAreaOfEffect: {newSpell.AreaOfEffect},\nType: {newSpell.Type},\nSpellEffectPrefab: {newSpell.SpellEffectPrefab},\nSelfTargeting: {newSpell.SelfTargeting},\nCanChase: {newSpell.CanChase},\nLearnableByClasses: {newSpell.LearnableByClasses},\nSpellStatusEffects: {newSpell.SpellStatusEffects}"
            );
            Debug.Log(
                $"Effects of learned spell: {string.Join("\n", newSpell.StatusEffectDuration)}"
            );
        }

        // Optionally, provide a getter for known spells.
        public List<Spell> GetKnownSpells()
        {
            return knownSpells;
        }

        private void UpdateEnemiesInRange()
        {
            if (playerStats.HasEnteredDungeon == false)
            {
                return;
            }
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

        public void SelectSpell(Spell spell)
        {
            selectedSpell = spell;
            FloatingTextManager.Instance.ShowFloatingText(
                $"{spell.SpellName} selected",
                transform,
                Color.yellow
            );
        }

        public void CastSelfTargetingSpell(Spell spell)
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

        public void ExecuteSpell(Spell spell, Vector3 targetPosition)
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

        private void CastLightningBolt(Spell spell, Vector3 targetPosition)
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

                foreach (var effect in spell.SpellStatusEffects)
                {
                    StatusEffectManager.Instance.AddStatusEffect(
                        currentTarget,
                        effect,
                        spell.StatusEffectDuration
                    );
                }
            }
        }

        private void CastProjectileSpell(Spell spell, Vector3 targetPosition)
        {
            GameObject spellObject = Instantiate(
                spell.SpellEffectPrefab,
                spellSpawnPoint.position,
                Quaternion.identity
            );
            PlayerProjectile projectile = spellObject.GetComponent<PlayerProjectile>();

            if (projectile != null)
            {
                DamageInfo damageInfo = new DamageInfo(spell.DamageTypes, spell.SpellStatusEffects);

                projectile.Initialize(
                    damageInfo,
                    (targetPosition - spellSpawnPoint.position).normalized,
                    spell.StatusEffectDuration
                );

                projectile.lifetime = spell.Lifetime;
                projectile.collisionRadius = spell.CollisionRadius;
                projectile.speed = spell.Speed;
                projectile.SetTargetPosition(targetPosition);
            }
        }

        private void CastAoESpell(Spell spell, Vector3 targetPosition)
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
                        spell.SpellStatusEffects
                    );
                    enemyStats.TakeDamage(damageInfo);
                }
            }
        }

        private void CastHealSpell(Spell spell)
        {
            playerStats.Heal(spell.DamageTypes[DamageType.Physical]);
            if (spell.SpellEffectPrefab != null)
            {
                Instantiate(spell.SpellEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        private bool IsSpellOnCooldown(Spell spell)
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
}
