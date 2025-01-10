using CoED;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public string equipmentName => itemName;
    public float attackModifier;
    public float defenseModifier;
    public float healthModifier;
    public float speedModifier;
    public float magicModifier;
    public float staminaModifier;
    public int durability;
    public bool IsEnchanted { get; private set; }
    public bool IsCursed { get; private set; }
    public EquipmentType equipmentType;
    public EquipmentSlot equipmentSlot;
    public Rarity rarity;

    private const float enchantmentChance = 0.1f;
    private const float curseChance = 0.1f;

    [SerializeField]
    private DescriptionField descriptionField = DescriptionField.None; // Default to None

    [SerializeField, TextArea]
    private string description = ""; // Optional custom text

    public void InitializeEquipment()
    {
        IsEnchanted = Random.value < enchantmentChance;
        IsCursed = !IsEnchanted && Random.value < curseChance;
    }

    public void RemoveCurse()
    {
        IsCursed = false;
    }

    public string Description
    {
        get
        {
            float value = GetDescriptionValue();
            string baseDescription = $"{itemName} +{value}";

            // Append the custom description, if it exists
            if (!string.IsNullOrEmpty(description))
            {
                baseDescription += $" {description}";
            }

            return baseDescription;
        }
    }

    private float GetDescriptionValue()
    {
        float value = descriptionField switch
        {
            DescriptionField.AttackBoost => attackModifier,
            DescriptionField.DefenseBoost => defenseModifier,
            DescriptionField.SpeedBoost => speedModifier,
            DescriptionField.HealthBoost => healthModifier,
            DescriptionField.MagicBoost => magicModifier,
            DescriptionField.StaminaBoost => staminaModifier,
            _ => 0, // Handle None or undefined cases
        };
        return value;
    }

    public enum EquipmentType
    {
        Weapon,
        Armor,
        Accessory,
    }

    public enum EquipmentSlot
    {
        None,
        Head,
        Chest,
        Legs,
        Waist,
        Weapon,
        Shield,
        Boots,
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
        None, // To handle cases where no description field is selected
        AttackBoost,
        DefenseBoost,
        SpeedBoost,
        HealthBoost,
        MagicBoost,
        StaminaBoost,
    }
}
