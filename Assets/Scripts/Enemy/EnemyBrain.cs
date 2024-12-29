using System.Collections.Generic;
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

        public EnemyState CurrentState = EnemyState.Patrol;

        private EnemyNavigator navigator;
        private EnemyStats enemyStats;

        private HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;

        private Transform playerTransform;

        private FloorData floorData;
        private float thinkCooldown = 0.5f;
        private float nextThinkTime = 0f;
        private int wanderRange = 5;
        public bool CanAttackPlayer = true;

        [SerializeField]
        private LayerMask visualObstructionLayer;

        public void Initialize(FloorData floorData, IEnumerable<Vector2Int> patrolPoints)
        {
            this.floorData = floorData;
            this.patrolPoints = new HashSet<Vector2Int>(patrolPoints);
        }

        private void Awake()
        {
            navigator = GetComponent<EnemyNavigator>();
            enemyStats = GetComponent<EnemyStats>();
        }

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }

            ChooseRandomPatrolDestination();
            navigator.SetMoveSpeed(1f / enemyStats.PatrolSpeed); // Delay for patrol movements
        }

        private void Update()
        {
            if (Time.time >= nextThinkTime)
            {
                DecideNextAction();
                nextThinkTime = Time.time + thinkCooldown;
            }
        }

        private void DecideNextAction()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool seesPlayer = HasLineOfSightToPlayer(distanceToPlayer);
            bool isPlayerSurrounded = TileOccupancyManager.Instance.IsPlayerSurroundedByEnemies();

            if (distanceToPlayer <= enemyStats.CurrentAttackRange)
            {
                ChangeState(EnemyState.Attack);
                transform.position = new Vector3(
                    Mathf.Floor(transform.position.x),
                    Mathf.Floor(transform.position.y),
                    0
                );
            }
            else if (
                distanceToPlayer > enemyStats.CurrentAttackRange
                && seesPlayer
                && isPlayerSurrounded
            )
            {
                ChangeState(EnemyState.WanderNearPlayer);
            }
            else if (seesPlayer)
            {
                ChangeState(EnemyState.Chase);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }

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
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                direction,
                distanceToPlayer,
                visualObstructionLayer
            );
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
                    navigator.SetMoveSpeed(1f / enemyStats.PatrolSpeed);
                    ChooseRandomPatrolDestination(); // Ensure new point is selected immediately
                    navigator.SetDestination(patrolDestination);
                    break;
                case EnemyState.Chase:
                    navigator.SetMoveSpeed(1f / enemyStats.ChaseSpeed);
                    navigator.ClearPath();
                    break;
                case EnemyState.Attack:
                    navigator.SetMoveSpeed(0); // Effectively stop movement
                    break;
            }
        }

        private void HandlePatrol()
        {
            if (!navigator.HasPath())
            {
                ChooseRandomPatrolDestination();
                navigator.SetDestination(patrolDestination);
            }
            else
            {
                if (
                    IsCongested(
                        new Vector2Int(
                            Mathf.RoundToInt(transform.position.x),
                            Mathf.RoundToInt(transform.position.y)
                        )
                    )
                )
                {
                    ChooseRandomPatrolDestination();
                    navigator.SetDestination(patrolDestination);
                }
            }
        }

        private bool IsCongested(Vector2Int position)
        {
            int congestionThreshold = 3;
            int nearbyEnemies = 0;

            foreach (var offset in TileOccupancyManager.adjacentOffsets)
            {
                Vector2Int neighbor = position + offset;
                if (!TileOccupancyManager.Instance.IsTileFree(neighbor))
                {
                    nearbyEnemies++;
                    if (nearbyEnemies >= congestionThreshold)
                        return true;
                }
            }

            return false;
        }

        private void HandleChase()
        {
            if (playerTransform != null)
            {
                Vector2Int playerPosGrid = Vector2Int.RoundToInt(
                    playerTransform.position
                        - DungeonManager.Instance.GetFloorTransform(enemyStats.spawnFloor).position
                );
                navigator.SetDestination(playerPosGrid);
            }
        }

        private void HandleAttack()
        {
            if (CanAttackPlayer)
            {
                PlayerStats.Instance.TakeDamage(enemyStats.CurrentAttack);
                CanAttackPlayer = false;
            }
        }

        private float orbitAngle = 0f;

        private void HandleWanderNearPlayer()
        {
            if (playerTransform == null)
                return;

            // Adjust orbit angle with slight randomness
            orbitAngle += Random.Range(-15f, 15f);

            float radius = 3f; // Consider making this configurable
            Vector2 playerPos = playerTransform.position;

            // Calculate the target position based on the orbit angle
            float angleRadians = orbitAngle * Mathf.Deg2Rad;
            float offsetX = radius * Mathf.Cos(angleRadians);
            float offsetY = radius * Mathf.Sin(angleRadians);

            Vector2 targetPosWorld = new Vector2(playerPos.x + offsetX, playerPos.y + offsetY);

            // Snap target position to grid
            Vector3Int targetCellPos = floorData.FloorTilemap.WorldToCell(targetPosWorld);
            Vector2Int targetTilePos = new Vector2Int(targetCellPos.x, targetCellPos.y);

            // Check for valid position and set destination
            if (DungeonSpawner.Instance.IsValidSpawnPosition(floorData, targetTilePos))
            {
                navigator.SetDestination(targetTilePos);
            }
            else
            {
                // Adjust orbit angle to find another position
                orbitAngle += 180f;
            }
        }

        private void ChooseRandomPatrolDestination()
        {
            if (patrolPoints.Count == 0)
            {
                Debug.LogWarning("[EnemyBrain] No patrol points available.");
                patrolDestination = Vector2Int.RoundToInt(transform.position);
                return;
            }

            int index = Random.Range(0, patrolPoints.Count);
            foreach (var pt in patrolPoints)
            {
                if (index == 0)
                {
                    patrolDestination = pt;
                    navigator.SetDestination(patrolDestination);

                    break;
                }
                index--;
            }
        }
    }
}
