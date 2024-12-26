using System.Collections;
using CoED;
using UnityEditor.Rendering;
using UnityEngine;

namespace CoED
{
    /// <summary>
    /// Handles player actions such as resting, collecting items, searching for secrets, etc.
    /// </summary>
    public class PlayerActions : MonoBehaviour
    {
        public static PlayerActions Instance { get; private set; }

        [Header("Resting Settings")]
        [SerializeField]
        private int restHealRate = 5;

        private GameObject inventoryUI;
        private bool isResting = false;
        private PlayerStats playerStats;
        public Inventory playerInventory;
        private FloatingTextManager floatingTextManager;
        private Enemy enemy;

        [SerializeField]
        private int restMagicRate = 1;

        [SerializeField]
        private float restInterval = 1f;

        [SerializeField]
        private float dangerRadius = 5f;

        // ================== NEW FIELDS FOR SEARCHING ==================
        [Header("Search Settings")]
        [SerializeField]
        private float searchRadius = 5f;

        [Tooltip("Layer mask for objects that can be found. If empty, we'll check all collisions.")]
        [SerializeField]
        private LayerMask searchableLayer;

        // ================ End of NEW FIELDS ==========================

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
            playerInventory = Inventory.Instance;
            floatingTextManager = FindAnyObjectByType<FloatingTextManager>();

            ValidateComponents();
        }

        private void Update()
        {
            HandleInput();
        }

        private void ValidateComponents()
        {
            if (playerStats == null || playerInventory == null)
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

            // Press 'R' to rest for HP
            if (Input.GetKeyDown(KeyCode.R) && !isResting)
            {
                if (!IsDangerNearby())
                {
                    StartCoroutine(RestUntilHealed());
                }
                else
                {
                    Debug.Log("Cannot rest, danger is nearby");
                }
            }
        }

        /// <summary>
        /// Overlaps a small circle around the player to find objects that implement ISearchable.
        /// Calls OnSearch() on any discovered objects.
        /// </summary>
        public void CollectItem(ItemCollectible itemCollectible)
        {
            if (itemCollectible.Item is Consumable consumable)
            {
                if (playerInventory.AddItem(consumable))
                {
                    Destroy(itemCollectible.gameObject);
                    Debug.Log($"Collected item: {consumable.ItemName}");
                }
                else
                {
                    Debug.LogWarning("Inventory is full. Cannot collect item.");
                }
            }
            else
            {
                Debug.LogWarning("Collected item is not a consumable.");
            }
        }

        public void CollectItems()
        {
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, 1f);
            //bool collectedAny = false;

            foreach (var itemCollider in items)
            {
                if (itemCollider.CompareTag("Item"))
                {
                    Money money = itemCollider.GetComponent<Money>();
                    if (money != null)
                    {
                        money.Collect();
                        //collectedAny = true;
                    }

                    ItemCollectible collectible = itemCollider.GetComponent<ItemCollectible>();
                    if (collectible != null)
                    {
                        playerInventory.AddItem(collectible.Item);
                        Destroy(itemCollider.gameObject);
                        //collectedAny = true;
                    }
                }
            }

            // Debug.Log(collectedAny ? "PlayerActions: Collected items." : "PlayerActions: No items to collect nearby.");
        }

        private void SearchForSecrets()
        {
            Vector2 center = transform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, searchRadius, searchableLayer);

            if (hits.Length == 0)
            {
                Debug.Log("SearchForSecrets: Nothing was found.");
                return;
            }

            // Check each collider for an object implementing ISearchable
            bool foundSomething = false;
            foreach (var hit in hits)
            {
                var searchable = hit.GetComponent<ISearchable>();
                if (searchable != null)
                {
                    searchable.OnSearch();
                    foundSomething = true;
                }
            }

            if (!foundSomething)
            {
                Debug.Log("SearchForSecrets: No searchable objects found in range.");
            }
        }

        private IEnumerator RestUntilHealed()
        {
            isResting = true;
            ShowFloatingText("Resting and healing...");
            // Debug.Log("PlayerActions: Starting to rest and heal...");

            while (playerStats.CurrentHealth < playerStats.MaxHealth && !IsDangerNearby())
            {
                playerStats.Heal(restHealRate);
                ShowFloatingText($"Healed {restHealRate} HP");
                yield return new WaitForSeconds(restInterval);
            }

            isResting = false;
            ShowFloatingText("Resting completed");
            // Debug.Log("PlayerActions: Resting completed.");
        }

        private IEnumerator RestUntilMagic()
        {
            isResting = true;
            ShowFloatingText("Resting to regain magic...");
            // Debug.Log("PlayerActions: Starting to rest and refill magic...");

            while (playerStats.CurrentMagic < playerStats.MaxMagic && !IsDangerNearby())
            {
                playerStats.RefillMagic(restMagicRate);
                ShowFloatingText($"Regained {restMagicRate} MP");
                yield return new WaitForSeconds(restInterval);
            }

            isResting = false;
            ShowFloatingText("Magic refill completed");
            // Debug.Log("PlayerActions: Magic refill completed.");
        }

        private void ToggleInventoryUI()
        {
            if (inventoryUI != null)
            {
                inventoryUI.SetActive(!inventoryUI.activeSelf);
                // Debug.Log($"PlayerActions: Inventory UI {(inventoryUI.activeSelf ? "opened" : "closed")}");
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
                    return true;
                }
            }
            return false;
        }

        // Optional: for visual debug in Editor
        private void UseAbility()
        {
            // Debug.Log("PlayerActions: Using ability...");
            // have possible other script to use for player abilities.
        }

        private void ShowFloatingText(string message)
        {
            //  floatingTextManager?.ShowFloatingText(message, transform, Color.yellow);
        }

        private void OnDrawGizmosSelected()
        {
            // Show search radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, searchRadius);

            // Danger radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, dangerRadius);
        }
    }
}
