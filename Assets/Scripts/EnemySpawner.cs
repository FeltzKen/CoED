using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using YourGameNamespace;
namespace YourGameNamespace
{
    public class EnemySpawner 
    {
        [SerializeField] private DungeonSettings dungeonSettings;
        private Dictionary<int, Transform> enemyParents = new Dictionary<int, Transform>();
        private Transform playerTransform;
        private EnemyAI enemyAI;
        public EnemySpawner(DungeonSettings settings, Transform player)
        {
            dungeonSettings = settings;
            playerTransform = player;
        }

        /// <summary>
        /// Initializes enemy parent objects for each floor.
        /// </summary>
        public void InitializeEnemyParents()
        {
            Debug.Log("Initializing enemy parents...");
            foreach (var pair in DungeonManager.Instance.FloorTransforms)
            {
                if (pair.Value == null)
                {
                    Debug.LogError($"Floor_{pair.Key} transform is null. Cannot create enemy parent.");
                    continue;
                }
                CreateEnemyParentForFloor(pair.Key, pair.Value);
            }
        }

private void CreateEnemyParentForFloor(int floorNumber, Transform floorTransform)
{
    if (floorTransform == null)
    {
        Debug.LogError($"Floor_{floorNumber} transform is null. Cannot create enemy parent.");
        return;
    }

    if (enemyParents.ContainsKey(floorNumber))
    {
        Debug.LogWarning($"Enemy parent already exists for Floor {floorNumber}. Skipping creation.");
        return;
    }

    GameObject enemyParent = new GameObject($"EnemyParent_{floorNumber}");
    enemyParent.transform.parent = floorTransform;
    enemyParents[floorNumber] = enemyParent.transform;

    Debug.Log($"Enemy parent created for Floor {floorNumber}: {enemyParent.name}, Parent: {floorTransform.name}");
}


        /// <summary>
        /// Spawns enemies for all floors with enhanced features.
        /// </summary>
public IEnumerator SpawnEnemiesForAllFloorsAsync()
{
    foreach (var floorPair in DungeonManager.Instance.floors)
    {
        int floorNumber = floorPair.Key;
        FloorData floorData = floorPair.Value;

        // Log floor information
        Debug.Log($"Floor {floorNumber} has {floorData.FloorTiles.Count} tiles and {floorData.PatrolPoints.Count} patrol points.");

        // Get the enemy parent for this floor
        if (!enemyParents.TryGetValue(floorNumber, out Transform enemyParent))
        {
            Debug.LogError($"Enemy parent not found for Floor {floorNumber}. Skipping enemy spawning.");
            continue;
        }

        // Call SpawnEnemiesForFloor for the current floor
        yield return SpawnEnemiesForFloor(floorData, enemyParent);
    }
}


private IEnumerator SpawnEnemiesForFloor(FloorData floorData, Transform enemyParent)
{
    if (floorData.PatrolPoints.Count == 0)
    {
        Debug.LogError($"No patrol points available for Floor {floorData.FloorNumber}. Cannot spawn enemies.");
        yield break;
    }

    int enemyCount = Mathf.Min(floorData.PatrolPoints.Count, GetDynamicEnemyCount(floorData.FloorNumber));
    List<Vector3> spawnPositions = floorData.PatrolPoints.Take(enemyCount).ToList();

    foreach (var position in spawnPositions)
    {
        if (!IsValidSpawnPosition(position))
            continue;

        // Snap position to whole numbers
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            position.z
        );

        yield return PlaySpawnEffect(snappedPosition);

        // Spawn the enemy
        GameObject enemyPrefab = dungeonSettings.enemyPrefabs[Random.Range(0, dungeonSettings.enemyPrefabs.Count)];
        GameObject enemy = Object.Instantiate(enemyPrefab, snappedPosition, Quaternion.identity, enemyParent);

        // Set the spawn floor and patrol points for the enemy
        var enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.spawnFloor = floorData.FloorNumber; // Set the spawn floor
            Debug.Log($"EnemySpawner: Set spawnFloor to {floorData.FloorNumber} for spawned enemy.");
        }
        
        var enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            // Convert the walkable tiles from Vector3Int to Vector2Int
            HashSet<Vector3Int> walkableTiles3D = floorData.GetWalkableTiles();
            HashSet<Vector2Int> walkableTiles2D = new HashSet<Vector2Int>(
                walkableTiles3D.Select(tile => new Vector2Int(tile.x, tile.y))
            );
            enemyAI.SetWalkableTiles(walkableTiles2D);

