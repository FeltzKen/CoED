using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class ItemSpawner : MonoBehaviour
    {
        public static ItemSpawner Instance { get; private set; }

        [Header("Item Spawning Settings")]
        [SerializeField]
        private GameObject itemPrefab; // Prefab with Item component attached

        [SerializeField]
        private int numberOfItems = 10;

        [SerializeField]
        private Vector2 spawnAreaMin = new Vector2(0, 0);

        [SerializeField]
        private Vector2 spawnAreaMax = new Vector2(100, 100);

        private List<Vector2> itemSpawnPositions = new List<Vector2>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("ItemSpawner instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            ValidateSettings();
            GenerateSpawnPositions();
            SpawnItems();
        }

        private void ValidateSettings()
        {
            if (itemPrefab == null)
            {
                Debug.LogError("ItemSpawner: Item prefab is not assigned.");
            }

            if (spawnAreaMax.x < spawnAreaMin.x || spawnAreaMax.y < spawnAreaMin.y)
            {
                Debug.LogError("ItemSpawner: Invalid spawn area boundaries.");
            }
        }

        private void GenerateSpawnPositions()
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Vector2 randomPosition = new Vector2(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );

                itemSpawnPositions.Add(randomPosition);
            }

            Debug.Log($"ItemSpawner: Generated {itemSpawnPositions.Count} spawn positions.");
        }

        private void SpawnItems()
        {
            foreach (Vector2 position in itemSpawnPositions)
            {
                GameObject itemObj = Instantiate(itemPrefab, position, Quaternion.identity);

                // Attempt to get the Item component and initialize it
                Item itemComponent = itemObj.GetComponent<Item>();
                if (itemComponent != null)
                {
                    itemComponent.InitializeItem();
                    Debug.Log(
                        $"ItemSpawner: Spawned item '{itemComponent.ItemName}' at {position} with Enchanted={itemComponent.IsEnchanted}, Cursed={itemComponent.IsCursed}"
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "ItemSpawner: Spawned object does not contain an Item component."
                    );
                }
            }

            Debug.Log("ItemSpawner: All items have been spawned.");
        }
    }
}
