using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class EquippableItemsUIManager : MonoBehaviour
    {
        public static EquippableItemsUIManager Instance { get; private set; }

        [Header("Item Panel Reference")]
        [SerializeField]
        private Transform equipmentPanel;
        private Transform consumablesPanel;
        private Transform equipmentParent;
        private Transform consumablesParent;

        [Header("Description Panel References")]
        [SerializeField]
        private TextMeshProUGUI equipmentNameText;

        [SerializeField]
        private TextMeshProUGUI attackModifierText;

        [SerializeField]
        private TextMeshProUGUI defenseModifierText;

        [SerializeField]
        private TextMeshProUGUI magicModifierText;

        [SerializeField]
        private TextMeshProUGUI healthModifierText;

        [SerializeField]
        private TextMeshProUGUI speedModifierText;

        [SerializeField]
        private TextMeshProUGUI intelligenceModifierText;

        [SerializeField]
        private TextMeshProUGUI dexterityModifierText;

        [SerializeField]
        private TextMeshProUGUI staminaModifierText;

        [SerializeField]
        private TextMeshProUGUI critChanceModifierText;

        [SerializeField]
        private TextMeshProUGUI burnDamageModifierText;

        [SerializeField]
        private TextMeshProUGUI iceDamageModifierText;

        [SerializeField]
        private TextMeshProUGUI lightningDamageModifierText;

        [SerializeField]
        private TextMeshProUGUI poisonDamageModifierText;

        [SerializeField]
        private TextMeshProUGUI activeStatusEffectsText;

        [SerializeField]
        private TextMeshProUGUI inflictedStatusEffectsText;

        [SerializeField]
        private TextMeshProUGUI resistanceEffectsText;

        [SerializeField]
        private TextMeshProUGUI weaknessEffectsText;

        [Header("Player Stats Panel")]
        [SerializeField]
        private TextMeshProUGUI playerAttackText;

        [SerializeField]
        private TextMeshProUGUI playerDefenseText;

        [SerializeField]
        private TextMeshProUGUI playerMagicText;

        [SerializeField]
        private TextMeshProUGUI playerHealthText;

        [SerializeField]
        private TextMeshProUGUI playerSpeedText;

        [SerializeField]
        private TextMeshProUGUI playerIntelligenceText;

        [SerializeField]
        private TextMeshProUGUI playerDexterityText;

        [SerializeField]
        private TextMeshProUGUI playerStaminaText;

        [SerializeField]
        private TextMeshProUGUI playerCritChanceText;

        [SerializeField]
        private TextMeshProUGUI playerBurnDamageText;

        [SerializeField]
        private TextMeshProUGUI playerIceDamageText;

        [SerializeField]
        private TextMeshProUGUI playerLightningDamageText;

        [SerializeField]
        private TextMeshProUGUI playerPoisonDamageText;

        [SerializeField]
        private TextMeshProUGUI playerResistanceText;

        [SerializeField]
        private TextMeshProUGUI playerWeaknessText;

        [SerializeField]
        private TextMeshProUGUI playerActiveStatusText;

        [SerializeField]
        private TextMeshProUGUI playerInflictedStatusText;

        [Header("Equipped Items Panel References")]
        [SerializeField]
        private Transform equippedHeadSlot;

        [SerializeField]
        private Transform equippedChestSlot;

        [SerializeField]
        private Transform equippedLegsSlot;

        [SerializeField]
        private Transform equippedWaistSlot;

        [SerializeField]
        private Transform equippedHandsSlot;

        [SerializeField]
        private Transform equippedWeaponSlot;

        [SerializeField]
        private Transform equippedShieldSlot;

        [SerializeField]
        private Transform equippedBootsSlot;

        [SerializeField]
        private Transform equippedRingSlot;

        [SerializeField]
        private Transform equippedAmuletSlot;

        [Header("Equip/Unequip/Drop Buttons")]
        [SerializeField]
        private Button equipButton;

        [SerializeField]
        private Button unequipButton;

        [SerializeField]
        private Button dropButton;

        // Maps each EquipmentSlot -> the container in the "equipped" items panel
        private Dictionary<EquipmentSlot, Transform> equippedSlotMapping;

        // Tracks the item-button GameObjects for each slot
        private Dictionary<EquipmentSlot, List<GameObject>> equipmentButtonMapping;

        private Equipment selectedEquipment;
        private GameObject selectedButton;
        private Equipment selectedEquippableItem;
        private Equipment selectedEquippedItem;
        private GameObject selectedEquippableButton;
        private GameObject selectedEquippedButton;

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (equipButton == null)
                Debug.LogError("Equip Button is not assigned in inspector!");
            if (unequipButton == null)
                Debug.LogError("Unequip Button is not assigned in inspector!");
            if (dropButton == null)
                Debug.LogError("Drop Button is not assigned in inspector!");

            // Hook up button events
            equipButton.onClick.AddListener(HandleEquipButton);
            unequipButton.onClick.AddListener(HandleUnequipButton);
            dropButton.onClick.AddListener(HandleDropButton);
            InitializeSlotMappings();
        }

        private void Start()
        {
            UpdateEquipmentUI();
        }

        public void UpdateEquipmentUI()
        {
            // 1) Clear existing buttons from both equippable and equipped panels
            foreach (Transform child in equipmentPanel)
            {
                Destroy(child.gameObject);
            }

            foreach (var slotParent in equippedSlotMapping.Values)
            {
                foreach (Transform child in slotParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // 2) Reset the mapping
            foreach (var slot in equipmentButtonMapping.Keys.ToList())
            {
                equipmentButtonMapping[slot].Clear();
            }

            // 3) Populate the equippable panel from inventory
            List<Equipment> itemsInInventory = EquipmentInventory.Instance.GetAllEquipment();
            if (itemsInInventory == null)
                return;

            foreach (var item in itemsInInventory)
            {
                // Pass equipmentPanel (the correct transform), not transform.GetChild(0)
                CreateItemButton(item, equipmentPanel, false);
            }

            // 4) Populate the equipped panel
            foreach (var kvp in equippedSlotMapping)
            {
                EquipmentSlot equipmentSlot = kvp.Key;
                Transform slotContainer = kvp.Value;

                Equipment equippedItem = EquipmentManager.Instance.GetEquippedItem(equipmentSlot);
                if (equippedItem != null)
                {
                    CreateItemButton(equippedItem, slotContainer, true);
                }
            }

            // 5) Show final player stats
            DisplayPlayerStats();
        }

        private void InitializeSlotMappings()
        {
            equippedSlotMapping = new Dictionary<EquipmentSlot, Transform>
            {
                { EquipmentSlot.Head, equippedHeadSlot },
                { EquipmentSlot.Chest, equippedChestSlot },
                { EquipmentSlot.Legs, equippedLegsSlot },
                { EquipmentSlot.Waist, equippedWaistSlot },
                { EquipmentSlot.Weapon, equippedWeaponSlot },
                { EquipmentSlot.Shield, equippedShieldSlot },
                { EquipmentSlot.Boots, equippedBootsSlot },
                { EquipmentSlot.Ring, equippedRingSlot },
                { EquipmentSlot.Amulet, equippedAmuletSlot },
                { EquipmentSlot.Hands, equippedHandsSlot },
            };

            equipmentButtonMapping = new Dictionary<EquipmentSlot, List<GameObject>>();
        }

        private void CreateItemButton(Equipment equipment, Transform parentSlot, bool isEquipped)
        {
            if (equipment == null)
            {
                Debug.LogWarning("Cannot create button for null equipment.");
                return;
            }
            GameObject buttonObj = new GameObject($"{equipment.itemName}_{Guid.NewGuid()}");
            buttonObj.transform.SetParent(parentSlot, false);

            var slot = equipment.slot;
            if (!equipmentButtonMapping.ContainsKey(slot))
            {
                equipmentButtonMapping[slot] = new List<GameObject>();
            }
            equipmentButtonMapping[slot].Add(buttonObj);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(1.65f, 1.65f); // permanent size

            // Create the background layer
            GameObject background = new GameObject("Border");
            background.transform.SetParent(buttonObj.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(1.75f, 1.75f); // permanent size
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.green; // Highlight color

            // Create the icon mask layer
            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(buttonObj.transform, false);
            RectTransform maskRect = mask.AddComponent<RectTransform>();
            maskRect.sizeDelta = new Vector2(1.65f, 1.65f); // permanent size
            Image maskImage = mask.AddComponent<Image>();
            maskImage.color = Color.black; // black

            // Create the icon layer
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(buttonObj.transform, false);
            RectTransform iconRect = icon.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(1.65f, 1.65f); // permanent size
            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = equipment.baseSprite;
            iconImage.preserveAspect = true;

            Button buttonComponent = buttonObj.AddComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                OnItemButtonClicked(equipment, buttonObj, isEquipped);
            });
            mask.SetActive(false);
            background.SetActive(false);
        }

        #endregion

        #region Selection Handling
        private void OnItemButtonClicked(
            Equipment equipment,
            GameObject buttonObj,
            bool isEquipped = false
        )
        {
            // 1) Deselect previous item (no matter what)
            DeselectCurrentlySelectedItems();

            // 2) Mark new item as selected
            if (isEquipped)
            {
                selectedEquippedItem = equipment;
                selectedEquippedButton = buttonObj;
            }
            else
            {
                selectedEquippableItem = equipment;
                selectedEquippableButton = buttonObj;
            }

            // 3) Highlight the button
            ShowButtonHighlight(buttonObj, true);

            // 4) Display item stats
            DisplayItemStats(equipment, false);
        }

        /// <summary>
        /// Ensures any previously selected item (in either panel) is unselected and un-highlighted.
        /// </summary>
        private void DeselectCurrentlySelectedItems()
        {
            if (selectedEquippableButton != null)
            {
                ShowButtonHighlight(selectedEquippableButton, false);
                selectedEquippableButton = null;
            }
            if (selectedEquippedButton != null)
            {
                ShowButtonHighlight(selectedEquippedButton, false);
                selectedEquippedButton = null;
            }
            selectedEquippableItem = null;
            selectedEquippedItem = null;
            DisplayItemStats(null, true);
        }

        private void ShowButtonHighlight(GameObject buttonObj, bool show)
        {
            if (buttonObj == null)
                return;
            Transform border = buttonObj.transform.Find("Border");
            Transform mask = buttonObj.transform.Find("Mask");
            if (border != null)
                border.gameObject.SetActive(show);
            if (mask != null)
                mask.gameObject.SetActive(show);
        }
        #endregion

        #region (Un)Equip + Drop
        private void HandleEquipButton()
        {
            // ONLY do something if an unequipped equipment item is selected
            if (selectedEquippableItem == null)
            {
                Debug.LogWarning("No unequipped equipment selected to equip.");
                return;
            }

            EquipmentSlot slot = selectedEquippableItem.slot;
            Equipment currentlyEquipped = EquipmentManager.Instance.GetEquippedItem(slot);

            if (currentlyEquipped != null)
            {
                if (currentlyEquipped.itemName.Contains("Cursed"))
                {
                    Debug.LogWarning("Cannot replace a cursed item.");
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Cannot unequip a cursed item.",
                        PlayerStats.Instance.transform,
                        Color.red
                    );
                    return;
                }
                // Remove old from equip
                EquipmentManager.Instance.UnequipItem(currentlyEquipped);
                // Put old back in inventory
                EquipmentInventory.Instance.AddEquipment(currentlyEquipped);
            }

            // Now equip the new one
            EquipmentManager.Instance.EquipItem(selectedEquippableItem);
            EquipmentInventory.Instance.RemoveEquipment(selectedEquippableItem);

            DeselectCurrentlySelectedItems();
            PlayerUI.Instance.UpdateUIPanels();
            UpdateEquipmentUI();
        }

        private void HandleUnequipButton()
        {
            // ONLY do something if an equipped equipment item is selected
            if (selectedEquippedItem == null)
            {
                Debug.LogWarning("No equipped equipment selected to unequip.");
                return;
            }

            if (selectedEquippedItem.itemName.Contains("Cursed"))
            {
                Debug.LogWarning("Cannot unequip a cursed item.");
                FloatingTextManager.Instance.ShowFloatingText(
                    "Cannot unequip a cursed item.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
                return;
            }

            EquipmentManager.Instance.UnequipItem(selectedEquippedItem);
            EquipmentInventory.Instance.AddEquipment(selectedEquippedItem);

            DeselectCurrentlySelectedItems();
            PlayerUI.Instance.UpdateUIPanels();
            UpdateEquipmentUI();
        }

        private void HandleDropButton()
        {
            // 1) If we have an equipped or unequipped *equipment* selected, drop it
            if (selectedEquippedItem != null)
            {
                if (selectedEquippedItem.itemName.Contains("Cursed"))
                {
                    Debug.LogWarning("Cannot drop a cursed equipment item.");
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Cannot drop a cursed item.",
                        PlayerStats.Instance.transform,
                        Color.red
                    );
                    return;
                }

                // First unequip
                EquipmentManager.Instance.UnequipItem(selectedEquippedItem);

                // Then drop
                DropEquipment(selectedEquippedItem);

                // Done
                DeselectCurrentlySelectedItems();
                PlayerUI.Instance.UpdateUIPanels();
                UpdateEquipmentUI();
                return;
            }
            else if (selectedEquippableItem != null)
            {
                if (selectedEquippableItem.itemName.Contains("Cursed"))
                {
                    Debug.LogWarning("Cannot drop a cursed equipment item.");
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Cannot drop a cursed item.",
                        PlayerStats.Instance.transform,
                        Color.red
                    );
                    return;
                }
                // Remove from inventory
                EquipmentInventory.Instance.RemoveEquipment(selectedEquippableItem);

                // Drop
                DropEquipment(selectedEquippableItem);

                // Done
                DeselectCurrentlySelectedItems();
                PlayerUI.Instance.UpdateUIPanels();
                UpdateEquipmentUI();
                return;
            }

            // 2) If no equipment selected, check if a consumable is selected in the Consumable UI
            var selectedConsumable = ConsumableItemsUIManager.Instance.GetSelectedConsumable();
            if (selectedConsumable != null)
            {
                // (Optional) check if cursed
                if (selectedConsumable.name.Contains("Cursed"))
                {
                    Debug.LogWarning("Cannot drop a cursed consumable item.");
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Cannot drop a cursed item.",
                        PlayerStats.Instance.transform,
                        Color.red
                    );
                    return;
                }

                // Use the Consumable UI Manager's drop logic
                ConsumableItemsUIManager.Instance.DropConsumable(selectedConsumable);
                return;
            }

            // 3) If neither an equipment item nor a consumable is selected
            Debug.LogWarning("No item selected to drop.");
        }

        private void DropEquipment(Equipment equipment)
        {
            if (equipment == null)
                return;

            Vector3 dropPosition = PlayerStats.Instance.transform.position;
            GameObject dropObj = new GameObject($"DroppedItem_{equipment.itemName}");
            dropObj.transform.position = dropPosition;
            dropObj.transform.localScale = new Vector3(2f, 2f, 0f);

            dropObj.layer = LayerMask.NameToLayer("items");
            dropObj.tag = "Item";

            var sr = dropObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 3;
            sr.sprite = equipment.baseSprite; // show the equipment's sprite

            var col = dropObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            EquipmentPickup pickup = dropObj.AddComponent<EquipmentPickup>();
            pickup.SetEquipment(equipment);

            Debug.Log($"{equipment.itemName} dropped at {dropPosition}.");
        }
        #endregion

        #region Player Stats Display
        public void DisplayPlayerStats()
        {
            PlayerStats ps = PlayerStats.Instance;
            if (ps == null)
                return;

            playerAttackText.text = ps.GetCurrentAttack().ToString();
            playerDefenseText.text = ps.GetCurrentDefense().ToString();
            playerMagicText.text = ps.GetCurrentMagic().ToString();
            playerHealthText.text = ps.GetCurrentHealth().ToString();
            playerSpeedText.text = ps.GetCurrentSpeed().ToString();
            playerIntelligenceText.text = ps.GetCurrentIntelligence().ToString();
            playerDexterityText.text = ps.GetCurrentDexterity().ToString();
            playerStaminaText.text = ps.GetCurrentStamina().ToString();
            playerCritChanceText.text = ps.GetCurrentCritChance().ToString();
            playerBurnDamageText.text = ps.GetCurrentBurnDamage().ToString();
            playerIceDamageText.text = ps.GetCurrentIceDamage().ToString();
            playerLightningDamageText.text = ps.GetCurrentLightningDamage().ToString();
            playerPoisonDamageText.text = ps.GetCurrentPoisonDamage().ToString();

            // Resistances / Weakness / Status
            playerResistanceText.text = ps.GetResistances();
            playerWeaknessText.text = ps.GetWeaknesses();
            playerActiveStatusText.text = string.Join(", ", ps.activeStatusEffects);
            playerInflictedStatusText.text = string.Join(", ", ps.inflictableStatusEffects);
        }
        #endregion

        #region Item Stats Display
        public void DisplayItemStats(Equipment equipment, bool clear = false)
        {
            if (clear || equipment == null)
            {
                equipmentNameText.text = "No Item";
                attackModifierText.text = "0";
                defenseModifierText.text = "0";
                magicModifierText.text = "0";
                healthModifierText.text = "0";
                speedModifierText.text = "0";
                intelligenceModifierText.text = "0";
                dexterityModifierText.text = "0";
                staminaModifierText.text = "0";
                critChanceModifierText.text = "0";
                burnDamageModifierText.text = "0";
                iceDamageModifierText.text = "0";
                lightningDamageModifierText.text = "0";
                poisonDamageModifierText.text = "0";
                activeStatusEffectsText.text = "None";
                inflictedStatusEffectsText.text = "None";
                resistanceEffectsText.text = "None";
                weaknessEffectsText.text = "None";
                return;
            }

            equipmentNameText.text = equipment.itemName;
            attackModifierText.text = $"+ {equipment.equipmentStats[Stat.Attack]}";
            defenseModifierText.text = $"+ {equipment.equipmentStats[Stat.Defense]}";
            magicModifierText.text = $"+ {equipment.equipmentStats[Stat.MaxMagic]}";
            healthModifierText.text = $"+ {equipment.equipmentStats[Stat.MaxHP]}";
            speedModifierText.text = $"+ {equipment.equipmentStats[Stat.Speed]}";
            intelligenceModifierText.text = $"+ {equipment.equipmentStats[Stat.Intelligence]}";
            dexterityModifierText.text = $"+ {equipment.equipmentStats[Stat.Dexterity]}";
            staminaModifierText.text = $"+ {equipment.equipmentStats[Stat.MaxStamina]}";
            critChanceModifierText.text = $"+ {equipment.equipmentStats[Stat.CritChance]}";
            burnDamageModifierText.text =
                $"+ {equipment.damageModifiers.GetValueOrDefault(DamageType.Fire, 0)}";
            iceDamageModifierText.text =
                $"+ {equipment.damageModifiers.GetValueOrDefault(DamageType.Ice, 0)}";
            lightningDamageModifierText.text =
                $"+ {equipment.damageModifiers.GetValueOrDefault(DamageType.Lightning, 0)}";
            poisonDamageModifierText.text =
                $"+ {equipment.damageModifiers.GetValueOrDefault(DamageType.Poison, 0)}";
            activeStatusEffectsText.text = string.Join(", ", equipment.activeStatusEffects);
            inflictedStatusEffectsText.text = string.Join(", ", equipment.inflictedStatusEffects);
            resistanceEffectsText.text = string.Join(", ", equipment.resistanceEffects);
            weaknessEffectsText.text = string.Join(", ", equipment.weaknessEffects);
        }
        #endregion
    }
}
