using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoED
{
    public class Enemy : MonoBehaviour
    {
        private EnemyStats enemyStats;
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

            while (Random.value <= currentDropRate)
            {
                DropEquipment();
                currentDropRate *= dropRateDecreaseFactor;
                if (currentDropRate <= 0.01f)
                    break;
            }
        }

        private void DropEquipment()
        {
            // 1) Randomly generate an item
            float roll = Random.value;
            Equipment droppedEquipment;
            if (roll < 0.33f)
                droppedEquipment = EquipmentGenerator.GenerateRandomEquipment(1, "weapon");
            else if (roll < 0.66f)
                droppedEquipment = EquipmentGenerator.GenerateRandomEquipment(1, "armor");
            else
                droppedEquipment = EquipmentGenerator.GenerateRandomEquipment(1, "accessory");

            // 2) Attempt to modify the item stats based on enemy level/scaleFactor
            ModifyBaseEquipment(droppedEquipment);
            // 3) Create a new GameObject in the scene for the drop
            GameObject dropObject = new GameObject($"Dropped_{droppedEquipment.itemName}");
            dropObject.transform.position = transform.position;
            dropObject.tag = "Item";
            dropObject.transform.localScale = new Vector3(2f, 2f, 0f);

            // 4) Add components: a SpriteRenderer to see it, plus EquipmentPickup to store the data
            SpriteRenderer rend = dropObject.AddComponent<SpriteRenderer>();
            rend.sprite = droppedEquipment.baseSprite;
            rend.sortingLayerName = "items";
            rend.sortingOrder = 3;

            // Ensure correct Z in 2D
            dropObject.transform.position = new Vector3(
                dropObject.transform.position.x,
                dropObject.transform.position.y,
                0f
            );

            // Optionally add a collider so the item can be detected
            var col = dropObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            // dropObject.tag = "Item"; // If you want a tag

            // 5) Add the EquipmentPickup script and assign the data
            EquipmentPickup pickup = dropObject.AddComponent<EquipmentPickup>();
            pickup.SetEquipment(droppedEquipment);

            Debug.Log($"Dropped equipment: {droppedEquipment.itemName}");
        }

        /// <summary>
        /// (NEW) Attempts to modify the generated equipment's stats based on this enemy's ScaledFactor.
        /// There's a random chance to apply some scaling or bonus.
        /// </summary>
        private void ModifyBaseEquipment(Equipment equipment)
        {
            // If there's no enemyStats or scale factor is trivial, skip
            if (enemyStats == null || enemyStats.ScaledFactor <= 1f)
                return;

            // Check if we should modify stats
            if (Random.value >= chanceToModifyStats)
                return;

            float factor = enemyStats.ScaledFactor;

            // Define a mapping of stats to modify
            Dictionary<Func<int>, Action<int>> statModifiers = new Dictionary<
                Func<int>,
                Action<int>
            >
            {
                { () => equipment.attack, value => equipment.attack = value },
                { () => equipment.defense, value => equipment.defense = value },
                { () => equipment.magic, value => equipment.magic = value },
                { () => equipment.health, value => equipment.health = value },
                { () => equipment.stamina, value => equipment.stamina = value },
                { () => equipment.intelligence, value => equipment.intelligence = value },
                { () => equipment.dexterity, value => equipment.dexterity = value },
                { () => equipment.speed, value => equipment.speed = value },
                { () => equipment.critChance, value => equipment.critChance = value },
            };

            // Iterate through all stats and apply the modifier
            foreach (var stat in statModifiers)
            {
                int currentValue = stat.Key();
                if (currentValue > 0)
                {
                    int addedValue = Mathf.RoundToInt(factor * modificationMultiplier);
                    stat.Value(currentValue + addedValue);
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
    }
}
