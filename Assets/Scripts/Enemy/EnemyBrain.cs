using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
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
        private _EnemyStats enemyStats;

        // Patrol
        private HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>();
        private Vector2Int patrolDestination;

        // Player detection
        private Transform playerTransform;

        [SerializeField, Min(0.1f)]
        private float engageDistance; // The distance at which enemy tries to start the attack

        [SerializeField, Min(0.1f)]
        private float disengageDistance; // distance to revert to chase
        private Vector2 lastKnownPlayerPos;
        private bool hadLOSLastFrame = false;

        [SerializeField]
        public LayerMask visualObstructionLayer;

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

        public void Initialize(IEnumerable<Vector2Int> patrolPoints)
        {
            this.patrolPoints = new HashSet<Vector2Int>(patrolPoints);
        }

        private void Awake()
        {
            enemyStats = GetComponent<_EnemyStats>();
        }

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            if (navigator == null)
            {
                navigator = GetComponent<EnemyNavigator>();
            }
            if (navigator == null)
            {
                Debug.LogError("EnemyNavigator is missing from this enemy!", this);
                return;
            }

            // Basic patrol setup
            ChooseRandomPatrolDestination();
            navigator.SetMoveSpeed(1f / enemyStats.GetEnemyPatrolSpeed());

            // Initialize projectile timers
            foreach (var projectile in availableProjectiles)
            {
                projectileCooldownTimers[projectile] = 0f;
            }
            launchProjectileCooldown = projectileCooldownDuration;

            // Default engage distances based on CurrentAttackRange
            engageDistance = enemyStats.GetEnemyAttackRange() + 0.2f; // CHANGED: Slightly bigger
            disengageDistance = enemyStats.GetEnemyAttackRange() + 0.8f; // CHANGED: bigger gap
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
            return hit.collider == null || hit.collider.CompareTag("Player");
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
                    navigator.SetMoveSpeed(1f / enemyStats.GetEnemyPatrolSpeed());
                    ChooseRandomPatrolDestination();
                    navigator.SetDestination(patrolDestination);
                    break;

                case EnemyState.Chase:
                    navigator.SetMoveSpeed(1f / enemyStats.GetEnemyChaseSpeed());
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
            if (distanceToPlayer <= enemyStats.GetEnemyProjectileRange())
            {
                if (CanAttackPlayer)
                {
                    TryFireProjectile();
                    launchProjectileCooldown = projectileCooldownDuration;
                }
            }

            // Actually handle the "melee approach + attack"
            if (CanAttackPlayer)
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

        private void HandleAttack()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // If still outside actual melee range, keep "inching" forward
            if (distanceToPlayer > (enemyStats.GetEnemyAttackRange() - 0.1f))
            {
                // "Push into the crowd" at half chase speed
                navigator.SetMoveSpeed(1f / (enemyStats.GetEnemyChaseSpeed() * 2f));

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

        public void PerformMeleeAttack()
        {
            if (playerTransform == null)
                return;

            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance > enemyStats.GetEnemyAttackRange())
                return;

            PlayerStats playerStats = playerTransform.GetComponent<PlayerStats>();
            if (playerStats == null)
                return;

            // ✅ Calculate hit success
            bool hitSuccess = BattleCalculations.IsAttackSuccessful(
                enemyStats.GetEnemyAccuracy(),
                playerStats.GetCurrentEvasion()
            );

            if (!hitSuccess)
            {
                Debug.Log($"{gameObject.name} missed the attack on {playerStats.name}!");
                return;
            }

            // ✅ Determine if attack is critical
            bool isCritical = BattleCalculations.IsCriticalHit(enemyStats.GetEnemyDexterity());

            // ✅ Calculate physical damage
            float physicalDamage = BattleCalculations.CalculateDamage(
                enemyStats.GetEnemyAttack(),
                0, // No additional weapon power here (could be modified later)
                playerStats.GetCurrentDefense(),
                isCritical
            );

            // ✅ Calculate elemental damage separately
            Dictionary<DamageType, float> damageDealt = new Dictionary<DamageType, float>
            {
                {
                    DamageType.Physical,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Physical)
                        ? enemyStats.dynamicDamageTypes[DamageType.Physical]
                        : 0
                },
                {
                    DamageType.Fire,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Fire)
                        ? enemyStats.dynamicDamageTypes[DamageType.Fire]
                        : 0
                },
                {
                    DamageType.Poison,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Poison)
                        ? enemyStats.dynamicDamageTypes[DamageType.Poison]
                        : 0
                },
                {
                    DamageType.Ice,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Ice)
                        ? enemyStats.dynamicDamageTypes[DamageType.Ice]
                        : 0
                },
                {
                    DamageType.Lightning,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Lightning)
                        ? enemyStats.dynamicDamageTypes[DamageType.Lightning]
                        : 0
                },
                {
                    DamageType.Shadow,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Shadow)
                        ? enemyStats.dynamicDamageTypes[DamageType.Shadow]
                        : 0
                },
                {
                    DamageType.Arcane,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Arcane)
                        ? enemyStats.dynamicDamageTypes[DamageType.Arcane]
                        : 0
                },
                {
                    DamageType.Holy,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Holy)
                        ? enemyStats.dynamicDamageTypes[DamageType.Holy]
                        : 0
                },
                {
                    DamageType.Bleed,
                    enemyStats.dynamicDamageTypes.ContainsKey(DamageType.Bleed)
                        ? enemyStats.dynamicDamageTypes[DamageType.Bleed]
                        : 0
                },
            };

            // ✅ Apply status effects based on enemy's chance
            List<StatusEffectType> successfulEffects = new List<StatusEffectType>();
            foreach (var statusEffect in enemyStats.monsterData.inflictedStatusEffect)
            {
                if (
                    BattleCalculations.ApplyStatusEffect(
                        enemyStats.GetEnemyChanceToInflictStatusEffect(),
                        enemyStats.GetEnemyIntelligence()
                    )
                )
                {
                    successfulEffects.Add(statusEffect);
                }
            }

            // ✅ Send damage and effects to player
            DamageInfo damageInfo = new DamageInfo(damageDealt, successfulEffects);
            playerStats.TakeDamage(damageInfo);

            lastAttackTime = Time.time;

            Debug.Log(
                $"{gameObject.name} attacked {playerStats.name} for {physicalDamage} damage {(isCritical ? "(Critical Hit!)" : "")}."
            );
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
                    - DungeonManager.Instance.GetFloorTransform(enemyStats.spawnFloorLevel).position
            );
        }

        private Vector2Int PlayerGridPos()
        {
            return Vector2Int.RoundToInt(
                playerTransform.position
                    - DungeonManager.Instance.GetFloorTransform(enemyStats.spawnFloorLevel).position
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
