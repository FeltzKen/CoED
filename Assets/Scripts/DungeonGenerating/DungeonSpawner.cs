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
        public DungeonSettings dungeonSettings;

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
                dungeonSettings = FindAnyObjectByType<DungeonGenerator>()?.GetComponent<DungeonGenerator>().dungeonSettings;
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
        public void TransportPlayerToDungeon(Transform player)
        {

            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
            }

            if (DungeonManager.Instance.FloorTransforms[1] != null)
            {

                FloorData firstFloorData = DungeonManager.Instance.floors[1];
                if (firstFloorData.FloorTiles.Count > 0)
                {
                    // Get a random walkable tile
                    Vector2Int randomTile = firstFloorData.FloorTiles.OrderBy(t => Random.value).First();
                    Vector3 exitPosition = new Vector3(randomTile.x, randomTile.y, 0);

                    // Move the player to the selected position
                    player.position = exitPosition;
                    rb.position = exitPosition;
                    PlayerMovement.Instance.UpdateCurrentTilePosition(exitPosition);

                }
            }
            else
            {
                Debug.LogError("First floor Transform not found. Cannot transport player.");
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
                    enemyAI.Initialize(floorData); 
                    enemyAI.SetPatrolPoints(floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints));
                    enemyAI.SpawningFloor = floorData.FloorNumber;

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

        #region SpawnAmbush

        public void SpawnAmbush(Vector3 location, int floorNumber)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError("DungeonSpawner: FloorData is null. Cannot spawn ambush.");
                return;
            }

            Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNumber);
            Transform enemyParent = floorParent.Find("EnemyParent");

            if (enemyParent == null)
            {
                Debug.LogError("DungeonSpawner: EnemyParent not found. Cannot spawn ambush.");
                return;
            }

            SpawnEnemiesForAmbush(dungeonSettings.ambushEnemyCount, dungeonSettings.ambushRadius, location, floorData, enemyParent);
        }

        private void SpawnEnemiesForAmbush(int numberOfEnemies, float radius, Vector3 location, FloorData floorData, Transform enemyParent)
        {
            List<Vector2Int> spawnPositions = GetRandomPositionsWithinRadius(numberOfEnemies, radius, location, floorData);

            foreach (Vector2Int position in spawnPositions)
            {
                Vector3 worldPosition = new Vector3(position.x - 0.5f, position.y - 0.5f, 0);
                Instantiate(dungeonSettings.spawnEffectPrefab, worldPosition, Quaternion.identity);
                SpawnEnemy(worldPosition, enemyParent, floorData);
            }
        }
 
        private List<Vector2Int> GetRandomPositionsWithinRadius(int count, float radius, Vector3 center, FloorData floorData)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for (int i = 0; i < count; i++)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(0, radius);
                Vector3 randomPosition = center + new Vector3(randomDirection.x, randomDirection.y, 0);
                Vector2Int tilePosition = Vector2Int.RoundToInt(randomPosition);

                if (floorData.FloorTilemap.HasTile(new Vector3Int(tilePosition.x, tilePosition.y, 0)))
                {
                    positions.Add(tilePosition);
                }
            }

            return positions;
        }

        private void SpawnEnemy(Vector3 position, Transform parent, FloorData floorData)
        {
            GameObject enemyPrefab = DungeonManager.Instance.GetRandomEnemyPrefab();
            if (enemyPrefab == null)
            {
                Debug.LogError("DungeonSpawner: No enemy prefab available. Cannot spawn enemy.");
                return;
            }

            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, parent);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Initialize(floorData);
                enemyAI.SetPatrolPoints(floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints));
            }
        }
        #endregion
    }
}
