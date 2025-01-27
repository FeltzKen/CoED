using System.Collections.Generic;
using CoED;

[System.Serializable]
public class EquipmentSuffixData
{
    public string suffixName;
    public string description;
    public int healthBonus;
    public int staminaBonus;
    public int speedBonus;
    public int dexterityBonus;
    public int attackBonus;
    public int defenseBonus;
    public int magicBonus;
    public int intelligenceBonus;
    public int critChanceBonus;

    // Elemental Damage
    public Dictionary<DamageType, int> damageModifiers = new Dictionary<DamageType, int>();
    public bool isOneTimeEffect;
    public List<StatusEffectType> activeStatusEffects = new List<StatusEffectType>();
    public List<ActiveWhileEquipped> equipmentEffects = new List<ActiveWhileEquipped>();
    public List<StatusEffectType> inflictedStatusEffects = new List<StatusEffectType>();
}
