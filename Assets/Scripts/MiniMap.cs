using UnityEngine;
using UnityEngine.Tilemaps;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class Minimap : MonoBehaviour
    {
        public static Minimap Instance { get; private set; }

        [Header("Minimap Settings")]
        [SerializeField]
        private Tilemap minimapTilemap;

        [SerializeField]
        private TileBase exploredTile;

        [SerializeField]
        private TileBase playerMarkerTile;

        [SerializeField]
        private Transform playerTransform;

        [SerializeField]
        private Vector2Int tileOffset = Vector2Int.zero;

        private Vector2Int lastPlayerPos;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("Minimap instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            if (minimapTilemap == null || exploredTile == null || playerMarkerTile == null)
            {
                Debug.LogError(
                    "Minimap: Tilemap, ExploredTile, or PlayerMarkerTile is not assigned."
                );
                enabled = false;
                return;
            }

            if (playerTransform == null)
            {
                playerTransform = Player.Instance?.transform;
                if (playerTransform == null)
                {
                    Debug.LogError(
                        "Minimap: Player not found. Please assign the playerTransform in the inspector or ensure the Player instance is correctly set."
                    );
                    enabled = false;
                    return;
                }
            }

            lastPlayerPos = Vector2Int.FloorToInt(playerTransform.position);
            UpdateMinimap();
        }

        private void Update()
        {
            if (playerTransform == null)
                return;

            Vector2Int currentPlayerPos = Vector2Int.FloorToInt(playerTransform.position);
            if (currentPlayerPos != lastPlayerPos)
            {
                ClearPlayerMarker(lastPlayerPos);
                UpdateMinimap();
                lastPlayerPos = currentPlayerPos;
            }
        }

        private void UpdateMinimap()
        {
            Vector3Int exploredPos = new Vector3Int(
                lastPlayerPos.x + tileOffset.x,
                lastPlayerPos.y + tileOffset.y,
                0
            );
            minimapTilemap.SetTile(exploredPos, exploredTile);

            Vector3Int playerTilePos = new Vector3Int(
                lastPlayerPos.x + tileOffset.x,
                lastPlayerPos.y + tileOffset.y,
                0
            );
            minimapTilemap.SetTile(playerTilePos, playerMarkerTile);
        }

        private void ClearPlayerMarker(Vector2Int previousPos)
        {
            Vector3Int prevTilePos = new Vector3Int(
                previousPos.x + tileOffset.x,
                previousPos.y + tileOffset.y,
                0
            );
            minimapTilemap.SetTile(prevTilePos, exploredTile);
        }
    }
}
