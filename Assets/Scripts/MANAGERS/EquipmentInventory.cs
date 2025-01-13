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

        private Dictionary<Equipment.EquipmentSlot, List<EquipmentWrapper>> equipmentPerSlot =
            new();

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

        public bool AddEquipment(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentWrapper == null || equipmentWrapper.equipmentData == null)
            {
                Debug.LogWarning("EquipmentInventory: EquipmentWrapper or its data is null.");
                return false;
            }

            var slot = equipmentWrapper.equipmentData.equipmentSlot;

            if (!equipmentPerSlot.ContainsKey(slot))
            {
                equipmentPerSlot[slot] = new List<EquipmentWrapper>();
            }

            if (equipmentPerSlot[slot].Count >= maxInventorySlots)
            {
                Debug.LogWarning("EquipmentInventory: Inventory is full.");
                return false;
            }

            equipmentPerSlot[slot].Add(equipmentWrapper);
            Debug.Log(
                $"EquipmentInventory: Added {equipmentWrapper.equipmentData.equipmentName} to inventory."
            );
            EquippableItemsUIManager.Instance.UpdateEquipmentUI();

            return true;
        }

        public List<EquipmentWrapper> GetAllEquipment(Equipment.EquipmentSlot slot)
        {
            if (equipmentPerSlot.ContainsKey(slot))
            {
                return equipmentPerSlot[slot];
            }
            return null;
        }

        public void RemoveEquipment(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentPerSlot.ContainsKey(equipmentWrapper.equipmentData.equipmentSlot))
            {
                equipmentPerSlot[equipmentWrapper.equipmentData.equipmentSlot]
                    .Remove(equipmentWrapper);
                Debug.Log(
                    $"Removed {equipmentWrapper.equipmentData.equipmentName} from inventory."
                );
            }
        }
    }
}
