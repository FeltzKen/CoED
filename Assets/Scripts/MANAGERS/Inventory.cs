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
                Debug.LogWarning("Inventory: Duplicate instance detected. Destroying this instance.");
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
        //    Debug.Log($"Inventory: Added {item.ItemName}.");
        //    Debug.Log($"Inventory: {string.Join(", ", items)}");
            OnInventoryChanged?.Invoke(); // Notify subscribers that inventory has changed
            return true;
        }

        public void AddQuestItem(Item reward)
        {
            // Add special item from quest.
        }


        public bool RemoveItem(Item item)
        {
            if (items.Remove(item))
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
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
            // Debug.Log("Inventory: Inventory cleared.");
            OnInventoryChanged?.Invoke(); // Notify subscribers that inventory has changed
        }
    }
}
