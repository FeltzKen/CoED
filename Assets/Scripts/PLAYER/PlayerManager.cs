// PlayerManager.cs
using UnityEngine;
using System.Collections;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class PlayerManager : MonoBehaviour, IActor
    {
        public static PlayerManager Instance { get; private set; }
        public Vector3Int CurrentPosition { get; set; } // Player's current grid position

        private PlayerStats playerStats;
        private PlayerCombat playerCombat;
        private PlayerMagic playerMagic;
        private TurnManager turnManager;
        public Rigidbody2D rb;

        private System.Action lastAction; // Stores the last planned action
        private bool isActionComplete = false;

        [SerializeField] public LayerMask obstacleLayer; // Layer for obstacles

        public float Speed { get; private set; } = 5f; // Player speed
        public float ActionPoints { get; set; } = 0f;   // Accumulated action points

        private bool actionSelected = false;

        private void Awake()
        {
            // Singleton pattern to ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerManager: Duplicate instance detected. Destroying.");
                return;
            }

            // Get references to required components
            playerStats = GetComponent<PlayerStats>();
            playerCombat = GetComponent<PlayerCombat>();
            playerMagic = GetComponent<PlayerMagic>();
            turnManager = TurnManager.Instance;
            rb = GetComponent<Rigidbody2D>();

            ValidateComponents();
        }

        private void Start()
        {
            // Initialize player's grid position
            Vector3 playerPosition = transform.position;
            CurrentPosition = new Vector3Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y), 0);

            // Configure Rigidbody2D for kinematic movement
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            turnManager.RegisterActor(this);
            Debug.Log("PlayerManager: Registered actor with TurnManager.");
        }

        private void ValidateComponents()
        {
            // Ensure all necessary components are present
            if (playerStats == null || playerCombat == null || playerMagic == null)
            {
                Debug.LogError("PlayerManager: Missing required components. Disabling script.");
                enabled = false;
            }
        }

        public void PerformAction()
        {
            Act();
        }

        public bool IsActionComplete()
        {
            return isActionComplete;
        }

        public void CommitMoveAction(Vector2Int targetDirection)
        {
            // Calculate the target tile
            Vector3Int targetTile = CurrentPosition + new Vector3Int(targetDirection.x, targetDirection.y, 0);
            Collider2D hit = Physics2D.OverlapBox(new Vector2(targetTile.x, targetTile.y), Vector2.one * 0.9f, 0, obstacleLayer);

            // Check if the tile is reserved
            if (TurnManager.Instance.IsTileReserved(targetTile))
            {
                Debug.Log("PlayerManager: Target tile is reserved. Movement blocked.");
                isActionComplete = true; // Skip the turn
                return;
            }

            if (hit != null)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Debug.Log("PlayerManager: Collided with an enemy. Triggering attack.");
                    // Trigger attack on enemy
                    EnemyStats enemy = hit.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(playerStats.CurrentAttack);
                    }
                }
                else
                {
                    Debug.Log("PlayerManager: Movement blocked by an obstacle.");
                }

                isActionComplete = true; // End turn even if movement fails
                return;
            }

            // Reserve the target tile
            if (TurnManager.Instance.ReserveTile(targetTile, this))
            {
                // Release the current tile reservation
                TurnManager.Instance.ReleaseTile(CurrentPosition);

                // Snap to the target position
                rb.position = (Vector3)targetTile;       // Update Rigidbody position
                transform.position = rb.position;        // Synchronize the transform

                // Update the current tile position
                CurrentPosition = targetTile;
                Debug.Log($"PlayerManager: Moved to {CurrentPosition}");

                isActionComplete = true; // Movement is complete
            }
            else
            {
                Debug.Log("PlayerManager: Unable to reserve target tile. Skipping turn.");
                isActionComplete = true; // Skip the turn
            }
        }

        public void CommitCombatAction(bool isMelee, Vector3 targetPosition)
        {
            // Plan a combat action to be executed during the turn
            lastAction = () =>
            {
                if (isMelee)
                {
                    playerCombat.PerformMeleeAttack();
                }
                else
                {
                    playerCombat.AttemptRangedAttack(targetPosition);
                }
                isActionComplete = true;
            };
            actionSelected = true; // Action has been selected
        }

        public void CommitMagicAction(Vector3 targetPosition, int spellCost, int spellDamage)
        {
            // Plan a magic action to be executed during the turn
            lastAction = () =>
            {
                if (playerMagic.HasEnoughMagic(spellCost))
                {
                    playerMagic.CastMagicAction(targetPosition, spellCost, spellDamage);
                }
                isActionComplete = true;
            };
            actionSelected = true; // Action has been selected
        }

        public void Act()
        {
            isActionComplete = false; // Reset action completion flag
            actionSelected = false;   // Reset action selected flag

            Debug.Log("PlayerManager: It's the player's turn.");

            // Start the coroutine to handle the player's turn
            StartCoroutine(HandlePlayerTurn());
        }

        private IEnumerator HandlePlayerTurn()
        {
            // Show UI or prompt for player action
            ShowPlayerActionUI();

            // Wait until the player has selected an action
            yield return new WaitUntil(() => actionSelected);

            if (lastAction != null)
            {
                // Execute the planned action
                lastAction.Invoke();
                lastAction = null;
                Debug.Log("PlayerManager: Executed planned action.");
                // isActionComplete is set within the action
            }
            else
            {
                Debug.Log("PlayerManager: No action to perform.");
                isActionComplete = true; // No action to perform
            }
        }

        private void ShowPlayerActionUI()
        {
            // Implement UI display logic here
            // For example, enable action buttons or wait for player input
        }

        public void UpdateCurrentTilePosition() // DO NOT REMOVE!!!!
        {
            Vector3 position = rb.position;
            CurrentPosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
            Debug.Log($"PlayerManager: Updated current position to {CurrentPosition}");
        }

        public void CommitSpecialAction(System.Action specialAction) // DO NOT REMOVE!!!!
        {
            lastAction = specialAction;
            Debug.Log("PlayerManager: Registered special action.");
        }

        public void TakeDamage(int damage)
        {
            // Apply damage to the player
            playerStats.TakeDamage(damage);
        }

        public void ClearLastAction()
        {
            lastAction = null;
            Debug.Log("PlayerManager: Cleared the last planned action.");
        }
    }
}
