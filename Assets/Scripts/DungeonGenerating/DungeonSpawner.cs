using System.Collections.Generic;
using System.Linq;
using CoED.Items;
using CoED.Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
        private ConsumableItemWrapper spawningRoomPotion;
        private static int occupantIDCounter = 0;
        private Dictionary<int, Pathfinder> floorPathfinders = new Dictionary<int, Pathfinder>();
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

                SpawnEnemiesForFloor(floorNum, floorParent, enemyParent);
            }
        }

        private void SpawnEnemiesForFloor(
            int floorNumber,
            Transform floorParent,
            Transform enemyParent
        )
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"No FloorData for floor {floorNumber}.");
                return;
            }

            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            List<Vector2Int> allFloorTiles = DungeonManager
                .Instance.GetFloorData(floorNumber)
                .FloorTiles.ToList();

            List<Vector2Int> validTiles = allFloorTiles
                .Where(pos => IsValidSpawnPosition(floorData, pos))
                .ToList();

            int enemyCount = dungeonSettings.numberOfEnemiesPerFloor;
            if (validTiles.Count < enemyCount)
            {
                Debug.LogWarning(
                    $"Floor {floorNumber} does not have enough valid tiles for {enemyCount} enemies."
                );
                enemyCount = validTiles.Count;
            }

            if (enemyParent == null)
            {
                Debug.LogError(
                    $"EnemyParent not found under {floorParent.name}. Skipping floor {floorNumber}."
                );
                return;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                int randIndex = Random.Range(0, validTiles.Count);
                Vector2Int tilePos = validTiles[randIndex];
                validTiles.RemoveAt(randIndex);

                Vector3Int cellPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                Vector3 baseWorldPos = floorData.FloorTilemap.CellToWorld(cellPos);

                Vector3 finalSpawnPos = floorParent.position + baseWorldPos;

                GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                    Random.Range(0, dungeonSettings.enemyPrefabs.Count)
                ];
                if (enemyPrefab == null)
                {
                    Debug.LogError($"Null enemy prefab for floor {floorNumber}, skipping...");
                    continue;
                }

                GameObject enemy = Instantiate(
                    enemyPrefab,
                    baseWorldPos,
                    Quaternion.identity,
                    enemyParent
                );
                if (enemy == null)
                {
                    Debug.LogError(
                        $"Failed to instantiate enemy at {finalSpawnPos} for floor {floorNumber}."
                    );
                    continue;
                }

                occupantIDCounter++;
                int occupantID = occupantIDCounter;

                EnemyStats stats = enemy.GetComponent<EnemyStats>();
                if (stats != null)
                    stats.spawnFloor = floorNumber;

                EnemyNavigator navigator = enemy.GetComponent<EnemyNavigator>();
                if (navigator == null)
                    navigator = enemy.AddComponent<EnemyNavigator>();
                navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

                EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
                if (brain == null)
                    brain = enemy.AddComponent<EnemyBrain>();
                HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>(
                    floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                );
                brain.Initialize(floorData, patrolPoints);
            }
        }

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
            Instantiate(spawningRoomPotion, new Vector3(-15f, -11f, 0), Quaternion.identity);
            List<Equipment> equipmentDrops = new List<Equipment>();
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
                SpawnConsumables(
                    floorNum,
                    itemParent,
                    dungeonSettings
                        .itemPrefabs.Select(wrapper => wrapper.consumableData as Item)
                        .ToList(),
                    dungeonSettings.numberOfItemsPerFloor
                );

                // Spawm hidden consumables
                SpawnConsumables(
                    floorNum,
                    itemParent,
                    dungeonSettings
                        .itemPrefabs.Select(wrapper => wrapper.consumableData as Item)
                        .ToList(),
                    dungeonSettings.numberOfHiddenConsumableItemsPerFloor,
                    hidden: true
                );

                equipmentDrops = GetEquipmentDrops();
                SpawnEquipment(
                    floorNum,
                    itemParent,
                    equipmentDrops,
                    dungeonSettings.numberOfItemsPerFloor
                );
                equipmentDrops = GetEquipmentDrops();
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
            foreach (var item in dungeonSettings.equipmentPrefabs)
            {
                Debug.Log(
                    item != null
                        ? $"Loaded item: {item.name} (Type: {item.GetType().Name})----------------------"
                        : "NULL item in the list!----------------------------"
                );
            }
        }

        private List<Equipment> GetEquipmentDrops()
        {
            List<Equipment> equipmentDrops = new List<Equipment>();

            for (int i = 0; i < dungeonSettings.numberOfItemsPerFloor; i++)
            {
                float roll = Random.value;
                if (roll < 0.33f)
                    equipmentDrops.Add(EquipmentGenerator.GenerateRandomEquipment(1, "weapon"));
                else if (roll < 0.66f)
                    equipmentDrops.Add(EquipmentGenerator.GenerateRandomEquipment(1, "armor"));
                else
                    equipmentDrops.Add(EquipmentGenerator.GenerateRandomEquipment(1, "accessory"));
            }

            return equipmentDrops;
        }

        private void ApplyEquipmentModifiers(Equipment equipment, int modifierScale)
        {
            if (equipment.attack > 0)
                equipment.attack += modifierScale;
            if (equipment.defense > 0)
                equipment.defense += modifierScale;
            if (equipment.speed > 0)
                equipment.speed += modifierScale;
            if (equipment.health > 0)
                equipment.health += modifierScale;
            if (equipment.magic > 0)
                equipment.magic += modifierScale;
            if (equipment.stamina > 0)
                equipment.stamina += modifierScale;
            if (equipment.intelligence > 0)
                equipment.intelligence += modifierScale;
            if (equipment.dexterity > 0)
                equipment.dexterity += modifierScale;
            if (equipment.critChance > 0)
                equipment.critChance += modifierScale;

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
            List<Item> consumableItems,
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
                tiles = floorData.WallTiles.ToList();
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

                Consumable coItem =
                    consumableItems[Random.Range(0, consumableItems.Count)] as Consumable;

                if (coItem != null)
                {
                    GameObject itemObject = Instantiate(
                        coItem.itemPrefab,
                        spawnPos,
                        Quaternion.identity,
                        parent
                    );
                    var consumableWrapper = itemObject.GetComponent<ConsumableItemWrapper>();
                    if (consumableWrapper != null)
                    {
                        consumableWrapper.consumableData = ScriptableObject.Instantiate(coItem);
                        ApplyConsumableModifiers(consumableWrapper, Random.Range(1.1f, 1.5f));
                    }
                }
            }
        }
        #endregion

        #region Utility Methods
        private void ApplyConsumableModifiers(
            ConsumableItemWrapper consumableWrapper,
            float modifierScale
        )
        {
            if (consumableWrapper.consumableData.attackBoost > 0)
                consumableWrapper.consumableData.attackBoost += modifierScale;
            if (consumableWrapper.consumableData.defenseBoost > 0)
                consumableWrapper.consumableData.defenseBoost += modifierScale;
            if (consumableWrapper.consumableData.speedBoost > 0)
                consumableWrapper.consumableData.speedBoost += modifierScale;
            if (consumableWrapper.consumableData.healthBoost > 0)
                consumableWrapper.consumableData.healthBoost += modifierScale;
            if (consumableWrapper.consumableData.magicBoost > 0)
                consumableWrapper.consumableData.magicBoost += modifierScale;
            if (consumableWrapper.consumableData.staminaBoost > 0)
                consumableWrapper.consumableData.staminaBoost += modifierScale;
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
            if (floorData == null || floorData.FloorTilemap == null || floorData.FloorTiles == null)
            {
                Debug.LogError("DungeonSpawner: Invalid floor data. Cannot spawn ambush.");
                return;
            }

            Vector3Int centerTile = floorData.FloorTilemap.WorldToCell(center);

            List<Vector2Int> spawnPositions = GetValidSpawnPositions(
                dungeonSettings.ambushEnemyCount,
                dungeonSettings.ambushRadius,
                centerTile,
                floorData
            );

            if (spawnPositions.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No valid spawn positions found for ambush.");
                return;
            }

            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);

            foreach (Vector2Int tilePos in spawnPositions)
            {
                PositionHelper.InstantiateOnTile(
                    floorData.FloorTilemap,
                    new Vector3Int(tilePos.x, tilePos.y, 0),
                    dungeonSettings.spawnEffectPrefab,
                    enemyParent
                );

                GameObject enemy = SpawnEnemyAtTile(tilePos, floorData, enemyParent);
                if (enemy != null)
                {
                    int occupantID = occupantIDCounter++;

                    EnemyNavigator navigator = enemy.GetComponent<EnemyNavigator>();
                    if (navigator == null)
                    {
                        navigator = enemy.GetComponent<EnemyNavigator>();
                    }
                    navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

                    EnemyBrain brain = enemy.GetComponent<EnemyBrain>();
                    if (brain == null)
                    {
                        brain = enemy.GetComponent<EnemyBrain>();
                    }

                    HashSet<Vector2Int> patrolPoints = new HashSet<Vector2Int>(
                        floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
                    );
                    brain.Initialize(floorData, patrolPoints);

                    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.spawnFloor = floorData.FloorNumber;
                    }
                }
            }
        }

        private GameObject SpawnEnemyAtTile(
            Vector2Int tilePosition,
            FloorData floorData,
            Transform parent
        )
        {
            if (dungeonSettings.enemyPrefabs == null || dungeonSettings.enemyPrefabs.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No enemy prefabs available.");
                return null;
            }

            GameObject enemyPrefab = dungeonSettings.enemyPrefabs[
                Random.Range(0, dungeonSettings.enemyPrefabs.Count)
            ];

            return PositionHelper.InstantiateOnTile(
                floorData.FloorTilemap,
                new Vector3Int(tilePosition.x, tilePosition.y, 0),
                enemyPrefab,
                parent
            );
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

        #region Boss Spawning
        public void SpawnBossOnFloor(int floorNumber)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError("DungeonSpawner: FloorData is null. Cannot spawn boss.");
                return;
            }

            Transform floorParent = DungeonManager.Instance.GetFloorTransform(floorNumber);
            Transform enemyParent = floorParent?.Find("EnemyParent");

            if (enemyParent == null)
            {
                Debug.LogError("DungeonSpawner: EnemyParent not found. Cannot spawn boss.");
                return;
            }

            GameObject bossPrefab = dungeonSettings.bossPrefabs[
                Random.Range(0, dungeonSettings.bossPrefabs.Count)
            ];

            SpawnBoss(bossPrefab, floorData, enemyParent);
        }

        private void SpawnBoss(GameObject bossPrefab, FloorData floorData, Transform enemyParent)
        {
            if (bossPrefab == null)
            {
                Debug.LogError("DungeonSpawner: No boss prefab available. Cannot spawn boss.");
                return;
            }

            List<Vector2Int> spawnPositions = DungeonManager
                .Instance.GetFloorData(floorData.FloorNumber)
                .FloorTiles.ToList();

            if (spawnPositions.Count == 0)
            {
                Debug.LogError("DungeonSpawner: No valid spawn positions found for boss.");
                return;
            }

            Pathfinder pathfinder = GetOrCreateFloorPathfinder(floorData);
            Vector2Int tilePos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            Vector3 pos = floorData.FloorTilemap.CellToWorld(
                new Vector3Int(tilePos.x, tilePos.y, 0)
            );

            Instantiate(dungeonSettings.spawnEffectPrefab, pos, Quaternion.identity);

            GameObject boss = Instantiate(bossPrefab, pos, Quaternion.identity, enemyParent);
            if (boss == null)
            {
                Debug.LogError("DungeonSpawner: Failed to instantiate boss.");
                return;
            }

            int occupantID = occupantIDCounter++;

            EnemyNavigator navigator = boss.GetComponent<EnemyNavigator>();
            if (navigator == null)
            {
                navigator = boss.AddComponent<EnemyNavigator>();
            }
            navigator.Initialize(pathfinder, floorData.FloorTilemap, occupantID);

            EnemyBrain brain = boss.GetComponent<EnemyBrain>();
            if (brain == null)
            {
                brain = boss.AddComponent<EnemyBrain>();
            }

            HashSet<Vector2Int> bossPatrolPoints = new HashSet<Vector2Int>(
                floorData.GetRandomFloorTiles(dungeonSettings.numberOfPatrolPoints)
            );
            brain.Initialize(floorData, bossPatrolPoints);

            EnemyStats bossStats = boss.GetComponent<EnemyStats>();
            if (bossStats != null)
            {
                bossStats.spawnFloor = floorData.FloorNumber;
            }
        }
        #endregion
    }
}
