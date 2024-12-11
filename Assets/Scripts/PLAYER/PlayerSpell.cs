using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Magic/Spell")]
    public class PlayerSpell : ScriptableObject
    {
        [Header("Basic Attributes")]
        public string spellName;
        public Sprite icon;
        public int magicCost;
        public int damage;
        public float speed; // Speed of the projectile
        public float cooldown; // Time in seconds before the spell can be used again

        [Header("Advanced Attributes")]
        public SpellType type; // Enum: Projectile, AoE, Heal, etc.
        public float range; // Maximum casting distance
        public float areaOfEffect; // Radius for AoE spells; 0 for single target
        public GameObject spellEffectPrefab; // Visual effect prefab

        [Header("Status Effects")]
        public bool hasBurnEffect;
        public float burnDamage;
        public float burnDuration;

        public bool hasFreezeEffect;
        public float freezeDuration;

        [Header("Special Attributes")]
        public bool canChase; // If true, projectile will chase targets
        public bool isInstant; // If true, instantly affects the target
    }

    public enum SpellType
    {
        Projectile,
        AoE,
        Heal,
        // Add more types as needed
    }
}
