using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public Object equipmentButtonPrefab;
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
    public GameObject modelPrefab;
    public Rarity rarity;
    public Sprite icon;

    private const float enchantmentChance = 0.1f;
    private const float curseChance = 0.1f;

    [TextArea]
    public string description;

    public void InitializeEquipment()
    {
        IsEnchanted = Random.value < enchantmentChance;
        IsCursed = !IsEnchanted && Random.value < curseChance;
    }

    public void RemoveCurse()
    {
        IsCursed = false;
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
}
