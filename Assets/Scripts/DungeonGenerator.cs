using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Linq;
using YourGameNamespace;
namespace YourGameNamespace
{
    public class DungeonGenerator : MonoBehaviour
    {
        public static DungeonGenerator Instance { get; private set; }
   
        [Header("Dungeon Settings")]
        [SerializeField] private Vector2Int dungeonSizeRange = new Vector2Int(200, 200);
        [SerializeField] private int maxFloors = 5;
        [SerializeField] private int seed;

        [Header("Room Settings")]
        [SerializeField] private Vector2Int roomSizeRange = new Vector2Int(15, 12);
        [SerializeField] private int maxBSPIterations = 6;
        [SerializeField] private int carvingIterationsMin = 2;
        [SerializeField] private int carvingIterationsMax = 5;

        [Header("Corridor Settings")]
        [SerializeField, Range(0.0f, 1.0f)] private float connectionChance = 0.13f;
        [SerializeField, Range(1, 3)] private int minCorridorWidth = 2;
        [SerializeField, Range(2, 4)] private int maxCorridorWidth = 3;
        [SerializeField] private int maxSegmentLength = 5;
        [SerializeField] private float straightCorridorChance = 0.3f;

        [Header("Prefabs")]
        [SerializeField] private GameObject spawningRoomPrefab; // Reference to the spawning room prefab
        [SerializeField] private GameObject stairsUpPrefab;
        [SerializeField] private GameObject stairsDownPrefab;

        [Header("Tile Settings")]
        public TileBase floorTile, corridorTile, wallTile; // Tilebases for floors, corridors, and walls

        [Header("Parent Object")]
        public GameObject dungeonParent; // Parent object for hierarchy organization

        // Storage for each generated floor's GameObject for management
        private List<GameObject> floorObjects = new List<GameObject>();

        [Header("Enemy Settings")]
        [SerializeField] private List<GameObject> enemyPrefabs;
        [SerializeField] private int numberOfEnemiesPerFloor = 5;
        [SerializeField] private int numberOfPatrolPointsPerFloor = 10;
        [SerializeField] private DungeonSettings dungeonSettings; // Assign via Inspector
        [SerializeField] private Transform playerTransform;       // Assign via Inspector or dynamically
        private EnemySpawner enemySpawner;                        // EnemySpawner instance

        // Exit point for the player to transition to the dungeon
        private Vector3 exitPoint;

        // Reference to the spawning room instance
        public GameObject spawningRoomInstance { get; private set; }

        // Instance of DungeonGeneratorMethods for utility functions
        private DungeonGeneratorMethods methods;
        
        // List to hold data for each floor
        private List<FloorData> floors = new List<FloorData>();
        private Dictionary<int, List<Vector3>> floorPatrolPoints = new Dictionary<int, List<Vector3>>(); // Store patrol points for each floor

        
        public bool IsDungeonGenerated { get; private set; } = false;

        // Additional settings for tilemap management
        private Tilemap floorTilemap;
        private Tilemap corridorTilemap;
        private Tilemap wallTilemap;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                  
                methods = new DungeonGeneratorMethods(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (dungeonParent != null)
            {
                if (dungeonParent.scene.name != "DontDestroyOnLoad")
                {
                    DontDestroyOnLoad(dungeonParent);
                    Debug.Log($"{dungeonParent.name} added to the Do Not Destroy list.");
                }
                else
                {
                        Destroy(dungeonParent);
                        return;
                }
                if (dungeonParent == null)
                {
                    Debug.LogError("DungeonParent is not assigned in the Inspector.");
                    return;
                }

            }
        }

        private void Start()
        {
            // Ensure dungeonSettings and playerTransform are assigned
            if (dungeonSettings == null || playerTransform == null)
            {
                Debug.LogError("DungeonSettings or PlayerTransform is not assigned in the Inspector.");
                return;
            }

            // Initialize EnemySpawner
            enemySpawner = new EnemySpawner(dungeonSettings, playerTransform);

            InstantiateSpawningRoom();
            StartCoroutine(GenerateDungeonCoroutine());
            StartCoroutine(InitializeDependencies());
        }

