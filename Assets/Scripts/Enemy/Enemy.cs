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
        private float baseDropRate = 0.5f; // Increased for testing

        [SerializeField]
        private float dropRateDecreaseFactor = 0.5f;

        [SerializeField]
        private int minMoneyAmount = 1;

        [SerializeField]
        private int maxMoneyDropAmount = 20;

        private EnemyStats enemyStats;
        private SpriteRenderer spriteRenderer;
        private Color normalColor = Color.white;
        private Color highlightedColor = Color.red;

        [Header("Resistance Settings")]
        [SerializeField]
        private float resistanceChance = 0.05f; // 5% chance to add a resistance

        [SerializeField]
        private List<StatusEffect> possibleResistances = new List<StatusEffect>(); // List of possible resistances

        [SerializeField, Range(0, 1)]
        private float enchantmentChance = 0.05f;

        [SerializeField, Range(0, 1)]
        private float curseChance = 0.1f;

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
            float currentDropRate = baseDropRate;

            while (Random.value <= currentDropRate && lootTable.Count > 0)
            {
                // Pick a random item from the loot table
                Item randomLoot = lootTable[Random.Range(0, lootTable.Count)];

                // Drop the selected item
                DropItem(randomLoot);

                // Decrease the drop rate for the next roll
                currentDropRate *= dropRateDecreaseFactor;

                // Optional safeguard to avoid infinite loops
                if (currentDropRate <= 0.01f)
                    break;
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
                var pickupScript = equipmentObject.GetComponent<EquipmentPickup>();
                if (pickupScript != null)
                {
                    // Step 1: Create a new EquipmentWrapper data object in code
                    EquipmentWrapper eqData = new EquipmentWrapper();
                    eqData.Initialize(equipment);
                    eqData.ApplyStatModifiers(Mathf.RoundToInt(enemyStats.ScaledFactor));

                    // Step 2: Let the enemy apply curses, enchantments, etc.
                    TryAddSpecialEffect(eqData);
                    Debug.Log($"Dropped equipment: {equipment.equipmentName} with scaled stats.");

                    // Step 3: Assign it to the pickup script
                    pickupScript.SetData(eqData);

                    // Step 4: If the prefab also has a HiddenItemController, reveal it:
                    var hiddenController = equipmentObject.GetComponent<HiddenItemController>();
                    if (hiddenController != null)
                        hiddenController.isHidden = false;
                }
                else
                {
                    Debug.LogWarning("Equipment prefab is missing an EquipmentPickup script!");
                    Destroy(equipmentObject);
                }
            }
            else if (lootItem is Consumable consumable)
            {
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

        private void AddResistance(EquipmentWrapper equipmentWrapper)
        {
            if (possibleResistances.Count == 0)
            {
                Debug.LogWarning("No resistances available for enemies to apply.");
                return;
            }

            // Select a random resistance from the list
            StatusEffect randomResistance = possibleResistances[
                Random.Range(0, possibleResistances.Count)
            ];

            // Apply the resistance to the equipment
            equipmentWrapper.equipmentData.resistance = randomResistance;

            Debug.Log(
                $"Applied {randomResistance.effectType} resistance to {equipmentWrapper.equipmentData.equipmentName}."
            );
        }

        private void TryAddSpecialEffect(EquipmentWrapper equipmentWrapper)
        {
            bool isEnchanted = false;

            // Check for enchantment first
            if (Random.value <= enchantmentChance)
            {
                equipmentWrapper.IsEnchanted = true;
                ApplyEnchantment(equipmentWrapper);
                isEnchanted = true;
                Debug.Log($"Enchanted {equipmentWrapper.equipmentData.equipmentName}.");
            }
            else if (Random.value <= curseChance)
            {
                equipmentWrapper.IsCursed = true;
                ApplyCurse(equipmentWrapper);
                Debug.Log($"Cursed {equipmentWrapper.equipmentData.equipmentName}.");
            }

            // 🎯 Adjusted resistance chance: 50% if enchanted, default value otherwise
            float chance = isEnchanted ? 0.50f : resistanceChance;

            if (Random.value <= chance)
            {
                AddResistance(equipmentWrapper);
            }
        }

        private void ApplyEnchantment(EquipmentWrapper equipment)
        {
            Dictionary<string, System.Action> enchantableStats = new Dictionary<
                string,
                System.Action
            >
            {
                {
                    "Attack",
                    () =>
                        equipment.enchantedAttackModifier += Mathf.RoundToInt(
                            Random.Range(1, 5) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Defense",
                    () =>
                        equipment.enchantedDefenseModifier += Mathf.RoundToInt(
                            Random.Range(1, 5) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Magic",
                    () =>
                        equipment.enchantedMagicModifier += Mathf.RoundToInt(
                            Random.Range(1, 5) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Health",
                    () =>
                        equipment.enchantedHealthModifier += Mathf.RoundToInt(
                            Random.Range(1, 5) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Speed",
                    () =>
                        equipment.enchantedSpeedModifier += Mathf.RoundToInt(
                            Random.Range(1, 5) * enemyStats.ScaledFactor
                        )
                },
            };

            foreach (var stat in enchantableStats)
            {
                if (Random.value <= 0.10f) // 10% chance to enhance each stat
                {
                    stat.Value.Invoke();
                    Debug.Log(
                        $"Enchanted {equipment.equipmentData.equipmentName}'s {stat.Key} stat."
                    );
                }
            }
        }

        private void ApplyCurse(EquipmentWrapper equipment)
        {
            Dictionary<string, System.Action> curseableStats = new Dictionary<string, System.Action>
            {
                {
                    "Attack",
                    () =>
                        equipment.cursedAttackModifier += Mathf.RoundToInt(
                            Random.Range(1, 3) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Defense",
                    () =>
                        equipment.cursedDefenseModifier += Mathf.RoundToInt(
                            Random.Range(1, 3) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Magic",
                    () =>
                        equipment.cursedMagicModifier += Mathf.RoundToInt(
                            Random.Range(1, 3) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Health",
                    () =>
                        equipment.cursedHealthModifier += Mathf.RoundToInt(
                            Random.Range(1, 3) * enemyStats.ScaledFactor
                        )
                },
                {
                    "Speed",
                    () =>
                        equipment.cursedSpeedModifier += Mathf.RoundToInt(
                            Random.Range(1, 3) * enemyStats.ScaledFactor
                        )
                },
            };

            foreach (var stat in curseableStats)
            {
                if (Random.value <= 1f) // 10% chance to curse each stat
                {
                    stat.Value.Invoke();
                    Debug.Log($"Cursed {equipment.equipmentData.equipmentName}'s {stat.Key} stat.");
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
