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
        public int spellLevel; // need to implement this
        public int levelUpThreshold = 5; // Player level required for spell to level up

        [Header("Advanced Attributes")]
        public SpellType type;
        public float areaOfEffect;
        public GameObject spellEffectPrefab;

        [Header("Status Effects")]
        public bool hasPoisonEffect;
        public bool hasFreezeEffect;
        public bool hasBurnEffect;
        public bool hasSlowEffect;
        public bool hasStunEffect;
        public bool hasRegenEffect;
        public bool hasShieldEffect;
        public bool hasInvincibleEffect;

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
