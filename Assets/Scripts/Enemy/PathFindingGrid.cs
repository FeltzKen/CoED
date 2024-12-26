using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED.Pathfinding
{
    public class PathfindingGrid
    {
        private Dictionary<Vector2Int, Node> nodes;

        public PathfindingGrid(HashSet<Vector2Int> walkableTiles)
        {
            nodes = new Dictionary<Vector2Int, Node>();
            foreach (var pos in walkableTiles)
            {
                nodes[pos] = new Node(pos, true);
                if (!nodes.ContainsKey(pos))
                    Debug.LogError($"PathfindingGrid: Missing node for walkable tile at {pos}");
            }
            // Debug.Log($"PathfindingGrid: Initialized with {nodes.Count} walkable nodes.");
            // Debug.Log($"PathfindingGrid: Grid fully connected: {IsGridFullyConnected()}");
        }

        public Node GetNode(Vector2Int position)
        {
            nodes.TryGetValue(position, out Node node);
            return node;
        }

        public IEnumerable<Node> GetNeighbors(Node node)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, 1),
                new Vector2Int(-1, -1),
            };
            ;

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = node.GridPosition + dir;
                if (nodes.ContainsKey(neighborPos))
                {
                    yield return nodes[neighborPos];
                }
            }
        }

        public bool IsGridFullyConnected()
        {
            if (nodes == null || nodes.Count == 0)
                return false;

            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Node>();
            var startNode = nodes.Values.FirstOrDefault(n => n.IsWalkable);

            if (startNode == null)
                return false;

            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (!visited.Contains(currentNode.GridPosition))
                {
                    visited.Add(currentNode.GridPosition);
                    foreach (var neighbor in GetNeighbors(currentNode))
                    {
                        if (neighbor.IsWalkable && !visited.Contains(neighbor.GridPosition))
                            queue.Enqueue(neighbor);
                    }
                }
            }

            return visited.Count == nodes.Count(n => n.Value.IsWalkable);
        }
    }
}
