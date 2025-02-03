using System.Linq;
using UnityEngine;

namespace CoED
{
    public static class MonsterInitializer
    {
        private static float RandomBoost(float baseValue)
        {
            return baseValue * Random.Range(0.9f, 1.1f);
        }

        /// <summary>
        /// Adds a random bonus to each monster stat based on the given level.
        /// Does not overwrite existing values, but adds to them.
        /// </summary>
        public static void CalculateMonsterBaseStatsFromLevel(Monster monster, int level)
        {
            // Note: Since monster.monsterStats maps Stat to float,
            // we add the computed bonus to each existing value.
            monster.monsterStats[Stat.MaxHP] += Mathf.RoundToInt(RandomBoost(20 + (level * 15)));
            monster.monsterStats[Stat.Attack] += RandomBoost(3 + (level * 0.55f));
            monster.monsterStats[Stat.Defense] += RandomBoost(1 + (level * 0.55f));
            monster.monsterStats[Stat.Dexterity] += RandomBoost(1 + (level * 0.55f));
            monster.monsterStats[Stat.ProjectileRange] += RandomBoost(1 + (level * 0.25f));
            monster.monsterStats[Stat.AttackRange] += RandomBoost(1 + (level * 0.25f));
            monster.monsterStats[Stat.Speed] += RandomBoost(1 + (level * 0.25f));
            monster.monsterStats[Stat.ElementalDamage] += RandomBoost(1 + (level * 0.55f));
            monster.monsterStats[Stat.ChanceToInflictStatusEffect] += RandomBoost(
                1 + (level * 0.1f)
            );
            monster.monsterStats[Stat.StatusEffectDuration] += RandomBoost(1 + (level * 0.25f));
            monster.monsterStats[Stat.CritChance] += RandomBoost(1 + (level * 0.15f));
            monster.monsterStats[Stat.CritDamage] += RandomBoost(1 + (level * 0.2f));
            monster.monsterStats[Stat.FireRate] += RandomBoost(1 + (level * 0.15f));
            monster.monsterStats[Stat.Shield] += RandomBoost(1 + (level * 0.05f));
            monster.monsterStats[Stat.Accuracy] += RandomBoost(1 + (level * 0.1f));
            monster.monsterStats[Stat.Intelligence] += RandomBoost(1 + (level * 0.55f));
            monster.monsterStats[Stat.PatrolSpeed] = RandomBoost(1 + (level * 0.25f));
            monster.monsterStats[Stat.ChaseSpeed] = monster.monsterStats[Stat.PatrolSpeed] * 1.5f;
        }

        public static void InitializeEnemy(
            GameObject enemy,
            Pathfinding.Pathfinder pathfinder,
            FloorData floorData,
            int occupantIDCounter,
            DungeonSettings dungeonSettings,
            LayerMask obstacleLayer
        )
        {
            int occupantID = ++occupantIDCounter;

            Debug.Log($"Floor {floorData.FloorTilemap.name} - Spawning enemy {occupantID}");
            enemy.layer = LayerMask.NameToLayer("enemies");
            enemy.tag = "Enemy";
            Canvas can = enemy.AddComponent<Canvas>();
            can.sortingOrder = 3;

            EnemyNavigator navigator = enemy.AddComponent<EnemyNavigator>();
            navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

            EnemyBrain brain = enemy.AddComponent<EnemyBrain>();
            brain.Initialize(
                floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints).ToHashSet()
            );
            brain.visualObstructionLayer = obstacleLayer;

            SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
            if (enemy.GetComponent<_EnemyStats>().monsterData.monsterSprite != null)
            {
                sr.sprite = enemy.GetComponent<_EnemyStats>().monsterData.monsterSprite;
                sr.sortingOrder = 3;
            }
            GameObject healthBarPrefab = Resources.Load<GameObject>("Prefabs/enemyHealthBar");
            if (healthBarPrefab != null)
            {
                GameObject healthBar = Object.Instantiate(healthBarPrefab);
                healthBar.transform.SetParent(enemy.transform, false);
                healthBar.transform.localPosition = new Vector3(0, 0.5f, 0); // e.g., above the enemy’s head

                // If your health bar has a script or a Slider, link it to enemy
                EnemyUI barController = enemy.AddComponent<EnemyUI>();

                barController.SetHealthBarMax(enemy.GetComponent<_EnemyStats>().GetEnemyHP()); // e.g., pass the enemy’s data
            }
        }
    }
}
