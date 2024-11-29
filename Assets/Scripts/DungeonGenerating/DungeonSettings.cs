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

    [Header("Tile Palette")]
    [SerializeField] public TilePalette tilePalette;

    // Expose the TilePalette instance for access
        public TilePalette TilePalette => tilePalette;


        [Tooltip("List of item prefabs to spawn in the dungeon.")]
        public List<GameObject> itemPrefabs = new List<GameObject>();


        [Tooltip("Random seed for procedural generation. Set to 0 for random seed each run.")]
        public int seed;

        [Header("Generation Algorithms")]
        [Tooltip("Prioritized list of algorithms for generating the dungeon.")]
        [Header("Selected Algorithm")]
        public AlgorithmConfig selectedAlgorithm = new AlgorithmConfig();  
        
        [Header("Prefab Settings")]
        [Tooltip("Prefab for the spawning room.")]
        public GameObject spawningRoomPrefab;

        [Tooltip("Prefab for spawn effects (e.g., particle effects).")]
        public GameObject spawnEffectPrefab;

        [Header("Enemy Settings")]
        [Tooltip("List of enemy prefabs to use in the dungeon.")]
        public List<GameObject> enemyPrefabs = new List<GameObject>();
        [Header("UI Elements")]
        [SerializeField] public GameObject healthBarPrefab;

        [Tooltip("List of boss prefabs to use for special encounters.")]
        public List<GameObject> bossPrefabs = new List<GameObject>();

        [Tooltip("Default number of enemies to spawn per floor.")]
        public int numberOfEnemiesPerFloor = 5;
        public int numberOfPatrolPoints = 10;
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


        [System.Serializable]
        public class AlgorithmConfig
        {
            [Header("Algorithm Type")]
            [Tooltip("Select the carving algorithm to use.")]
            public CarvingAlgorithmType algorithmType;

            [Header("Cellular Automata Settings")]
            [Tooltip("Initial wall density (value between 0 and 1).")]
            [Range(0f, 1f)]
            public float initialWallDensity = 0.4f;

            [Tooltip("Number of iterations for smoothing the map.")]
            public int iterations = 5;

            [Tooltip("Minimum neighbors required for a cell to become a wall.")]
            public int neighborWallThreshold = 4;

            [Header("Perlin Noise Settings")]
            [Tooltip("Scale factor for Perlin noise.")]
            public float edgeBias = 10f;

            [Header("BSP Settings")]
            [Tooltip("Number of subdivisions for BSP algorithm.")]
            public int bspSubdivisions = 4;

            [Tooltip("Chance to carve out a space.")]
            [Range(0f, 1f)]
            public float bspCarveChance = 0.7f;

            [Header("Spiral Pattern Settings")]
            [Tooltip("Step size for spiral generation.")]
            public int spiralStepSize = 1;

            [Header("Wave Function Collapse Settings")]
            [Tooltip("Weighting factor for tile probabilities.")]
            public float waveCollapseBias = 1.0f;

            [Header("Fractal Maze Settings")]
            [Tooltip("Maximum recursion depth for fractal subdivision.")]
            public int fractalDepth = 3;

            [Tooltip("Chance to split the area at each depth level.")]
            [Range(0f, 1f)]
            public float fractalSplitChance = 0.5f;

            [Header("L-System Settings")]
            [Tooltip("Number of iterations for the L-system.")]
            public int lSystemIterations = 3;

            [Tooltip("Chance for branching in the L-system.")]
            [Range(0f, 1f)]
            public float branchChance = 0.5f;

            [Header("Radial Growth Settings")]
            [Tooltip("Radius of radial growth.")]
            public int radialGrowthRadius = 10;

            [Tooltip("Chance for a cell to grow into a room.")]
            [Range(0f, 1f)]
            public float roomGrowthChance = 0.6f;

            [Tooltip("Chance for a cell to grow into a corridor.")]
            [Range(0f, 1f)]
            public float corridorGrowthChance = 0.4f;
        }


    }
}
