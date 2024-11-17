using System.Collections.Generic;
using UnityEngine;

namespace YourGameNamespace
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        [Header("Floors")]
        public List<FloorData> Floors = new List<FloorData>();

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
            }
        }

        /// <summary>
        /// Adds a floor to the dungeon's list of floors.
        /// </summary>
        public void AddFloor(FloorData floor)
        {
            Floors.Add(floor);
            Debug.Log($"Floor {floor.FloorNumber} added to DungeonManager.");
        }

        /// <summary>
        /// Gets the FloorData for a specific floor number.
        /// </summary>
        public FloorData GetFloor(int floorNumber)
        {
            foreach (var floor in Floors)
            {
                if (floor.FloorNumber == floorNumber)
                    return floor;
            }

            Debug.LogError($"Floor {floorNumber} not found.");
            return null;
        }

        /// <summary>
        /// Gets a random tile from any floor in the dungeon.
        /// </summary>
        public Vector2Int GetRandomDungeonTile()
        {
            if (Floors.Count == 0)
            {
                Debug.LogError("No floors available in the dungeon.");
                return Vector2Int.zero;
            }

            FloorData randomFloor = Floors[Random.Range(0, Floors.Count)];
            return randomFloor.GetRandomFloorTile();
        }

        /// <summary>
        /// Gets a random tile from a specific floor.
        /// </summary>
        public Vector2Int GetRandomTileFromFloor(int floorNumber)
        {
            FloorData floor = GetFloor(floorNumber);
            if (floor != null)
                return floor.GetRandomFloorTile();

            Debug.LogError($"Cannot get random tile. Floor {floorNumber} does not exist.");
            return Vector2Int.zero;
        }

        /// <summary>
        /// Gets a specified number of random tiles from a specific floor.
        /// </summary>
        public List<Vector2Int> GetRandomTilesFromFloor(int floorNumber, int count)
        {
            FloorData floor = GetFloor(floorNumber);
            if (floor != null)
                return floor.GetRandomFloorTiles(count);

            Debug.LogError($"Cannot get random tiles. Floor {floorNumber} does not exist.");
            return new List<Vector2Int>();
        }
    }
}
