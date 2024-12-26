using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoED.Items; // Assuming HiddenItemController is in the CoED.Items namespace
using CoED.Pathfinding; // For Pathfinder, PathfindingGrid
using Unity.VisualScripting;
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

        [SerializeField]
        private LayerMask obstacleLayer;

        /// <summary>
        /// We will maintain a static counter for unique occupant IDs.
        /// Each newly spawned enemy increments this counter and stores its ID.
        /// </summary>
        private static int occupantIDCounter = 0;

        /// <summary>
        /// Caches a Pathfinder per floorNumber so we only build one per floor.
        /// </summary>
        private Dictionary<int, Pathfinder> floorPathfinders = new Dictionary<int, Pathfinder>();

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
                Debug.LogWarning("DungeonSpawner instance already exists. Destroying duplicate.");
                return;
            }

            // Get player reference components
            playerRB = player.GetComponent<Rigidbody2D>();

            // Get DungeonSettings (optional if you already have it set)
            dungeonSettings = FindAnyObjectByType<DungeonGenerator>()
                ?.GetComponent<DungeonGenerator>()
                .dungeonSettings;
        }

        #region Player Transport

        /// <summary>
        /// Transports the player to a random valid position on Floor 1 (or any floor you choose).
        /// </summary>
        public void TransportPlayerToDungeon(Transform player)
        {
            // Destroy any leftover spawning room
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
            }

            // Check if floor 1 exists
            if (!DungeonManager.Instance.FloorTransforms.ContainsKey(1))
            {
                Debug.LogError("First floor Transform not found. Cannot transport player.");
                return;
            }

            // Retrieve FloorData for floor 1
            FloorData firstFloorData = DungeonManager.Instance.GetFloorData(1);
            if (firstFloorData == null || firstFloorData.FloorTiles.Count == 0)
            {
                Debug.LogWarning("No floor tiles found for Floor 1. Cannot transport player.");
                return;
            }

            // 1) Get a random valid tile from FloorTiles (i.e., not in WallTiles or VoidTiles)
            Vector2Int randomTile = GetRandomValidTile(firstFloorData);

            // 2) Convert to actual world position via CellToWorld + offset
            Vector3Int cellPos = new Vector3Int(randomTile.x, randomTile.y, 0);
            Vector3 baseWorldPos = firstFloorData.FloorTilemap.CellToWorld(cellPos);
            Vector3 centeredWorldPos = baseWorldPos + new Vector3(0.5f, 0.5f, 0f);

            // 3) Move the player and its Rigidbody to that position
            player.position = centeredWorldPos;
            playerRB.position = centeredWorldPos;

            // 4) Update player's tile position if your system needs that
            PlayerMovement.Instance.UpdateCurrentTilePosition(centeredWorldPos);
        }

        /// <summary>
        /// Picks a random tile from FloorTiles that is NOT in WallTiles or VoidTiles.
        /// Repeats until it finds a valid one, or times out after a set number of tries.
        /// </summary>
        private Vector2Int GetRandomValidTile(FloorData floorData)
        {
            // Make a copy of FloorTiles in a list
            List<Vector2Int> floorTileList = floorData.FloorTiles.ToList();
            int maxAttempts = 100; // Just to avoid infinite loops

            while (maxAttempts > 0 && floorTileList.Count > 0)
            {
                // Pick a random tile from floorTileList
                Vector2Int candidate = floorTileList[Random.Range(0, floorTileList.Count)];

                // Check if it's actually valid
                if (
                    !floorData.WallTiles.Contains(candidate)
                    && !floorData.VoidTiles.Contains(candidate)
                )
                {
                    return candidate;
                }

                // Remove candidate to avoid picking it again
                floorTileList.Remove(candidate);
                maxAttempts--;
            }

            // Fallback: if we somehow run out of tiles, just return the first floor tile
            Debug.LogWarning(
                "GetRandomValidTile: No valid tile found, returning first floor tile as fallback."
            );
            return floorData.FloorTiles.First();
        }

        #endregion

        #region Enemy Spawning (Refactored)

        /// <summary>
        /// Spawns enemies for all floors in the dungeon,
        /// using EnemyBrain + EnemyNavigator + TileOccupancyManager approach.
        /// </summary>
        public void SpawnEnemiesForAllFloors()
        {
            if (DungeonManager.Instance == null)
            {
                Debug.LogError("DungeonManager is not initialized. Cannot spawn enemies.");
                return;
            }

            // Iterate over each floor in the DungeonManager
            foreach (var floorEntry in DungeonManager.Instance.floors)
            {
                FloorData floorData = floorEntry.Value;
                Transform floorParent = DungeonManager.Instance.GetFloorTransform(
                    floorData.FloorNumber
                );
                Transform enemyParent = floorParent?.Find("EnemyParent");

                if (enemyParent == null)
                {
                    Debug.LogError(
                        $"EnemyParent not found for Floor {floorData.FloorNumber}. Skipping enemy spawning."
                    );
                    continue;
                }

                // Start a coroutine to spawn a batch of enemies for this floor
                StartCoroutine(SpawnEnemiesForFloor(floorData, floorParent, enemyParent));
            }
        }

        /// <summary>
        /// Spawns a set of enemies on the given floor, at random valid positions.
        /// </summary>
        private IEnumerator SpawnEnemiesForFloor(
            FloorData floorData,
            Transform floorParent,
            Transform enemyParent
        )
        {
            // Build or retrieve a cached pathfinder for this floor
            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            int enemyCount = dungeonSettings.numberOfEnemiesPerFloor;

            // Pick random spawn positions
            List<Vector2Int> candidateTiles = floorData.GetRandomFloorTiles(enemyCount * 2);
            List<Vector2Int> validTiles = candidateTiles
                .Where(t => IsValidSpawnPosition(floorData, t))
                .Take(enemyCount)
                .ToList();

            if (validTiles.Count == 0)
            {
                Debug.LogWarning(
                    $"No valid spawn positions found for Floor {floorData.FloorNumber}. "
                        + "Skipping enemy spawning."
                );
                yield break;
            }

            Vector3 floorParentPosition = floorParent.position;

            foreach (var tile in validTiles)
            {
                Vector3Int cellPos = new Vector3Int(tile.x, tile.y, 0);
                Vector3 worldPos =
                    floorData.FloorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);

                // Optionally show spawn effect here (commented out for brevity)
                // yield return PlaySpawnEffect(worldPosition);

                // Pick a random enemy prefab
                GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                    UnityEngine.Random.Range(0, dungeonSettings.enemyPrefabs.Count)
                ];
                if (enemyPrefab == null)
                {
                    Debug.LogError($"Enemy prefab is null for floor {floorData.FloorNumber}.");
                    continue;
                }

                // Instantiate
                GameObject enemy = Instantiate(
                    enemyPrefab,
                    worldPos,
                    Quaternion.identity,
                    enemyParent
                );
                if (enemy == null)
                {
                    Debug.LogError($"Failed to instantiate enemy prefab at {worldPos}.");
                    continue;
                }

                // Set basic floor stats if needed
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.spawnFloor = floorData.FloorNumber;
                }

                // -------- NEW: Add / Initialize EnemyBrain & EnemyNavigator ----------

                // 1) Give each enemy a unique occupant ID
                int occupantID = occupantIDCounter++;

                // 2) Ensure the enemy has an EnemyNavigator. If your prefab
                //    doesn't already have one, we add it dynamically:
                EnemyNavigator navigator = enemy.GetComponent<EnemyNavigator>();
                if (navigator == null)
                {
                    navigator = enemy.GetComponent<EnemyNavigator>();
                }
                navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

                // 3) Ensure the enemy has an EnemyBrain
                EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
                if (brain == null)
                {
                    brain = enemy.GetComponent<EnemyBrain>();
                }
                // Example: set random patrol points
                // Use your existing "floorData.GetRandomFloorTiles(...)" approach:
                HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>(
                    floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                );
                brain.Initialize(floorData, patrolPoints);

                // --------------------------------------------------------------------

                yield return null; // (Optional) yield so large spawns don’t block the main thread
            }
        }

        /// <summary>
        /// Returns true if the tile is in floorTiles and not in wallTiles or voidTiles.
        /// </summary>
        public bool IsValidSpawnPosition(FloorData floorData, Vector2Int position)
        {
            return floorData.FloorTiles.Contains(position)
                && !floorData.WallTiles.Contains(position)
                && !floorData.VoidTiles.Contains(position);
        }

        /// <summary>
        /// Returns a Pathfinder for the given floorData.
        /// Caches it so we only build once per floor.
        /// </summary>
        private Pathfinder GetOrCreateFloorPathfinder(FloorData floorData)
        {
            int floorNum = floorData.FloorNumber;

            // If we already built a pathfinder for this floor, reuse it
            if (floorPathfinders.ContainsKey(floorNum))
                return floorPathfinders[floorNum];

            // Otherwise, create a new PathfindingGrid from the floor’s walkable tiles
            HashSet<Vector2Int> walkableTiles = new HashSet<Vector2Int>(floorData.FloorTiles);
            // If you want to exclude certain special tiles, do so here

            PathfindingGrid grid = new PathfindingGrid(walkableTiles);

            // If you have 8-direction movement, ensure your Node neighbor logic
            // in PathfindingGrid includes diagonals, and your heuristic is octile/chebyshev
            Pathfinder pathfinder = new Pathfinder(grid);

            floorPathfinders[floorNum] = pathfinder;
            return pathfinder;
        }

        #endregion

        #region Item Spawning (unchanged)

        public void SpawnItemsForAllFloors()
        {
            if (DungeonManager.Instance == null)
            {
                Debug.LogError("DungeonManager is not initialized. Cannot spawn items.");
                return;
            }

            foreach (var floorEntry in DungeonManager.Instance.floors)
            {
                FloorData floorData = floorEntry.Value;
                Transform floorParent = DungeonManager.Instance.GetFloorTransform(
                    floorData.FloorNumber
                );

                Transform itemParent = floorParent?.Find("ItemParent");
                if (itemParent == null)
                {
                    Debug.LogError(
                        $"ItemParent not found for Floor {floorData.FloorNumber}. Skipping item spawning."
                    );
                    continue;
                }

                StartCoroutine(SpawnItemsForFloor(floorData, floorParent, itemParent));
                SpawnHiddenItemsForFloor(floorData, floorParent, itemParent);
            }
        }

        private IEnumerator SpawnItemsForFloor(
            FloorData floorData,
            Transform floorParent,
            Transform itemParent
        )
        {
            int itemCount = dungeonSettings.numberOfItemsPerFloor;
            List<Vector2Int> randomFloorTiles = floorData.GetRandomFloorTiles(itemCount);

            foreach (var tilePos in randomFloorTiles)
            {
                // 1) Convert the tile position to an exact world position via CellToWorld
                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 baseWorldPos = floorData.FloorTilemap.CellToWorld(cellPos);

                // 2) Center the item in the tile
                Vector3 centeredWorldPos = baseWorldPos + new Vector3(0.5f, 0.5f, 0f);

                // 3) (Optional) If your tilemap is nested under floorParent,
                //    then you might already have offset. Usually,
                //    you can just parent the item under itemParent without adding floorParent.position.
                //    So we omit "floorParentPosition" entirely to avoid double offsets.

                // 4) Pick a random item from your itemPrefabs
                Item itemSO = (Item)
                    dungeonSettings.itemPrefabs[
                        UnityEngine.Random.Range(0, dungeonSettings.itemPrefabs.Count)
                    ];
                if (itemSO == null || itemSO.itemPrefab == null)
                {
                    Debug.LogError($"DungeonSpawner: Item prefab is null, skipping item spawn.");
                    continue;
                }

                // 5) Instantiate the item
                GameObject itemInstance = Instantiate(
                    itemSO.itemPrefab,
                    centeredWorldPos,
                    Quaternion.identity,
                    itemParent
                );

                if (itemInstance == null)
                {
                    Debug.LogError($"Failed to instantiate item at {centeredWorldPos}.");
                    continue;
                }
            }

            yield return null;
        }

        public void SpawnHiddenItemsForFloor(
            FloorData floorData,
            Transform floorParent,
            Transform itemParent
        )
        {
            // 1) Filter out a set of candidate tiles from floorData.WallTiles
            //    or special "secret" tiles if you prefer.
            List<Vector2Int> wallTiles = new List<Vector2Int>(floorData.WallTiles);

            // 2) Decide how many hidden items you want.
            //    Could be a random range or a fixed number
            int hiddenItemCount = 15;

            // 3) Pick random wall tile positions
            //    Make sure your list has enough tiles
            List<Vector2Int> spawnPositions = new List<Vector2Int>();
            for (int i = 0; i < hiddenItemCount; i++)
            {
                // pick random from wallTiles
                int index = UnityEngine.Random.Range(0, wallTiles.Count);
                spawnPositions.Add(wallTiles[index]);
                // Optionally remove the tile from the list so you don't pick it again
                wallTiles.RemoveAt(index);
            }

            // 4) For each chosen tile, spawn a hidden item
            foreach (var tilePos in spawnPositions)
            {
                // Grab a hidden item (some item with isHidden = true)
                Item randomItem = (Item)
                    dungeonSettings.itemPrefabs[
                        UnityEngine.Random.Range(0, dungeonSettings.itemPrefabs.Count)
                    ];
                if (randomItem == null)
                    continue; // skip if none found

                // Convert tile coords to world
                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 baseWorldPos = floorData.WallTilemap.CellToWorld(cellPos);
                // Possibly shift if you want it centered in the tile:
                Vector3 worldPos = baseWorldPos + new Vector3(0.5f, 0.5f, 0f);

                // Instantiate
                GameObject itemInstance = Instantiate(
                    randomItem.itemPrefab,
                    worldPos,
                    Quaternion.identity,
                    itemParent
                );
                // 5) Mark it as hidden in some script
                HiddenItemController hic = itemInstance.GetComponent<HiddenItemController>();
                if (hic != null)
                {
                    hic.isHidden = true; // The instance is hidden
                }
            }
        }

        #endregion

        #region Ambush/Boss Spawning (optional partial refactor)

        public void SpawnAmbush(Vector3 center, FloorData floorData, Transform enemyParent)
        {
            if (floorData == null || floorData.FloorTilemap == null || floorData.FloorTiles == null)
            {
                Debug.LogError("DungeonSpawner: Invalid floor data. Cannot spawn ambush.");
                return;
            }

            Vector3Int centerTile = floorData.FloorTilemap.WorldToCell(center);

            List<Vector2Int> spawnPositions = GetValidSpawnPositions(
                dungeonSettings.ambushEnemyCount,
                dungeonSettings.ambushRadius,
                centerTile,
                floorData
            );

            if (spawnPositions.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No valid spawn positions found for ambush.");
                return;
            }

            // Reuse or create pathfinder for the floor
            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            foreach (Vector2Int tilePos in spawnPositions)
            {
                // Optional effect
                PositionHelper.InstantiateOnTile(
                    floorData.FloorTilemap,
                    new Vector3Int(tilePos.x, tilePos.y, 0),
                    dungeonSettings.spawnEffectPrefab,
                    enemyParent,
                    new Vector3(0.5f, 0.5f, 0)
                );

                // --------------------- SPOT THE CHANGE HERE ---------------------
                // We spawn the enemy...
                GameObject enemy = SpawnEnemyAtTile(tilePos, floorData, enemyParent, pathfinder);
                // ...Then we do the same "setup" code as normal enemies:
                if (enemy != null)
                {
                    // 1) occupant ID
                    int occupantID = occupantIDCounter++;

                    // 2) get or add EnemyNavigator
                    EnemyNavigator navigator = enemy.GetComponent<EnemyNavigator>();
                    if (navigator == null)
                    {
                        navigator = enemy.GetComponent<EnemyNavigator>();
                    }
                    navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

                    // 3) get or add EnemyBrain
                    EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
                    if (brain == null)
                    {
                        brain = enemy.GetComponent<EnemyBrain>();
                    }

                    // 4) give some random patrol points (or an empty set if you want them stationary)
                    HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>(
                        floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                    );
                    brain.Initialize(floorData, patrolPoints);

                    // 5) set spawn floor on EnemyStats if present
                    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.spawnFloor = floorData.FloorNumber;
                    }
                }
                // -------------------- END CHANGE --------------------
            }
        }

        private GameObject SpawnEnemyAtTile(
            Vector2Int tilePosition,
            FloorData floorData,
            Transform parent,
            Pathfinder pathfinder
        )
        {
            if (dungeonSettings.enemyPrefabs == null || dungeonSettings.enemyPrefabs.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No enemy prefabs available.");
                return null;
            }

            GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                UnityEngine.Random.Range(0, dungeonSettings.enemyPrefabs.Count)
            ];

            return PositionHelper.InstantiateOnTile(
                floorData.FloorTilemap,
                new Vector3Int(tilePosition.x, tilePosition.y, 0),
                enemyPrefab,
                parent,
                new Vector3(0.5f, 0.5f, 0)
            );
        }

        private List<Vector2Int> GetValidSpawnPositions(
            int count,
            float radius,
            Vector3Int centerTile,
            FloorData floorData
        )
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            int attempts = 0;
            int maxAttempts = count * 10;

            while (positions.Count < count && attempts < maxAttempts)
            {
                int offsetX = UnityEngine.Random.Range(
                    -Mathf.CeilToInt(radius),
                    Mathf.CeilToInt(radius) + 1
                );
                int offsetY = UnityEngine.Random.Range(
                    -Mathf.CeilToInt(radius),
                    Mathf.CeilToInt(radius) + 1
                );

                Vector2Int candidate = new Vector2Int(
                    centerTile.x + offsetX,
                    centerTile.y + offsetY
                );

                if (floorData.FloorTiles.Contains(candidate) && !positions.Contains(candidate))
                {
                    positions.Add(candidate);
                }
                attempts++;
            }

            if (positions.Count < count)
            {
                Debug.LogWarning(
                    "GetValidSpawnPositions: Unable to find enough valid spawn positions."
                );
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
            Transform enemyParent = floorParent?.Find("EnemyParent");

            if (enemyParent == null)
            {
                Debug.LogError("DungeonSpawner: EnemyParent not found. Cannot spawn boss.");
                return;
            }

            GameObject bossPrefab = dungeonSettings.bossPrefabs[
                UnityEngine.Random.Range(0, dungeonSettings.bossPrefabs.Count)
            ];

            SpawnBoss(bossPrefab, floorData, enemyParent);
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
                Debug.LogError("DungeonSpawner: No valid spawn positions found for boss.");
                return;
            }

            // Reuse or create a pathfinder for the floor
            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            // Pick a valid tile for the boss
            Vector2Int tilePos = spawnPositions[0];
            // Convert tile to world coords + 0.5 offset
            Vector3 pos = new Vector3(tilePos.x, tilePos.y, 0);
            Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);

            // Optional effect
            Instantiate(dungeonSettings.spawnEffectPrefab, worldPos, Quaternion.identity);

            // Spawn the boss
            GameObject boss = Instantiate(bossPrefab, pos, Quaternion.identity, enemyParent);
            if (boss == null)
            {
                Debug.LogError("DungeonSpawner: Failed to instantiate boss.");
                return;
            }

            // ========== The same AI setup logic as normal enemies ==========

            // 1) occupant ID for tile reservation
            int occupantID = occupantIDCounter++;

            // 2) Navigator
            EnemyNavigator navigator = boss.GetComponent<EnemyNavigator>();
            if (navigator == null)
            {
                navigator = boss.AddComponent<EnemyNavigator>();
            }
            navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

            // 3) Brain
            EnemyBrain brain = boss.GetComponent<EnemyBrain>();
            if (brain == null)
            {
                brain = boss.AddComponent<EnemyBrain>();
            }

            // If you want the boss to actually have patrol points:
            HashSet<Vector2Int> bossPatrolPoints = new HashSet<Vector2Int>(
                floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
            );
            brain.Initialize(floorData, bossPatrolPoints);

            // 4) Stats
            EnemyStats bossStats = boss.GetComponent<EnemyStats>();
            if (bossStats != null)
            {
                bossStats.spawnFloor = floorData.FloorNumber;
            }
        }

        #endregion
    }
}
