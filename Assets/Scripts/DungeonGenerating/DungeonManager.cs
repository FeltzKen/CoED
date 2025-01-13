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

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

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
    }
}
