using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class MiniBoss
    {
        public string name;
        public string description;

        public Dictionary<Stat, float> MiniBossStats = new Dictionary<Stat, float>()
        {
            { Stat.MaxHP, 0 },
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 },
            { Stat.Intelligence, 0 },
            { Stat.ChanceToInflictStatusEffect, 0 },
            { Stat.StatusEffectDuration, 0 },
            { Stat.PatrolSpeed, 0 },
            { Stat.CritChance, 0 },
            { Stat.CritDamage, 0 },
            { Stat.FireRate, 0 },
            { Stat.Shield, 0 },
            { Stat.Accuracy, 0 },
            { Stat.ElementalDamage, 0 },
            { Stat.Dexterity, 0 },
            { Stat.ProjectileRange, 0 },
            { Stat.AttackRange, 0 },
            { Stat.ChaseSpeed, 0 },
        };
        public int level;
        public DamageType damageType;
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

                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 200 },
                    { Stat.Attack, 28 },
                    { Stat.Defense, 20 },
                    { Stat.Speed, 4 },
                    { Stat.Intelligence, 5 },
                    { Stat.ChanceToInflictStatusEffect, 0.22f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 3 },
                    { Stat.CritChance, 0.25f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Fire,
                inflictedStatusEffect = StatusEffectType.Burn,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/MoltenGuardian"),
            },
            new MiniBoss
            {
                name = "Frost Commander",
                description =
                    "An elite soldier from the frozen north, channeling blizzards in battle.",
                level = 13,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 220 },
                    { Stat.Attack, 26 },
                    { Stat.Defense, 18 },
                    { Stat.Speed, 6 },
                    { Stat.Intelligence, 10 },
                    { Stat.ChanceToInflictStatusEffect, 0.25f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 4 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Ice,
                inflictedStatusEffect = StatusEffectType.Slow,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/FrostCommander"),
            },
            new MiniBoss
            {
                name = "Shadow Ravager",
                description = "A stealthy, four-armed creature that spreads relentless curses.",
                level = 14,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 230 },
                    { Stat.Attack, 30 },
                    { Stat.Defense, 16 },
                    { Stat.Speed, 8 },
                    { Stat.Intelligence, 8 },
                    { Stat.ChanceToInflictStatusEffect, 0.28f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 5 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Shadow,
                inflictedStatusEffect = StatusEffectType.Curse,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/ShadowRavager"),
            },
            new MiniBoss
            {
                name = "Arcane Gargoyle",
                description =
                    "A gargoyle awakened by arcane magic, perched to attack with mystic bolts.",
                level = 14,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 240 },
                    { Stat.Attack, 32 },
                    { Stat.Defense, 18 },
                    { Stat.Speed, 6 },
                    { Stat.Intelligence, 12 },
                    { Stat.ChanceToInflictStatusEffect, 0.2f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 6 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Arcane,
                inflictedStatusEffect = StatusEffectType.Silence,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/ArcaneGargoyle"),
            },
            new MiniBoss
            {
                name = "Thundering Scorpion",
                description =
                    "An oversized scorpion with a lightning-charged stinger that paralyzes prey.",
                level = 15,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 260 },
                    { Stat.Attack, 34 },
                    { Stat.Defense, 20 },
                    { Stat.Speed, 7 },
                    { Stat.Intelligence, 6 },
                    { Stat.ChanceToInflictStatusEffect, 0.3f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 7 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Lightning,
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
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 280 },
                    { Stat.Attack, 36 },
                    { Stat.Defense, 16 },
                    { Stat.Speed, 5 },
                    { Stat.Intelligence, 4 },
                    { Stat.ChanceToInflictStatusEffect, 0.25f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 8 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Bleed,
                inflictedStatusEffect = StatusEffectType.Bleed,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/VileBloodOgre"),
            },
            new MiniBoss
            {
                name = "Holy Warden",
                description =
                    "A zealous guardian of ancient ruins, channeling holy power to blind foes.",
                level = 16,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 300 },
                    { Stat.Attack, 32 },
                    { Stat.Defense, 22 },
                    { Stat.Speed, 5 },
                    { Stat.Intelligence, 10 },
                    { Stat.ChanceToInflictStatusEffect, 0.18f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 9 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Holy,
                inflictedStatusEffect = StatusEffectType.Blindness,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/HolyWarden"),
            },
            new MiniBoss
            {
                name = "Poison Broodmother",
                description = "A massive spider queen with a highly toxic bite.",
                level = 16,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 290 },
                    { Stat.Attack, 34 },
                    { Stat.Defense, 20 },
                    { Stat.Speed, 7 },
                    { Stat.Intelligence, 5 },
                    { Stat.ChanceToInflictStatusEffect, 0.35f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 10 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Poison,
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
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 320 },
                    { Stat.Attack, 38 },
                    { Stat.Defense, 24 },
                    { Stat.Speed, 6 },
                    { Stat.Intelligence, 8 },
                    { Stat.ChanceToInflictStatusEffect, 0.2f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 11 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Lightning,
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
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 350 },
                    { Stat.Attack, 40 },
                    { Stat.Defense, 22 },
                    { Stat.Speed, 3 },
                    { Stat.Intelligence, 3 },
                    { Stat.ChanceToInflictStatusEffect, 0.4f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 12 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Poison,
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
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 310 },
                    { Stat.Attack, 37 },
                    { Stat.Defense, 20 },
                    { Stat.Speed, 8 },
                    { Stat.Intelligence, 10 },
                    { Stat.ChanceToInflictStatusEffect, 0.25f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 13 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Lightning,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/TempestWyvern"),
            },
            new MiniBoss
            {
                name = "Arcane Matriarch",
                description =
                    "An ancient sorceress bound to arcane energies, draining magic from foes.",
                level = 18,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 330 },
                    { Stat.Attack, 35 },
                    { Stat.Defense, 25 },
                    { Stat.Speed, 7 },
                    { Stat.Intelligence, 20 },
                    { Stat.ChanceToInflictStatusEffect, 0.28f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 14 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Arcane,
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
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 340 },
                    { Stat.Attack, 38 },
                    { Stat.Defense, 18 },
                    { Stat.Speed, 9 },
                    { Stat.Intelligence, 6 },
                    { Stat.ChanceToInflictStatusEffect, 0.33f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 15 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Bleed,
                inflictedStatusEffect = StatusEffectType.Bleed,
                miniBossSprite = Resources.Load<Sprite>("Sprites/Monsters/MiniBoss/GorefiendRaven"),
            },
            new MiniBoss
            {
                name = "Sunscale Basilisk",
                description =
                    "A giant basilisk harnessing solar fire. Its gaze both burns and petrifies.",
                level = 19,
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 370 },
                    { Stat.Attack, 42 },
                    { Stat.Defense, 26 },
                    { Stat.Speed, 7 },
                    { Stat.Intelligence, 10 },
                    { Stat.ChanceToInflictStatusEffect, 0.3f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 16 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Fire,
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
                MiniBossStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, 390 },
                    { Stat.Attack, 45 },
                    { Stat.Defense, 28 },
                    { Stat.Speed, 8 },
                    { Stat.Intelligence, 12 },
                    { Stat.ChanceToInflictStatusEffect, 0.2f },
                    { Stat.StatusEffectDuration, 30f },
                    { Stat.PatrolSpeed, 17 },
                    { Stat.CritChance, 0.2f },
                    { Stat.CritDamage, 1.5f },
                    { Stat.FireRate, 0.5f },
                    { Stat.Shield, 0 },
                    { Stat.Accuracy, 10 },
                    { Stat.ElementalDamage, 50 },
                    { Stat.Dexterity, 19 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
                damageType = DamageType.Holy,
                inflictedStatusEffect = StatusEffectType.Blindness,
                miniBossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/MiniBoss/StarfallenGuardian"
                ),
            },
        };
    }
}
