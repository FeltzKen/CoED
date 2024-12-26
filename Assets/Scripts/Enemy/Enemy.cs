using System;
using CoED;
using UnityEngine;

namespace CoED
{
    public class Enemy : MonoBehaviour
    {
        private StatusEffectManager statusEffectManager; // Manages enemy status effects
        private FloatingTextManager floatingTextManager; // Manages floating text displays for feedback
        public bool IsVisible = false; // Tracks if the enemy is visible (not covered by fog)

        [Header("Loot Settings")]
        [SerializeField]
        private GameObject[] possibleDrops; // Array of possible loot items the enemy can drop

        [SerializeField]
        private float baseDropRate = 0.3f; // Base chance for dropping items

        [SerializeField]
        private float dropRateDecreaseFactor = 0.5f; // Drop rate decay factor after each item

        [SerializeField]
        private GameObject moneyPrefab; // Prefab for the money drop

        [SerializeField]
        private int minMoneyAmount = 1; // Minimum money amount dropped

        private EnemyStats enemyStats;

        [SerializeField]
        private int maxMoneyDropAmount = 20; // Maximum money amount dropped

        [SerializeField]
        private float moneyDropRate = 0.5f; // Chance to drop money
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color highlightedColor = Color.red;

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            // Initialize necessary components
            //statusEffectManager = GetComponent<StatusEffectManager>();
            //floatingTextManager = FindAnyObjectByType<FloatingTextManager>();

            // Check for required components
            if (statusEffectManager == null)
            {
                Debug.LogError("Enemy: StatusEffectManager component missing.");
                return;
            }

            // Set initial health
        }

        public void DropLoot()
        {
            float currentDropRate = baseDropRate;

            // Loop through possible drops, instantiating them based on current drop rate
            foreach (var drop in possibleDrops)
            {
                if (UnityEngine.Random.value <= currentDropRate)
                {
                    Instantiate(drop, transform.position, Quaternion.identity);
                }
                currentDropRate *= dropRateDecreaseFactor; // Reduces chance for subsequent items
            }

            // Determine if money should be dropped
            if (UnityEngine.Random.value <= moneyDropRate)
            {
                int moneyAmount = UnityEngine.Random.Range(minMoneyAmount, maxMoneyDropAmount + 1);
                GameObject money = Instantiate(
                    moneyPrefab,
                    transform.position,
                    Quaternion.identity
                );

                // Set the amount of money dropped if Money component is found
                Money moneyComponent = money.GetComponent<Money>();
                if (moneyComponent != null)
                {
                    moneyComponent.SetAmount(moneyAmount);
                    floatingTextManager?.ShowFloatingText(
                        $"Dropped {moneyAmount} gold",
                        transform,
                        Color.blue
                    );
                }
                else
                {
                    Debug.LogWarning("Enemy: Money prefab does not have a Money component.");
                }
            }
        }

        public void Attack()
        {
            PlayerStats.Instance.TakeDamage(enemyStats.CurrentAttack);
        }

        public void SetHighlighted(bool isHighlighted)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isHighlighted ? highlightedColor : normalColor;
            }
        }

        public void ResetEnemyAttackFlags()
        {
            foreach (var enemy in FindObjectsByType<EnemyBrain>(FindObjectsSortMode.None))
            {
                enemy.CanAttackPlayer = true;
            }
        }
    }
}
