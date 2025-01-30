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
        public HashSet<Vector2Int> VoidTiles { get; private set; } = new HashSet<Vector2Int>();
        public Tilemap FloorTilemap { get; private set; }
        public Tilemap WallTilemap { get; private set; }
        public Tilemap VoidTilemap { get; private set; }
        public List<GameObject> StairsUp { get; private set; } = new List<GameObject>();
        public List<GameObject> StairsDown { get; private set; } = new List<GameObject>();
        public Transform itemParent;
        public bool hasSpawnedMiniBoss = false;
        public bool hasSpawnedBoss = false;

        public FloorData(int floorNumber)
        {
            FloorNumber = floorNumber;
        }

        public void SetTilemaps(
            Tilemap floorTilemap,
            Tilemap wallTilemap,
            Tilemap voidTilemap = null
        )
        {
            FloorTilemap = floorTilemap;
            WallTilemap = wallTilemap;
            VoidTilemap = voidTilemap;
        }

        public void AddAllFloorTiles(
            IEnumerable<Vector2Int> floorTiles,
            IEnumerable<Vector2Int> wallTiles,
            IEnumerable<Vector2Int> voidTiles = null
        )
        {
            foreach (var tile in floorTiles)
            {
                FloorTiles.Add(tile);
            }
            foreach (var tile in wallTiles)
            {
                WallTiles.Add(tile);
            }

            if (voidTiles == null)
                return;

            foreach (var tile in voidTiles)
            {
                VoidTiles.Add(tile);
            }
        }

        public List<Vector2Int> GetRandomFloorTiles(int count)
        {
            if (FloorTiles.Count == 0 || count <= 0)
            {
                throw new System.InvalidOperationException(
                    "No floor tiles available or invalid count."
                );
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
