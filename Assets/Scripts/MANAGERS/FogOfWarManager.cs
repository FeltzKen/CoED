using UnityEngine;
using UnityEngine.Tilemaps;
using CoED;

namespace CoED
{
    public class FogOfWarManager : MonoBehaviour
    {
        public static FogOfWarManager Instance { get; private set; }

        [Header("Fog Settings")]
        [SerializeField]
        private Tilemap fogTilemap;

        [SerializeField]
        private TileBase fogTile;

        [SerializeField, Range(1, 20)]
        private int visionRadius = 5;

        private void Awake()
        {
            // Implement Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("FogOfWarManager instance already exists. Destroying duplicate.");
                return;
            }

            // Validate fogTilemap and fogTile
            if (fogTilemap == null)
            {
                Debug.LogError("FogOfWarManager: Fog Tilemap is not assigned.");
            }

            if (fogTile == null)
            {
                Debug.LogError("FogOfWarManager: Fog Tile is not assigned.");
            }
        }

        public void UpdateFog(Vector2Int playerPosition)
        {
            if (fogTilemap == null || fogTile == null)
            {
                Debug.LogError(
                    "FogOfWarManager: Cannot update fog because fogTilemap or fogTile is not assigned."
                );
                return;
            }

            // Iterate over tiles within the vision radius and clear fog
            for (int x = playerPosition.x - visionRadius; x <= playerPosition.x + visionRadius; x++)
            {
                for (
                    int y = playerPosition.y - visionRadius;
                    y <= playerPosition.y + visionRadius;
                    y++
                )
                {
                    Vector2Int tileCoord = new Vector2Int(x, y);
                    if (Vector2Int.Distance(playerPosition, tileCoord) <= visionRadius)
                    {
                        fogTilemap.SetTile(new Vector3Int(x, y, 0), null); // Clear fog
                    }
                }
            }
        }

        public bool IsTileHidden(Vector2Int position)
        {
            if (fogTilemap == null)
            {
                Debug.LogError("FogOfWarManager: fogTilemap is not assigned.");
                return false;
            }

            return fogTilemap.HasTile(new Vector3Int(position.x, position.y, 0));
        }
    }
}
