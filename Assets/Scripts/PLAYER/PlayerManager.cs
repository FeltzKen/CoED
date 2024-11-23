using UnityEngine;
using System.Collections;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class PlayerManager : MonoBehaviour, IActor
    {
        public static PlayerManager Instance { get; private set; }
        public Vector3Int CurrentPosition { get; private set; } // Player's current grid position
        public float Speed { get; private set; } = 5.0f; // Example speed value
        public float ActionPoints { get; set; } = 0; // Initialize action points

        private PlayerStats playerStats;
        private PlayerCombat playerCombat;
        private PlayerMagic playerMagic;
        private TurnManager turnManager;
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
            turnManager = TurnManager.Instance;

            ValidateComponents();
        }

        private void Start()
        {
            // Initialize player's grid position
            Vector3 playerPosition = transform.position;
            CurrentPosition = new Vector3Int(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y), 0);
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

        public void Act()
        {
            isActionComplete = false;
            actionSelected = false;
            Debug.Log("PlayerManager: It's the player's turn.");

            // If there's a pre-planned action, perform it
            if (lastAction != null)
            {
                PerformLastAction();
            }
            else
            {
                PlayerMovement.Instance.HandlePlayerTurn();
            }
        }

        public void PerformAction()
        {
            // Perform action through PlayerMovement or combat/magic classes
            if (lastAction != null)
            {
                PerformLastAction();
            }
            else
            {
                Debug.LogWarning("PlayerManager: No action has been planned.");
            }
        }

        private void PerformLastAction()
        {
            lastAction.Invoke(); // Execute the planned action
            lastAction = null; // Clear the action after execution
            isActionComplete = true;
            Debug.Log("PlayerManager: Performed action.");
        }

        public void CommitCombatAction(bool isMelee, Vector3 targetPosition)
        {
            // Plan a combat action to be executed during the turn
            lastAction = () =>
            {
                if (isMelee)
                {
                    playerCombat.PerformMeleeAttack();
                    Debug.Log("PlayerManager: Committed melee attack action.");
                }
                else
                {
                    playerCombat.AttemptRangedAttack(targetPosition);
                    Debug.Log("PlayerManager: Committed ranged attack action.");
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
                    Debug.Log($"PlayerManager: Cast magic at {targetPosition} with cost {spellCost} and damage {spellDamage}.");
                }
                else
                {
                    Debug.LogWarning("PlayerManager: Not enough magic to cast the spell.");
                }
            };
            actionSelected = true; // Action has been selected
        }


        public void CommitSpecialAction(System.Action specialAction)
        {
            lastAction = specialAction;
            actionSelected = true;
            Debug.Log("PlayerManager: Committed special action.");
        }

        public void TakeDamage(int damage)
        {
            // Apply damage to the player
            playerStats.TakeDamage(damage);
            Debug.Log($"PlayerManager: Player took {damage} damage.");
        }

        public bool IsActionComplete()
        {
            return isActionComplete;
        }
    }
}
