using System.Collections.Generic;
using CoED;
using UnityEngine;

[System.Serializable]
public class ConsumableSuffixData
{
    public string suffixName;
    public float attackBoost;
    public float defenseBoost;
    public float speedBoost;
    public float healthBoost;
    public float magicBoost;
    public float staminaBoost;
    public float dexterityBoost;
    public float intelligenceBoost;
    public float critChanceBoost;
    public int priceIncrease;
    public int duration;

    public Dictionary<Stat, float> statModifiers = new Dictionary<Stat, float>();
    // Status Effect
    public List<StatusEffectType> addedEffects = new List<StatusEffectType>();
    public List<StatusEffectType> removedEffects = new List<StatusEffectType>();
}
