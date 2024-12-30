using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class ConsumableInventory : MonoBehaviour
    {
        public static ConsumableInventory Instance { get; private set; }

        [Header("Inventory Settings")]
        [SerializeField]
        private int maxInventorySlots = 20;

        private List<Item> items = new List<Item>();

        public delegate void InventoryChangedHandler();
        public event InventoryChangedHandler OnInventoryChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning(
                    "Inventory: Duplicate instance detected. Destroying this instance."
                );
                Destroy(gameObject);
            }
        }

        public bool AddItem(Item item)
        {
            if (items.Count >= maxInventorySlots)
            {
                Debug.Log("Inventory: Inventory is full, cannot add item.");
                return false;
            }

            items.Add(item);
            OnInventoryChanged?.Invoke();
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

        public bool HasItem(Item item)
        {
            return items.Contains(item);
        }

        public List<Item> GetAllItems()
        {
            return new List<Item>(items);
        }

        public void ClearInventory()
        {
            items.Clear();
            OnInventoryChanged?.Invoke();
        }
    }
}
