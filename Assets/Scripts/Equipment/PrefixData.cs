using System.Collections.Generic;
using CoED;
using UnityEngine;

[System.Serializable]
public class EquipmentPrefixData
{
    public string prefixName;
    public string description;

    // Multi-stat modification support
    public int attackModifier;
    public int defenseModifier;
    public int magicModifier;
    public int healthModifier;
    public int staminaModifier;
    public int intelligenceModifier;
    public int dexterityModifier;
    public int speedModifier;
    public int critChanceModifier;

    // Elemental Damage
    public Dictionary<DamageType, int> damageModifiers = new Dictionary<DamageType, int>();

    // Status Effect
    public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
    public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();

    // One-time effect flag
    public bool isOneTimeEffect;
}
