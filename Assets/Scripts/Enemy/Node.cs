using UnityEngine;

namespace CoED.Pathfinding
{
    public class Node
    {
        public Vector2Int GridPosition;
        public bool IsWalkable;
        public Vector2Int Position { get; set; }
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public Node Parent;

        public Node(Vector2Int gridPosition, bool isWalkable)
        {
            GridPosition = gridPosition;
            IsWalkable = isWalkable;
            Position = gridPosition;
            GCost = int.MaxValue;
            HCost = 0;
            Parent = null;
        }
    }
}
