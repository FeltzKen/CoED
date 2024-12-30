using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class EquipmentInventory : MonoBehaviour
    {
        public static EquipmentInventory Instance { get; private set; }

        [Header("Inventory Settings")]
        [SerializeField]
        private int maxInventorySlots = 20;

        private Dictionary<Equipment.EquipmentSlot, List<Equipment>> equipmentPerSlot = new();

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

        public void AddQuestItem(Item reward)
        {
            // Add special item from quest.
        }

        public bool AddEquipment(Equipment equipment)
        {
            if (!equipmentPerSlot.ContainsKey(equipment.equipmentSlot))
            {
                equipmentPerSlot[equipment.equipmentSlot] = new List<Equipment>();
            }
            if (equipmentPerSlot[equipment.equipmentSlot].Count >= maxInventorySlots)
            {
                Debug.LogWarning("EquipmentInventory: Inventory is full.");
                return false;
            }
            equipmentPerSlot[equipment.equipmentSlot].Add(equipment);
            Debug.Log($"EquipmentInventory: Added {equipment.equipmentName} to inventory.");

            return true;
        }
    }
}
