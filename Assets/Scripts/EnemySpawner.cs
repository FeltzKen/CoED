using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YourGameNamespace
{
    public class EnemySpawner
    {
        private DungeonSettings dungeonSettings;
        private Dictionary<int, Transform> enemyParents = new Dictionary<int, Transform>();
        private Transform playerTransform;

        public EnemySpawner(DungeonSettings settings, Transform player)
        {
            dungeonSettings = settings;
            playerTransform = player;
        }

        /// <summary>
        /// Initializes enemy parent objects for each floor.
        /// </summary>
        public void InitializeEnemyParents(Dictionary<int, Transform> floorTransforms)
        {
            foreach (var pair in floorTransforms)
            {
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

            GameObject enemyParent = new GameObject($"EnemyParent_{floorNumber}");
            enemyParent.transform.parent = floorTransform;
            enemyParents[floorNumber] = enemyParent.transform;
            Debug.Log($"Enemy parent created for Floor {floorNumber}.");
        }

        /// <summary>
        /// Spawns enemies for all floors with enhanced features.
        /// </summary>
        public IEnumerator SpawnEnemiesForAllFloorsAsync()
        {
            foreach (var floor in DungeonManager.Instance.Floors)
            {
                if (!enemyParents.TryGetValue(floor.FloorNumber, out Transform enemyParent))
                {
                    Debug.LogError($"Enemy parent not found for Floor {floor.FloorNumber}. Skipping enemy spawn.");
                    continue;
                }

                yield return SpawnEnemiesForFloor(floor, enemyParent);
            }
        }

        private IEnumerator SpawnEnemiesForFloor(FloorData floorData, Transform enemyParent)
        {
            int enemyCount = GetDynamicEnemyCount(floorData.FloorNumber);
            List<Vector3> spawnPositions = GetStrategicSpawnPositions(floorData, enemyCount);

            foreach (var position in spawnPositions)
            {
                if (!IsValidSpawnPosition(position))
                    continue;

                yield return PlaySpawnEffect(position);

                GameObject enemyPrefab = GetEnemyPrefabForFloor(floorData.FloorNumber);
                GameObject enemy = Object.Instantiate(enemyPrefab, position, Quaternion.identity, enemyParent);

                InitializeEnemy(enemy, floorData);
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
            List<Vector3> positions = new List<Vector3>();

            // Clustered spawn example
            for (int i = 0; i < count; i++)
            {
                Vector3 center = floorData.PatrolPoints[Random.Range(0, floorData.PatrolPoints.Count)];
                positions.Add(center + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0));
            }

            return positions;
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
            // Return enemy prefab based on floor difficulty
            return floorNumber > 3 ? dungeonSettings.strongEnemyPrefab : dungeonSettings.enemyPrefab;
        }

        private void InitializeEnemy(GameObject enemy, FloorData floorData)
        {
            // Assign stats, patrol routes, or behaviors
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.spawnFloor = floorData.FloorNumber;
                Debug.Log($"Enemy initialized for Floor {floorData.FloorNumber}.");
            }

            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.SetPatrolPoints(GetRandomizedPatrolRoute(floorData));
            }
        }

        private List<Vector3> GetRandomizedPatrolRoute(FloorData floorData)
        {
            List<Vector3> patrolRoute = new List<Vector3>();
            for (int i = 0; i < 5; i++) // Example patrol route size
            {
                patrolRoute.Add(floorData.PatrolPoints[Random.Range(0, floorData.PatrolPoints.Count)]);
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
                    GameObject enemy = Object.Instantiate(dungeonSettings.enemyPrefab, ambushPosition, Quaternion.identity, enemyParent);
                    InitializeEnemy(enemy, floorData);
                }
            }

            Debug.Log("Ambush triggered!");
        }
    }
}
