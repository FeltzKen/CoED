using UnityEngine;
using UnityEngine.UI;
using CoED;

namespace CoED
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
      //  private TurnManager turnManager;
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

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

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
                   // Debug.Log("PlayerMovement: Movement action completed.");
                }
            }
        }


        public void HandlePlayerTurn()
        {
            isActionComplete = false;
           // Debug.Log("PlayerMovement: Handling player turn for movement.");
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

    // If a direction is chosen, check if the tile is walkable or contains an enemy
    if (direction != Vector2Int.zero)
    {
        Vector2Int newTilePosition = currentTilePosition + direction;
        Vector2 newPosition = new Vector2(newTilePosition.x, newTilePosition.y);

        // Check for collisions separately for each axis
        bool canMoveX = IsMovePossible(new Vector2Int(direction.x, 0));
        bool canMoveY = IsMovePossible(new Vector2Int(0, direction.y));
        bool canMoveDiagonal = IsMovePossible(direction);

        if (canMoveDiagonal && canMoveX && canMoveY)
        {
            // Move diagonally if both directions are possible
            currentTilePosition = newTilePosition;
            targetPosition = newPosition;
        }
        else if (canMoveX)
        {
            // Move horizontally if only the X direction is possible
            currentTilePosition += new Vector2Int(direction.x, 0);
            targetPosition = new Vector2(currentTilePosition.x, currentTilePosition.y);
        }
        else if (canMoveY)
        {
            // Move vertically if only the Y direction is possible
            currentTilePosition += new Vector2Int(0, direction.y);
            targetPosition = new Vector2(currentTilePosition.x, currentTilePosition.y);
        }

        if (canMoveX || canMoveY || (canMoveDiagonal && canMoveX && canMoveY))
        {
            if (IsEnemyAtPosition(newTilePosition))
            {
                // Perform attack if an enemy is at the target position
                PlayerCombat.Instance.PerformMeleeAttack(newTilePosition);
            }
            else
            {
                // Move to the target position
                rb.position = targetPosition;
                transform.position = targetPosition;
                isMoving = true;
                moveCooldown = Time.time + moveDelay;
                PlayerManager.Instance.ResetEnemyAttackFlags();
            }
        }
        else
        {
        }
    }
}

        private bool IsEnemyAtPosition(Vector2Int position)
        {
            Vector2 targetPosition = new Vector2(position.x, position.y);
            Collider2D hitCollider = Physics2D.OverlapBox(targetPosition, Vector2.one * 0.8f, 0f, LayerMask.GetMask("enemies"));

            return hitCollider != null;
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
            PlayerManager.Instance.ResetEnemyAttackFlags();
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
