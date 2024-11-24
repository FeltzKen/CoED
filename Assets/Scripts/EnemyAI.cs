using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace YourGameNamespace
{
    public class EnemyAI : MonoBehaviour, IActor
    {
        private static int nextID = 0; // Shared across all instances
        public int uniqueID;
        public float Speed { get; private set; } = 1f;
        public float ActionPoints { get; set; } = 0f;

        public enum EnemyState { Patrol, Chase, Attack }
        public EnemyState currentState = EnemyState.Patrol;
        private EnemyStats enemyStats;
        public HashSet<Vector2Int> patrolPoints { get; private set; } = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;
        private Transform playerTransform;

        private Rigidbody2D rb;
        public Vector2Int CurrentPosition { get; private set; }
        [SerializeField] private LayerMask obstacleLayer;

        private HashSet<Vector2Int> walkableTiles;

        private float moveCooldown = .5f; // Time between free movements
        private float nextMoveTime = 0f; // Timestamp for next free movement

        private Vector2Int lastDirection = Vector2Int.zero;

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

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("EnemyAI: Player not found. Disabling script.");
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

        private void Start()
        {
            CurrentPosition = Vector2Int.RoundToInt(transform.position);
            transform.position = new Vector3(CurrentPosition.x, CurrentPosition.y, transform.position.z);

            currentState = EnemyState.Patrol; // Default to patrolling
            SetNewPatrolDestination();

            TurnManager.Instance.RegisterActor(this); // Register for combat
        }

        private void Update()
        {
            // Handle free movement based on cooldown
            if (Time.time >= nextMoveTime)
            {
                Act();
                nextMoveTime = Time.time + moveCooldown;
            }
        }

        public void Act()
        {
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

        public void ChangeState(EnemyState newState)
        {
            if (currentState != newState)
            {
                Debug.Log($"Enemy [ID: {uniqueID}] changing state from {currentState} to {newState}");
                currentState = newState;
            }
        }

        public void PerformAction()
        {
            if (currentState == EnemyState.Attack)
            {
                PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.TakeDamage(enemyStats.CurrentAttack);
                }
                ActionPoints = 0f; // Reset action points after attacking
            }
        }

        public bool IsActionComplete()
        {
            return currentState != EnemyState.Attack;
        }

        private void PatrolBehavior()
        {
            Debug.Log($"Enemy [ID: {uniqueID}] is patrolling.");

            if (Vector2.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentDetectionRange)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            if (!walkableTiles.Contains(patrolDestination) || CurrentPosition == patrolDestination)
            {
                SetNewPatrolDestination();
            }

            MoveInPreferredDirection();
        }

        private void AttackBehavior()
        {
            Debug.Log($"Enemy [ID: {uniqueID}] is attacking the player.");

            if (Vector2.Distance(transform.position, playerTransform.position) > enemyStats.CurrentProjectileRange)
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(enemyStats.CurrentAttack);
            }
        }

        private void ChaseBehavior() {
            Debug.Log($"Enemy [ID: {uniqueID}] is chasing the player.");
            Debug.Log($"Enemy [ID: {uniqueID}] is chasing the player.");

            if (Vector2.Distance(transform.position, playerTransform.position) > enemyStats.CurrentDetectionRange)
            {
                ChangeState(EnemyState.Patrol);
                SetNewPatrolDestination();
                return;
            }

            if (Vector2.Distance(transform.position, playerTransform.position) <= 1.0f) {
                ChangeState(EnemyState.Attack);
                return;
            }

            MoveTowardsPlayer(); // in chase state the enemy should move toward the player
        }

        private void MoveInPreferredDirection()
        {
            List<Vector2Int> possibleDirections = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }; // Possible movement directions

            // Prefer continuing in the last direction if possible
            Vector2Int randomDirection;
            if (lastDirection != Vector2Int.zero && Random.Range(0f, 1f) <= 0.75f && walkableTiles.Contains(CurrentPosition + lastDirection))
            {
                randomDirection = lastDirection;
            }
            else
            {
                randomDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
            }

            Vector2Int newPosition = CurrentPosition + randomDirection;

            if (walkableTiles.Contains(newPosition))
            {
                lastDirection = randomDirection;
                UpdateCurrentTilePosition(newPosition);
            }
        }

        private void MoveTowardsPlayer() {
            Debug.Log($"Enemy [ID: {uniqueID}] is moving towards the player.");
            Debug.Log($"Enemy [ID: {uniqueID}] is moving towards the player.");

            Vector2Int playerTilePosition = Vector2Int.RoundToInt(playerTransform.position);
            Vector2Int direction = new Vector2Int(
                playerTilePosition.x > CurrentPosition.x ? 1 : (playerTilePosition.x < CurrentPosition.x ? -1 : 0),
                playerTilePosition.y > CurrentPosition.y ? 1 : (playerTilePosition.y < CurrentPosition.y ? -1 : 0)
            );

            // Ensure the enemy moves adjacent to the player and stops if already adjacent
            if (Vector2.Distance((Vector2)CurrentPosition, (Vector2)playerTilePosition) <= 1.0f)
            {
                ChangeState(EnemyState.Attack);
                return;
            }
            Vector2Int newPosition = CurrentPosition + direction;

            if (walkableTiles.Contains(newPosition))
            {
                UpdateCurrentTilePosition(newPosition);
            }
        }

        private void UpdateCurrentTilePosition(Vector2Int position)
        {
            CurrentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

            rb.linearVelocity = Vector2.zero;
            rb.position = CurrentPosition;
            transform.position = new Vector3(CurrentPosition.x, CurrentPosition.y, 0);
        }

        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Count > 0)
            {
                patrolDestination = patrolPoints.ElementAt(Random.Range(0, patrolPoints.Count));

                if (!walkableTiles.Contains(patrolDestination))
                {
                    patrolDestination = CurrentPosition;
                }
            }
        }

        public void SetWalkableTiles(HashSet<Vector2Int> tiles)
        {
            walkableTiles = new HashSet<Vector2Int>(tiles);
        }

        private void RefreshWalkableTiles()
        {
            if (DungeonManager.Instance == null)
            {
                Debug.LogError($"Enemy [ID: {uniqueID}]: DungeonManager is null!");
                return;
            }

            walkableTiles = DungeonManager.Instance.GetWalkableTilesForFloor(enemyStats.spawnFloor);
        }

        public void SetPatrolPoints(IEnumerable<Vector2Int> points)
        {
            if (points == null)
            {
                Debug.LogError("Enemy [ID: {uniqueID}]: Patrol points cannot be null.");
                return;
            }

            patrolPoints.Clear();
            patrolPoints = new HashSet<Vector2Int>(points);
        }
    }
}
