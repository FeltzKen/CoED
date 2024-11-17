using System.Collections.Generic;
using UnityEngine;

namespace YourGameNamespace
{
    public class FloorData
    {
        public int FloorNumber { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Corridor> Corridors { get; private set; }
        public HashSet<Vector2Int> FloorTiles { get; private set; }
        public List<(Room, Room)> Connections { get; private set; }
        public List<Vector3> PatrolPoints { get; private set; }

        public FloorData(int floorNumber)
        {
            FloorNumber = floorNumber;
            Rooms = new List<Room>();
            Corridors = new List<Corridor>();
            FloorTiles = new HashSet<Vector2Int>();
            Connections = new List<(Room, Room)>();
            PatrolPoints = new List<Vector3>();
        }

        /// <summary>
        /// Adds a room to the floor and updates the tile data.
        /// </summary>
        public void AddRoom(Room room)
        {
            Rooms.Add(room);
            FloorTiles.UnionWith(room.FloorTiles);
        }

        /// <summary>
        /// Adds a corridor and its connections to the floor.
        /// </summary>
        public void AddCorridor(HashSet<Vector2Int> corridorTiles, Room roomA, Room roomB)
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
        /// Checks if a given tile is part of the floor.
        /// </summary>
        public bool IsTilePartOfFloor(Vector2Int tile)
        {
            return FloorTiles.Contains(tile);
        }

                /// <summary>
        /// Gets a specified number of random tiles from the current floor.
        /// </summary>
        public List<Vector2Int> GetRandomFloorTiles(int count)
        {
            List<Vector2Int> floorTilesList = new List<Vector2Int>(FloorTiles);
            List<Vector2Int> randomTiles = new List<Vector2Int>();

            if (floorTilesList.Count == 0)
                throw new System.InvalidOperationException("No floor tiles available on this floor.");

            for (int i = 0; i < count; i++)
            {
                randomTiles.Add(floorTilesList[Random.Range(0, floorTilesList.Count)]);
            }

            return randomTiles;
        }

        /// <summary>
        /// Gets a single random tile from the current floor.
        /// </summary>
        public Vector2Int GetRandomFloorTile()
        {
            return GetRandomFloorTiles(1)[0];
        }
    }
}
