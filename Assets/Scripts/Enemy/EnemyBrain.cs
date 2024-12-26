using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CoED
{
    [RequireComponent(typeof(EnemyNavigator))]
    [RequireComponent(typeof(EnemyStats))]
    public class EnemyBrain : MonoBehaviour
    {
        public enum EnemyState
        {
            Patrol,
            Chase,
            Attack,
            WanderNearPlayer,
        }

        public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

        private EnemyNavigator navigator;
        private EnemyStats enemyStats;

        // Patrol / detection logic
        private HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;

        private Transform playerTransform;
        private FloorData floorData;
        private float thinkCooldown = 0.5f; // how often we decide states
        private float nextThinkTime = 0f;
        private int WanderRadius = 5;
        public bool CanAttackPlayer = true;

        [SerializeField]
        private LayerMask visualObstructionLayer; // E.g. “Walls” or “Obstacles”

        /// <summary>
        /// Called from spawner or initialization code.
        /// We pass in references like floorData and patrolPoints.
        /// </summary>
        public void Initialize(FloorData floorData, IEnumerable<Vector2Int> patrolPoints)
        {
            this.floorData = floorData;
            this.patrolPoints = new HashSet<Vector2Int>(patrolPoints);

            ValidateStartingPosition();
        }

        private void ValidateStartingPosition()
        {
            Vector2Int currentPosition = Vector2Int.RoundToInt(transform.position);

            if (!DungeonSpawner.Instance.IsValidSpawnPosition(floorData, currentPosition))
            {
                //Debug.Log($"Invalid starting position for {gameObject.name}, relocating...");
                RepositionToValidTile();
            }
        }

        private void RepositionToValidTile()
        {
            if (patrolPoints == null || patrolPoints.Count == 0)
            {
                //Debug.LogError($"No patrol points available to relocate {gameObject.name}.");
                return;
            }

            // Choose a random valid patrol point
            Vector2Int newTile = new List<Vector2Int>(patrolPoints)[
                Random.Range(0, patrolPoints.Count)
            ];
            Vector3Int cellPos = new Vector3Int(newTile.x, newTile.y, 0);
            Vector3 newWorldPos = floorData.FloorTilemap.CellToWorld(cellPos); // + new Vector3(-0.5f, -0.5f, 0);
            //log enemy id and new position
            Debug.Log($"Relocating {navigator.occupantID} to {newWorldPos}");

            // Move the enemy
            transform.position = newWorldPos;
            GetComponent<Rigidbody2D>().position = newWorldPos;
            //ValidateStartingPosition();
        }

        private void Awake()
        {
            navigator = GetComponent<EnemyNavigator>();
            enemyStats = GetComponent<EnemyStats>();
        }

        private void Start()
        {
            // Acquire Player reference
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }

            // Pick an initial patrol destination
            ChooseRandomPatrolDestination();
            navigator.SetMoveSpeed(enemyStats.PatrolSpeed);
        }

        private void Update()
        {
            // Evaluate AI states at intervals
            if (Time.time >= nextThinkTime)
            {
                DecideNextAction();
                nextThinkTime = Time.time + thinkCooldown;
            }

            // Let the Navigator do continuous movement
            navigator.UpdateNavigation();
        }

        private void DecideNextAction()
        {
            if (playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool seesPlayer = HasLineOfSightToPlayer(distanceToPlayer);

            // Transition to appropriate state
            if (distanceToPlayer <= enemyStats.CurrentAttackRange)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (distanceToPlayer <= WanderRadius && seesPlayer)
            {
                ChangeState(EnemyState.WanderNearPlayer);
            }
            else if (seesPlayer && distanceToPlayer >= enemyStats.CurrentAttackRange)
            {
                ChangeState(EnemyState.Chase);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }

            // Handle logic based on the state
            switch (CurrentState)
            {
                case EnemyState.Chase:
                    HandleChase();
                    break;
                case EnemyState.Attack:
                    HandleAttack();
                    break;
                case EnemyState.Patrol:
                    HandlePatrol();
                    break;
                case EnemyState.WanderNearPlayer:
                    HandleWanderNearPlayer();
                    break;
            }
        }

        private bool HasLineOfSightToPlayer(float distanceToPlayer)
        {
            // Similar to your old code:
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                direction,
                distanceToPlayer,
                visualObstructionLayer
            );
            // If we didn’t hit anything or we hit the Player => we see the player
            return hit.collider == null || hit.collider.CompareTag("Player");
        }

        private void ChangeState(EnemyState newState)
        {
            if (CurrentState == newState)
                return;
            CurrentState = newState;

            switch (newState)
            {
                case EnemyState.Patrol:
                    navigator.SetMoveSpeed(enemyStats.PatrolSpeed);
                    break;
                case EnemyState.Chase:
                    navigator.SetMoveSpeed(enemyStats.ChaseSpeed);
                    break;
                case EnemyState.Attack:
                    // Potentially set speed = 0 or minimal
                    navigator.SetMoveSpeed(0.1f);
                    break;
            }
        }

        private void HandlePatrol()
        {
            // If we have no path or we reached our destination
            if (!navigator.HasPath())
            {
                ChooseRandomPatrolDestination();
                navigator.SetDestination(patrolDestination);
            }
        }

        private void HandleChase()
        {
            if (playerTransform != null)
            {
                // Always set the player's current position as the destination
                Vector2Int playerPosGrid = Vector2Int.RoundToInt(playerTransform.position);
                navigator.SetDestination(playerPosGrid);
            }
        }

        private void HandleAttack()
        {
            // Attack logic here, e.g.:
            if (CanAttackPlayer)
            {
                PlayerStats.Instance.TakeDamage(enemyStats.CurrentAttack);
                CanAttackPlayer = false;
            }
            // You can add a cooldown or animation triggers, etc.
        }

        private void HandleWanderNearPlayer()
        {
            if (playerTransform != null)
            {
                // Pick a random tile near the player within the wander radius
                Vector2Int playerPos = Vector2Int.RoundToInt(playerTransform.position);
                Vector2Int randomOffset = new Vector2Int(
                    Random.Range(-WanderRadius, WanderRadius + 1),
                    Random.Range(-WanderRadius, WanderRadius + 1)
                );
                Vector2Int wanderTile = playerPos + randomOffset;

                // Ensure the tile is valid
                if (DungeonSpawner.Instance.IsValidSpawnPosition(floorData, wanderTile))
                {
                    navigator.SetDestination(wanderTile, true);
                }
            }
        }

        private void ChooseRandomPatrolDestination()
        {
            if (patrolPoints.Count == 0)
                return;

            int index = Random.Range(0, patrolPoints.Count);
            foreach (var pt in patrolPoints)
            {
                if (index == 0)
                {
                    patrolDestination = pt;
                    break;
                }
                index--;
            }
        }
    }
}
