using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace YourGameNamespace
{
    public class FloorGenerator
    {
        private DungeonSettings dungeonSettings;

        public FloorGenerator(DungeonSettings settings)
        {
            dungeonSettings = settings;
        }

        /// <summary>
        /// Generates a single floor and populates its data.
        /// </summary>
        public FloorData GenerateFloor(int floorNumber, Tilemap floorTilemap, Tilemap corridorTilemap, TileBase floorTile, TileBase corridorTile)
        {
            // Create runtime FloorData
            FloorData floorData = new FloorData(floorNumber);

            // Generate rooms
            Rect initialRect = new Rect(
                -dungeonSettings.dungeonSizeRange.x / 2,
                -dungeonSettings.dungeonSizeRange.y / 2,
                dungeonSettings.dungeonSizeRange.x,
                dungeonSettings.dungeonSizeRange.y
            );
            SplitSpace(initialRect, dungeonSettings.maxBSPIterations, floorData);
           
            GenerateRooms(floorData);

            // Corridor generation
            ConnectRoomsWithCorridors(floorData, dungeonSettings.straightCorridorChance);

            // Render floor tiles (rooms and corridors)
            RenderFloorTiles(floorData, floorTilemap, floorTile, corridorTile);

            // Render walls
            CreateWallsForFloor(floorData.FloorTiles, wallTilemap, wallTile);
            // Render rooms
            foreach (Room room in floorData.Rooms)
            {
                RenderRoom(room, floorTilemap, floorTile);
            }

            // Connect rooms with corridors
            ConnectRoomsUsingMST(floorData);
            foreach (var corridor in floorData.Corridors)
            {
                RenderCorridor(corridor.CorridorTiles, corridorTilemap, corridorTile);
            }

            return floorData;
        }
        private void GenerateRooms(FloorData floorData)
        {
            // Step 1: Divide the floor area using Binary Space Partitioning (BSP)
            List<Rect> partitions = PerformBSPPartitioning(floorData.FloorBounds, dungeonSettings.maxBSPIterations);

            // Step 2: Generate rooms within each partition
            foreach (Rect partition in partitions)
            {
                // Use the CreateRoom method to generate a room for this partition
                Room room = CreateRoom(partition, dungeonSettings.roomSizeRange, dungeonSettings.carvingIterationsMin, dungeonSettings.carvingIterationsMax);

                // Add the generated room to the FloorData
                floorData.AddRoom(room);
            }
        }
        private List<Rect> PerformBSPPartitioning(Rect floorBounds, int maxIterations)
        {
            List<Rect> partitions = new List<Rect> { floorBounds };

            for (int i = 0; i < maxIterations; i++)
            {
                List<Rect> newPartitions = new List<Rect>();

                foreach (Rect partition in partitions)
                {
                    // Decide whether to split horizontally or vertically
                    bool splitHorizontally = partition.width > partition.height;

                    // Minimum size for a partition to allow splitting
                    float minPartitionSize = Mathf.Max(dungeonSettings.roomSizeRange.x * 2, 20); // Example: room size or fixed minimum

                    if (splitHorizontally && partition.width >= minPartitionSize * 2)
                    {
                        // Split vertically into two partitions
                        float splitPoint = Random.Range(minPartitionSize, partition.width - minPartitionSize);
                        Rect leftPartition = new Rect(partition.x, partition.y, splitPoint, partition.height);
                        Rect rightPartition = new Rect(partition.x + splitPoint, partition.y, partition.width - splitPoint, partition.height);

                        newPartitions.Add(leftPartition);
                        newPartitions.Add(rightPartition);
                    }
                    else if (partition.height >= minPartitionSize * 2)
                    {
                        // Split horizontally into two partitions
                        float splitPoint = Random.Range(minPartitionSize, partition.height - minPartitionSize);
                        Rect topPartition = new Rect(partition.x, partition.y + splitPoint, partition.width, partition.height - splitPoint);
                        Rect bottomPartition = new Rect(partition.x, partition.y, partition.width, splitPoint);

                        newPartitions.Add(topPartition);
                        newPartitions.Add(bottomPartition);
                    }
                    else
                    {
                        // If the partition is too small to split, keep it as-is
                        newPartitions.Add(partition);
                    }
                }

                partitions = newPartitions;
            }

            return partitions;
        }


        private void ConnectRoomsWithCorridors(FloorData floorData, float straightCorridorChance)
        {
            List<Edge> edges = GenerateMinimumSpanningTree(floorData.Rooms);

            foreach (var edge in edges)
            {
                CreateCorridorBetweenRooms(edge.RoomA, edge.RoomB, floorData, straightCorridorChance);
            }
        }

        private void RenderFloorTiles(FloorData floorData, Tilemap floorTilemap, TileBase floorTile, TileBase corridorTile)
        {
            // Render rooms
            foreach (var room in floorData.Rooms)
            {
                foreach (var tile in room.FloorTiles)
                {
                    floorTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), floorTile);
                }
            }

            // Render corridors
            foreach (var corridor in floorData.Corridors)
            {
                foreach (var tile in corridor.CorridorTiles)
                {
                    floorTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), corridorTile);
                }
            }
        }

        /// <summary>
        /// Creates walls around the edges of the floor tiles.
        /// </summary>
        private void CreateWallsForFloor(HashSet<Vector2Int> floorTiles, Tilemap wallTilemap, TileBase wallTile)
        {
            foreach (var tile in floorTiles)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int neighbor = tile + new Vector2Int(x, y);
                        if (!floorTiles.Contains(neighbor))
                        {
                            wallTilemap.SetTile(new Vector3Int(neighbor.x, neighbor.y, 0), wallTile);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Splits a space recursively to generate rooms using BSP.
        /// </summary>
        private void SplitSpace(Rect rect, int iterations, FloorData floorData)
        {
            if (iterations == 0)
            {
                Room room = CreateRoom(rect, floorData);
                floorData.AddRoom(room);
                return;
            }

            bool splitVertically = Random.value > 0.5f;
            float splitPoint = splitVertically
                ? Random.Range(rect.xMin + dungeonSettings.roomSizeRange.x, rect.xMax - dungeonSettings.roomSizeRange.x)
                : Random.Range(rect.yMin + dungeonSettings.roomSizeRange.x, rect.yMax - dungeonSettings.roomSizeRange.x);

            if (splitVertically)
            {
                Rect left = new Rect(rect.xMin, rect.yMin, splitPoint - rect.xMin, rect.height);
                Rect right = new Rect(splitPoint, rect.yMin, rect.xMax - splitPoint, rect.height);
                SplitSpace(left, iterations - 1, floorData);
                SplitSpace(right, iterations - 1, floorData);
            }
            else
            {
                Rect top = new Rect(rect.xMin, splitPoint, rect.width, rect.yMax - splitPoint);
                Rect bottom = new Rect(rect.xMin, rect.yMin, rect.width, splitPoint - rect.yMin);
                SplitSpace(top, iterations - 1, floorData);
                SplitSpace(bottom, iterations - 1, floorData);
            }
        }

        /// <summary>
        /// Creates a room within a given rectangular space.
        /// </summary>
private Room CreateRoom(Rect rect, Vector2Int roomSizeRange, int carveMin, int carveMax)
{
    // Validate the rect size
    if (rect.width < roomSizeRange.x || rect.height < roomSizeRange.x)
    {
        throw new System.Exception("Room rect is too small to fit the minimum room size.");
    }

    // Calculate random room size and position
    float roomWidth = Random.Range(roomSizeRange.x, Mathf.Min(roomSizeRange.y, rect.width));
    float roomHeight = Random.Range(roomSizeRange.x, Mathf.Min(roomSizeRange.y, rect.height));
    float roomX = rect.x + Random.Range(0, rect.width - roomWidth);
    float roomY = rect.y + Random.Range(0, rect.height - roomHeight);

    Rect roomRect = new Rect(roomX, roomY, roomWidth, roomHeight);

    // Create a local collection for the room's floor tiles
    HashSet<Vector2Int> roomFloorTiles = new HashSet<Vector2Int>();

    // Fill the room area with floor tiles
    for (int x = Mathf.FloorToInt(roomRect.xMin); x < Mathf.CeilToInt(roomRect.xMax); x++)
    {
        for (int y = Mathf.FloorToInt(roomRect.yMin); y < Mathf.CeilToInt(roomRect.yMax); y++)
        {
            roomFloorTiles.Add(new Vector2Int(x, y));
        }
    }

    // Randomly carve out areas within the room
    int carveAttempts = Random.Range(carveMin, carveMax);
    for (int i = 0; i < carveAttempts; i++)
    {
        int carveWidth = Random.Range(1, Mathf.FloorToInt(roomWidth / 4));
        int carveHeight = Random.Range(1, Mathf.FloorToInt(roomHeight / 4));

        bool carveAlongX = Random.value > 0.5f;
        int carveX = carveAlongX
            ? (Random.value > 0.5f ? Mathf.FloorToInt(roomRect.xMin) : Mathf.FloorToInt(roomRect.xMax) - carveWidth)
            : Mathf.FloorToInt(roomRect.xMin) + Random.Range(0, Mathf.FloorToInt(roomWidth - carveWidth));

        int carveY = !carveAlongX
            ? (Random.value > 0.5f ? Mathf.FloorToInt(roomRect.yMin) : Mathf.FloorToInt(roomRect.yMax) - carveHeight)
            : Mathf.FloorToInt(roomRect.yMin) + Random.Range(0, Mathf.FloorToInt(roomHeight - carveHeight));

        // Remove carved tiles
        for (int x = carveX; x < carveX + carveWidth; x++)
        {
            for (int y = carveY; y < carveY + carveHeight; y++)
            {
                roomFloorTiles.Remove(new Vector2Int(x, y));
            }
        }
    }

    return new Room(roomRect, roomFloorTiles);
}


        /// <summary>
        /// Renders a room on the given tilemap.
        /// </summary>
        private void RenderRoom(Room room, Tilemap tilemap, TileBase floorTile)
        {
            foreach (Vector2Int tilePos in room.FloorTiles)
            {
                tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);
            }
        }

        /// <summary>
        /// Connects rooms using Minimum Spanning Tree (MST).
        /// </summary>
        private void ConnectRoomsUsingMST(FloorData floorData)
        {
            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < floorData.Rooms.Count; i++)
            {
                for (int j = i + 1; j < floorData.Rooms.Count; j++)
                {
                    float weight = Vector2.Distance(floorData.Rooms[i].Center, floorData.Rooms[j].Center);
                    edges.Add(new Edge(floorData.Rooms[i], floorData.Rooms[j], weight));
                }
            }

            edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            DisjointSet disjointSet = new DisjointSet(floorData.Rooms);

            foreach (var edge in edges)
            {
                if (disjointSet.Find(edge.RoomA) != disjointSet.Find(edge.RoomB))
                {
                    disjointSet.Union(edge.RoomA, edge.RoomB);
                    CreateCorridorBetweenRooms(edge.RoomA, edge.RoomB, floorData);
                }
            }
        }

        /// <summary>
        /// Creates a corridor between two rooms and adds it to the floor data.
        /// </summary>
        private void CreateCorridorBetweenRooms(Room roomA, Room roomB, FloorData floorData)
        {
            Vector2Int start = roomA.GetRandomWallPosition();
            Vector2Int end = roomB.GetRandomWallPosition();

            HashSet<Vector2Int> corridorTiles = GenerateCorridor(start, end);
            floorData.AddCorridor(corridorTiles, roomA, roomB);
        }

        /// <summary>
        /// Renders a corridor on the given tilemap.
        /// </summary>
        private void RenderCorridor(HashSet<Vector2Int> corridorTiles, Tilemap tilemap, TileBase corridorTile)
        {
            foreach (Vector2Int tilePos in corridorTiles)
            {
                tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), corridorTile);
            }
        }

        /// <summary>
        /// Generates corridor tiles between two points.
        /// </summary>
        private HashSet<Vector2Int> GenerateCorridor(Vector2Int start, Vector2Int end)
        {
            HashSet<Vector2Int> corridorTiles = new HashSet<Vector2Int>();
            Vector2Int current = start;

            while (current != end)
            {
                corridorTiles.Add(current);

                if (current.x != end.x)
                {
                    current.x += (current.x < end.x) ? 1 : -1;
                }
                else if (current.y != end.y)
                {
                    current.y += (current.y < end.y) ? 1 : -1;
                }
            }

            corridorTiles.Add(end);
            return corridorTiles;
        }
    }
}

