using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        private Dictionary<Equipment.EquipmentSlot, EquipmentWrapper> equippedItems = new();

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

        public void EquipItem(EquipmentWrapper equipmentWrapper)
        {
            Equipment.EquipmentSlot slot = equipmentWrapper.equipmentData.equipmentSlot;

            // 🚨 Check if the slot is occupied by a cursed item
            if (equippedItems.ContainsKey(slot) && equippedItems[slot].IsCursed)
            {
                Debug.LogWarning(
                    $"Cannot replace {equippedItems[slot].equipmentData.equipmentName} because it is cursed."
                );
                FloatingTextManager.Instance.ShowFloatingText(
                    "Cannot unequip a cursed item.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return; // 🚫 Block replacement
            }

            // Reveal the item upon equipping
            if (!equipmentWrapper.hasBeenRevealed)
            {
                RevealItem(equipmentWrapper);
            }

            // Equip the item
            equippedItems[slot] = equipmentWrapper;

            // Apply resistance if present
            AddResistance(equipmentWrapper);

            Debug.Log($"Equipped {equipmentWrapper.equipmentData.equipmentName} in {slot} slot.");
            PlayerStats.Instance?.SetEquipmentStats(equippedItems.Values.ToList());
            PlayerStats.Instance?.CalculateStats();
            EquippableItemsUIManager.Instance.UpdateEquipmentUI();
        }

        private void RevealItem(EquipmentWrapper equipmentWrapper)
        {
            equipmentWrapper.hasBeenRevealed = true;

            if (equipmentWrapper.IsEnchanted)
            {
                equipmentWrapper.equipmentData.itemName =
                    $"Enchanted {equipmentWrapper.equipmentData.itemName}";
                ChangeItemSprite(equipmentWrapper, Color.blue);
            }
            else if (equipmentWrapper.IsCursed)
            {
                equipmentWrapper.equipmentData.itemName =
                    $"Cursed {equipmentWrapper.equipmentData.itemName}";
                ChangeItemSprite(equipmentWrapper, Color.red);
            }

            Debug.Log($"Item {equipmentWrapper.equipmentData.itemName} has been revealed.");
        }

        private void ChangeItemSprite(EquipmentWrapper equipmentWrapper, Color color)
        {
            if (
                equipmentWrapper.equipmentData.itemPrefab.TryGetComponent(
                    out SpriteRenderer renderer
                )
            )
            {
                renderer.color = color;
            }
        }

        public EquipmentWrapper GetEquippedItem(Equipment.EquipmentSlot slot)
        {
            return equippedItems.ContainsKey(slot) ? equippedItems[slot] : null;
        }

        public void UnequipItem(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentWrapper.IsCursed)
            {
                Debug.LogWarning(
                    $"{equipmentWrapper.equipmentData.equipmentName} is cursed and cannot be unequipped."
                );
                FloatingTextManager.Instance.ShowFloatingText(
                    "Cannot unequip a cursed item.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return; // 🚫 Block unequipping
            }

            Equipment.EquipmentSlot slot = equipmentWrapper.equipmentData.equipmentSlot;

            if (equippedItems.ContainsKey(slot))
            {
                equippedItems.Remove(slot);

                RemoveResistance(equipmentWrapper);

                Debug.Log(
                    $"Unequipped {equipmentWrapper.equipmentData.equipmentName} from {slot} slot."
                );
                PlayerStats.Instance?.SetEquipmentStats(equippedItems.Values.ToList());
                PlayerStats.Instance?.CalculateStats();
                EquippableItemsUIManager.Instance.UpdateEquipmentUI();
            }
        }

        private void AddResistance(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentWrapper.equipmentData.resistance != null)
            {
                if (
                    !PlayerStats.Instance.activeResistances.Contains(
                        equipmentWrapper.equipmentData.resistance.effectType
                    )
                )
                {
                    PlayerStats.Instance.activeResistances.Add(
                        equipmentWrapper.equipmentData.resistance.effectType
                    );
                    Debug.Log(
                        $"Added resistance to {equipmentWrapper.equipmentData.resistance.effectType}"
                    );
                }
            }
        }

        private void RemoveResistance(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentWrapper.equipmentData.resistance != null)
            {
                PlayerStats.Instance.activeResistances.Remove(
                    equipmentWrapper.equipmentData.resistance.effectType
                );
                Debug.Log(
                    $"Removed resistance to {equipmentWrapper.equipmentData.resistance.effectType}"
                );
            }
        }
    }
}
