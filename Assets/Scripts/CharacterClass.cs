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
            { Stat.Shield, 0 },
            { Stat.FireRate, 0 },
            { Stat.CritChance, 0 },
            { Stat.CritDamage, 0 },
            { Stat.ProjectileRange, 0 },
            { Stat.AttackRange, 0 },
            { Stat.Speed, 0 },
            { Stat.ElementalDamage, 0 },
            { Stat.ChanceToInflictStatusEffect, 0 },
            { Stat.StatusEffectDuration, 0 },
            { Stat.PatrolSpeed, 0 },
            { Stat.ChaseSpeed, 0 },
        };
    }

    public static class CharacterClasses
    {
        public static List<CharacterClass> Classes = new List<CharacterClass>()
        {
            new CharacterClass
            {
                ClassName = "Warrior",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Warrior"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 100 },
                    { Stat.MaxHP, 100 },
                    { Stat.Attack, 10 },
                    { Stat.Intelligence, 5 },
                    { Stat.Evasion, 5 },
                    { Stat.Defense, 10 },
                    { Stat.Dexterity, 5 },
                    { Stat.Accuracy, 5 },
                    { Stat.Magic, 5 },
                    { Stat.MaxMagic, 100 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.Shield, 5 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 5 },
                    { Stat.CritDamage, 5 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 5 },
                    { Stat.ElementalDamage, 5 },
                    { Stat.ChanceToInflictStatusEffect, 5 },
                    { Stat.StatusEffectDuration, 5 },
                    { Stat.PatrolSpeed, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
            },
            new CharacterClass
            {
                ClassName = "Mage",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Mage"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 50 },
                    { Stat.MaxHP, 50 },
                    { Stat.Attack, 5 },
                    { Stat.Intelligence, 10 },
                    { Stat.Evasion, 5 },
                    { Stat.Defense, 5 },
                    { Stat.Dexterity, 5 },
                    { Stat.Accuracy, 5 },
                    { Stat.Magic, 10 },
                    { Stat.MaxMagic, 100 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.Shield, 5 },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 5 },
                    { Stat.CritDamage, 5 },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 5 },
                    { Stat.ElementalDamage, 5 },
                    { Stat.ChanceToInflictStatusEffect, 5 },
                    { Stat.StatusEffectDuration, 5 },
                    { Stat.PatrolSpeed, 5 },
                    { Stat.ChaseSpeed, 5 },
                },
            },
            new CharacterClass
            {
                ClassName = "Rogue",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Rogue"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 75 },
                    { Stat.MaxHP, 75 },
                    { Stat.Attack, 7.5f },
                    { Stat.Intelligence, 7.5f },
                    { Stat.Evasion, 10 },
                    { Stat.Defense, 7.5f },
                    { Stat.Dexterity, 10 },
                    { Stat.Accuracy, 7.5f },
                    { Stat.Magic, 7.5f },
                    { Stat.MaxMagic, 100 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.Shield, 7.5f },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 10 },
                    { Stat.CritDamage, 7.5f },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 10 },
                    { Stat.ElementalDamage, 7.5f },
                    { Stat.ChanceToInflictStatusEffect, 7.5f },
                    { Stat.StatusEffectDuration, 7.5f },
                    { Stat.PatrolSpeed, 7.5f },
                    { Stat.ChaseSpeed, 7.5f },
                },
            },
            new CharacterClass
            {
                ClassName = "Cleric",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Cleric"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 75 },
                    { Stat.MaxHP, 75 },
                    { Stat.Attack, 7.5f },
                    { Stat.Intelligence, 7.5f },
                    { Stat.Evasion, 7.5f },
                    { Stat.Defense, 7.5f },
                    { Stat.Dexterity, 7.5f },
                    { Stat.Accuracy, 7.5f },
                    { Stat.Magic, 7.5f },
                    { Stat.MaxMagic, 100 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.Shield, 7.5f },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 7.5f },
                    { Stat.CritDamage, 7.5f },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7.5f },
                    { Stat.ElementalDamage, 7.5f },
                    { Stat.ChanceToInflictStatusEffect, 7.5f },
                    { Stat.StatusEffectDuration, 7.5f },
                    { Stat.PatrolSpeed, 7.5f },
                    { Stat.ChaseSpeed, 7.5f },
                },
            },
            new CharacterClass
            {
                ClassName = "Paladin",
                CharacterSprite = Resources.Load<Sprite>("Sprites/Player/Paladin"),
                BaseStats = new Dictionary<Stat, float>()
                {
                    { Stat.HP, 100 },
                    { Stat.MaxHP, 100 },
                    { Stat.Attack, 7.5f },
                    { Stat.Intelligence, 7.5f },
                    { Stat.Evasion, 7.5f },
                    { Stat.Defense, 7.5f },
                    { Stat.Dexterity, 7.5f },
                    { Stat.Accuracy, 7.5f },
                    { Stat.Magic, 7.5f },
                    { Stat.MaxMagic, 100 },
                    { Stat.Stamina, 100 },
                    { Stat.MaxStamina, 100 },
                    { Stat.Shield, 7.5f },
                    { Stat.FireRate, 1 },
                    { Stat.CritChance, 7.5f },
                    { Stat.CritDamage, 7.5f },
                    { Stat.ProjectileRange, 1 },
                    { Stat.AttackRange, 1 },
                    { Stat.Speed, 7.5f },
                    { Stat.ElementalDamage, 7.5f },
                    { Stat.ChanceToInflictStatusEffect, 7.5f },
                    { Stat.StatusEffectDuration, 7.5f },
                    { Stat.PatrolSpeed, 7.5f },
                    { Stat.ChaseSpeed, 7.5f },
                },
            },
        };
    }
}
