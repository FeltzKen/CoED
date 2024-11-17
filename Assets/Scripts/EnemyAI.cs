// EnemyAI.cs
using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class EnemyAI : MonoBehaviour, IActor
    {
        public float Speed { get; private set; } = 1f; // Speed determining actions per turn
        public float ActionPoints { get; set; } = 0f;  // Accumulated action points

        private enum EnemyState { Patrol, Chase, Attack }
        private EnemyState currentState = EnemyState.Patrol;

        private EnemyStats enemyStats;
        private List<Vector3> patrolPoints = new List<Vector3>();
        private Vector3 patrolDestination;
        private Transform playerTransform;
        private System.Action lastAction;
        private bool isActionComplete = false;

        [SerializeField] private LayerMask obstacleLayer;

        private Rigidbody2D rb;

        public Vector3Int CurrentPosition { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("EnemyAI: Missing Rigidbody2D component. Disabling script.");
                enabled = false;
                return;
            }

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

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
            if (DungeonGenerator.Instance != null)
            {
                List<Vector3> points = DungeonGenerator.Instance.GetPatrolPointsForFloor(enemyStats.spawnFloor);
                SetPatrolPoints(points);
            }
            else
            {
                Debug.LogError("EnemyAI: DungeonGenerator instance not found.");
            }

            TurnManager.Instance.RegisterActor(this); // Register with TurnManager

            // Initialize enemy's grid position
            Vector3 enemyPosition = transform.position;
            CurrentPosition = new Vector3Int(Mathf.RoundToInt(enemyPosition.x), Mathf.RoundToInt(enemyPosition.y), 0);
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
            isActionComplete = false; // Reset action completion flag
            Debug.Log($"EnemyAI: Acting in state {currentState}");

            // Plan action based on current state
            switch (currentState)
            {
                case EnemyState.Patrol:
                    PlanPatrol();
                    break;
                case EnemyState.Chase:
                    PlanChasePlayer();
                    break;
                case EnemyState.Attack:
                    PlanAttackPlayer();
                    break;
            }

            // Execute the planned action
            lastAction?.Invoke();
            lastAction = null;
        }

        private void PlanPatrol()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentDetectionRange)
            {
                currentState = EnemyState.Chase;
                PlanChasePlayer();
                return;
            }

            PlanMoveTowardsDestination(patrolDestination);

            if (Vector3Int.RoundToInt(transform.position) == Vector3Int.RoundToInt(patrolDestination))
            {
                SetNewPatrolDestination();
            }
        }

        private void PlanChasePlayer()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > enemyStats.CurrentDetectionRange)
            {
                currentState = EnemyState.Patrol;
                PlanPatrol();
                return;
            }

            if (Vector3.Distance(transform.position, playerTransform.position) <= enemyStats.CurrentProjectileRange)
            {
                currentState = EnemyState.Attack;
                PlanAttackPlayer();
                return;
            }

            PlanMoveTowardsDestination(playerTransform.position);
        }

        private void PlanAttackPlayer()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > enemyStats.CurrentProjectileRange)
            {
                currentState = EnemyState.Chase;
                PlanChasePlayer();
                return;
            }

            lastAction = () =>
            {
                Debug.Log("EnemyAI: Attacking the player!");
                PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.TakeDamage(enemyStats.CurrentAttack);
                }
                else
                {
                    Debug.LogError("EnemyAI: PlayerManager component not found on Player.");
                }
                isActionComplete = true; // Action is complete after attack
            };
        }

        private void PlanMoveTowardsDestination(Vector3 destination)
        {
            lastAction = () =>
            {
                Vector3Int currentTile = CurrentPosition;
                Vector3Int destinationTile = Vector3Int.RoundToInt(destination);
                Vector3Int direction = destinationTile - currentTile;

                // Normalize direction to a single step
                if (direction.x != 0) direction.x = direction.x > 0 ? 1 : -1;
                else if (direction.y != 0) direction.y = direction.y > 0 ? 1 : -1;

                Vector3Int targetTile = currentTile + new Vector3Int(direction.x, direction.y, 0);
                Collider2D hit = Physics2D.OverlapBox(new Vector2(targetTile.x, targetTile.y), Vector2.one * 0.9f, 0, obstacleLayer);

                if (TurnManager.Instance.IsTileReserved(targetTile))
                {
                    Debug.Log("EnemyAI: Target tile is reserved. Skipping movement.");
                    isActionComplete = true;
                    return;
                }

                if (hit != null)
                {
                    if (hit.CompareTag("Player"))
                    {
                        Debug.Log("EnemyAI: Collided with the player. Attacking.");
                        PlayerManager player = hit.GetComponent<PlayerManager>();
                        if (player != null)
                        {
                            player.TakeDamage(enemyStats.CurrentAttack);
                        }
                        else
                        {
                            Debug.LogError("EnemyAI: PlayerManager component not found on Player.");
                        }
                    }
                    else
                    {
                        Debug.Log("EnemyAI: Movement blocked by an obstacle.");
                    }
                    isActionComplete = true;
                    return;
                }

                if (TurnManager.Instance.ReserveTile(targetTile, this))
                {
                    // Release the current tile reservation
                    TurnManager.Instance.ReleaseTile(CurrentPosition);

                    rb.position = (Vector3)targetTile;
                    transform.position = rb.position; // Ensure synchronization
                    CurrentPosition = targetTile;
                    Debug.Log($"EnemyAI: Moved to tile {CurrentPosition}");
                }
                else
                {
                    Debug.Log("EnemyAI: Unable to reserve target tile. Skipping turn.");
                }
                isActionComplete = true;
            };
        }

        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Count > 0)
            {
                patrolDestination = patrolPoints[Random.Range(0, patrolPoints.Count)];
                Debug.Log($"{gameObject.name}: New patrol destination set to {patrolDestination}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: No patrol points available to set a new destination.");
                isActionComplete = true;
            }
        }

        /// <summary>
        /// Assigns patrol points to the enemy AI.
        /// </summary>
        /// <param name="points">List of Vector3 positions representing patrol points.</param>
        public void SetPatrolPoints(List<Vector3> points)
        {
            if (points == null || points.Count == 0)
            {
                Debug.LogWarning($"{gameObject.name}: No patrol points provided.");
                patrolPoints = new List<Vector3>(); // Ensure patrolPoints is not null
                isActionComplete = true; // Optionally, set to complete to prevent idle states
                return;
            }

            patrolPoints = points;
            InitializePatrol();
        }

        /// <summary>
        /// Initializes patrol behavior after patrol points are set.
        /// </summary>
        private void InitializePatrol()
        {
            SetNewPatrolDestination();
            // Additional initialization logic can be added here if needed
        }

        /// <summary>
        /// Updates the enemy's floor and assigns new patrol points accordingly.
        /// </summary>
        /// <param name="newFloorNumber">The new floor number the enemy has moved to.</param>
        public void UpdateFloor(int newFloorNumber)
        {
            enemyStats.spawnFloor = newFloorNumber;
            if (DungeonGenerator.Instance != null)
            {
                List<Vector3> newPatrolPoints = DungeonGenerator.Instance.GetPatrolPointsForFloor(newFloorNumber);
                SetPatrolPoints(newPatrolPoints);
            }
            else
            {
                Debug.LogError("EnemyAI: DungeonGenerator instance not found.");
            }
        }

        public void ClearLastAction()
        {
            lastAction = null;
            Debug.Log("EnemyAI: Cleared the last planned action.");
        }
    }
}
