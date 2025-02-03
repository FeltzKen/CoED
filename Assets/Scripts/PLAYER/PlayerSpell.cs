using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Magic/Spell")]
    public class PlayerSpell : ScriptableObject
    {
        [Header("Basic Attributes")]
        public string spellName;
        public Sprite icon;
        public int magicCost = 10;
        public int damage = 10;
        public float lifetime = 1f;
        public float collisionRadius = 0.5f;
        public float speed = 2f;
        public float cooldown = 5f;
        public int levelUpThreshold = 5; // Player level multiple required for spell to level up
        public DamageType damageType = DamageType.Physical;
        public float criticalChance = 0.1f;

        [Header("Advanced Attributes")]
        public float areaOfEffect = 0f;
        public SpellType type;

        [Header("Gainable Status Effects")]
        public List<StatusEffectType> gainableStatusEffects = new();

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
