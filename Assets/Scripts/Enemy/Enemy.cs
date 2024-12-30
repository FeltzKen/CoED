using System;
using CoED;
using UnityEngine;

namespace CoED
{
    public class Enemy : MonoBehaviour
    {
        private StatusEffectManager statusEffectManager;
        public bool IsVisible = false;

        [Header("Loot Settings")]
        [SerializeField]
        private GameObject[] possibleDrops;

        [SerializeField]
        private float baseDropRate = .01f;

        [SerializeField]
        private float dropRateDecreaseFactor = 0.5f;

        [SerializeField]
        private GameObject moneyPrefab;

        [SerializeField]
        private int minMoneyAmount = 1;

        private EnemyStats enemyStats;

        [SerializeField]
        private int maxMoneyDropAmount = 20;

        [SerializeField]
        private float moneyDropRate = 0.5f;
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color highlightedColor = Color.red;

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (statusEffectManager == null)
            {
                Debug.LogError("Enemy: StatusEffectManager component missing.");
                return;
            }
        }

        public void DropLoot()
        {
            float currentDropRate = baseDropRate;

            foreach (var drop in possibleDrops)
            {
                if (UnityEngine.Random.value <= currentDropRate)
                {
                    GameObject loot = Instantiate(drop, transform.position, Quaternion.identity);

                    // Check if the loot has a script to modify stats
                    Equipment equipment = loot.GetComponent<Equipment>();
                    if (equipment != null)
                    {
                        ApplyStatModifiers(equipment);
                    }

                    Consumable consumable = loot.GetComponent<Consumable>();
                    if (consumable != null)
                    {
                        ApplyStatModifiers(consumable);
                    }
                }
                currentDropRate *= dropRateDecreaseFactor;
            }

            if (UnityEngine.Random.value <= moneyDropRate)
            {
                int moneyAmount = UnityEngine.Random.Range(minMoneyAmount, maxMoneyDropAmount + 1);
                GameObject money = Instantiate(
                    moneyPrefab,
                    transform.position,
                    Quaternion.identity
                );

                Money moneyComponent = money.GetComponent<Money>();
                if (moneyComponent != null)
                {
                    moneyComponent.SetAmount(moneyAmount);
                    FloatingTextManager.Instance.ShowFloatingText(
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

        // Applies stat modifiers to an equipment item
        private void ApplyStatModifiers(Equipment equipment)
        {
            equipment.attackModifier += equipment.attackModifier * enemyStats.ScaledFactor;
            equipment.defenseModifier += equipment.defenseModifier * enemyStats.ScaledFactor;
        }

        // Applies stat modifiers to a consumable item
        private void ApplyStatModifiers(Consumable consumable)
        {
            consumable.healthBoost += consumable.healthBoost * enemyStats.ScaledFactor;
            consumable.magicBoost += consumable.magicBoost * enemyStats.ScaledFactor;
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
