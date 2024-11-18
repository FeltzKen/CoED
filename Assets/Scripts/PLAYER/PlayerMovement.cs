using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveDelay = 0.2f; // Delay between moves
        [SerializeField] private float staminaCostPerRun = 2f; // Stamina cost for running

        [Header("UI Elements")]
        [SerializeField] private Slider staminaBar; // UI slider for stamina

        private PlayerStats playerStats;
        private PlayerManager playerManager;
        private float moveCooldown;

        private void Start()
        {
            // Initialize references
            playerStats = PlayerStats.Instance;
            playerManager = PlayerManager.Instance;

            UpdateStaminaUI();
        }

        private void Update()
        {
            HandleMovementInput();
        }

        private void HandleMovementInput()
        {
            // Prevent movement if cooldown is active
            if (Time.time < moveCooldown) return;

            Vector2Int direction = Vector2Int.zero;

            // Get movement direction from input
            if (Input.GetKey(KeyCode.UpArrow)) direction += Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow)) direction += Vector2Int.down;
            if (Input.GetKey(KeyCode.LeftArrow)) direction += Vector2Int.left;
            if (Input.GetKey(KeyCode.RightArrow)) direction += Vector2Int.right;

            if (direction != Vector2Int.zero)
            {
                // Check for running input and available stamina
                bool isRunning = Input.GetKey(KeyCode.LeftShift) && playerStats.CurrentStamina >= staminaCostPerRun;
                Vector2Int targetDirection = isRunning ? direction * 2 : direction;

                // Deduct stamina if running
                if (isRunning)
                {
                    DeductStamina(staminaCostPerRun);
                }

                // Commit move action through PlayerManager
                playerManager.CommitMoveAction(targetDirection);
                moveCooldown = Time.time + moveDelay;
            }
        }

        private void DeductStamina(float amount)
        {
            // Deduct stamina and update UI
            playerStats.CurrentStamina = Mathf.Max(playerStats.CurrentStamina - amount, 0);
            UpdateStaminaUI();
            Debug.Log($"PlayerMovement: Deducted {amount} stamina. Current stamina: {playerStats.CurrentStamina}");
        }

        private void UpdateStaminaUI()
        {
            // Update the stamina UI slider
            if (staminaBar != null)
            {
                staminaBar.value = playerStats.CurrentStamina;
            }
        }
    }
}
