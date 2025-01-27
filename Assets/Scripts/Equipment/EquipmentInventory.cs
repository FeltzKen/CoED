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

        private Dictionary<EquipmentSlot, List<Equipment>> equipmentPerSlot = new();

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

        public void AddQuestItem(QuestItem reward)
        {
            // Add special item from quest.
        }

        public bool AddEquipment(Equipment equipmentData)
        {
            if (equipmentData == null || equipmentData == null)
            {
                Debug.LogWarning("EquipmentInventory: EquipmentWrapper or its data is null.");
                return false;
            }

            var slot = equipmentData.slot;

            if (!equipmentPerSlot.ContainsKey(slot))
            {
                equipmentPerSlot[slot] = new List<Equipment>();
            }

            if (equipmentPerSlot[slot].Count >= maxInventorySlots)
            {
                Debug.LogWarning("EquipmentInventory: Inventory is full.");
                return false;
            }

            equipmentPerSlot[slot].Add(equipmentData);
            Debug.Log($"EquipmentInventory: Added {equipmentData.itemName} to inventory.");
            EquippableItemsUIManager.Instance.UpdateEquipmentUI();

            return true;
        }

        public List<Equipment> GetAllEquipment()
        {
            List<Equipment> allEquipment = new();
            foreach (var slot in equipmentPerSlot)
            {
                allEquipment.AddRange(slot.Value);
            }
            return allEquipment;
        }


        public void RemoveEquipment(Equipment equipment)
        {
            if (equipmentPerSlot.ContainsKey(equipment.slot))
            {
                equipmentPerSlot[equipment.slot].Remove(equipment);
                Debug.Log($"Removed {equipment.itemName} from inventory.");
            }
        }
    }
}
