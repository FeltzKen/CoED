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
            }
            else
            {
                Debug.LogWarning($"Floor_{floor.FloorNumber} already exists.");
            }
        }

        public void SetSpawningRoomInstance(GameObject instance)
        {
            SpawningRoomInstance = instance;
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

    }
}
