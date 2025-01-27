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

    // Multi-stat modification support
    public float modifierAmount;

    // Status Effect
    public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
    public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();

    // One-time effect flag
    public bool isOneTimeEffect;
}
