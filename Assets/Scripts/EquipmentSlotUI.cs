using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class EquipmentSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private Button equipButton;

        [SerializeField]
        private Button unequipButton;

        private Item equippedItem;

        private void Start()
        {
            UpdateSlotUI();

            equipButton.onClick.AddListener(OnEquipButtonClicked);
            unequipButton.onClick.AddListener(OnUnequipButtonClicked);
        }

        public void SetEquippedItem(Item item)
        {
            equippedItem = item;
            UpdateSlotUI();
        }

        public Item EquippedItem => equippedItem;

        private void OnEquipButtonClicked()
        {
            if (equippedItem != null)
            {
                int slotIndex = GetSlotIndex(); // Get the index of this slot
                EquipmentManager.Instance?.EquipItem(equippedItem, slotIndex);
                FloatingTextManager.Instance.ShowFloatingText(
                    $"Equipped {equippedItem.ItemName}",
                    PlayerMovement.Instance.transform,
                    Color.green
                );
                UpdateSlotUI();
            }
            else
            {
                Debug.LogWarning("EquipmentSlotUI: No item to equip.");
            }
        }

        private void OnUnequipButtonClicked()
        {
            int slotIndex = GetSlotIndex();
            EquipmentManager.Instance?.UnequipItem(slotIndex);
            FloatingTextManager.Instance.ShowFloatingText(
                $"Unequipped {equippedItem.ItemName}",
                PlayerMovement.Instance.transform,
                Color.red
            );
            equippedItem = null;
            UpdateSlotUI();
        }

        private void UpdateSlotUI()
        {
            if (equippedItem != null)
            {
                itemIcon.sprite = equippedItem.Icon;
                itemIcon.enabled = true;
                equipButton.gameObject.SetActive(false);
                unequipButton.gameObject.SetActive(true);
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
                equipButton.gameObject.SetActive(true);
                unequipButton.gameObject.SetActive(false);
            }
        }

        private int GetSlotIndex()
        {
            return transform.GetSiblingIndex();
        }
    }
}
