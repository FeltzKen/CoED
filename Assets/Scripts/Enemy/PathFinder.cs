using System.Collections.Generic;
using UnityEngine;

namespace CoED.Pathfinding
{
    public class Pathfinder
    {
        private PathfindingGrid grid;

        public Pathfinder(PathfindingGrid pathfindingGrid)
        {
            this.grid = pathfindingGrid;
        }

        public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int targetPosition)
        {
            Node startNode = grid.GetNode(startPosition);
            Node targetNode = grid.GetNode(targetPosition);

            if (startNode == null || targetNode == null)
            {
                return null;
            }

            List<Node> openSet = new List<Node> { startNode };
            HashSet<Node> closedSet = new HashSet<Node>();

            startNode.GCost = 0;
            startNode.HCost = CalculateDistance(startNode, targetNode);

            while (openSet.Count > 0)
            {
                Node currentNode = GetNodeWithLowestCost(openSet);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Node neighbor in grid.GetNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor) || !neighbor.IsWalkable)
                        continue;

                    float tentativeGCost =
                        currentNode.GCost + CalculateDistance(currentNode, neighbor);
                    if (tentativeGCost < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = CalculateDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private Node GetNodeWithLowestCost(List<Node> nodeSet)
        {
            Node lowestCostNode = nodeSet[0];
            foreach (Node node in nodeSet)
            {
                if (
                    node.FCost < lowestCostNode.FCost
                    || (node.FCost == lowestCostNode.FCost && node.HCost < lowestCostNode.HCost)
                )
                {
                    lowestCostNode = node;
                }
            }
            return lowestCostNode;
        }

        private int CalculateDistance(Node a, Node b)
        {
            int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
            int dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
            int straight = 10;
            int diagonal = 14;
            if (dx > dy)
                return diagonal * dy + straight * (dx - dy);
            return diagonal * dx + straight * (dy - dx);
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
    }
}
