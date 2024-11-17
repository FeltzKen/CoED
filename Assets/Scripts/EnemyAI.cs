using System.Collections.Generic;
using UnityEngine;

namespace YourGameNamespace
{
    public class EnemyAI : MonoBehaviour, IActor
    {
        public float Speed { get; private set; } = 1f;
        public float ActionPoints { get; set; } = 0f;

        private enum EnemyState { Patrol, Chase, Attack }
        private EnemyState currentState = EnemyState.Patrol;

        private EnemyStats enemyStats;
        private List<Vector3> patrolPoints = new List<Vector3>();
        private Vector3 patrolDestination;
        private Transform playerTransform;
        private List<Vector2Int> pathToDestination;
        private bool isActionComplete = false;

        private Rigidbody2D rb;

        public Vector3Int CurrentPosition { get; set; }

        [SerializeField] private LayerMask obstacleLayer;

        private HashSet<Vector2Int> walkableTiles;

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
            var floorData = DungeonManager.Instance.GetFloor(enemyStats.spawnFloor);
            if (floorData != null)
            {
                walkableTiles = floorData.FloorTiles; // Assign FloorTiles as walkable tiles
                SetPatrolPoints(floorData.PatrolPoints);
            }
            else
            {
                Debug.LogError($"EnemyAI: FloorData not found for floor {enemyStats.spawnFloor}");
            }

            TurnManager.Instance.RegisterActor(this);
            CurrentPosition = Vector3Int.RoundToInt(transform.position);
        }

        public void PerformAction()
        {
            Act();
        }

        public bool IsActionComplete()
        {
            return isActionComplete;
        }

        public void Act()
        {
            isActionComplete = false;

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

        private void PatrolBehavior()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentDetectionRange)
            {
                currentState = EnemyState.Chase;
                return;
            }

            if (Vector3Int.RoundToInt(transform.position) == Vector3Int.RoundToInt(patrolDestination))
            {
                SetNewPatrolDestination();
            }

            FollowPathToDestination(patrolDestination);
        }

        private void ChaseBehavior()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > enemyStats.CurrentDetectionRange)
            {
                currentState = EnemyState.Patrol;
                return;
            }

            if (Vector3.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentProjectileRange)
            {
                currentState = EnemyState.Attack;
                return;
            }

            FollowPathToDestination(playerTransform.position);
        }

        private void AttackBehavior()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > enemyStats.CurrentProjectileRange)
            {
                currentState = EnemyState.Chase;
                return;
            }

            Debug.Log("EnemyAI: Attacking the player!");
            PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(enemyStats.CurrentAttack);
            }
            isActionComplete = true;
        }

        private void FollowPathToDestination(Vector3 destination)
        {
            Vector2Int currentTile = Vector2Int.RoundToInt(transform.position);
            Vector2Int destinationTile = Vector2Int.RoundToInt(destination);

            // If no path exists or destination changed, recalculate path
            if (pathToDestination == null || pathToDestination.Count == 0 || pathToDestination[^1] != destinationTile)
            {
                pathToDestination = AStarPathfinding.FindPath(currentTile, destinationTile, walkableTiles);

                if (pathToDestination == null || pathToDestination.Count == 0)
                {
                    Debug.LogWarning("EnemyAI: No path found to destination.");
                    isActionComplete = true;
                    return;
                }
            }

            // Follow the next step in the path
            Vector2Int nextStep = pathToDestination[0];
            pathToDestination.RemoveAt(0);

            Vector3 nextPosition = new Vector3(nextStep.x, nextStep.y, transform.position.z);
            rb.MovePosition(nextPosition);
            transform.position = nextPosition;

            CurrentPosition = Vector3Int.RoundToInt(nextPosition);

            if (pathToDestination.Count == 0)
            {
                Debug.Log("EnemyAI: Reached destination.");
                isActionComplete = true;
            }
        }

        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Count > 0)
            {
                patrolDestination = patrolPoints[Random.Range(0, patrolPoints.Count)];
                Debug.Log($"EnemyAI: New patrol destination set to {patrolDestination}");
            }
            else
            {
                Debug.LogWarning("EnemyAI: No patrol points available.");
            }
        }

        public void SetPatrolPoints(List<Vector3> points)
        {
            patrolPoints = points;
            SetNewPatrolDestination();
        }
    }
}
