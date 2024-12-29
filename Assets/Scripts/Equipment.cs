using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "GameItems/Equipment", order = 1)]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public EquipmentType equipmentType;
    public int attackBonus;
    public int defenseBonus;
    public int durability;
    public Rarity rarity;
    public Sprite icon;
    public GameObject modelPrefab;
    public bool IsEnchanted { get; set; }
    public bool IsCursed { get; set; }

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
