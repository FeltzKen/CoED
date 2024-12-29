using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Magic/Spell")]
    public class PlayerSpell : ScriptableObject
    {
        [Header("Basic Attributes")]
        public string spellName;
        public Sprite icon;
        public float magicCost;
        public float damage;
        public float lifetime;
        public float collisionRadius;
        public float speed;
        public float cooldown;

        [Header("Advanced Attributes")]
        public SpellType type;
        public float range;
        public float areaOfEffect;
        public GameObject spellEffectPrefab;

        [Header("Status Effects")]
        public bool hasBurnEffect;
        public float burnDamage;
        public float burnDuration;

        public bool hasFreezeEffect;
        public float freezeDuration;

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
