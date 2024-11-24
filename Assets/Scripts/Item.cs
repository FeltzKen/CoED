using UnityEngine;
using CoED;
namespace CoED
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
        private int attackBoost = 0;

        [SerializeField]
        private int defenseBoost = 0;
        [SerializeField]
        private int speedBoost = 0;

        [SerializeField]
        private int healthBoost = 0;

        // Gets the name of the item.
        public string ItemName => itemName;

        // Gets the icon sprite of the item.
        public Sprite Icon => icon;

        // Gets the attack boost provided by the item.
        public int AttackBoost => attackBoost;

        // Gets the defense boost provided by the item.
        public int DefenseBoost => defenseBoost;

        // Gets the health boost provided by the item.
        public int HealthBoost => healthBoost;

        // Method to initialize the item, setting enchantment or curse status based on chance.
        public void InitializeItem()
        {
            //IsEnchanted = Random.value < enchantmentChance;
            //IsCursed = !IsEnchanted && Random.value < curseChance; // Only assign curse if not enchanted

           // Debug.Log($"{itemName} initialized: Enchanted = {IsEnchanted}, Cursed = {IsCursed}");
        }

    }


}
