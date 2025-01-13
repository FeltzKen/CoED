using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class EquippableItemsUIManager : MonoBehaviour
    {
        public static EquippableItemsUIManager Instance { get; private set; }

        [Header("Equippable Items Panel References")]
        [SerializeField]
        private Transform headSlot,
            chestSlot,
            legsSlot,
            waistSlot,
            weaponSlot,
            shieldSlot,
            bootsSlot;

        [Header("Equipped Items Panel References")]
        [SerializeField]
        private Transform equippedHeadSlot,
            equippedChestSlot,
            equippedLegsSlot,
            equippedWaistSlot,
            equippedWeaponSlot,
            equippedShieldSlot,
            equippedBootsSlot;

        [Header("Description Panel References")]
        [SerializeField]
        private TextMeshProUGUI itemNameText,
            attackModifierText,
            defenseModifierText,
            magicModifierText,
            healthModifierText,
            resistanceModifierText,
            speedModifierText;

        [Header("Stat Fields for Equipped Items")]
        [SerializeField]
        private TextMeshProUGUI headAttackText,
            headDefenseText,
            headMagicText,
            headHealthText,
            headSpeedText;

        [SerializeField]
        private TextMeshProUGUI chestAttackText,
            chestDefenseText,
            chestMagicText,
            chestHealthText,
            chestSpeedText;

        [SerializeField]
        private TextMeshProUGUI legsAttackText,
            legsDefenseText,
            legsMagicText,
            legsHealthText,
            legsSpeedText;

        [SerializeField]
        private TextMeshProUGUI waistAttackText,
            waistDefenseText,
            waistMagicText,
            waistHealthText,
            waistSpeedText;

        [SerializeField]
        private TextMeshProUGUI weaponAttackText,
            weaponDefenseText,
            weaponMagicText,
            weaponHealthText,
            weaponSpeedText;

        [SerializeField]
        private TextMeshProUGUI shieldAttackText,
            shieldDefenseText,
            shieldMagicText,
            shieldHealthText,
            shieldSpeedText;

        [SerializeField]
        private TextMeshProUGUI bootsAttackText,
            bootsDefenseText,
            bootsMagicText,
            bootsHealthText,
            bootsSpeedText;

        [Header("Resistance Fields for Equipped Items")]
        [SerializeField]
        private TextMeshProUGUI headResistanceText;

        [SerializeField]
        private TextMeshProUGUI chestResistanceText;

        [SerializeField]
        private TextMeshProUGUI legsResistanceText;

        [SerializeField]
        private TextMeshProUGUI waistResistanceText;

        [SerializeField]
        private TextMeshProUGUI weaponResistanceText;

        [SerializeField]
        private TextMeshProUGUI shieldResistanceText;

        [SerializeField]
        private TextMeshProUGUI bootsResistanceText;

        [Header("Equip/Unequip Buttons")]
        [SerializeField]
        private Button equipButton,
            unequipButton,
            dropButton;

        private Dictionary<Equipment.EquipmentSlot, Transform> equippableSlotMapping;
        private Dictionary<Equipment.EquipmentSlot, Transform> equippedSlotMapping;
        private Dictionary<Equipment.EquipmentSlot, List<GameObject>> equipmentButtonMapping =
            new();

        private EquipmentWrapper selectedEquippableItem = null;
        private EquipmentWrapper selectedEquippedItem = null;

        // Track selected UI buttons for both panels
        private GameObject selectedEquippableButton = null;
        private GameObject selectedEquippedButton = null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            InitializeSlotMappings();
            Debug.Log("EquippableItemsUIManager initialized.");
            if (equipButton == null)
                Debug.LogError("Equip Button is not assigned!");
            if (unequipButton == null)
                Debug.LogError("Unequip Button is not assigned!");
            if (dropButton == null)
                Debug.LogError("Drop Button is not assigned!");

            equipButton.onClick.AddListener(HandleEquipButton);
            unequipButton.onClick.AddListener(HandleUnequipButton);
            dropButton.onClick.AddListener(HandleDropButton);
        }

        private void InitializeSlotMappings()
        {
            equippableSlotMapping = new Dictionary<Equipment.EquipmentSlot, Transform>
            {
                { Equipment.EquipmentSlot.Head, headSlot },
                { Equipment.EquipmentSlot.Chest, chestSlot },
                { Equipment.EquipmentSlot.Legs, legsSlot },
                { Equipment.EquipmentSlot.Waist, waistSlot },
                { Equipment.EquipmentSlot.Weapon, weaponSlot },
                { Equipment.EquipmentSlot.Shield, shieldSlot },
                { Equipment.EquipmentSlot.Boots, bootsSlot },
            };

            equippedSlotMapping = new Dictionary<Equipment.EquipmentSlot, Transform>
            {
                { Equipment.EquipmentSlot.Head, equippedHeadSlot },
                { Equipment.EquipmentSlot.Chest, equippedChestSlot },
                { Equipment.EquipmentSlot.Legs, equippedLegsSlot },
                { Equipment.EquipmentSlot.Waist, equippedWaistSlot },
                { Equipment.EquipmentSlot.Weapon, equippedWeaponSlot },
                { Equipment.EquipmentSlot.Shield, equippedShieldSlot },
                { Equipment.EquipmentSlot.Boots, equippedBootsSlot },
            };
            equipmentButtonMapping = new Dictionary<Equipment.EquipmentSlot, List<GameObject>>
            {
                { Equipment.EquipmentSlot.Head, new List<GameObject>() },
                { Equipment.EquipmentSlot.Chest, new List<GameObject>() },
                { Equipment.EquipmentSlot.Legs, new List<GameObject>() },
                { Equipment.EquipmentSlot.Waist, new List<GameObject>() },
                { Equipment.EquipmentSlot.Weapon, new List<GameObject>() },
                { Equipment.EquipmentSlot.Shield, new List<GameObject>() },
                { Equipment.EquipmentSlot.Boots, new List<GameObject>() },
            };
            //UpdateEquipmentUI();
        }

        public void UpdateEquipmentUI()
        {
            // Clear all existing buttons from Equippable Items Panel
            foreach (var slot in equippableSlotMapping.Values)
            {
                foreach (Transform child in slot)
                {
                    Destroy(child.gameObject);
                }
            }

            // Clear all existing buttons from Equipped Items Panel
            foreach (var slot in equippedSlotMapping.Values)
            {
                foreach (Transform child in slot)
                {
                    Destroy(child.gameObject);
                }
            }

            // Reinitialize equipmentButtonMapping for all slots
            equipmentButtonMapping = new Dictionary<Equipment.EquipmentSlot, List<GameObject>>
            {
                { Equipment.EquipmentSlot.Head, new List<GameObject>() },
                { Equipment.EquipmentSlot.Chest, new List<GameObject>() },
                { Equipment.EquipmentSlot.Legs, new List<GameObject>() },
                { Equipment.EquipmentSlot.Waist, new List<GameObject>() },
                { Equipment.EquipmentSlot.Weapon, new List<GameObject>() },
                { Equipment.EquipmentSlot.Shield, new List<GameObject>() },
                { Equipment.EquipmentSlot.Boots, new List<GameObject>() },
            };

            // Repopulate Equippable Items Panel
            foreach (var slot in equippableSlotMapping)
            {
                Equipment.EquipmentSlot equipmentSlot = slot.Key;
                Transform slotContainer = slot.Value;

                List<EquipmentWrapper> currentEquipments =
                    EquipmentInventory.Instance.GetAllEquipment(equipmentSlot);
                if (currentEquipments != null)
                {
                    foreach (var equipment in currentEquipments)
                    {
                        GameObject buttonObj = CreateItemButton(equipment, slotContainer);
                        equipmentButtonMapping[equipment.equipmentData.equipmentSlot]
                            .Add(buttonObj);
                    }
                }
            }

            // Repopulate Equipped Items Panel
            foreach (var slot in equippedSlotMapping)
            {
                Equipment.EquipmentSlot equipmentSlot = slot.Key;
                Transform slotContainer = slot.Value;

                EquipmentWrapper equippedItem = EquipmentManager.Instance.GetEquippedItem(
                    equipmentSlot
                );
                if (equippedItem != null)
                {
                    GameObject buttonObj = CreateItemButton(equippedItem, slotContainer);
                    equipmentButtonMapping[equippedItem.equipmentData.equipmentSlot].Add(buttonObj);
                }
            }
        }

        private GameObject CreateItemButton(EquipmentWrapper equipmentWrapper, Transform parentSlot)
        {
            // Ensure the slot list is initialized
            if (!equipmentButtonMapping.ContainsKey(equipmentWrapper.equipmentData.equipmentSlot))
            {
                equipmentButtonMapping[equipmentWrapper.equipmentData.equipmentSlot] =
                    new List<GameObject>();
            }

            // Create a unique button if it doesn't already exist
            GameObject button = new GameObject(
                $"{equipmentWrapper.equipmentData.equipmentName}_{Guid.NewGuid()}"
            );
            button.transform.SetParent(parentSlot, false);

            RectTransform buttonRect = button.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0.5f, 0.5f); // permanent size

            // Create the background layer
            GameObject background = new GameObject("Border");
            background.transform.SetParent(button.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(0.65f, 0.65f); // permanent size
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.green; // Highlight color

            // Create the icon mask layer
            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(button.transform, false);
            RectTransform maskRect = mask.AddComponent<RectTransform>();
            maskRect.sizeDelta = new Vector2(0.55f, 0.55f); // permanent size
            Image maskImage = mask.AddComponent<Image>();
            maskImage.color = Color.black; // black

            // Create the icon layer
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(button.transform, false);
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(0.55f, 0.55f); // permanent size
            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = equipmentWrapper.equipmentData.icon;
            iconImage.preserveAspect = true;

            mask.SetActive(false);
            background.SetActive(false);
            // Add the Button component
            Button buttonComponent = button.AddComponent<Button>();

            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(() =>
            {
                Debug.Log(
                    $"Button for {equipmentWrapper.equipmentData.equipmentName} clicked. | Hash: {equipmentWrapper.GetHashCode()}"
                );
                SelectItem(
                    equipmentWrapper,
                    parentSlot
                        == equippableSlotMapping[equipmentWrapper.equipmentData.equipmentSlot],
                    button
                );
            });

            // Add to the mapping
            if (
                !equipmentButtonMapping[equipmentWrapper.equipmentData.equipmentSlot]
                    .Contains(button)
            )
            {
                equipmentButtonMapping[equipmentWrapper.equipmentData.equipmentSlot].Add(button);
            }
            Debug.Log(
                $"Created button for {equipmentWrapper.equipmentData.equipmentName} in {parentSlot.name}."
            );
            return button;
        }

        private void SelectItem(
            EquipmentWrapper equipmentWrapper,
            bool isEquippablePanel,
            GameObject clickedButton
        )
        {
            Debug.Log($"Selected {equipmentWrapper.equipmentData.equipmentName}");

            // Deselect any previously selected item
            if (selectedEquippableButton != null)
            {
                selectedEquippableButton.transform.Find("Border").gameObject.SetActive(false);
                selectedEquippableButton.transform.Find("Mask").gameObject.SetActive(false);
                selectedEquippableItem = null;
                selectedEquippableButton = null;
            }

            if (selectedEquippedButton != null)
            {
                selectedEquippedButton.transform.Find("Border").gameObject.SetActive(false);
                selectedEquippedButton.transform.Find("Mask").gameObject.SetActive(false);
                selectedEquippedItem = null;
                selectedEquippedButton = null;
            }

            // Assign the new selection
            if (isEquippablePanel)
            {
                selectedEquippableItem = equipmentWrapper;
                selectedEquippableButton = clickedButton;
                Debug.Log(
                    $"current selected item: {selectedEquippableItem.equipmentData.equipmentName} is from the equippable panel"
                );
            }
            else
            {
                selectedEquippedItem = equipmentWrapper;
                selectedEquippedButton = clickedButton;
                Debug.Log(
                    $"current selected item: {selectedEquippedItem.equipmentData.equipmentName} is from the equipped panel"
                );
            }

            // Highlight selected item
            clickedButton.transform.Find("Border").gameObject.SetActive(true);
            clickedButton.transform.Find("Mask").gameObject.SetActive(true);

            // Display item stats
            DisplayItemStats(equipmentWrapper);
        }

        private void HandleEquipButton()
        {
            Debug.Log("Equip button clicked.");

            if (selectedEquippableItem == null)
            {
                Debug.LogWarning("No item selected to equip.");
                return;
            }

            Equipment.EquipmentSlot slot = selectedEquippableItem.equipmentData.equipmentSlot;
            EquipmentWrapper currentlyEquippedItem = EquipmentManager.Instance.GetEquippedItem(
                slot
            );

            // Check if the item currently equipped in the target slot is cursed
            if (currentlyEquippedItem != null && currentlyEquippedItem.IsCursed)
            {
                Debug.LogWarning(
                    $"Cannot unequip {currentlyEquippedItem.equipmentData.equipmentName} because it is cursed."
                );
                FloatingTextManager.Instance.ShowFloatingText(
                    $"{currentlyEquippedItem.equipmentData.equipmentName} is cursed and cannot be removed!",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return;
            }

            if (currentlyEquippedItem != null)
            {
                Debug.Log(
                    $"Replacing {currentlyEquippedItem.equipmentData.equipmentName} in slot {slot}"
                );
                EquipmentManager.Instance.UnequipItem(currentlyEquippedItem);
                EquipmentInventory.Instance.AddEquipment(currentlyEquippedItem);
                MoveButtonToPanel(equipmentButtonMapping[slot][0], equippableSlotMapping[slot]);
                UpdateEquippedItemStats(slot, null);
            }

            EquipmentManager.Instance.EquipItem(selectedEquippableItem);
            MoveButtonToPanel(selectedEquippableButton, equippedSlotMapping[slot]);
            EquipmentInventory.Instance.RemoveEquipment(selectedEquippableItem);
            UpdateEquippedItemStats(slot, selectedEquippableItem);

            selectedEquippableItem = null;
            selectedEquippableButton = null;

            // Force UI update to reflect changes
            UpdateEquipmentUI();
        }

        private void HandleUnequipButton()
        {
            if (selectedEquippedItem == null)
                return;

            if (selectedEquippedItem.IsCursed)
            {
                Debug.Log("Cannot unequip a cursed item.");
                FloatingTextManager.Instance.ShowFloatingText(
                    "Cannot unequip a cursed item.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return;
            }
            EquipmentManager.Instance.UnequipItem(selectedEquippedItem);

            // Move the existing button back to the equippable panel
            MoveButtonToPanel(
                selectedEquippedButton,
                equippableSlotMapping[selectedEquippedItem.equipmentData.equipmentSlot]
            );

            EquipmentInventory.Instance.AddEquipment(selectedEquippedItem);

            // Clear the equipped item's stats
            UpdateEquippedItemStats(selectedEquippedItem.equipmentData.equipmentSlot, null);

            selectedEquippedItem = null;
            selectedEquippedButton = null;

            UpdateEquipmentUI(); // Optional if dynamic updates are needed
        }

        private void MoveButtonToPanel(GameObject button, Transform targetPanel)
        {
            button.transform.SetParent(targetPanel, false);
            button.transform.SetAsLastSibling(); // Optional: keeps the latest item on top
        }

        private void UpdateEquippedItemStats(Equipment.EquipmentSlot slot, EquipmentWrapper item)
        {
            // Reference the correct UI fields based on the equipmentWrapper slot
            TextMeshProUGUI attackText = null,
                defenseText = null,
                magicText = null,
                healthText = null,
                speedText = null,
                resistanceText = null;

            switch (slot)
            {
                case Equipment.EquipmentSlot.Head:
                    attackText = headAttackText;
                    defenseText = headDefenseText;
                    magicText = headMagicText;
                    healthText = headHealthText;
                    speedText = headSpeedText;
                    resistanceText = headResistanceText;
                    break;
                case Equipment.EquipmentSlot.Chest:
                    attackText = chestAttackText;
                    defenseText = chestDefenseText;
                    magicText = chestMagicText;
                    healthText = chestHealthText;
                    speedText = chestSpeedText;
                    resistanceText = chestResistanceText;
                    break;
                case Equipment.EquipmentSlot.Legs:
                    attackText = legsAttackText;
                    defenseText = legsDefenseText;
                    magicText = legsMagicText;
                    healthText = legsHealthText;
                    speedText = legsSpeedText;
                    resistanceText = legsResistanceText;
                    break;
                case Equipment.EquipmentSlot.Waist:
                    attackText = waistAttackText;
                    defenseText = waistDefenseText;
                    magicText = waistMagicText;
                    healthText = waistHealthText;
                    speedText = waistSpeedText;
                    resistanceText = waistResistanceText;
                    break;
                case Equipment.EquipmentSlot.Weapon:
                    attackText = weaponAttackText;
                    defenseText = weaponDefenseText;
                    magicText = weaponMagicText;
                    healthText = weaponHealthText;
                    speedText = weaponSpeedText;
                    resistanceText = weaponResistanceText;
                    break;
                case Equipment.EquipmentSlot.Shield:
                    attackText = shieldAttackText;
                    defenseText = shieldDefenseText;
                    magicText = shieldMagicText;
                    healthText = shieldHealthText;
                    speedText = shieldSpeedText;
                    resistanceText = shieldResistanceText;
                    break;
                case Equipment.EquipmentSlot.Boots:
                    attackText = bootsAttackText;
                    defenseText = bootsDefenseText;
                    magicText = bootsMagicText;
                    healthText = bootsHealthText;
                    speedText = bootsSpeedText;
                    resistanceText = bootsResistanceText;
                    break;
            }

            // Populate or clear the fields
            if (item != null)
            {
                if (!item.hasBeenRevealed)
                {
                    attackText.text = item.equipmentData.attackModifier.ToString();
                    defenseText.text = item.equipmentData.defenseModifier.ToString();
                    magicText.text = item.equipmentData.magicModifier.ToString();
                    healthText.text = item.equipmentData.healthModifier.ToString();
                    speedText.text = item.equipmentData.speedModifier.ToString();

                    // Display resistance if present
                    if (item.equipmentData.resistance != null)
                    {
                        resistanceText.text = item.equipmentData.resistance.effectType.ToString();
                    }
                    else
                    {
                        resistanceText.text = "None";
                    }
                }
                else
                {
                    attackText.text = item.GetTotalAttackModifier().ToString();
                    defenseText.text = item.GetTotalDefenseModifier().ToString();
                    magicText.text = item.GetTotalMagicModifier().ToString();
                    healthText.text = item.GetTotalHealthModifier().ToString();
                    speedText.text = item.GetTotalSpeedModifier().ToString();

                    // Display resistance if present
                    if (item.equipmentData.resistance != null)
                    {
                        resistanceText.text = item.equipmentData.resistance.effectType.ToString();
                    }
                    else
                    {
                        resistanceText.text = "None";
                    }
                }
            }
            else
            {
                attackText.text = "0";
                defenseText.text = "0";
                magicText.text = "0";
                healthText.text = "0";
                speedText.text = "0";
                resistanceText.text = "None";
            }
        }

        private void HandleDropButton()
        {
            Debug.Log("Drop button clicked.");

            if (selectedEquippedItem != null)
            {
                if (selectedEquippedItem.IsCursed)
                {
                    Debug.LogWarning(
                        $"{selectedEquippedItem.equipmentData.equipmentName} is cursed and cannot be dropped."
                    );
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Cannot drop a cursed item.",
                        PlayerStats.Instance.transform,
                        Color.red
                    );
                    return;
                }

                EquipmentManager.Instance.UnequipItem(selectedEquippedItem);
                DropItem(selectedEquippedItem);
                Destroy(selectedEquippedButton);

                selectedEquippedItem = null;
                selectedEquippedButton = null;
            }
            else if (selectedEquippableItem != null)
            {
                EquipmentInventory.Instance.RemoveEquipment(selectedEquippableItem);
                DropItem(selectedEquippableItem);
                Destroy(selectedEquippableButton);

                selectedEquippableItem = null;
                selectedEquippableButton = null;
            }
            DisplayItemStats(null, true);
            // Force UI update
            UpdateEquipmentUI();
        }

        private void DropItem(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentWrapper == null)
            {
                Debug.LogWarning("DropItem called with null equipmentWrapper.");
                return;
            }

            GameObject dropPrefab = equipmentWrapper.equipmentData.itemPrefab;

            if (dropPrefab == null)
            {
                Debug.LogWarning(
                    $"No drop prefab assigned for {equipmentWrapper.equipmentData.equipmentName}."
                );
                return;
            }

            Vector3 dropPosition = PlayerStats.Instance.transform.position + Vector3.forward;
            Instantiate(dropPrefab, dropPosition, Quaternion.identity);

            Debug.Log(
                $"{equipmentWrapper.equipmentData.equipmentName} has been dropped at {dropPosition}."
            );
        }

        public void DisplayItemStats(EquipmentWrapper equipmentWrapper, bool clear = false)
        {
            if (clear)
            {
                Debug.Log("Item stats are being cleared.");
                itemNameText.text = "No Item";
                attackModifierText.text = "0";
                defenseModifierText.text = "0";
                magicModifierText.text = "0";
                healthModifierText.text = "0";
                speedModifierText.text = "0";
                // Add resistance
                resistanceModifierText.text = "None";
                return;
            }
            if (!equipmentWrapper.hasBeenRevealed)
            {
                Debug.Log("Concealed Item stats are being displayed.");
                itemNameText.text = equipmentWrapper.equipmentData.equipmentName;
                attackModifierText.text =
                    "+ " + equipmentWrapper.equipmentData.attackModifier.ToString();
                defenseModifierText.text =
                    "+ " + equipmentWrapper.equipmentData.defenseModifier.ToString();
                magicModifierText.text =
                    "+ " + equipmentWrapper.equipmentData.magicModifier.ToString();
                healthModifierText.text =
                    "+ " + equipmentWrapper.equipmentData.healthModifier.ToString();
                speedModifierText.text =
                    "+ " + equipmentWrapper.equipmentData.speedModifier.ToString();
                // Show resistance if available
                if (equipmentWrapper.equipmentData.resistance != null)
                {
                    resistanceModifierText.text =
                        equipmentWrapper.equipmentData.resistance.effectType.ToString();
                }
                else
                {
                    resistanceModifierText.text = "None";
                }
            }
            else
            {
                Debug.Log("Revealed Item stats are being displayed.");
                itemNameText.text = equipmentWrapper.equipmentData.equipmentName;
                attackModifierText.text = equipmentWrapper.GetTotalAttackModifier().ToString();
                defenseModifierText.text = equipmentWrapper.GetTotalDefenseModifier().ToString();
                magicModifierText.text = equipmentWrapper.GetTotalMagicModifier().ToString();
                healthModifierText.text = equipmentWrapper.GetTotalHealthModifier().ToString();
                speedModifierText.text = equipmentWrapper.GetTotalSpeedModifier().ToString();
                // Show resistance if available
                if (equipmentWrapper.equipmentData.resistance != null)
                {
                    resistanceModifierText.text =
                        equipmentWrapper.equipmentData.resistance.effectType.ToString();
                }
                else
                {
                    resistanceModifierText.text = "None";
                }
            }
        }
    }
}
