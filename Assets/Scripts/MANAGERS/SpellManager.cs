using UnityEngine;

namespace CoED
{
    public class SpellManager : MonoBehaviour
    {
        public static SpellManager Instance { get; private set; }
        [Header("Dependencies")]
        [SerializeField] private PlayerMagic playerMagic;
        [SerializeField] private PlayerCombat playerCombat;
        [SerializeField] private ProjectileManager projectileManager;
        [SerializeField] private MagicPanel magicPanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            if (!playerMagic || !playerCombat || !projectileManager || !magicPanel)
            {
                Debug.LogError("SpellManager: Missing dependencies. Ensure all references are assigned.");
                enabled = false;
                return;
            }
        }

        public void CastSpell(Vector3 targetPosition, int spellIndex)
        {
            Spell selectedSpell = magicPanel.GetSpellAtIndex(spellIndex);
            if (selectedSpell == null)
            {
                Debug.LogWarning("SpellManager: No spell equipped at this index.");
                return;
            }

            if (!playerMagic.HasEnoughMagic(selectedSpell.MagicCost))
            {
                Debug.LogWarning("SpellManager: Not enough magic to cast the spell.");
                return;
            }

            playerMagic.ConsumeMagic(selectedSpell.MagicCost);
            ExecuteSpell(selectedSpell, targetPosition);
        }

        private void ExecuteSpell(Spell spell, Vector3 targetPosition)
        {
            switch (spell.Type)
            {
                case SpellType.Projectile:
                    projectileManager.LaunchProjectile(playerCombat.transform.position, targetPosition);
                    break;
                case SpellType.AoE:
                    ApplyAoEEffect(spell, targetPosition);
                    break;
                case SpellType.Heal:
                    playerMagic.RefillMagic(spell.Damage);
                    break;
                default:
                    Debug.LogWarning("SpellManager: Unsupported spell type.");
                    break;
            }
        }

        private void ApplyAoEEffect(Spell spell, Vector3 targetPosition)
        {
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(targetPosition, spell.AreaOfEffect);
            foreach (var target in hitTargets)
            {
                if (target.CompareTag("Enemy"))
                {
                    EnemyStats enemy = target.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(spell.Damage);
                    }
                }
            }
        }
    }
}
