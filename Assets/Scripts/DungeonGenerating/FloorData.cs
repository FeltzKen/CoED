using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    public class FloorData
    {
        public int FloorNumber { get; private set; }
        public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> WallTiles { get; private set; } = new HashSet<Vector2Int>();
        public Tilemap FloorTilemap { get; private set; }
        public Tilemap WallTilemap { get; private set; }

        // Constructor
        public FloorData(int floorNumber)
        {
            FloorNumber = floorNumber;
        }

        /// <summary>
        /// Sets the tilemaps for the floor.
        /// </summary>
        public void SetTilemaps(Tilemap floorTilemap, Tilemap wallTilemap)
        {
            FloorTilemap = floorTilemap;
            WallTilemap = wallTilemap;
        }

        /// <summary>
        /// Adds a collection of floor tiles.
        /// </summary>
        public void AddFloorTiles(IEnumerable<Vector2Int> tiles)
        {
            foreach (var tile in tiles)
            {
                FloorTiles.Add(tile);
            }
        }

        /// <summary>
        /// Adds a collection of wall tiles.
        /// </summary>
        public void AddWallTiles(IEnumerable<Vector2Int> tiles)
        {
            foreach (var tile in tiles)
            {
                WallTiles.Add(tile);
            }
        }

        /// <summary>
        /// Checks if a tile is part of the floor.
        /// </summary>
        public bool IsTilePartOfFloor(Vector2Int tile)
        {
            return FloorTiles.Contains(tile);
        }

        /// <summary>
        /// Gets a single random floor tile.
        /// </summary>
        public Vector2Int GetRandomFloorTile()
        {
            if (FloorTiles.Count == 0)
            {
                throw new System.InvalidOperationException("No floor tiles available.");
            }

            List<Vector2Int> tileList = new List<Vector2Int>(FloorTiles);
            return tileList[Random.Range(0, tileList.Count)];
        }

        /// <summary>
        /// Gets multiple random floor tiles.
        /// </summary>
        public List<Vector2Int> GetRandomFloorTiles(int count)
        {
            if (FloorTiles.Count == 0 || count <= 0)
            {
                throw new System.InvalidOperationException("No floor tiles available or invalid count.");
            }

            List<Vector2Int> tileList = new List<Vector2Int>(FloorTiles);
            List<Vector2Int> selectedTiles = new List<Vector2Int>();
            for (int i = 0; i < count; i++)
            {
                selectedTiles.Add(tileList[Random.Range(0, tileList.Count)]);
            }

            return selectedTiles;
        }
    }
}
