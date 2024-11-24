using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using YourGameNamespace;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace YourGameNamespace
{
    public class DungeonGeneratorMethods
    {        
        
        [SerializeField] private DungeonGenerator dungeonGenerator; // Assign via Inspector
        public HashSet<Vector2Int> floorTiles  = new HashSet<Vector2Int>();

        public DungeonGeneratorMethods(DungeonGenerator generator)
        {
            dungeonGenerator = generator;

        }

        public Vector3 GetRandomFloorTile()
        {
            // Select a random floor tile position from the HashSet
            Vector2Int[] floorTileArray = floorTiles.ToArray();
            Vector2Int randomTile = floorTileArray[Random.Range(0, floorTileArray.Length)];
            return new Vector3(randomTile.x, randomTile.y, 0);
        }

        public List<Vector3> GetRandomFloorTile(int numberOfPositions )
        {
            Vector2Int[] floorTileArray = floorTiles.ToArray();
            List<Vector3> positions = new List<Vector3>();

            for (int i = 0; i < numberOfPositions; i++)
            {

            // Select a random floor tile position from the HashSet
            Vector2Int randomTile = floorTileArray[Random.Range(0, floorTileArray.Length)];
            positions.Add(new Vector3(randomTile.x, randomTile.y, 0));
            }
            return positions;
        }


        private GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Transform parent)
        {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }

        public void GenerateFloorTiles(Rect initialRect, int maxIterations, FloorData floor, Vector2Int roomSizeRange, int carveMin, int carveMax)
        {
            SplitSpace(initialRect, maxIterations, floor, roomSizeRange, carveMin, carveMax);
        }

        private void SplitSpace(Rect rect, int iterations, FloorData floor, Vector2Int roomSizeRange, int carveMin, int carveMax)
        {
            if (iterations == 0)
            {
                Room room = CreateRoom(rect, roomSizeRange, carveMin, carveMax);
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

        private Room CreateRoom(Rect rect, Vector2Int roomSizeRange, int carveMin, int carveMax)
        {
            // Define room dimensions and position within bounds
            float roomWidth = Random.Range(roomSizeRange.x, Mathf.Min(roomSizeRange.y, rect.width));
            float roomHeight = Random.Range(roomSizeRange.x, Mathf.Min(roomSizeRange.y, rect.height));
            float roomX = rect.x + Random.Range(0, rect.width - roomWidth);
            float roomY = rect.y + Random.Range(0, rect.height - roomHeight);

            // Create the room with calculated size and position
            Rect roomRect = new Rect(roomX, roomY, roomWidth, roomHeight);
            Room newRoom = new Room(roomRect, new HashSet<Vector2Int>());

            // Carving logic: Add random carving within the roomRect bounds
            for (int x = Mathf.FloorToInt(roomRect.xMin); x < Mathf.CeilToInt(roomRect.xMax); x++)
            {
                for (int y = Mathf.FloorToInt(roomRect.yMin); y < Mathf.CeilToInt(roomRect.yMax); y++)
                {
                    Vector2Int position = new Vector2Int(x, y);

                    // Decide whether to carve this tile based on carve logic
                    if (ShouldCarveTile(position, roomRect))
                    {
                        newRoom.CarvedTiles.Add(position);
                        newRoom.FloorTiles.Add(position);  // Keep track of floor tiles as well
                    }
                }
            }

            return newRoom;
        }

        private bool ShouldCarveTile(Vector2Int position, Rect roomRect)
        {
            // Calculate distance from edges
            float distanceFromLeft = position.x - roomRect.xMin;
            float distanceFromRight = roomRect.xMax - position.x;
            float distanceFromBottom = position.y - roomRect.yMin;
            float distanceFromTop = roomRect.yMax - position.y;

            // Find the minimum distance to an edge
            float minDistanceToEdge = Mathf.Min(distanceFromLeft, distanceFromRight, distanceFromBottom, distanceFromTop);

            // Set a threshold for carving based on the distance to edges
            float edgeThreshold = 3f; // The closer to the edge, the more likely to carve

            // Probability calculation
            if (minDistanceToEdge <= edgeThreshold)
            {
                // Higher probability to carve near edges
                return Random.value > 0.95f; // 80% chance to carve if near the edge
            }
            else
            {
                // Lower probability to carve in the middle
                return Random.value > 0.05f; // 20% chance to carve if in the middle
            }
        }


        // This method checks if a given position corresponds to a floor tile
        public bool IsFloorTile(Vector2Int position)
        {
            // Return true if the tilePosition is in the floorTiles set
            bool isFloor = floorTiles.Contains(position);
            //// // Debug.Log($"Checking tile position {position}: IsFloor = {isFloor}");
            return isFloor;       
        }

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
                    CreateCorridorBetweenRoomEntrances(edge.RoomA, edge.RoomB, floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance);
                }
                // If MST is complete, break out of loop
                if (disjointSet.Count == 1) break;
            }

            // Add additional connections based on extra connection chance
            foreach (Edge edge in edges)
            {
                if (Random.value < extraConnectionChance && disjointSet.Find(edge.RoomA) != disjointSet.Find(edge.RoomB))
                {
                    CreateCorridorBetweenRoomEntrances(edge.RoomA, edge.RoomB, floor, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance);
                }
            }
        }

        private void CreateCorridorBetweenRoomEntrances(Room roomA, Room roomB, FloorData floor, int minCorridorWidth, int maxCorridorWidth, int maxSegmentLength, float straightCorridorChance)
        {
            // Find entrance points for both rooms
            Vector2Int entranceA = GetRoomEntrance(roomA);
            Vector2Int entranceB = GetRoomEntrance(roomB);

            // Generate corridor to connect the two entrance points without passing through the rooms
            List<Vector2Int> corridorPath = GenerateCorridorTiles(entranceA, entranceB, minCorridorWidth, maxCorridorWidth, maxSegmentLength, straightCorridorChance);
            HashSet<Vector3Int> corridorTiles = new HashSet<Vector3Int>(corridorPath.Select(tile => new Vector3Int(tile.x, tile.y, 0)));
            
            // Create and add the corridor
            Corridor corridor = new Corridor(corridorTiles, roomA, roomB);
            floor.AddCorridor(corridorTiles, roomA, roomB);

            // Render the corridor
            RenderCorridor(corridor.CorridorTiles, floor.CorridorTilemap, dungeonGenerator.corridorTile);
        }

        private Vector2Int GetRoomEntrance(Room room)
        {
            // Choose an entrance point on the boundary of the room
            List<Vector2Int> boundaryTiles = GetBoundaryTiles(room);
            return boundaryTiles[Random.Range(0, boundaryTiles.Count)];
        }

        private List<Vector2Int> GetBoundaryTiles(Room room)
        {
            List<Vector2Int> boundaryTiles = new List<Vector2Int>();
            Vector2Int roomMin = Vector2Int.FloorToInt(new Vector2(room.Rect.xMin, room.Rect.yMin));
            Vector2Int roomMax = Vector2Int.FloorToInt(new Vector2(room.Rect.xMax, room.Rect.yMax));

            // Get all boundary tiles of the room
            for (int x = roomMin.x; x <= roomMax.x; x++)
            {
                boundaryTiles.Add(new Vector2Int(x, roomMin.y)); // Bottom boundary
                boundaryTiles.Add(new Vector2Int(x, roomMax.y)); // Top boundary
            }
            for (int y = roomMin.y + 1; y < roomMax.y; y++) // +1 and < to avoid corners being duplicated
            {
                boundaryTiles.Add(new Vector2Int(roomMin.x, y)); // Left boundary
                boundaryTiles.Add(new Vector2Int(roomMax.x, y)); // Right boundary
            }

            return boundaryTiles;
        }

