using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        [SerializeField] private GameObject dungeonParent; // Assign this manually in the Inspector
        public Dictionary<int, Transform> FloorTransforms { get; private set; } = new Dictionary<int, Transform>();

        [Header("Floors")]
        public Dictionary<int, FloorData> floors = new Dictionary<int, FloorData>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // // Debug.Log("DungeonManager initialized.");
            }
            else
            {
                Debug.LogWarning("Duplicate DungeonManager instance detected. Destroying this instance.");
                Destroy(gameObject);
            }
        }

        public Dictionary<int, Transform> GetFloorTransforms()
        {
            return FloorTransforms;
        }

        /// <summary>
        /// Adds a floor to the dungeon's collection.
        /// </summary>
        public void AddFloor(FloorData floor, Transform floorTransform)
        {
            if (!floors.ContainsKey(floor.FloorNumber))
            {
                floors[floor.FloorNumber] = floor;
                FloorTransforms[floor.FloorNumber] = floorTransform;
                // // Debug.Log($"Added Floor_{floor.FloorNumber} with Transform {floorTransform.name}.");
            }
            else
            {
                Debug.LogWarning($"Floor_{floor.FloorNumber} already exists.");
            }
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
        /// Gets a random tile from any floor.
        /// </summary>
        public Vector3Int GetRandomDungeonTile()
        {
            if (floors.Count == 0)
            {
                Debug.LogError("No floors available in the dungeon.");
                return Vector3Int.zero;
            }

            int randomFloorKey = Random.Range(1, floors.Count + 1); // Assuming floor numbers start at 1
            FloorData randomFloor = floors[randomFloorKey];
            return randomFloor.GetRandomFloorTile();
        }

        /// <summary>
        /// Gets a random tile from a specific floor.
        /// </summary>
        public Vector3Int GetRandomTileFromFloor(int floorNumber)
        {
            FloorData floor = GetFloor(floorNumber);
            if (floor != null)
                return floor.GetRandomFloorTile();

            return Vector3Int.zero;
        }

        /// <summary>
        /// Gets a specified number of random tiles from a specific floor.
        /// </summary>
        public List<Vector3Int> GetRandomTilesFromFloor(int floorNumber, int count)
        {
            FloorData floor = GetFloor(floorNumber);
            if (floor != null)
                return floor.GetRandomFloorTiles(count);

            return new List<Vector3Int>();
        }
    }
}