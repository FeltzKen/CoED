using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

namespace CoED
{
    public class DungeonSpawner : MonoBehaviour
    {
        public static DungeonSpawner Instance { get; private set; }

        [Header("References")]
        [SerializeField]
        private GameObject player;
        public DungeonSettings dungeonSettings;

        private Rigidbody2D playerRB;
        private Rigidbody2D enemyRB;

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
            playerRB = player.GetComponent<Rigidbody2D>();
            dungeonSettings = FindAnyObjectByType<DungeonGenerator>()
                ?.GetComponent<DungeonGenerator>()
                .dungeonSettings;
        }

        public void Start()
        {
            // Start by placing the player in the spawning room
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                // Set player position in the spawning room
                player.transform.position =
                    DungeonManager.Instance.SpawningRoomInstance.transform.position
                    + Vector3.up * 2; // Adjust if needed
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
                    Vector2Int randomTile = firstFloorData
                        .FloorTiles.OrderBy(t => UnityEngine.Random.value)
                        .First();
                    Vector3 exitPosition = new Vector3(randomTile.x, randomTile.y, 0);

                    // Move the player to the selected position
                    player.position = exitPosition;
                    playerRB.position = exitPosition;
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
                Transform floorParent = DungeonManager.Instance.FloorTransforms[
                    floorData.FloorNumber
                ];
                Transform enemyParent = floorParent.Find("EnemyParent");

                if (enemyParent == null)
                {
                    Debug.LogError(
                        $"EnemyParent not found for Floor {floorData.FloorNumber}. Skipping enemy spawning."
                    );
                    continue;
                }

                StartCoroutine(SpawnEnemiesForFloor(floorData, floorParent, enemyParent));
            }
        }

        private IEnumerator SpawnEnemiesForFloor(
            FloorData floorData,
            Transform floorParent,
            Transform enemyParent
        )
        {
            int enemyCount = dungeonSettings.numberOfEnemiesPerFloor;
            List<Vector3> spawnPositions = floorData
                .GetRandomFloorTiles(enemyCount)
                .Select(tile => new Vector3(tile.x, tile.y, 0))
                .ToList();

            if (spawnPositions == null || spawnPositions.Count == 0)
            {
                Debug.LogWarning(
                    $"No valid spawn positions found for Floor {floorData.FloorNumber}. Skipping enemy spawning."
                );
                yield break;
            }

            Vector3 floorParentPosition = floorParent.position;

            foreach (var position in spawnPositions)
            {
                Vector3 snappedPosition = new Vector3(
                    Mathf.Round(position.x + 0.5f),
                    Mathf.Round(position.y + 0.5f),
                    position.z
                );
                Vector3 worldPosition = floorParentPosition + snappedPosition;

                //yield return PlaySpawnEffect(worldPosition);

                GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                    UnityEngine.Random.Range(0, dungeonSettings.enemyPrefabs.Count)
                ];
                GameObject enemy = Instantiate(
                    enemyPrefab,
                    worldPosition,
                    Quaternion.identity,
                    enemyParent
                );

                if (enemy == null)
                {
                    Debug.LogError(
                        $"Failed to instantiate enemy prefab at position {worldPosition}."
                    );
                    continue;
                }

                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Initialize(floorData);
                    enemyAI.SetPatrolPoints(
                        floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                    );
                    enemyAI.SpawningFloor = floorData.FloorNumber;
                    // enemyAI.walkableTiles = floorData.
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
        // spawn items for all floors
        public void SpawnItemsForAllFloors()
        {
            // Debug.Log("Starting item spawning for all floors...");


            if (DungeonManager.Instance == null)
            {
                Debug.LogError("DungeonManager is not initialized. Cannot spawn items.");
                return;
            }

            foreach (var floorEntry in DungeonManager.Instance.floors)
            {
                FloorData floorData = floorEntry.Value;
                Transform floorParent = DungeonManager.Instance.FloorTransforms[
                    floorData.FloorNumber
                ];
                //creating a parent for the items
                Transform itemParent = floorParent.Find("ItemParent");

                if (itemParent == null)
                {
                    Debug.LogError(
                        $"ItemParent not found for Floor {floorData.FloorNumber}. Skipping item spawning."
                    );
                    continue;
                }
                StartCoroutine(SpawnItemsForFloor(floorData, floorParent, itemParent));
            }
        }

        private IEnumerator SpawnItemsForFloor(
            FloorData floorData,
            Transform floorParent,
            Transform itemParent
        )
        {
            int itemCount = dungeonSettings.numberOfItemsPerFloor;
            List<Vector3> spawnPositions = floorData
                .GetRandomFloorTiles(itemCount)
                .Select(tile => new Vector3(tile.x, tile.y, 0))
                .ToList();

            if (spawnPositions == null || spawnPositions.Count == 0)
            {
                Debug.LogWarning(
                    $"No valid spawn positions found for Floor {floorData.FloorNumber}. Skipping item spawning."
                );
                yield break;
            }

            Vector3 floorParentPosition = floorParent.position;

            foreach (var position in spawnPositions)
            {
                Vector3 snappedPosition = new Vector3(
                    Mathf.Round(position.x + 0.5f),
                    Mathf.Round(position.y + 0.5f),
                    position.z
                );
                Vector3 worldPosition = floorParentPosition + snappedPosition;

                // Instantiate the item prefab at the calculated world position
                Item itemScriptableObject = (Item)
                    dungeonSettings.itemPrefabs[
                        UnityEngine.Random.Range(0, dungeonSettings.itemPrefabs.Count)
                    ];
                if (itemScriptableObject == null || itemScriptableObject.itemPrefab == null)
                {
                    Debug.LogError(
                        $"Item prefab is null. Skipping item spawning at position {worldPosition}."
                    );
                    continue;
                }

                GameObject itemInstance = Instantiate(
                    itemScriptableObject.itemPrefab,
                    worldPosition,
                    Quaternion.identity,
                    itemParent
                );
                if (itemInstance == null)
                {
                    Debug.LogError(
                        $"Failed to instantiate item prefab at position {worldPosition}."
                    );
                    continue;
                }

                // Verify the parent and position
                if (itemInstance.transform.parent != itemParent)
                {
                    Debug.LogError(
                        $"Item '{itemInstance.name}' is not parented correctly. Expected parent: {itemParent.name}, Actual parent: {itemInstance.transform.parent.name}"
                    );
                }
                else
                {
                    //       Debug.Log($"Item '{itemInstance.name}' successfully parented to '{itemParent.name}' at position {worldPosition}.");
                }

                //  Debug.Log($"Spawned item '{itemInstance.name}' at position {worldPosition} on Floor {floorData.FloorNumber}.");
            }
        }
        #endregion

        #region SpawnAmbush

        public void SpawnAmbush(Vector3 center, FloorData floorData, Transform enemyParent)
        {
            if (floorData == null)
            {
                Debug.LogError("DungeonSpawner: FloorData is null. Cannot spawn ambush.");
                return;
            }

            if (floorData.FloorTilemap == null)
            {
                Debug.LogError("DungeonSpawner: FloorTilemap is null. Cannot spawn ambush.");
                return;
            }

            if (floorData.FloorTiles == null)
            {
                Debug.LogError("DungeonSpawner: FloorTiles is null. Cannot spawn ambush.");
                return;
            }
            List<Vector2Int> spawnPositions = GetValidSpawnPositions(
                dungeonSettings.ambushEnemyCount,
                dungeonSettings.ambushRadius,
                center,
                floorData
            );

            if (spawnPositions.Count == 0)
            {
                Debug.LogError(
                    "DungeonSpawner: No valid spawn positions found for ambush. Cannot spawn enemies."
                );
                return;
            }

            foreach (var tilePosition in spawnPositions)
            {
                Vector3 worldPosition = floorData.FloorTilemap.CellToWorld(
                    new Vector3Int(tilePosition.x, tilePosition.y, 0)
                ); // + new Vector3(0.5f, 0.5f, 0);
                Instantiate(dungeonSettings.spawnEffectPrefab, worldPosition, Quaternion.identity);
                SpawnEnemy(worldPosition, enemyParent, floorData);
            }
        }

        private List<Vector2Int> GetValidSpawnPositions(
            int count,
            float radius,
            Vector3 center,
            FloorData floorData
        )
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            int attempts = 0;
            int maxAttempts = count * 10; // Limit the number of attempts to avoid infinite loops

            while (positions.Count < count && attempts < maxAttempts)
            {
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * radius;
                Vector3 randomPosition =
                    center + new Vector3(randomDirection.x, randomDirection.y, 0);
                Vector3Int cellPosition = floorData.FloorTilemap.WorldToCell(
                    new Vector3(Mathf.Round(randomPosition.x), Mathf.Round(randomPosition.y), 0)
                );
                Vector2Int tilePosition = new Vector2Int(cellPosition.x, cellPosition.y);

                if (
                    floorData.FloorTiles.Contains(tilePosition) && !positions.Contains(tilePosition)
                )
                {
                    positions.Add(tilePosition);
                }

                attempts++;
            }

            return positions;
        }

        public void SpawnBossOnFloor(int floorNumber)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError("DungeonSpawner: FloorData is null. Cannot spawn boss.");
                return;
            }

            Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNumber);
            Transform enemyParent = floorParent.Find("EnemyParent");

            if (enemyParent == null)
            {
                Debug.LogError("DungeonSpawner: EnemyParent not found. Cannot spawn boss.");
                return;
            }

            SpawnBoss(
                dungeonSettings.bossPrefabs[
                    UnityEngine.Random.Range(0, dungeonSettings.bossPrefabs.Count)
                ],
                floorData,
                enemyParent
            );
        }

        private void SpawnBoss(GameObject bossPrefab, FloorData floorData, Transform enemyParent)
        {
            if (bossPrefab == null)
            {
                Debug.LogError("DungeonSpawner: No boss prefab available. Cannot spawn boss.");
                return;
            }

            List<Vector2Int> spawnPositions = floorData.GetRandomFloorTiles(1);

            if (spawnPositions.Count == 0)
            {
                Debug.LogError(
                    "DungeonSpawner: No valid spawn positions found for boss. Cannot spawn boss."
                );
                return;
            }

            Vector3 position = new Vector3(spawnPositions[0].x, spawnPositions[0].y, 0);
            Vector3 worldPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
            Instantiate(dungeonSettings.spawnEffectPrefab, worldPosition, Quaternion.identity);
            if (bossPrefab == null)
            {
                Debug.LogError("DungeonSpawner: No enemy prefab available. Cannot spawn enemy.");
                return;
            }

            GameObject enemy = Instantiate(bossPrefab, position, Quaternion.identity, enemyParent);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Initialize(floorData);
                enemyAI.SetPatrolPoints(
                    floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                );
            }
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
                enemyAI.SetPatrolPoints(
                    floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                );
            }
        }
        #endregion
    }
}
