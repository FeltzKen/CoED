using UnityEngine;

namespace CoED.Pathfinding
{
    public class Node
    {
        public Vector2Int GridPosition { get; private set; } // Position on the grid
        public bool IsWalkable { get; private set; } // Whether the node is walkable
        public float GCost { get; set; } // Movement cost from the start node
        public float HCost { get; set; } // Heuristic cost to the target
        public float FCost => GCost + HCost; // Total cost (G + H)
        public Node Parent { get; set; } // Parent node for retracing the path

        public Node(Vector2Int gridPosition, bool isWalkable)
        {
            GridPosition = gridPosition;
            IsWalkable = isWalkable;
            GCost = int.MaxValue; // Default to "unvisited"
        }
    }
}
