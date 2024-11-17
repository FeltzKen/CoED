using System.Collections.Generic;
using UnityEngine;

namespace YourGameNamespace
{
    // Represents a connection between two rooms with a weight based on distance
    public class Corridor
    {
        public HashSet<Vector2Int> CorridorTiles { get; private set; }
        public Room RoomA { get; private set; }
        public Room RoomB { get; private set; }

        // Constructor
        public Corridor(HashSet<Vector2Int> corridorTiles, Room roomA, Room roomB)
        {
            CorridorTiles = new HashSet<Vector2Int>(corridorTiles); // Ensure no unintended reference issues
            RoomA = roomA;
            RoomB = roomB;
        }

        /// <summary>
        /// Checks if a given tile is part of the corridor.
        /// </summary>
        public bool IsTilePartOfCorridor(Vector2Int tile)
        {
            return CorridorTiles.Contains(tile);
        }

        /// <summary>
        /// Calculates the length of the corridor based on the number of tiles.
        /// </summary>
        public int GetCorridorLength()
        {
            return CorridorTiles.Count;
        }

        /// <summary>
        /// Returns the distance between the centers of the connected rooms.
        /// </summary>
        public float GetRoomDistance()
        {
            return Vector2.Distance(RoomA.Center, RoomB.Center);
        }

        /// <summary>
        /// Merges another corridor's tiles into this corridor.
        /// </summary>
        public void MergeWith(Corridor otherCorridor)
        {
            CorridorTiles.UnionWith(otherCorridor.CorridorTiles);
        }
    }
}
