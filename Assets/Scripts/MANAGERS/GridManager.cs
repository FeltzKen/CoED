using UnityEngine;

namespace CoED
{
    public class _GridManager : MonoBehaviour
    {
        public static _GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField, Range(1, 100)]
        private int gridWidth = 100;

        [SerializeField, Range(1, 100)]
        private int gridHeight = 100;

        [SerializeField, Min(0.1f)]
        private float tileSize = 1f;

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
                Debug.LogWarning("GridManager instance already exists. Destroying duplicate.");
            }
        }

        public Vector3 GetTilePosition(int x, int y)
        {
            if (!IsValidTileCoordinates(x, y))
            {
                Debug.LogWarning($"GridManager: Tile coordinates ({x}, {y}) are out of bounds.");
                return Vector3.zero;
            }
            return new Vector3(x * tileSize, y * tileSize, 0f);
        }

        public Vector2Int GetTileCoordinates(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x / tileSize);
            int y = Mathf.FloorToInt(worldPosition.y / tileSize);
            return new Vector2Int(x, y);
        }

        private bool IsValidTileCoordinates(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float TileSize => tileSize;
    }
}
