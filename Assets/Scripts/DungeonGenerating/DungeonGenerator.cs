using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        [SerializeField] private DungeonSettings dungeonSettings;

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
        spawningRoom.transform.localPosition = Vector3.zero;

        // Store the reference to the spawning room in DungeonManager
       DungeonManager.Instance.SpawningRoomInstance = spawningRoom;

        Debug.Log("Spawning room successfully created.");
    }
    else
    {
        Debug.LogError("Spawning room prefab is not assigned in DungeonSettings!");
    }
}


        private void CreateDungeonParent()
        {
                GameObject gridObject = GameObject.Find("Grid");

            // Create the dungeon parent GameObject dynamically
            dungeonParent = new GameObject("DungeonParent");
                dungeonParent.transform.SetParent(gridObject.transform);

            Debug.Log("Dungeon parent created.");
        }

public void GenerateDungeon()
{
    Debug.Log("Starting dungeon generation...");
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
            Debug.Log($"Generating Floor {currentFloorNumber}...");

            // Create floor parent object
            GameObject floorParent = new GameObject($"Floor_{currentFloorNumber}");
            floorParent.transform.SetParent(dungeonParent.transform);
            floorParent.layer = LayerMask.NameToLayer("FloorLayer");

            // Log the assignment
            Debug.Log($"Floor {currentFloorNumber} parent assigned to dungeonParent.");

            // Create tilemaps for this floor
            Tilemap floorTilemap = CreateTilemap(floorParent.transform, "FloorTilemap");
            Tilemap wallTilemap = CreateTilemap(floorParent.transform, "WallTilemap");

            // Initialize FloorData
            floorData = new FloorData(currentFloorNumber);
            floorData.SetTilemaps(floorTilemap, wallTilemap);

            // Generate floor tiles using the selected algorithm
            HashSet<Vector2Int> floorTiles = GenerateFloorTiles();
            Debug.Log($"Generated {floorTiles.Count} floor tiles for Floor {currentFloorNumber}.");

            // Render floor tiles
            RenderTiles(floorTiles, floorTilemap, dungeonSettings.floorTile);
            Debug.Log($"Rendered {floorTiles.Count} floor tiles on Floor {currentFloorNumber}.");

            // Generate and render wall tiles
            HashSet<Vector2Int> wallTiles = GenerateWallTiles(floorTiles);
            Debug.Log($"Generated {wallTiles.Count} wall tiles for Floor {currentFloorNumber}.");
            RenderTiles(wallTiles, wallTilemap, dungeonSettings.wallTile);
            Debug.Log($"Rendered {wallTiles.Count} wall tiles on Floor {currentFloorNumber}.");

            // Store floor data
            StoreFloorData(floorTiles);
            Debug.Log($"Stored floor data for Floor {currentFloorNumber}.");

            // Spawn stairs
            SpawnStairs(floorTiles, floorParent.transform);
            Debug.Log($"Spawned stairs on Floor {currentFloorNumber}.");

            // Disable components manually instead of setting the floor inactive
            floorParent.SetActive(false);

            Debug.Log($"Components of Floor {currentFloorNumber} disabled.");
            // Set the generated floor inactive initially
            Debug.Log($"Setting Floor {currentFloorNumber} to inactive...");

            Debug.Log($"Floor {currentFloorNumber} is now inactive.");

            // Store floor reference in DungeonManager for future activation
            DungeonManager.Instance.FloorTransforms[currentFloorNumber] = floorParent.transform;

            Debug.Log($"Dungeon generation complete for Floor {currentFloorNumber}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating floor {i}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    Debug.Log("Dungeon generation finished.");
    DungeonSpawner.Instance.SpawnEnemiesOnFloor(floorData);
}


   private Tilemap CreateTilemap(Transform parent, string name)
        {
            GameObject tilemapObject = new GameObject(name);
            tilemapObject.transform.SetParent(parent);

            Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
            tilemapObject.AddComponent<TilemapRenderer>();
                // Add TilemapCollider2D to the tilemap
            tilemapObject.AddComponent<TilemapCollider2D>();
            tilemap.tileAnchor = Vector3.zero; // Set anchor to (0, 0, 0)

    Debug.Log($"Tilemap '{name}' created under parent '{parent.name}' at position {tilemapObject.transform.position}");

            return tilemap;
        }

        private HashSet<Vector2Int> GenerateFloorTiles()
        {
            RectInt dungeonBounds = new RectInt(0, 0, dungeonSettings.dungeonSizeRange.x, dungeonSettings.dungeonSizeRange.y);
            HashSet<Vector2Int> floorTiles = CarvingAlgorithm.Execute(dungeonSettings.selectedAlgorithm.algorithmType, dungeonSettings, dungeonBounds);

            Debug.Log($"Generated {floorTiles.Count} floor tiles.");

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

        private void RenderTiles(HashSet<Vector2Int> tiles, Tilemap tilemap, TileBase tile)
        {
            foreach (var tilePos in tiles)
            {
                Vector3Int position = new Vector3Int(tilePos.x, tilePos.y, 0);
                tilemap.SetTile(position, tile);
            
            }

            tilemap.RefreshAllTiles();
        }

private void StoreFloorData(HashSet<Vector2Int> floorTiles)
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

    floorData.AddFloorTiles(floorTiles);
    DungeonManager.Instance.AddFloor(floorData);
    Debug.Log($"Stored floor data for Floor {currentFloorNumber}.");
}


        private void SpawnStairs(HashSet<Vector2Int> floorTiles, Transform parent)
        {
            Vector2Int upStairsPosition = GetRandomTile(floorTiles);
            Vector2Int downStairsPosition = GetRandomTile(floorTiles);

            if (dungeonSettings.stairsUpPrefab != null)
            {
                Instantiate(dungeonSettings.stairsUpPrefab, new Vector3(upStairsPosition.x, upStairsPosition.y, 0), Quaternion.identity, parent);
            }

            if (dungeonSettings.stairsDownPrefab != null)
            {
                Instantiate(dungeonSettings.stairsDownPrefab, new Vector3(downStairsPosition.x, downStairsPosition.y, 0), Quaternion.identity, parent);
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
    }
}
