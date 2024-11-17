using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace YourGameNamespace
{
    public class DungeonGeneratorMethods
    {        
        private DungeonGenerator dungeonGenerator;

        public DungeonGeneratorMethods(DungeonGenerator generator)
        {
            dungeonGenerator = generator;
        }

        /// <summary>
        /// Retrieves a specified number of random floor tiles from a given floor.
        /// </summary>
        /// <param name="floor">The floor data to retrieve tiles from.</param>
        /// <param name="count">Number of random tiles to retrieve.</param>
        /// <returns>List of world positions of the selected tiles.</returns>
        public List<Vector3> GetRandomFloorTiles(FloorData floor, int count)
        {
            List<Vector3> randomTiles = new List<Vector3>();
            List<Vector2Int> floorTilePositions = floor.FloorTiles.ToList();

            if (floorTilePositions.Count == 0)
                return randomTiles;

            for (int i = 0; i < count; i++)
            {
                Vector2Int tilePos = floorTilePositions[Random.Range(0, floorTilePositions.Count)];
                Vector3 worldPos = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0);
                randomTiles.Add(worldPos);
            }

            return randomTiles;
        }

        /// <summary>
        /// Instantiates a prefab at a given position with a specified parent.
        /// </summary>
        private GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Transform parent)
        {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }

        /// <summary>
        /// Generates floor tiles using space splitting (BSP) algorithm.
        /// </summary>
        public void GenerateFloorTiles(Rect initialRect, int maxIterations, FloorData floor, Vector2Int roomSizeRange, int carveMin, int carveMax)
        {
            SplitSpace(initialRect, maxIterations, floor, roomSizeRange, carveMin, carveMax);
        }

        /// <summary>
        /// Recursively splits the space to create rooms.
        /// </summary>
        private void SplitSpace(Rect rect, int iterations, FloorData floor, Vector2Int roomSizeRange, int carveMin, int carveMax)
        {
            if (iterations == 0)
            {
                Room room = CreateRoom(rect, roomSizeRange, carveMin, carveMax, floor);
                floor.AddRoom(room);
                return;
            }

            bool splitVertically = Random.value > 0.5f;
            float minSplit = roomSizeRange.x;
            float maxSplit = splitVertically ? rect.width - roomSizeRange.x : rect.height - roomSizeRange.x;

            if (splitVertically && rect.width < roomSizeRange.x * 2) return;
            if (!splitVertically && rect.height < roomSizeRange.x * 2) return;

            float splitPoint = Random.Range(minSplit, maxSplit);
            Rect first = splitVertically ? new Rect(rect.x, rect.y, splitPoint, rect.height) : new Rect(rect.x, rect.y, rect.width, splitPoint);
            Rect second = splitVertically ? new Rect(rect.x + splitPoint, rect.y, rect.width - splitPoint, rect.height) : new Rect(rect.x, rect.y + splitPoint, rect.width, rect.height - splitPoint);

            SplitSpace(first, iterations - 1, floor, roomSizeRange, carveMin, carveMax);
            SplitSpace(second, iterations - 1, floor, roomSizeRange, carveMin, carveMax);
        }

        /// <summary>
        /// Creates a room within the given rectangle, carves out passages, and updates the floor data.
        /// </summary>
        private Room CreateRoom(Rect rect, Vector2Int roomSizeRange, int carveMin, int carveMax, FloorData floor)
        {
            float roomWidth = Random.Range(roomSizeRange.x, Mathf.Min(roomSizeRange.y, rect.width));
            float roomHeight = Random.Range(roomSizeRange.x, Mathf.Min(roomSizeRange.y, rect.height));
            float roomX = rect.x + Random.Range(0, rect.width - roomWidth);
            float roomY = rect.y + Random.Range(0, rect.height - roomHeight);

            Rect roomRect = new Rect(roomX, roomY, roomWidth, roomHeight);
            Room room = new Room(roomRect);

            // Fill the entire room area with floor tiles
            for (int x = Mathf.FloorToInt(roomRect.xMin); x < Mathf.CeilToInt(roomRect.xMax); x++)
            {
                for (int y = Mathf.FloorToInt(roomRect.yMin); y < Mathf.CeilToInt(roomRect.yMax); y++)
                {
                    Vector2Int tilePos = new Vector2Int(x, y);
                    room.FloorTiles.Add(tilePos);
                    floor.FloorTiles.Add(tilePos);
                }
            }

            // Carve out passages
            int carveAttempts = Random.Range(carveMin, carveMax);
            for (int i = 0; i < carveAttempts; i++)
            {
                int carveWidth = Mathf.Max(1, Random.Range(1, Mathf.FloorToInt(roomWidth / 4)));
                int carveHeight = Mathf.Max(1, Random.Range(1, Mathf.FloorToInt(roomHeight / 4)));

                bool carveAlongX = Random.value > 0.5f;
                int carveX = carveAlongX
                    ? (Random.value > 0.5f ? Mathf.FloorToInt(roomRect.xMin) : Mathf.FloorToInt(roomRect.xMax) - carveWidth)
                    : Mathf.FloorToInt(roomRect.xMin) + Random.Range(0, Mathf.FloorToInt(roomWidth - carveWidth));
                
                int carveY = !carveAlongX
                    ? (Random.value > 0.5f ? Mathf.FloorToInt(roomRect.yMin) : Mathf.FloorToInt(roomRect.yMax) - carveHeight)
                    : Mathf.FloorToInt(roomRect.yMin) + Random.Range(0, Mathf.FloorToInt(roomHeight - carveHeight));

                // Carve out the selected area
                for (int x = carveX; x < carveX + carveWidth; x++)
                {
                    for (int y = carveY; y < carveY + carveHeight; y++)
                    {
                        Vector2Int carvePos = new Vector2Int(x, y);
                        room.FloorTiles.Remove(carvePos);
                        floor.FloorTiles.Remove(carvePos);
                    }
                }
            }

            return room;
        }

        /// <summary>
        /// Connects rooms using the Minimum Spanning Tree (MST) algorithm and adds additional connections based on chance.
        /// </summary>
        public void ConnectRoomsUsingMST(FloorData floor, int minCorridorWidth, int maxCorridorWidth, int maxSegmentLength, float straightCorridorChance, float extraConnectionChance = 0.2f)
        {
            List<Edge> edges = new List<Edge>();

            // Generate all possible edges between rooms with their distances
            for (int i = 0; i < floor.Rooms.Count; i++)
            {
                for (int j = i + 1; j < floor.Rooms.Count; j++)
                {
                    float weight = Vector2.Distance(floor.Rooms[i].Center, floor.Rooms[j].Center);
                    edges.Add(new Edge(floor.Rooms[i], floor.Rooms[j], weight)); 
                }
            }

            // Sort edges by distance (smallest to largest)
            edges.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));
            DisjointSet disjointSet = new DisjointSet(floor.Rooms);

            foreach (Edge edge in edges)
            {
                // If the rooms are in different sets, connect them
                if (disjointSet.Find(edge.RoomA) != disjointSet.Find(edge.RoomB))
                {
                    disjointSet.Union(edge.RoomA, edge.RoomB);
                    CreateCorridorBetweenRooms(edge.RoomA, edge.RoomB, floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance);
                }
                // If MST is complete, break out of loop
                if (disjointSet.Count == 1) break;
            }

            // Add additional connections based on extra connection chance
            foreach (Edge edge in edges)
            {
                if (Random.value < extraConnectionChance && disjointSet.Find(edge.RoomA) != disjointSet.Find(edge.RoomB))
                {
                    disjointSet.Union(edge.RoomA, edge.RoomB);
                    CreateCorridorBetweenRooms(edge.RoomA, edge.RoomB, floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance);
                }
            }
        }

        /// <summary>
        /// Creates a corridor between two rooms and adds it to the floor data.
        /// </summary>
        private void CreateCorridorBetweenRooms(Room roomA, Room roomB, FloorData floor, int minCorridorWidth, int maxCorridorWidth, int maxSegmentLength, float straightCorridorChance)
        {
            Vector2Int start = GetRandomWallPosition(roomA);
            Vector2Int end = GetRandomWallPosition(roomB);
            HashSet<Vector2Int> corridorTiles = new HashSet<Vector2Int>(
                GenerateCorridorTiles(start, end, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance)
            );
            floor.AddCorridor(corridorTiles, roomA, roomB);
        }

        /// <summary>
        /// Renders a room onto the specified tilemap.
        /// </summary>
        public void RenderRoom(Room room, Tilemap tilemap, TileBase tile)
        {
            // Fill the room area with tiles
            foreach (Vector2Int tilePos in room.FloorTiles)
            {
                Vector3Int tilePosition = new Vector3Int(tilePos.x, tilePos.y, 0);
                tilemap.SetTile(tilePosition, tile);
            }
        }

        /// <summary>
        /// Renders a corridor onto the specified tilemap.
        /// </summary>
        public void RenderCorridor(HashSet<Vector2Int> corridor, Tilemap tilemap, TileBase corridorTile)
        {
            foreach (Vector2Int tilePos in corridor)
            {
                Vector3Int tilePosition = new Vector3Int(tilePos.x, tilePos.y, 0);
                tilemap.SetTile(tilePosition, corridorTile);
            }
        }

        /// <summary>
        /// Creates walls around the floor tiles.
        /// </summary>
        public void CreateWallsForFloor(HashSet<Vector2Int> floorTiles, Tilemap wallTilemap, TileBase wallTile)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int tile in floorTiles)
            {
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = tile + dir;
                    if (!floorTiles.Contains(neighbor))
                    {
                        Vector3Int wallPosition = new Vector3Int(neighbor.x, neighbor.y, 0);
                        wallTilemap.SetTile(wallPosition, wallTile);
                    }
                }
            }
        }

        /// <summary>
        /// Generates patrol points for a floor.
        /// </summary>
        public void GeneratePatrolPoints(FloorData floor)
        {
            List<Vector3> patrolPoints = GetRandomFloorTiles(floor, 30); // Assuming 30 patrol points
            dungeonGenerator.floorPatrolPoints[floor.FloorNumber] = patrolPoints;
        }

        /// <summary>
        /// Places stairs between the current and next floor.
        /// </summary>
        public void PlaceStairs(FloorData currentFloor, FloorData nextFloor, GameObject stairsUpPrefab, GameObject stairsDownPrefab)
        {
            // Choose a random walkable position in a room or corridor on the current floor for the down stairs
            Vector2Int downStairsPosition = GetRandomWalkablePosition(currentFloor);

            // Instantiate the stairs going down on the current floor
            InstantiatePrefab(stairsDownPrefab, new Vector3(downStairsPosition.x, downStairsPosition.y, 0), GetFloorParent(currentFloor));

            // Ensure the position for up stairs on the next floor aligns with the down stairs and is walkable
            Vector2Int upStairsPosition = AlignStairsOnNextFloor(nextFloor, downStairsPosition);

            // Instantiate the stairs going up on the next floor
            InstantiatePrefab(stairsUpPrefab, new Vector3(upStairsPosition.x, upStairsPosition.y, 0), GetFloorParent(nextFloor));
        }

        /// <summary>
        /// Aligns stairs on the next floor to the specified position, ensuring walkability.
        /// </summary>
        private Vector2Int AlignStairsOnNextFloor(FloorData nextFloor, Vector2Int targetPosition)
        {
            // Check if target position is within the existing walkable tiles of the next floor
            if (!nextFloor.FloorTiles.Contains(targetPosition))
            {
                // Find the nearest walkable tile
                targetPosition = FindNearestWalkableTile(nextFloor, targetPosition);
            }

            return targetPosition;
        }

        /// <summary>
        /// Finds the nearest walkable tile to a given position on the floor.
        /// </summary>
        private Vector2Int FindNearestWalkableTile(FloorData floor, Vector2Int position)
        {
            return floor.FloorTiles
                        .OrderBy(t => Vector2Int.Distance(t, position))
                        .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a random walkable position within the specified floor.
        /// </summary>
        private Vector2Int GetRandomWalkablePosition(FloorData floor)
        {
            Room randomRoom = floor.Rooms[Random.Range(0, floor.Rooms.Count)];
            return randomRoom.FloorTiles.ElementAt(Random.Range(0, randomRoom.FloorTiles.Count));
        }

        /// <summary>
        /// Retrieves a random wall position from a room.
        /// </summary>
        private Vector2Int GetRandomWallPosition(Room room)
        {
            List<Vector2Int> wallTiles = new List<Vector2Int>();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int tile in room.FloorTiles)
            {
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = tile + dir;
                    if (!room.FloorTiles.Contains(neighbor))
                    {
                        wallTiles.Add(tile);
                        break;
                    }
                }
            }

            if (wallTiles.Count > 0)
                return wallTiles[Random.Range(0, wallTiles.Count)];
            else
                return Vector2Int.RoundToInt(room.Center);
        }

        /// <summary>
        /// Retrieves the parent transform for a given floor.
        /// </summary>
        private Transform GetFloorParent(FloorData floor)
        {
            return dungeonGenerator.dungeonParent.transform.Find($"Floor_{floor.FloorNumber}");
        }

        /// <summary>
        /// Generates corridor tiles between two points.
        /// </summary>
        private List<Vector2Int> GenerateCorridorTiles(Vector2Int start, Vector2Int end, int minWidth, int maxWidth, int maxSegmentLength, float straightChance)
        {
            List<Vector2Int> corridorTiles = new List<Vector2Int>();
            Vector2Int current = start;
            int corridorWidth = 1;
            bool hasWidened = false;

            while (current != end)
            {
                Vector2Int direction;

                if (current.x != end.x && Random.value > straightChance)
                {
                    direction = new Vector2Int(Mathf.Sign(end.x - current.x), 0);
                }
                else if (current.y != end.y)
                {
                    direction = new Vector2Int(0, Mathf.Sign(end.y - current.y));
                }
                else
                {
                    direction = new Vector2Int(Mathf.Sign(end.x - current.x), Mathf.Sign(end.y - current.y));
                }

                int segmentLength = Random.Range(2, maxSegmentLength + 1);
                for (int i = 0; i < segmentLength; i++)
                {
                    if (current == end) break;

                    // Add corridor tiles with the current width
                    corridorTiles.Add(current);
                    for (int w = 1; w < corridorWidth; w++)
                    {
                        Vector2Int offset = (direction.y == 0) ? new Vector2Int(0, w) : new Vector2Int(w, 0);
                        corridorTiles.Add(current + offset);
                    }

                    current += direction;

                    // Increase corridor width once
                    if (!hasWidened && corridorWidth < maxWidth && Mathf.Abs(current.x - start.x) > 1 && Mathf.Abs(current.y - start.y) > 1)
                    {
                        corridorWidth = Random.Range(minWidth, maxWidth + 1);
                        hasWidened = true;
                    }

                    // Prevent overshooting
                    if (Vector2Int.Distance(current, end) <= 1)
                        current = end;
                }
            }

            return corridorTiles;
        }

        /// <summary>
        /// Checks if a given position is a floor tile.
        /// </summary>
        public bool IsFloorTile(FloorData floor, Vector2Int position)
        {
            return floor.FloorTiles.Contains(position);
        }

        /// <summary>
        /// Finds the nearest walkable tile within a floor's tiles.
        /// </summary>
        private Vector2Int FindNearestWalkableTile(FloorData floor, Vector2Int position)
        {
            return floor.FloorTiles
                        .OrderBy(t => Vector2Int.Distance(t, position))
                        .FirstOrDefault();
        }

        // Additional utility classes and methods remain unchanged or are refined as needed

        /// <summary>
        /// Represents an edge between two rooms with an associated weight (distance).
        /// </summary>
        private class Edge
        {
            public Room RoomA { get; }
            public Room RoomB { get; }
            public float Weight { get; }

            public Edge(Room roomA, Room roomB, float weight)
            {
                RoomA = roomA;
                RoomB = roomB;
                Weight = weight;
            }
        }

        /// <summary>
        /// Implements a Disjoint Set (Union-Find) data structure for Krusky's algorithm.
        /// </summary>
        private class DisjointSet
        {
            private readonly Dictionary<Room, Room> parent;
            private int count;

            public DisjointSet(List<Room> rooms)
            {
                parent = new Dictionary<Room, Room>();
                count = rooms.Count;

                foreach (Room room in rooms)
                {
                    parent[room] = room;
                }
            }

            public Room Find(Room room)
            {
                if (parent[room] != room)
                {
                    parent[room] = Find(parent[room]); // Path compression
                }
                return parent[room];
            }

            public void Union(Room roomA, Room roomB)
            {
                Room rootA = Find(roomA);
                Room rootB = Find(roomB);

                if (rootA != rootB)
                {
                    parent[rootA] = rootB;
                    count--;
                }
            }

            public int Count => count;
        }
    }
}
