using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public static class PositionHelper
    {
        /// <summary>
        /// Centers a prefab on a target tile based on the tilemap's anchor.
        /// </summary>
        /// <param name="tilemap">The tilemap containing the target tile.</param>
        /// <param name="tilePosition">The tile's position in tile coordinates.</param>
        /// <param name="prefab">The prefab to instantiate and center.</param>
        /// <param name="parent">The parent transform for the instantiated prefab.</param>
        /// <param name="customOffset">Optional: Custom offset to apply to the prefab.</param>
        /// <returns>The instantiated and centered GameObject.</returns>
        public static GameObject InstantiateOnTile(
            Tilemap tilemap,
            Vector3Int tilePosition,
            GameObject prefab,
            Transform parent = null,
            Vector3 customOffset = default
        )
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null. Cannot instantiate on tile.");
                return null;
            }

            if (tilemap == null)
            {
                Debug.LogError("Tilemap is null. Cannot calculate position.");
                return null;
            }

            // Calculate the world position centered on the tile
            Vector3 worldPosition =
                tilemap.CellToWorld(tilePosition) + tilemap.tileAnchor + customOffset;

            // Instantiate the prefab at the calculated position
            GameObject instance = Object.Instantiate(prefab, worldPosition, Quaternion.identity);
            // Explicitly set the parent
            if (parent != null)
            {
                instance.transform.SetParent(parent, true); // World position stays intact
            }
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate prefab on tile at {worldPosition}.");
            }

            return instance;
        }

        /// <summary>
        /// Updates an existing object's position to align it with a tile's center.
        /// </summary>
        /// <param name="tilemap">The tilemap containing the target tile.</param>
        /// <param name="tilePosition">The tile's position in tile coordinates.</param>
        /// <param name="gameObject">The GameObject to move.</param>
        /// <param name="customOffset">Optional: Custom offset for the GameObject.</param>
        public static void AlignObjectToTile(
            Tilemap tilemap,
            Vector3Int tilePosition,
            GameObject gameObject,
            Vector3 customOffset = default
        )
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null. Cannot align to tile.");
                return;
            }

            if (tilemap == null)
            {
                Debug.LogError("Tilemap is null. Cannot calculate position.");
                return;
            }

            // Calculate the target position
            Vector3 worldPosition =
                tilemap.CellToWorld(tilePosition) + tilemap.tileAnchor + customOffset;

            // Update the GameObject's position
            gameObject.transform.position = worldPosition;
        }

        /// <summary>
        /// Gets the world position centered on the specified tile.
        /// </summary>
        /// <param name="tilemap">The tilemap containing the target tile.</param>
        /// <param name="tilePosition">The tile's position in tile coordinates.</param>
        /// <param name="customOffset">Optional: Custom offset to apply to the position.</param>
        /// <returns>The world position centered on the tile.</returns>
        public static Vector3 GetCenteredWorldPosition(
            Tilemap tilemap,
            Vector2Int tilePosition,
            Vector3 customOffset = default
        )
        {
            if (tilemap == null)
            {
                Debug.LogError("Tilemap is null. Cannot calculate position.");
                return Vector3.zero;
            }

            Vector3Int tilePosition3D = new Vector3Int(tilePosition.x, tilePosition.y, 0);
            return tilemap.CellToWorld(tilePosition3D) + tilemap.tileAnchor + customOffset;
        }

        /// <summary>
        /// Converts a world position to a tile position on the given tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to query.</param>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <returns>The tile position corresponding to the world position.</returns>
        public static Vector2Int WorldToTilePosition(Tilemap tilemap, Vector3 worldPosition)
        {
            if (tilemap == null)
            {
                Debug.LogError("Tilemap is null. Cannot calculate tile position.");
                return Vector2Int.zero;
            }

            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            return new Vector2Int(cellPosition.x, cellPosition.y);
        }
    }
}
