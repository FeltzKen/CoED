using System.Collections.Generic;
using CoED.Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public class EnemyNavigator : MonoBehaviour
    {
        [SerializeField]
        private Tilemap floorTilemap;

        private Pathfinder pathfinder;
        private Rigidbody2D rb;

        private float moveDelay; // Delay between movements
        private float moveCooldownTimer = 0f;

        private List<Vector2Int> currentPath;
        public Vector2Int currentGridPos;
        private Vector2Int destination;

        private int retryCount = 0;
        private const int maxRetries = 3;

        public int occupantID = 0;

        public void Initialize(Pathfinder pathfinder, Tilemap floorTilemap, int occupantID)
        {
            if (occupantID.ToString() == "0")
            {
                Debug.Log("Initializing enemy navigator");
                Debug.Log($"Pathfinder: {pathfinder}");
                Debug.Log($"Floor tilemap: {floorTilemap}");
                Debug.Log($"Occupant ID: {occupantID}");
            }
            this.pathfinder = pathfinder;
            this.floorTilemap = floorTilemap;
            this.occupantID = occupantID;
        }

        public void ClearPath()
        {
            currentPath = null;
        }

        public void SetDestination(Vector2Int newDestination)
        {
            if (occupantID.ToString() == "0")
            {
                Debug.Log($"Setting destination to {newDestination}");
                Debug.Log($"Current grid pos: {currentGridPos}");
                Debug.Log($"Valid pathfinder: {pathfinder != null}");
            }
            currentPath = pathfinder.FindPath(currentGridPos, newDestination);

            retryCount = 0; // Reset retries whenever a new destination is set
        }

        public void SetMoveSpeed(float newSpeed)
        {
            if (occupantID.ToString() == "0")
            {
                Debug.Log($"Setting move speed to {newSpeed}");
            }
            moveDelay = newSpeed;
        }

        public bool HasPath()
        {
            if (occupantID.ToString() == "0")
            {
                Debug.Log(
                    $"Checking if path exists: {currentPath != null && currentPath.Count > 0}"
                );
            }
            return currentPath != null && currentPath.Count > 0;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void Start()
        {
            if (occupantID.ToString() == "0")
            {
                Debug.Log("Starting enemy navigator");
                Debug.Log($"floorTilemap: {floorTilemap.name}"); // this is failing!!!
            }
            Vector3Int cellPos = floorTilemap.WorldToCell(transform.position);
            currentGridPos = new Vector2Int(cellPos.x, cellPos.y);
            if (occupantID.ToString() == "0")
            {
                Debug.Log($"Starting at {currentGridPos}");
            }
        }

        private void Update()
        {
            if (currentPath == null || currentPath.Count == 0)
                return;

            moveCooldownTimer -= Time.deltaTime;

            if (moveCooldownTimer <= 0f)
            {
                AttemptNextMove();
            }
        }

        private void AttemptNextMove()
        {
            if (currentPath == null || currentPath.Count == 0)
            {
                currentPath = null;
                return;
            }

            Vector2Int nextTile = currentPath[0];
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2Int playerGridPosition = new Vector2Int(
                (int)playerPosition.x,
                (int)playerPosition.y
            );
            if (nextTile == playerGridPosition)
            {
                return;
            }
            if (TileOccupancyManager.Instance.TryReserveTile(nextTile, occupantID))
            {
                TileOccupancyManager.Instance.ReleaseTile(currentGridPos, occupantID);
                currentGridPos = nextTile;
                currentPath.RemoveAt(0);

                MoveToTile(nextTile);
                moveCooldownTimer = moveDelay;
            }
            else
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    retryCount = 0;
                    Vector2Int alternativeDestination = FindAlternativeDestination();
                    currentPath = pathfinder.FindPath(currentGridPos, alternativeDestination);
                }
                else
                {
                    moveCooldownTimer = 0.2f; // Small delay before retrying
                }
            }
            if (occupantID.ToString() == "0")
            {
                Debug.Log("Attempting next move");
                Debug.Log($"Current grid pos: {currentGridPos}");
                Debug.Log($"Next tile: {nextTile}");
            }
        }

        private Vector2Int FindAlternativeDestination()
        {
            Debug.Log(
                $"Finding alternative destination for {currentGridPos} and {destination} for {occupantID}"
            );
            Vector2Int randomDirection = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
            return currentGridPos + randomDirection;
        }

        private void MoveToTile(Vector2Int targetTile)
        {
            Vector3 targetWorldPos =
                floorTilemap.CellToWorld(new Vector3Int(targetTile.x, targetTile.y, 0))
                + new Vector3(0.5f, 0.5f, 0);
            rb.position = targetWorldPos;
            transform.position = targetWorldPos;
            if (occupantID.ToString() == "0")
            {
                Debug.Log("Moving to tile");
                Debug.Log($"Target tile: {targetTile}");
                Debug.Log($"Target world pos: {targetWorldPos}");
            }
        }
    }
}
