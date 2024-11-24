using System.Collections;
using UnityEngine;
using CoED;

namespace CoED
{
    // Manages the player's magic resources, including casting spells and refilling magic.
    public class PlayerMagic : MonoBehaviour
    {
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
        private UIManager uiManager;
        private PlayerManager playerManager;

        public int MaxMagic => maxMagic;
        public int CurrentMagic
        {
            get { return currentMagic; }
            set { currentMagic = (int)Mathf.Max(0, value); } // Prevent going below zero
        }

        private void Start()
        {
            uiManager = FindAnyObjectByType<UIManager>();
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
                projectileManager?.LaunchProjectile(transform.position, targetPosition, spellDamage, false);
                Debug.Log($"PlayerMagic: Cast spell towards {targetPosition} for {spellDamage} damage.");
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
            uiManager?.UpdateMagicBar(currentMagic);
        }

        public void RefillMagic(int amount)
        {
            currentMagic = Mathf.Min(currentMagic + amount, maxMagic);
            UpdateMagicUI();
            Debug.Log($"PlayerMagic: Refilled magic by {amount}. Current magic: {currentMagic}");
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

/*
Changes made:
1. Removed any direct reference to managing actions with TurnManager. All magic actions are now intended to be called via PlayerManager.
2. Added `CastMagicAction()` method to be called by PlayerManager, delegating casting logic through it.
3. Removed unnecessary redundant logic or left-over remnants that managed turn actions. Now all action registration must go through PlayerManager.
*/
