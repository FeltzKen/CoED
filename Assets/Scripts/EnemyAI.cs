using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using CoED.Pathfinding;
using System.Collections;
using NUnit.Framework;
namespace CoED
{
    public class EnemyAI : MonoBehaviour//, IActor
    {
        private static int nextID = 0; // Shared across all instances
        public float Speed { get; private set; } = 1f;
        public int uniqueID;
        public float ActionPoints { get; set; } = 0f;
        private Pathfinder pathfinding;
        private List<Vector2Int> currentPath;
        private int pathIndex;
        private Tilemap floorTilemap;

        public enum EnemyState { Patrol, Chase, Attack }
        public EnemyState currentState = EnemyState.Patrol;
        private EnemyStats enemyStats;
        public HashSet<Vector2Int> patrolPoints { get; private set; } = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;
        private Transform playerTransform;

        private Rigidbody2D rb;
        public Vector2Int CurrentPosition { get; private set; }
        [SerializeField] private LayerMask obstacleLayer;

        private float moveCooldown = .25f; // Time between free movements
        private float nextMoveTime = 0f; // Timestamp for next free movement

        private Vector2Int lastDirection = Vector2Int.zero;
        public bool CanAttackPlayer { get; set; } = true;        
        private bool isPaused = false;
        private float pauseDuration = 0.5f;
        public int SpawningFloor { get; set; } = 1;

        #region Setup
        private void Awake()
        {
            uniqueID = nextID++; // Generates a unique identifier
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("EnemyAI: Missing Rigidbody2D component. Disabling script.");
                enabled = false;
                return;
            }

            enemyStats = GetComponent<EnemyStats>();
            if (enemyStats == null)
            {
                Debug.LogError("EnemyAI: Missing EnemyStats component. Disabling script.");
                enabled = false;
                return;
            }
        }
        public void Initialize(FloorData floorData)
        {
            if (floorData != null)
            {
                floorTilemap = floorData.FloorTilemap;
            //    Debug.Log($"Enemy [ID: {uniqueID}] floorData accessed isfloor {floorData.FloorTiles}.");
                SpawningFloor = floorData.FloorNumber;
            }
            else
            {
                Debug.LogError("EnemyAI: FloorData is null.");
            }
        }

        private void Start()
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(SpawningFloor);
            if (floorData != null)
            {
                floorTilemap = floorData.FloorTilemap;
            }
            else
            {
                Debug.LogError("EnemyAI: FloorData is null.");
                return;
            }

            // Initialize CurrentPosition using tilemap grid coordinates
            Vector3Int cellPosition = floorTilemap.WorldToCell(transform.position);
            CurrentPosition = new Vector2Int(cellPosition.x, cellPosition.y);

            // Align the enemy's position to the grid
            Vector3 worldPosition = floorTilemap.CellToWorld(cellPosition);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

