using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED.Pathfinding
{
    public class Pathfinder
    {
        private PathfindingGrid grid;

        public Pathfinder(PathfindingGrid grid)
        {
            this.grid = grid;
        }

        public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
        {
            Node startNode = grid.GetNode(startPos);
            Node targetNode = grid.GetNode(targetPos);

            if (startNode == null || targetNode == null)
                return null;

            var openSet = new List<Node> { startNode };
            var closedSet = new HashSet<Node>();

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startNode, targetNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbor in grid.GetNeighbors(currentNode))
                {
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                        continue;

                    int tentativeGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (tentativeGCost < neighbor.GCost)
                    {
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // No path found
        }

        private List<Vector2Int> RetracePath(Node startNode, Node endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.GridPosition);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        private int GetDistance(Node a, Node b)
        {
            int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
            int dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
            return dx + dy; // Manhattan distance
        }
    }
}
