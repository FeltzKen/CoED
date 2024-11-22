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
        public HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;
        private Transform playerTransform;
        private List<Vector2Int> pathToDestination;
        private bool isActionComplete = false;

        private Rigidbody2D rb;

        public Vector2Int CurrentPosition { get; private set; }

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
        private HashSet<Vector2Int> ConvertToVector2Int(HashSet<Vector3Int> vector3Set)
        {
            return new HashSet<Vector2Int>(vector3Set.Select(v3 => new Vector2Int(v3.x, v3.y)));
        }
        private void Start()
        {
             var floorData = DungeonManager.Instance.GetFloor(enemyStats.spawnFloor);
            if (floorData != null)
            {
                walkableTiles = ConvertToVector2Int(floorData.FloorTiles); // Convert before assignment
                SetPatrolPoints(floorData.PatrolPoints.Select(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y))));
            }
            else
            {
                Debug.LogError($"EnemyAI: FloorData not found for floor {enemyStats.spawnFloor}");
            }

            TurnManager.Instance.RegisterActor(this);
            CurrentPosition = Vector2Int.RoundToInt(transform.position);
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

        private void AttackBehavior()
        {
            if (Vector2.Distance(transform.position, playerTransform.position) > enemyStats.CurrentProjectileRange)
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

        private void FollowPathToDestination(Vector2Int destination)
        {
            if (pathToDestination == null || pathToDestination.Count == 0 || pathToDestination[^1] != destination)
            {
                pathToDestination = AStarPathfinding.FindPath(CurrentPosition, destination, walkableTiles);

                if (pathToDestination == null || pathToDestination.Count == 0)
                {
                    Debug.LogWarning("EnemyAI: No path found to destination.");
                    isActionComplete = true;
                    return;
                }
            }

            Vector2Int nextStep = pathToDestination[0];
            pathToDestination.RemoveAt(0);

            Vector2 nextPosition = new Vector2(nextStep.x, nextStep.y);
            rb.MovePosition(nextPosition);
            CurrentPosition = nextStep;

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
                patrolDestination = patrolPoints.ElementAt(Random.Range(0, patrolPoints.Count));
                Debug.Log($"EnemyAI: New patrol destination set to {patrolDestination}");
            }
            else
            {
                Debug.LogWarning("EnemyAI: No patrol points available.");
            }
        }


        public void SetPatrolPoints(IEnumerable<Vector2Int> points)
        {
            patrolPoints = new HashSet<Vector2Int>(points);
            Debug.Log($"EnemyAI set patrol points: {string.Join(", ", patrolPoints)}");
        }

        private Vector2Int RandomPatrolPoint()
        {
            int index = Random.Range(0, patrolPoints.Count);
            foreach (var point in patrolPoints)
            {
                if (index-- == 0)
                    return point;
            }
            return Vector2Int.zero;
        }
    }
}
