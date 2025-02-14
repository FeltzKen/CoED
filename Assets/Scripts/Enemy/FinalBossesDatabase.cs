using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class FinalBoss
    {
        public string name;
        public string description;
        public int level;
        public Dictionary<Stat, float> finalBossStats = new Dictionary<Stat, float>()
        {
            { Stat.MaxHP, 0 },
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 },
            { Stat.Intelligence, 0 },
            { Stat.ChanceToInflict, 0 },
            { Stat.StatusEffectDuration, 0 },
            { Stat.PatrolSpeed, 0 },
            { Stat.CritChance, 0 },
            { Stat.CritDamage, 0 },
            { Stat.FireRate, 0 },
            { Stat.Shield, 0 },
            { Stat.Dexterity, 0 },
            { Stat.ProjectileRange, 0 },
            { Stat.AttackRange, 0 },
            { Stat.ChaseSpeed, 0 },
        };
        public DamageType damageType;
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
                finalBossStats = new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 1000 },
                    { Stat.Attack, 70 },
                    { Stat.Defense, 50 },
                    { Stat.Speed, 15 },
                    { Stat.Intelligence, 20 },
                    { Stat.ChanceToInflict, 0.40f },
                    { Stat.StatusEffectDuration, 60 },
                    { Stat.PatrolSpeed, 6 },
                    { Stat.CritChance, 0 },
                    { Stat.CritDamage, .40f },
                    { Stat.FireRate, 6 },
                    { Stat.Shield, 0 },
                    { Stat.Dexterity, 20 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 10 },
                },
                damageType = DamageType.Fire,
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
                finalBossStats = new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 1200 },
                    { Stat.Attack, 65 },
                    { Stat.Defense, 60 },
                    { Stat.Speed, 12 },
                    { Stat.Intelligence, 35 },
                    { Stat.ChanceToInflict, 0.45f },
                    { Stat.StatusEffectDuration, 60 },
                    { Stat.PatrolSpeed, 6 },
                    { Stat.CritChance, 0 },
                    { Stat.CritDamage, .40f },
                    { Stat.FireRate, 6 },
                    { Stat.Shield, 0 },
                    { Stat.Dexterity, 20 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 10 },
                },

                damageType = DamageType.Shadow,
                inflictedStatusEffect = StatusEffectType.Curse,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/AbyssalDevourer"),
            },
            new FinalBoss
            {
                name = "Celestial Overlord",
                description =
                    "A grand being of pure light, commanding holy retribution against all sinners.",
                level = 25,
                finalBossStats = new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 1100 },
                    { Stat.Attack, 68 },
                    { Stat.Defense, 55 },
                    { Stat.Speed, 14 },
                    { Stat.Intelligence, 40 },
                    { Stat.ChanceToInflict, 0.35f },
                    { Stat.StatusEffectDuration, 60 },
                    { Stat.PatrolSpeed, 6 },
                    { Stat.CritChance, 0 },
                    { Stat.CritDamage, .40f },
                    { Stat.FireRate, 6 },
                    { Stat.Shield, 0 },
                    { Stat.Dexterity, 20 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 10 },
                },

                damageType = DamageType.Holy,
                inflictedStatusEffect = StatusEffectType.Blindness,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/CelestialOverlord"),
            },
            new FinalBoss
            {
                name = "Arcane Void Hydra",
                description =
                    "A multi-headed monstrosity harnessing chaotic arcane powers to rend reality.",
                level = 26,
                finalBossStats = new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 1300 },
                    { Stat.Attack, 75 },
                    { Stat.Defense, 50 },
                    { Stat.Speed, 10 },
                    { Stat.Intelligence, 45 },
                    { Stat.ChanceToInflict, 0.50f },
                    { Stat.StatusEffectDuration, 60 },
                    { Stat.PatrolSpeed, 6 },
                    { Stat.CritChance, 0 },
                    { Stat.CritDamage, .40f },
                    { Stat.FireRate, 6 },
                    { Stat.Shield, 0 },
                    { Stat.Dexterity, 20 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 10 },
                },
                damageType = DamageType.Arcane,
                inflictedStatusEffect = StatusEffectType.Silence,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/ArcaneVoidHydra"),
            },
            new FinalBoss
            {
                name = "Primordial Titan",
                description =
                    "A towering giant formed from stone and living storms, rumored to be older than time.",
                level = 26,
                finalBossStats = new Dictionary<Stat, float>
                {
                    { Stat.MaxHP, 1500 },
                    { Stat.Attack, 80 },
                    { Stat.Defense, 60 },
                    { Stat.Speed, 8 },
                    { Stat.Intelligence, 30 },
                    { Stat.ChanceToInflict, 0.40f },
                    { Stat.StatusEffectDuration, 60 },
                    { Stat.PatrolSpeed, 6 },
                    { Stat.CritChance, 0 },
                    { Stat.CritDamage, .40f },
                    { Stat.FireRate, 6 },
                    { Stat.Shield, 0 },
                    { Stat.Dexterity, 20 },
                    { Stat.ProjectileRange, 10 },
                    { Stat.AttackRange, 5 },
                    { Stat.ChaseSpeed, 10 },
                },

                damageType = DamageType.Lightning,
                inflictedStatusEffect = StatusEffectType.Paralyze,
                bossSprite = Resources.Load<Sprite>("Sprites/Monsters/FinalBoss/PrimordialTitan"),
            },
        };
    }
}
