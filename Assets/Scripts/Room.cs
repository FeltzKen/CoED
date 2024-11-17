using System.Collections.Generic;
using UnityEngine;

namespace YourGameNamespace
{
    public class Room
    {
        public Rect Rect { get; private set; }
        public HashSet<Vector2Int> FloorTiles { get; private set; }
        public Vector2 Center => Rect.center;

        public Room(Rect rect)
        {
            Rect = rect;
            FloorTiles = new HashSet<Vector2Int>();
        }

        /// <summary>
        /// Checks if a given position is within the room's boundaries.
        /// </summary>
        public bool ContainsPosition(Vector2Int position)
        {
            return Rect.Contains(new Vector2(position.x, position.y));
        }

        /// <summary>
        /// Adds a floor tile to the room.
        /// </summary>
        public void AddFloorTile(Vector2Int tile)
        {
            FloorTiles.Add(tile);
        }

        /// <summary>
        /// Calculates the distance between this room and another room.
        /// </summary>
        public float DistanceTo(Room otherRoom)
        {
            return Vector2.Distance(this.Center, otherRoom.Center);
        }

        /// <summary>
        /// Expands the room's Rect boundaries by the given margin.
        /// </summary>
        public void Expand(float margin)
        {
            Rect = new Rect(
                Rect.xMin - margin,
                Rect.yMin - margin,
                Rect.width + 2 * margin,
                Rect.height + 2 * margin
            );
        }

        /// <summary>
        /// Merges another room's floor tiles into this room.
        /// </summary>
        public void MergeFloorTiles(Room otherRoom)
        {
            FloorTiles.UnionWith(otherRoom.FloorTiles);
        }
    }
}