            currentState = EnemyState.Patrol;
            SetNewPatrolDestination();
        }


        private void Update()
        {
            // Ensure the player is assigned
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
                else
                {
                    // Player not found, wait until the next frame
                    return;
                }
            }

            // Handle free movement based on cooldown
            if (Time.time >= nextMoveTime)
            {
                Act();
                nextMoveTime = Time.time + moveCooldown;
            }
        }
        #endregion

        #region State Management
        private void Act()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Transition between states based on player distance
            if (distanceToPlayer <= enemyStats.CurrentAttackRange)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (distanceToPlayer <= enemyStats.CurrentDetectionRange)
            {
                ChangeState(EnemyState.Chase);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }

            // Execute behavior based on the current state
            switch (currentState)
            {
                case EnemyState.Patrol:
                    PatrolBehavior();
                    break;
                case EnemyState.Chase:
                    ChaseBehavior();
                    break;
                case EnemyState.Attack:
                    AttackBehavior();
                    break;
            }
        }

        private void ChangeState(EnemyState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
            }
        }

        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Count > 0)
            {
                patrolDestination = patrolPoints.ElementAt(Random.Range(0, patrolPoints.Count));
                // Loop until we find a valid and different destination (up to 10 tries)
                int attempts = 0;
                HashSet<Vector2Int> floorTiles = DungeonManager.Instance.GetFloorData(SpawningFloor).FloorTiles;
                while ((floorTiles.Contains(patrolDestination) || patrolDestination == CurrentPosition) && attempts < 10)
                {
                    patrolDestination = patrolPoints.ElementAt(Random.Range(0, patrolPoints.Count));
                    attempts++;
                }
            }
            else
            {
                Debug.LogWarning($"Enemy [ID: {uniqueID}]: No patrol points available.");
            }
        }
        #endregion

        #region Patrol Behavior
        private void PatrolBehavior()
        {
            MoveInPreferredDirection();
        }
        #endregion

        #region Chase Behavior
        private void ChaseBehavior()
        {
            MoveTowardsPlayer();
        }
        #endregion

        #region Attack Behavior
        private void AttackBehavior()
        {
            if (!CanAttackPlayer)
            {
                return;
            }
            PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(enemyStats.CurrentAttack);
            }
            CanAttackPlayer = false;
        }
        #endregion

        #region Movement
        private void MoveTowardsPlayer()
        {
            Vector2Int playerTilePosition = Vector2Int.RoundToInt(playerTransform.position);
            Vector2Int direction = new Vector2Int(
                playerTilePosition.x > CurrentPosition.x ? 1 : (playerTilePosition.x < CurrentPosition.x ? -1 : 0),
                playerTilePosition.y > CurrentPosition.y ? 1 : (playerTilePosition.y < CurrentPosition.y ? -1 : 0)
            );

            Vector2Int newPosition = CurrentPosition + direction;
            HashSet<Vector2Int> floorTiles = DungeonManager.Instance.GetFloorData(SpawningFloor).FloorTiles;
            if (floorTiles.Contains(newPosition) && !IsObstacle(newPosition))
            {
                UpdateCurrentTilePosition(newPosition);
            }
        }
        #endregion

        #region Movement
        private void MoveInPreferredDirection()
        {
            HashSet<Vector2Int> floorTiles = DungeonManager.Instance.GetFloorData(SpawningFloor).FloorTiles;    
            List<Vector2Int> possibleDirections = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }; // Possible movement directions

            // Prefer continuing in the last direction if possible
            Vector2Int randomDirection = Vector2Int.zero;

            if (lastDirection != Vector2Int.zero && Random.Range(0f, 1f) <= 0.75f)
            {
                Vector2Int preferredPosition = CurrentPosition + lastDirection;
                if (floorTiles.Contains(preferredPosition) && !IsObstacle(preferredPosition))
                {
                    randomDirection = lastDirection;
                }
            }

            if (randomDirection == Vector2Int.zero)
            {
                randomDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
            }

            Vector2Int newPosition = CurrentPosition + randomDirection;
            if (floorTiles.Contains(newPosition) && !IsObstacle(newPosition))
            {
                lastDirection = randomDirection;
                UpdateCurrentTilePosition(newPosition);
            }
        }

        private bool IsObstacle(Vector2Int position)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(new Vector2(position.x, position.y), Vector2.one * 0.8f, 0f, obstacleLayer);
            return hitCollider != null;
        }

        private void UpdateCurrentTilePosition(Vector2Int gridPosition)
        {

            CurrentPosition = gridPosition;
            // Convert the grid position back to world coordinates
            Vector3 worldPosition = floorTilemap.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
            worldPosition += new Vector3(-0.5f, -0.5f, 0);

            rb.position = new Vector2(worldPosition.x, worldPosition.y);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }
        #endregion

        #region Public Methods



        public void SetPatrolPoints(IEnumerable<Vector2Int> points)
        {
            if (points == null)
            {
                Debug.LogError($"Enemy [ID: {uniqueID}]: Patrol points cannot be null.");
                return;
            }

            patrolPoints = new HashSet<Vector2Int>(points);
        }
        private void OnDrawGizmos()
        {
            if (currentPath != null)
            {
                Gizmos.color = Color.blue;
                foreach (var position in currentPath)
                {
                    Vector3 worldPos = floorTilemap.CellToWorld(new Vector3Int(position.x, position.y, 0)) + new Vector3(0.5f, 0.5f, 0);
                    Gizmos.DrawCube(worldPos, Vector3.one * 0.5f);
                }
            }
        }
        #endregion

    }
}

