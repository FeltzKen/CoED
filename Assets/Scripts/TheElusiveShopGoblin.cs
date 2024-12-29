using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class ShopGoblin : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField]
        private string[] introDialogue =
        {
            "Ah, greetings, traveler! I am the elusive Shop Goblin!",
            "I move often, and each time you see me, I may have new wares.",
            "Let's see if I have anything that might help you.",
        };
        private bool hasIntroduced = false;
        private int dialogueIndex = 0;

        [Header("Shop Settings")]
        [SerializeField]
        private List<ShopItem> inventory = new List<ShopItem>();

        [SerializeField]
        private GameObject shopUI;

        [SerializeField]
        private int debuffRemovalCost = 50;

        [SerializeField]
        private int curseRemovalCost = 100;

        [Header("Teleportation Settings")]
        [SerializeField]
        private float teleportCooldown = 60f;
        private bool playerVisited = false;
        private bool isInitialEncounter = true;

        [Header("References")]
        [SerializeField]
        private FogOfWar fogOfWar;

        [SerializeField]
        private FogOfWarManager fogOfWarManager;

        [SerializeField]
        private Inventory playerInventory;

        [SerializeField]
        private Transform playerTransform;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject teleportEffectPrefab;

        private void Start()
        {
            ValidateReferences();
            GenerateInitialInventory();
            if (isInitialEncounter)
            {
                //  SpawnNearPlayer();
                isInitialEncounter = false;
            }
            else
            {
                TeleportToFoggedLocation();
            }
        }

        private void Update()
        {
            HandleInteractionInput();
        }

        private void ValidateReferences()
        {
            if (fogOfWar == null)
            {
                fogOfWar = FindAnyObjectByType<FogOfWar>();
                if (fogOfWar == null)
                {
                    Debug.LogError("ShopGoblin: FogOfWar component not found in the scene.");
                }
            }
            if (playerInventory == null)
            {
                playerInventory = FindAnyObjectByType<Inventory>();
                if (playerInventory == null)
                {
                    Debug.LogError("ShopGoblin: Player Inventory not found in the scene.");
                }
            }
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
                else
                {
                    Debug.LogError(
                        "ShopGoblin: Player GameObject not found. Ensure the player is tagged as 'Player'."
                    );
                }
            }
        }

        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.T) && IsPlayerAdjacent())
            {
                if (!hasIntroduced)
                {
                    ShowIntroDialogue();
                }
                else
                {
                    OpenShop();
                }
            }
        }

        private bool IsPlayerAdjacent()
        {
            if (playerTransform == null)
                return false;
            Vector2 goblinPosition = transform.position;
            Vector2 playerPosition = playerTransform.position;
            float distance = Vector2.Distance(goblinPosition, playerPosition);
            return distance <= 1.5f;
        }

        private void ShowIntroDialogue()
        {
            if (dialogueIndex < introDialogue.Length)
            {
                // Debug.Log($"ShopGoblin: {introDialogue[dialogueIndex]}");
                dialogueIndex++;
            }
            else
            {
                hasIntroduced = true;
                dialogueIndex = 0;
                OpenShop();
            }
        }

        private void OpenShop()
        {
            if (shopUI != null)
            {
                shopUI.SetActive(true);
                // Debug.Log("ShopGoblin: Shop UI opened.");
            }
            else
            {
                Debug.LogWarning("ShopGoblin: Shop UI is not assigned.");
            }
        }

        public void CloseShop()
        {
            if (shopUI != null)
            {
                shopUI.SetActive(false);
                // Debug.Log("ShopGoblin: Shop UI closed.");
            }
            else
            {
                Debug.LogWarning("ShopGoblin: Shop UI is not assigned.");
            }
        }

        private void TeleportToFoggedLocation()
        {
            Vector2 randomFogLocation = GetRandomFoggedLocation();
            if (randomFogLocation != Vector2.zero)
            {
                if (teleportEffectPrefab != null)
                {
                    Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
                }
                transform.position = randomFogLocation;
                if (teleportEffectPrefab != null)
                {
                    Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
                }
                playerVisited = false;
                // Debug.Log("ShopGoblin: Teleported to a new location!");
            }
            else
            {
                Debug.LogWarning("ShopGoblin: No fogged locations available for teleportation.");
            }
        }

        private Vector2 GetRandomFoggedLocation()
        {
            List<Vector2> foggedLocations = new List<Vector2>();
            for (
                int x = (int)fogOfWar.transform.position.x - 50;
                x <= (int)fogOfWar.transform.position.x + 50;
                x++
            )
            {
                for (
                    int y = (int)fogOfWar.transform.position.y - 50;
                    y <= (int)fogOfWar.transform.position.y + 50;
                    y++
                )
                {
                    Vector2 position = new Vector2(x, y);
                    if (fogOfWarManager.IsTileHidden(Vector2Int.RoundToInt(position)))
                    {
                        foggedLocations.Add(position);
                    }
                }
            }
            if (foggedLocations.Count == 0)
            {
                return Vector2.zero;
            }
            int randomIndex = Random.Range(0, foggedLocations.Count);
            return foggedLocations[randomIndex];
        }

        private void SpawnNearPlayer()
        {
            if (playerTransform == null)
            {
                Debug.LogError("ShopGoblin: PlayerTransform is not assigned.");
                return;
            }
            Vector2 playerPosition = playerTransform.position;
            Vector2 spawnPosition = GetNearbyPosition(playerPosition, 3, 5);
            //transform.position = spawnPosition;
            // Debug.Log($"ShopGoblin: Spawned near player at {spawnPosition}.");
        }

        private void GenerateInitialInventory()
        {
            inventory.Clear();
            int itemsToAdd = Random.Range(3, 6);
            for (int i = 0; i < itemsToAdd; i++)
            {
                ShopItem newItem = GenerateRandomShopItem();
                inventory.Add(newItem);
            }
            // Debug.Log($"ShopGoblin: Generated {inventory.Count} initial items for sale.");
        }

        private ShopItem GenerateRandomShopItem()
        {
            bool isEnchanted = Random.value < 0.3f;
            int basePrice = Random.Range(10, 100);
            string itemResourcePath = isEnchanted ? "Items/EnchantedItem" : "Items/NormalItem";
            Item item = Resources.Load<Item>(itemResourcePath);

            GameObject itemPrefab = null;
            if (item != null)
            {
                // Assuming you have a prefab associated with the item, load it from resources
                itemPrefab = Resources.Load<GameObject>($"Prefabs/{item.ItemName}Prefab");
                if (itemPrefab == null)
                {
                    Debug.LogError(
                        $"ShopGoblin: Failed to load prefab for item '{item.ItemName}'."
                    );
                }
            }

            return new ShopItem
            {
                itemName = isEnchanted ? "Enchanted " + item?.ItemName : item?.ItemName,
                basePrice = basePrice,
                isEnchanted = isEnchanted,
                itemPrefab =
                    itemPrefab // Assign the loaded prefab here
                ,
            };
        }

        private Vector2 GetNearbyPosition(Vector2 origin, float minDistance, float maxDistance)
        {
            float distance = Random.Range(minDistance, maxDistance);
            float angle = Random.Range(0f, 360f);
            Vector2 offset = distance * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return origin + offset;
        }

        public void Interact()
        {
            HandleInteractionInput();
        }
    }

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public int basePrice;
        public bool isEnchanted;
        public GameObject itemPrefab;

        public int GetPrice() => isEnchanted ? basePrice * 3 : basePrice;
    }
}
