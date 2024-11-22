using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class InstantiateWalls : MonoBehaviour
    {
        // Singleton instance for easy access across the project
        public static InstantiateWalls Instance { get; private set; }

        [Header("Wall Settings")]
        [SerializeField]
        private GameObject wallPrefab;

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
                Debug.LogWarning("InstantiateWalls instance already exists. Destroying duplicate.");
            }
        }

        // Creates walls around the combined floor tiles of a floor
        public void CreateWalls(FloorData floor, Transform floorParent)
        {
            HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

            // Directions to check around each floor tile for potential walls
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector3Int floorTile in floor.FloorTiles) // Looping through Vector3Int
            {
                foreach (Vector2Int dir in directions)
                {
                    // Create a neighbor position as Vector2Int
                    Vector2Int neighbor = new Vector2Int(floorTile.x + dir.x, floorTile.y + dir.y);

                    // Only add a wall if this tile is not already a floor tile and not in wall positions
                    if (!floor.FloorTiles.Contains(new Vector3Int(neighbor.x, neighbor.y, 0)) && !wallPositions.Contains(neighbor))
                    {
                        wallPositions.Add(neighbor);

                        // Instantiate wall at the neighbor position
                        Vector3 wallPosition = new Vector3(neighbor.x, neighbor.y, 0);
                        Instantiate(wallPrefab, wallPosition, Quaternion.identity, floorParent);
                    }
                }
            }
        }


    }
}
