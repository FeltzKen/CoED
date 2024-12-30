using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class EquippableItemsUIManager : MonoBehaviour
    {
        public static EquippableItemsUIManager Instance { get; private set; }

        [Header("UI Slot References")]
        [SerializeField]
        private Transform headSlot,
            chestSlot,
            legsSlot,
            waistSlot,
            weaponSlot,
            shieldSlot,
            bootsSlot;

        [Header("Description Panel References")]
        [SerializeField]
        private TextMeshProUGUI itemNameText,
            attackModifierText,
            defenseModifierText,
            magicModifierText,
            healthModifierText,
            speedModifierText;

        private Dictionary<Equipment.EquipmentSlot, Transform> slotMapping;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Duplicate EquippableItemsUIManager detected. Destroying.");
                Destroy(gameObject);
            }

            InitializeSlotMapping();
        }

        private void InitializeSlotMapping()
        {
            slotMapping = new Dictionary<Equipment.EquipmentSlot, Transform>
            {
                { Equipment.EquipmentSlot.Head, headSlot },
                { Equipment.EquipmentSlot.Chest, chestSlot },
                { Equipment.EquipmentSlot.Legs, legsSlot },
                { Equipment.EquipmentSlot.Waist, waistSlot },
                { Equipment.EquipmentSlot.Weapon, weaponSlot },
                { Equipment.EquipmentSlot.Shield, shieldSlot },
                { Equipment.EquipmentSlot.Boots, bootsSlot },
            };

            foreach (var slot in slotMapping)
            {
                if (slot.Value == null)
                {
                    Debug.LogError($"Slot for {slot.Key} is not assigned.");
                }
            }
        }

        public void AddEquipmentToUI(Equipment equipment)
        {
            Transform categoryPanel = GetCategoryPanel(equipment.equipmentSlot);
            if (categoryPanel == null)
            {
                Debug.LogError(
                    $"EquippableItemsUIManager: No panel for slot {equipment.equipmentSlot}"
                );
                return;
            }

            // Create the button dynamically
            GameObject button = new GameObject("EquipmentButton");
            button.transform.SetParent(categoryPanel, false);

            // Set icon
            Image iconImage = button.AddComponent<Image>();
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
            iconImage.sprite = equipment.icon;

            // Add Button component and set its behavior
            Button buttonComponent = button.AddComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                EquipmentManager.Instance.EquipItem(equipment);
                DisplayItemStats(equipment);
            });
        }

        private Transform GetCategoryPanel(Equipment.EquipmentSlot slot)
        {
            if (slotMapping.TryGetValue(slot, out Transform slotTransform))
            {
                Debug.Log($"Panel found for slot {slot}");
                return slotTransform;
            }
            else
            {
                Debug.LogError($"No panel found for slot {slot}");
                return null;
            }
        }

        private void HandleItemClick(Equipment equipment)
        {
            if (equipment == null)
                return;

            EquipmentManager.Instance.EquipItem(equipment);
            DisplayItemStats(equipment);
        }

        public void DisplayItemStats(Equipment equipment)
        {
            if (equipment == null)
            {
                itemNameText.text = "No Item";
                attackModifierText.text = "0";
                defenseModifierText.text = "0";
                magicModifierText.text = "0";
                healthModifierText.text = "0";
                speedModifierText.text = "0";
                return;
            }

            itemNameText.text = equipment.equipmentName;
            attackModifierText.text = equipment.attackModifier.ToString();
            defenseModifierText.text = equipment.defenseModifier.ToString();
            magicModifierText.text = equipment.magicModifier.ToString();
            healthModifierText.text = equipment.healthModifier.ToString();
            speedModifierText.text = equipment.speedModifier.ToString();
        }
    }
}
