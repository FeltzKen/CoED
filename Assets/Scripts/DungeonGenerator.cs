using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace YourGameNamespace
{
    public class DungeonGenerator : MonoBehaviour
    {
        public static DungeonGenerator Instance { get; private set; }

        [Header("Dependencies")]
        [SerializeField] private DungeonSettings dungeonSettings; // ScriptableObject for dungeon-wide settings
        [SerializeField] private GameObject dungeonParent; // Parent for organizing dungeon hierarchy
        [SerializeField] private GameObject player; // Reference to the player GameObject

        private FloorGenerator floorGenerator;
        private EnemySpawner enemySpawner;
        private PlayerTransporter playerTransporter;

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
                return;
            }

            InitializeDependencies();
        }

        private void Start()
        {
            StartCoroutine(GenerateDungeonCoroutine());
        }

        /// <summary>
        /// Initializes dependencies and managers.
        /// </summary>
        private void InitializeDependencies()
        {
            floorGenerator = new FloorGenerator(dungeonSettings);
            enemySpawner = new EnemySpawner(dungeonSettings, player.transform);

            // PlayerTransporter relies on GridManager for positioning
            if (DungeonManager.Instance.TryGetComponent(out GridManager gridManager))
            {
                playerTransporter = new PlayerTransporter(gridManager);
                gridManager.AdjustGridPosition(dungeonParent); // Ensure grid alignment
            }
            else
            {
                Debug.LogError("GridManager not found on DungeonManager. PlayerTransporter will not function correctly.");
            }

            DungeonManager.Instance.Floors.Clear(); // Reset floor data
        }

        /// <summary>
        /// Coroutine for generating the dungeon.
        /// </summary>
        private IEnumerator GenerateDungeonCoroutine()
        {
            Debug.Log("Starting dungeon generation...");

            // Generate each floor
            for (int floorNumber = 1; floorNumber <= dungeonSettings.maxFloors; floorNumber++)
            {
                yield return GenerateFloor(floorNumber);
            }

            // Spawn enemies after floors are generated
            yield return enemySpawner.SpawnEnemiesForAllFloorsAsync();

            Debug.Log("Dungeon generation completed.");
        }

        /// <summary>
        /// Generates an individual floor.
        /// </summary>
        private IEnumerator GenerateFloor(int floorNumber)
        {
            // Create a container for the floor
            GameObject floorObject = new GameObject($"Floor_{floorNumber}");
            floorObject.transform.parent = dungeonParent.transform;

            // Create tilemaps for floor elements
            Tilemap floorTilemap = CreateTilemap(floorObject.transform, "FloorTilemap");
            Tilemap corridorTilemap = CreateTilemap(floorObject.transform, "CorridorTilemap");
            Tilemap wallTilemap = CreateTilemap(floorObject.transform, "WallTilemap");

            // Generate the floor using FloorGenerator
            FloorData floorData = floorGenerator.GenerateFloor(
                floorNumber,
                floorTilemap,
                corridorTilemap,
                wallTilemap,
                dungeonSettings.floorTile,
                dungeonSettings.corridorTile,
                dungeonSettings.wallTile
            );

            // Register the floor with DungeonManager
            DungeonManager.Instance.AddFloor(floorData);

            yield return null;
        }

        /// <summary>
        /// Creates a tilemap under a specified parent.
        /// </summary>
        private Tilemap CreateTilemap(Transform parent, string name)
        {
            GameObject tilemapObject = new GameObject(name);
            tilemapObject.transform.parent = parent;

            Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
            tilemapObject.AddComponent<TilemapRenderer>();

            return tilemap;
        }

        /// <summary>
        /// Transports the player to the first floor of the dungeon.
        /// </summary>
        public void TransportPlayerToDungeon()
        {
            if (playerTransporter != null)
            {
                playerTransporter.TransportPlayerToFloor(player, 1);
            }
            else
            {
                Debug.LogError("PlayerTransporter is not initialized.");
            }
        }
    }
}