public void RenderRoom(Room room, Tilemap tilemap, TileBase tile)
{
    // Render only the carved floor tiles within the room boundaries
    foreach (Vector2Int position in room.CarvedTiles)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        tilemap.SetTile(tilePosition, tile);
    }
}


                public void RenderCorridor(HashSet<Vector3Int> corridor, Tilemap tilemap, TileBase corridorTile)
        {
            foreach (Vector2Int tilePos in corridor)
            {
                Vector3Int tilePosition = new Vector3Int(tilePos.x, tilePos.y, 0);
                tilemap.SetTile(tilePosition, corridorTile); // Set the corridor tile at each position
            }
        }

        public void CreateWallsForFloor(HashSet<Vector3Int> floorTiles, Tilemap wallTilemap, TileBase wallTile)
        {
            // Define all directions, including diagonals
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                                        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };

            foreach (Vector3Int tile in floorTiles)
            {
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor2D = new Vector2Int(tile.x + dir.x, tile.y + dir.y);
                    Vector3Int neighbor3D = new Vector3Int(neighbor2D.x, neighbor2D.y, 0);

                    if (!floorTiles.Contains(neighbor3D)) // Only add walls where there is no floor tile
                    {
                        Vector3Int wallPosition = new Vector3Int(neighbor3D.x, neighbor3D.y, 0);
                        wallTilemap.SetTile(wallPosition, wallTile); // Place the wall tile on the Tilemap
                    }
                }
            }
        }



        public class DisjointSet
        {
            private readonly Dictionary<Room, Room> parent;
            private int count;

            // Constructor initializes each room as its own set
            public DisjointSet(List<Room> rooms)
            {
                parent = new Dictionary<Room, Room>();
                count = rooms.Count;

                foreach (Room room in rooms)
                {
                    parent[room] = room; // Each room is its own parent initially
                }
            }

            // Find method with path compression
            public Room Find(Room room)
            {
                if (parent[room] != room)
                {
                    parent[room] = Find(parent[room]); // Path compression
                }
                return parent[room];
            }

            // Union method to connect two sets
            public void Union(Room roomA, Room roomB)
            {
                Room rootA = Find(roomA);
                Room rootB = Find(roomB);

                if (rootA != rootB)
                {
                    parent[rootA] = rootB;
                    count--; // Decrease disjoint set count
                }
            }

            // Property to get the current count of disjoint sets
            public int Count => count;  // This ensures 'Count' is accessed as a property
        }

        private List<Vector2Int> GenerateCorridorTiles(Vector2Int start, Vector2Int end, int minWidth, int maxWidth, int maxSegmentLength, float straightChance)
        {
            List<Vector2Int> corridorTiles = new List<Vector2Int>();
            Vector2Int current = start;
            int initialWidth = 1; // Start with a width of one tile at the connection point
            int corridorWidth = initialWidth;

            bool hasWidened = false;

            while (current != end)
            {
                Vector2Int direction;

                if (current.x != end.x && Random.value > straightChance)
                {
                    direction = new Vector2Int(Mathf.RoundToInt(Mathf.Sign(end.x - current.x)), 0);
                }
                else if (current.y != end.y)
                {
                    direction = new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(end.y - current.y)));
                }
                else
                {
                    direction = new Vector2Int(Mathf.RoundToInt(Mathf.Sign(end.x - current.x)), Mathf.RoundToInt(Mathf.Sign(end.y - current.y)));
                }

                int segmentLength = Random.Range(2, maxSegmentLength);
                for (int i = 0; i < segmentLength; i++)
                {
                    if (current == end) break;

                    // Add corridor tiles with the current width
                    corridorTiles.Add(current);
                    for (int w = 1; w < corridorWidth; w++)
                    {
                        Vector2Int offset = direction.y == 0 ? new Vector2Int(0, w) : new Vector2Int(w, 0);
                        corridorTiles.Add(current + offset);
                    }

                    current += direction;

                    // If we've moved away from the room connection point, increase corridor width
                    if (!hasWidened && corridorWidth < maxWidth && Mathf.Abs(current.x - start.x) > 1 && Mathf.Abs(current.y - start.y) > 1)
                    {
                        corridorWidth = Random.Range(minWidth, maxWidth + 1);
                        hasWidened = true; // Ensure widening only happens once
                    }

                    // Stop early if very close to the end point to avoid overshooting
                    if (Mathf.Abs(current.x - end.x) <= 1 && Mathf.Abs(current.y - end.y) <= 1)
                        current = end;
                }
            }

            return corridorTiles;
        }

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

        // Ensures there is a walkable position on the next floor, aligning with the down stairs on the previous floor
        private Vector2Int AlignStairsOnNextFloor(FloorData nextFloor, Vector2Int targetPosition)
        {
            // Check if target position is within the existing walkable tiles of the next floor
            Vector3Int targetPosition3D = new Vector3Int(targetPosition.x, targetPosition.y, 0);

            if (!nextFloor.FloorTiles.Contains(targetPosition3D))
            {
                // Option 1: Make the target position walkable by adding a floor tile
                // nextFloor.FloorTiles.Add(targetPosition3D);

                // Option 2: Or find the nearest walkable tile if you want to avoid creating a new floor tile
                targetPosition = FindNearestWalkableTile(nextFloor, targetPosition);
            }

            return targetPosition;
        }



                // Retrieves a random walkable position within rooms or corridors of the specified floor
        private Vector2Int GetRandomWalkablePosition(FloorData floor)
        {
            Room randomRoom = floor.Rooms[Random.Range(0, floor.Rooms.Count)];
            
            // Convert Vector3Int to Vector2Int when returning
            Vector2Int randomTile = randomRoom.FloorTiles.ElementAt(Random.Range(0, randomRoom.FloorTiles.Count));
            return new Vector2Int(randomTile.x, randomTile.y);
        }

        // Helper method to find the nearest walkable tile if needed (Optional)
        private Vector2Int FindNearestWalkableTile(FloorData floor, Vector2Int position)
        {
            // Convert floor.FloorTiles (Vector3Int) to Vector2Int for comparison
            foreach (Vector3Int tile in floor.FloorTiles.OrderBy(t => Vector2Int.Distance(new Vector2Int(t.x, t.y), position)))
            {
                Vector2Int tile2D = new Vector2Int(tile.x, tile.y);

                if (floor.FloorTiles.Contains(new Vector3Int(tile2D.x, tile2D.y, 0)))
                    return tile2D;
            }
            return position; // Fallback if no walkable tile found
}



        private Transform GetFloorParent(FloorData floor)
        {
            return dungeonGenerator.dungeonParent.transform.Find($"Floor_{floor.FloorNumber}");
        }

        private Vector2Int GetRandomWalkablePositionInRoom(FloorData floor)
        {
            Room room = floor.Rooms[Random.Range(0, floor.Rooms.Count)];
            return room.FloorTiles.ElementAt(Random.Range(0, room.FloorTiles.Count));
        }

        private Vector2Int GetRandomWallPosition(Room room)
        {
            List<Vector2Int> wallTiles = new List<Vector2Int>();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int tile in room.FloorTiles)
            {
                foreach (Vector2Int dir in directions)
                {
                    if (!room.FloorTiles.Contains(tile + dir))
                    {
                        wallTiles.Add(tile);
                        break;
                    }
                }
            }

            return wallTiles.Count > 0 ? wallTiles[Random.Range(0, wallTiles.Count)] : Vector2Int.RoundToInt(room.Center);
        }
    }
}
