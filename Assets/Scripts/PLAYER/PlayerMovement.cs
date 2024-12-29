using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance { get; private set; }

        [Header("Movement Settings")]
        [SerializeField]
        private float staminaCostPerRun = 2f;

        [SerializeField]
        private LayerMask obstacleLayer;

        [SerializeField]
        private float moveDelay = 0.2f;

        [SerializeField]
        private Slider staminaBar;

        [SerializeField]
        private LayerMask enemyLayer;
        private Enemy enemy;
        private PlayerStats playerStats;
        public Vector2Int currentTilePosition;
        private float moveCooldown;
        private Rigidbody2D rb;
        private bool isMoving = false;
        private Vector2 targetPosition;
        private bool isMouseHeld = false;
        private bool isInitialClickValid = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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
            if (Time.time < moveCooldown)
                return;
            isMouseHeld = Input.GetMouseButton(0);

            if (Input.GetMouseButtonDown(0))
            {
                isInitialClickValid = !IsPointerOverSpecificUIElement();
            }
            if (
                isMouseHeld && IsMouseOverEnemy(Camera.main.ScreenToWorldPoint(Input.mousePosition))
            )
            {
                PerformContinuousAttack(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                return;
            }
            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKey(KeyCode.UpArrow))
                direction += Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow))
                direction += Vector2Int.down;
            if (Input.GetKey(KeyCode.LeftArrow))
                direction += Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow))
                direction += Vector2Int.right;

            if (direction != Vector2Int.zero)
            {
                MoveInDirection(direction);
                return;
            }

            if (isMouseHeld && isInitialClickValid)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;

                Transform floorParent = GetCurrentFloorParent();
                if (floorParent != null)
                {
                    mousePosition -= floorParent.position;
                }

                Vector2 directionToMouse = (mousePosition - transform.position).normalized;

                direction = DetermineDirection(directionToMouse);

                if (!IsMouseOverEnemy(mousePosition))
                {
                    MoveInDirection(direction);
                }
            }
        }

        private Vector2Int DetermineDirection(Vector2 directionToMouse)
        {
            Vector2Int direction = Vector2Int.zero;

            if (Mathf.Abs(directionToMouse.x) > Mathf.Abs(directionToMouse.y))
            {
                direction.x = directionToMouse.x > 0 ? 1 : -1;
            }
            else
            {
                direction.y = directionToMouse.y > 0 ? 1 : -1;
            }

            if (Mathf.Abs(directionToMouse.x) > 0.5f && Mathf.Abs(directionToMouse.y) > 0.5f)
            {
                direction.x = directionToMouse.x > 0 ? 1 : -1;
                direction.y = directionToMouse.y > 0 ? 1 : -1;
            }

            return direction;
        }

        private Transform GetCurrentFloorParent()
        {
            int currentFloor = playerStats.currentFloor;
            return DungeonManager.Instance.GetFloorTransform(currentFloor);
        }

        private void MoveInDirection(Vector2Int direction)
        {
            Vector2Int newTilePosition = currentTilePosition + direction;
            Vector2 newPosition = new Vector2(newTilePosition.x, newTilePosition.y);
            if (IsEnemyAtPosition(newTilePosition))
            {
                PlayerCombat.Instance.PerformMeleeAttack(newTilePosition);
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

                isMoving = true;
                moveCooldown = Time.time + moveDelay;
                enemy.ResetEnemyAttackFlags();
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

        private bool IsPointerOverSpecificUIElement()
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
                        continue;
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

        private bool IsMouseOverEnemy(Vector3 mousePosition)
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

        private bool IsEnemyAtPosition(Vector2Int position)
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
            enemy.ResetEnemyAttackFlags();
            PlayerStats.Instance.AddStep();
            TileOccupancyManager.Instance.SetPlayerPosition(
                Vector2Int.RoundToInt(transform.position)
            );
        }

        public object GetPlayerTransform()
        {
            return transform;
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
            playerStats.CurrentStamina = Mathf.RoundToInt(
                Mathf.Max(playerStats.CurrentStamina - amount, 0)
            );
            UpdateStaminaUI();
        }
    }
}
