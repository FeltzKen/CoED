using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class EnemyAI : MonoBehaviour, IActor
    {
        private enum EnemyState { Patrol, Chase, Attack }
        private EnemyState currentState = EnemyState.Patrol;

        [Header("Settings")]
        public float detectionRange = 5f;
        public float attackRange = 1.5f;
        public int damage = 10;
        public float CurrentHealth = 100;

        public float CurrentSpeed { get; set; } = 2f; // Default patrol speed
        public float CurrentDefense { get; set; } = 0f;
        public float patrolSpeed = 2f; // For StatusEffectManager reference

        private List<Vector3> patrolPoints;
        private Vector3 patrolDestination;
        private Transform playerTransform;
        private System.Action lastAction;
        private bool isActionComplete = false;

        [SerializeField] private LayerMask obstacleLayer;

        private Rigidbody2D rb;

        private Vector2 targetPosition;
        private bool isMoving = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("EnemyAI: Missing Rigidbody2D component. Disabling script.");
                enabled = false;
                return;
            }

            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (playerTransform == null)
            {
                Debug.LogError("EnemyAI: Player not found. Disabling script.");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            patrolPoints = DungeonGenerator.Instance.GetPatrolPointsForFloor(transform.position);
            SetNewPatrolDestination();
            TurnManager.Instance.RegisterActor(this); // Register with TurnManager
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                float step = CurrentSpeed * Time.fixedDeltaTime;
                Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, step);
                rb.MovePosition(newPosition);

                if ((Vector2)rb.position == targetPosition)
                {
                    isMoving = false;
                    isActionComplete = true; // Movement is complete
                }
            }
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

            // If not moving or attacking, action is complete
            if (!isMoving && currentState != EnemyState.Attack)
            {
                isActionComplete = true;
            }
        }

        private void PlanPatrol()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
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
            if (Vector3.Distance(transform.position, playerTransform.position) > detectionRange)
            {
                currentState = EnemyState.Patrol;
                PlanPatrol();
                return;
            }

            if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                currentState = EnemyState.Attack;
                PlanAttackPlayer();
                return;
            }

            PlanMoveTowardsDestination(playerTransform.position);
        }

        private void PlanAttackPlayer()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > attackRange)
            {
                currentState = EnemyState.Chase;
                PlanChasePlayer();
                return;
            }

            lastAction = () =>
            {
                Debug.Log("Enemy is attacking the player!");
                PlayerManager playerManager = playerTransform.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.TakeDamage(damage);
                }
                isActionComplete = true; // Action is complete after attack
            };
        }

        private void PlanMoveTowardsDestination(Vector3 destination)
        {
            lastAction = () =>
            {
                Vector3Int currentTile = Vector3Int.RoundToInt(transform.position);
                Vector3Int destinationTile = Vector3Int.RoundToInt(destination);
                Vector3Int direction = destinationTile - currentTile;

                // Normalize direction to a single step
                if (direction.x != 0) direction.x = direction.x > 0 ? 1 : -1;
                else if (direction.y != 0) direction.y = direction.y > 0 ? 1 : -1;

                Vector3Int targetTile = currentTile + new Vector3Int(direction.x, direction.y, 0);

                Collider2D hit = Physics2D.OverlapBox(new Vector2(targetTile.x, targetTile.y), Vector2.one * 0.9f, 0f, obstacleLayer);
                if (hit == null)
                {
                    targetPosition = new Vector2(targetTile.x, targetTile.y);
                    isMoving = true;
                    Debug.Log($"EnemyAI: Moving towards tile {targetTile}");
                }
                else
                {
                    Debug.Log("EnemyAI: Movement blocked by obstacle.");
                    isActionComplete = true; // Action is complete if movement is blocked
                }
            };
        }

        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Count > 0)
            {
                patrolDestination = patrolPoints[Random.Range(0, patrolPoints.Count)];
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                Debug.Log($"Enemy took {damage} damage! Current health: {CurrentHealth}");
            }
        }

        public bool IsActionComplete()
        {
            return isActionComplete;
        }

        private void Die()
        {
            Debug.Log("Enemy has died.");
            TurnManager.Instance.RemoveActor(this); // Remove from TurnManager
            Destroy(gameObject);
        }
    }
}