        private IEnumerator InitializeDependencies()
        {
            yield return null;

            for (int i = 0; i < floors.Count - 1; i++)
            {
                methods.PlaceStairs(floors[i], floors[i + 1], stairsUpPrefab, stairsDownPrefab);
            }
        }

private IEnumerator GenerateDungeonCoroutine()
{
    yield return GenerateDungeon();
    Debug.Log("Dungeon generation completed.");

    yield return StartCoroutine(InitializeDependencies());
    yield return StartCoroutine(enemySpawner.SpawnEnemiesForAllFloorsAsync());
}




        private IEnumerator GenerateDungeon()
        {
            Random.InitState(seed);

            for (int floorNum = 1; floorNum <= maxFloors; floorNum++)
            {
                // Create a container for each floor
                GameObject floorObj = new GameObject($"Floor_{floorNum}");
                floorObj.transform.parent = dungeonParent.transform;
                Debug.Log($"Floor {floorNum} added to floorObjects.");
                floorObjects.Add(floorObj);

                
                // Create separate Tilemaps for floor, corridors, and walls
                Tilemap floorTilemap = CreateTilemap(floorObj.transform, "FloorTilemap");
                Tilemap corridorTilemap = CreateTilemap(floorObj.transform, "CorridorTilemap");
                Tilemap wallTilemap = CreateTilemap(floorObj.transform, "WallTilemap");

                // Initialize FloorData
                FloorData floor = new FloorData(floorNum);
                floor.InitializeTilemaps(floorTilemap, corridorTilemap, wallTilemap);
                floors.Add(floor);
               // enemySpawner.GeneratePatrolPoints(floor);

                // Generate floor layout with rooms
                Rect initialRect = new Rect(-100, -100, Random.Range(dungeonSizeRange.x, dungeonSizeRange.y), Random.Range(dungeonSizeRange.x, dungeonSizeRange.y));
                methods.GenerateFloorTiles(initialRect, maxBSPIterations, floor, roomSizeRange, carvingIterationsMin, carvingIterationsMax);

                // Render and add each room
                foreach (Room room in floor.Rooms)
                {
                    methods.RenderRoom(room, floorTilemap, floorTile);
                 //   floor.AddRoom(room);
                }
                yield return null; // Yield after room generation

                // Connect rooms using corridors
                methods.ConnectRoomsUsingMST(floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance, connectionChance);

                // Render and add each corridor
                foreach (Corridor corridor in floor.Corridors)
                {
                    methods.RenderCorridor(corridor.CorridorTiles, corridorTilemap, corridorTile);
                  //  floor.AddCorridor(corridor.CorridorTiles, corridor.RoomA, corridor.RoomB);
                }
                yield return null; // Yield after corridor generation

                // Place walls around the combined floor tiles
                methods.CreateWallsForFloor(floor.FloorTiles, wallTilemap, wallTile);

                // Log floor summary
                Debug.Log($"Floor {floorNum} generated with {floor.Rooms.Count} rooms and {floor.Corridors.Count} corridors.");

                yield return null; 

                floor.GeneratePatrolPoints(50); // Generate shared patrol points
                DungeonManager.Instance.AddFloor(floor, floorObj.transform);
                // Validate patrol points
                Debug.Log($"Floor {floorNum} patrol points: {string.Join(", ", floor.PatrolPoints)}");
                // Add the floor transform to the dictionary
                if (!DungeonManager.Instance.FloorTransforms.ContainsKey(floorNum))
                {
                    DungeonManager.Instance.FloorTransforms[floorNum] = floorObj.transform;
                }
                
            }

            enemySpawner.InitializeEnemyParents();
            StartCoroutine(HideFloors()); // Set all floors inactive until the player enters
        }

        // Helper method to create a Tilemap under a specified parent transform
        private Tilemap CreateTilemap(Transform parent, string tilemapName)
        {
            GameObject tilemapObj = new GameObject(tilemapName);
            tilemapObj.transform.parent = parent;
            tilemapObj.transform.localPosition = new Vector3(-0.5f, -0.5f, 0);

            // Add Grid and Tilemap components
            Grid grid = tilemapObj.AddComponent<Grid>();
            Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
            tilemapObj.AddComponent<TilemapRenderer>();

            return tilemap;
        }

