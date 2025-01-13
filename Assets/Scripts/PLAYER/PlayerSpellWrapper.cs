using NUnit.Framework;
using UnityEngine;

namespace CoED
{
    public class PlayerSpellWrapper : MonoBehaviour
    {
        public PlayerSpell BaseSpell { get; private set; }
        public Sprite Icon => BaseSpell.icon;

        public string SpellName => BaseSpell.spellName;
        public int MagicCost { get; private set; }
        public int Damage { get; private set; }
        public float Lifetime { get; private set; }
        public float CollisionRadius { get; private set; }
        public float Speed { get; private set; }
        public float Cooldown { get; private set; }
        public int SpellLevel { get; private set; }
        public StatusEffect EffectType;
        public GameObject SpellEffectPrefab => BaseSpell.spellEffectPrefab;
        public SpellType Type => BaseSpell.type;
        public bool IsInstant => BaseSpell.isInstant;
        public bool CanChase => BaseSpell.canChase;
        public bool SelfTargeting => BaseSpell.selfTargeting;
        public int LevelUpThreshold => BaseSpell.levelUpThreshold;
        public float AreaOfEffect => BaseSpell.areaOfEffect;

        // Add other relevant properties as needed
        public void Initialize(PlayerSpell baseSpell)
        {
            BaseSpell = baseSpell;
            MagicCost = baseSpell.magicCost;
            Damage = baseSpell.damage;
            Lifetime = baseSpell.lifetime;
            CollisionRadius = baseSpell.collisionRadius;
            Speed = baseSpell.speed;
            Cooldown = baseSpell.cooldown;
            SpellLevel = 1; // Start at level 1
        }

        public void LevelUp()
        {
            SpellLevel++;
            Damage += 1;
            Cooldown *= 0.9f;
            MagicCost += 1;

            Debug.Log($"{SpellName} leveled up to Level {SpellLevel}!");
        }

        public void ResetToBaseValues()
        {
            MagicCost = BaseSpell.magicCost;
            Damage = BaseSpell.damage;
            Lifetime = BaseSpell.lifetime;
            CollisionRadius = BaseSpell.collisionRadius;
            Speed = BaseSpell.speed;
            Cooldown = BaseSpell.cooldown;
            SpellLevel = 1; // Start at base level
        }
    }
}
