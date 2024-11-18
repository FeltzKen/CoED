using System;
using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;
namespace YourGameNamespace
{
    public class FloorData
    {
        public int FloorNumber { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<(Room, Room, HashSet<Vector2Int>)> Corridors { get; private set; }
        public HashSet<Vector2Int> FloorTiles { get; private set; }
        public List<(Room, Room)> Connections { get; private set; }

        public FloorData(int floorNumber)
        {
            FloorNumber = floorNumber;
            Rooms = new List<Room>();
            Corridors = new List<(Room, Room, HashSet<Vector2Int>)>();
            FloorTiles = new HashSet<Vector2Int>();
            Connections = new List<(Room, Room)>();
        }

        public void AddCorridor(HashSet<Vector2Int> corridorTiles, Room roomA, Room roomB)
        {
            Corridors.Add((roomA, roomB, corridorTiles));
        }

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
            FloorTiles.UnionWith(room.FloorTiles);
        }
    }
}
