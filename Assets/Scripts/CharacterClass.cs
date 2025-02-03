using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class CharacterClass
    {
        public string ClassName;
        public Sprite CharacterSprite;
        public Dictionary<Stat, float> BaseStats = new Dictionary<Stat, float>()
        {
            { Stat.HP, 0 },
            { Stat.MaxHP, 0 },
            { Stat.Attack, 0 },
            { Stat.Intelligence, 0 },
            { Stat.Evasion, 0 },
            { Stat.Defense, 0 },
            { Stat.Dexterity, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Magic, 0 },
            { Stat.MaxMagic, 0 },
            { Stat.Stamina, 0 },
            { Stat.MaxStamina, 0 },
            { Stat.FireRate, 0 },
            { Stat.CritChance, 0 },
            { Stat.CritDamage, 0 },
            { Stat.ProjectileRange, 0 },
            { Stat.AttackRange, 0 },
            { Stat.Speed, 0 },
            { Stat.ChanceToInflictStatusEffect, 0 },
            { Stat.StatusEffectDuration, 0 },
        };
    }

    public static class CharacterClasses
    {
        public static List<CharacterClass> Classes = new List<CharacterClass>()
        {
            // 1. Warrior – A robust front-line fighter with high HP and defense.
            new CharacterClass
            {
                ClassName = "Warrior",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Warrior"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 120 },
                    { Stat.MaxHP, 120 },
                    { Stat.Attack, 12 },
                    { Stat.Intelligence, 4 },
                    { Stat.Evasion, 5 },
                    { Stat.Defense, 12 },
                    { Stat.Dexterity, 6 },
                    { Stat.Accuracy, 6 },
                    { Stat.Magic, 3 },
                    { Stat.MaxMagic, 50 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.05f },
                    { Stat.CritDamage, 5 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 5 },
                    { Stat.ChanceToInflictStatusEffect, 0.05f },
                    { Stat.StatusEffectDuration, 15 },
                },
            },
            // 2. Mage – A glass cannon wielding powerful elemental magic.
            new CharacterClass
            {
                ClassName = "Mage",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Mage"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 50 },
                    { Stat.MaxHP, 50 },
                    { Stat.Attack, 4 },
                    { Stat.Intelligence, 14 },
                    { Stat.Evasion, 6 },
                    { Stat.Defense, 4 },
                    { Stat.Dexterity, 6 },
                    { Stat.Accuracy, 7 },
                    { Stat.Magic, 14 },
                    { Stat.MaxMagic, 150 },
                    { Stat.Stamina, 80 },
                    { Stat.MaxStamina, 80 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.07f },
                    { Stat.CritDamage, 6 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 6 },
                    { Stat.ChanceToInflictStatusEffect, 0.07f },
                    { Stat.StatusEffectDuration, 20 },
                },
            },
            // 3. Rogue – A nimble assassin with high evasion and critical potential.
            new CharacterClass
            {
                ClassName = "Rogue",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Rogue"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 80 },
                    { Stat.MaxHP, 80 },
                    { Stat.Attack, 10 },
                    { Stat.Intelligence, 8 },
                    { Stat.Evasion, 14 },
                    { Stat.Defense, 6 },
                    { Stat.Dexterity, 12 },
                    { Stat.Accuracy, 10 },
                    { Stat.Magic, 6 },
                    { Stat.MaxMagic, 80 },
                    { Stat.Stamina, 90 },
                    { Stat.MaxStamina, 90 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.15f },
                    { Stat.CritDamage, 8 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 12 },
                    { Stat.ChanceToInflictStatusEffect, 0.08f },
                    { Stat.StatusEffectDuration, 20 },
                },
            },
            // 4. Cleric – A divine healer and protector who specializes in support magic.
            new CharacterClass
            {
                ClassName = "Cleric",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Cleric"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 90 },
                    { Stat.MaxHP, 90 },
                    { Stat.Attack, 7 },
                    { Stat.Intelligence, 10 },
                    { Stat.Evasion, 6 },
                    { Stat.Defense, 8 },
                    { Stat.Dexterity, 7 },
                    { Stat.Accuracy, 8 },
                    { Stat.Magic, 12 },
                    { Stat.MaxMagic, 120 },
                    { Stat.Stamina, 90 },
                    { Stat.MaxStamina, 90 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.05f },
                    { Stat.CritDamage, 5 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7 },
                    { Stat.ChanceToInflictStatusEffect, 0.06f },
                    { Stat.StatusEffectDuration, 25 },
                },
            },
            // 5. Paladin – A holy warrior blending melee prowess with divine magic.
            new CharacterClass
            {
                ClassName = "Paladin",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Paladin"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 110 },
                    { Stat.MaxHP, 110 },
                    { Stat.Attack, 9 },
                    { Stat.Intelligence, 9 },
                    { Stat.Evasion, 7 },
                    { Stat.Defense, 11 },
                    { Stat.Dexterity, 8 },
                    { Stat.Accuracy, 8 },
                    { Stat.Magic, 8 },
                    { Stat.MaxMagic, 100 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.06f },
                    { Stat.CritDamage, 6 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7 },
                    { Stat.ChanceToInflictStatusEffect, 0.06f },
                    { Stat.StatusEffectDuration, 30 },
                },
            },
            // 6. Ranger – A master of ranged combat with high dexterity and accuracy.
            new CharacterClass
            {
                ClassName = "Ranger",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Ranger"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 85 },
                    { Stat.MaxHP, 85 },
                    { Stat.Attack, 9 },
                    { Stat.Intelligence, 8 },
                    { Stat.Evasion, 12 },
                    { Stat.Defense, 7 },
                    { Stat.Dexterity, 14 },
                    { Stat.Accuracy, 12 },
                    { Stat.Magic, 7 },
                    { Stat.MaxMagic, 90 },
                    { Stat.Stamina, 95 },
                    { Stat.MaxStamina, 95 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.1f },
                    { Stat.CritDamage, 8 },
                    { Stat.ProjectileRange, 2 },
                    { Stat.AttackRange, 2 },
                    { Stat.Speed, 10 },
                    { Stat.ChanceToInflictStatusEffect, 0.07f },
                    { Stat.StatusEffectDuration, 25 },
                },
            },
            // 7. Bard – A versatile support character who uses music and lore to inspire and hinder.
            new CharacterClass
            {
                ClassName = "Bard",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Bard"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 80 },
                    { Stat.MaxHP, 80 },
                    { Stat.Attack, 7 },
                    { Stat.Intelligence, 10 },
                    { Stat.Evasion, 10 },
                    { Stat.Defense, 6 },
                    { Stat.Dexterity, 10 },
                    { Stat.Accuracy, 10 },
                    { Stat.Magic, 10 },
                    { Stat.MaxMagic, 110 },
                    { Stat.Stamina, 90 },
                    { Stat.MaxStamina, 90 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.08f },
                    { Stat.CritDamage, 7 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 9 },
                    { Stat.ChanceToInflictStatusEffect, 0.08f },
                    { Stat.StatusEffectDuration, 25 },
                },
            },
            // 8. Druid – A nature-bound caster who uses elemental and healing magic.
            new CharacterClass
            {
                ClassName = "Druid",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Druid"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 80 },
                    { Stat.MaxHP, 80 },
                    { Stat.Attack, 8 },
                    { Stat.Intelligence, 12 },
                    { Stat.Evasion, 8 },
                    { Stat.Defense, 7 },
                    { Stat.Dexterity, 9 },
                    { Stat.Accuracy, 9 },
                    { Stat.Magic, 14 },
                    { Stat.MaxMagic, 130 },
                    { Stat.Stamina, 85 },
                    { Stat.MaxStamina, 85 },
                    { Stat.Shield, 5 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.07f },
                    { Stat.CritDamage, 7 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 8 },
                    { Stat.ChanceToInflictStatusEffect, 0.18f },
                    { Stat.StatusEffectDuration, 25 },
                },
            },
            // 9. Monk – A martial artist who combines physical prowess with inner energy.
            new CharacterClass
            {
                ClassName = "Monk",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Monk"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 85 },
                    { Stat.MaxHP, 85 },
                    { Stat.Attack, 9 },
                    { Stat.Intelligence, 8 },
                    { Stat.Evasion, 13 },
                    { Stat.Defense, 7 },
                    { Stat.Dexterity, 11 },
                    { Stat.Accuracy, 9 },
                    { Stat.Magic, 6 },
                    { Stat.MaxMagic, 80 },
                    { Stat.Stamina, 110 },
                    { Stat.MaxStamina, 110 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.1f },
                    { Stat.CritDamage, 8 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 14 },
                    { Stat.ChanceToInflictStatusEffect, 0.27f },
                    { Stat.StatusEffectDuration, 30 },
                },
            },
            // 10. Necromancer – A dark caster who manipulates life and death.
            new CharacterClass
            {
                ClassName = "Necromancer",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Necromancer"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 60 },
                    { Stat.MaxHP, 60 },
                    { Stat.Attack, 6 },
                    { Stat.Intelligence, 14 },
                    { Stat.Evasion, 7 },
                    { Stat.Defense, 5 },
                    { Stat.Dexterity, 7 },
                    { Stat.Accuracy, 8 },
                    { Stat.Magic, 16 },
                    { Stat.MaxMagic, 140 },
                    { Stat.Stamina, 70 },
                    { Stat.MaxStamina, 70 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.06f },
                    { Stat.CritDamage, 6 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7 },
                    { Stat.ChanceToInflictStatusEffect, 0.2f },
                    { Stat.StatusEffectDuration, 30 },
                },
            },
            // 11. Sorcerer – An innate magic user with raw, unpredictable power.
            new CharacterClass
            {
                ClassName = "Sorcerer",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Sorcerer"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 55 },
                    { Stat.MaxHP, 55 },
                    { Stat.Attack, 5 },
                    { Stat.Intelligence, 16 },
                    { Stat.Evasion, 7 },
                    { Stat.Defense, 4 },
                    { Stat.Dexterity, 7 },
                    { Stat.Accuracy, 9 },
                    { Stat.Magic, 18 },
                    { Stat.MaxMagic, 150 },
                    { Stat.Stamina, 70 },
                    { Stat.MaxStamina, 70 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.08f },
                    { Stat.CritDamage, 7 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7 },
                    { Stat.ChanceToInflictStatusEffect, 0.18f },
                    { Stat.StatusEffectDuration, 25 },
                },
            },
            // 12. Warlock – A caster empowered by dark pacts and eldritch forces.
            new CharacterClass
            {
                ClassName = "Warlock",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Warlock"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 60 },
                    { Stat.MaxHP, 60 },
                    { Stat.Attack, 6 },
                    { Stat.Intelligence, 15 },
                    { Stat.Evasion, 7 },
                    { Stat.Defense, 5 },
                    { Stat.Dexterity, 7 },
                    { Stat.Accuracy, 8 },
                    { Stat.Magic, 17 },
                    { Stat.MaxMagic, 145 },
                    { Stat.Stamina, 70 },
                    { Stat.MaxStamina, 70 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.07f },
                    { Stat.CritDamage, 7 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7 },
                    { Stat.ChanceToInflictStatusEffect, 0.23f },
                    { Stat.StatusEffectDuration, 25 },
                },
            },
            // 13. Barbarian – A furious, primal combatant with extreme physical power.
            new CharacterClass
            {
                ClassName = "Barbarian",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Barbarian"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 130 },
                    { Stat.MaxHP, 130 },
                    { Stat.Attack, 14 },
                    { Stat.Intelligence, 4 },
                    { Stat.Evasion, 6 },
                    { Stat.Defense, 13 },
                    { Stat.Dexterity, 8 },
                    { Stat.Accuracy, 7 },
                    { Stat.Magic, 3 },
                    { Stat.MaxMagic, 40 },
                    { Stat.Stamina, 120 },
                    { Stat.MaxStamina, 120 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 0.06f },
                    { Stat.CritDamage, 7 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 6 },
                    { Stat.ChanceToInflictStatusEffect, 0.04f },
                    { Stat.StatusEffectDuration, 15 },
                },
            },
        };
    }
}
