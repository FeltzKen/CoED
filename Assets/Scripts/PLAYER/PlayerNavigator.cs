using System.Collections.Generic;
using CoED.Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public class PlayerNavigator : MonoBehaviour
    {
        private Tilemap floorTilemap;
        private Pathfinder pathfinder;
        private Rigidbody2D rb;
        private List<Vector2Int> currentPath = new List<Vector2Int>();
        private Vector2Int currentGridPos;
        private float moveDelay = 0.3f;
        private float moveCooldownTimer = 0f;

        private PlayerMovement playerMovement; // Cached reference for efficient access

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerMovement = GetComponent<PlayerMovement>();

            if (playerMovement == null)
            {
                Debug.LogError("PlayerNavigator: PlayerMovement component is missing.");
                enabled = false;
                return;
            }

            // Initialize with Floor 0 (Spawning Room)
            var floorData = DungeonManager.Instance.GetFloorData(0);
            if (floorData != null && floorData.FloorTilemap != null)
            {
                floorTilemap = floorData.FloorTilemap;
                InitializePathfinder(floorTilemap);
                Debug.Log("PlayerNavigator: Initialized with spawning room floor tilemap.");
            }
            else
            {
                Debug.LogError("PlayerNavigator: Floor 0 data is missing or incomplete.");
                enabled = false;
            }
        }

        private void Update()
        {
            moveCooldownTimer += Time.deltaTime;

            // Handle mouse input for movement
            if (Input.GetMouseButtonDown(0) && !playerMovement.IsPointerOverSpecificUIElement())
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int targetGridPos = (Vector2Int)floorTilemap.WorldToCell(mouseWorldPos);

                if (playerMovement.IsMouseOverEnemy(mouseWorldPos))
                {
                    Debug.Log("PlayerNavigator: Cannot move to target - Enemy detected.");
                    return;
                }

                if (!IsTileWalkable(targetGridPos))
                {
                    // Find the nearest walkable tile
                    targetGridPos = FindNearestWalkableTile(targetGridPos);
                    if (targetGridPos == currentGridPos)
                    {
                        Debug.Log("PlayerNavigator: No valid nearby tile found.");
                        return;
                    }
                }

                FindPathToTarget(targetGridPos);
            }

            FollowPath();
        }

        private void FindPathToTarget(Vector2Int targetGridPos)
        {
            currentGridPos = (Vector2Int)floorTilemap.WorldToCell(rb.position);

            // Get the path
            currentPath = pathfinder.FindPath(currentGridPos, targetGridPos);

            // If no path was found, log a warning and set currentPath to an empty list
            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning("PlayerNavigator: No valid path found.");
                currentPath = new List<Vector2Int>(); // Prevents null reference in FollowPath.
            }
        }

        private void FollowPath()
        {
            if (currentPath == null || currentPath.Count == 0 || moveCooldownTimer < moveDelay)
                return;

            moveCooldownTimer = 0f;
            Vector2 nextPos = floorTilemap.CellToWorld((Vector3Int)currentPath[0]);
            currentPath.RemoveAt(0);

            // Update player position through PlayerMovement.
            playerMovement.UpdateCurrentTilePosition(
                new Vector3(nextPos.x + 0.5f, nextPos.y + 0.5f, 0)
            );
        }

        private bool IsTileWalkable(Vector2Int gridPosition)
        {
            // Check if the tile is walkable
            return DungeonManager
                .Instance.GetFloorData(PlayerStats.Instance.currentFloor)
                .FloorTiles.Contains(gridPosition);
        }

        private Vector2Int FindNearestWalkableTile(Vector2Int targetGridPos)
        {
            var floorData = DungeonManager.Instance.GetFloorData(PlayerStats.Instance.currentFloor);
            if (floorData == null)
            {
                Debug.LogError("PlayerNavigator: FloorData is null.");
                return currentGridPos;
            }

            // Find the nearest walkable tile using Manhattan distance
            Vector2Int nearestTile = currentGridPos;
            int shortestDistance = int.MaxValue;

            foreach (var tile in floorData.FloorTiles)
            {
                int distance =
                    Mathf.Abs(tile.x - targetGridPos.x) + Mathf.Abs(tile.y - targetGridPos.y);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }

            return nearestTile;
        }

        public void SetTilemap(Tilemap newTilemap)
        {
            floorTilemap = newTilemap;
            InitializePathfinder(floorTilemap);
        }

        public void CancelPath()
        {
            currentPath.Clear();
        }

        private void InitializePathfinder(Tilemap tilemap)
        {
            var tilePositions = new HashSet<Vector2Int>();
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    tilePositions.Add((Vector2Int)pos);
                }
            }
            pathfinder = new Pathfinder(new PathfindingGrid(tilePositions));
        }
    }
}
