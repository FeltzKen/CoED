using UnityEngine;
using System.Collections;

using YourGameNamespace;

namespace YourGameNamespace
{
    public class PlayerManager : MonoBehaviour, IActor
    {
        public static PlayerManager Instance { get; private set; }
        public Vector3Int CurrentPosition { get; private set; } // Player's current grid position

        private PlayerStats playerStats;
        private PlayerCombat playerCombat;
        private PlayerMagic playerMagic;
        private TurnManager turnManager;
        private Rigidbody2D rb;

        private System.Action lastAction; // Stores the last planned action
        private bool isActionComplete = false;

        // Declared variables
        private Vector2 targetPosition;
        private bool isMoving = false;
        [SerializeField] private LayerMask obstacleLayer; // Layer for obstacles

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
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            turnManager.RegisterActor(this);
            Debug.Log("PlayerManager: Registered actor with TurnManager.");
        }

        private void FixedUpdate()
        {
            // Handle movement during the physics update cycle
            if (isMoving)
            {
                rb.MovePosition(targetPosition);
                isMoving = false;

                isActionComplete = true; // Movement is complete
            }
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

        public void CommitMoveAction(Vector2Int targetDirection)
        {
            // Calculate the target position
            Vector2 currentPos = rb.position;
            targetPosition = currentPos + (Vector2)targetDirection;

            // Check for obstacles before moving
            if (!Physics2D.OverlapBox(targetPosition, Vector2.one * 0.9f, 0, obstacleLayer))
            {
                isMoving = true;
                CurrentPosition = new Vector3Int(Mathf.RoundToInt(targetPosition.x), Mathf.RoundToInt(targetPosition.y), 0);
                Debug.Log($"PlayerManager: Committed move action to {CurrentPosition}");
                actionSelected = true; // Action has been selected
            }
            else
            {
                Debug.Log("Move blocked by obstacle.");
                isActionComplete = true; // No movement possible, action is complete
                actionSelected = true; // Action selection is complete
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
            };
            actionSelected = true; // Action has been selected
        }

        public void Act()
        {
            isActionComplete = false; // Reset action completion flag
            actionSelected = false; // Reset action selected flag

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

            if (isMoving)
            {
                // Movement will be processed in FixedUpdate
                Debug.Log("PlayerManager: Movement action will be executed in FixedUpdate.");
                // isActionComplete will be set to true after movement is complete
            }
            else if (lastAction != null)
            {
                // Execute the planned action
                lastAction.Invoke();
                lastAction = null;
                Debug.Log("PlayerManager: Executed planned action.");
                isActionComplete = true; // Action is complete
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
                public void UpdateCurrentTilePosition()// DO NOT REMOVE!!!!
        {
            Vector3 position = transform.position;
            CurrentPosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
            Debug.Log($"PlayerManager: Updated current position to {CurrentPosition}");
        }
        public void CommitSpecialAction(System.Action specialAction)// DO NOT REMOVE!!!!
        {
            lastAction = specialAction;
            Debug.Log("PlayerManager: Registered action with TurnManager.");
        }
        public void TakeDamage(int damage)
        {
            // Apply damage to the player
            playerStats.TakeDamage(damage);
        }

        public bool IsActionComplete()
        {
            return isActionComplete;
        }
    }
}
