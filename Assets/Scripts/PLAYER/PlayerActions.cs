using System.Collections;
using UnityEngine;
using CoED;

namespace CoED
{
    // Handles player actions such as resting, collecting items, searching for secrets, and using abilities.
    public class PlayerActions : MonoBehaviour
    {
        public static PlayerActions Instance { get; private set; }
        [Header("Resting Settings")]
        [SerializeField]
        private int restHealRate = 5;

        [SerializeField]
        private int restMagicRate = 1;

        [SerializeField]
        private float restInterval = 1f;

        [SerializeField]
        private float dangerRadius = 5f;

        [Header("Inventory UI")]
        [SerializeField]
        private GameObject inventoryUI;

        private bool isResting = false;
        private PlayerStats playerStats;
        private PlayerMagic playerMagic;
        private Inventory playerInventory;
        private FloatingTextManager floatingTextManager;
        private PlayerManager playerManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerActions instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            playerStats = GetComponent<PlayerStats>();
            playerMagic = GetComponent<PlayerMagic>();
            playerInventory = GetComponent<Inventory>();
            floatingTextManager = FindAnyObjectByType<FloatingTextManager>();
            playerManager = PlayerManager.Instance;

            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (playerStats == null || playerMagic == null || playerInventory == null)
            {
                Debug.LogError("PlayerActions: Missing required components (PlayerStats, PlayerMagic, Inventory). Disabling script.");
                enabled = false;
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                playerManager.CommitSpecialAction(CollectItems);
                PlayerManager.Instance.ResetEnemyAttackFlags();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                playerManager.CommitSpecialAction(SearchForSecrets);
                PlayerManager.Instance.ResetEnemyAttackFlags();
            }
            else if (Input.GetKeyDown(KeyCode.R) && !isResting)
            {
                if (!IsDangerNearby())
                {
                    playerManager.CommitSpecialAction(() => StartCoroutine(RestUntilHealed()));
                    PlayerManager.Instance.ResetEnemyAttackFlags();
                }
                else
                {
                    ShowFloatingText("Cannot rest, danger is nearby");
                }
            }
            else if (Input.GetKeyDown(KeyCode.M) && !isResting)
            {
                if (!IsDangerNearby())
                {
                    playerManager.CommitSpecialAction(() => StartCoroutine(RestUntilMagic()));
                    PlayerManager.Instance.ResetEnemyAttackFlags();
                }
                else
                {
                    ShowFloatingText("Cannot rest, danger is nearby");
                }
            }
        }

        private void CollectItems()
        {
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, 1f);
            bool collectedAny = false;

            foreach (var itemCollider in items)
            {
                if (itemCollider.CompareTag("Item"))
                {
                    Money money = itemCollider.GetComponent<Money>();
                    if (money != null)
                    {
                        money.Collect();
                        collectedAny = true;
                    }

                    ItemCollectible collectible = itemCollider.GetComponent<ItemCollectible>();
                    if (collectible != null)
                    {
                        playerInventory.AddItem(collectible.Item);
                        Destroy(itemCollider.gameObject);
                        collectedAny = true;
                    }
                }
            }

            Debug.Log(collectedAny ? "PlayerActions: Collected items." : "PlayerActions: No items to collect nearby.");
        }

        private void SearchForSecrets()
        {
            Debug.Log("PlayerActions: Searching for hidden doors and traps...");
            Collider2D[] secrets = Physics2D.OverlapCircleAll(transform.position, 1f);
            bool foundAny = false;

            foreach (var secretCollider in secrets)
            {
                if (secretCollider.CompareTag("HiddenDoor") || secretCollider.CompareTag("Trap"))
                {
                    secretCollider.gameObject.SetActive(true);
                    foundAny = true;
                    Debug.Log($"PlayerActions: Revealed {secretCollider.tag}.");
                }
            }

            if (!foundAny)
            {
                Debug.Log("PlayerActions: No hidden objects found.");
            }
        }

        private IEnumerator RestUntilHealed()
        {
            isResting = true;
            ShowFloatingText("Resting and healing...");
            Debug.Log("PlayerActions: Starting to rest and heal...");

            while (playerStats.CurrentHealth < playerStats.MaxHealth && !IsDangerNearby())
            {
                playerStats.Heal(restHealRate);
                ShowFloatingText($"Healed {restHealRate} HP");
                yield return new WaitForSeconds(restInterval);
            }

            isResting = false;
            ShowFloatingText("Resting completed");
            Debug.Log("PlayerActions: Resting completed.");
        }

        private IEnumerator RestUntilMagic()
        {
            isResting = true;
            ShowFloatingText("Resting to regain magic...");
            Debug.Log("PlayerActions: Starting to rest and refill magic...");

            while (playerMagic.CurrentMagic < playerMagic.MaxMagic && !IsDangerNearby())
            {
                playerMagic.RefillMagic(restMagicRate);
                ShowFloatingText($"Regained {restMagicRate} MP");
                yield return new WaitForSeconds(restInterval);
            }

            isResting = false;
            ShowFloatingText("Magic refill completed");
            Debug.Log("PlayerActions: Magic refill completed.");
        }

        private void ToggleInventoryUI()
        {
            if (inventoryUI != null)
            {
                inventoryUI.SetActive(!inventoryUI.activeSelf);
                Debug.Log($"PlayerActions: Inventory UI {(inventoryUI.activeSelf ? "opened" : "closed")}");
            }
            else
            {
                Debug.LogWarning("PlayerActions: InventoryUI is not assigned.");
            }
        }

        private bool IsDangerNearby()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, dangerRadius);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    return true;
                }
            }
            return false;
        }

        private void UseAbility()
        {
            Debug.Log("PlayerActions: Using ability...");
            // have possible other script to use for player abilities.
        }

        private void ShowFloatingText(string message)
        {
            floatingTextManager?.ShowFloatingText(message, transform.position, Color.yellow);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, dangerRadius);
        }
    }

    public class ItemCollectible : MonoBehaviour
    {
        [SerializeField]
        private Item item;

        public Item Item => item;
    }
}

