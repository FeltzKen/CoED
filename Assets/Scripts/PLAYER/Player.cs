using UnityEngine;
using YourGameNamespace;
using System;

namespace YourGameNamespace
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        [Header("Player Components")]
        public PlayerStats playerStats{ get; private set; }
        public PlayerCombat playerCombat{ get; private set; }
        public PlayerMagic playerMagic{ get; private set; }
        public CastSpell castSpell{ get; private set; }
        public PlayerMovement playerMovement{ get; private set; }
        public PlayerActions playerActions{ get; private set; }
        public Rigidbody2D playerRigidbody;
        public StatusEffectManager statusEffectManager{ get; private set; }
        public PlayerManager playerManager{ get; private set; }
        public StatusEffect statusEffect;

        private void Awake()
        {
            Debug.Log("player awake.");
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("Player instance already exists. Destroying duplicate.");
                return;
            }

            // Retrieve and validate essential components

            ValidateComponents();
                playerStats = GetComponent<PlayerStats>();
                playerCombat = GetComponent<PlayerCombat>();
                playerMagic = GetComponent<PlayerMagic>();
                castSpell = GetComponent<CastSpell>();
                playerManager = GetComponent<PlayerManager>();
                playerMovement = GetComponent<PlayerMovement>();
                playerActions = GetComponent<PlayerActions>();
                playerRigidbody = GetComponent<Rigidbody2D>();
                statusEffectManager = GetComponent<StatusEffectManager>();
                statusEffect = GetComponent<StatusEffect>();
        }

        private void ValidateComponents()
        {
            if (playerStats == null) Debug.LogError("Player: Missing PlayerStats component.");
            if (playerCombat == null) Debug.LogError("Player: Missing PlayerCombat component.");
            if (playerMagic == null) Debug.LogError("Player: Missing PlayerMagic component.");
            if (playerMovement == null) Debug.LogError("Player: Missing PlayerMovement component.");
            if (playerActions == null) Debug.LogError("Player: Missing PlayerActions component.");
            if (playerRigidbody == null) Debug.LogError("Player: Missing Rigidbody2D component.");
            if (statusEffectManager == null) Debug.LogError("Player: Missing StatusEffectManager component.");
        }

        public bool IsAlive()
        {
            return playerStats != null && playerStats.CurrentHealth > 0;
        }

        public void HandleDeath()
        {
            if (playerStats != null && playerStats.CurrentHealth <= 0)
            {
                Debug.Log("Player: Player has died.");
                GameManager.Instance?.OnPlayerDeath();
            }
        }

        public void AddExperience(int amount)
        {
            playerStats?.GainExperience(amount);
        }

        public void Heal(int amount)
        {
            playerStats?.Heal(amount);
        }

      /*  public bool CastSpell(Vector3 targetPosition, int spellCost, int spellDamage)
        {
            return playerMagic != null && playerMagic.HasEnoughMagic(spellCost) &&
                   playerMagic.CastMagicAction(targetPosition, spellCost, spellDamage);
        }
      */
        public void RefillMagic(int amount)
        {
            playerMagic?.RefillMagic(amount);
        }

        public void StartMagicRefill()
        {
            playerMagic?.StartMagicRefill();
        }

        public void StopMagicRefill()
        {
            playerMagic?.StopMagicRefill();
        }

        public void AddStatusEffect(StatusEffect effect)
        {
            statusEffectManager?.AddStatusEffect(effect);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        //Debug.Log("player collison detected.");
        }

    }
}
