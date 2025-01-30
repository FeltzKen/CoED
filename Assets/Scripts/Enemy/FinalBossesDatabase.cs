using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class FinalBoss
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
        public Sprite bossSprite;
    }

    public static class FinalBossesDatabase
    {
        public static List<FinalBoss> finalBosses = new List<FinalBoss>
        {
            new FinalBoss
            {
                name = "Eternal Dragon of Ember",
                description =
                    "A colossal dragon whose flames never die, scorching the world in perpetual blaze.",
                level = 25,
                maxHP = 1000,
                attack = 70,
                defense = 50,
                speed = 15,
                intelligence = 25,
                damageType = DamageType.Fire,
                statusInflictionChance = 0.40f, // 40% chance
                inflictedStatusEffect = StatusEffectType.Burn,
                bossSprite = Resources.Load<Sprite>(
                    "Sprites/Monsters/FinalBoss/EternalEmberDragon"
                ),
            },
            new FinalBoss
            {
                name = "Abyssal Devourer",
                description =
                    "An ancient horror from the darkest depths, devouring souls to grow stronger.",
                level = 25,
                maxHP = 1200,
                attack = 65,
                defense = 60,
                speed = 12,
                intelligence = 35,
                damageType = DamageType.Shadow,
                statusInflictionChance = 0.45f,
                inflictedStatusEffect = StatusEffectType.Curse,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/AbyssalDevourer"),
            },
            new FinalBoss
            {
                name = "Celestial Overlord",
                description =
                    "A grand being of pure light, commanding holy retribution against all sinners.",
                level = 25,
                maxHP = 1100,
                attack = 68,
                defense = 55,
                speed = 14,
                intelligence = 40,
                damageType = DamageType.Holy,
                statusInflictionChance = 0.35f,
                inflictedStatusEffect = StatusEffectType.Blindness,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/CelestialOverlord"),
            },
            new FinalBoss
            {
                name = "Arcane Void Hydra",
                description =
                    "A multi-headed monstrosity harnessing chaotic arcane powers to rend reality.",
                level = 26,
                maxHP = 1300,
                attack = 75,
                defense = 50,
                speed = 10,
                intelligence = 45,
                damageType = DamageType.Arcane,
                statusInflictionChance = 0.50f,
                inflictedStatusEffect = StatusEffectType.Silence,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/ArcaneVoidHydra"),
            },
            new FinalBoss
            {
                name = "Primordial Titan",
                description =
                    "A towering giant formed from stone and living storms, rumored to be older than time.",
                level = 26,
                maxHP = 1500,
                attack = 80,
                defense = 60,
                speed = 8,
                intelligence = 30,
                damageType = DamageType.Lightning,
                statusInflictionChance = 0.40f,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/PrimordialTitan"),
            },
        };
    }
}
