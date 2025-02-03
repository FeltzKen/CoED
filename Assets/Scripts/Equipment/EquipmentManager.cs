using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        // Tracks currently equipped items per slot.
        private Dictionary<EquipmentSlot, Equipment> equippedItems =
            new Dictionary<EquipmentSlot, Equipment>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        /// <summary>
        /// Equips the given equipment. If an item is already equipped in that slot (and is not cursed),
        /// it is unequipped first. All stat modifiers and extra effects (damage modifiers, resistances,
        /// weaknesses, status effects, etc.) are then applied to the player.
        /// </summary>
        public void EquipItem(Equipment equipment)
        {
            EquipmentSlot slot = equipment.slot;

            // Prevent replacing a cursed item.
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

            // Reveal any hidden modifiers if needed.
            if (!equipment.hasBeenRevealed)
                equipment.RevealHiddenAttributes();

            // If an item is already equipped in this slot, unequip it first.
            if (equippedItems.ContainsKey(slot))
            {
                Equipment old = equippedItems[slot];
                UnequipItem(old);
                EquipmentInventory.Instance.AddEquipment(old);
            }

            // Equip the new item.
            equippedItems[slot] = equipment;

            // Instead of recalculating from scratch, simply add this item's stat bonuses.
            PlayerStats.Instance.AddEquipmentStats(equipment.equipmentStats);

            // Also apply damage modifiers, resistances, weaknesses, and status effects.
            ApplyItemEffects(equipment);

            Debug.Log($"Equipped {equipment.itemName} in {slot} slot.");
            PlayerStats.Instance.CalculateStats(refillResources: false);
            EquippableItemsUIManager.Instance.UpdateEquipmentUI();
        }

        /// <summary>
        /// Unequips the given equipment (if it is not cursed) and removes its effects from the player.
        /// </summary>
        public void UnequipItem(Equipment equipment)
        {
            if (equipment.itemName.Contains("Cursed"))
            {
                Debug.LogWarning("Cannot unequip a cursed item.");
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

                // Remove the item's stat bonuses.
                PlayerStats.Instance.RemoveEquipmentStats(equipment.equipmentStats);

                // Also remove damage modifiers, resistances, weaknesses, and status effects.
                RemoveItemEffects(equipment);

                Debug.Log($"Unequipped {equipment.itemName} from {slot} slot.");
                PlayerStats.Instance.CalculateStats(refillResources: false);
                EquippableItemsUIManager.Instance.UpdateEquipmentUI();
            }
        }

        /// <summary>
        /// Applies all equipment modifications to the player.
        /// This includes:
        ///   • Base stat modifiers (from equipmentStats)
        ///   • Damage modifiers (from damageModifiers)
        ///   • Resistances, weaknesses, and immunities
        ///   • Active and inflictable status effects
        /// </summary>
        private void ApplyItemEffects(Equipment equipment)
        {
            PlayerStats ps = PlayerStats.Instance;
            // Add base stats.
            ps.AddEquipmentStats(equipment.equipmentStats);

            // Add damage modifiers.
            foreach (var kvp in equipment.damageModifiers)
            {
                ps.AddDamageModifier(kvp.Key, kvp.Value);
            }

            // Apply resistances.
            if (equipment.resistanceEffects != null)
            {
                foreach (var res in equipment.resistanceEffects)
                {
                    ps.AddResistance(res);
                }
            }

            // Apply weaknesses.
            if (equipment.weaknessEffects != null)
            {
                foreach (var weak in equipment.weaknessEffects)
                {
                    ps.AddWeakness(weak);
                }
            }

            // Apply active status effects.
            if (equipment.activeStatusEffects != null)
            {
                foreach (var status in equipment.activeStatusEffects)
                {
                    ps.AddActiveStatusEffect(status);
                }
            }

            // Apply inflictable status effects.
            if (equipment.inflictedStatusEffects != null)
            {
                foreach (var status in equipment.inflictedStatusEffects)
                {
                    ps.AddInflictableStatusEffect(status);
                }
            }
        }

        /// <summary>
        /// Removes all effects of the given equipment from the player.
        /// This reverses the application of base stat modifiers, damage modifiers,
        /// resistances, weaknesses, and status effects.
        /// </summary>
        private void RemoveItemEffects(Equipment equipment)
        {
            PlayerStats ps = PlayerStats.Instance;
            // Remove base stats.
            ps.RemoveEquipmentStats(equipment.equipmentStats);

            // Remove damage modifiers.
            foreach (var kvp in equipment.damageModifiers)
            {
                ps.RemoveDamageModifier(kvp.Key, kvp.Value);
            }

            // Remove resistances.
            if (equipment.resistanceEffects != null)
            {
                foreach (var res in equipment.resistanceEffects)
                {
                    ps.RemoveResistance(res);
                }
            }

            // Remove weaknesses.
            if (equipment.weaknessEffects != null)
            {
                foreach (var weak in equipment.weaknessEffects)
                {
                    ps.RemoveWeakness(weak);
                }
            }

            // Remove active status effects.
            if (equipment.activeStatusEffects != null)
            {
                foreach (var status in equipment.activeStatusEffects)
                {
                    ps.RemoveActiveStatusEffect(status);
                }
            }

            // Remove inflictable status effects.
            if (equipment.inflictedStatusEffects != null)
            {
                foreach (var status in equipment.inflictedStatusEffects)
                {
                    ps.RemoveInflictableStatusEffect(status);
                }
            }
        }

        public List<Equipment> GetAllEquippedItems()
        {
            return equippedItems.Values.ToList();
        }

        public Equipment GetEquippedItem(EquipmentSlot slot)
        {
            return equippedItems.ContainsKey(slot) ? equippedItems[slot] : null;
        }
    }
}
