using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class MonstersDatabase
    {
        #region Tier 1 Monsters
        // Tier 1 Monsters
        public static List<Monster> tierOneEnemies = new List<Monster>()
        {
            new Monster(
                monsterName: "Feral Slime",
                description: "A viscous blob oozing toxic slime. It has a decent chance to poison those who touch it.",
                1,
                damageType: DamageType.Poison,
                0.10f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Poison },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 50 },
                    { Stat.Attack, 5 },
                    { Stat.Defense, 2 },
                    { Stat.Speed, 3 },
                    { Stat.ElementalDamage, 2 },
                    { Stat.ChanceToInflict, 10 },
                    { Stat.StatusEffectDuration, 5 },
                },
                immunities: null,
                resistances: new List<Resistances> { Resistances.Physical },
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FeralSlime")
            ),
            new Monster(
                monsterName: "Crackle Beetle",
                description: "A small beetle with sparking antennae that deliver paralyzing jolts.",
                1,
                damageType: DamageType.Lightning,
                0.05f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Paralyze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 40 },
                    { Stat.Attack, 4 },
                    { Stat.Defense, 3 },
                    { Stat.Speed, 4 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Ice },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/Beetle")
            ),
            new Monster(
                monsterName: "Ash Rat",
                description: "A feral rat thriving amidst volcanic ash, leaving scorched wounds.",
                1,
                damageType: DamageType.Physical,
                0.08f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.None },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 35 },
                    { Stat.Attack, 3 },
                    { Stat.Defense, 2 },
                    { Stat.Speed, 5 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/Rat")
            ),
            new Monster(
                monsterName: "Frost Pup",
                description: "A tiny, icy wolf with a chilling breath that can freeze its prey.",
                2,
                damageType: DamageType.Ice,
                0.12f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Freeze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 70 },
                    { Stat.Attack, 6 },
                    { Stat.Defense, 4 },
                    { Stat.Speed, 6 },
                    { Stat.MaxMagic, 10 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FrostPup")
            ),
            new Monster(
                monsterName: "Stone Crawler",
                description: "A lumbering bug with a rock-hard exoskeleton. It’s slow but tough.",
                2,
                damageType: DamageType.Physical,
                0f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.None },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 100 },
                    { Stat.Attack, 8 },
                    { Stat.Defense, 10 },
                    { Stat.Speed, 2 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Lightning },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/StoneCrawler")
            ),
        };
        #endregion

        #region Tier 2 Monsters
        public static List<Monster> tierTwoEnemies = new List<Monster>()
        {
            // Tier 2 Monsters
            new Monster(
                monsterName: "Lightbringer Golem",
                description: "A massive, sentient statue of holy light built to smite evil.",
                3,
                damageType: DamageType.Holy,
                0.06f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Blindness },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 150 },
                    { Stat.Attack, 10 },
                    { Stat.Defense, 12 },
                    { Stat.Speed, 3 },
                },
                immunities: new List<Immunities> { Immunities.Physical },
                resistances: new List<Resistances> { Resistances.Holy },
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/LightbringerGolem")
            ),
            new Monster(
                monsterName: "Frost Creature",
                description: "A being of pure ice that chills enemies with its frosty aura.",
                3,
                damageType: DamageType.Ice,
                0.10f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Freeze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 140 },
                    { Stat.Attack, 9 },
                    { Stat.Defense, 10 },
                    { Stat.Speed, 4 },
                },
                immunities: new List<Immunities> { Immunities.Ice },
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FrostCreature")
            ),
            new Monster(
                monsterName: "Wisp of Dawn",
                description: "A flickering spirit of holy energy, banishing curses with each touch.",
                2,
                damageType: DamageType.Holy,
                0.05f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Blindness },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 160 },
                    { Stat.Attack, 14 },
                    { Stat.Defense, 11 },
                    { Stat.Speed, 7 },
                },
                immunities: new List<Immunities> { Immunities.Physical },
                resistances: new List<Resistances> { Resistances.Holy },
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/WispOfDawn")
            ),
            new Monster(
                monsterName: "Fire Imp",
                description: "A mischievous, flame-spewing creature from the underworld.",
                2,
                damageType: DamageType.Fire,
                0.15f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Burn },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 145 },
                    { Stat.Attack, 12 },
                    { Stat.Defense, 13 },
                    { Stat.Speed, 8 },
                },
                immunities: null,
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FireImp")
            ),
            new Monster(
                monsterName: "Venom Toad",
                description: "A bloated toad exuding potent poison.",
                2,
                damageType: DamageType.Poison,
                0.20f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Poison },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 155 },
                    { Stat.Attack, 16 },
                    { Stat.Defense, 11 },
                    { Stat.Speed, 4 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/VenomToad")
            ),
        };
        #endregion

        #region Tier 3 Monsters
        // Tier 3 Monsters
        public static List<Monster> tierThreeEnemies = new List<Monster>()
        {
            new Monster(
                monsterName: "Storm Harpy",
                description: "A harpy that conjures lightning with every screech.",
                3,
                damageType: DamageType.Lightning,
                0.15f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Paralyze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 170 },
                    { Stat.Attack, 11 },
                    { Stat.Defense, 8 },
                    { Stat.Speed, 10 },
                },
                immunities: new List<Immunities> { Immunities.Lightning },
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Ice },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/StormHarpy")
            ),
            new Monster(
                monsterName: "Bloodfang Mantis",
                description: "A giant mantis with razor-sharp claws that cause bleeding.",
                3,
                damageType: DamageType.Bleed,
                0.25f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Bleed },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 110 },
                    { Stat.Attack, 12 },
                    { Stat.Defense, 6 },
                    { Stat.Speed, 8 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/BloodfangMantis")
            ),
            new Monster(
                monsterName: "Shadow Wraith",
                description: "An ethereal specter that drains life force and curses its victims.",
                3,
                damageType: DamageType.Shadow,
                0.12f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Curse },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 150 },
                    { Stat.Attack, 10 },
                    { Stat.Defense, 7 },
                    { Stat.Speed, 9 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Holy },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/ShadowWraith")
            ),
            new Monster(
                monsterName: "Infernal Drake",
                description: "A young drake wreathed in flames, scorching its foes relentlessly.",
                4,
                damageType: DamageType.Fire,
                0.20f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Burn },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 180 },
                    { Stat.Attack, 15 },
                    { Stat.Defense, 10 },
                    { Stat.Speed, 7 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Ice },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/InfernalDrake")
            ),
            new Monster(
                monsterName: "Ghoul Stalker",
                description: "A relentless undead predator slinking through shadows.",
                4,
                damageType: DamageType.Shadow,
                0.18f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Curse },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 160 },
                    { Stat.Attack, 14 },
                    { Stat.Defense, 9 },
                    { Stat.Speed, 8 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Holy },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/GhoulStalker")
            ),
        };
        #endregion

        #region Tier 4 Monsters
        public static List<Monster> tierFourEnemies = new List<Monster>()
        {
            new Monster(
                monsterName: "Fallen Paladin",
                description: "Once holy, now corrupted by darkness. Wields unholy might.",
                4,
                damageType: DamageType.Shadow,
                0.22f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Confusion },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 200 },
                    { Stat.Attack, 16 },
                    { Stat.Defense, 12 },
                    { Stat.Speed, 6 },
                },
                immunities: new List<Immunities> { Immunities.Shadow },
                resistances: new List<Resistances> { Resistances.Physical },
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FallenPaladin")
            ),
            new Monster(
                monsterName: "Bone Hydra",
                description: "A skeletal hydra with regenerating heads fueled by necrotic energy.",
                4,
                damageType: DamageType.Shadow,
                0.18f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.None },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 220 },
                    { Stat.Attack, 18 },
                    { Stat.Defense, 14 },
                    { Stat.Speed, 5 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Holy },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/BoneHydra")
            ),
            new Monster(
                monsterName: "Arcane Construct",
                description: "A magical golem animated by raw arcane forces.",
                4,
                damageType: DamageType.Arcane,
                0.10f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.None },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 170 },
                    { Stat.Attack, 14 },
                    { Stat.Defense, 10 },
                    { Stat.Speed, 7 },
                    { Stat.MaxMagic, 50 },
                },
                immunities: null,
                resistances: new List<Resistances> { Resistances.Arcane },
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/ArcaneConstruct")
            ),
            new Monster(
                monsterName: "Flame Titan",
                description: "A colossal giant engulfed in flames, incinerating all in its path.",
                5,
                damageType: DamageType.Fire,
                0.20f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Burn },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 250 },
                    { Stat.Attack, 20 },
                    { Stat.Defense, 15 },
                    { Stat.Speed, 6 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Ice, Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FlameTitan")
            ),
            new Monster(
                monsterName: "Ice Golem",
                description: "A towering golem of ice that freezes foes with each chilling touch.",
                5,
                damageType: DamageType.Ice,
                0.25f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Slow },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 240 },
                    { Stat.Attack, 18 },
                    { Stat.Defense, 14 },
                    { Stat.Speed, 4 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Fire },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/IceGolem")
            ),
        };
        #endregion

        #region Tier 5 Monsters
        // Tier 5 Monsters
        public static List<Monster> tierFiveEnemies = new List<Monster>()
        {
            new Monster(
                monsterName: "Angelic Guardian",
                description: "A divine sentinel wielding holy wrath against darkness.",
                5,
                damageType: DamageType.Holy,
                statusInflictionChance: 0.10f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Blindness },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 300 },
                    { Stat.Attack, 20 },
                    { Stat.Defense, 23 },
                    { Stat.Speed, 9 },
                },
                immunities: new List<Immunities> { Immunities.Physical, Immunities.Poison },
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/AngelicGuardian")
            ),
            new Monster(
                monsterName: "Abyssal Lich",
                description: "An ancient sorcerer turned undead, channeling shadow and ice.",
                5,
                damageType: DamageType.Shadow,
                statusInflictionChance: 0.30f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Freeze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 310 },
                    { Stat.Attack, 22 },
                    { Stat.Defense, 18 },
                    { Stat.Speed, 8 },
                },
                immunities: new List<Immunities> { Immunities.Shadow },
                resistances: null,
                weaknesses: new List<Weaknesses> { Weaknesses.Holy },
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/AbyssalLich")
            ),
            new Monster(
                monsterName: "Fire Element",
                description: "A living flame that dances with the wind, scorching all in its path.",
                5,
                damageType: DamageType.Fire,
                statusInflictionChance: 0.22f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Burn },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 290 },
                    { Stat.Attack, 17 },
                    { Stat.Defense, 19 },
                    { Stat.Speed, 7 },
                },
                immunities: null,
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/FireElement")
            ),
            new Monster(
                monsterName: "Thunder Serpent",
                description: "A massive sea serpent roaring with thunder, controlling storms at sea.",
                5,
                damageType: DamageType.Lightning,
                statusInflictionChance: 0.28f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Paralyze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 320 },
                    { Stat.Attack, 19 },
                    { Stat.Defense, 25 },
                    { Stat.Speed, 8 },
                },
                immunities: null,
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/ThunderSerpent")
            ),
        };
        #endregion

        #region Tier 6 Monsters
        // Tier 6 Monsters
        public static List<Monster> tierSixEnemies = new List<Monster>()
        {
            new Monster(
                monsterName: "Electric Orb",
                description: "A hovering orb of pure lightning, zapping foes with each touch.",
                5,
                damageType: DamageType.Lightning,
                statusInflictionChance: 0.05f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Paralyze },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 380 },
                    { Stat.Attack, 16 },
                    { Stat.Defense, 15 },
                    { Stat.Speed, 12 },
                },
                immunities: null,
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/ElectricOrb")
            ),
            new Monster(
                monsterName: "Arcane Shadow Orb",
                description: "A mysterious orb blending arcane and shadow energies to silence foes.",
                5,
                damageType: DamageType.Shadow,
                statusInflictionChance: 0.30f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Silence },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 390 },
                    { Stat.Attack, 26 },
                    { Stat.Defense, 29 },
                    { Stat.Speed, 10 },
                },
                immunities: null,
                resistances: null,
                weaknesses: new List<Weaknesses>(), // no resistances/weaknesses provided
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/ArcaneShadowOrb")
            ),
            new Monster(
                monsterName: "Abyssal Oracle",
                description: "A prophet of the deep void, weaving illusions and curses.",
                5,
                damageType: DamageType.Shadow,
                statusInflictionChance: 0.35f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Curse },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 340 },
                    { Stat.Attack, 28 },
                    { Stat.Defense, 24 },
                    { Stat.Speed, 12 },
                },
                immunities: null,
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/AbyssalOracle")
            ),
            new Monster(
                monsterName: "Aurora Dragon",
                description: "A legendary dragon whose shimmering scales refract arcane light in dazzling displays.",
                5,
                damageType: DamageType.Arcane,
                statusInflictionChance: 0.40f,
                inflictedStatusEffect: new List<StatusEffectType> { StatusEffectType.Silence },
                statOverrides: new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 420 },
                    { Stat.Attack, 27 },
                    { Stat.Defense, 26 },
                    { Stat.Speed, 14 },
                },
                immunities: new List<Immunities> { Immunities.Arcane, Immunities.Holy },
                resistances: null,
                weaknesses: null,
                monsterSprite: Resources.Load<Sprite>("Sprites/Monsters/AuroraDragon")
            ),
        };
    }
}
        #endregion
