using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    private class Node
    {
        public Vector2Int Position { get; set; }
        public Node Parent { get; set; }
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost => GCost + HCost;

        public Node(Vector2Int position)
        {
            Position = position;
            GCost = float.MaxValue; // Initially set to a very high value
        }
    }

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    public static List<Vector2Int> FindPath(
        Vector2Int start,
        Vector2Int target,
        HashSet<Vector2Int> walkablePositions
    )
    {
        // Create open and closed lists
        Dictionary<Vector2Int, Node> openList = new Dictionary<Vector2Int, Node>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        // Initialize start node
        Node startNode = new Node(start);
        startNode.GCost = 0;
        startNode.HCost = Heuristic(start, target);
        openList[start] = startNode;

        while (openList.Count > 0)
        {
            // Get the node with the lowest F-cost
            Node currentNode = GetLowestFNode(openList);
            if (currentNode.Position == target)
            {
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode.Position);
            closedList.Add(currentNode.Position);

            // Iterate through neighbors
            foreach (Vector2Int direction in Directions)
            {
                Vector2Int neighborPos = currentNode.Position + direction;

                // Skip if neighbor is not walkable or is already in the closed list
                if (!walkablePositions.Contains(neighborPos) || closedList.Contains(neighborPos))
                {
                    continue;
                }

                // Calculate the G-cost for the neighbor
                float tentativeGCost = currentNode.GCost + 1; // Assuming uniform cost of 1 per tile

                if (!openList.ContainsKey(neighborPos))
                {
                    // Create a new neighbor node
                    Node neighborNode = new Node(neighborPos)
                    {
                        GCost = tentativeGCost,
                        HCost = Heuristic(neighborPos, target),
                        Parent = currentNode,
                    };
                    openList[neighborPos] = neighborNode;
                }
                else if (tentativeGCost < openList[neighborPos].GCost)
                {
                    // Update G-cost and parent for the existing neighbor node
                    Node neighborNode = openList[neighborPos];
                    neighborNode.GCost = tentativeGCost;
                    neighborNode.Parent = currentNode;
                }
            }
        }

        // If we reach here, there's no path
        return null;
    }

    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Vector2Int.Distance(a, b); // Euclidean distance (can use Manhattan distance for grid paths)
    }

    private static Node GetLowestFNode(Dictionary<Vector2Int, Node> openList)
    {
        Node lowestFNode = null;
        foreach (var node in openList.Values)
        {
            if (lowestFNode == null || node.FCost < lowestFNode.FCost)
            {
                lowestFNode = node;
            }
        }
        return lowestFNode;
    }

    private static List<Vector2Int> ReconstructPath(Node currentNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
}
