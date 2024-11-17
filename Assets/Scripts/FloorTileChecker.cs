using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class FloorTileChecker : MonoBehaviour
    {
        private HashSet<Vector2Int> floorTilePositions;

        public void SetFloorTiles(HashSet<Vector2Int> floorTiles)
        {
            floorTilePositions = floorTiles;
            Debug.Log($"SetFloorTiles called. Floor tile count: {floorTilePositions?.Count ?? 0}");
        }

        public bool IsFloorTile(Vector3 worldPosition)
        {
            if (floorTilePositions == null)
            {
                Debug.LogError("FloorTilePositions is null. Ensure SetFloorTiles is called after dungeon generation.");
                return false;
            }

            Vector2Int tilePosition = new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
            return floorTilePositions.Contains(tilePosition);
        }

        [ContextMenu("Test Floor Tile Check")]
        public void TestFloorTileCheck()
        {
            if (floorTilePositions == null)
            {
                Debug.LogError("FloorTilePositions has not been initialized. Make sure SetFloorTiles() is called.");
                return;
            }

            int validFloorCount = 0;
            Vector3 areaCenter = Vector3.zero;
            float areaSize = 100f;

            for (int i = 0; i < 100; i++)
            {
                Vector3 randomPoint = areaCenter + new Vector3(
                    Random.Range(-areaSize / 2, areaSize / 2),
                    0,
                    Random.Range(-areaSize / 2, areaSize / 2)
                );

                if (IsFloorTile(randomPoint))
                {
                    validFloorCount++;
                    Debug.Log($"Floor tile found at {randomPoint}");
                }
                else
                {
                    Debug.LogWarning($"No floor tile at {randomPoint}");
                }
            }

            Debug.Log($"Total floor tiles found: {validFloorCount} out of 100");
        }
    }
}
