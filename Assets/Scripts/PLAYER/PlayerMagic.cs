using System.Collections;
using UnityEngine;
using CoED;

namespace CoED
{
    // Manages the player's magic resources, including casting spells and refilling magic.
    public class PlayerMagic : MonoBehaviour
    {
        public static PlayerMagic Instance { get; private set; }
        [Header("Magic Settings")]
        [SerializeField]
        private int maxMagic = 100;

        [SerializeField]
        private int currentMagic = 100;

        [Header("Magic Refill Settings")]
        [SerializeField]
        private int refillAmount = 1;

        [SerializeField]
        private float refillInterval = 1f;

        [Header("Projectile Settings")]
        [SerializeField]
        private ProjectileManager projectileManager;

        private Coroutine refillCoroutine;
        private PlayerUI playerUI;
        private PlayerManager playerManager;

        public int MaxMagic => maxMagic;
        public int CurrentMagic
        {
            get { return currentMagic; }
            set { currentMagic = (int)Mathf.Max(0, value); } // Prevent going below zero
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerMagic instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            playerUI = FindAnyObjectByType<PlayerUI>();
            projectileManager = projectileManager ?? FindAnyObjectByType<ProjectileManager>();
            playerManager = PlayerManager.Instance;

            if (projectileManager == null)
            {
                Debug.LogError("PlayerMagic: ProjectileManager not found in the scene.");
            }

            UpdateMagicUI();
        }

        public void CastMagicAction(Vector3 targetPosition, int spellCost, int spellDamage)
        {
            if (HasEnoughMagic(spellCost))
            {
                SpendMagic(spellCost);
                projectileManager?.LaunchProjectile(transform.position, targetPosition);
                // Debug.Log($"PlayerMagic: Cast spell towards {targetPosition} for {spellDamage} damage.");
            }
            else
            {
                Debug.LogWarning("PlayerMagic: Not enough magic to cast the spell.");
            }
        }

        public bool HasEnoughMagic(int cost)
        {
            return currentMagic >= cost;
        }

        private void SpendMagic(int amount)
        {
            currentMagic = Mathf.Max(0, currentMagic - amount); // Prevent going below zero
            UpdateMagicUI();
        }

        private void UpdateMagicUI()
        {
            playerUI?.UpdateMagicBar(currentMagic, maxMagic);
        }

        public void RefillMagic(int amount)
        {
            currentMagic = Mathf.Min(currentMagic + amount, maxMagic);
            UpdateMagicUI();
            // Debug.Log($"PlayerMagic: Refilled magic by {amount}. Current magic: {currentMagic}");
        }

        public void StartMagicRefill()
        {
            if (refillCoroutine == null)
            {
                refillCoroutine = StartCoroutine(MagicRefillCoroutine());
            }
        }

        public void StopMagicRefill()
        {
            if (refillCoroutine != null)
            {
                StopCoroutine(refillCoroutine);
                refillCoroutine = null;
            }
        }

        private IEnumerator MagicRefillCoroutine()
        {
            while (currentMagic < maxMagic)
            {
                RefillMagic(refillAmount);
                yield return new WaitForSeconds(refillInterval);
            }

            refillCoroutine = null; // Reset coroutine reference
        }
    }
}
