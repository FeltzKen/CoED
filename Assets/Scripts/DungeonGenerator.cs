using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEditor.Callbacks;

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
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int numberOfEnemiesPerFloor = 5;

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


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                        // Initialize `methods` here
                methods = new DungeonGeneratorMethods(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        private void Start()
        {
            InstantiateSpawningRoom();
            StartCoroutine(GenerateDungeonCoroutine());
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
            yield return StartCoroutine(SpawnEnemiesForAllFloorsAsync());
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

                // Add the floor to floorObjects so we can toggle it later
                floorObj.AddComponent<Grid>();
                floorObjects.Add(floorObj);

                // Create separate Tilemaps for floor, corridors, and walls
                Tilemap floorTilemap = CreateTilemap(floorObj.transform, "FloorTilemap");
                Tilemap corridorTilemap = CreateTilemap(floorObj.transform, "CorridorTilemap");
                Tilemap wallTilemap = CreateTilemap(floorObj.transform, "WallTilemap");

                FloorData floor = new FloorData(floorNum);
                floors.Add(floor);

                // Generate floor layout with rooms
                Rect initialRect = new Rect(-100, -100, Random.Range(dungeonSizeRange.x, dungeonSizeRange.y), Random.Range(dungeonSizeRange.x, dungeonSizeRange.y));
                methods.GenerateFloorTiles(initialRect, maxBSPIterations, floor, roomSizeRange, carvingIterationsMin, carvingIterationsMax);

                // Create and render each room
                foreach (Room room in floor.Rooms)
                {
                    methods.RenderRoom(room, floorTilemap, floorTile);
                    floor.FloorTiles.UnionWith(room.FloorTiles); // Add room floor tiles to the floor’s overall collection
                }

                // Connect rooms using corridors
                methods.ConnectRoomsUsingMST(floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance, connectionChance);

                // Render each corridor and add its tiles to the floor’s overall collection
                foreach (var (roomA, roomB, corridorTiles) in floor.Corridors)
                {
                    methods.RenderCorridor(corridorTiles, corridorTilemap, corridorTile);
                    floor.FloorTiles.UnionWith(corridorTiles); // Add corridor floor tiles to the floor’s overall collection
                }

                // Place walls around the combined floor tiles
                methods.CreateWallsForFloor(floor.FloorTiles, wallTilemap, wallTile);
                GeneratePatrolPoints(floor);
                // Increment the seed for procedural variety
                seed += 1;
                yield return null;
            }
                HideFloors();
        }

        // Helper method to create a Tilemap under a specified parent transform
        private Tilemap CreateTilemap(Transform parent, string tilemapName)
        {
            GameObject tilemapObj = new GameObject(tilemapName);
            tilemapObj.transform.parent = parent;
            tilemapObj.transform.localPosition = Vector3.zero; // No need to offset if parent has Grid

            // Add Tilemap components
            Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
            tilemapObj.AddComponent<TilemapRenderer>();

            return tilemap;
        }

        public Vector3 GetExitPoint()
        {
    return methods.GetRandomFloorTiles(floors[0], 1).FirstOrDefault();
        }
 

private IEnumerator SpawnEnemiesForAllFloorsAsync()
{
    foreach (var floor in floors)
    {
        Debug.Log($"Attempting to spawn enemies on Floor {floor.FloorNumber}.");

        List<Vector3> enemyPositions = methods.GetRandomFloorTiles(floor, numberOfEnemiesPerFloor);
        if (enemyPositions == null || enemyPositions.Count == 0)
        {
            Debug.LogError($"No valid enemy positions found for Floor {floor.FloorNumber}. Skipping enemy spawn.");
            continue;
        }

        Transform floorTransform = dungeonParent.transform.Find($"Floor_{floor.FloorNumber}");
        if (floorTransform == null)
        {
            Debug.LogError($"Floor_{floor.FloorNumber} not found under dungeonParent. Skipping enemy spawn.");
            continue;
        }

        foreach (Vector3 position in enemyPositions)
        {
            Debug.Log($"Spawning enemy on Floor {floor.FloorNumber} at position {position}.");
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, floorTransform);

            // Set the spawnFloor for the enemy
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.spawnFloor = floor.FloorNumber;
                Debug.Log($"Enemy spawned on Floor {enemyStats.spawnFloor} at position {position}.");
            }
            else
            {
                Debug.LogError("EnemyStats component not found on instantiated enemy.");
            }

            yield return new WaitForSeconds(0.05f); // Small delay to prevent freezing
        }
    }
}


        // Generate patrol points during dungeon generation
private void GeneratePatrolPoints(FloorData floor)
{
    // Generate a set number of patrol points (e.g., 30) for the floor
    List<Vector3> patrolPoints = methods.GetRandomFloorTiles(floor, 30); // Assuming this method returns a list of positions from the floor's tiles
    floorPatrolPoints[floor.FloorNumber] = patrolPoints;
}

public List<Vector3> GetPatrolPointsForFloor(int floorNumber)
{
    if (floorPatrolPoints.TryGetValue(floorNumber, out var patrolPoints))
        return patrolPoints;
    else
        return new List<Vector3>();
}


public void TransportPlayerToDungeon(GameObject player)
{
    Vector3 exitPosition = GetExitPoint();
    player.transform.position = exitPosition;

    // Retrieve and set the Rigidbody position
    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.position = exitPosition; // Synchronize Rigidbody position with transform
    }

    PlayerManager.Instance.UpdateCurrentTilePosition(); // Updated line
    Camera.main.GetComponent<CameraController>().SetPlayerTransform(player.transform);
    StartCoroutine(ShowFloor(1));
    Debug.Log("Player transported to dungeon.");
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

        private IEnumerator ShowFloor(int floorNumber)
        {
            HideFloors(); // First deactivate all floors

            if (floorNumber > 0 && floorNumber <= floorObjects.Count)
            {
                GameObject floorObj = floorObjects[floorNumber - 1];
                floorObj.SetActive(true); // Activate the specified floor
            }
                yield return null;
        }


        private void HideFloors()
        {
            foreach (GameObject floorObj in floorObjects)
            {
                floorObj.SetActive(false);
            }
        }

    }
}
