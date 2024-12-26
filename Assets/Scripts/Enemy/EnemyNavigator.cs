using System.Collections.Generic;
using CoED.Pathfinding; // for Pathfinder, PathfindingGrid
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyNavigator : MonoBehaviour
    {
        [SerializeField]
        private Tilemap floorTilemap;

        private Pathfinder pathfinder;
        private Rigidbody2D rb;

        // This is updated by EnemyBrain to either PatrolSpeed or ChaseSpeed
        private float currentSpeed = 1f;

        private List<Vector2Int> currentPath;
        private Vector2Int currentGridPos;
        private Vector2Int destination;
        public int occupantID { get; private set; } = -1; // Unique ID for tile reservation

        private int blockedCounter = 0;

        // We add a timer-based system for tile movement
        private float nextMoveTime = 0f; // When we can move again

        public void Initialize(Pathfinder pathfinder, Tilemap floorTilemap, int occupantID)
        {
            this.pathfinder = pathfinder;
            this.floorTilemap = floorTilemap;
            this.occupantID = occupantID;
        }

        /// <summary>
        /// Called by EnemyBrain when the AI chooses a new destination tile.
        /// We recalc the path from currentGridPos to that destination.
        /// </summary>
        public void SetDestination(Vector2Int newDestination)
        {
            destination = newDestination;
            currentPath = pathfinder.FindPath(currentGridPos, destination);
            blockedCounter = 0;
        }

        public void SetDestination(Vector2Int newDestination, bool forceRandomNearby = false)
        {
            if (forceRandomNearby)
            {
                // Randomize the destination slightly within a radius
                Vector2Int randomOffset = new Vector2Int(
                    Random.Range(-1, 2), // Offset X
                    Random.Range(-1, 2) // Offset Y
                );
                newDestination += randomOffset;
            }

            destination = newDestination;
            currentPath = pathfinder.FindPath(currentGridPos, destination);
            blockedCounter = 0;
        }

        /// <summary>
        /// Called by EnemyBrain whenever we switch states (Patrol vs. Chase),
        /// to update the enemy's tile-to-tile speed.
        /// </summary>
        public void SetMoveSpeed(float newSpeed)
        {
            currentSpeed = newSpeed;
        }

        /// <summary>
        /// Tells EnemyBrain whether we still have tiles left to move along,
        /// or if the path is empty/null.
        /// </summary>
        public bool HasPath()
        {
            return currentPath != null && currentPath.Count > 0;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Align to grid
            Vector3Int cellPos = floorTilemap.WorldToCell(transform.position);
            currentGridPos = new Vector2Int(cellPos.x, cellPos.y);

            // Attempt to reserve the tile we start on
            TileOccupancyManager.Instance.TryReserveTile(currentGridPos, occupantID);
        }

        /// <summary>
        /// Called every frame by EnemyBrain.Update().
        /// We do tile-based movement only if enough time has passed
        /// (based on the enemy's currentSpeed).
        /// </summary>
        public void UpdateNavigation()
        {
            // If we have no path or a near-zero speed, do nothing
            if (currentSpeed <= 0.01f)
                return;
            if (currentPath == null || currentPath.Count == 0)
                return;

            // Check if it's time to move again
            if (Time.time < nextMoveTime)
                return;

            // Attempt to move one tile along the path
            Vector2Int nextTile = currentPath[0];

            // Try to reserve that tile
            bool canMove =
                TileOccupancyManager.Instance.TryReserveTile(nextTile, occupantID)
                && !TileOccupancyManager.Instance.IsTileOccupiedByPlayer(nextTile);
            if (canMove)
            {
                // Successfully reserved => release the old tile
                TileOccupancyManager.Instance.ReleaseTile(currentGridPos, occupantID);

                // "Teleport" instantly to nextTile
                MoveToTile(nextTile);

                // Remove it from the path
                currentPath.RemoveAt(0);

                blockedCounter = 0;

                // Reset the cooldown
                float moveCooldown = 1f / currentSpeed;
                nextMoveTime = Time.time + moveCooldown;
            }
            else
            {
                // The tile is occupied => attempt re-path
                blockedCounter++;

                currentPath = pathfinder.FindPath(currentGridPos, destination);

                // If no path or stuck too long => clear path so Brain picks new destination
                if (currentPath == null || currentPath.Count == 0 || blockedCounter >= 3)
                {
                    currentPath = null;
                    blockedCounter = 0;
                }

                // Even if we can't move, let's set a small cooldown
                // so we don't spam re-path every single frame
                float rePathCooldown = 0.2f;
                nextMoveTime = Time.time + rePathCooldown;
            }
        }

        /// <summary>
        /// Moves instantly to the specified tile (no Lerp).
        /// </summary>
        private void MoveToTile(Vector2Int tile)
        {
            // Update our grid position
            currentGridPos = tile;

            // Convert grid coords to world center
            Vector3Int cell = new Vector3Int(tile.x, tile.y, 0);
            Vector3 worldPos = floorTilemap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0f);

            // Teleport
            rb.position = worldPos;
        }
    }
}
