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
       [SerializeField] private DungeonGenerator dungeonGenerator;
        private Rigidbody2D rb;
        private void Awake()
        {
                // Debug.Log("DungeonSpawner Awake called");

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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
        // Debug.Log("Player placed in the spawning room.");
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
            // Debug.Log("Transporting player to the dungeon...");

            // Destroy the spawning room if it exists
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
                // Debug.Log("Spawning room destroyed.");
            }

            // Get the walkable tiles for the first floor and teleport the player
            if (DungeonManager.Instance.FloorTransforms.ContainsKey(1) && DungeonManager.Instance.FloorTransforms[1] != null)
            {
            DungeonManager.Instance.ShowRenderersForFloor(1);
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

                        // Debug.Log($"Player transported to position: {exitPosition}");
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
public void SpawnEnemiesForAllFloors()
{
    // Debug.Log("Starting enemy spawning for all floors...");
    
    if (DungeonManager.Instance == null)
    {
        Debug.LogError("DungeonManager is not initialized. Cannot spawn enemies.");
        return;
    }

    foreach (var floorEntry in DungeonManager.Instance.floors)
    {
        FloorData floorData = floorEntry.Value;
        Transform floorParent = DungeonManager.Instance.FloorTransforms[floorData.FloorNumber];
        Transform enemyParent = floorParent.Find("EnemyParent");

        if (enemyParent == null)
        {
            Debug.LogError($"EnemyParent not found for Floor {floorData.FloorNumber}. Skipping enemy spawning.");
            continue;
        }

        StartCoroutine(SpawnEnemiesForFloor(floorData, floorParent, enemyParent));
    }
}

private IEnumerator SpawnEnemiesForFloor(FloorData floorData, Transform floorParent, Transform enemyParent)
{
    int enemyCount = dungeonSettings.numberOfEnemiesPerFloor ;
    List<Vector3> spawnPositions = floorData.GetRandomFloorTiles(enemyCount)
        .Select(tile => new Vector3(tile.x, tile.y, 0))
        .ToList();

    if (spawnPositions == null || spawnPositions.Count == 0)
    {
        Debug.LogWarning($"No valid spawn positions found for Floor {floorData.FloorNumber}. Skipping enemy spawning.");
        yield break;
    }

    Vector3 floorParentPosition = floorParent.position;

    foreach (var position in spawnPositions)
    {
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            position.z
        );
        Vector3 worldPosition = floorParentPosition + snappedPosition;

        //yield return PlaySpawnEffect(worldPosition);

        GameObject enemyPrefab = dungeonSettings.enemyPrefabs[Random.Range(0, dungeonSettings.enemyPrefabs.Count)];
        GameObject enemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity, enemyParent);

        if (enemy == null)
        {
            Debug.LogError($"Failed to instantiate enemy prefab at position {worldPosition}.");
            continue;
        }

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.SetWalkableTiles(floorData); 
            enemyAI.SetPatrolPoints(floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints));

        }
        var enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.spawnFloor = floorData.FloorNumber;
        }
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

            // Debug.Log($"Spawned {itemCount} items on Floor {floorData.FloorNumber}");
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
