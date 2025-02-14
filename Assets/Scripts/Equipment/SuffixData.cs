using System.Collections.Generic;
using CoED;

[System.Serializable]
public class EquipmentSuffixData
{
    public string suffixName;
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
        { Stat.ChanceToInflict, 0 },
        { Stat.StatusEffectDuration, 0 },
        { Stat.FireRate, 0 },
        { Stat.Shield, 0 },
        { Stat.ProjectileRange, 0 },
        { Stat.AttackRange, 0 },
    };

    // Elemental Damage
    public Dictionary<DamageType, float> damageModifiers = new Dictionary<DamageType, float>();
    public bool isOneTimeEffect;
    public List<StatusEffectType> equipmentEffects = new List<StatusEffectType>();
    public List<StatusEffectType> inflictedEffectsFromEquipment = new List<StatusEffectType>();
}
