using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        [SerializeField] public DungeonSettings dungeonSettings;

        private int currentFloorNumber;
        private GameObject dungeonParent;
        private FloorData floorData;

        private void Awake()
        {
            if (dungeonSettings == null)
            {
                Debug.LogError("DungeonSettings is not assigned!");
                return;
            }

            // Create dungeon parent
            CreateDungeonParent();

            // Generate the dungeon floors first before handling the spawning room
            GenerateDungeon();

            // Create the spawning room using the prefab from DungeonSettings
            if (dungeonSettings.spawningRoomPrefab != null)
            {
                GameObject spawningRoom = Instantiate(dungeonSettings.spawningRoomPrefab, dungeonParent.transform);
                spawningRoom.name = "SpawningRoom";
               // spawningRoom.transform.localPosition = Vector3.zero;

                // Store the reference to the spawning room in DungeonManager
            DungeonManager.Instance.SpawningRoomInstance = spawningRoom;

            }
            else
            {
                Debug.LogError("Spawning room prefab is not assigned in DungeonSettings!");
            }
            PlaceStairs();
        }


        private void CreateDungeonParent()
        {
                GameObject gridObject = GameObject.Find("Grid");

            // Create the dungeon parent GameObject dynamically
            dungeonParent = new GameObject("DungeonParent");
                dungeonParent.transform.SetParent(gridObject.transform);

        }

        public void GenerateDungeon()
        {
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(dungeonSettings.maxFloors));

            if (DungeonManager.Instance == null)
            {
                Debug.LogError("DungeonManager.Instance is null. Make sure DungeonManager is initialized first.");
                return;
            }

            for (int i = 1; i <= dungeonSettings.maxFloors; i++)
            {
                try
                {
                    currentFloorNumber = i;

                    // Create the floor parent object
                    GameObject floorParent = new GameObject($"Floor_{currentFloorNumber}");
                    floorParent.transform.SetParent(dungeonParent.transform);

                    // Calculate grid position for the floor
                    int row = (i - 1) / gridSize;
                    int col = (i - 1) % gridSize;
                    Vector3 floorPosition = new Vector3(
                        col * dungeonSettings.dungeonSizeRange.x,
                        row * dungeonSettings.dungeonSizeRange.y,
                        0
                    );
                    floorParent.transform.position = floorPosition;

                    // Create the enemy parent object
                    GameObject enemyParentObject = new GameObject("EnemyParent");
                    enemyParentObject.transform.SetParent(floorParent.transform);

                    // Create tilemaps for this floor
                    Tilemap floorTilemap = CreateTilemap(floorParent.transform, "FloorTilemap");
                    floorTilemap.GetComponent<TilemapRenderer>().sortingOrder = 1; // Sorting order for floors (below walls)
                    floorTilemap.gameObject.layer = LayerMask.NameToLayer("Default"); // Set to Default layer

                    Tilemap wallTilemap = CreateTilemap(floorParent.transform, "WallTilemap", true);
                    wallTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2; // Sorting order for walls (above floor)
                    wallTilemap.gameObject.layer = LayerMask.NameToLayer("Obstacles"); // Set to Obstacles layer
                    
                    Tilemap voidTilemap = CreateTilemap(floorParent.transform, "VoidTilemap", true);
                    voidTilemap.GetComponent<TilemapRenderer>().sortingOrder = 0; // Sorting order for void space (below floor)
                    voidTilemap.gameObject.layer = LayerMask.NameToLayer("Obstacles"); // Set to Obstacles layer 

                    // Initialize FloorData
                    floorData = new FloorData(currentFloorNumber);
                    floorData.SetTilemaps(floorTilemap, wallTilemap, voidTilemap);

                    // Generate floor tiles using the selected algorithm
                    HashSet<Vector2Int> floorTiles = GenerateFloorTiles();
                    RenderTiles(floorTiles, floorTilemap, dungeonSettings.tilePalette.floorTiles);

                    // Generate and render wall tiles
                    HashSet<Vector2Int> wallTiles = GenerateWallTiles(floorTiles);
                    RenderTiles(wallTiles, wallTilemap, dungeonSettings.tilePalette.wallTiles);

                    HashSet<Vector2Int> voidTiles = GenerateVoidTiles(floorTiles, wallTiles);
                    RenderTiles(voidTiles, voidTilemap, dungeonSettings.tilePalette.voidTiles);



                    // Store floor data
                    StoreFloorData(floorTiles, wallTiles, voidTiles);

                    DungeonManager.Instance.FloorTransforms[currentFloorNumber] = floorParent.transform;
                    // Debug.Log($"Stored floor reference for Floor {currentFloorNumber} in DungeonManager.");

                    // Place stairs

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error generating floor {i}: {ex.Message}\n{ex.StackTrace}");
                }
            }

            // Debug.Log("Dungeon generation finished.");
            if (DungeonManager.Instance != null)
            {
                DungeonSpawner.Instance.SpawnEnemiesForAllFloors();
            }
            else
            {
                Debug.LogError("DungeonManager instance is not available.");
            }
            ApplyOffsetToAllTilemaps(); // Offset all tilemaps to match their parent floor positions
            Debug.Log($"ambushtileprefab: {dungeonSettings.ambushTilePrefab}");
            ScatterAmbushTriggers();
        }


        private Tilemap CreateTilemap(Transform parent, string name, bool addCollider = false)
        {
            GameObject tilemapObject = new GameObject(name);
            tilemapObject.transform.SetParent(parent);

            Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
                    _ = tilemapObject.AddComponent<TilemapRenderer>();

                    if (addCollider)
            {
                _ = tilemapObject.AddComponent<TilemapCollider2D>();

            }

            // Debug.Log($"Tilemap '{name}' created under parent '{parent.name}'.");

            return tilemap;
        }


        public void EnableTilemapsForFloor(int floorNumber)
        {
            if (DungeonManager.Instance.FloorTransforms.ContainsKey(floorNumber))
            {
                Transform floorParent = DungeonManager.Instance.FloorTransforms[floorNumber];

                // Iterate over all Tilemaps in the floor parent
                foreach (Tilemap tilemap in floorParent.GetComponentsInChildren<Tilemap>())
                {
                    TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = true;
                    }
                }
            }
            else
            {
                Debug.LogError($"Floor {floorNumber} not found in DungeonManager.");
            }
        }

        private HashSet<Vector2Int> GenerateFloorTiles()
        {
            RectInt dungeonBounds = new RectInt(0, 0, dungeonSettings.dungeonSizeRange.x, dungeonSettings.dungeonSizeRange.y);
            HashSet<Vector2Int> floorTiles = CarvingAlgorithm.Execute(dungeonSettings.selectedAlgorithm.algorithmType, dungeonSettings, dungeonBounds);


            return floorTiles;
        }


        private HashSet<Vector2Int> GenerateWallTiles(HashSet<Vector2Int> floorTiles)
        {
            HashSet<Vector2Int> wallTiles = new HashSet<Vector2Int>();

            foreach (var tile in floorTiles)
            {
                foreach (var direction in Direction2D.GetAllDirections())
                {
                    Vector2Int neighbor = tile + direction;
                    if (!floorTiles.Contains(neighbor))
                    {
                        wallTiles.Add(neighbor);
                    }
                }
            }

            return wallTiles;
        }
        private HashSet<Vector2Int> GenerateVoidTiles(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
        {
            HashSet<Vector2Int> voidTiles = new HashSet<Vector2Int>();

            RectInt dungeonBounds = new RectInt(0, 0, dungeonSettings.dungeonSizeRange.x, dungeonSettings.dungeonSizeRange.y);

            for (int x = dungeonBounds.xMin; x < dungeonBounds.xMax; x++)
            {
                for (int y = dungeonBounds.yMin; y < dungeonBounds.yMax; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (!floorTiles.Contains(position) && !wallTiles.Contains(position))
                    {
                        voidTiles.Add(position);
                    }
                }
            }

            return voidTiles;
        }
        public void RenderTiles(HashSet<Vector2Int> tiles, Tilemap tilemap, TileBase[] tilePalette)
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning($"No tiles to render on {tilemap.name}.");
                return;
            }

            foreach (Vector2Int position in tiles)
            {
                Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
                
                // Select a random tile from the palette
                TileBase selectedTile = tilePalette[UnityEngine.Random.Range(0, tilePalette.Length)];
                tilemap.SetTile(tilePosition, selectedTile);
            }
        }

        private void StoreFloorData(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles, HashSet<Vector2Int> voidTiles)
        {
            if (floorData == null)
            {
                Debug.LogError($"Error in StoreFloorData: `floorData` is null for Floor {currentFloorNumber}");
                return;
            }

            if (DungeonManager.Instance == null)
            {
                Debug.LogError("Error in StoreFloorData: `DungeonManager.Instance` is null.");
                return;
            }

            floorData.AddAllFloorTiles(floorTiles, wallTiles, voidTiles);
            DungeonManager.Instance.AddFloor(floorData);

        }

        private void PlaceStairs()
        {
            var floors = DungeonManager.Instance.floors;

            for (int currentFloor = dungeonSettings.maxFloors; currentFloor > 1; currentFloor--)
            {
                FloorData currentFloorData = floors[currentFloor];
                HashSet<Vector2Int> currentFloorTiles = currentFloorData.FloorTiles;

                int stairsPlacedCount = 0;
                while (stairsPlacedCount < dungeonSettings.numberOfStairs)
                {
                    Vector2Int randomTile = GetRandomTile(currentFloorTiles);
                    FloorData aboveFloorData = floors[currentFloor - 1];

                    // Check if the above floor has a floor tile at the same position
                    if (aboveFloorData.FloorTiles.Contains(randomTile))
                    {
                        // Place stairs up on the current floor
                        PlaceGameObjectAtTile(randomTile, currentFloorData.FloorTilemap, dungeonSettings.tilePalette.stairsUpTile, currentFloorData.FloorTilemap.transform);
                        currentFloorData.StairTiles.Add(randomTile);

                        // Place stairs down on the above floor
                        PlaceGameObjectAtTile(randomTile, aboveFloorData.FloorTilemap, dungeonSettings.tilePalette.stairsDownTile, aboveFloorData.FloorTilemap.transform);
                        aboveFloorData.StairTiles.Add(randomTile);

                        stairsPlacedCount++;
                    }
                }
            }
        }


        public void ApplyOffsetToAllTilemaps()
        {
            Vector3 offset = new Vector3(-0.5f, -0.5f, 0); // Offset by half a tile to center the tilemap
            foreach (Transform floorTransform in dungeonParent.transform)
            {
                // Iterate through all child tilemaps under each floor
                foreach (Tilemap tilemap in floorTransform.GetComponentsInChildren<Tilemap>(true))
                {
                    if (tilemap != null)
                    {
                        tilemap.transform.localPosition += floorTransform.position + offset;
                    }
                }
            }
        }

        public static class Direction2D
        {
            public static List<Vector2Int> GetAllDirections()
            {
                return new List<Vector2Int>
                {
                    new Vector2Int(0, 1),   // Up
                    new Vector2Int(0, -1),  // Down
                    new Vector2Int(1, 0),   // Right
                    new Vector2Int(-1, 0)   // Left
                };
            }
        }

        private GameObject PlaceGameObjectAtTile(Vector2Int tilePosition, Tilemap tilemap, GameObject prefab, Transform parent)
        {
            Vector3Int cellPosition = new Vector3Int(tilePosition.x, tilePosition.y, 0);
            Vector3 worldPosition = tilemap.CellToWorld(cellPosition) + tilemap.tileAnchor;

            GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity, parent);
            return instance;
        }

        private Vector2Int GetRandomTile(HashSet<Vector2Int> tiles)
        {
            int index = UnityEngine.Random.Range(0, tiles.Count);
            foreach (var tile in tiles)
            {
                if (index == 0) return tile;
                index--;
            }
            return Vector2Int.zero;
        }

        // scatter ambush Triggers throughout the dungeon there is only one amush tile prefab saved in dungeon settings. there is also a number of ambush tiles to spawn per floor saved in dungeon settings.
        // use  the floorData.floorTilemap and generate a random tile positions place the ambush triggers on.  
        public void ScatterAmbushTriggers()
        {
            if (dungeonSettings.ambushTilePrefab == null)
            {
                Debug.LogError("Ambush tile prefab is not assigned in DungeonSettings!");
                return;
            }

            if (dungeonSettings.numberOfAmbushTiles == 0)
            {
                Debug.LogWarning("numberOfAmbushTriggersPerFloor is set to 0. No ambush triggers will be spawned.");
                return;
            }

            foreach (FloorData floorData in DungeonManager.Instance.floors.Values)
            {
                Tilemap floorTilemap = floorData.FloorTilemap;
                int floorNumber = floorData.FloorNumber;

                for (int i = 0; i < dungeonSettings.numberOfAmbushTiles; i++)
                {
                    Vector2Int ambushTilePosition = GetRandomTile(floorData.FloorTiles);

                    // Place the ambush tile prefab and get the instantiated GameObject
                    GameObject ambushInstance = PlaceGameObjectAtTile(ambushTilePosition, floorTilemap, dungeonSettings.ambushTilePrefab, floorTilemap.transform);

                    if (ambushInstance != null)
                    {
                        // Assign the floorNumber to the ambush trigger component
                        AmbushTriggerTile ambushTrigger = ambushInstance.GetComponent<AmbushTriggerTile>();
                        if (ambushTrigger != null)
                        {
                            ambushTrigger.floorNumber = floorNumber;
                        }
                        else
                        {
                            Debug.LogError("AmbushTrigger component not found on the ambush prefab.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to instantiate ambush prefab.");
                    }
                }
            }
        }
    }
}
