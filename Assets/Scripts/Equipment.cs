using CoED;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public string equipmentName => itemName;

    [Header("Base Modifiers")]
    public float attackModifier;
    public float defenseModifier;
    public float healthModifier;
    public float speedModifier;
    public float magicModifier;
    public float staminaModifier;

    [Header("Equipment Settings")]
    public EquipmentType equipmentType;
    public EquipmentSlot equipmentSlot; // 🔥 Added this for proper slot mapping
    public Rarity rarity;
    public StatusEffect resistance;

    [SerializeField, TextArea]
    private string description = "";

    [SerializeField]
    private DescriptionField descriptionField = DescriptionField.None;

    public string Description
    {
        get
        {
            float value = GetDescriptionValue();
            string baseDescription = $"{itemName} +{value}";
            if (!string.IsNullOrEmpty(description))
                baseDescription += $" {description}";
            return baseDescription;
        }
    }

    private float GetDescriptionValue()
    {
        return descriptionField switch
        {
            DescriptionField.AttackBoost => attackModifier,
            DescriptionField.DefenseBoost => defenseModifier,
            DescriptionField.SpeedBoost => speedModifier,
            DescriptionField.HealthBoost => healthModifier,
            DescriptionField.MagicBoost => magicModifier,
            DescriptionField.StaminaBoost => staminaModifier,
            _ => 0,
        };
    }

    public enum EquipmentType
    {
        Weapon,
        Armor,
        Accessory,
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
    }

    public enum DescriptionField
    {
        None,
        AttackBoost,
        DefenseBoost,
        SpeedBoost,
        HealthBoost,
        MagicBoost,
        StaminaBoost,
    }

    public enum EquipmentSlot
    {
        Head,
        Chest,
        Legs,
        Waist,
        Weapon,
        Shield,
        Boots,
    }
}
