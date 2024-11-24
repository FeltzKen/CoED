using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CoED;


namespace CoED
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        public Dictionary<int, Transform> FloorTransforms { get; private set; } = new Dictionary<int, Transform>();

        [Header("Floors")]
        public Dictionary<int, FloorData> floors = new Dictionary<int, FloorData>();
        public GameObject SpawningRoomInstance { get; set; }
        private FloorData floorData;
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
            Debug.Log("DungeonManager: Singleton instance created.");
        }
    }

    // Other code...



        /// <summary>
        /// Retrieves the next available floor number.
        /// </summary>
        public int GetNextFloorNumber()
        {
            return ++currentFloorNumber;
        }

        /// <summary>
        /// Adds a floor to the dungeon's collection.
        /// </summary>
        public void AddFloor(FloorData floor)
        {
            if (!floors.ContainsKey(floor.FloorNumber))
            {
                floors[floor.FloorNumber] = floor;
                Debug.Log($"Added Floor_{floor.FloorNumber}.");
            }
            else
            {
                Debug.LogWarning($"Floor_{floor.FloorNumber} already exists.");
            }
        }

        /// <summary>
        /// Retrieves the walkable tiles for a specific floor.
        /// </summary>
        public HashSet<Vector2> GetWalkableTilesForFloor(int floorNumber)
        {
            if (floors.TryGetValue(floorNumber, out var floor))
            {
                return new HashSet<Vector2>(floorData.FloorTiles.Select(tile => (Vector2)tile));
            }
            Debug.LogError($"No FloorData found for floor {floorNumber}.");
            return new HashSet<Vector2>(); // Return an empty set as fallback
        }

        /// <summary>
        /// Gets the FloorData for a specific floor number.
        /// </summary>
        public FloorData GetFloor(int floorNumber, bool logError = true)
        {
            if (floors.TryGetValue(floorNumber, out var floor))
            {
                return floor;
            }

            if (logError)
            {
                Debug.LogError($"Floor {floorNumber} not found.");
            }
            return null;
        }

        /// <summary>
        /// Gets a random walkable tile from any floor.
        /// </summary>
        public Vector2 GetRandomDungeonTile()
        {
            if (floors.Count == 0)
            {
                Debug.LogError("No floors available in the dungeon.");
                return Vector2.zero;
            }

            int randomFloorKey = Random.Range(1, floors.Count + 1); // Assuming floor numbers start at 1
            FloorData randomFloor = floors[randomFloorKey];
            return randomFloor.GetRandomFloorTile();
        }

        /// <summary>
        /// Gets a random walkable tile from a specific floor.
        /// </summary>
        public Vector2 GetRandomTileFromFloor(int floorNumber)
        {
            FloorData floor = GetFloor(floorNumber);
            if (floor != null)
            {
                return floor.GetRandomFloorTile();
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Gets a specified number of random tiles from a specific floor.
        /// </summary>
        public List<Vector2Int> GetRandomTilesFromFloor(int floorNumber, int count)
        {
            FloorData floor = GetFloor(floorNumber);
            if (floor != null)
            {
                return floor.GetRandomFloorTiles(count);
            }

            return new List<Vector2Int>();
        }

        public void SetSpawningRoomInstance(GameObject instance)
        {
            SpawningRoomInstance = instance;
        }

    }
}
