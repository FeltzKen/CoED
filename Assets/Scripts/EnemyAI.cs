using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace YourGameNamespace
{
    public class EnemyAI : MonoBehaviour, IActor
    {
        public float Speed { get; private set; } = 1f;
        public float ActionPoints { get; set; } = 0f;

        private enum EnemyState { Patrol, Chase, Attack }
        private EnemyState currentState = EnemyState.Patrol;

        private EnemyStats enemyStats;
        public HashSet<Vector2Int> patrolPoints {get; private set;} = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;
        private Transform playerTransform;
        private List<Vector2Int> pathToDestination;

        private Rigidbody2D rb;
        public Vector2Int CurrentPosition { get; private set; }
        [SerializeField] private LayerMask obstacleLayer;

        private HashSet<Vector2Int> walkableTiles;

        private float moveCooldown = 1f; // Time between free movements
        private float nextMoveTime = 0f; // Timestamp for next free movement

        private void Awake()
        {
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
            TurnManager.Instance.RegisterActor(this); // Still register with TurnManager for combat
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
                    // AttackBehavior will be handled via TurnManager
                    break;
            }
        }

        public void PerformAction()
        {
            // Only handle attack-related logic here
            if (currentState == EnemyState.Attack)
            {
                PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.TakeDamage(enemyStats.CurrentAttack);
                }

                Debug.Log("EnemyAI: Attacked the player!");
                ActionPoints = 0f; // Reset action points after attacking
            }
        }

        public bool IsActionComplete()
        {
            // Only mark action as complete for combat scenarios
            return currentState != EnemyState.Attack;
        }

        private void PatrolBehavior()
        {
            if (Vector2.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentDetectionRange)
            {
                currentState = EnemyState.Chase;
                return;
            }

            if (CurrentPosition == patrolDestination)
            {
                SetNewPatrolDestination();
            }

            FollowPathToDestination(patrolDestination);
        }

        private void ChaseBehavior()
        {
            if (Vector2.Distance(transform.position, playerTransform.position) > enemyStats.CurrentDetectionRange)
            {
                currentState = EnemyState.Patrol;
                return;
            }

            if (Vector2.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentProjectileRange)
            {
                currentState = EnemyState.Attack;
                return;
            }

            FollowPathToDestination(Vector2Int.RoundToInt(playerTransform.position));
        }

        private void FollowPathToDestination(Vector2Int destination)
        {
            Debug.Log("-------HELLO-------");
            if (pathToDestination == null || pathToDestination.Count == 0 || pathToDestination[^1] != destination)
            {
                pathToDestination = AStarPathfinding.FindPath(CurrentPosition, destination, walkableTiles);

                if (pathToDestination == null || pathToDestination.Count == 0)
                {
                    Debug.LogWarning($"EnemyAI: No path found from {CurrentPosition} to {destination}");
                    return;
                }

                Debug.Log($"EnemyAI: Path calculated with {pathToDestination.Count} steps. Path: {string.Join(" -> ", pathToDestination)}");
            }

            Vector2Int nextStep = pathToDestination[0];
            pathToDestination.RemoveAt(0);

            UpdateCurrentTilePosition(nextStep);

            if (pathToDestination.Count == 0)
            {
                Debug.Log("EnemyAI: Reached destination.");
            }
        }

        private void UpdateCurrentTilePosition(Vector2Int position)
        {
            // Snap the position to exact tile increments
            CurrentPosition = position;

            Vector3 newPosition = new Vector3(position.x, position.y, transform.position.z);

            // Update Rigidbody and Transform to ensure consistency
            rb.linearVelocity = Vector2.zero; // Stop any ongoing movement
            rb.position = newPosition;
            transform.position = newPosition;

            Debug.Log($"EnemyAI: Updated current position to {position}");
        }


        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Count > 0)
            {
                patrolDestination = patrolPoints.ElementAt(Random.Range(0, patrolPoints.Count));
                Debug.Log($"EnemyAI: New patrol destination set to {patrolDestination}");
            }
            else
            {
                Debug.LogWarning("EnemyAI: No patrol points available.");
            }
        }

        public void SetWalkableTiles(HashSet<Vector2Int> tiles)
        {
            walkableTiles = tiles;
        }

        public void SetPatrolPoints(IEnumerable<Vector2Int> points)
        {
            if (points == null)
            {
                Debug.LogError("EnemyAI: Patrol points cannot be null.");
                return;
            }

            // Clear existing points and set the new ones
            patrolPoints.Clear();
            foreach (var point in points)
            {
                patrolPoints.Add(point);
            }

            Debug.Log($"EnemyAI: Patrol points set. Total: {patrolPoints.Count}");
        }
    }
}
