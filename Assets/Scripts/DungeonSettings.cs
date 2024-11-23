using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace YourGameNamespace
{
    [CreateAssetMenu(fileName = "DungeonSettings", menuName = "GameSettings/DungeonSettings")]
    public class DungeonSettings : ScriptableObject
    {
        [Header("General Dungeon Settings")]
        [Tooltip("The range for dungeon size (width x height) in tiles.")]
        public Vector2Int dungeonSizeRange = new Vector2Int(200, 200);

        [Tooltip("Maximum number of dungeon floors.")]
        public int maxFloors = 5;

        [Tooltip("Random seed for procedural generation. Set to 0 for random seed each run.")]
        public int seed;

        [Header("Room Settings")]
        [Tooltip("The range for room sizes (width x height) in tiles.")]
        public Vector2Int roomSizeRange = new Vector2Int(15, 12);

        [Tooltip("Maximum iterations for Binary Space Partitioning.")]
        public int maxBSPIterations = 6;

        [Tooltip("Minimum carving iterations to connect rooms.")]
        public int carvingIterationsMin = 2;

        [Tooltip("Maximum carving iterations to connect rooms.")]
        public int carvingIterationsMax = 5;

        [Header("Corridor Settings")]
        [Tooltip("Chance to create a corridor connection between rooms.")]
        [Range(0.0f, 1.0f)] public float connectionChance = 0.13f;

        [Tooltip("Minimum width for corridors.")]
        [Range(1, 3)] public int minCorridorWidth = 2;

        [Tooltip("Maximum width for corridors.")]
        [Range(2, 4)] public int maxCorridorWidth = 3;

        [Tooltip("Maximum length of a single corridor segment.")]
        public int maxSegmentLength = 5;

        [Tooltip("Chance for corridors to remain straight.")]
        public float straightCorridorChance = 0.3f;

        [Header("Tile Settings")]
        [Tooltip("Tile used for dungeon floors.")]
        public TileBase floorTile;

        [Tooltip("Tile used for corridors.")]
        public TileBase corridorTile;

        [Tooltip("Tile used for walls.")]
        public TileBase wallTile;

        [Header("Prefab Settings")]
        [Tooltip("Prefab for the spawning room.")]
        public GameObject spawningRoomPrefab;

        [Tooltip("Prefab for stairs leading up.")]
        public GameObject stairsUpPrefab;

        [Tooltip("Prefab for stairs leading down.")]
        public GameObject stairsDownPrefab;

        [Tooltip("Prefab for spawn effects (e.g., particle effects).")]
        public GameObject spawnEffectPrefab;

        [Header("Enemy Settings")]
        [Tooltip("List of enemy prefabs to use in the dungeon.")]
        public List<GameObject> enemyPrefabs = new List<GameObject>();

        [Tooltip("List of boss prefabs to use for special encounters.")]
        public List<GameObject> bossPrefabs = new List<GameObject>();

        [Tooltip("Default number of enemies to spawn per floor.")]
        public int numberOfEnemiesPerFloor = 5;

        [Header("Scaling Settings")]
        [Tooltip("Enable adaptive difficulty scaling for enemies.")]
        public bool enableEnemyScaling = true;

        [Tooltip("Base health multiplier for enemies.")]
        public float baseHealthMultiplier = 1.0f;

        [Tooltip("Base damage multiplier for enemies.")]
        public float baseDamageMultiplier = 1.0f;

        [Tooltip("Multiplier per floor for enemy difficulty.")]
        public float floorDifficultyFactor = 0.2f;

        [Tooltip("Multiplier per player level for enemy difficulty.")]
        public float playerLevelFactor = 0.1f;

        [Tooltip("Maximum level enemies can scale to.")]
        public int maxEnemyLevel = 20;

        [Header("Boss Settings")]
        [Tooltip("Enable boss fights in the dungeon.")]
        public bool enableBossFights = true;

        [Tooltip("Frequency of boss fights (e.g., every N floors).")]
        public int bossSpawnFloorInterval = 3;

        [Header("Ambush Settings")]
        [Tooltip("Enable ambush mechanics where enemies spawn near the player.")]
        public bool enableAmbush = true;

        [Tooltip("Number of enemies to spawn during an ambush.")]
        public int ambushEnemyCount = 3;

        [Tooltip("Radius around the player for ambush enemy spawn points.")]
        public float ambushRadius = 3f;

        [Header("Debug Settings")]
        [Tooltip("Enable debug logs for dungeon generation.")]
        public bool enableDebugLogs = false;

        [Tooltip("Enable visualization for patrol routes.")]
        public bool visualizePatrolRoutes = false;

        [Tooltip("Enable visualization for enemy spawn positions.")]
        public bool visualizeSpawnPositions = false;
    }
}
