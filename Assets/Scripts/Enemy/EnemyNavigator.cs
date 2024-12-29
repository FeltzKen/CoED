using System.Collections.Generic;
using CoED.Pathfinding;
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

        private float moveDelay; // Delay between movements
        private float moveCooldownTimer = 0f;

        private List<Vector2Int> currentPath;
        private Vector2Int currentGridPos;
        private Vector2Int destination;

        private int retryCount = 0;
        private const int maxRetries = 3;

        public int occupantID { get; private set; } = -1;

        public void Initialize(Pathfinder pathfinder, Tilemap floorTilemap, int occupantID)
        {
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
            destination = newDestination;
            currentPath = pathfinder.FindPath(currentGridPos, destination);

            retryCount = 0; // Reset retries whenever a new destination is set
        }

        public void SetMoveSpeed(float newSpeed)
        {
            moveDelay = newSpeed;
        }

        public bool HasPath()
        {
            return currentPath != null && currentPath.Count > 0;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        private void Start()
        {
            Vector3Int cellPos = floorTilemap.WorldToCell(transform.position);
            currentGridPos = new Vector2Int(cellPos.x, cellPos.y);
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
        }

        private Vector2Int FindAlternativeDestination()
        {
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
        }
    }
}
