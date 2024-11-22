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

        private PlayerStats playerStats;
        public Vector2Int currentTilePosition;
        private float moveCooldown;
        private PlayerManager playerManager;

        private void Start()
        {
            playerStats = PlayerStats.Instance;
            currentTilePosition = Vector2Int.RoundToInt(transform.position);
            playerManager = PlayerManager.Instance;

            UpdateStaminaUI();
        }

        private void Update()
        {
            HandleMovementInput();
        }

        private void HandleMovementInput()
        {
            if (Time.time < moveCooldown) return;

            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKey(KeyCode.UpArrow)) direction += Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow)) direction += Vector2Int.down;
            if (Input.GetKey(KeyCode.LeftArrow)) direction += Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow)) direction += Vector2Int.right;

            if (direction != Vector2Int.zero)
            {
                // Handle two-tile movement with left shift key
                bool isRunning = Input.GetKey(KeyCode.LeftShift) && playerStats.CurrentStamina >= staminaCostPerRun;
                Vector2Int targetDirection = isRunning ? direction * 2 : direction;

                // Check for collisions if moving two tiles, fallback to single tile if blocked
                if (isRunning && !IsTwoTileMovePossible(direction))
                {
                    targetDirection = direction;
                }

                if (IsMovePossible(targetDirection))
                {
                    if (isRunning)
                    {
                        DeductStamina(staminaCostPerRun);
                    }
                    playerManager.CommitMoveAction(targetDirection);
                    moveCooldown = Time.time + moveDelay;
                }
            }
        }

        private bool IsMovePossible(Vector2Int direction)
        {
            Vector2Int targetTile = currentTilePosition + direction;
            Vector2 targetPosition = new Vector2(targetTile.x, targetTile.y);
            return IsTileWalkable(targetPosition);
        }

        private bool IsTwoTileMovePossible(Vector2Int direction)
        {
            Vector2Int firstTile = currentTilePosition + direction;
            Vector2Int secondTile = firstTile + direction;

            // Check if both tiles are walkable
            return IsTileWalkable(new Vector2(firstTile.x, firstTile.y)) &&
                   IsTileWalkable(new Vector2(secondTile.x, secondTile.y));
        }


        private bool IsTileWalkable(Vector2 position)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(position, Vector2.one * 0.5f, 0f, obstacleLayer);
            return hitCollider == null;
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
    }
}

