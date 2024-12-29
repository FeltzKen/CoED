using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public static class PositionHelper
    {
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

            Vector3 worldPosition =
                tilemap.CellToWorld(tilePosition) + tilemap.tileAnchor + customOffset;

            GameObject instance = Object.Instantiate(prefab, worldPosition, Quaternion.identity);
            if (parent != null)
            {
                instance.transform.SetParent(parent, true);
            }
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate prefab on tile at {worldPosition}.");
            }

            return instance;
        }
    }
}
