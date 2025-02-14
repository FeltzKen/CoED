using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class SpellsDatabase
    {
        public static List<Spell> Spells = new List<Spell>()
        {
            // 1. Fireball
            new Spell(
                spellID: "fireball001",
                spellName: "Fireball",
                icon: Resources.Load<Sprite>("Sprites/Spells/Fireball")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 300,
                magicCost: 1,
                lifetime: 1f,
                collisionRadius: 0.5f,
                speed: 2f,
                cooldown: .5f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Fire, 10 } },
                criticalChance: 0.1f,
                areaOfEffect: 0f,
                type: SpellType.Projectile,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Burn },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/FireballEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[1], // Mage
                    CharacterClasses.Classes[10], // Sorcerer
                    CharacterClasses.Classes[11], // Warlock
                }
            ),
            // 2. Ice Shard
            new Spell(
                spellID: "iceshard001",
                spellName: "Ice Shard",
                icon: Resources.Load<Sprite>("Sprites/Spells/IceShard")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 12,
                lifetime: 1.2f,
                collisionRadius: 0.4f,
                speed: 2.5f,
                cooldown: 4.8f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Ice, 12 } },
                criticalChance: 0.08f,
                areaOfEffect: 0f,
                type: SpellType.Projectile,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Freeze },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/IceShardEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[1], // Mage
                    CharacterClasses.Classes[2], // Rogue
                    CharacterClasses.Classes[7], // Druid
                }
            ),
            // 3. Heal
            new Spell(
                spellID: "heal001",
                spellName: "Heal",
                icon: Resources.Load<Sprite>("Sprites/Spells/Heal")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 15,
                lifetime: 1f,
                collisionRadius: 0f,
                speed: 0f,
                cooldown: 6f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Heal, -15 } }, // Use an appropriate heal type
                criticalChance: 0f,
                areaOfEffect: 0f,
                type: SpellType.Heal,
                spellDataStatusEffects: new List<StatusEffectType>(),
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/HealEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: true,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[3], // Cleric
                    CharacterClasses.Classes[4], // Paladin
                    CharacterClasses.Classes[7], // Druid
                }
            ),
            // 4. Lightning Bolt
            new Spell(
                spellID: "lightning001",
                spellName: "Lightning Bolt",
                icon: Resources.Load<Sprite>("Sprites/Spells/LightningBolt")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 18,
                lifetime: 0.8f,
                collisionRadius: 0.6f,
                speed: 3f,
                cooldown: 5.5f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Lightning, 14 } },
                criticalChance: 0.12f,
                areaOfEffect: 0f,
                type: SpellType.Projectile,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Stun },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/LightningBoltEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[1], // Mage
                    CharacterClasses.Classes[4], // Paladin
                    CharacterClasses.Classes[11], // Warlock
                }
            ),
            // 5. Earthquake
            new Spell(
                spellID: "earthquake001",
                spellName: "Earthquake",
                icon: Resources.Load<Sprite>("Sprites/Spells/Earthquake")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 20,
                lifetime: 2f,
                collisionRadius: 1f,
                speed: 0f, // AoE spell without projectile movement
                cooldown: 7f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Physical, 15 } },
                criticalChance: 0.05f,
                areaOfEffect: 5f,
                type: SpellType.AoE,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Paralyze },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/EarthquakeEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[3], // Cleric
                }
            ),
            // 6. Poison Cloud
            new Spell(
                spellID: "poisoncloud001",
                spellName: "Poison Cloud",
                icon: Resources.Load<Sprite>("Sprites/Spells/PoisonCloud")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 16,
                lifetime: 3f,
                collisionRadius: 0f,
                speed: 0f,
                cooldown: 8f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Poison, 18 } },
                criticalChance: 0f,
                areaOfEffect: 4f,
                type: SpellType.AoE,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Poison },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/PoisonCloudEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[2], // Rogue
                    CharacterClasses.Classes[9], // Necromancer
                    CharacterClasses.Classes[11], // Warlock
                }
            ),
            // 7. Shadow Strike
            new Spell(
                spellID: "shadowstrike001",
                spellName: "Shadow Strike",
                icon: Resources.Load<Sprite>("Sprites/Spells/ShadowStrike")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 14,
                lifetime: 0.5f,
                collisionRadius: 0.3f,
                speed: 4f,
                cooldown: 6f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Shadow, 20 } },
                criticalChance: 0.15f,
                areaOfEffect: 0f,
                type: SpellType.Projectile,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Bleed },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/ShadowStrikeEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: true,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[11], // Warlock
                }
            ),
            // 8. Nature's Grasp
            new Spell(
                spellID: "naturesgrasp001",
                spellName: "Nature's Grasp",
                icon: Resources.Load<Sprite>("Sprites/Spells/NaturesGrasp")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 13,
                lifetime: 2.5f,
                collisionRadius: 0.7f,
                speed: 1.5f,
                cooldown: 6.5f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Nature, 9 } },
                criticalChance: 0.05f,
                areaOfEffect: 3f,
                type: SpellType.AoE,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Root },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/NaturesGraspEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[7], // Druid
                    CharacterClasses.Classes[6], // Bard
                }
            ),
            // 9. Holy Light
            new Spell(
                spellID: "holylight001",
                spellName: "Holy Light",
                icon: Resources.Load<Sprite>("Sprites/Spells/HolyLight")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 17,
                lifetime: 1.2f,
                collisionRadius: 0f,
                speed: 0f,
                cooldown: 7f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Heal, -20 } },
                criticalChance: 0f,
                areaOfEffect: 0f,
                type: SpellType.Heal,
                spellDataStatusEffects: new List<StatusEffectType>(),
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/HolyLightEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: true,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[3], // Cleric
                    CharacterClasses.Classes[4], // Paladin
                    CharacterClasses.Classes[6], // Bard
                }
            ),
            // 10. Arcane Missile
            new Spell(
                spellID: "arcanemissile001",
                spellName: "Arcane Missile",
                icon: Resources.Load<Sprite>("Sprites/Spells/ArcaneMissile")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 14,
                lifetime: 1f,
                collisionRadius: 0.3f,
                speed: 3.5f,
                cooldown: 5f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Arcane, 15 } },
                criticalChance: 0.1f,
                areaOfEffect: 0f,
                type: SpellType.Projectile,
                spellDataStatusEffects: new List<StatusEffectType>(),
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/ArcaneMissileEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: true,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[1], // Mage
                    CharacterClasses.Classes[10], // Sorcerer
                    CharacterClasses.Classes[11], // Warlock
                }
            ),
            // 11. Wind Gust
            new Spell(
                spellID: "windgust001",
                spellName: "Wind Gust",
                icon: Resources.Load<Sprite>("Sprites/Spells/WindGust")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 11,
                lifetime: 1.5f,
                collisionRadius: 0.5f,
                speed: 2.8f,
                cooldown: 4.5f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Physical, 16 } },
                criticalChance: 0.05f,
                areaOfEffect: 3f,
                type: SpellType.AoE,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Slow },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/WindGustEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: false,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[6], // Bard
                    CharacterClasses.Classes[8], // Monk
                }
            ),
            // 12. Berserk
            new Spell(
                spellID: "berserk001",
                spellName: "Berserk",
                icon: Resources.Load<Sprite>("Sprites/Spells/Berserk")
                    ?? Resources.Load<Sprite>("Sprites/Spells/Default"),
                statusEffectDuration: 15,
                magicCost: 16,
                lifetime: 3f,
                collisionRadius: 0f,
                speed: 0f,
                cooldown: 8f,
                levelUpThreshold: 5,
                damageTypes: new Dictionary<DamageType, float> { { DamageType.Physical, 0 } },
                criticalChance: 0f,
                areaOfEffect: 0f,
                type: SpellType.Buff,
                spellDataStatusEffects: new List<StatusEffectType> { StatusEffectType.Berserk },
                spellEffectPrefab: Resources.Load<GameObject>("Prefabs/Spells/BerserkEffect")
                    ?? Resources.Load<GameObject>("Prefabs/Spells/Default"),
                selfTargeting: true,
                canChase: false,
                learnableByClasses: new List<CharacterClass>
                {
                    CharacterClasses.Classes[8], // Monk
                }
            ),
        };
    }
}
