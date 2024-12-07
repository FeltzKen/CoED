using UnityEngine;
using System.Collections;
using CoED;

namespace CoED
{
    public class PlayerManager : MonoBehaviour//, IActor
    {
        public static PlayerManager Instance { get; private set; }
        public Vector3Int CurrentPosition { get; private set; } // Player's current grid position
        public float Speed { get; private set; } = 5.0f; // Example speed value
        public float ActionPoints { get; set; } = 0; // Initialize action points

        private PlayerStats playerStats;
        private PlayerCombat playerCombat;
        private PlayerMagic playerMagic;
        private System.Action lastAction; // Stores the last planned action
        private bool isActionComplete = false;
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

            ValidateComponents();
        }

        private void Start()
        {
            // Initialize player's grid position
            Vector3 playerPosition = transform.position;
            CurrentPosition = new Vector3Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y), 0);
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

        public void CommitSpecialAction(System.Action specialAction)
        {
            lastAction = specialAction;
            actionSelected = true;
            // Debug.Log("PlayerManager: Committed special action.");
        }


        public void ResetEnemyAttackFlags()
        {
            foreach (var enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
            {
                enemy.CanAttackPlayer = true;
            }
        }
    }
}
