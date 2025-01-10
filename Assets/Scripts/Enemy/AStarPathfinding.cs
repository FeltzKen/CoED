using System.Collections.Generic;
using UnityEngine;

namespace CoED.Pathfinding
{
    public class AStarPathfinding : MonoBehaviour
    {
        public static AStarPathfinding Instance { get; private set; }

        private HashSet<Vector2Int> walkableTiles;
        private HashSet<Vector2Int> obstacleTiles;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep across scenes

            walkableTiles = new HashSet<Vector2Int>();
            obstacleTiles = new HashSet<Vector2Int>();
        }

        public void UpdateFloorTiles(int floorNumber)
        {
            walkableTiles.Clear();
            obstacleTiles.Clear();

            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError("FloorData not found.");
                return;
            }

            // Walkable tiles are the floor tiles
            walkableTiles.UnionWith(floorData.FloorTiles);

            // Obstacles include walls and void tiles
            obstacleTiles.UnionWith(floorData.WallTiles);
            obstacleTiles.UnionWith(floorData.VoidTiles);

            Debug.Log(
                $"Initialized floor {floorNumber}. Walkable: {walkableTiles.Count}, Obstacles: {obstacleTiles.Count}"
            );
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
        {
            // Redirect to the closest walkable tile if the target is invalid
            if (!walkableTiles.Contains(target))
            {
                Debug.Log(
                    $"Target tile {target} is not walkable. Redirecting to the nearest walkable tile..."
                );
                target = FindClosestWalkableTile(target);

                // If no valid tile is found, abort pathfinding
                if (target == Vector2Int.zero)
                {
                    Debug.LogWarning("No valid walkable tile found near the target.");
                    return null;
                }

                Debug.Log($"Redirected to closest walkable tile: {target}");
            }

            // Standard A* pathfinding logic starts here
            var openList = new List<Node>();
            var closedList = new HashSet<Node>();
            var startNode = new Node(start, true);
            var targetNode = new Node(target, true);

            openList.Add(startNode);

            int maxIterations = 5000; // Fail-safe to prevent infinite loops
            int iterationCount = 0;

            while (openList.Count > 0)
            {
                iterationCount++;
                if (iterationCount > maxIterations)
                {
                    Debug.LogError("Pathfinding exceeded maximum iterations. Aborting.");
                    return null;
                }

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

                // If target is reached
                if (currentNode.GridPosition == targetNode.GridPosition)
                {
                    return RetracePath(startNode, currentNode);
                }

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    float newGCost =
                        currentNode.GCost
                        + (
                            neighbor.GridPosition.x != currentNode.GridPosition.x
                            && neighbor.GridPosition.y != currentNode.GridPosition.y
                                ? 1.414f
                                : 1f
                        ); // √2 for diagonals
                    if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost =
                            Mathf.Abs(neighbor.GridPosition.x - targetNode.GridPosition.x)
                            + Mathf.Abs(neighbor.GridPosition.y - targetNode.GridPosition.y); // Manhattan heuristic
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            Debug.LogWarning("No path found.");
            return null;
        }

        private Vector2Int FindClosestWalkableTile(Vector2Int target)
        {
            Vector2Int closestTile = Vector2Int.zero;
            int closestDistance = int.MaxValue;

            foreach (Vector2Int tile in walkableTiles)
            {
                int distance = Mathf.Abs(tile.x - target.x) + Mathf.Abs(tile.y - target.y); // Manhattan distance
                if (distance < closestDistance)
                {
                    closestTile = tile;
                    closestDistance = distance;
                }
            }

            return closestTile;
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
                new Vector2Int(1, 1), // Diagonal: Top-Right
                new Vector2Int(-1, 1), // Diagonal: Top-Left
                new Vector2Int(1, -1), // Diagonal: Bottom-Right
                new Vector2Int(
                    -1,
                    -1
                ) // Diagonal: Bottom-Left
                ,
            };

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPos = node.GridPosition + direction;
                if (walkableTiles.Contains(neighborPos) && !obstacleTiles.Contains(neighborPos))
                {
                    neighbors.Add(new Node(neighborPos, true));
                }
            }

            return neighbors;
        }
    }
}
