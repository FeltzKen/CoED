using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CoED;
using UnityEngine.Tilemaps;


namespace CoED
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        public Dictionary<int, Transform> FloorTransforms { get; private set; } = new Dictionary<int, Transform>();
        public Dictionary<(int, int), HashSet<Vector2Int>> floorIntersections = new Dictionary<(int, int), HashSet<Vector2Int>>();

        [Header("Floors")]
        public Dictionary<int, FloorData> floors = new Dictionary<int, FloorData>();
        public GameObject SpawningRoomInstance { get; set; }
        private FloorData floorData;
        private DungeonSettings dungeonSettings;
        private int currentFloorNumber = 0;

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
                Debug.Log($"Floor_{floor.FloorNumber} added to the dungeon.");
            }
            else
            {
                Debug.LogWarning($"Floor_{floor.FloorNumber} already exists.");
            }
            dungeonSettings = FindAnyObjectByType<DungeonGenerator>()?.GetComponent<DungeonGenerator>().dungeonSettings;

        }



        public void ShowRenderersForFloor(int floorNumber)
        {
            if (FloorTransforms.TryGetValue(floorNumber, out Transform floorTransform))
            {
                Renderer[] renderers = floorTransform.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                // Debug.Log($"All renderers for Floor {floorNumber} are now visible.");
            }
            else
            {
                Debug.LogError($"Floor {floorNumber} not found. Cannot show renderers.");
            }
        }

        public Tilemap GetCurrentFloorTilemap(int currentFloorNumber)
        {
            // Assuming you have a way to get the current floor number
            if (floors.TryGetValue(currentFloorNumber, out FloorData floorData))
            {
                return floorData.FloorTilemap;
            }
            Debug.LogError($"DungeonManager: Floor {currentFloorNumber} not found.");
            return null;
        }

        private Dictionary<int, Vector2Int> stairsDownPositions = new Dictionary<int, Vector2Int>();

        public void StoreStairsDownPosition(int floorNumber, Vector2Int position)
        {
            stairsDownPositions[floorNumber] = position;
        }

        public Vector2Int GetStairsDownPosition(int floorNumber)
        {
            if (stairsDownPositions.TryGetValue(floorNumber, out Vector2Int position))
            {
                return position;
            }
            return Vector2Int.zero; // Default if not found
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

        public int GetCurrentFloor()
        {
            return currentFloorNumber;
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
                return dungeonSettings.enemyPrefabs[Random.Range(0, dungeonSettings.enemyPrefabs.Count)];
            }
            return null;
        }
    }
}
