using System.Collections.Generic;
using CoED.Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float staminaCostPerRun = 2f;

        [Header("Movement Settings")]
        [SerializeField]
        private float moveDelay = 0.3f;

        [SerializeField]
        private float actionDelay = 0.5f;

        [SerializeField]
        private LayerMask obstacleLayer;

        [SerializeField]
        private Slider staminaBar;

        [SerializeField]
        private LayerMask enemyLayer;
        private Enemy enemy;
        private PlayerStats playerStats;
        public Vector2Int currentTilePosition;
        private Rigidbody2D rb;
        private bool isMoving = false;
        private Vector2 targetPosition;
        private bool isMouseHeld = false;
        private float moveCooldownTimer = 0f;
        private float actionCooldownTimer = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("PlayerMovement: Missing Rigidbody2D.");
                enabled = false;
                return;
            }
            enemy = FindAnyObjectByType<Enemy>();
        }

        private void Start()
        {
            playerStats = PlayerStats.Instance;
            currentTilePosition = Vector2Int.RoundToInt(transform.position);

            UpdateStaminaUI();
        }

        private void Update()
        {
            // Update timers
            if (moveCooldownTimer > 0)
                moveCooldownTimer -= Time.deltaTime;
            if (actionCooldownTimer > 0)
                actionCooldownTimer -= Time.deltaTime;

            HandleMovementInput();
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    isMoving = false;
                }
            }
        }

        private void HandleMovementInput()
        {
            if (moveCooldownTimer > 0 || isMoving)
                return;

            // Handle key-based movement
            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKey(KeyCode.UpArrow))
                direction += Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow))
                direction += Vector2Int.down;
            if (Input.GetKey(KeyCode.LeftArrow))
                direction += Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow))
                direction += Vector2Int.right;

            if (direction != Vector2.zero)
            {
                MoveInDirection(direction);
                GetComponent<PlayerNavigator>().CancelPath(); // Cancel ongoing pathfinding
                return;
            }

            // Handle mouse-based attack
            isMouseHeld = Input.GetMouseButton(0);
            if (
                isMouseHeld
                && IsMouseOverEnemy(Camera.main.ScreenToWorldPoint(Input.mousePosition))
                && actionCooldownTimer <= 0
            )
            {
                PerformContinuousAttack(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                actionCooldownTimer = actionDelay;
            }
        }

        private void MoveInDirection(Vector2Int direction)
        {
            Vector2Int newTilePosition = currentTilePosition + direction;
            Vector2 newPosition = new Vector2(newTilePosition.x, newTilePosition.y);
            if (IsEnemyAtPosition(newTilePosition) && actionCooldownTimer <= 0)
            {
                PlayerCombat.Instance.PerformMeleeAttack(newTilePosition);
                actionCooldownTimer = actionDelay; // Reset action cooldown timer
                return;
            }
            bool canMoveX = IsMovePossible(new Vector2Int(direction.x, 0));
            bool canMoveY = IsMovePossible(new Vector2Int(0, direction.y));
            bool canMoveDiagonal = IsMovePossible(direction);

            if (canMoveDiagonal)
            {
                currentTilePosition = newTilePosition;
                targetPosition = newPosition;
            }
            else if (canMoveX)
            {
                currentTilePosition += new Vector2Int(direction.x, 0);
                targetPosition = new Vector2(currentTilePosition.x, currentTilePosition.y);
            }
            else if (canMoveY)
            {
                currentTilePosition += new Vector2Int(0, direction.y);
                targetPosition = new Vector2(currentTilePosition.x, currentTilePosition.y);
            }

            if (canMoveX || canMoveY || canMoveDiagonal)
            {
                UpdateCurrentTilePosition(targetPosition);
                moveCooldownTimer = moveDelay; // Reset move cooldown
                isMoving = true;
                PlayerCombat.Instance.ResetEnemyAttackFlags();
            }
        }

        private bool IsMovePossible(Vector2Int direction)
        {
            Vector2Int targetTile = currentTilePosition + direction;
            Vector2 targetPosition = new Vector2(targetTile.x, targetTile.y);
            Vector2 boxSize = Vector2.one * 0.8f;
            Collider2D hitCollider = Physics2D.OverlapBox(
                targetPosition,
                boxSize,
                0f,
                obstacleLayer
            );

            Debug.DrawLine(
                targetPosition - boxSize * 0.5f,
                targetPosition + boxSize * 0.5f,
                Color.red,
                0.5f
            );

            return hitCollider == null;
        }

        public bool IsPointerOverSpecificUIElement()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition,
                };

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.GetComponent<Slider>() != null)
                    {
                        return false;
                    }

                    if (result.gameObject.GetComponent<Button>() != null)
                    {
                        return true;
                    }

                    if (result.gameObject.GetComponent<Image>() != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsMouseOverEnemy(Vector3 mousePosition)
        {
            Vector3 roundedMousePosition = new Vector3(
                Mathf.Round(mousePosition.x),
                Mathf.Round(mousePosition.y),
                0
            );
            Collider2D hitCollider = Physics2D.OverlapCircle(
                roundedMousePosition,
                0.5f,
                enemyLayer
            );
            bool isOverEnemy =
                hitCollider != null
                && Vector2.Distance(transform.position, roundedMousePosition)
                    <= playerStats.AttackRange;
            return isOverEnemy;
        }

        private void PerformContinuousAttack(Vector3 targetPosition)
        {
            Vector2Int targetTile = Vector2Int.RoundToInt(targetPosition);
            if (IsEnemyAtPosition(targetTile))
            {
                PlayerCombat.Instance.PerformMeleeAttack(targetTile);
            }
        }

        public bool IsEnemyAtPosition(Vector2Int position)
        {
            Vector2 targetPosition = new Vector2(position.x, position.y);
            Collider2D hitCollider = Physics2D.OverlapBox(
                targetPosition,
                Vector2.one * 0.8f,
                0f,
                enemyLayer
            );
            bool isEnemyAtPos = hitCollider != null;
            return isEnemyAtPos;
        }

        public void UpdateCurrentTilePosition(Vector3 position)
        {
            rb = GetComponent<Rigidbody2D>();

            currentTilePosition = new Vector2Int(
                Mathf.RoundToInt(position.x),
                Mathf.RoundToInt(position.y)
            );

            rb.position = position;
            transform.position = position;
            PlayerCombat.Instance.ResetEnemyAttackFlags();
            PlayerStats.Instance.AddStep();
            UpdatePlayerTile();
        }

        public void UpdatePlayerTile()
        {
            Vector2Int playerPos = new Vector2Int(
                (int)transform.position.x,
                (int)transform.position.y
            );
            TileOccupancyManager.Instance.SetPlayerTileOccupied(playerPos);
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
            playerStats.DecreaseStamina(amount);
            UpdateStaminaUI();
        }
    }
}
