using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED
{
    public class DungeonSpawner : MonoBehaviour
    {
        public static DungeonSpawner Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject player;
       [SerializeField] private DungeonSettings dungeonSettings;
        private Rigidbody2D rb;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("DungeonSpawner instance already exists. Destroying duplicate.");
                return;
            }
                rb = player.GetComponent<Rigidbody2D>();

        }
public void Start()
{
    // Start by placing the player in the spawning room
    if (DungeonManager.Instance.SpawningRoomInstance != null)
    {
        // Set player position in the spawning room
        player.transform.position = DungeonManager.Instance.SpawningRoomInstance.transform.position + Vector3.up * 2; // Adjust if needed
        Debug.Log("Player placed in the spawning room.");
    }
    else
    {
        Debug.LogError("Spawning room instance not found. Player placement failed.");
    }
}
        #region Player Transport
        /// <summary>
        /// Transports the player to a random valid position on the specified floor.
        /// </summary>
public void TransportPlayerToDungeon(GameObject player)
        {
            Debug.Log("Transporting player to the dungeon...");

            // Destroy the spawning room if it exists
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
                Debug.Log("Spawning room destroyed.");
            }

            // Get the walkable tiles for the first floor and teleport the player
            if (DungeonManager.Instance.FloorTransforms.ContainsKey(1) && DungeonManager.Instance.FloorTransforms[1] != null)
            {
                DungeonManager.Instance.FloorTransforms[1].gameObject.SetActive(true);
            if (DungeonManager.Instance.floors.ContainsKey(1))
                {
                    FloorData firstFloorData = DungeonManager.Instance.floors[1];
                    if (firstFloorData.FloorTiles.Count > 0)
                    {
                        // Get a random walkable tile
                        Vector2Int randomTile = firstFloorData.FloorTiles.OrderBy(t => Random.value).First();
                        Vector3 exitPosition = new Vector3(randomTile.x, randomTile.y, 0);

                        // Move the player to the selected position
                        player.transform.position = exitPosition;
                        rb.position = exitPosition;
                        PlayerMovement.Instance.UpdateCurrentTilePosition(exitPosition);

                        // Update camera position
                        Camera.main.GetComponent<CameraController>().SetPlayerTransform(player.transform);

                        Debug.Log($"Player transported to position: {exitPosition}");
                    }
                }
                else
                {
                    Debug.LogError("First floor data not found. Cannot transport player.");
                }
            }
            else
            {
                Debug.LogError("First floor not found. Cannot transport player.");
            }
        }
        #endregion

        #region Enemy Spawning
        /// <summary>
        /// Spawns enemies for the specified floor.
        /// </summary>
        public void SpawnEnemiesOnFloor(FloorData floorData)
        {
            if (dungeonSettings.enemyPrefabs.Count == 0)
            {
                Debug.LogError("No enemy prefabs available in DungeonSettings.");
                return;
            }

            int enemyCount = Mathf.Clamp(
                floorData.FloorTiles.Count / 10, // Adjust count based on floor size
                dungeonSettings.numberOfEnemiesPerFloor,
                20
            );

            List<Vector2> spawnPositions = floorData.GetRandomFloorTiles(enemyCount).Select(t => (Vector2)t).ToList();

            foreach (Vector2 position in spawnPositions)
            {
                SpawnEnemy(position, floorData);
            }

            Debug.Log($"Spawned {enemyCount} enemies on Floor {floorData.FloorNumber}");
        }

        private void SpawnEnemy(Vector2 position, FloorData floorData)
        {
            GameObject enemyPrefab = dungeonSettings.enemyPrefabs[Random.Range(0, dungeonSettings.enemyPrefabs.Count)];
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

            // Initialize enemy stats or AI
            var enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.spawnFloor = floorData.FloorNumber;
            }

            var enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.SetWalkableTiles(floorData.FloorTiles);
            }
        }
        #endregion

        #region Item Spawning
        /// <summary>
        /// Spawns items on the specified floor.
        /// </summary>
        public void SpawnItemsOnFloor(FloorData floorData, int itemCount)
        {
            if (dungeonSettings.itemPrefabs == null || dungeonSettings.itemPrefabs.Count == 0)
            {
                Debug.LogError("No item prefabs available in DungeonSettings.");
                return;
            }

            List<Vector2> spawnPositions = floorData.GetRandomFloorTiles(itemCount).Select(t => (Vector2)t).ToList();

            foreach (Vector2 position in spawnPositions)
            {
                SpawnItem(position);
            }

            Debug.Log($"Spawned {itemCount} items on Floor {floorData.FloorNumber}");
        }
        private void SpawnItem(Vector2 position)
        {
            if (dungeonSettings.itemPrefabs == null || dungeonSettings.itemPrefabs.Count == 0)
            {
                Debug.LogError("No item prefabs available in DungeonSettings.");
                return;
            }

            // Select a random item prefab
            GameObject itemPrefab = dungeonSettings.itemPrefabs[Random.Range(0, dungeonSettings.itemPrefabs.Count)];

            // Instantiate the item
            GameObject item = Instantiate(itemPrefab, position, Quaternion.identity);

            var itemComponent = item.GetComponent<Item>();
            if (itemComponent != null)
            {
                itemComponent.InitializeItem();
            }
        }
        #endregion
    }
}
