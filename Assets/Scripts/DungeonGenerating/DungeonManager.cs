using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        public Dictionary<int, Transform> FloorTransforms { get; private set; } =
            new Dictionary<int, Transform>();
        public Dictionary<(int, int), HashSet<Vector2Int>> floorIntersections =
            new Dictionary<(int, int), HashSet<Vector2Int>>();

        [Header("Floors")]
        public Dictionary<int, FloorData> floors = new Dictionary<int, FloorData>();
        public GameObject SpawningRoomInstance { get; set; }
        private DungeonSettings dungeonSettings;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Optional: use if this object should persist across scenes.
                // Debug.Log("DungeonManager: Singleton instance created.");
            }
        }

        /// <summary>
        /// Adds a floor to the dungeon's collection.
        /// </summary>
        public void AddFloor(FloorData floor)
        {
            if (!floors.ContainsKey(floor.FloorNumber))
            {
                floors[floor.FloorNumber] = floor;
                //        Debug.Log($"Floor_{floor.FloorNumber} added to the dungeon.");
            }
            else
            {
                Debug.LogWarning($"Floor_{floor.FloorNumber} already exists.");
            }
            dungeonSettings = FindAnyObjectByType<DungeonGenerator>()
                ?.GetComponent<DungeonGenerator>()
                .dungeonSettings;
        }

        public Transform GetFloorTransform(int floorNumber)
        {
            if (FloorTransforms.TryGetValue(floorNumber, out Transform floorTransform))
            {
                return floorTransform;
            }
            Debug.LogError($"DungeonManager: Floor {floorNumber} not found.");
            return null;
        }

        public FloorData GetFloorData(int floorNumber)
        {
            if (floors.TryGetValue(floorNumber, out FloorData floorData))
            {
                return floorData;
            }
            Debug.LogError($"DungeonManager: Floor {floorNumber} not found.");
            return null;
        }

        //get random enemy prefab from the list in dugeon settings.
        public GameObject GetRandomEnemyPrefab()
        {
            if (dungeonSettings.enemyPrefabs.Count > 0)
            {
                return dungeonSettings.enemyPrefabs[
                    Random.Range(0, dungeonSettings.enemyPrefabs.Count)
                ];
            }
            return null;
        }
    }
}
