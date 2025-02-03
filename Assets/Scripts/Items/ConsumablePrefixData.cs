using System.Collections.Generic;
using CoED;
using UnityEngine;

[System.Serializable]
public class ConsumablePrefixData
{
    public string prefixName;
    public string description;
    public float imageSize;
    public bool glow;
    public float duration;
    public float priceMultiplier;

    // Multi-stat modification support
    public Dictionary<Stat, float> statModifiers = new Dictionary<Stat, float>();
    public List<StatusEffectType> removedEffects = new List<StatusEffectType>();
    public List<StatusEffectType> addedEffects = new List<StatusEffectType>();
    public float modifierAmount;

    // One-time effect flag
    public bool isOneTimeEffect;
}
