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
        private Dictionary<int, HashSet<Vector2Int>> walkableTilesByFloor = new Dictionary<int, HashSet<Vector2Int>>();

        
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
                    // // Debug.Log($"{dungeonParent.name} added to the Do Not Destroy list.");
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
            // // Debug.Log("Dungeon generation completed.");

            yield return StartCoroutine(InitializeDependencies());
            yield return StartCoroutine(enemySpawner.SpawnEnemiesForAllFloorsAsync());
        }

        private IEnumerator GenerateDungeon()
        {
            Random.InitState(seed);

            for (int floorNum = 1; floorNum <= maxFloors; floorNum++)
            {
                GameObject floorObj = new GameObject($"Floor_{floorNum}");
                floorObj.transform.parent = dungeonParent.transform;
                floorObjects.Add(floorObj);

                // Create separate Tilemaps for floor, corridors, and walls
                floorTilemap = CreateTilemap(floorObj.transform, "FloorTilemap");
                corridorTilemap = CreateTilemap(floorObj.transform, "CorridorTilemap");
                wallTilemap = CreateTilemap(floorObj.transform, "WallTilemap");

                FloorData floor = new FloorData(floorNum);
                floor.InitializeTilemaps(floorTilemap, corridorTilemap, wallTilemap);
                floors.Add(floor);

                // Generate floor layout with rooms
                Rect initialRect = new Rect(-100, -100, Random.Range(dungeonSizeRange.x, dungeonSizeRange.y), Random.Range(dungeonSizeRange.x, dungeonSizeRange.y));
                methods.GenerateFloorTiles(initialRect, maxBSPIterations, floor, roomSizeRange, carvingIterationsMin, carvingIterationsMax);

                foreach (Room room in floor.Rooms)
                {
                    methods.RenderRoom(room, floorTilemap, floorTile);
                }
                yield return null;

                // Connect rooms using corridors
                methods.ConnectRoomsUsingMST(floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance, connectionChance);

                foreach (Corridor corridor in floor.Corridors)
                {
                    methods.RenderCorridor(corridor.CorridorTiles, corridorTilemap, corridorTile);
                }
                yield return null;

                // Place walls around the combined floor tiles
                methods.CreateWallsForFloor(floor.FloorTiles, wallTilemap, wallTile);
                yield return null;

                // Generate shared patrol points
                floor.GeneratePatrolPoints(50);
                DungeonManager.Instance.AddFloor(floor, floorObj.transform);

                if (!DungeonManager.Instance.FloorTransforms.ContainsKey(floorNum))
                {
                    DungeonManager.Instance.FloorTransforms[floorNum] = floorObj.transform;
                }

                HashSet<Vector2Int> walkableTiles = new HashSet<Vector2Int>();

                // Extract floor tiles
                foreach (var position in floorTilemap.cellBounds.allPositionsWithin)
                {
                    if (floorTilemap.HasTile(position))
                    {
                        walkableTiles.Add(new Vector2Int(position.x, position.y));
                    }
                }

                // Extract corridor tiles
                foreach (var position in corridorTilemap.cellBounds.allPositionsWithin)
                {
                    if (corridorTilemap.HasTile(position))
                    {
                        walkableTiles.Add(new Vector2Int(position.x, position.y));
                    }
                }

                walkableTilesByFloor[floorNum] = walkableTiles;
            }

            enemySpawner.InitializeEnemyParents();
            StartCoroutine(HideFloors());
        }


        private void LockTilemaps()
        {
            // Add logic here to lock or disable further rendering to the tilemaps.
            floorTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
            corridorTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
            wallTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = true; // Keep the walls visible
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
              //  // // Debug.Log("DungeonGenerator: Spawning room instantiated.");
            }
            else
            {
               // Debug.LogError("DungeonGenerator: Spawning room prefab is not assigned.");
            }
        }

public void TransportPlayerToDungeon(GameObject player)
{
    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

    if (walkableTilesByFloor.ContainsKey(1) && walkableTilesByFloor[1].Count > 0)
    {
        // Get a random tile from the walkable tiles of the first floor
        Vector2Int randomTile = walkableTilesByFloor[1].OrderBy(t => Random.value).First();
        Vector3 exitPosition = new Vector3(randomTile.x, randomTile.y, 0);
        // Update logical position and align physical position
        playerMovement.UpdateCurrentTilePosition(exitPosition);
        // Update the camera to follow the new player position
        Camera.main.GetComponent<CameraController>().SetPlayerTransform(player.transform);
    }

    // Show the first floor (assuming this is a visual effect or animation)
    StartCoroutine(ShowFloor(1));
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
