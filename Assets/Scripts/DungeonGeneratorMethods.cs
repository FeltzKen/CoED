using System.Collections.Generic;
using UnityEngine;

namespace YourGameNamespace
{
    public class DungeonGeneratorMethods
    {
        /// <summary>
        /// Generates corridor tiles between two points.
        /// </summary>
        public static HashSet<Vector2Int> GenerateCorridor(Vector2Int start, Vector2Int end, float straightCorridorChance)
        {
            HashSet<Vector2Int> corridorTiles = new HashSet<Vector2Int>();
            Vector2Int current = start;

            while (current != end)
            {
                corridorTiles.Add(current);

                if (current.x != end.x && Random.value < straightCorridorChance)
                {
                    current.x += (current.x < end.x) ? 1 : -1;
                }
                else if (current.y != end.y)
                {
                    current.y += (current.y < end.y) ? 1 : -1;
                }
            }

            corridorTiles.Add(end);
            return corridorTiles;
        }

        /// <summary>
        /// Connects rooms using a Minimum Spanning Tree.
        /// </summary>
        public static List<(Room, Room)> ConnectRoomsUsingMST(List<Room> rooms)
        {
            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    float weight = Vector2.Distance(rooms[i].Center, rooms[j].Center);
                    edges.Add(new Edge(rooms[i], rooms[j], weight));
                }
            }

            edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            DisjointSet disjointSet = new DisjointSet(rooms);

            List<(Room, Room)> connections = new List<(Room, Room)>();

            foreach (var edge in edges)
            {
                if (disjointSet.Find(edge.RoomA) != disjointSet.Find(edge.RoomB))
                {
                    disjointSet.Union(edge.RoomA, edge.RoomB);
                    connections.Add((edge.RoomA, edge.RoomB));
                }
            }

            return connections;
        }
    }

    // Helper classes for MST
    public class Edge
    {
        public Room RoomA;
        public Room RoomB;
        public float Weight;

        public Edge(Room roomA, Room roomB, float weight)
        {
            RoomA = roomA;
            RoomB = roomB;
            Weight = weight;
        }
    }

    public class DisjointSet
    {
        private Dictionary<Room, Room> parent = new Dictionary<Room, Room>();

        public DisjointSet(List<Room> rooms)
        {
            foreach (var room in rooms)
                parent[room] = room;
        }

        public Room Find(Room room)
        {
            if (parent[room] == room)
                return room;

            parent[room] = Find(parent[room]);
            return parent[room];
        }

        public void Union(Room roomA, Room roomB)
        {
            Room rootA = Find(roomA);
            Room rootB = Find(roomB);
            if (rootA != rootB)
                parent[rootB] = rootA;
        }
    }
}
