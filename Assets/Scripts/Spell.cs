using UnityEngine;

namespace YourGameNamespace
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Magic/Spell")]
    public class Spell : ScriptableObject
    {
        [Header("Basic Attributes")]
        [SerializeField] private string spellName;
        [SerializeField] private Sprite icon;
        [SerializeField] private int magicCost;
        [SerializeField] private int damage;
        [SerializeField] private float cooldown;  // Time in seconds before the spell can be used again

        [Header("Advanced Attributes")]
        [SerializeField] private SpellType type;
        [SerializeField] private float range = 10f;
        [SerializeField] private float areaOfEffect = 0f;  // 0 for single target spells
        [SerializeField] private GameObject spellEffectPrefab;

        // Properties for accessing fields
        public string SpellName => spellName;
        public Sprite Icon => icon;
        public int MagicCost => magicCost;
        public int Damage => damage;
        public float Cooldown => cooldown;
        public SpellType Type => type;
        public float Range => range;
        public float AreaOfEffect => areaOfEffect;
        public GameObject SpellEffectPrefab => spellEffectPrefab;

        public void Cast(Transform caster, Vector3 targetPosition)
        {
            if (spellEffectPrefab != null)
            {
                Instantiate(spellEffectPrefab, targetPosition, Quaternion.identity);
                Debug.Log($"Spell '{spellName}' cast at position {targetPosition}.");
            }
            else
            {
                Debug.LogWarning($"Spell '{spellName}': SpellEffectPrefab is not assigned.");
            }

            ApplySpellEffect(caster, targetPosition);
        }

        private void ApplySpellEffect(Transform caster, Vector3 targetPosition)
        {
            Collider2D[] affectedTargets = Physics2D.OverlapCircleAll(targetPosition, areaOfEffect);
            foreach (var target in affectedTargets)
            {
                if (target.TryGetComponent(out IActor actor) && actor != caster.GetComponent<IActor>())
                {
                    PlayerStats.Instance.TakeDamage(damage);
                }
            }
        }

        // Method to set spell data (useful for creating default spells on the fly)
        public void SetSpellData(string name, Sprite icon, int mpCost, int dmg, float cd, SpellType sType, float range, float aoe, GameObject prefab)
        {
            this.spellName = name;
            this.icon = icon;
            this.magicCost = mpCost;
            this.damage = dmg;
            this.cooldown = cd;
            this.type = sType;
            this.range = range;
            this.areaOfEffect = aoe;
            this.spellEffectPrefab = prefab;
        }
    }
}
