using System;
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
            // Validate the EquipmentWrapper and the Equipment data
            if (equipment == null)
            {
                Debug.LogError(
                    "EquippableItemsUIManager: Invalid EquipmentWrapper or Equipment data."
                );
                return;
            }

            // Find the appropriate panel for the equipment slot category
            Transform categoryPanel = GetCategoryPanel(equipment.equipmentSlot);
            if (categoryPanel == null)
            {
                Debug.LogError(
                    $"EquippableItemsUIManager: No panel for slot {equipment.equipmentSlot}"
                );
                return;
            }

            // Create the main button object
            GameObject button = new GameObject($"{equipment.equipmentName}Button");
            button.transform.SetParent(categoryPanel, false);

            RectTransform buttonRect = button.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0.5f, 0.5f); // Adjust as needed

            // Create the background layer
            GameObject background = new GameObject("Border");
            background.transform.SetParent(button.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(0.65f, 0.65f);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.green; // green

            // Create the icon mask layer
            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(button.transform, false);
            RectTransform maskRect = mask.AddComponent<RectTransform>();
            maskRect.sizeDelta = new Vector2(0.55f, 0.55f); // Adjust as needed
            Image maskImage = mask.AddComponent<Image>();
            maskImage.color = Color.black; // black

            // Add the icon inside the mask
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(button.transform, false);
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(0.5f, 0.5f); // Adjust as needed
            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = equipment.icon;
            iconImage.preserveAspect = true;

            mask.SetActive(false); // Hide the mask initially
            background.SetActive(false); // Hide the background initially
            // Add a button component for interactivity
            Button buttonComponent = button.AddComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                // Equip the selected item via the EquipmentManager
                EquipmentManager.Instance.EquipItem(equipment);
                DisplayItemStats(equipment);

                // Toggle the background visibility
                ToggleBordersOff(categoryPanel); // Ensure only one item is selected
                background.SetActive(true);
                mask.SetActive(true);
            });
        }

        private void ToggleBordersOff(Transform categoryPanel)
        {
            foreach (Transform child in categoryPanel)
            {
                Transform background = child.Find("Border");
                Transform mask = child.Find("Mask");
                if (background != null)
                {
                    background.gameObject.SetActive(false);
                    mask.gameObject.SetActive(false);
                }
            }
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
            attackModifierText.text = "+ " + equipment.attackModifier.ToString();
            defenseModifierText.text = "+ " + equipment.defenseModifier.ToString();
            magicModifierText.text = "+ " + equipment.magicModifier.ToString();
            healthModifierText.text = "+ " + equipment.healthModifier.ToString();
            speedModifierText.text = "+ " + equipment.speedModifier.ToString();
        }
    }
}
