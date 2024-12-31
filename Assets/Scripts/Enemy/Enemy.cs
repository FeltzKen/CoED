using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace CoED
{
    public class Enemy : MonoBehaviour
    {
        private StatusEffectManager statusEffectManager;
        public bool IsVisible = false;

        [Header("Loot Settings")]
        [SerializeField]
        private List<EquipmentWrapper> possibleDrops = new List<EquipmentWrapper>();

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
            if (possibleDrops == null || possibleDrops.Count == 0)
            {
                Debug.LogWarning("Enemy: No possible drops assigned.");
                return;
            }
            int randomIndex = Random.Range(0, possibleDrops.Count);

            if (possibleDrops[randomIndex]?.equipmentData == null)
            {
                Debug.LogError($"Enemy.DropLoot: equipmentData is null at index {randomIndex}.");
                return;
            }
            // Drop equipment based on drop rate
            if (Random.value <= baseDropRate)
            {
                Equipment selectedLoot = possibleDrops[
                    Random.Range(0, possibleDrops.Count)
                ].equipmentData;
                if (selectedLoot.equipmentPrefab == null)
                {
                    Debug.LogError(
                        $"Enemy: equipmentPrefab is null for {selectedLoot.equipmentName}."
                    );
                    return;
                }
                if (selectedLoot == null)
                {
                    Debug.LogError("Enemy: Selected loot is null.");
                    return;
                }

                // Use prefab instantiation for the loot object
                GameObject lootPrefab = Instantiate(
                    selectedLoot.equipmentPrefab,
                    transform.position,
                    Quaternion.identity
                );
                EquipmentWrapper lootWrapper = lootPrefab.GetComponent<EquipmentWrapper>();

                if (lootWrapper == null)
                {
                    Debug.LogError("Enemy: Loot prefab is missing EquipmentWrapper component.");
                    Destroy(lootPrefab);
                    return;
                }

                lootWrapper.Initialize(selectedLoot);
                lootWrapper.ApplyStatModifiers(enemyStats.ScaledFactor);

                Debug.Log($"Enemy dropped {selectedLoot.equipmentName} with scaled stats.");
            }

            // Drop money based on money drop rate
            if (Random.value <= moneyDropRate)
            {
                int moneyAmount = Random.Range(minMoneyAmount, maxMoneyDropAmount + 1);

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
                        Color.yellow
                    );
                }
                else
                {
                    Debug.LogWarning("Enemy: Money prefab does not have a Money component.");
                }
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
