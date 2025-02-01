using System.Collections.Generic;
using CoED;
using UnityEngine;

[System.Serializable]
public class EquipmentPrefixData
{
    public string prefixName;
    public string description;

    // Multi-stat modification support
    public Dictionary<Stat, float> statModifiers = new Dictionary<Stat, float>()
    {
        { Stat.Attack, 0 },
        { Stat.Defense, 0 },
        { Stat.MaxMagic, 0 },
        { Stat.MaxHP, 0 },
        { Stat.MaxStamina, 0 },
        { Stat.Intelligence, 0 },
        { Stat.Dexterity, 0 },
        { Stat.Speed, 0 },
        { Stat.CritChance, 0 },
        { Stat.CritDamage, 0 },
        { Stat.ElementalDamage, 0 },
        { Stat.ChanceToInflictStatusEffect, 0 },
        { Stat.StatusEffectDuration, 0 },
        { Stat.FireRate, 0 },
        { Stat.Shield, 0 },
        { Stat.Accuracy, 0 },
        { Stat.ProjectileRange, 0 },
        { Stat.AttackRange, 0 },
        { Stat.Evasion, 0 },
    };

    // Elemental Damage
    public Dictionary<DamageType, float> damageModifiers = new Dictionary<DamageType, float>();

    // Status Effect
    public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
    public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();

    // One-time effect flag
    public bool isOneTimeEffect;
}
