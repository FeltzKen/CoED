using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace CoED
{
    public class ConsumableItemsUIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject consumableItemButtonPrefab;

        [SerializeField]
        private Transform consumableItemsPanel;

        private List<GameObject> consumableItemButtons = new List<GameObject>();

        private void Start()
        {
            if (Inventory.Instance == null)
            {
                Debug.LogError("ConsumableItemsUIManager: Inventory instance is null.");
                return;
            }

            if (consumableItemButtonPrefab == null)
            {
                Debug.LogError("ConsumableItemsUIManager: consumableItemButtonPrefab is not assigned.");
                return;
            }

            if (consumableItemsPanel == null)
            {
                Debug.LogError("ConsumableItemsUIManager: consumableItemsPanel is not assigned.");
                return;
            }

            Inventory.Instance.OnInventoryChanged += UpdateConsumableItemsUI;
            UpdateConsumableItemsUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) // Change 'C' to any key you prefer
            {
                ConsumeFirstItem();
            }
        }

        public void UpdateConsumableItemsUI()
        {
            Debug.Log("ConsumableItemsUIManager: Updating consumable items UI.");

            // Clear existing buttons
            foreach (var button in consumableItemButtons)
            {
                Destroy(button);
            }
            consumableItemButtons.Clear();
            Debug.Log($"Total number of items in the inventory: {Inventory.Instance.GetAllItems().Count}");
            Debug.Log($"Total number of buttons in the consumableItemButtons: {consumableItemButtons.Count}");

            // Create new buttons for each consumable item
            foreach (var item in Inventory.Instance.GetAllItems())
            {
                Debug.Log($"Item in inventory: {item.ItemName}, Type: {item.GetType()}");

                if (item is Consumable consumable)
                {
                    Debug.Log($"ConsumableItemsUIManager: Creating button for {consumable.ItemName}.");
                    GameObject buttonInstance = Instantiate(consumableItemButtonPrefab, consumableItemsPanel);
                    TextMeshProUGUI buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
                    Button buttonComponent = buttonInstance.GetComponent<Button>();
                    Image buttonImage = buttonInstance.GetComponent<Image>();

                    if (buttonText != null)
                    {
                        buttonText.text = consumable.ItemName;
                    }
                    else
                    {
                        Debug.LogError("ConsumableItemsUIManager: TextMeshProUGUI component not found in button prefab.");
                    }

                    if (buttonImage != null && consumable.Icon != null)
                    {
                        buttonImage.sprite = consumable.Icon;
                    }
                    else
                    {
                        Debug.LogError("ConsumableItemsUIManager: Image component not found in button prefab or icon not assigned.");
                    }

                    if (buttonComponent != null)
                    {
                        EventTrigger trigger = buttonInstance.AddComponent<EventTrigger>();
                        EventTrigger.Entry entry = new EventTrigger.Entry();
                        entry.eventID = EventTriggerType.PointerClick;
                        entry.callback.AddListener((data) => { OnRightClick((PointerEventData)data, consumable); });
                        trigger.triggers.Add(entry);
                    }
                    else
                    {
                        Debug.LogError("ConsumableItemsUIManager: Button component not found in button prefab.");
                    }

                    consumableItemButtons.Add(buttonInstance);
                    Debug.Log($"ConsumableItemsUIManager: Added button for {consumable.ItemName}");
                }
            }
        }

        private void OnRightClick(PointerEventData data, Consumable consumable)
        {
            if (data.button == PointerEventData.InputButton.Right)
            {
                ConsumeItem(consumable);
            }
        }

        private void ConsumeItem(Consumable consumable)
        {
            Debug.Log($"ConsumableItemsUIManager: Consuming item {consumable.ItemName}.");
            consumable.Consume(PlayerStats.Instance);
            Inventory.Instance.RemoveItem(consumable);
            UpdateConsumableItemsUI(); // Update UI after consuming the item
        }

        private void ConsumeFirstItem()
        {
            List<Item> items = Inventory.Instance.GetAllItems();
            if (items.Count > 0 && items[0] is Consumable consumable)
            {
                Debug.Log($"ConsumableItemsUIManager: Consuming first item {consumable.ItemName}.");
                ConsumeItem(consumable);
            }
            else
            {
                Debug.LogWarning("ConsumableItemsUIManager: No consumable items in the inventory.");
            }
        }
    }
}