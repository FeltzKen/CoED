using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float staminaCostPerRun = 2f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private float moveDelay = 0.2f;
        [SerializeField] private Slider staminaBar;

        public static PlayerMovement Instance { get; private set; }

        private PlayerStats playerStats;
        public Vector2Int currentTilePosition;
        private float moveCooldown;
        private Rigidbody2D rb;
        private TurnManager turnManager;
        private bool isActionComplete = false;
        private bool isMoving = false;    
        private Vector2 targetPosition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerMovement: Duplicate instance detected. Destroying.");
                return;
            }

            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("PlayerMovement: Missing Rigidbody2D component. Disabling script.");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            playerStats = PlayerStats.Instance;
            currentTilePosition = Vector2Int.RoundToInt(transform.position);
            turnManager = TurnManager.Instance;

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            turnManager.RegisterActor(PlayerManager.Instance);
            Debug.Log("PlayerMovement: Registered PlayerManager with TurnManager.");

            UpdateStaminaUI();
        }

        private void Update()
        {
            if (!isMoving)
            {
                HandleMovementInput();
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    isMoving = false;
                    isActionComplete = true;
                    Debug.Log("PlayerMovement: Movement action completed.");
                }
            }
        }


        public void HandlePlayerTurn()
        {
            isActionComplete = false;
            Debug.Log("PlayerMovement: Handling player turn for movement.");
        }

        private void HandleMovementInput()
        {
            if (Time.time < moveCooldown) return;

            Vector2Int direction = Vector2Int.zero;

            // Determine direction based on player input
            if (Input.GetKey(KeyCode.UpArrow)) direction += Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow)) direction += Vector2Int.down;
            if (Input.GetKey(KeyCode.LeftArrow)) direction += Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow)) direction += Vector2Int.right;

            // If a direction is chosen, check if the tile is walkable
            if (direction != Vector2Int.zero)
            {
                if (IsMovePossible(direction))
                {
                    // Update current tile position and move the player
                    currentTilePosition += direction;
                    targetPosition = new Vector2(currentTilePosition.x, currentTilePosition.y);
                    rb.position = targetPosition; 
                    transform.position = targetPosition;
                    isMoving = true;
                    moveCooldown = Time.time + moveDelay;
                    Debug.Log($"PlayerMovement: Moving to {targetPosition}.");
                }
                else
                {
                    Debug.Log("PlayerMovement: Move blocked by an obstacle.");
                }
            }
        }

        private bool IsMovePossible(Vector2Int direction)
        {
            Vector2Int targetTile = currentTilePosition + direction;
            Vector2 targetPosition = new Vector2(targetTile.x, targetTile.y);
            Vector2 boxSize = Vector2.one * 0.8f;
            Collider2D hitCollider = Physics2D.OverlapBox(targetPosition, boxSize, 0f, obstacleLayer);

            Debug.DrawLine(targetPosition - boxSize * 0.5f, targetPosition + boxSize * 0.5f, Color.red, 0.5f);

            if (hitCollider != null)
            {
                Debug.Log($"PlayerMovement: Move blocked by {hitCollider.name} at {targetPosition}");
                return false;
            }

            return true;
        }

        public void UpdateCurrentTilePosition(Vector3 position) // DO NOT REMOVE!!!!
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            
            // Update the logical representation of the player's position
            currentTilePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            
            // Ensure the physical player position is also updated 
            // by setting Rigidbody and transform position directly
            rb.position = position; 
            transform.position = position;
        }


        private void UpdateStaminaUI()
        {
            if (staminaBar != null)
            {
                staminaBar.value = playerStats.CurrentStamina;
            }
        }

        private void DeductStamina(float amount)
        {
            playerStats.CurrentStamina = Mathf.Max(playerStats.CurrentStamina - amount, 0);
            UpdateStaminaUI();
            Debug.Log($"PlayerMovement: Deducted {amount} stamina. Current stamina: {playerStats.CurrentStamina}");
        }

        public bool IsActionComplete()
        {
            return isActionComplete && !isMoving;
        }
    }
}
