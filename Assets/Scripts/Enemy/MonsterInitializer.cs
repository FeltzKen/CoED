using System.Linq;
using UnityEngine;

namespace CoED
{
    public static class MonsterInitializer
    {
        public static void CalculateMonsterBaseStatsFromLevel(Monster monster, int level)
        {
            monster.monsterStats[Stat.MaxHP] = Mathf.RoundToInt(20 + (level * 15));
            monster.monsterStats[Stat.Attack] = 3 + (level * 2.5f);
            monster.monsterStats[Stat.Defense] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.Dexterity] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.ProjectileRange] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.AttackRange] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.Speed] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.ElementalDamage] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.ChanceToInflictStatusEffect] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.StatusEffectDuration] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.PatrolSpeed] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.CritChance] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.CritDamage] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.FireRate] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.Shield] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.Accuracy] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.Intelligence] = 1 + (level * 1.5f);
            monster.monsterStats[Stat.ChaseSpeed] = 1 + (level * 1.5f);
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
