using System.Collections.Generic;
using System.Linq;
using CoED.Items;
using CoED.Pathfinding;
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

        [SerializeField]
        private LayerMask validSpawnLayer;

        [SerializeField]
        private Item spawningRoomPotion;
        private static int occupantIDCounter = 0;
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

            playerRB = player.GetComponent<Rigidbody2D>();
            dungeonSettings = FindAnyObjectByType<DungeonGenerator>()
                ?.GetComponent<DungeonGenerator>()
                .dungeonSettings;
        }

        #region Player Transport
        public void TransportPlayerToDungeon(Transform player)
        {
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
            }
            if (!DungeonManager.Instance.FloorTransforms.ContainsKey(1))
            {
                Debug.LogError("First floor Transform not found. Cannot transport player.");
                return;
            }

            FloorData firstFloorData = DungeonManager.Instance.GetFloorData(1);
            if (firstFloorData == null || firstFloorData.FloorTiles.Count == 0)
            {
                Debug.LogWarning("No floor tiles found for Floor 1. Cannot transport player.");
                return;
            }

            Vector2Int randomTile = GetRandomValidTile(firstFloorData);

            Vector3Int cellPos = new Vector3Int(randomTile.x, randomTile.y, 0);
            Vector3 baseWorldPos = firstFloorData.FloorTilemap.CellToWorld(cellPos);
            Vector3 centeredWorldPos = baseWorldPos + new Vector3(0.5f, 0.5f, 0f);

            player.position = centeredWorldPos;
            playerRB.position = centeredWorldPos;

            PlayerMovement.Instance.UpdateCurrentTilePosition(centeredWorldPos);
        }

        private Vector2Int GetRandomValidTile(FloorData floorData)
        {
            List<Vector2Int> floorTileList = floorData.FloorTiles.ToList();
            int maxAttempts = 100;

            while (maxAttempts > 0 && floorTileList.Count > 0)
            {
                Vector2Int candidate = floorTileList[Random.Range(0, floorTileList.Count)];

                if (
                    !floorData.WallTiles.Contains(candidate)
                    && !floorData.VoidTiles.Contains(candidate)
                )
                {
                    return candidate;
                }

                floorTileList.Remove(candidate);
                maxAttempts--;
            }

            Debug.LogWarning(
                "GetRandomValidTile: No valid tile found, returning first floor tile as fallback."
            );
            return floorData.FloorTiles.First();
        }

        #endregion

        #region Enemy Spawning (refactored)
        public void SpawnEnemiesForAllFloors()
        {
            foreach (var kvp in DungeonManager.Instance.floors.OrderBy(e => e.Key))
            {
                int floorNum = kvp.Key;
                FloorData floorData = DungeonManager.Instance.GetFloorData(floorNum);
                if (floorData == null)
                    continue;

                Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNum);
                if (floorParent == null)
                    continue;

                Transform enemyParent = floorParent.Find("EnemyParent");
                if (enemyParent == null)
                {
                    Debug.LogError(
                        $"EnemyParent not found for Floor {floorNum}. Skipping enemies."
                    );
                    continue;
                }

                SpawnEnemiesForFloor(floorNum, floorParent, enemyParent);
            }
        }

        private void SpawnEnemiesForFloor(
            int floorNumber,
            Transform floorParent,
            Transform enemyParent
        )
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"No FloorData for floor {floorNumber}.");
                return;
            }

            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            List<Vector2Int> allFloorTiles = DungeonManager
                .Instance.GetFloorData(floorNumber)
                .FloorTiles.ToList();

            List<Vector2Int> validTiles = allFloorTiles
                .Where(pos => IsValidSpawnPosition(floorData, pos))
                .ToList();

            int enemyCount = dungeonSettings.numberOfEnemiesPerFloor;
            if (validTiles.Count < enemyCount)
            {
                Debug.LogWarning(
                    $"Floor {floorNumber} does not have enough valid tiles for {enemyCount} enemies."
                );
                enemyCount = validTiles.Count;
            }

            if (enemyParent == null)
            {
                Debug.LogError(
                    $"EnemyParent not found under {floorParent.name}. Skipping floor {floorNumber}."
                );
                return;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                int randIndex = Random.Range(0, validTiles.Count);
                Vector2Int tilePos = validTiles[randIndex];
                validTiles.RemoveAt(randIndex);

                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 baseWorldPos = floorData.FloorTilemap.CellToWorld(cellPos);

                Vector3 finalSpawnPos = floorParent.position + baseWorldPos;

                GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                    Random.Range(0, dungeonSettings.enemyPrefabs.Count)
                ];
                if (enemyPrefab == null)
                {
                    Debug.LogError($"Null enemy prefab for floor {floorNumber}, skipping...");
                    continue;
                }

                GameObject enemy = Instantiate(
                    enemyPrefab,
                    baseWorldPos,
                    Quaternion.identity,
                    enemyParent
                );
                if (enemy == null)
                {
                    Debug.LogError(
                        $"Failed to instantiate enemy at {finalSpawnPos} for floor {floorNumber}."
                    );
                    continue;
                }

                occupantIDCounter++;
                int occupantID = occupantIDCounter;

                EnemyStats stats = enemy.GetComponent<EnemyStats>();
                if (stats != null)
                    stats.spawnFloor = floorNumber;

                EnemyNavigator navigator = enemy.GetComponent<EnemyNavigator>();
                if (navigator == null)
                    navigator = enemy.AddComponent<EnemyNavigator>();
                navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

                EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
                if (brain == null)
                    brain = enemy.AddComponent<EnemyBrain>();
                HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>(
                    floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                );
                brain.Initialize(floorData, patrolPoints);
            }
        }

        public bool IsValidSpawnPosition(FloorData floorData, Vector2Int gridPos)
        {
            List<Vector2Int> wallList = DungeonManager
                .Instance.GetFloorData(floorData.FloorNumber)
                .WallTiles.ToList();
            List<Vector2Int> voidList = DungeonManager
                .Instance.GetFloorData(floorData.FloorNumber)
                .VoidTiles.ToList();

            if (
                !floorData.FloorTiles.Contains(gridPos)
                || wallList.Contains(gridPos)
                || voidList.Contains(gridPos)
            )
            {
                return false;
            }

            Vector3 worldPos = floorData.FloorTilemap.CellToWorld(
                new Vector3Int(gridPos.x, gridPos.y, 0)
            );
            float checkRadius = 0.4f;
            bool overlapsWall =
                Physics2D.OverlapCircle(worldPos, checkRadius, obstacleLayer) != null;
            if (overlapsWall)
                return false;

            return true;
        }

        private Pathfinder GetOrCreateFloorPathfinder(FloorData floorData)
        {
            int floorNum = floorData.FloorNumber;
            if (floorPathfinders.ContainsKey(floorNum))
                return floorPathfinders[floorNum];

            HashSet<Vector2Int> walkable = DungeonManager
                .Instance.GetFloorData(floorNum)
                .FloorTiles;
            PathfindingGrid grid = new PathfindingGrid(walkable);
            Pathfinder pathfinder = new Pathfinder(grid);
            floorPathfinders[floorNum] = pathfinder;
            return pathfinder;
        }
        #endregion

        #region Item Spawning
        public void SpawnItemsForAllFloors()
        {
            if (DungeonManager.Instance == null)
            {
                Debug.LogError("DungeonManager is not initialized. Cannot spawn items.");
                return;
            }

            Instantiate(
                spawningRoomPotion.itemPrefab,
                new Vector3(-15f, -11f, 0),
                Quaternion.identity
            );

            foreach (var kvp in DungeonManager.Instance.floors.OrderBy(e => e.Key))
            {
                int floorNum = kvp.Key;
                FloorData floorData = DungeonManager.Instance.GetFloorData(floorNum);
                if (floorData == null)
                    continue;

                Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNum);
                if (floorParent == null)
                    continue;

                Transform itemParent = floorParent.Find("ItemParent");
                if (itemParent == null)
                {
                    Debug.LogError(
                        $"ItemParent not found for Floor {floorNum}. Skipping item spawning."
                    );
                    continue;
                }
                SpawnItemsOnFloor(floorNum, itemParent);
                SpawnHiddenItemsOnFloor(floorNum, itemParent);
            }
        }

        private void SpawnItemsOnFloor(int floorNumber, Transform itemParent)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"No FloorData for floor {floorNumber} to spawn items.");
                return;
            }

            int itemCount = dungeonSettings.numberOfItemsPerFloor;

            List<Vector2Int> floorTiles = DungeonManager
                .Instance.GetFloorData(floorNumber)
                .FloorTiles.ToList();

            List<Vector2Int> validTiles = floorTiles
                .Where(pos => IsValidSpawnPosition(floorData, pos))
                .ToList();

            if (validTiles.Count < itemCount)
            {
                Debug.LogWarning(
                    $"Floor {floorNumber} has only {validTiles.Count} valid tiles, "
                        + $"but we want {itemCount} items."
                );
                itemCount = validTiles.Count;
            }

            for (int i = 0; i < itemCount; i++)
            {
                int randIndex = Random.Range(0, validTiles.Count);
                Vector2Int tilePos = validTiles[randIndex];
                validTiles.RemoveAt(randIndex);

                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 spawnPos = floorData.FloorTilemap.CellToWorld(cellPos);
                spawnPos += new Vector3(0.5f, 0.5f, 0);

                Item itemSO = (Item)
                    dungeonSettings.itemPrefabs[Random.Range(0, dungeonSettings.itemPrefabs.Count)];
                if (itemSO == null || itemSO.itemPrefab == null)
                {
                    Debug.LogError($"Item prefab is null, skipping spawn on floor {floorNumber}.");
                    continue;
                }

                GameObject itemInstance = Instantiate(
                    itemSO.itemPrefab,
                    spawnPos,
                    Quaternion.identity,
                    itemParent
                );
                if (itemInstance == null)
                {
                    Debug.LogError(
                        $"Failed to instantiate item at {spawnPos} for floor {floorNumber}."
                    );
                    continue;
                }
            }
        }

        private void SpawnHiddenItemsOnFloor(int floorNumber, Transform itemParent)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"No FloorData for floor {floorNumber} to spawn hidden items.");
                return;
            }

            List<Vector2Int> wallTiles = DungeonManager
                .Instance.GetFloorData(floorNumber)
                .WallTiles.ToList();

            int hiddenItemCount = 15;
            if (wallTiles.Count < hiddenItemCount)
            {
                Debug.LogWarning(
                    $"Floor {floorNumber} only has {wallTiles.Count} wall tiles, but we want {hiddenItemCount}."
                );
                hiddenItemCount = wallTiles.Count;
            }

            for (int i = 0; i < hiddenItemCount; i++)
            {
                int randIndex = Random.Range(0, wallTiles.Count);
                Vector2Int tilePos = wallTiles[randIndex];
                wallTiles.RemoveAt(randIndex);

                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 spawnPos = floorData.WallTilemap.CellToWorld(cellPos);
                spawnPos += new Vector3(0.5f, 0.5f, 0);

                Item randomItem = (Item)
                    dungeonSettings.itemPrefabs[Random.Range(0, dungeonSettings.itemPrefabs.Count)];
                if (randomItem == null || randomItem.itemPrefab == null)
                    continue;

                GameObject itemInstance = Instantiate(
                    randomItem.itemPrefab,
                    spawnPos,
                    Quaternion.identity,
                    itemParent
                );

                if (itemInstance == null)
                    continue;

                HiddenItemController hic = itemInstance.GetComponent<HiddenItemController>();
                if (hic != null)
                {
                    hic.isHidden = true;
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

            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            foreach (Vector2Int tilePos in spawnPositions)
            {
                PositionHelper.InstantiateOnTile(
                    floorData.FloorTilemap,
                    new Vector3Int(tilePos.x, tilePos.y, 0),
                    dungeonSettings.spawnEffectPrefab,
                    enemyParent
                );

                GameObject enemy = SpawnEnemyAtTile(tilePos, floorData, enemyParent);
                if (enemy != null)
                {
                    int occupantID = occupantIDCounter++;

                    EnemyNavigator navigator = enemy.GetComponent<EnemyNavigator>();
                    if (navigator == null)
                    {
                        navigator = enemy.GetComponent<EnemyNavigator>();
                    }
                    navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

                    EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
                    if (brain == null)
                    {
                        brain = enemy.GetComponent<EnemyBrain>();
                    }

                    HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>(
                        floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                    );
                    brain.Initialize(floorData, patrolPoints);

                    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.spawnFloor = floorData.FloorNumber;
                    }
                }
            }
        }

        private GameObject SpawnEnemyAtTile(
            Vector2Int tilePosition,
            FloorData floorData,
            Transform parent
        )
        {
            if (dungeonSettings.enemyPrefabs == null || dungeonSettings.enemyPrefabs.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No enemy prefabs available.");
                return null;
            }

            GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                Random.Range(0, dungeonSettings.enemyPrefabs.Count)
            ];

            return PositionHelper.InstantiateOnTile(
                floorData.FloorTilemap,
                new Vector3Int(tilePosition.x, tilePosition.y, 0),
                enemyPrefab,
                parent
            );
        }
        #endregion
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
                int offsetX = Random.Range(-Mathf.CeilToInt(radius), Mathf.CeilToInt(radius) + 1);
                int offsetY = Random.Range(-Mathf.CeilToInt(radius), Mathf.CeilToInt(radius) + 1);

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

        #region Boss Spawning
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
                Random.Range(0, dungeonSettings.bossPrefabs.Count)
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

            List<Vector2Int> spawnPositions = DungeonManager
                .Instance.GetFloorData(floorData.FloorNumber)
                .FloorTiles.ToList();

            if (spawnPositions.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No valid spawn positions found for boss.");
                return;
            }

            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);
            Vector2Int tilePos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            Vector3 pos = floorData.FloorTilemap.CellToWorld(
                new Vector3Int(tilePos.x, tilePos.y, 0)
            );

            Instantiate(dungeonSettings.spawnEffectPrefab, pos, Quaternion.identity);

            GameObject boss = Instantiate(bossPrefab, pos, Quaternion.identity, enemyParent);
            if (boss == null)
            {
                Debug.LogError("DungeonSpawner: Failed to instantiate boss.");
                return;
            }

            int occupantID = occupantIDCounter++;

            EnemyNavigator navigator = boss.GetComponent<EnemyNavigator>();
            if (navigator == null)
            {
                navigator = boss.AddComponent<EnemyNavigator>();
            }
            navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

            EnemyBrain brain = boss.GetComponent<EnemyBrain>();
            if (brain == null)
            {
                brain = boss.AddComponent<EnemyBrain>();
            }

            HashSet<Vector2Int> bossPatrolPoints = new HashSet<Vector2Int>(
                floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
            );
            brain.Initialize(floorData, bossPatrolPoints);

            EnemyStats bossStats = boss.GetComponent<EnemyStats>();
            if (bossStats != null)
            {
                bossStats.spawnFloor = floorData.FloorNumber;
            }
        }
        #endregion
    }
}
