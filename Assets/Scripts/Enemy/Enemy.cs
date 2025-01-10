using System.Collections.Generic;
using CoED.Items;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

namespace CoED
{
    public class Enemy : MonoBehaviour
    {
        private StatusEffectManager statusEffectManager;
        public bool IsVisible = false;

        [Header("Loot Settings")]
        [SerializeField]
        private List<Item> lootTable = new List<Item>();

        [SerializeField]
        private float baseDropRate = 0.9f; // Increased for testing

        [SerializeField]
        private float dropRateDecreaseFactor = 0.5f;

        [SerializeField]
        private GameObject moneyPrefab;

        [SerializeField]
        private int minMoneyAmount = 1;

        [SerializeField]
        private int maxMoneyDropAmount = 20;

        [SerializeField]
        private float moneyDropRate = 0.5f; // Increased for better testing

        private EnemyStats enemyStats;
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField]
        private Color highlightedColor = Color.red;

        private void Start()
        {
            enemyStats = GetComponent<EnemyStats>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (enemyStats == null)
            {
                Debug.LogError("Enemy: Missing EnemyStats component.");
            }
        }

        public void DropLoot()
        {
            foreach (var lootItem in lootTable)
            {
                if (Random.value <= 0.5f) // Example drop chance logic
                {
                    DropItem(lootItem);
                }
            }
        }

        private void DropItem(Item lootItem)
        {
            if (lootItem is Equipment equipment)
            {
                // Instantiate and initialize the equipment wrapper
                var equipmentObject = Instantiate(
                    equipment.itemPrefab,
                    transform.position,
                    Quaternion.identity
                );
                var equipmentWrapper = equipmentObject.GetComponent<EquipmentWrapper>();

                if (equipmentWrapper != null)
                {
                    equipmentWrapper.Initialize(equipment);
                    equipmentWrapper.ApplyStatModifiers(enemyStats.ScaledFactor); // Apply scaled factor
                    Debug.Log($"Dropped equipment: {equipment.equipmentName} with scaled stats.");
                    equipmentWrapper.GetComponent<HiddenItemController>().isHidden = false;
                }
                else
                {
                    Debug.LogWarning(
                        $"Equipment prefab {equipment.equipmentName} is missing an EquipmentWrapper component."
                    );
                    Destroy(equipmentObject);
                }
            }
            else if (lootItem is Consumable consumable)
            {
                // Instantiate the consumable item
                GameObject consumableItem = Instantiate(
                    consumable.itemPrefab,
                    transform.position,
                    Quaternion.identity
                );
                Debug.Log($"Dropped consumable: {consumable.itemName}");
                consumableItem.GetComponent<HiddenItemController>().isHidden = false;
            }
            else if (lootItem is Money money)
            {
                // Instantiate money with a random amount based on ScaledFactor
                var moneyObject = Instantiate(
                    money.itemPrefab,
                    transform.position,
                    Quaternion.identity
                );
                var moneyComponent = moneyObject.GetComponent<MoneyPickup>();

                if (moneyComponent != null)
                {
                    int baseMoneyAmount = Random.Range(minMoneyAmount, maxMoneyDropAmount + 1);
                    int scaledMoneyAmount = Mathf.RoundToInt(
                        baseMoneyAmount * enemyStats.ScaledFactor
                    );
                    moneyComponent.SetAmount(scaledMoneyAmount);
                    Debug.Log($"Dropped money: {scaledMoneyAmount} gold.");
                }
                else
                {
                    Debug.LogWarning("Money prefab does not have a Money component.");
                    Destroy(moneyObject);
                }
            }
            else
            {
                Debug.LogWarning($"Unknown loot type: {lootItem.itemName}");
            }
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
