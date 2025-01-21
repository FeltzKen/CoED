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

        [SerializeField]
        private List<Projectile> availableProjectiles = new List<Projectile>();
        public EnemyState CurrentState = EnemyState.Patrol;

        private EnemyNavigator navigator;
        private EnemyStats enemyStats;

        // Patrol
        private HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;

        // Player detection
        public Transform playerTransform;

        [SerializeField, Min(0.1f)]
        private float engageDistance; // The distance at which enemy tries to start the attack

        [SerializeField, Min(0.1f)]
        private float disengageDistance; // distance to revert to chase
        private Vector2 lastKnownPlayerPos;
        private bool hadLOSLastFrame = false;

        [SerializeField]
        private LayerMask visualObstructionLayer;

        // Timers & state
        private float thinkCooldown = 0.5f;
        private float nextThinkTime = 0f;
        private float wanderRange = 5; // For WanderNearPlayer

        // Attack logic
        public bool CanAttackPlayer = true;

        // NEW: We add a cooldown-based approach
        [SerializeField]
        private float meleeAttackCooldown = 1.5f; // Time between hits
        private float lastAttackTime = 0f;

        // Projectiles
        private Dictionary<Projectile, float> projectileCooldownTimers =
            new Dictionary<Projectile, float>();

        [SerializeField]
        private float projectileCooldownDuration = 15f;
        private float launchProjectileCooldown;

        private FloorData floorData;

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

            // Basic patrol setup
            ChooseRandomPatrolDestination();
            navigator.SetMoveSpeed(1f / enemyStats.PatrolSpeed);

            // Initialize projectile timers
            foreach (var projectile in availableProjectiles)
            {
                projectileCooldownTimers[projectile] = 0f;
            }
            launchProjectileCooldown = projectileCooldownDuration;

            // Default engage distances based on CurrentAttackRange
            engageDistance = enemyStats.CurrentAttackRange + 0.2f; // CHANGED: Slightly bigger
            disengageDistance = enemyStats.CurrentAttackRange + 0.8f; // CHANGED: bigger gap
        }

        private void Update()
        {
            UpdateProjectileCooldowns();
            launchProjectileCooldown -= Time.deltaTime;

            if (Time.time >= nextThinkTime)
            {
                DecideNextAction();
                nextThinkTime = Time.time + thinkCooldown;
            }
        }

        private void UpdateProjectileCooldowns()
        {
            List<Projectile> keys = new List<Projectile>(projectileCooldownTimers.Keys);
            foreach (var projectile in keys)
            {
                if (projectileCooldownTimers[projectile] > 0)
                {
                    projectileCooldownTimers[projectile] -= Time.deltaTime;
                }
            }
        }

        // ===============================
        // Main State Decision
        // ===============================
        private void DecideNextAction()
        {
            if (playerTransform == null)
                return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool seesPlayer = HasLineOfSightToPlayer(distanceToPlayer);

            switch (CurrentState)
            {
                case EnemyState.Patrol:
                    DecidePatrolState(distanceToPlayer, seesPlayer);
                    break;

                case EnemyState.Chase:
                    DecideChaseState(distanceToPlayer, seesPlayer);
                    break;

                case EnemyState.Attack:
                    DecideAttackState(distanceToPlayer, seesPlayer);
                    break;

                case EnemyState.WanderNearPlayer:
                    DecideWanderNearPlayerState(distanceToPlayer, seesPlayer);
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
            return (hit.collider == null || hit.collider.CompareTag("Player"));
        }

        // ===============================
        // Change State
        // ===============================
        private void ChangeState(EnemyState newState)
        {
            if (CurrentState == newState)
                return;
            CurrentState = newState;

            switch (newState)
            {
                case EnemyState.Patrol:
                    navigator.SetMoveSpeed(1f / enemyStats.PatrolSpeed);
                    ChooseRandomPatrolDestination();
                    navigator.SetDestination(patrolDestination);
                    break;

                case EnemyState.Chase:
                    navigator.SetMoveSpeed(1f / enemyStats.ChaseSpeed);
                    navigator.ClearPath();
                    break;

                case EnemyState.Attack:
                    // DO NOT forcibly set speed=0 here
                    // We'll let the attack logic handle movement
                    break;
            }
        }

        // ===============================
        // Individual State Logic
        // ===============================
        private void DecidePatrolState(float distanceToPlayer, bool seesPlayer)
        {
            // If the enemy sees the player, we transition to chase
            if (seesPlayer && distanceToPlayer > engageDistance)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // If we see the player AND they're within engage distance => Attack
            if (seesPlayer && distanceToPlayer <= engageDistance)
            {
                ChangeState(EnemyState.Attack);
                return;
            }

            // If neither, remain in patrol
            HandlePatrol();
        }

        private void DecideChaseState(float distanceToPlayer, bool seesPlayer)
        {
            // "Memory" for last line of sight
            if (seesPlayer)
            {
                lastKnownPlayerPos = playerTransform.position;
                hadLOSLastFrame = true;
            }
            else
            {
                // If we just lost LOS, move to last known position
                if (hadLOSLastFrame)
                {
                    Vector2Int lastKnownGrid = new Vector2Int(
                        Mathf.RoundToInt(lastKnownPlayerPos.x),
                        Mathf.RoundToInt(lastKnownPlayerPos.y)
                    );
                    navigator.SetDestination(lastKnownGrid);
                    hadLOSLastFrame = false;
                    return;
                }
                else
                {
                    // truly revert to Patrol if no LOS for consecutive frames
                    ChangeState(EnemyState.Patrol);
                    return;
                }
            }

            // If in melee "engage" range, switch to Attack
            if (distanceToPlayer <= engageDistance)
            {
                ChangeState(EnemyState.Attack);
                return;
            }

            // Otherwise keep chasing
            HandleChase();
        }

        private void DecideAttackState(float distanceToPlayer, bool seesPlayer)
        {
            if (!seesPlayer)
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (distanceToPlayer > disengageDistance)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // Also consider projectile logic
            if (
                distanceToPlayer <= enemyStats.ProjectileCurrentAttackRange
                && launchProjectileCooldown <= 0
            )
            {
                TryFireProjectile();
                launchProjectileCooldown = projectileCooldownDuration;
            }

            // Actually handle the "melee approach + attack"
            HandleAttack();
        }

        private void DecideWanderNearPlayerState(float distanceToPlayer, bool seesPlayer)
        {
            if (!seesPlayer)
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (distanceToPlayer <= engageDistance)
            {
                ChangeState(EnemyState.Attack);
                return;
            }

            bool isPlayerSurrounded = TileOccupancyManager.Instance.IsPlayerSurroundedByEnemies();
            if (!isPlayerSurrounded)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // remain in WanderNearPlayer, if that's your design
        }

        // ===============================
        // Movement + Attack Routines
        // ===============================
        private void HandlePatrol()
        {
            if (!navigator.HasPath())
            {
                ChooseRandomPatrolDestination();
                navigator.SetDestination(patrolDestination);
            }
            else
            {
                if (IsCongested(CurrentGridPos()))
                {
                    ChooseRandomPatrolDestination();
                    navigator.SetDestination(patrolDestination);
                }
            }
        }

        private void HandleChase()
        {
            if (playerTransform == null)
                return;

            // Attempt to path near the player's tile
            Vector2Int playerTile = PlayerGridPos();
            navigator.SetDestination(playerTile);
        }

        /// <summary>
        /// The big "swarm" logic: The enemy tries to move closer if not in range.
        /// Once in range, it attempts an attack (with a cooldown).
        /// </summary>
        private void HandleAttack()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // If still outside actual melee range, keep "inching" forward
            if (distanceToPlayer > (enemyStats.CurrentAttackRange - 0.1f))
            {
                // "Push into the crowd" at half chase speed
                navigator.SetMoveSpeed(1f / (enemyStats.ChaseSpeed * 2f));

                // Instead of going exactly to player's tile, we find a free adjacent tile if possible
                Vector2Int bestTile = FindFreeAdjacentTileNearPlayer();
                navigator.SetDestination(bestTile);
            }
            else
            {
                // We're truly in striking distance, so hold position
                navigator.SetMoveSpeed(0f);

                // Check cooldown for multiple attacks
                if (Time.time >= lastAttackTime + meleeAttackCooldown)
                {
                    PerformMeleeAttack();
                    lastAttackTime = Time.time;
                }
            }
        }

        /// <summary>
        /// Actually performs the melee attack damage logic.
        /// Previously you had if(CanAttackPlayer) { ... }
        /// but now we do a time-based approach.
        /// You could re-enable CanAttackPlayer if you want a one-and-done approach.
        /// </summary>
        private void PerformMeleeAttack()
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance <= enemyStats.CurrentAttackRange)
            {
                // Build damage
                Dictionary<DamageType, float> damageDealt = new Dictionary<DamageType, float>();
                foreach (var kvp in enemyStats.dynamicDamageTypes)
                {
                    damageDealt[kvp.Key] = kvp.Value;
                }

                // Build status
                List<StatusEffectType> successfulEffects = new List<StatusEffectType>();

                foreach (var effect in enemyStats.inflictedStatusEffects)
                {
                    if (Random.value < enemyStats.chanceToInflictStatusEffect)
                        successfulEffects.Add(effect);
                }

                DamageInfo damageInfo = new DamageInfo(damageDealt, successfulEffects);

                // Apply to player
                PlayerStats.Instance.TakeDamage(damageInfo);

                Debug.Log($"{name} attacked the player with {damageDealt.Count} damage types.");
                CanAttackPlayer = false;
            }
        }

        /// <summary>
        /// Attempts to find any free adjacent tile near the player.
        /// If all are blocked, returns the player's tile itself (which might be blocked).
        /// This ensures multiple enemies can gather around different tiles.
        /// </summary>
        private Vector2Int FindFreeAdjacentTileNearPlayer()
        {
            Vector2Int playerPos = PlayerGridPos();

            // Check the 8 surrounding offsets
            foreach (var offset in TileOccupancyManager.adjacentOffsets)
            {
                Vector2Int checkPos = playerPos + offset;
                if (TileOccupancyManager.Instance.IsTileFree(checkPos))
                {
                    return checkPos;
                }
            }

            // fallback
            return playerPos;
        }

        // ===============================
        // Utility
        // ===============================

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

        private Vector2Int CurrentGridPos()
        {
            return Vector2Int.RoundToInt(
                transform.position
                    - DungeonManager.Instance.GetFloorTransform(enemyStats.spawnFloor).position
            );
        }

        private Vector2Int PlayerGridPos()
        {
            return Vector2Int.RoundToInt(
                playerTransform.position
                    - DungeonManager.Instance.GetFloorTransform(enemyStats.spawnFloor).position
            );
        }

        private void TryFireProjectile()
        {
            Projectile selectedProjectile = SelectCooledDownProjectile();
            if (selectedProjectile != null)
            {
                FireProjectile(selectedProjectile);
            }
        }

        private Projectile SelectCooledDownProjectile()
        {
            var cooledDownProjectiles = availableProjectiles.FindAll(p =>
                projectileCooldownTimers[p] <= 0
            );
            if (cooledDownProjectiles.Count > 0)
            {
                return cooledDownProjectiles[Random.Range(0, cooledDownProjectiles.Count)];
            }
            return null;
        }

        private void FireProjectile(Projectile projectile)
        {
            // Instantiate the projectile's prefab
            GameObject projectileObject = Instantiate(
                projectile.projectilePrefab,
                new Vector3(transform.position.x - 0.25f, transform.position.y, 0),
                Quaternion.identity
            );

            // Get the ProjectileWrapper from the instantiated prefab
            var wrapperInstance = projectileObject.GetComponent<ProjectileWrapper>();
            if (wrapperInstance != null)
            {
                // Initialize the new wrapper with the BaseProjectile data
                wrapperInstance.Initialize(projectile);

                // Launch the projectile toward the player
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                wrapperInstance.Launch(direction);

                // We can remove this line if you only want to stop once after a shot
                // But if you want a single shot, you can do something else
                // or use the timer approach so you can shoot repeatedly
                CanAttackPlayer = false;
            }
            else
            {
                Debug.LogError("Projectile prefab is missing the ProjectileWrapper component.");
            }

            // Reset the cooldown
            projectileCooldownTimers[projectile] = projectile.cooldown;
        }
    }
}
