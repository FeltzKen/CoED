using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class MiniBoss
    {
        public string name;
        public string description;
        public int level;
        public int maxHP;
        public int attack;
        public int defense;
        public int speed;
        public int intelligence;
        public DamageType damageType;
        public float statusInflictionChance;
        public StatusEffectType inflictedStatusEffect;
        public Sprite miniBossSprite; // optional
    }

    public static class MiniBossesDatabase
    {
        public static List<MiniBoss> miniBosses = new List<MiniBoss>()
        {
            new MiniBoss
            {
                name = "Molten Guardian",
                description = "A heavy, armor-plated beast wreathed in molten lava streams.",
                level = 12,
                maxHP = 220,
                attack = 28,
                defense = 20,
                speed = 4,
                intelligence = 5,
                damageType = DamageType.Fire,
                statusInflictionChance = 0.22f,
                inflictedStatusEffect = StatusEffectType.Burn,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/MoltenGuardian"),
            },
            new MiniBoss
            {
                name = "Frost Commander",
                description =
                    "An elite soldier from the frozen north, channeling blizzards in battle.",
                level = 13,
                maxHP = 210,
                attack = 27,
                defense = 18,
                speed = 5,
                intelligence = 8,
                damageType = DamageType.Ice,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Slow,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/FrostCommander"),
            },
            new MiniBoss
            {
                name = "Shadow Ravager",
                description = "A stealthy, four-armed creature that spreads relentless curses.",
                level = 14,
                maxHP = 250,
                attack = 30,
                defense = 16,
                speed = 8,
                intelligence = 12,
                damageType = DamageType.Shadow,
                statusInflictionChance = 0.28f,
                inflictedStatusEffect = StatusEffectType.Curse,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/ShadowRavager"),
            },
            new MiniBoss
            {
                name = "Arcane Gargoyle",
                description =
                    "A gargoyle awakened by arcane magic, perched to attack with mystic bolts.",
                level = 14,
                maxHP = 240,
                attack = 32,
                defense = 20,
                speed = 5,
                intelligence = 14,
                damageType = DamageType.Arcane,
                statusInflictionChance = 0.2f,
                inflictedStatusEffect = StatusEffectType.Silence,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/ArcaneGargoyle"),
            },
            new MiniBoss
            {
                name = "Thundering Scorpion",
                description =
                    "An oversized scorpion with a lightning-charged stinger that paralyzes prey.",
                level = 15,
                maxHP = 270,
                attack = 35,
                defense = 18,
                speed = 7,
                intelligence = 6,
                damageType = DamageType.Lightning,
                statusInflictionChance = 0.3f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/ThunderingScorpion"
                ),
            },
            new MiniBoss
            {
                name = "Vile Blood Ogre",
                description =
                    "A hulking ogre infused with corrupt energies that cause deep bleeding wounds.",
                level = 15,
                maxHP = 280,
                attack = 36,
                defense = 16,
                speed = 5,
                intelligence = 4,
                damageType = DamageType.Bleed,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Bleed,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/VileBloodOgre"),
            },
            new MiniBoss
            {
                name = "Holy Warden",
                description =
                    "A zealous guardian of ancient ruins, channeling holy power to blind foes.",
                level = 16,
                maxHP = 300,
                attack = 32,
                defense = 22,
                speed = 5,
                intelligence = 10,
                damageType = DamageType.Holy,
                statusInflictionChance = 0.18f,
                inflictedStatusEffect = StatusEffectType.Blindness,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/HolyWarden"),
            },
            new MiniBoss
            {
                name = "Poison Broodmother",
                description = "A massive spider queen with a highly toxic bite.",
                level = 16,
                maxHP = 290,
                attack = 34,
                defense = 20,
                speed = 7,
                intelligence = 5,
                damageType = DamageType.Poison,
                statusInflictionChance = 0.35f,
                inflictedStatusEffect = StatusEffectType.Poison,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/PoisonBroodmother"
                ),
            },
            new MiniBoss
            {
                name = "Crimson Champion",
                description =
                    "A fierce knight clad in blood-red armor, empowered by violent storms.",
                level = 17,
                maxHP = 320,
                attack = 38,
                defense = 24,
                speed = 6,
                intelligence = 8,
                damageType = DamageType.Lightning,
                statusInflictionChance = 0.2f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/CrimsonChampion"
                ),
            },
            new MiniBoss
            {
                name = "Abomination of Rot",
                description = "A mutated beast patchworked with limbs, exuding disease and decay.",
                level = 17,
                maxHP = 350,
                attack = 40,
                defense = 22,
                speed = 3,
                intelligence = 3,
                damageType = DamageType.Poison,
                statusInflictionChance = 0.4f,
                inflictedStatusEffect = StatusEffectType.Poison,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/AbominationOfRot"
                ),
            },
            new MiniBoss
            {
                name = "Tempest Wyvern",
                description = "A mid-level wyvern that commands fierce winds and lightning breath.",
                level = 17,
                maxHP = 310,
                attack = 37,
                defense = 20,
                speed = 8,
                intelligence = 10,
                damageType = DamageType.Lightning,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/TempestWyvern"),
            },
            new MiniBoss
            {
                name = "Arcane Matriarch",
                description =
                    "An ancient sorceress bound to arcane energies, draining magic from foes.",
                level = 18,
                maxHP = 330,
                attack = 35,
                defense = 25,
                speed = 7,
                intelligence = 20,
                damageType = DamageType.Arcane,
                statusInflictionChance = 0.28f,
                inflictedStatusEffect = StatusEffectType.Silence,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/ArcaneMatriarch"
                ),
            },
            new MiniBoss
            {
                name = "Gorefiend Raven",
                description = "A colossal raven that pecks with bleed-inflicting beak strikes.",
                level = 18,
                maxHP = 340,
                attack = 38,
                defense = 18,
                speed = 9,
                intelligence = 6,
                damageType = DamageType.Bleed,
                statusInflictionChance = 0.33f,
                inflictedStatusEffect = StatusEffectType.Bleed,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/GorefiendRaven"),
            },
            new MiniBoss
            {
                name = "Sunscale Basilisk",
                description =
                    "A giant basilisk harnessing solar fire. Its gaze both burns and petrifies.",
                level = 19,
                maxHP = 370,
                attack = 42,
                defense = 26,
                speed = 7,
                intelligence = 10,
                damageType = DamageType.Fire,
                statusInflictionChance = 0.3f,
                inflictedStatusEffect = StatusEffectType.Burn,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/SunscaleBasilisk"
                ),
            },
            new MiniBoss
            {
                name = "Starfallen Guardian",
                description =
                    "An armored knight rumored to have descended from the stars, striking with cosmic might.",
                level = 20,
                maxHP = 390,
                attack = 45,
                defense = 28,
                speed = 8,
                intelligence = 12,
                damageType = DamageType.Holy,
                statusInflictionChance = 0.2f,
                inflictedStatusEffect = StatusEffectType.Blindness,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/StarfallenGuardian"
                ),
            },
        };
    }
}
