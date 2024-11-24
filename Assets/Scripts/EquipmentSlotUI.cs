using UnityEngine;
using UnityEngine.UI;
using CoED;

namespace CoED
{
    public class EquipmentSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private Image itemIcon; // Reference to the UI image that displays the item icon

        [SerializeField]
        private Button equipButton; // Button to equip the item

        [SerializeField]
        private Button unequipButton; // Button to unequip the item

        private Item equippedItem; // The item currently equipped in this slot

        private void Start()
        {
            UpdateSlotUI(); // Initialize the UI state based on the current equipped item

            // Add listeners to button clicks
            equipButton.onClick.AddListener(OnEquipButtonClicked);
            unequipButton.onClick.AddListener(OnUnequipButtonClicked);
        }

        // Sets the equipped item for this slot and updates the UI
        public void SetEquippedItem(Item item)
        {
            equippedItem = item; // Assign the item to this slot
            UpdateSlotUI(); // Refresh the UI
        }

        // Property to get the currently equipped item
        public Item EquippedItem => equippedItem;

        // Handle equip button click
        private void OnEquipButtonClicked()
        {
            if (equippedItem != null)
            {
                int slotIndex = GetSlotIndex(); // Get the index of this slot
                EquipmentManager.Instance?.EquipItem(equippedItem, slotIndex);
                Debug.Log($"Equipped item: {equippedItem.ItemName}");
                UpdateSlotUI(); // Refresh the UI after equipping
            }
            else
            {
                Debug.LogWarning("EquipmentSlotUI: No item to equip.");
            }
        }

        // Handle unequip button click
        private void OnUnequipButtonClicked()
        {
            int slotIndex = GetSlotIndex(); // Get the index of this slot
            EquipmentManager.Instance?.UnequipItem(slotIndex);
            Debug.Log($"Unequipped item: {equippedItem.ItemName}");
            equippedItem = null; // Clear the reference
            UpdateSlotUI(); // Refresh the UI after unequipping
        }

        // Updates the UI to reflect the current state of the equipment slot
        private void UpdateSlotUI()
        {
            if (equippedItem != null)
            {
                itemIcon.sprite = equippedItem.Icon; // Set the icon to the equipped item's icon
                itemIcon.enabled = true; // Show the icon
                equipButton.gameObject.SetActive(false); // Hide the equip button
                unequipButton.gameObject.SetActive(true); // Show the unequip button
            }
            else
            {
                itemIcon.sprite = null; // Clear the icon
                itemIcon.enabled = false; // Hide the icon
                equipButton.gameObject.SetActive(true); // Show the equip button
                unequipButton.gameObject.SetActive(false); // Hide the unequip button
            }
        }

        // Gets the index of the equipment slot, which should be set based on the UI setup
        private int GetSlotIndex()
        {
            // This should return the actual slot index based on the current setup
            return transform.GetSiblingIndex(); // Get the index of this slot among its siblings
        }
    }
}
