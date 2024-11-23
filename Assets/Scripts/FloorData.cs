using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using YourGameNamespace;
namespace YourGameNamespace
{
        public class FloorData
        {
        public int FloorNumber { get; private set; }
        public List<Room> Rooms { get; private set; } = new List<Room>();
        public List<Corridor> Corridors { get; private set; } = new List<Corridor>();
        public HashSet<Vector3Int> FloorTiles { get; private set; } = new HashSet<Vector3Int>();
        public List<(Room, Room)> Connections { get; private set; } = new List<(Room, Room)>();
        public List<Vector3> PatrolPoints { get; private set; } = new List<Vector3>();
        public Tilemap FloorTilemap { get; set; }
        public Tilemap CorridorTilemap { get; set; }
        public Tilemap WallTilemap { get; set; }
        public Transform FloorTransform { get; private set; }

        public FloorData(int floorNumber)
        {
            FloorNumber = floorNumber;
        }
 
        public FloorData(int floorNumber, Transform floorTransform)
        {
            FloorNumber = floorNumber;
            FloorTransform = floorTransform;
        }


        /// <summary>
        /// Adds a room to the floor and updates the tile data.
        /// </summary>
        public void AddRoom(Room room)
        {
            if (room == null)
            {
                Debug.LogError("Attempted to add a null room to FloorData.");
                return;
            }

            Rooms.Add(room);

            if (room.FloorTiles != null)
            {
                foreach (var tile in room.FloorTiles)
                {
                    FloorTiles.Add(new Vector3Int(tile.x, tile.y, 0));
                }
            }
            else
            {
                Debug.LogWarning("Room has no FloorTiles to merge.");
            }
            
        }

        /// <summary>
        /// Adds a corridor and its connections to the floor.
        /// </summary>
        public void AddCorridor(HashSet<Vector3Int> corridorTiles, Room roomA, Room roomB)
        {
            Corridor corridor = new Corridor(corridorTiles, roomA, roomB);
            Corridors.Add(corridor);
            Connections.Add((roomA, roomB));
            FloorTiles.UnionWith(corridorTiles);
        }

        /// <summary>
        /// Sets patrol points for this floor.
        /// </summary>
        public void SetPatrolPoints(List<Vector3> points)
        {
            PatrolPoints.Clear();
            PatrolPoints.AddRange(points);
            Debug.Log($"Set {PatrolPoints.Count} patrol points for Floor {FloorNumber}: {string.Join(", ", PatrolPoints)}");
        }

        /// <summary>
        /// Gets a random patrol point from the list.
        /// </summary>
        public Vector3 GetRandomPatrolPoint()
        {
            if (PatrolPoints.Count == 0)
                throw new System.InvalidOperationException("No patrol points available on this floor.");

            return PatrolPoints[Random.Range(0, PatrolPoints.Count)];
        }

        /// <summary>
        /// Calculates the bounds of the floor based on its tiles.
        /// </summary>
        public BoundsInt FloorBounds
        {
            get
            {
                if (FloorTiles == null || FloorTiles.Count == 0)
                {
                    return new BoundsInt();
                }

                int minX = int.MaxValue, minY = int.MaxValue;
                int maxX = int.MinValue, maxY = int.MinValue;

                foreach (var tile in FloorTiles)
                {
                    if (tile.x < minX) minX = tile.x;
                    if (tile.y < minY) minY = tile.y;
                    if (tile.x > maxX) maxX = tile.x;
                    if (tile.y > maxY) maxY = tile.y;
                }

                return new BoundsInt(minX, minY, 0, maxX - minX + 1, maxY - minY + 1, 1);
            }
        }


        // FloorData.cs - GeneratePatrolPoints
        public void GeneratePatrolPoints(int numberOfPoints)
        {
            PatrolPoints.Clear();

            if (FloorTiles == null || FloorTiles.Count == 0)
            {
                Debug.LogError($"Cannot generate patrol points: Floor {FloorNumber} has no floor tiles.");
                return;
            }

            List<Vector3Int> floorTiles = FloorTiles.ToList();
            for (int i = 0; i < numberOfPoints; i++)
            {
                Vector3Int randomTile = floorTiles[Random.Range(0, floorTiles.Count)];
                Vector3 patrolPoint = FloorTilemap.CellToWorld(randomTile) + new Vector3(0, 0, 0); // Center of the tile
                PatrolPoints.Add(patrolPoint);
            }

            Debug.Log($"Generated {PatrolPoints.Count} patrol points for Floor {FloorNumber}: {string.Join(", ", PatrolPoints)}");
        }

        public HashSet<Vector3Int> GetWalkableTiles()
        {
            var walkableTiles = new HashSet<Vector3Int>(FloorTiles);
            if (CorridorTilemap != null)
            {
                foreach (var position in CorridorTilemap.cellBounds.allPositionsWithin)
                {
                    if (CorridorTilemap.HasTile(position))
                    {
                        walkableTiles.Add(new Vector3Int(position.x, position.y, 0));
                    }
                }
            }
            return walkableTiles;
        }



        /// <summary>
        /// Checks if a given tile is part of the floor.
        /// </summary>
        public bool IsTilePartOfFloor(Vector3Int tile)
        {
            return FloorTiles.Contains(tile);
        }

        /// <summary>
        /// Gets a specified number of random tiles from the current floor.
        /// </summary>
        public List<Vector3Int> GetRandomFloorTiles(int count)
        {
            List<Vector3Int> floorTilesList = new List<Vector3Int>(FloorTiles);
            List<Vector3Int> randomTiles = new List<Vector3Int>();

            if (floorTilesList.Count == 0)
                throw new System.InvalidOperationException("No floor tiles available on this floor.");

            for (int i = 0; i < count; i++)
            {
                randomTiles.Add(floorTilesList[Random.Range(0, floorTilesList.Count)]);
            }

            return randomTiles;
        }
        public void InitializeTilemaps(Tilemap floorTilemap, Tilemap corridorTilemap, Tilemap wallTilemap)
        {
            if (floorTilemap == null || corridorTilemap == null || wallTilemap == null)
            {
                Debug.LogError($"FloorData {FloorNumber}: One or more tilemaps are null. Initialization failed.");
                return;
            }

            FloorTilemap = floorTilemap;
            CorridorTilemap = corridorTilemap;
            WallTilemap = wallTilemap;

            Debug.Log($"FloorData {FloorNumber}: Tilemaps initialized successfully.");
        }
        /// <summary>
        /// Gets a single random tile from the current floor.
        /// </summary>
        public Vector3Int GetRandomFloorTile()
        {
            return GetRandomFloorTiles(1)[0];
        }

        /// <summary>
        /// Adds a floor tile to the floor data.
        /// </summary>
        public void AddFloorTile(Vector3Int tile)
        {
            FloorTiles.Add(tile);
        }
    }
}
