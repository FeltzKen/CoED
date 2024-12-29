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
            if (Inventory.Instance == null)
            {
                Debug.LogError("ConsumableItemsUIManager: Inventory instance is null.");
                return;
            }

            if (consumableItemButtonPrefab == null)
            {
                Debug.LogError(
                    "ConsumableItemsUIManager: consumableItemButtonPrefab is not assigned."
                );
                return;
            }

            if (consumableItemsPanel == null)
            {
                Debug.LogError("ConsumableItemsUIManager: consumableItemsPanel is not assigned.");
                return;
            }

            if (inventoryPanel == null)
            {
                Debug.LogError("ConsumableItemsUIManager: inventoryPanel is not assigned.");
                return;
            }

            Inventory.Instance.OnInventoryChanged += UpdateConsumableItemsUI;
            UpdateConsumableItemsUI();

            inventoryPanel.SetActive(false);
        }

        public void UpdateConsumableItemsUI()
        {
            Debug.Log("ConsumableItemsUIManager: Updating consumable items UI.");

            foreach (var button in consumableItemButtons)
            {
                Destroy(button);
            }
            consumableItemButtons.Clear();
            Debug.Log(
                $"Total number of items in the inventory: {Inventory.Instance.GetAllItems().Count}"
            );
            Debug.Log(
                $"Total number of buttons in the consumableItemButtons: {consumableItemButtons.Count}"
            );

            foreach (var item in Inventory.Instance.GetAllItems())
            {
                Debug.Log($"Item in inventory: {item.ItemName}, Type: {item.GetType()}");

                if (item is Consumable consumable)
                {
                    Debug.Log(
                        $"ConsumableItemsUIManager: Creating button for {consumable.ItemName}."
                    );
                    GameObject buttonInstance = Instantiate(
                        consumableItemButtonPrefab,
                        consumableItemsPanel
                    );
                    TextMeshProUGUI buttonText = buttonInstance
                        .transform.Find("Name")
                        .GetComponent<TextMeshProUGUI>();
                    Image buttonImage = buttonInstance.transform.Find("Icon").GetComponent<Image>();

                    if (buttonText != null)
                    {
                        buttonText.text = consumable.ItemName;
                    }
                    else
                    {
                        Debug.LogError(
                            "ConsumableItemsUIManager: TextMeshProUGUI component not found in button prefab."
                        );
                    }

                    if (buttonImage != null && consumable.Icon != null)
                    {
                        buttonImage.sprite = consumable.Icon;
                    }
                    else
                    {
                        Debug.LogError(
                            "ConsumableItemsUIManager: Image component not found in button prefab or icon not assigned."
                        );
                    }

                    EventTrigger trigger = buttonInstance.AddComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener(
                        (data) =>
                        {
                            OnPointerClick((PointerEventData)data, consumable);
                        }
                    );
                    trigger.triggers.Add(entry);

                    consumableItemButtons.Add(buttonInstance);
                    Debug.Log($"ConsumableItemsUIManager: Added button for {consumable.ItemName}");
                }
            }
        }

        private void OnPointerClick(PointerEventData data, Consumable consumable)
        {
            if (data.button == PointerEventData.InputButton.Left)
            {
                // Suppress left-clicks
                return;
            }

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

        public void ToggleInventoryPanel() //call back for button that controls the consumable UI menu
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}
