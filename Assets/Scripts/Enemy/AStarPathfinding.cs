using System.Collections.Generic;
using CoED.Pathfinding;
using UnityEngine;

public class AStarPathfinding
{
    private readonly HashSet<Vector2Int> walkableTiles;
    private readonly HashSet<Vector2Int> obstacleTiles;

    public AStarPathfinding(HashSet<Vector2Int> walkableTiles, HashSet<Vector2Int> obstacleTiles)
    {
        this.walkableTiles = walkableTiles;
        this.obstacleTiles = obstacleTiles;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        var openList = new List<Node>();
        var closedList = new HashSet<Node>();

        var startNode = new Node(start, true);
        var targetNode = new Node(target, true);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (
                    openList[i].FCost < currentNode.FCost
                    || (
                        openList[i].FCost == currentNode.FCost
                        && openList[i].HCost < currentNode.HCost
                    )
                )
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.GridPosition == targetNode.GridPosition)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor) || obstacleTiles.Contains(neighbor.GridPosition))
                {
                    continue;
                }

                float newGCost =
                    currentNode.GCost
                    + (int)Vector2Int.Distance(currentNode.GridPosition, neighbor.GridPosition);
                if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = (int)newGCost;
                    neighbor.HCost = (int)
                        Vector2Int.Distance(neighbor.GridPosition, targetNode.GridPosition);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // Path not found
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.GridPosition);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = node.GridPosition + direction;
            if (walkableTiles.Contains(neighborPos))
            {
                neighbors.Add(new Node(neighborPos, true));
            }
        }

        return neighbors;
    }
}
