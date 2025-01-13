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
        public float lifetime;
        public float collisionRadius;
        public float speed;
        public float cooldown;
        public int spellLevel;
        public int levelUpThreshold = 5; // Player level multiple required for spell to level up

        [Header("Advanced Attributes")]
        public float areaOfEffect;
        public SpellType type;

        public GameObject spellEffectPrefab;

        [Header("Special Attributes")]
        public bool selfTargeting;
        public bool canChase;
        public bool isInstant;
    }

    public enum SpellType
    {
        Projectile,
        AoE,
        Heal,
        Buff,
        Debuff,
    }
}
