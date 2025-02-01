using System.Collections.Generic;
using System.Linq;
using CoED.Items;
using CoED.Pathfinding;
using UnityEngine;

namespace CoED
{
    public class DungeonSpawner : MonoBehaviour
    {
        public static DungeonSpawner Instance { get; private set; }

        [Header("References")]
        [SerializeField]
        private GameObject player;
        public DungeonSettings dungeonSettings;

        private Rigidbody2D playerRB;

        [SerializeField]
        private LayerMask obstacleLayer;

        [SerializeField]
        private LayerMask validSpawnLayer;

        [SerializeField]
        private ConsumableItem spawningRoomPotion;
        private static int occupantIDCounter = 0;
        private Dictionary<int, Pathfinder> floorPathfinders = new Dictionary<int, Pathfinder>();
        private Dictionary<int, List<GameObject>> enemiesByFloor;
        public Material fireOverlayMaterial;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("DungeonSpawner instance already exists. Destroying duplicate.");
                return;
            }

            playerRB = player.GetComponent<Rigidbody2D>();
            dungeonSettings = FindAnyObjectByType<DungeonGenerator>()
                ?.GetComponent<DungeonGenerator>()
                .dungeonSettings;

            enemiesByFloor = new Dictionary<int, List<GameObject>>
            {
                { 1, dungeonSettings.enemyPrefabsFloor_1 },
                { 2, dungeonSettings.enemyPrefabsFloor_2 },
                { 3, dungeonSettings.enemyPrefabsFloor_3 },
                { 4, dungeonSettings.enemyPrefabsFloor_4 },
                { 5, dungeonSettings.enemyPrefabsFloor_5 },
                { 6, dungeonSettings.enemyPrefabsFloor_6 },
            };
        }

        #region Player Transport
        public void TransportPlayerToDungeon(Transform player)
        {
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
            }
            if (!DungeonManager.Instance.FloorTransforms.ContainsKey(1))
            {
                Debug.LogError("First floor Transform not found. Cannot transport player.");
                return;
            }

            FloorData firstFloorData = DungeonManager.Instance.GetFloorData(1);
            PlayerStats.Instance.currentFloor = 1;
            player.GetComponent<PlayerNavigator>().SetTilemap(firstFloorData.FloorTilemap);
            if (firstFloorData == null || firstFloorData.FloorTiles.Count == 0)
            {
                Debug.LogWarning("No floor tiles found for Floor 1. Cannot transport player.");
                return;
            }

            Vector2Int randomTile = GetRandomValidTile(firstFloorData);

            Vector3Int cellPos = new Vector3Int(randomTile.x, randomTile.y, 0);
            Vector3 baseWorldPos = firstFloorData.FloorTilemap.CellToWorld(cellPos);
            Vector3 centeredWorldPos = baseWorldPos + new Vector3(0.5f, 0.5f, 0f);

            player.position = centeredWorldPos;
            playerRB.position = centeredWorldPos;

            player.GetComponent<PlayerMovement>().UpdateCurrentTilePosition(centeredWorldPos);
        }

        private Vector2Int GetRandomValidTile(FloorData floorData)
        {
            List<Vector2Int> floorTileList = floorData.FloorTiles.ToList();
            int maxAttempts = 100;

            while (maxAttempts > 0 && floorTileList.Count > 0)
            {
                Vector2Int candidate = floorTileList[Random.Range(0, floorTileList.Count)];

                if (
                    !floorData.WallTiles.Contains(candidate)
                    && !floorData.VoidTiles.Contains(candidate)
                )
                {
                    return candidate;
                }

                floorTileList.Remove(candidate);
                maxAttempts--;
            }

            Debug.LogWarning(
                "GetRandomValidTile: No valid tile found, returning first floor tile as fallback."
            );
            return floorData.FloorTiles.First();
        }

        #endregion

        #region Enemy Spawning (refactored)
        public void SpawnEnemiesForAllFloors()
        {
            // We iterate all floors from DungeonManager, skipping floor 0
            foreach (var kvp in DungeonManager.Instance.floors.OrderBy(e => e.Key))
            {
                int floorNum = kvp.Key;
                if (floorNum == 0)
                    continue;

                FloorData floorData = DungeonManager.Instance.GetFloorData(floorNum);
                if (floorData == null)
                    continue;

                Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNum);
                if (floorParent == null)
                    continue;

                Transform enemyParent = floorParent.Find("EnemyParent");
                if (enemyParent == null)
                {
                    Debug.LogError(
                        $"EnemyParent not found for Floor {floorNum}. Skipping enemies."
                    );
                    continue;
                }

                SpawnEnemiesForFloor(floorNum, enemyParent, floorData);
            }
        }

        private void SpawnEnemiesForFloor(
            int floorNumber,
            Transform enemyParent,
            FloorData floorData
        )
        {
            List<Vector2Int> validTiles = floorData
                .FloorTiles.Where(pos => IsValidSpawnPosition(floorData, pos))
                .ToList();

            if (validTiles.Count == 0)
            {
                Debug.LogWarning($"No valid tiles available for enemies on floor {floorNumber}.");
                return;
            }
            int spawnCount = Mathf.Min(dungeonSettings.numberOfEnemiesPerFloor, validTiles.Count);
            var pathfinder = GetOrCreateFloorPathfinder(floorData);
            if (pathfinder == null)
            {
                Debug.LogError(
                    $"No pathfinder found for floor {floorNumber}. Skipping enemy spawn."
                );
                return;
            }
            var floorTransformData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorTransformData == null)
            {
                Debug.LogError(
                    $"No floor transform data found for floor {floorNumber}. Skipping enemy spawn."
                );
                return;
            }

            for (int i = 0; i < spawnCount; i++)
            {
                Vector2Int tilePos = validTiles[Random.Range(0, validTiles.Count)];
                validTiles.Remove(tilePos);
                Vector3 worldPos =
                    floorData.FloorTilemap.CellToWorld((Vector3Int)tilePos)
                    + new Vector3(0.5f, 0.5f, 0);

                Monster monsterData = null;
                // Attempt mini-boss/final boss logic
                /* if (floorNumber % 2 == 0 && !floorTransformData.hasSpawnedMiniBoss)
                 {
                     floorTransformData.hasSpawnedMiniBoss = true;
                     var mb = MonsterGenerator.GetRandomMiniBoss();
                     if (mb != null)
                         monsterData = ConvertMiniBossToMonster(mb);
                 }
                 else if (floorNumber == 6 && !floorTransformData.hasSpawnedBoss)
                 {
                     floorTransformData.hasSpawnedBoss = true;
                     var fb = MonsterGenerator.GetRandomFinalBoss();
                     if (fb != null)
                         monsterData = ConvertFinalBossToMonster(fb);
                 }
                */
                // If not mini/final
                if (monsterData == null)
                {
                    // 1) Pick a random "template" monster from DB
                    Monster templateMonster = MonsterGenerator.GetRandomMonster();
                    if (templateMonster == null)
                    {
                        Debug.LogWarning("No standard monster found! Skipping spawn.");
                        continue;
                    }

                    // 2) create a new monster data instance from template
                    monsterData = new Monster
                    {
                        name = templateMonster.name,
                        description = templateMonster.description,
                        monsterStats = new Dictionary<Stat, float>(templateMonster.monsterStats),
                        damageType = templateMonster.damageType,
                        monsterSprite = templateMonster.monsterSprite,
                        statusInflictionChance = templateMonster.statusInflictionChance,
                        inflictedStatusEffect = templateMonster.inflictedStatusEffect,
                        resistance = templateMonster.resistance,
                        weakness = templateMonster.weakness,
                    };

                    // 3) Decide the monster's new level from floor + random variation
                    int randomLevel = Mathf.RoundToInt(floorNumber * Random.Range(0.8f, 1.6f));
                    randomLevel = Mathf.Clamp(randomLevel, 1, 30);
                    monsterData.level = randomLevel;

                    // 4) Recompute stats from that level
                    MonsterInitializer.CalculateMonsterBaseStatsFromLevel(
                        monsterData,
                        monsterData.level
                    );
                    // etc.
                }

                // Now monsterData has either pre-set mini/final stats or newly computed standard stats
                GameObject enemyGO = new GameObject($"Enemy_{monsterData.name}");
                enemyGO.AddComponent<Rigidbody2D>();

                RectTransform enemyRect = enemyGO.AddComponent<RectTransform>();
                enemyRect.sizeDelta = new Vector2(1, 1);

                enemyGO.transform.position = worldPos;
                enemyGO.transform.SetParent(enemyParent);

                var col = enemyGO.AddComponent<CircleCollider2D>();
                col.isTrigger = false;

                _EnemyStats statsComp = enemyGO.AddComponent<_EnemyStats>();
                statsComp.monsterData = monsterData;
                statsComp.spawnFloorLevel = floorNumber;

                MonsterInitializer.InitializeEnemy(
                    enemyGO,
                    pathfinder,
                    floorData,
                    occupantIDCounter,
                    dungeonSettings,
                    obstacleLayer
                );
            }
        }

        /// <summary>
        /// Convert MiniBoss => Monster
        /// </summary>
        private Monster ConvertMiniBossToMonster(MiniBoss mb)
        {
            return new Monster
            {
                name = mb.name,
                description = mb.description,
                level = mb.level,
                monsterStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, mb.MiniBossStats[Stat.MaxHP] },
                    { Stat.Attack, mb.MiniBossStats[Stat.Attack] },
                    { Stat.Defense, mb.MiniBossStats[Stat.Defense] },
                    { Stat.Speed, mb.MiniBossStats[Stat.Speed] },
                    { Stat.Intelligence, mb.MiniBossStats[Stat.Intelligence] },
                    { Stat.Dexterity, mb.MiniBossStats[Stat.Dexterity] },
                    { Stat.CritChance, mb.MiniBossStats[Stat.CritChance] },
                    { Stat.CritDamage, mb.MiniBossStats[Stat.CritDamage] },
                    { Stat.FireRate, mb.MiniBossStats[Stat.FireRate] },
                    { Stat.Shield, mb.MiniBossStats[Stat.Shield] },
                    { Stat.Accuracy, mb.MiniBossStats[Stat.Accuracy] },
                    { Stat.ElementalDamage, mb.MiniBossStats[Stat.ElementalDamage] },
                    {
                        Stat.ChanceToInflictStatusEffect,
                        mb.MiniBossStats[Stat.ChanceToInflictStatusEffect]
                    },
                    { Stat.StatusEffectDuration, mb.MiniBossStats[Stat.StatusEffectDuration] },
                    { Stat.PatrolSpeed, mb.MiniBossStats[Stat.PatrolSpeed] },
                    { Stat.ChaseSpeed, mb.MiniBossStats[Stat.ChaseSpeed] },
                },
                damageType = mb.damageType,
                statusInflictionChance = mb.MiniBossStats[Stat.ChanceToInflictStatusEffect],
                inflictedStatusEffect = mb.inflictedStatusEffect,
                monsterSprite = mb.miniBossSprite,
            };
        }

        /// <summary>
        /// Convert FinalBoss => Monster
        /// </summary>
        private Monster ConvertFinalBossToMonster(FinalBoss fb)
        {
            return new Monster
            {
                name = fb.name,
                description = fb.description,
                level = fb.level,
                monsterStats = new Dictionary<Stat, float>()
                {
                    { Stat.MaxHP, fb.finalBossStats[Stat.MaxHP] },
                    { Stat.Attack, fb.finalBossStats[Stat.Attack] },
                    { Stat.Defense, fb.finalBossStats[Stat.Defense] },
                    { Stat.Speed, fb.finalBossStats[Stat.Speed] },
                    { Stat.Intelligence, fb.finalBossStats[Stat.Intelligence] },
                    { Stat.Dexterity, fb.finalBossStats[Stat.Dexterity] },
                    { Stat.CritChance, fb.finalBossStats[Stat.CritChance] },
                    { Stat.CritDamage, fb.finalBossStats[Stat.CritDamage] },
                    { Stat.FireRate, fb.finalBossStats[Stat.FireRate] },
                    { Stat.Shield, fb.finalBossStats[Stat.Shield] },
                    { Stat.Accuracy, fb.finalBossStats[Stat.Accuracy] },
                    { Stat.ElementalDamage, fb.finalBossStats[Stat.ElementalDamage] },
                    {
                        Stat.ChanceToInflictStatusEffect,
                        fb.finalBossStats[Stat.ChanceToInflictStatusEffect]
                    },
                    { Stat.StatusEffectDuration, fb.finalBossStats[Stat.StatusEffectDuration] },
                    { Stat.PatrolSpeed, fb.finalBossStats[Stat.PatrolSpeed] },
                    { Stat.ChaseSpeed, fb.finalBossStats[Stat.ChaseSpeed] },
                },
                damageType = fb.damageType,
                statusInflictionChance = fb.finalBossStats[Stat.ChanceToInflictStatusEffect],
                inflictedStatusEffect = fb.inflictedStatusEffect,
                monsterSprite = fb.bossSprite,
            };
        }

        /// <summary>
        /// Same as before: checks if a floor tile is valid for spawn
        /// (unchanged).
        /// </summary>
        public bool IsValidSpawnPosition(FloorData floorData, Vector2Int gridPos)
        {
            List<Vector2Int> wallList = DungeonManager
                .Instance.GetFloorData(floorData.FloorNumber)
                .WallTiles.ToList();
            List<Vector2Int> voidList = DungeonManager
                .Instance.GetFloorData(floorData.FloorNumber)
                .VoidTiles.ToList();

            if (
                !floorData.FloorTiles.Contains(gridPos)
                || wallList.Contains(gridPos)
                || voidList.Contains(gridPos)
            )
            {
                return false;
            }

            Vector3 worldPos = floorData.FloorTilemap.CellToWorld(
                new Vector3Int(gridPos.x, gridPos.y, 0)
            );
            float checkRadius = 0.4f;
            bool overlapsWall =
                Physics2D.OverlapCircle(worldPos, checkRadius, obstacleLayer) != null;
            if (overlapsWall)
                return false;

            return true;
        }

        /// <summary>
        /// Same as before: creates a pathfinder for the floor if needed.
        /// </summary>
        private Pathfinder GetOrCreateFloorPathfinder(FloorData floorData)
        {
            int floorNum = floorData.FloorNumber;
            if (floorPathfinders.ContainsKey(floorNum))
                return floorPathfinders[floorNum];

            HashSet<Vector2Int> walkable = DungeonManager
                .Instance.GetFloorData(floorNum)
                .FloorTiles;
            PathfindingGrid grid = new PathfindingGrid(walkable);
            Pathfinder pathfinder = new Pathfinder(grid);
            floorPathfinders[floorNum] = pathfinder;
            return pathfinder;
        }
        #endregion


        #region Item Spawning
        public void SpawnItemsForAllFloors()
        {
            if (DungeonManager.Instance == null)
            {
                Debug.LogError("DungeonManager is not initialized. Cannot spawn items.");
                return;
            }
            ConsumableItem item = spawningRoomPotion;
            GameObject potion = new GameObject("SpawningRoomPotion");
            potion.transform.position = new Vector3(-15f, -11f, 0);
            List<Equipment> equipmentDrops = new List<Equipment>();
            List<ConsumableItem> consumableDrops = new List<ConsumableItem>();
            foreach (var kvp in DungeonManager.Instance.floors.OrderBy(e => e.Key))
            {
                int floorNum = kvp.Key;
                if (floorNum == 0)
                    continue;
                FloorData floorData = DungeonManager.Instance.GetFloorData(floorNum);
                if (floorData == null)
                    continue;

                Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNum);
                if (floorParent == null)
                    continue;

                Transform itemParent = floorParent.Find("ItemParent");
                if (itemParent == null)
                {
                    Debug.LogError(
                        $"ItemParent not found for Floor {floorNum}. Skipping item spawning."
                    );
                    continue;
                }

                // Spawn consumables

                //consumableDrops = GetItemDrops();
                SpawnConsumables(floorNum, itemParent, dungeonSettings.numberOfItemsPerFloor);

                // Spawm hidden consumables
                //consumableDrops = GetItemDrops();
                SpawnConsumables(
                    floorNum,
                    itemParent,
                    dungeonSettings.numberOfHiddenConsumableItemsPerFloor,
                    hidden: true
                );

                equipmentDrops = GetEquipmentDrops(floorNum);
                SpawnEquipment(
                    floorNum,
                    itemParent,
                    equipmentDrops,
                    dungeonSettings.numberOfItemsPerFloor
                );
                equipmentDrops = GetEquipmentDrops(floorNum);
                SpawnEquipment(
                    floorNum,
                    itemParent,
                    equipmentDrops,
                    dungeonSettings.numberOfItemsPerFloor,
                    true
                );

                // Spawn money
                SpawnMoney(floorNum, itemParent, dungeonSettings.moneyCountPerFloor);
            }
        }

        private List<Equipment> GetEquipmentDrops(int floorNumber)
        {
            List<Equipment> equipmentDrops = new List<Equipment>();
            int equipmentTier = Mathf.Clamp(Mathf.FloorToInt((floorNumber - 1) / 2) + 1, 1, 3);

            for (int i = 0; i < dungeonSettings.numberOfItemsPerFloor; i++)
            {
                equipmentDrops.Add(EquipmentGenerator.GenerateRandomEquipment(equipmentTier));
            }

            return equipmentDrops;
        }

        private List<ConsumableItem> GetItemDrops()
        {
            List<ConsumableItem> consumableDrops = new List<ConsumableItem>();
            for (int i = 0; i < dungeonSettings.numberOfItemsPerFloor; i++)
            {
                ConsumableItem consumable = ConsumableItemGenerator.GenerateRandomConsumable();
                consumableDrops.Add(consumable);
            }
            return consumableDrops;
        }

        private void ApplyEquipmentModifiers(Equipment equipment, int modifierScale)
        {
            if (equipment.equipmentStats[Stat.Attack] > 0)
                equipment.equipmentStats[Stat.Attack] += modifierScale;
            if (equipment.equipmentStats[Stat.Defense] > 0)
                equipment.equipmentStats[Stat.Defense] += modifierScale;
            if (equipment.equipmentStats[Stat.MaxMagic] > 0)
                equipment.equipmentStats[Stat.MaxMagic] += modifierScale;
            if (equipment.equipmentStats[Stat.MaxHP] > 0)
                equipment.equipmentStats[Stat.MaxHP] += modifierScale;
            if (equipment.equipmentStats[Stat.MaxStamina] > 0)
                equipment.equipmentStats[Stat.MaxStamina] += modifierScale;
            if (equipment.equipmentStats[Stat.Intelligence] > 0)
                equipment.equipmentStats[Stat.Intelligence] += modifierScale;
            if (equipment.equipmentStats[Stat.Dexterity] > 0)
                equipment.equipmentStats[Stat.Dexterity] += modifierScale;
            if (equipment.equipmentStats[Stat.Speed] > 0)
                equipment.equipmentStats[Stat.Speed] += modifierScale;
            if (equipment.equipmentStats[Stat.CritChance] > 0)
                equipment.equipmentStats[Stat.CritChance] += modifierScale;
            if (equipment.equipmentStats[Stat.CritDamage] > 0)
                equipment.equipmentStats[Stat.CritDamage] += modifierScale;
            if (equipment.equipmentStats[Stat.ElementalDamage] > 0)
                equipment.equipmentStats[Stat.ElementalDamage] += modifierScale;
            if (equipment.equipmentStats[Stat.ChanceToInflictStatusEffect] > 0)
                equipment.equipmentStats[Stat.ChanceToInflictStatusEffect] += modifierScale;
            if (equipment.equipmentStats[Stat.StatusEffectDuration] > 0)
                equipment.equipmentStats[Stat.StatusEffectDuration] += modifierScale;
            if (equipment.equipmentStats[Stat.FireRate] > 0)
                equipment.equipmentStats[Stat.FireRate] += modifierScale;
            if (equipment.equipmentStats[Stat.Shield] > 0)
                equipment.equipmentStats[Stat.Shield] += modifierScale;
            if (equipment.equipmentStats[Stat.Accuracy] > 0)
                equipment.equipmentStats[Stat.Accuracy] += modifierScale;
            if (equipment.equipmentStats[Stat.Evasion] > 0)
                equipment.equipmentStats[Stat.Evasion] += modifierScale;


            var keys = new List<DamageType>(equipment.damageModifiers.Keys);
            foreach (var key in keys)
            {
                equipment.damageModifiers[key] += modifierScale;
            }
        }

        private void SpawnEquipment(
            int floorNumber,
            Transform parent,
            List<Equipment> equipmentItems,
            int itemCount,
            bool hidden = false
        )
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
                return;

            List<Vector2Int> tiles;

            if (hidden)
            {
                // Only choose wall tiles that are near floor tiles
                tiles = floorData
                    .WallTiles.Where(tile => IsNearFloorTile(tile, floorData, 1.4f))
                    .ToList();
            }
            else
            {
                tiles = floorData.FloorTiles.ToList();
            }

            if (tiles.Count < itemCount)
                itemCount = tiles.Count;

            for (int i = 0; i < itemCount; i++)
            {
                Vector2Int tilePos = tiles[Random.Range(0, tiles.Count)];
                tiles.Remove(tilePos);

                Vector3 spawnPos =
                    (
                        hidden
                            ? floorData.WallTilemap.CellToWorld(
                                new Vector3Int(tilePos.x, tilePos.y, 0)
                            )
                            : floorData.FloorTilemap.CellToWorld(
                                new Vector3Int(tilePos.x, tilePos.y, 0)
                            )
                    ) + new Vector3(0.5f, 0.5f, 0);

                Equipment equipment = equipmentItems[Random.Range(0, equipmentItems.Count)];

                GameObject itemObject = new GameObject(
                    $"Dungeon_Spawner_Equipment_{equipment.itemName}"
                );
                itemObject.transform.position = spawnPos;
                itemObject.transform.SetParent(parent);
                itemObject.tag = "Item";

                SpriteRenderer renderer = itemObject.AddComponent<SpriteRenderer>();
                itemObject.layer = LayerMask.NameToLayer("items");
                itemObject.AddComponent<CapsuleCollider2D>().isTrigger = true;
                itemObject.AddComponent<EquipmentPickup>();

                renderer.sprite =
                    equipment.baseSprite != null
                        ? equipment.baseSprite
                        : Resources.Load<Sprite>("Sprites/Items/Weapons/Default");

                renderer.sortingOrder = 3;

                if (hidden)
                {
                    itemObject.AddComponent<HiddenItemController>().isHidden = true;
                }
                itemObject.transform.localScale = new Vector3(2f, 2f, 0f);
                ApplyEquipmentModifiers(equipment, Random.Range(1, 3));
                itemObject.GetComponent<EquipmentPickup>().SetEquipment(equipment);
            }
        }

        private bool IsNearFloorTile(Vector2Int wallTile, FloorData floorData, float radius)
        {
            foreach (Vector2Int floorTile in floorData.FloorTiles)
            {
                if (Vector2.Distance(wallTile, floorTile) <= radius)
                {
                    return true; // This wall tile is close enough to a floor tile
                }
            }
            return false;
        }

        #endregion

        #region Consumable Spawning

        private void SpawnConsumables(
            int floorNumber,
            Transform parent,
            int itemCount,
            bool hidden = false
        )
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
                return;

            List<Vector2Int> tiles;
            if (hidden)
            {
                // Only choose wall tiles that are near floor tiles
                tiles = floorData
                    .WallTiles.Where(tile => IsNearFloorTile(tile, floorData, 1.4f))
                    .ToList();
            }
            else
            {
                tiles = floorData.FloorTiles.ToList();
            }

            if (tiles.Count < itemCount)
                itemCount = tiles.Count;

            for (int i = 0; i < itemCount; i++)
            {
                Vector2Int tilePos = tiles[Random.Range(0, tiles.Count)];
                tiles.Remove(tilePos);

                ConsumableItem coItem = ConsumableItemGenerator.GenerateRandomConsumable();
                if (coItem == null || string.IsNullOrEmpty(coItem.name))
                {
                    Debug.LogWarning("Invalid consumable item generated, skipping spawn");
                    continue;
                }

                Vector3 spawnPos =
                    (
                        hidden
                            ? floorData.WallTilemap.CellToWorld(
                                new Vector3Int(tilePos.x, tilePos.y, 0)
                            )
                            : floorData.FloorTilemap.CellToWorld(
                                new Vector3Int(tilePos.x, tilePos.y, 0)
                            )
                    ) + new Vector3(0.5f, 0.5f, 0);

                try
                {
                    GameObject itemObject = new GameObject($"Consumable_{coItem.name}");
                    itemObject.transform.SetParent(parent);

                    itemObject.transform.position = spawnPos;
                    itemObject.tag = "Item";

                    var renderer = itemObject.AddComponent<SpriteRenderer>();
                    if (coItem.icon == null)
                    {
                        Debug.LogError($"Consumable {coItem.name} has no icon");
                        Destroy(itemObject);
                        continue;
                    }
                    renderer.sprite = coItem.icon;
                    renderer.sortingOrder = 3;

                    var collider = itemObject.AddComponent<CapsuleCollider2D>();
                    collider.isTrigger = true;

                    var pickup = itemObject.AddComponent<ConsumablePickup>();
                    pickup.SetConsumable(coItem);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to spawn consumable: {e.Message}");
                    continue;
                }
            }
        }
        #endregion


        #region Money Spawning
        private void SpawnMoney(int floorNumber, Transform parent, int moneyCount)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"No FloorData for floor {floorNumber} to spawn money.");
                return;
            }

            List<Vector2Int> floorTiles = floorData.FloorTiles.ToList();

            if (floorTiles.Count < moneyCount)
            {
                Debug.LogWarning($"Floor {floorNumber} has only {floorTiles.Count} valid tiles.");
                moneyCount = floorTiles.Count;
            }

            for (int i = 0; i < moneyCount; i++)
            {
                int randIndex = Random.Range(0, floorTiles.Count);
                Vector2Int tilePos = floorTiles[randIndex];
                floorTiles.RemoveAt(randIndex);

                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 spawnPos =
                    floorData.FloorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);

                if (
                    dungeonSettings.moneyPrefab == null
                    || dungeonSettings.moneyPrefab.moneyData == null
                )
                {
                    Debug.LogError("Money prefab is null, skipping spawn.");
                    continue;
                }

                // Create a random variation for the money amount
                int baseAmount = dungeonSettings.moneyPrefab.moneyData.GetBaseAmount();
                int randomVariation = Random.Range(-baseAmount / 5, baseAmount / 5); // ±20% of base amount
                int scaledAmount = Mathf.Max(1, baseAmount + randomVariation); // Ensure non-negative value

                MoneyPickup moneyPickup = Instantiate(
                        dungeonSettings.moneyPrefab,
                        spawnPos,
                        Quaternion.identity,
                        parent
                    )
                    .GetComponent<MoneyPickup>();

                moneyPickup.Initialize(dungeonSettings.moneyPrefab.moneyData, scaledAmount);
            }
        }

        #endregion


        #region Ambush/Boss Spawning (optional partial refactor)
        public void SpawnAmbush(Vector3 center, FloorData floorData, Transform enemyParent)
        {
            List<Vector2Int> spawnPositions = GetValidSpawnPositions(
                dungeonSettings.ambushEnemyCount,
                5f,
                floorData.FloorTilemap.WorldToCell(center),
                floorData
            );

            if (spawnPositions.Count == 0)
            {
                Debug.LogWarning("No valid ambush spawn positions found.");
                return;
            }

            var pathfinder = GetOrCreateFloorPathfinder(floorData);
            if (pathfinder == null)
            {
                Debug.LogError("No pathfinder found for ambush spawn.");
                return;
            }

            for (int i = 0; i < spawnPositions.Count; i++)
            {
                Vector2Int spawnPos = spawnPositions[i];
                Vector3 worldPos =
                    floorData.FloorTilemap.CellToWorld(new Vector3Int(spawnPos.x, spawnPos.y, 0))
                    + new Vector3(0.5f, 0.5f, 0);

                Monster monsterData = MonsterGenerator.GetRandomMonster();
                if (monsterData == null)
                {
                    Debug.LogWarning("No monster found for ambush spawn.");
                    continue;
                }
                MonsterInitializer.CalculateMonsterBaseStatsFromLevel(
                    monsterData,
                    floorData.FloorNumber
                );

                GameObject enemyGO = new GameObject($"Enemy_{monsterData.name}");
                enemyGO.AddComponent<Rigidbody2D>();

                RectTransform enemyRect = enemyGO.AddComponent<RectTransform>();
                enemyRect.sizeDelta = new Vector2(1, 1);

                enemyGO.transform.position = worldPos;
                enemyGO.transform.SetParent(enemyParent);

                var col = enemyGO.AddComponent<CircleCollider2D>();
                col.isTrigger = false;

                _EnemyStats statsComp = enemyGO.AddComponent<_EnemyStats>();
                statsComp.monsterData = monsterData;
                statsComp.spawnFloorLevel = floorData.FloorNumber;

                MonsterInitializer.InitializeEnemy(
                    enemyGO,
                    pathfinder,
                    floorData,
                    occupantIDCounter,
                    dungeonSettings,
                    obstacleLayer
                );
            }
        }

        #endregion

        private List<Vector2Int> GetValidSpawnPositions(
            int count,
            float radius,
            Vector3Int centerTile,
            FloorData floorData
        )
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            int attempts = 0;
            int maxAttempts = count * 10;

            while (positions.Count < count && attempts < maxAttempts)
            {
                int offsetX = Random.Range(-Mathf.CeilToInt(radius), Mathf.CeilToInt(radius) + 1);
                int offsetY = Random.Range(-Mathf.CeilToInt(radius), Mathf.CeilToInt(radius) + 1);

                Vector2Int candidate = new Vector2Int(
                    centerTile.x + offsetX,
                    centerTile.y + offsetY
                );

                if (floorData.FloorTiles.Contains(candidate) && !positions.Contains(candidate))
                {
                    positions.Add(candidate);
                }
                attempts++;
            }

            if (positions.Count < count)
            {
                Debug.LogWarning(
                    "GetValidSpawnPositions: Unable to find enough valid spawn positions."
                );
            }

            return positions;
        }
    }
}
