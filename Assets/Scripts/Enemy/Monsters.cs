using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class Monster
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
        public StatusEffectType inflictedStatusEffect = StatusEffectType.None;
        public Resistances resistance = Resistances.None;
        public Weaknesses weakness = Weaknesses.None;
        public Immunities immunity = Immunities.None;
        public Sprite monsterSprite;
    }

    public static class MonstersDatabase
    {
        public static List<Monster> monsters = new List<Monster>()
        {
            new Monster
            {
                name = "Feral Slime",
                description = "A small, squishy blob from the forest. Mildly poisonous if touched.",
                damageType = DamageType.Poison,
                statusInflictionChance = 0.10f,
                inflictedStatusEffect = StatusEffectType.Poison,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FeralSlime"),
            },
            new Monster
            {
                name = "Beetle",
                description = "A beetle with crackling antennas that unleash tiny jolts.",

                damageType = DamageType.Physical,
                statusInflictionChance = 0.05f,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/Beetle"),
                resistance = Resistances.Lightning,
            },
            new Monster
            {
                name = "Rat",
                description = "A rodent that dwells in volcanic ash, mildly singes attackers.",

                damageType = DamageType.Physical,
                statusInflictionChance = 0.08f,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/Rat"),
                resistance = Resistances.Shadow,
            },
            new Monster
            {
                name = "Frost Pup",
                description = "A baby wolf with icy breath that slows its prey.",

                damageType = DamageType.Ice,
                statusInflictionChance = 0.12f,
                inflictedStatusEffect = StatusEffectType.Freeze,
                immunity = Immunities.Ice,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FrostPup"),
            },
            new Monster
            {
                name = "Stone Crawler",
                description =
                    "A lumbering bug with a rock-like shell, best avoided in tight spaces.",

                damageType = DamageType.Physical,
                statusInflictionChance = 0f, // no effect
                resistance = Resistances.Physical,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/StoneCrawler"),
            },
            new Monster
            {
                name = "Wisp of Dawn",
                description =
                    "A flickering spirit of holy energy, banishing curses with each touch.",

                damageType = DamageType.Holy,
                statusInflictionChance = 0.05f,
                inflictedStatusEffect = StatusEffectType.Blindness, // example
                immunity = Immunities.Physical,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/WispOfDawn"),
            },
            new Monster
            {
                name = "Fire Imp",
                description = "A mischievous, flame-spewing creature from the underworld.",

                damageType = DamageType.Fire,
                statusInflictionChance = 0.15f,
                inflictedStatusEffect = StatusEffectType.Burn,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FireImp"),
            },
            new Monster
            {
                name = "Venom Toad",
                description = "A bloated toad from the swamp, exuding a potent poison.",

                damageType = DamageType.Poison,
                statusInflictionChance = 0.20f,
                inflictedStatusEffect = StatusEffectType.Poison,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/VenomToad"),
            },
            new Monster
            {
                name = "Lightbringer Golem",
                description = "A sentient statue infused with holy light, smiting the wicked.",

                damageType = DamageType.Holy,
                statusInflictionChance = 0.06f,
                inflictedStatusEffect = StatusEffectType.Blindness,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/LightbringerGolem"),
            },
            new Monster
            {
                name = "Frost Creature",
                description = "A creature imbued with icy powers. Leaves foes shivering.",

                damageType = DamageType.Ice,
                statusInflictionChance = 0.10f,
                inflictedStatusEffect = StatusEffectType.Freeze,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FrostCreature"),
            },
            new Monster
            {
                name = "Storm Harpy",
                description = "A harpy that commands lightning storms with shrill screeches.",

                damageType = DamageType.Lightning,
                statusInflictionChance = 0.15f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/StormHarpy"),
            },
            new Monster
            {
                name = "Bloodfang Mantis",
                description = "A giant mantis with razor-sharp claws that cause bleeding.",

                damageType = DamageType.Bleed,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Bleed,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/BloodfangMantis"),
            },
            new Monster
            {
                name = "Shadow Wraith",
                description = "An ethereal spirit of pure darkness, draining life force.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.12f,
                inflictedStatusEffect = StatusEffectType.Curse,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/ShadowWraith"),
            },
            new Monster
            {
                name = "Infernal Drake",
                description = "A young drake crackling with embers, scorching foes.",

                damageType = DamageType.Fire,
                statusInflictionChance = 0.20f,
                inflictedStatusEffect = StatusEffectType.Burn,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/InfernalDrake"),
            },
            new Monster
            {
                name = "Ghoul Stalker",
                description = "A cunning undead predator that slinks through shadows.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.18f,
                inflictedStatusEffect = StatusEffectType.Curse,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/GhoulStalker"),
            },
            new Monster
            {
                name = "Fallen Paladin",
                description = "Once holy, now corrupted by darkness. Wields unholy might.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.22f,
                inflictedStatusEffect = StatusEffectType.Shadow,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FallenPaladin"),
            },
            new Monster
            {
                name = "Venomous Blob",
                description = "A hulking mass of diseased flesh, attacking with toxic sludge.",

                damageType = DamageType.Poison,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Poison,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/VenomousBlob"),
            },
            new Monster
            {
                name = "Holy Guardian",
                description = "A valiant sentinel bathed in holy light to smite the unworthy.",

                damageType = DamageType.Holy,
                statusInflictionChance = 0.08f,
                inflictedStatusEffect = StatusEffectType.Blindness,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/HolyGuardian"),
            },
            new Monster
            {
                name = "Bone Hydra",
                description = "A skeletal hydra that regenerates heads with necrotic magic.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.18f,
                inflictedStatusEffect = StatusEffectType.Curse,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/BoneHydra"),
            },
            new Monster
            {
                name = "Arcane Construct",
                description = "A mystical golem woven from pure arcane energies.",

                damageType = DamageType.Arcane,
                statusInflictionChance = 0.10f,
                inflictedStatusEffect = StatusEffectType.Silence,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/ArcaneConstruct"),
            },
            new Monster
            {
                name = "Flame Titan",
                description = "A massive giant wreathed in flames, turning the battlefield to ash.",

                damageType = DamageType.Fire,
                statusInflictionChance = 0.20f,
                inflictedStatusEffect = StatusEffectType.Burn,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FlameTitan"),
            },
            new Monster
            {
                name = "Ice Golem",
                description = "A towering golem of ice that freezes foes with each chilling touch.",

                damageType = DamageType.Ice,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Slow,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/IceGolem"),
            },
            new Monster
            {
                name = "Angelic Guardian",
                description = "A higher angelic being offering no mercy to darkness.",

                damageType = DamageType.Holy,
                statusInflictionChance = 0.10f,
                inflictedStatusEffect = StatusEffectType.Blindness,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/AngelicGuardian"),
            },
            new Monster
            {
                name = "Abyssal Lich",
                description = "An ancient sorcerer from the depths, harnessing shadow and ice.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.30f,
                inflictedStatusEffect = StatusEffectType.Freeze,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/AbyssalLich"),
            },
            new Monster
            {
                name = "Fire Element",
                description =
                    "A living flame that dances with the wind, scorching all in its path.",

                damageType = DamageType.Fire,
                statusInflictionChance = 0.22f,
                inflictedStatusEffect = StatusEffectType.Burn,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/FireElement"),
            },
            new Monster
            {
                name = "Eclipse Reaver",
                description =
                    "A demon that thrives in total darkness, strikes with silent ferocity.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Curse,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/EclipseReaver"),
            },
            new Monster
            {
                name = "Ice Demon",
                description = "A demon of ice that freezes the air around it, slowing all nearby.",

                damageType = DamageType.Ice,
                statusInflictionChance = 0.28f,
                inflictedStatusEffect = StatusEffectType.Slow,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/IceDemon"),
            },
            new Monster
            {
                name = "Electric Element",
                description = "A living storm that crackles with lightning, paralyzing foes.",

                damageType = DamageType.Lightning,
                statusInflictionChance = 0.25f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/ElectricElement"),
            },
            new Monster
            {
                name = "Venomous Drake",
                description = "A dragon that spews toxic fumes, poisoning all in its path.",

                damageType = DamageType.Poison,
                statusInflictionChance = 0.3f,
                inflictedStatusEffect = StatusEffectType.Poison,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/VenomousDrake"),
            },
            new Monster
            {
                name = "Phantom Knight",
                description = "A spectral knight that melds swordsmanship with shadow magic.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.28f,
                inflictedStatusEffect = StatusEffectType.Curse,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/PhantomKnight"),
            },
            new Monster
            {
                name = "Molten Dragon",
                description = "A fearsome dragon fused with molten cores, incinerating foes.",

                damageType = DamageType.Fire,
                statusInflictionChance = 0.33f,
                inflictedStatusEffect = StatusEffectType.Burn,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/MoltenDragon"),
            },
            new Monster
            {
                name = "Thunder Serpent",
                description =
                    "A massive sea serpent that roars with thunder, controlling storms at sea.",

                damageType = DamageType.Lightning,
                statusInflictionChance = 0.28f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/ThunderSerpent"),
            },
            new Monster
            {
                name = "Electric Orb",
                description = "A floating orb of pure lightning, zapping foes with each touch.",

                damageType = DamageType.Lightning,
                statusInflictionChance = 0.05f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/ElectricOrb"),
            },
            new Monster
            {
                name = " Arcane Shadow Orb",
                description = "A floating orb of pure shadow, draining life force with each touch.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.3f,
                inflictedStatusEffect = StatusEffectType.Silence,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/ArcaneShadowOrb"),
            },
            new Monster
            {
                name = "Abyssal Oracle",
                description = "A prophet of the deep void, weaving illusions and curses.",

                damageType = DamageType.Shadow,
                statusInflictionChance = 0.35f,
                inflictedStatusEffect = StatusEffectType.Curse,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/AbyssalOracle"),
            },
            new Monster
            {
                name = "Aurora Dragon",
                description =
                    "A legendary dragon whose scales refract arcane light in dazzling displays.",

                damageType = DamageType.Arcane,
                statusInflictionChance = 0.4f,
                inflictedStatusEffect = StatusEffectType.Silence,
                monsterSprite = Resources.Load<Sprite>("Sprites/Monsters/AuroraDragon"),
            },
        };
    }
}
