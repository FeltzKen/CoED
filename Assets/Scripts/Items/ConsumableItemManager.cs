using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoED
{
    public class ConsumableItemsUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject consumableItemButtonPrefab;

        [SerializeField]
        private Transform consumableItemsPanel;

        [SerializeField]
        private GameObject inventoryPanel; // Reference to the nested panel

        private List<GameObject> consumableItemButtons = new List<GameObject>();

        private void Start()
        {
            if (ConsumableInventory.Instance == null)
            {
                Debug.LogError("ConsumableItemsUIManager: Inventory instance is null.");
                return;
            }

            if (
                consumableItemButtonPrefab == null
                || consumableItemsPanel == null
                || inventoryPanel == null
            )
            {
                Debug.LogError("ConsumableItemsUIManager: UI references are missing.");
                return;
            }

            TooltipManager.Instance.Initialize();
            ConsumableInventory.Instance.OnInventoryChanged += UpdateConsumableItemsUI;
            UpdateConsumableItemsUI();

            inventoryPanel.SetActive(false);
        }

        public void UpdateConsumableItemsUI()
        {
            // Log for debugging
            Debug.Log("Updating Consumable Items UI...");

            // Clear old buttons
            foreach (var button in consumableItemButtons)
            {
                Destroy(button);
            }
            consumableItemButtons.Clear();

            // Fetch current inventory items
            var currentItems = ConsumableInventory.Instance.GetAllItems();

            // Log the inventory count
            Debug.Log($"Inventory contains {currentItems.Count} items.");

            // Create new buttons for each item
            foreach (var item in currentItems)
            {
                var currentItem = item;

                GameObject buttonInstance = Instantiate(
                    consumableItemButtonPrefab,
                    consumableItemsPanel
                );

                // Set up button text and image
                TextMeshProUGUI buttonText = buttonInstance
                    .transform.Find("Name")
                    .GetComponent<TextMeshProUGUI>();
                buttonText.text = currentItem.ItemName;

                Image buttonImage = buttonInstance.transform.Find("Icon").GetComponent<Image>();
                buttonImage.sprite = currentItem.Icon;

                // Add event listeners
                AddEventListener(
                    buttonInstance,
                    EventTriggerType.PointerClick,
                    (eventData) => OnPointerClick((PointerEventData)eventData, currentItem)
                );
                AddEventListener(
                    buttonInstance,
                    EventTriggerType.PointerEnter,
                    (eventData) => OnTooltipShow(currentItem)
                );
                AddEventListener(
                    buttonInstance,
                    EventTriggerType.PointerExit,
                    (eventData) => OnTooltipHide()
                );

                // Add button to the list
                consumableItemButtons.Add(buttonInstance);
            }

            // Log the updated button count for verification
            Debug.Log($"UI updated: {consumableItemButtons.Count} buttons created.");
        }

        private void AddEventListener(
            GameObject target,
            EventTriggerType eventType,
            UnityEngine.Events.UnityAction<BaseEventData> action
        )
        {
            EventTrigger trigger =
                target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        private void OnPointerClick(PointerEventData data, ConsumableItemWrapper consumable)
        {
            if (data.button == PointerEventData.InputButton.Right)
            {
                consumable.Consume(GameObject.FindGameObjectWithTag("Player"));
                ConsumeItem(consumable);
            }
        }

        private void OnTooltipShow(ConsumableItemWrapper consumable)
        {
            Debug.Log($"[ConsumableItemsUIManager] Showing tooltip for {consumable.ItemName}");
            TooltipManager.Instance.ShowTooltip(consumable.GetDescription(), Input.mousePosition);
        }

        private void OnTooltipHide()
        {
            TooltipManager.Instance.HideTooltip();
            Debug.Log("[ConsumableItemsUIManager] Hiding tooltip.");
        }

        private void ConsumeItem(ConsumableItemWrapper consumable)
        {
            Debug.Log($"[ConsumableItemsUIManager] Consuming item {consumable.ItemName}");
            ConsumableInventory.Instance.RemoveItem(consumable);
            UpdateConsumableItemsUI(); // Refresh UI
        }
    }
}
