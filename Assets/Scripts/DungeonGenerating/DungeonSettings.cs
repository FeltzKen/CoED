using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

namespace CoED
{
    [CreateAssetMenu(fileName = "DungeonSettings", menuName = "GameSettings/DungeonSettings")]
    public class DungeonSettings : ScriptableObject
    {
        [Header("Dungeon Dimensions")]
        [Tooltip("The range for the dungeon's width and height.")]
        public Vector2Int dungeonSizeRange = new Vector2Int(200, 200);

        [Tooltip("Maximum number of dungeon floors.")]
        public int maxFloors = 5;

        [Header("Tile base Settings")]
        [Tooltip("Tile to use for floor generation.")]
        public TileBase floorTile;

        [Tooltip("Tile to use for wall generation.")]
        public TileBase wallTile;
 
        [Tooltip("List of item prefabs to spawn in the dungeon.")]
        public List<GameObject> itemPrefabs = new List<GameObject>();


        [Tooltip("Random seed for procedural generation. Set to 0 for random seed each run.")]
        public int seed;

        [Header("Room Settings")]
        [Tooltip("The range for room sizes (width x height) in tiles.")]
        public Vector2Int roomSizeRange = new Vector2Int(15, 12);

        [Header("Generation Algorithms")]
        [Tooltip("Prioritized list of algorithms for generating the dungeon.")]
        [Header("Selected Algorithm")]
        public AlgorithmConfig selectedAlgorithm = new AlgorithmConfig();  
        
        [HideInInspector][Range(0f, 1f)] public float initialWallDensity = 0.45f;
        [HideInInspector][Range(1, 10)] public int cellularAutomataIterations = 5;
        [HideInInspector][Range(0, 8)] public int neighborWallThreshold = 4;
        [HideInInspector][Range(1f, 10f)] public float edgeBias = 3.0f;
        [HideInInspector][Range(1, 10)] public int perlinNoiseIterations = 3;
        [HideInInspector][Range(1, 10)] public int bspSubdivisions = 3;
        [HideInInspector][Range(0f, 1f)] public float bspCarveChance = 0.5f;
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

        [Header("Wall Settings")]
        [Tooltip("Generate outer walls around the entire dungeon.")]
        public bool generateOuterWalls = true;

        [Header("Debug Settings")]
        [Tooltip("Enable debug logs for dungeon generation.")]
        public bool enableDebugLogs = false;

        [Tooltip("Enable visualization for patrol routes.")]
        public bool visualizePatrolRoutes = false;

        [Tooltip("Enable visualization for enemy spawn positions.")]
        public bool visualizeSpawnPositions = false;

        [Serializable]
        public class AlgorithmConfig
        {
            public CarvingAlgorithmType algorithmType;
            public float initialWallDensity;
            public int iterations;
            public int neighborWallThreshold;
            public float edgeBias;
        }


    }
}
