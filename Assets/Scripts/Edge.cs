using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    // Represents a connection between two rooms with a weight based on distance
    public class Edge
    {
        public Room RoomA { get; } // The first room connected by this edge
        public Room RoomB { get; } // The second room connected by this edge
        public float Weight { get; } // Weight of the edge, typically the distance between RoomA and RoomB

        // Constructor for Edge
        public Edge(Room roomA, Room roomB, float weight)
        {
            RoomA = roomA;
            RoomB = roomB;
            Weight = weight;
        }
    }
}
