using System.Collections.Generic;
using UnityEngine;
using CoED;

namespace CoED
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        [Header("Inventory Settings")]
        [SerializeField]
        private int maxInventorySlots = 20;

        private List<Item> items = new List<Item>();

        public delegate void InventoryChangedHandler();
        public event InventoryChangedHandler OnInventoryChanged;

        private void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning(
                    "Inventory: Duplicate instance detected. Destroying this instance."
                );
                Destroy(gameObject); // Destroy duplicate instance
            }
        }

        // Adds an item to the inventory. Returns true if successful.
        public bool AddItem(Item item)
        {
            if (items.Count >= maxInventorySlots)
            {
                Debug.Log("Inventory: Inventory is full, cannot add item.");
                return false;
            }

            items.Add(item);
            Debug.Log($"Inventory: Added {item.ItemName}.");
            OnInventoryChanged?.Invoke(); // Notify subscribers that inventory has changed
            return true;
        }

        public void AddQuestItem(Item reward)
        {
            // Add special item from quest.
        }

        // Removes an item from the inventory. Returns true if successful.
        public bool RemoveItem(Item item)
        {
            if (items.Remove(item))
            {
                Debug.Log($"Inventory: Removed {item.ItemName}.");
                OnInventoryChanged?.Invoke(); // Notify subscribers that inventory has changed
                return true;
            }

            Debug.LogWarning("Inventory: Tried to remove an item that wasn't in inventory.");
            return false;
        }

        // Checks if the inventory contains a specific item.
        public bool HasItem(Item item)
        {
            return items.Contains(item);
        }

        // Returns a copy of the list of items in the inventory.
        public List<Item> GetAllItems()
        {
            return new List<Item>(items);
        }

        // Clears all items from the inventory.
        public void ClearInventory()
        {
            items.Clear();
            Debug.Log("Inventory: Inventory cleared.");
            OnInventoryChanged?.Invoke(); // Notify subscribers that inventory has changed
        }

        private void OnValidate()
        {
            // Ensure maxInventorySlots is at least 1 in the inspector
            if (maxInventorySlots < 1)
            {
                maxInventorySlots = 1;
                Debug.LogWarning(
                    "Inventory: Max inventory slots cannot be less than 1. Setting to 1."
                );
            }
        }
    }
}
