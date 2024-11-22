using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "GameItems/Equipment", order = 1)]
public class Equipment : ScriptableObject
{
    public string equipmentName; // Name of the equipment
    public EquipmentType equipmentType; // Type of equipment (e.g., Weapon, Armor, Accessory)
    public int attackBonus; // Attack bonus provided by the equipment
    public int defenseBonus; // Defense bonus provided by the equipment
    public int durability; // Maximum durability of the equipment
    public Rarity rarity; // Rarity level of the equipment
    public Sprite icon; // Icon for the equipment
    public GameObject modelPrefab; // Optional prefab for 3D representation
        // Properties to indicate enchantment and curse status.
        public bool IsEnchanted { get; set; }
        public bool IsCursed { get; set; }

        private const float enchantmentChance = 0.1f; // 10% chance to be enchanted
        private const float curseChance = 0.1f; // 10% chance to be cursed


    [TextArea]
    public string description; // Description of the equipment
        // Method to initialize the item, setting enchantment or curse status based on chance.
        public void InitializeEquipment()
        {
            IsEnchanted = Random.value < enchantmentChance;
            IsCursed = !IsEnchanted && Random.value < curseChance; // Only assign curse if not enchanted

            Debug.Log($"{equipmentName} initialized: Enchanted = {IsEnchanted}, Cursed = {IsCursed}");
        }

        public void RemoveCurse()
        {
            IsCursed = false;
        }
// Enum for equipment types
public enum EquipmentType
{
    Weapon,
    Armor,
    Accessory
}
    // Defines the possible equipment slots for items.
    public enum EquipmentSlot
    {
        None, // Represents items that are not equippable
        Head,
        Chest,
        Legs,
        Waist,
        Weapon,
        Shield,
        Boots,
    }
// Enum for rarity levels
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
}
