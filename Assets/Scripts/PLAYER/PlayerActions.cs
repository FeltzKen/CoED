using System.Collections;
using UnityEngine;

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

        private void Update()
        {
            HandleInput();
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
                // playerManager.CommitSpecialAction(CollectItems);
                enemy.ResetEnemyAttackFlags();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SearchForSecrets();
                enemy.ResetEnemyAttackFlags();
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

        public void CollectItem(ItemCollectible itemCollectible)
        {
            if (itemCollectible.ConsumeItem is Consumable consumable)
            {
                if (consumableInventory.AddItem(consumable))
                {
                    Destroy(itemCollectible.gameObject);
                    FloatingTextManager.Instance.ShowFloatingText(
                        $"Collected {consumable.ItemName}",
                        transform,
                        Color.green
                    );
                }
                else
                {
                    FloatingTextManager.Instance.ShowFloatingText(
                        "Inventory is full",
                        transform,
                        Color.red
                    );
                }
            }
            else
            {
                Debug.LogWarning("ItemCollectible: Consumable not found.");
            }
        }

        public void CollectItem(EquipmentWrapper equipment)
        {
            if (equipmentInventory.AddEquipment(equipment.equipmentData))
            {
                EquippableItemsUIManager.Instance.AddEquipmentToUI(equipment.equipmentData);
                Destroy(equipment.gameObject);
                FloatingTextManager.Instance.ShowFloatingText(
                    $"Collected {equipment.equipmentData.equipmentName}",
                    transform,
                    Color.green
                );
            }
            else
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "Inventory is full",
                    transform,
                    Color.red
                );
            }
        }

        public void CollectItems()
        {
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, 0.2f);

            foreach (var itemCollider in items)
            {
                if (itemCollider.CompareTag("Item"))
                {
                    Money money = itemCollider.GetComponent<Money>();
                    if (money != null)
                    {
                        money.Collect();
                    }

                    ItemCollectible collectible = itemCollider.GetComponent<ItemCollectible>();
                    if (collectible != null)
                    {
                        consumableInventory.AddItem(collectible.ConsumeItem);
                        Destroy(itemCollider.gameObject);
                    }
                    if (collectible == null)
                    {
                        equipmentInventory = itemCollider.GetComponent<EquipmentInventory>();
                        Debug.LogWarning("ItemCollectible or Money component not found.");
                        FloatingTextManager.Instance.ShowFloatingText(
                            $"Collected {itemCollider.name}",
                            transform,
                            Color.green
                        );
                    }
                }
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
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, searchRadius, searchableLayer);

            if (hits.Length == 0)
            {
                return;
            }

            foreach (var hit in hits)
            {
                Debug.DrawLine(center, hit.transform.position, Color.cyan, 1f);
                var searchable = hit.GetComponent<ISearchable>();
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
