using UnityEngine;

namespace YourGameNamespace
{
    // Represents an item that can be held in the player's inventory.
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        public GameObject itemPrefab; // prefab assigned in the Inspector

        [Header("Item Attributes")]
        [SerializeField]
        private string itemName;

        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private EquipmentSlot equipmentSlot = EquipmentSlot.None;

        [SerializeField]
        private int attackBoost = 0;

        [SerializeField]
        private int defenseBoost = 0;

        [SerializeField]
        private int healthBoost = 0;

        // Properties to indicate enchantment and curse status.
        public bool IsEnchanted { get; set; }
        public bool IsCursed { get; set; }

        private const float enchantmentChance = 0.1f; // 10% chance to be enchanted
        private const float curseChance = 0.1f; // 10% chance to be cursed

        // Gets the name of the item.
        public string ItemName => itemName;

        // Gets the icon sprite of the item.
        public Sprite Icon => icon;

        // Gets the equipment slot assigned to the item.
        public EquipmentSlot EquipmentSlot => equipmentSlot;

        // Gets the attack boost provided by the item.
        public int AttackBoost => attackBoost;

        // Gets the defense boost provided by the item.
        public int DefenseBoost => defenseBoost;

        // Gets the health boost provided by the item.
        public int HealthBoost => healthBoost;

        // Method to initialize the item, setting enchantment or curse status based on chance.
        public void InitializeItem()
        {
            IsEnchanted = Random.value < enchantmentChance;
            IsCursed = !IsEnchanted && Random.value < curseChance; // Only assign curse if not enchanted

            Debug.Log($"{itemName} initialized: Enchanted = {IsEnchanted}, Cursed = {IsCursed}");
        }

        public void RemoveCurse()
        {
            IsCursed = false;
        }
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
}