            // Assign patrol points as well
            var selectedPatrolPoints = floorData.PatrolPoints
                .OrderBy(_ => Random.value)
                .Take(7)
                .Select(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y)))
                .ToList();
            enemyAI.SetPatrolPoints(selectedPatrolPoints);

            Debug.Log($"EnemySpawner: Assigned walkable tiles and {selectedPatrolPoints.Count} patrol points to enemy on Floor {floorData.FloorNumber}.");
        }


        InitializeEnemy(enemy, floorData);
        Debug.Log($"Spawned enemy at snapped position {snappedPosition} on Floor {floorData.FloorNumber}.");
        yield return null; // Avoid freezing
    }
}

        private int GetDynamicEnemyCount(int floorNumber)
        {
            // Adjust enemy count dynamically based on floor number or difficulty
            return Mathf.Clamp(floorNumber * 2, dungeonSettings.numberOfEnemiesPerFloor, 20);
        }

        private List<Vector3> GetStrategicSpawnPositions(FloorData floorData, int count)
        {
            // Convert HashSet<Vector2Int> to List<Vector2Int>
            List<Vector2Int> patrolPointsList = enemyAI.patrolPoints.ToList();
            List<Vector3> positions = new List<Vector3>();

            // Clustered spawn example
            for (int i = 0; i < count; i++)
            {
                if (patrolPointsList.Count == 0)
                {
                    Debug.LogWarning("No patrol points available.");
                    return positions;
                }

                // Pick a random point from the list
                Vector2Int center = patrolPointsList[Random.Range(0, patrolPointsList.Count)];
                positions.Add(new Vector3(center.x + Random.Range(-2f, 2f), center.y + Random.Range(-2f, 2f), 0));
            }

            return positions;
        }

    /// <summary>
    /// Generates patrol points for a specific floor.
    /// </summary>
    public List<Vector3> GeneratePatrolPointsForFloor(FloorData floorData, int numberOfPoints)
    {
        if (floorData == null || floorData.FloorTiles == null || floorData.FloorTiles.Count == 0)
        {
            Debug.LogError($"Cannot generate patrol points: FloorData {floorData?.FloorNumber} has no floor tiles.");
            return new List<Vector3>();
        }

        List<Vector3> patrolPoints = new List<Vector3>();
        List<Vector3Int> floorTiles = floorData.FloorTiles.ToList();

        for (int i = 0; i < numberOfPoints; i++)
        {
            // Select a random tile from the floor tiles
            Vector3Int randomTile = floorTiles[Random.Range(0, floorTiles.Count)];

            // Convert the tile position to world space
            Vector3 patrolPoint = floorData.FloorTilemap.CellToWorld(randomTile) + new Vector3(0.5f, 0.5f, 0); // Center of the tile
            patrolPoints.Add(patrolPoint);
        }
        Debug.Log($"Generated {patrolPoints.Count} patrol points for Floor {floorData.FloorNumber}: {string.Join(", ", patrolPoints)}");
        return patrolPoints;
    }

            /// <summary>
            /// Generates patrol points for all floors and assigns them to FloorData.
            /// </summary>
            public void GeneratePatrolPointsForAllFloors()
            {
                foreach (var floorData in DungeonManager.Instance.floors.Values)
                {
                    // Generate patrol points for the floor
                    var patrolPoints = GeneratePatrolPointsForFloor(floorData, 50); // Example: Generate 50 patrol points
                    floorData.SetPatrolPoints(patrolPoints);

                    Debug.Log($"Generated {patrolPoints.Count} patrol points for Floor {floorData.FloorNumber}.");
                }
            }

        private bool IsValidSpawnPosition(Vector3 position)
        {
            // Check if position is unobstructed
            Collider2D hit = Physics2D.OverlapCircle(position, 0.5f);
            return hit == null && Vector3.Distance(position, playerTransform.position) > 5f;
        }

        private IEnumerator PlaySpawnEffect(Vector3 position)
        {
            if (dungeonSettings.spawnEffectPrefab != null)
            {
                GameObject effect = Object.Instantiate(dungeonSettings.spawnEffectPrefab, position, Quaternion.identity);
                yield return new WaitForSeconds(1.0f);
                Object.Destroy(effect);
            }
        }

        private GameObject GetEnemyPrefabForFloor(int floorNumber)
        {
            if (dungeonSettings.enemyPrefabs == null || dungeonSettings.enemyPrefabs.Count == 0)
            {
                Debug.LogError("No enemy prefabs available in DungeonSettings.");
                return null;
            }

            // Randomly select a single prefab from the list
            int prefabIndex = Random.Range(0, dungeonSettings.enemyPrefabs.Count);
            return dungeonSettings.enemyPrefabs[prefabIndex];
        }


        //initalize enemy with patrol points
        private void InitializeEnemy(GameObject enemy, FloorData floorData)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
               // enemyAI.SetPatrolPoints(floorData.PatrolPoints.Select(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y))));
               // Debug.Log($"Assigned {floorData.PatrolPoints.Count} shared patrol points to enemy on Floor {floorData.FloorNumber}.");
            }
        }


        private List<Vector2Int> GetRandomizedPatrolRoute(FloorData floorData)
        {
            if (floorData.PatrolPoints == null || floorData.PatrolPoints.Count == 0)
            {
                Debug.LogError($"No patrol points available for Floor {floorData.FloorNumber}.");
                return new List<Vector2Int>();
            }

            List<Vector2Int> patrolRoute = new List<Vector2Int>();
            for (int i = 0; i < 5; i++) // Example patrol route size
            {
                Vector3 point = floorData.PatrolPoints[Random.Range(0, floorData.PatrolPoints.Count)];
                patrolRoute.Add(new Vector2Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y)));
            }

            return patrolRoute;
        }




        /// <summary>
        /// Triggers an ambush spawn near the player.
        /// </summary>
        public void TriggerAmbush(FloorData floorData)
        {
            if (!enemyParents.TryGetValue(floorData.FloorNumber, out Transform enemyParent))
                return;

            for (int i = 0; i < 3; i++) // Spawn 3 enemies as ambush
            {
                Vector3 ambushPosition = playerTransform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                if (IsValidSpawnPosition(ambushPosition))
                {
 GameObject randomEnemyPrefab = dungeonSettings.enemyPrefabs[Random.Range(0, dungeonSettings.enemyPrefabs.Count)];
GameObject enemy = Object.Instantiate(randomEnemyPrefab, ambushPosition, Quaternion.identity, enemyParent);

                }
            }

            Debug.Log("Ambush triggered!");
        }
    }
}
