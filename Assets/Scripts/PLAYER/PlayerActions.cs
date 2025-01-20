using System.Collections;
using CoED.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerActions : MonoBehaviour
    {
        public static PlayerActions Instance { get; private set; }

        [Header("Resting Settings")]
        [SerializeField]
        private int restHealRate = 5;

        private GameObject inventoryUI;
        private bool isResting = false;
        private PlayerStats playerStats;
        public ConsumableInventory consumableInventory;
        public EquipmentInventory equipmentInventory;
        private Enemy enemy;

        [SerializeField]
        private int magicRefillRateWhenResting = 5;

        [SerializeField]
        private float dangerRadius = 5f;

        [Header("Search Settings")]
        private float searchRadius = 1.4f;

        [SerializeField]
        private LayerMask searchableLayer;

        private void Update()
        {
            HandleInput();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerActions instance already exists. Destroying duplicate.");
                return;
            }
            enemy = FindAnyObjectByType<Enemy>();
        }

        private void Start()
        {
            playerStats = PlayerStats.Instance;
            consumableInventory = ConsumableInventory.Instance;
            equipmentInventory = EquipmentInventory.Instance;
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (playerStats == null || consumableInventory == null)
            {
                Debug.LogError(
                    "PlayerActions: Missing required components (PlayerStats, Inventory). Disabling script."
                );
                enabled = false;
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                CollectItems();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                //ToggleItemPanel();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SearchForSecrets();
                PlayerCombat.Instance.ResetEnemyAttackFlags();
            }

            if (Input.GetKeyDown(KeyCode.R) && !isResting)
            {
                if (!IsDangerNearby())
                {
                    Vector3 restPosition = transform.position;
                    StartCoroutine(RestUntilHealed(restPosition));
                }
            }
            if (Input.GetKeyDown(KeyCode.M) && !isResting)
            {
                if (!IsDangerNearby())
                {
                    Vector3 restPosition = transform.position;
                    StartCoroutine(RestUntilMagic(restPosition));
                }
            }
        }

        public void CollectItems()
        {
            Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(transform.position, 1.4f);

            foreach (var itemCollider in itemsInRange)
            {
                // Ensure only items are processed
                if (!itemCollider.CompareTag("Item"))
                {
                    Debug.Log($"Skipping non-item collider: {itemCollider.name}");
                    continue;
                }

                // 1) Handle MoneyPickup first
                MoneyPickup money = itemCollider.GetComponent<MoneyPickup>();
                if (money != null)
                {
                    money.Collect();
                    ItemCollectionAnimator.Instance.AnimateItemCollection(
                        money.moneyData.Icon,
                        itemCollider.transform.position,
                        GameObject.Find("MoneyPanel").GetComponent<RectTransform>()
                    );
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Collected coins",
                        transform,
                        Color.yellow
                    );
                    Destroy(itemCollider.gameObject);
                    continue; // Skip further checks for this collider
                }

                // 2) Handle items with HiddenItemController (consumables and equipment)
                var hiddenItemController = itemCollider.GetComponent<HiddenItemController>();
                if (hiddenItemController != null)
                {
                    if (hiddenItemController.isHidden)
                    {
                        Debug.Log($"Item {itemCollider.name} is hidden. Skipping collection.");
                        continue;
                    }
                }

                // 3) Check for Consumable
                ConsumableItemWrapper consumable =
                    itemCollider.GetComponent<ConsumableItemWrapper>();
                if (consumable != null)
                {
                    if (consumableInventory.AddItem(consumable))
                    {
                        ItemCollectionAnimator.Instance.AnimateItemCollection(
                            consumable.Icon,
                            itemCollider.transform.position,
                            GameObject.Find("ShowHideConsumablePanel").GetComponent<RectTransform>()
                        );
                        FloatingTextManager.Instance.ShowFloatingText(
                            $"Collected {consumable.ItemName}",
                            transform,
                            Color.green
                        );
                        Destroy(itemCollider.gameObject);
                    }
                    else
                    {
                        FloatingTextManager.Instance.ShowFloatingText(
                            "Inventory is full",
                            transform,
                            Color.red
                        );
                    }
                    continue;
                }

                // 4) Check for EquipmentPickup
                EquipmentPickup pickup = itemCollider.GetComponent<EquipmentPickup>();
                if (pickup != null)
                {
                    // A) Get the actual data wrapper
                    Equipment eqData = pickup.GetEquipmentData();
                    if (eqData == null)
                    {
                        Debug.LogWarning(
                            $"EquipmentPickup on {itemCollider.name} has no data. Skipping."
                        );
                        continue;
                    }

                    // B) Try to add the *data* to the inventory
                    if (equipmentInventory.AddEquipment(eqData))
                    {
                        // C) Animate and notify
                        ItemCollectionAnimator.Instance.AnimateItemCollection(
                            eqData.baseSprite,
                            itemCollider.transform.position,
                            GameObject.Find("ShowHideEquipmentPanel").GetComponent<RectTransform>()
                        );
                        Debug.Log($"Collected {eqData.itemName} slot: {eqData.slot}");
                        FloatingTextManager.Instance.ShowFloatingText(
                            $"Collected {eqData.itemName}",
                            transform,
                            Color.cyan
                        );

                        // D) Destroy the in-world object
                        Destroy(itemCollider.gameObject);
                    }
                    else
                    {
                        FloatingTextManager.Instance.ShowFloatingText(
                            "Inventory is full",
                            transform,
                            Color.red
                        );
                    }
                    continue;
                }

                // If the item doesn't match any type
                Debug.LogWarning($"Unrecognized item type: {itemCollider.name}");
            }
        }

        private void SearchForSecrets()
        {
            FloatingTextManager.Instance.ShowFloatingText(
                "Searching for secrets...",
                transform,
                Color.yellow
            );
            Vector2 center = transform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                center,
                searchRadius,
                LayerMask.GetMask("items")
            );

            if (hits.Length == 0)
            {
                return;
            }

            foreach (var hit in hits)
            {
                Debug.DrawLine(center, hit.transform.position, Color.cyan, 1f);
                var searchable = hit.GetComponent<HiddenItemController>();
                if (searchable != null)
                {
                    searchable.OnSearch();
                }
            }
        }

        private IEnumerator RestUntilHealed(Vector3 restPosition)
        {
            isResting = true;
            while (
                playerStats.CurrentHealth < playerStats.MaxHealth
                && !IsDangerNearby()
                && Vector3.Distance(restPosition, transform.position) < 0.1f
            )
            {
                playerStats.Heal(restHealRate);
                FloatingTextManager.Instance.ShowFloatingText(
                    $"Healed {restHealRate} HP",
                    transform,
                    Color.green
                );
                yield return new WaitForSeconds(PlayerStats.Instance.restInterval);
            }
            if (Vector3.Distance(restPosition, transform.position) >= 0.1f)
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Resting interrupted",
                    transform,
                    Color.red
                );
            }
            isResting = false;
            FloatingTextManager.Instance.ShowFloatingText(
                "Resting completed",
                transform,
                Color.green
            );
        }

        private IEnumerator RestUntilMagic(Vector3 restPosition)
        {
            isResting = true;
            while (
                playerStats.CurrentMagic < playerStats.MaxMagic
                && !IsDangerNearby()
                && Vector3.Distance(restPosition, transform.position) < 0.1f
            )
            {
                PlayerStats.Instance.Heal(20);
                playerStats.RefillMagic(magicRefillRateWhenResting);
                FloatingTextManager.Instance.ShowFloatingText(
                    $"Regained {magicRefillRateWhenResting} MP",
                    transform,
                    Color.green
                );
                yield return new WaitForSeconds(PlayerStats.Instance.restInterval * 3);
            }

            isResting = false;
            FloatingTextManager.Instance.ShowFloatingText(
                "Resting completed",
                transform,
                Color.green
            );
        }

        private void ToggleInventoryUI()
        {
            if (inventoryUI != null)
            {
                inventoryUI.SetActive(!inventoryUI.activeSelf);
            }
            else
            {
                Debug.LogWarning("PlayerActions: InventoryUI is not assigned.");
            }
            Debug.Log("PlayerActions: Resting (heal) completed.");
        }

        private bool IsDangerNearby()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, dangerRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Cannot rest. Danger nearby!",
                        transform,
                        Color.red
                    );
                    return true;
                }
            }
            return false;
        }

        private void UseAbility()
        {
            // Debug.Log("PlayerActions: Using ability...");
            // have possible other script to use for player abilities.
        }
    }
}
