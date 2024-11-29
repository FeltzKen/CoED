using System.Collections.Generic;
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
            }
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
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = node.GridPosition + dir;
                if (nodes.ContainsKey(neighborPos))
                {
                    yield return nodes[neighborPos];
                }
            }
        }
    }
}
