using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        private Dictionary<Equipment.EquipmentSlot, Equipment> equippedItems = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void EquipItem(Equipment equipment)
        {
            Equipment.EquipmentSlot slot = equipment.equipmentSlot;

            // Unequip existing item in the same slot
            if (equippedItems.ContainsKey(slot))
            {
                UnequipItem(equipment);
            }
            if (equippedItems.ContainsKey(slot))
            {
                UnequipItem(equipment);
            }

            // Equip new item
            equippedItems[slot] = equipment;
            Debug.Log($"EquipmentManager: Equipped {equipment.name} in {slot} slot.");

            // Recalculate stats
            PlayerStats.Instance?.SetEquipmentStats(equippedItems.Values.ToList());
            PlayerStats.Instance?.CalculateStats();
        }

        private void UnequipItem(Equipment equipment)
        {
            Equipment.EquipmentSlot slot = equipment.equipmentSlot;

            if (equippedItems.ContainsKey(slot))
            {
                equippedItems.Remove(slot);
                Debug.Log($"EquipmentManager: Unequipped {equipment.name} from {slot} slot.");

                // Recalculate stats
                PlayerStats.Instance?.CalculateStats();
            }
        }
    }
}
