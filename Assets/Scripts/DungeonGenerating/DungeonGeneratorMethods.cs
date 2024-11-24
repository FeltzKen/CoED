using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public class DungeonGeneratorMethods
    {
        private DungeonGenerator dungeonGenerator;
        private DungeonSettings dungeonSettings;

        public DungeonGeneratorMethods(DungeonGenerator generator)
        {
            dungeonGenerator = generator;
        }

        /// <summary>
        /// Generates floor tiles using the selected algorithm.
        /// </summary>
        public HashSet<Vector2Int> GenerateFloorTiles(RectInt bounds)
        {
            return CarvingAlgorithm.Execute(
                dungeonSettings.selectedAlgorithm.algorithmType,
                dungeonSettings,
                bounds
            );
        }

        /// <summary>
        /// Generates wall tiles surrounding the floor tiles.
        /// </summary>
private HashSet<Vector2Int> GenerateWallTiles(HashSet<Vector2Int> floorTiles)
{
    HashSet<Vector2Int> wallTiles = new HashSet<Vector2Int>();

    foreach (var tile in floorTiles)
    {
        foreach (var direction in Direction2D.GetAllDirections())
        {
            Vector2Int neighbor = tile + direction;
            if (!floorTiles.Contains(neighbor) && !wallTiles.Contains(neighbor))
            {
                wallTiles.Add(neighbor);
            }
        }
    }

    // Add a second layer of walls around the first wall layer
    HashSet<Vector2Int> additionalWallTiles = new HashSet<Vector2Int>();
    foreach (var wallTile in wallTiles)
    {
        foreach (var direction in Direction2D.GetAllDirections())
        {
            Vector2Int neighbor = wallTile + direction;
            if (!floorTiles.Contains(neighbor) && !wallTiles.Contains(neighbor) && !additionalWallTiles.Contains(neighbor))
            {
                additionalWallTiles.Add(neighbor);
            }
        }
    }

    // Combine the original wall tiles with the additional wall tiles
    wallTiles.UnionWith(additionalWallTiles);

    return wallTiles;
}


        /// <summary>
        /// Renders tiles onto a given Tilemap.
        /// </summary>
public void RenderTiles(HashSet<Vector2Int> tiles, Tilemap tilemap, TileBase tile)
{
    if (tiles.Count == 0)
    {
        Debug.LogWarning($"No tiles to render on {tilemap.name}.");
    }
    else
    {
        Debug.Log($"Rendering {tiles.Count} tiles on {tilemap.name}.");
    }

    foreach (Vector2Int position in tiles)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        tilemap.SetTile(tilePosition, tile);
    }
}


        /// <summary>
        /// Gets a random floor tile from the provided set.
        /// </summary>
        public Vector2 GetRandomFloorTile(HashSet<Vector2Int> floorTiles)
        {
            List<Vector2Int> tileList = new List<Vector2Int>(floorTiles);
            Vector2Int randomTile = tileList[Random.Range(0, tileList.Count)];
            return randomTile;
        }

        /// <summary>
        /// Validates if a position is part of the floor.
        /// </summary>
        public bool IsFloorTile(HashSet<Vector2Int> floorTiles, Vector2Int position)
        {
            return floorTiles.Contains(position);
        }

        /// <summary>
        /// Instantiates the spawning room and stores it in the DungeonManager.
        /// </summary>
        public GameObject CreateSpawningRoom(GameObject spawningRoomPrefab, Transform parent)
        {
            if (spawningRoomPrefab == null)
            {
                Debug.LogError("Spawning room prefab is not assigned!");
                return null;
            }

            Vector3 spawnPosition = Vector3.zero; // Default to origin
            GameObject spawningRoom = Object.Instantiate(spawningRoomPrefab, spawnPosition, Quaternion.identity, parent);
            DungeonManager.Instance.SetSpawningRoomInstance(spawningRoom);

            Debug.Log("Spawning room created.");
            return spawningRoom;
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
        
    }
}
