using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        private Dictionary<EquipmentSlot, Equipment> equippedItems = new();

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
            EquipmentSlot slot = equipment.slot;

            if (equippedItems.ContainsKey(slot) && equippedItems[slot].itemName.Contains("Cursed"))
            {
                Debug.LogWarning(
                    $"Cannot replace {equippedItems[slot].itemName} because it is cursed."
                );
                FloatingTextManager.Instance.ShowFloatingText(
                    "Cannot unequip a cursed item.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return;
            }

            if (!equipment.hasBeenRevealed)
            {
                RevealItem(equipment);
            }

            equippedItems[slot] = equipment;

            // ✅ Apply full effects
            ApplyItemEffects(equipment);

            Debug.Log($"Equipped {equipment.itemName} in {slot} slot.");
            PlayerStats.Instance?.SetEquipmentStats(equippedItems.Values.ToList());
            PlayerStats.Instance?.CalculateStats(refillResources: false);
            EquippableItemsUIManager.Instance.UpdateEquipmentUI();
        }

        private void RevealItem(Equipment equipment)
        {
            equipment.hasBeenRevealed = true;
        }

        private void ChangeItemSprite(Equipment equipment, Color color)
        {
            //
        }

        public Equipment GetEquippedItem(EquipmentSlot slot)
        {
            return equippedItems.ContainsKey(slot) ? equippedItems[slot] : null;
        }

        public void UnequipItem(Equipment equipment)
        {
            if (equipment.itemName.Contains("Cursed"))
            {
                Debug.LogWarning($"{equipment.itemName} is cursed and cannot be unequipped.");
                FloatingTextManager.Instance.ShowFloatingText(
                    "Cannot unequip a cursed item.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return;
            }

            EquipmentSlot slot = equipment.slot;

            if (equippedItems.ContainsKey(slot))
            {
                equippedItems.Remove(slot);

                // ✅ Remove all effects
                RemoveItemEffects(equipment);

                Debug.Log($"Unequipped {equipment.itemName} from {slot} slot.");
                PlayerStats.Instance?.SetEquipmentStats(equippedItems.Values.ToList());
                PlayerStats.Instance?.CalculateStats(refillResources: false);
                EquippableItemsUIManager.Instance.UpdateEquipmentUI();
            }
        }

        private void ApplyItemEffects(Equipment equipment)
        {
            // Apply Resistances
            if (equipment.resistanceEffects != null)
            {
                foreach (var effect in equipment.resistanceEffects)
                {
                    if (!PlayerStats.Instance.activeResistances.Contains(effect))
                    {
                        PlayerStats.Instance.activeResistances.Add(effect);
                        Debug.Log($"Added resistance to {effect}");
                    }
                }
            }

            // Apply Weaknesses
            if (equipment.weaknessEffects != null)
            {
                foreach (var effect in equipment.weaknessEffects)
                {
                    if (!PlayerStats.Instance.activeWeaknesses.Contains(effect))
                    {
                        PlayerStats.Instance.activeWeaknesses.Add(effect);
                        Debug.Log($"Added weakness to {effect}");
                    }
                }
            }

            // Apply Status Effects
            if (equipment.inflictedStatusEffects != null)
            {
                foreach (var effect in equipment.inflictedStatusEffects)
                {
                    if (!PlayerStats.Instance.inflictableStatusEffects.Contains(effect))
                    {
                        PlayerStats.Instance.inflictableStatusEffects.Add(effect);
                        Debug.Log($"Added inflictable status effect: {effect}");
                    }
                }
            }
            if (equipment.activeStatusEffects != null)
            {
                foreach (var effect in equipment.activeStatusEffects)
                {
                    if (!PlayerStats.Instance.activeStatusEffects.Contains(effect))
                    {
                        PlayerStats.Instance.activeStatusEffects.Add(effect);
                        Debug.Log($"Added inflictable status effect: {effect}");
                    }
                }
            }
        }

        private void RemoveItemEffects(Equipment equipment)
        {
            // Remove Resistances
            if (equipment.resistanceEffects != null)
            {
                foreach (var effect in equipment.resistanceEffects)
                {
                    PlayerStats.Instance.activeResistances.Remove(effect);
                    Debug.Log($"Removed resistance to {effect}");
                }
            }

            // Remove Weaknesses
            if (equipment.weaknessEffects != null)
            {
                foreach (var effect in equipment.weaknessEffects)
                {
                    PlayerStats.Instance.activeWeaknesses.Remove(effect);
                    Debug.Log($"Removed weakness to {effect}");
                }
            }

            // Remove Status Effects
            if (equipment.inflictedStatusEffects != null)
            {
                foreach (var effect in equipment.inflictedStatusEffects)
                {
                    PlayerStats.Instance.inflictableStatusEffects.Remove(effect);
                    Debug.Log($"Removed status effect: {effect}");
                }
            }
            if (equipment.activeStatusEffects != null)
            {
                foreach (var effect in equipment.activeStatusEffects)
                {
                    PlayerStats.Instance.activeStatusEffects.Remove(effect);
                    Debug.Log($"Removed status effect: {effect}");
                }
            }
        }
    }
}
