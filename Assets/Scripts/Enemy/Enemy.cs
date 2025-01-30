using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoED
{
    [RequireComponent(typeof(_EnemyStats))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class _Enemy : MonoBehaviour
    {
        private _EnemyStats enemyStats;
        private SpriteRenderer spriteRenderer;

        private Color normalColor = Color.white;
        private Color highlightedColor = Color.red;

        [Header("Loot Settings")]
        [SerializeField]
        private float baseDropRate = 0.5f; // Base drop rate for equipment

        [SerializeField]
        private float dropRateDecreaseFactor = 0.5f;

        [Header("Item Stat Modification Settings")]
        [Tooltip("Chance (0.0 - 1.0) that the enemy will modify stats of the dropped item.")]
        [SerializeField]
        private float chanceToModifyStats = 0.3f; // e.g. 30% chance

        [Tooltip(
            "Multiplier for how strongly stats are modified, scaled by enemyStats.ScaledFactor."
        )]
        [SerializeField]
        private float modificationMultiplier = 2f;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyStats = GetComponent<_EnemyStats>();

            if (enemyStats == null)
            {
                Debug.LogError("Enemy: Missing _EnemyStats component.");
            }
        }

        /// <summary>
        /// Public method to drop loot from this enemy if it dies.
        /// (Same signature as old version.)
        /// </summary>
        public void DropLoot()
        {
            float currentDropRate = baseDropRate;
            int maxDrops = 3;
            int itemsDropped = 0;

            while (itemsDropped < maxDrops && Random.value <= currentDropRate)
            {
                DropEquipment();
                currentDropRate *= dropRateDecreaseFactor;
                itemsDropped++;

                if (currentDropRate <= 0.01f)
                    break;
            }
        }

        /// <summary>
        /// Attempts to generate and drop equipment from EquipmentGenerator (if still used).
        /// </summary>
        private void DropEquipment()
        {
            // e.g. 33% chance to actually drop an item
            float roll = Random.value;
            Equipment droppedEquipment = null;
            if (roll < 0.33f)
            {
                int eqTier = enemyStats.EquipmentTier;
                droppedEquipment = EquipmentGenerator.GenerateRandomEquipment(eqTier);
            }

            if (droppedEquipment == null)
            {
                return;
            }

            ModifyBaseEquipment(droppedEquipment);

            // Create a new drop object in the scene
            GameObject dropObject = new GameObject($"Dropped_{droppedEquipment.itemName}");
            dropObject.transform.position = transform.position;
            dropObject.tag = "Item";
            dropObject.transform.localScale = new Vector3(2f, 2f, 0f);

            SpriteRenderer rend = dropObject.AddComponent<SpriteRenderer>();
            rend.sprite = droppedEquipment.baseSprite;
            rend.sortingLayerName = "items";
            rend.sortingOrder = 3;

            dropObject.transform.position = new Vector3(
                dropObject.transform.position.x,
                dropObject.transform.position.y,
                0f
            );

            var col = dropObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            EquipmentPickup pickup = dropObject.AddComponent<EquipmentPickup>();
            pickup.SetEquipment(droppedEquipment);

            Debug.Log($"Dropped equipment: {droppedEquipment.itemName}");
        }

        /// <summary>
        /// Chance-based stat buff for the dropped equipment, scaled by enemyStats.
        /// </summary>
        private void ModifyBaseEquipment(Equipment equipment)
        {
            if (enemyStats == null || enemyStats.ScaledFactor <= 1f)
                return;

            if (Random.value >= chanceToModifyStats)
                return;

            float factor = enemyStats.ScaledFactor;
            int addedValue = Mathf.RoundToInt(factor * modificationMultiplier);

            Dictionary<Func<int>, Action<int>> statModifiers = new Dictionary<
                Func<int>,
                Action<int>
            >
            {
                { () => equipment.attack, v => equipment.attack = v },
                { () => equipment.defense, v => equipment.defense = v },
                { () => equipment.magic, v => equipment.magic = v },
                { () => equipment.health, v => equipment.health = v },
                { () => equipment.stamina, v => equipment.stamina = v },
                { () => equipment.intelligence, v => equipment.intelligence = v },
                { () => equipment.dexterity, v => equipment.dexterity = v },
                { () => equipment.speed, v => equipment.speed = v },
                { () => equipment.critChance, v => equipment.critChance = v },
            };

            foreach (var stat in statModifiers)
            {
                int currentValue = stat.Key();
                if (currentValue > 0)
                {
                    stat.Value(currentValue + addedValue);
                }
            }
        }

        /// <summary>
        /// Toggles highlighting on this enemy.
        /// </summary>
        public void SetHighlighted(bool isHighlighted)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isHighlighted ? highlightedColor : normalColor;
            }
        }
    }
}
