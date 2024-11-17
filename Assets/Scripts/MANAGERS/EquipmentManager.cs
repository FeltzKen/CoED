using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance { get; private set; }

        [Header("Equipment Slots")]
        [SerializeField]
        private List<EquipmentSlotUI> equipmentSlots = new List<EquipmentSlotUI>();

        private PlayerStats playerStats;

        private void Awake()
        {
            // Implement Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("EquipmentManager instance already exists. Destroying duplicate.");
                return;
            }

            if (Player.Instance != null)
            {
                playerStats = Player.Instance.playerStats;
            }

            if (playerStats == null)
            {
                Debug.LogError("EquipmentManager: PlayerStats reference not found.");
                enabled = false; // Disable script to prevent runtime errors
            }
        }

        /// Equips an item to the specified slot and updates player stats.
        public void EquipItem(Item equipment, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= equipmentSlots.Count)
            {
                Debug.LogWarning("EquipmentManager: Invalid slot index.");
                return;
            }

            EquipmentSlotUI slot = equipmentSlots[slotIndex];
            if (slot == null)
            {
                Debug.LogWarning("EquipmentManager: Equipment slot not assigned.");
                return;
            }

            Item previousItem = slot.EquippedItem;
            slot.SetEquippedItem(equipment);

            if (playerStats != null)
            {
                playerStats.SetEquipmentStats(
                    equipment.AttackBoost,
                    equipment.DefenseBoost,
                    equipment.HealthBoost
                );
                Debug.Log(
                    $"EquipmentManager: Equipped '{equipment.ItemName}' to slot {slotIndex}, updating stats."
                );

                if (previousItem != null)
                {
                    playerStats.SetEquipmentStats(
                        -previousItem.AttackBoost,
                        -previousItem.DefenseBoost,
                        -previousItem.HealthBoost
                    );
                    Debug.Log(
                        $"EquipmentManager: Removed '{previousItem.ItemName}', adjusting stats."
                    );
                }
            }
        }

        /// Unequips an item from the specified slot and updates player stats.
        public void UnequipItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= equipmentSlots.Count)
            {
                Debug.LogWarning("EquipmentManager: Invalid slot index.");
                return;
            }

            EquipmentSlotUI slot = equipmentSlots[slotIndex];
            if (slot == null || slot.EquippedItem == null)
            {
                Debug.LogWarning("EquipmentManager: No item to unequip in this slot.");
                return;
            }

            Item unequippedItem = slot.EquippedItem;
            slot.SetEquippedItem(null); // Unequip the item by setting it to null

            if (playerStats != null && unequippedItem != null)
            {
                playerStats.SetEquipmentStats(
                    -unequippedItem.AttackBoost,
                    -unequippedItem.DefenseBoost,
                    -unequippedItem.HealthBoost
                );
                Debug.Log(
                    $"EquipmentManager: Unequipped '{unequippedItem.ItemName}', adjusting stats."
                );
            }
        }

        /// Updates player stats when equipment changes occur.
        public void UpdateAllEquipmentStats()
        {
            if (playerStats == null)
            {
                Debug.LogError(
                    "EquipmentManager: PlayerStats reference not found during stat update."
                );
                return;
            }

            int totalAttackModifier = 0;
            int totalDefenseModifier = 0;
            int totalHealthModifier = 0;

            foreach (EquipmentSlotUI slot in equipmentSlots)
            {
                if (slot != null && slot.EquippedItem != null)
                {
                    totalAttackModifier += slot.EquippedItem.AttackBoost;
                    totalDefenseModifier += slot.EquippedItem.DefenseBoost;
                    totalHealthModifier += slot.EquippedItem.HealthBoost;
                }
            }

            playerStats.SetEquipmentStats(
                totalAttackModifier,
                totalDefenseModifier,
                totalHealthModifier
            );
            Debug.Log("EquipmentManager: Updated all equipment stats.");
        }
    }
}
