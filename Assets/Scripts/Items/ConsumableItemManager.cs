using System; // for Guid
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoED
{
    public class ConsumableItemsUIManager : MonoBehaviour
    {
        public static ConsumableItemsUIManager Instance { get; private set; }

        [Header("Consumables Panel")]
        [SerializeField]
        private Transform consumableItemsPanel;

        [Header("Use Button")]
        [SerializeField]
        private Button useButton;

        // Mapping from each consumable -> the runtime button GameObject
        private Dictionary<ConsumableItem, GameObject> consumableToButtonMapping =
            new Dictionary<ConsumableItem, GameObject>();

        // Currently selected item & button
        private ConsumableItem selectedConsumable;
        private GameObject selectedConsumableButton;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (ConsumableInventory.Instance == null)
            {
                Debug.LogError("ConsumableItemsUIManager: ConsumableInventory.Instance is null.");
                return;
            }

            if (useButton == null)
            {
                Debug.LogError("Use Button is not assigned in the inspector!");
                return;
            }

            // Subscribe to inventory-changed event
            ConsumableInventory.Instance.OnInventoryChanged += UpdateConsumableItemsUI;

            // Hook up the Use button's onClick
            useButton.onClick.AddListener(OnUseButtonClicked);

            // Populate for the first time
            UpdateConsumableItemsUI();
        }

        #region Public Accessors
        public ConsumableItem GetSelectedConsumable()
        {
            return selectedConsumable;
        }

        /// <summary>
        /// Called by the Equipment UI when the "Drop" button is pressed
        /// and no equipment is selected (meaning we must be dropping a consumable).
        /// </summary>
        public void DropConsumable(ConsumableItem item)
        {
            if (item == null) return;

            // Optionally check if cursed here
            // if (item.name.Contains("Cursed")) {...}

            Vector3 dropPos = PlayerStats.Instance.transform.position;
            GameObject dropObj = new GameObject($"DroppedConsumable_{item.name}");
            dropObj.transform.position = dropPos;
            dropObj.transform.localScale = new Vector3(2f, 2f, 0f);

            dropObj.layer = LayerMask.NameToLayer("items");
            dropObj.tag = "Item";

            // Add a sprite renderer for the consumable icon
            var sr = dropObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 3;
            sr.sprite = item.icon; // show the consumable's sprite

            // Add a trigger collider
            var col = dropObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            // If you have a "ConsumablePickup" script:
                var pickup = dropObj.AddComponent<ConsumablePickup>();
                pickup.SetConsumable(item);

            // Remove from inventory
            ConsumableInventory.Instance.RemoveItem(item);

            Debug.Log($"{item.name} dropped at {dropPos}.");

            // Clear out the old selection
            DeselectOld();

            // Refresh UI
            UpdateConsumableItemsUI();
        }
        #endregion

        #region Building the UI
        public void UpdateConsumableItemsUI()
        {
            // Clear existing
            foreach (var pair in consumableToButtonMapping)
            {
                Destroy(pair.Value);
            }
            consumableToButtonMapping.Clear();

            // Get items
            var items = ConsumableInventory.Instance.GetAllItems();
            if (items == null || items.Count == 0)
            {
                Debug.Log("No consumable items in inventory.");
                return;
            }

            // Create a button for each
            foreach (var item in items)
            {
                CreateConsumableButton(item, consumableItemsPanel);
            }
        }

        private void CreateConsumableButton(ConsumableItem item, Transform parent)
        {
            if (item == null)
            {
                Debug.LogWarning("Cannot create button for null consumable.");
                return;
            }

            GameObject buttonObj = new GameObject($"{item.name}_{Guid.NewGuid()}");
            buttonObj.transform.SetParent(parent, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(1.65f, 1.65f);

            // Background "Border"
            GameObject background = new GameObject("Border");
            background.transform.SetParent(buttonObj.transform, false);
            var bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(1.75f, 1.75f);
            var bgImage = background.AddComponent<Image>();
            bgImage.color = Color.green; // highlight color, hidden by default

            // Mask
            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(buttonObj.transform, false);
            var maskRect = mask.AddComponent<RectTransform>();
            maskRect.sizeDelta = new Vector2(1.65f, 1.65f);
            var maskImg = mask.AddComponent<Image>();
            maskImg.color = Color.black;

            // Icon
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(buttonObj.transform, false);
            var iconRect = icon.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(1.65f, 1.65f);
            var iconImg = icon.AddComponent<Image>();
            if (item.icon != null)
            {
                iconImg.sprite = item.icon;
            }
            iconImg.preserveAspect = true;

            // Button
            Button buttonComponent = buttonObj.AddComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                OnConsumableButtonClicked(item, buttonObj);
            });

            // Hide highlight layers
            mask.SetActive(false);
            background.SetActive(false);

            // Track in dictionary
            consumableToButtonMapping[item] = buttonObj;
        }
        #endregion

        #region Selection & Highlight
        private void OnConsumableButtonClicked(ConsumableItem consumable, GameObject buttonObj)
        {
            DeselectOld();
            HighlightButtonFor(consumable);
        }

        private void DeselectOld()
        {
            if (selectedConsumableButton != null)
            {
                ShowButtonHighlight(selectedConsumableButton, false);
                selectedConsumableButton = null;
            }
            selectedConsumable = null;
        }

        private void HighlightButtonFor(ConsumableItem consumable)
        {
            if (consumable == null) return;

            if (!consumableToButtonMapping.ContainsKey(consumable))
            {
                Debug.LogWarning($"{consumable.name} not in mapping, can't highlight.");
                return;
            }

            GameObject buttonObj = consumableToButtonMapping[consumable];
            ShowButtonHighlight(buttonObj, true);

            selectedConsumable = consumable;
            selectedConsumableButton = buttonObj;
        }

        private void ShowButtonHighlight(GameObject buttonObj, bool show)
        {
            if (buttonObj == null) return;

            Transform border = buttonObj.transform.Find("Border");
            if (border != null)
                border.gameObject.SetActive(show);

            Transform mask = buttonObj.transform.Find("Mask");
            if (mask != null)
                mask.gameObject.SetActive(show);
        }
        #endregion

        #region Use Logic
        private void OnUseButtonClicked()
        {
            if (selectedConsumable == null)
            {
                Debug.LogWarning("No consumable item selected to use.");
                return;
            }

            // e.g., run the effect, heal, etc.
            selectedConsumable.Consume();

            // Remove from inventory
            ConsumableInventory.Instance.RemoveItem(selectedConsumable);

            // Clear selection & refresh
            DeselectOld();
            UpdateConsumableItemsUI();
        }
        #endregion
    }
}