        public Vector3 GetExitPoint()
        {
            return methods.GetRandomFloorTile();
        }

        private void InstantiateSpawningRoom()
        {
            if (spawningRoomPrefab != null)
            {
                Vector3 spawnPosition = new Vector3(0, 0, 0); // Adjust this position as needed
                spawningRoomInstance = Instantiate(spawningRoomPrefab, spawnPosition, Quaternion.identity);
                spawningRoomInstance.transform.parent = dungeonParent.transform;
              //  Debug.Log("DungeonGenerator: Spawning room instantiated.");
            }
            else
            {
               // Debug.LogError("DungeonGenerator: Spawning room prefab is not assigned.");
            }
        }

        /// <summary>
        /// Validates and logs the integrity of FloorData for all floors.
        /// </summary>
        public void ValidateFloorData()
        {
            if (floors == null || floors.Count == 0)
            {
                Debug.LogError("No floor data available. Ensure the dungeon is generated before validating.");
                return;
            }

            foreach (FloorData floor in floors)
            {
                Debug.Log($"Validating Floor {floor.FloorNumber}...");

                // Validate floor tiles
                if (floor.FloorTiles == null || floor.FloorTiles.Count == 0)
                {
                    Debug.LogError($"Floor {floor.FloorNumber}: No floor tiles found.");
                }
                else
                {
                    Debug.Log($"Floor {floor.FloorNumber}: {floor.FloorTiles.Count} floor tiles.");
                }

                // Validate rooms
                if (floor.Rooms == null || floor.Rooms.Count == 0)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: No rooms found.");
                }
                else
                {
                    Debug.Log($"Floor {floor.FloorNumber}: {floor.Rooms.Count} rooms.");
                }

                // Validate corridors
                if (floor.Corridors == null || floor.Corridors.Count == 0)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: No corridors found.");
                }
                else
                {
                    Debug.Log($"Floor {floor.FloorNumber}: {floor.Corridors.Count} corridors.");
                }

                // Validate patrol points
                if (floor.PatrolPoints == null || floor.PatrolPoints.Count == 0)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: No patrol points assigned.");
                }
                else
                {
                    Debug.Log($"Floor {floor.FloorNumber}: {floor.PatrolPoints.Count} patrol points.");
                }

                // Validate tilemaps
                if (floor.FloorTilemap == null)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: Floor tilemap is null.");
                }
                if (floor.CorridorTilemap == null)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: Corridor tilemap is null.");
                }
                if (floor.WallTilemap == null)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: Wall tilemap is null.");
                }

                // Log summary of connections
                if (floor.Connections == null || floor.Connections.Count == 0)
                {
                    Debug.LogWarning($"Floor {floor.FloorNumber}: No connections between rooms.");
                }
                else
                {
                    Debug.Log($"Floor {floor.FloorNumber}: {floor.Connections.Count} room connections.");
                }
            }

            Debug.Log("Floor data validation completed.");
        }


        public void TransportPlayerToDungeon(GameObject player)
        {
            Vector3 exitPosition = GetExitPoint();
            player.transform.position = exitPosition;
            player.GetComponent<Rigidbody2D>().MovePosition(exitPosition); // Update Rigidbody position
            player.GetComponent<PlayerManager>().UpdateCurrentTilePosition(); // Updated line
            Camera.main.GetComponent<CameraController>().SetPlayerTransform(player.transform);
            StartCoroutine(ShowFloor(1));
            Debug.Log("Player transported to dungeon.");
        }


        private IEnumerator ShowFloor(int floorNumber)
        {
            HideFloors(); // First deactivate all floors

            if (floorNumber > 0 && floorNumber <= floorObjects.Count)
            {
                GameObject floorObj = floorObjects[floorNumber - 1];
                floorObjects[floorNumber - 1].SetActive(true);
                yield return null;
            }
        }

        private IEnumerator HideFloors()
        {
            foreach (GameObject floorObj in floorObjects)
            {
                floorObj.SetActive(false);
            }
                yield return null;
        }

    }
}
