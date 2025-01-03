using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class CarvingAlgorithm
    {
        public static HashSet<Vector2Int> Execute(
            CarvingAlgorithmType algorithmType,
            DungeonSettings settings,
            RectInt bounds
        )
        {
            switch (algorithmType)
            {
                case CarvingAlgorithmType.CellularAutomata:
                    return CellularAutomata(settings, bounds);
                case CarvingAlgorithmType.PerlinNoise:
                    return PerlinNoise(settings, bounds);
                case CarvingAlgorithmType.BSP:
                    return BSP(settings, bounds);
                case CarvingAlgorithmType.SpiralPattern:
                    return SpiralPattern(settings, bounds);
                case CarvingAlgorithmType.WaveFunctionCollapse:
                    return WaveFunctionCollapse(settings, bounds);
                case CarvingAlgorithmType.HybridAlgorithm:
                    return HybridAlgorithm(settings, bounds);
                case CarvingAlgorithmType.IslandGrowthAlgorithm:
                    return IslandGrowthAlgorithm.GenerateDungeon(
                        bounds,
                        settings.selectedAlgorithm.islandCount,
                        settings.selectedAlgorithm.islandSize
                    );
                case CarvingAlgorithmType.LSystemAlgorithm:
                    return LSystemAlgorithm.GenerateDungeon(
                        settings.selectedAlgorithm.lSystemStart,
                        settings.selectedAlgorithm.lSystemIterations,
                        settings.selectedAlgorithm.lSystemBranchChance
                    );
                case CarvingAlgorithmType.RipplePropagationAlgorithm:
                    return RipplePropagationAlgorithm.GenerateDungeon(
                        bounds,
                        settings.selectedAlgorithm.rippleCenters,
                        settings.selectedAlgorithm.rippleMaxRadius
                    );
                case CarvingAlgorithmType.BiomeGenerationAlgorithm:
                    return BiomeGenerationAlgorithm.GenerateDungeon(
                        bounds,
                        settings.selectedAlgorithm.biomeCount
                    );
                case CarvingAlgorithmType.FractalMazeAlgorithm:
                    return FractalMazeAlgorithm.GenerateDungeon(
                        bounds,
                        settings.selectedAlgorithm.fractalMazeMaxDepth,
                        settings.selectedAlgorithm.fractalMazeSplitChance
                    );
                case CarvingAlgorithmType.RadialGrowthAlgorithm:
                    return RadialGrowthAlgorithm.GenerateDungeon(
                        settings.selectedAlgorithm.radialGrowthCenter,
                        settings.selectedAlgorithm.radialGrowthRadius,
                        settings.selectedAlgorithm.radialGrowthRoomChance,
                        settings.selectedAlgorithm.radialGrowthCorridorChance
                    );

                default:
                    Debug.LogError($"Unknown algorithm type: {algorithmType}");
                    return new HashSet<Vector2Int>();
            }
        }

        #region CellularAutomata
        private static HashSet<Vector2Int> CellularAutomata(
            DungeonSettings settings,
            RectInt bounds
        )
        {
            HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
            bool[,] map = new bool[bounds.width, bounds.height];

            // Initialize the map with random walls based on initial wall density
            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    map[x, y] = Random.value < settings.selectedAlgorithm.initialWallDensity;
                }
            }

            // Perform Cellular Automata smoothing
            for (int i = 0; i < settings.selectedAlgorithm.iterations; i++)
            {
                bool[,] newMap = new bool[bounds.width, bounds.height];
                for (int x = 0; x < bounds.width; x++)
                {
                    for (int y = 0; y < bounds.height; y++)
                    {
                        int neighborCount = CountNeighbors(map, x, y, bounds);
                        newMap[x, y] =
                            neighborCount >= settings.selectedAlgorithm.neighborWallThreshold;
                    }
                }
                map = newMap;
            }

            // Convert map to floor tiles (invert the logic)
            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    if (map[x, y]) // Cells marked as 'true' (initially walls) become floors
                    {
                        floorTiles.Add(new Vector2Int(x + bounds.xMin, y + bounds.yMin));
                    }
                }
            }
            ConnectDisconnectedAreas(floorTiles, bounds);

            return floorTiles;
        }

        private static int CountNeighbors(bool[,] map, int x, int y, RectInt bounds)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue; // Skip self

                    int nx = x + dx;
                    int ny = y + dy;

                    if (
                        nx >= 0
                        && nx < bounds.width
                        && ny >= 0
                        && ny < bounds.height
                        && map[nx, ny]
                    )
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        #endregion

        #region PerlinNoise
        private static HashSet<Vector2Int> PerlinNoise(DungeonSettings settings, RectInt bounds)
        {
            HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
            float scale = settings.selectedAlgorithm.edgeBias; // Adjust scale based on settings

            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    float noiseValue = Mathf.PerlinNoise(x / scale, y / scale);
                    if (noiseValue > 0.5f) // Threshold for floor tiles
                    {
                        floorTiles.Add(new Vector2Int(x + bounds.xMin, y + bounds.yMin));
                    }
                }
            }

            return floorTiles;
        }
        #endregion

        #region BSP
        private static HashSet<Vector2Int> BSP(DungeonSettings settings, RectInt bounds)
        {
            HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
            List<RectInt> rooms = new List<RectInt>();

            // Initialize root space
            Queue<RectInt> spaces = new Queue<RectInt>();
            spaces.Enqueue(bounds);

            // Subdivide space
            for (int i = 0; i < settings.selectedAlgorithm.bspSubdivisions; i++)
            {
                int count = spaces.Count;
                for (int j = 0; j < count; j++)
                {
                    RectInt space = spaces.Dequeue();
                    if (Random.value < settings.selectedAlgorithm.bspCarveChance)
                    {
                        SplitSpace(space, spaces);
                    }
                    else
                    {
                        rooms.Add(space);
                    }
                }
            }

            // Convert rooms to floor tiles
            foreach (var room in rooms)
            {
                for (int x = room.xMin; x < room.xMax; x++)
                {
                    for (int y = room.yMin; y < room.yMax; y++)
                    {
                        floorTiles.Add(new Vector2Int(x, y));
                    }
                }
            }

            return floorTiles;
        }

        private static void SplitSpace(RectInt space, Queue<RectInt> spaces)
        {
            bool splitVertically = Random.value > 0.5f;
            if (splitVertically)
            {
                int splitX = Random.Range(space.xMin + 1, space.xMax - 1);
                spaces.Enqueue(
                    new RectInt(space.xMin, space.yMin, splitX - space.xMin, space.height)
                );
                spaces.Enqueue(new RectInt(splitX, space.yMin, space.xMax - splitX, space.height));
            }
            else
            {
                int splitY = Random.Range(space.yMin + 1, space.yMax - 1);
                spaces.Enqueue(
                    new RectInt(space.xMin, space.yMin, space.width, splitY - space.yMin)
                );
                spaces.Enqueue(new RectInt(space.xMin, splitY, space.width, space.yMax - splitY));
            }
        }
        #endregion

        #region SpiralPattern
        private static HashSet<Vector2Int> SpiralPattern(DungeonSettings settings, RectInt bounds)
        {
            HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
            Vector2Int center = new Vector2Int(
                bounds.xMin + bounds.width / 2,
                bounds.yMin + bounds.height / 2
            );
            int radius = 0;
            int step = 1;

            while (radius < Mathf.Max(bounds.width, bounds.height) / 2)
            {
                for (int angle = 0; angle < 360; angle += step)
                {
                    float rad = Mathf.Deg2Rad * angle;
                    int x = center.x + Mathf.RoundToInt(radius * Mathf.Cos(rad));
                    int y = center.y + Mathf.RoundToInt(radius * Mathf.Sin(rad));
                    Vector2Int position = new Vector2Int(x, y);

                    if (bounds.Contains(position))
                    {
                        floorTiles.Add(position);
                    }
                }
                radius++;
            }

            return floorTiles;
        }
        #endregion

        #region WaveFunctionCollapse
        private static HashSet<Vector2Int> WaveFunctionCollapse(
            DungeonSettings settings,
            RectInt bounds
        )
        {
            HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
            bool[,] tiles = new bool[bounds.width, bounds.height];

            // Initialize randomly
            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    tiles[x, y] = Random.value > 0.5f;
                }
            }

            // Apply simple collapse rules
            for (int x = 1; x < bounds.width - 1; x++)
            {
                for (int y = 1; y < bounds.height - 1; y++)
                {
                    int neighborCount = CountNeighbors(tiles, x, y, bounds);
                    tiles[x, y] = neighborCount >= 3;
                }
            }

            // Convert to floor tiles
            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    if (tiles[x, y])
                    {
                        floorTiles.Add(new Vector2Int(bounds.xMin + x, bounds.yMin + y));
                    }
                }
            }

            return floorTiles;
        }

        private static HashSet<Vector2Int> HybridAlgorithm(DungeonSettings settings, RectInt bounds)
        {
            HashSet<Vector2Int> floorTiles = CellularAutomata(settings, bounds);
            HashSet<Vector2Int> noiseTiles = PerlinNoise(settings, bounds);

            foreach (var tile in noiseTiles)
            {
                floorTiles.Add(tile);
            }

            return floorTiles;
        }
        #endregion


        #region IslandGrowthAlgorithm
        public static class IslandGrowthAlgorithm
        {
            public static HashSet<Vector2Int> GenerateDungeon(
                RectInt bounds,
                int islandCount,
                int islandSize
            )
            {
                HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
                List<Vector2Int> islandCenters = GenerateIslandCenters(bounds, islandCount);

                foreach (var center in islandCenters)
                {
                    GrowIsland(center, islandSize, bounds, floorTiles);
                }

                ConnectIslands(islandCenters, floorTiles);

                return floorTiles;
            }

            private static List<Vector2Int> GenerateIslandCenters(RectInt bounds, int count)
            {
                List<Vector2Int> centers = new List<Vector2Int>();
                for (int i = 0; i < count; i++)
                {
                    centers.Add(
                        new Vector2Int(
                            Random.Range(bounds.xMin, bounds.xMax),
                            Random.Range(bounds.yMin, bounds.yMax)
                        )
                    );
                }
                return centers;
            }

            private static void GrowIsland(
                Vector2Int center,
                int size,
                RectInt bounds,
                HashSet<Vector2Int> floorTiles
            )
            {
                Queue<Vector2Int> growthPoints = new Queue<Vector2Int>();
                growthPoints.Enqueue(center);

                for (int i = 0; i < size; i++)
                {
                    if (growthPoints.Count == 0)
                        break;

                    Vector2Int current = growthPoints.Dequeue();
                    floorTiles.Add(current);

                    foreach (var direction in Direction2D.GetAllDirections())
                    {
                        Vector2Int neighbor = current + direction;
                        if (bounds.Contains(neighbor) && !floorTiles.Contains(neighbor))
                        {
                            growthPoints.Enqueue(neighbor);
                        }
                    }
                }
            }

            private static void ConnectIslands(
                List<Vector2Int> centers,
                HashSet<Vector2Int> floorTiles
            )
            {
                for (int i = 0; i < centers.Count - 1; i++)
                {
                    Vector2Int start = centers[i];
                    Vector2Int end = centers[i + 1];

                    Vector2Int current = start;
                    while (current != end)
                    {
                        floorTiles.Add(current);
                        current = _MoveTowards(current, end, 1);
                    }
                }
            }
        }

        public static Vector2Int _MoveTowards(
            Vector2Int current,
            Vector2Int target,
            int maxDistanceDelta
        )
        {
            Vector2Int delta = target - current;
            Vector2Int step = new Vector2Int(
                Mathf.Clamp(delta.x, -maxDistanceDelta, maxDistanceDelta),
                Mathf.Clamp(delta.y, -maxDistanceDelta, maxDistanceDelta)
            );
            return current + step;
        }
        #endregion

        #region LSystemAlgorithm
        public static class LSystemAlgorithm
        {
            public static HashSet<Vector2Int> GenerateDungeon(
                Vector2Int start,
                int iterations,
                float branchChance
            )
            {
                HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
                Queue<Vector2Int> growthPoints = new Queue<Vector2Int>();

                growthPoints.Enqueue(start);
                floorTiles.Add(start);

                for (int i = 0; i < iterations; i++)
                {
                    if (growthPoints.Count == 0)
                        break;

                    int currentCount = growthPoints.Count;
                    for (int j = 0; j < currentCount; j++)
                    {
                        Vector2Int current = growthPoints.Dequeue();
                        foreach (var direction in Direction2D.GetAllDirections())
                        {
                            if (Random.value < branchChance)
                            {
                                Vector2Int next = current + direction;
                                if (!floorTiles.Contains(next))
                                {
                                    floorTiles.Add(next);
                                    growthPoints.Enqueue(next);
                                }
                            }
                        }
                    }
                }

                return floorTiles;
            }
        }
        #endregion

        #region RipplePropagationAlgorithm
        public static class RipplePropagationAlgorithm
        {
            public static HashSet<Vector2Int> GenerateDungeon(
                RectInt bounds,
                int rippleCenters,
                int maxRadius
            )
            {
                HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
                List<Vector2Int> centers = GenerateCenters(bounds, rippleCenters);

                for (int radius = 1; radius <= maxRadius; radius++)
                {
                    foreach (var center in centers)
                    {
                        foreach (var point in GetCircle(center, radius))
                        {
                            if (bounds.Contains(point))
                                floorTiles.Add(point);
                        }
                    }
                }

                return floorTiles;
            }

            private static List<Vector2Int> GenerateCenters(RectInt bounds, int count)
            {
                List<Vector2Int> centers = new List<Vector2Int>();
                for (int i = 0; i < count; i++)
                {
                    centers.Add(
                        new Vector2Int(
                            Random.Range(bounds.xMin, bounds.xMax),
                            Random.Range(bounds.yMin, bounds.yMax)
                        )
                    );
                }
                return centers;
            }

            private static List<Vector2Int> GetCircle(Vector2Int center, int radius)
            {
                List<Vector2Int> circle = new List<Vector2Int>();

                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        if (x * x + y * y <= radius * radius)
                        {
                            circle.Add(center + new Vector2Int(x, y));
                        }
                    }
                }

                return circle;
            }
        }
        #endregion

        #region BiomeGenerationAlgorithm
        public static class BiomeGenerationAlgorithm
        {
            public enum BiomeType
            {
                Lava,
                Ice,
                Overgrowth,
            }

            public static HashSet<Vector2Int> GenerateDungeon(RectInt bounds, int biomeCount)
            {
                HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
                List<RectInt> biomes = DivideIntoBiomes(bounds, biomeCount);

                foreach (var biome in biomes)
                {
                    BiomeType biomeType = (BiomeType)Random.Range(0, 3);
                    GenerateBiome(biome, biomeType, floorTiles);
                }

                return floorTiles;
            }

            private static List<RectInt> DivideIntoBiomes(RectInt bounds, int biomeCount)
            {
                List<RectInt> biomes = new List<RectInt>();
                int width = bounds.width / biomeCount;
                for (int i = 0; i < biomeCount; i++)
                {
                    biomes.Add(
                        new RectInt(bounds.xMin + i * width, bounds.yMin, width, bounds.height)
                    );
                }
                return biomes;
            }

            private static void GenerateBiome(
                RectInt bounds,
                BiomeType biomeType,
                HashSet<Vector2Int> floorTiles
            )
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        if (Random.value > 0.5f)
                        {
                            floorTiles.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        #endregion

        #region FractalMazeAlgorithm
        public static class FractalMazeAlgorithm
        {
            public static HashSet<Vector2Int> GenerateDungeon(
                RectInt bounds,
                int maxDepth,
                float splitChance
            )
            {
                HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();

                Subdivide(bounds, maxDepth, splitChance, floorTiles);
                return floorTiles;
            }

            private static void Subdivide(
                RectInt bounds,
                int depth,
                float splitChance,
                HashSet<Vector2Int> floorTiles
            )
            {
                if (depth <= 0 || Random.value > splitChance)
                {
                    // Fill the remaining bounds as a "room"
                    for (int x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        for (int y = bounds.yMin; y < bounds.yMax; y++)
                        {
                            floorTiles.Add(new Vector2Int(x, y));
                        }
                    }
                    return;
                }

                // Decide on vertical or horizontal split
                bool splitVertical = Random.value > 0.5f;

                if (splitVertical)
                {
                    int splitX = Random.Range(bounds.xMin + 1, bounds.xMax - 1);
                    RectInt left = new RectInt(
                        bounds.xMin,
                        bounds.yMin,
                        splitX - bounds.xMin,
                        bounds.height
                    );
                    RectInt right = new RectInt(
                        splitX,
                        bounds.yMin,
                        bounds.xMax - splitX,
                        bounds.height
                    );
                    Subdivide(left, depth - 1, splitChance, floorTiles);
                    Subdivide(right, depth - 1, splitChance, floorTiles);
                }
                else
                {
                    int splitY = Random.Range(bounds.yMin + 1, bounds.yMax - 1);
                    RectInt top = new RectInt(
                        bounds.xMin,
                        bounds.yMin,
                        bounds.width,
                        splitY - bounds.yMin
                    );
                    RectInt bottom = new RectInt(
                        bounds.xMin,
                        splitY,
                        bounds.width,
                        bounds.yMax - splitY
                    );
                    Subdivide(top, depth - 1, splitChance, floorTiles);
                    Subdivide(bottom, depth - 1, splitChance, floorTiles);
                }
            }
        }
        #endregion

        #region RadialGrowthAlgorithm
        public static class RadialGrowthAlgorithm
        {
            public static HashSet<Vector2Int> GenerateDungeon(
                Vector2Int center,
                int radius,
                float roomChance,
                float corridorChance
            )
            {
                HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();
                Queue<Vector2Int> growthPoints = new Queue<Vector2Int>();

                growthPoints.Enqueue(center);
                floorTiles.Add(center);

                for (int r = 1; r <= radius; r++)
                {
                    int pointsToProcess = growthPoints.Count;

                    for (int i = 0; i < pointsToProcess; i++)
                    {
                        Vector2Int current = growthPoints.Dequeue();
                        foreach (Vector2Int direction in Direction2D.GetAllDirections())
                        {
                            Vector2Int neighbor = current + direction;

                            if (
                                !floorTiles.Contains(neighbor)
                                && Random.value < (r < radius / 2 ? corridorChance : roomChance)
                            )
                            {
                                floorTiles.Add(neighbor);
                                growthPoints.Enqueue(neighbor);
                            }
                        }
                    }
                }

                return floorTiles;
            }
        }
        #endregion

        public static class Direction2D
        {
            public static List<Vector2Int> GetAllDirections()
            {
                return new List<Vector2Int>
                {
                    new Vector2Int(0, 1), // Up
                    new Vector2Int(0, -1), // Down
                    new Vector2Int(1, 0), // Right
                    new Vector2Int(
                        -1,
                        0
                    ) // Left
                    ,
                };
            }
        }

        #region Connectivity
        private static void ConnectDisconnectedAreas(HashSet<Vector2Int> floorTiles, RectInt bounds)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            List<HashSet<Vector2Int>> disconnectedAreas = new List<HashSet<Vector2Int>>();

            // Find all disconnected areas using flood fill
            foreach (var tile in floorTiles)
            {
                if (!visited.Contains(tile))
                {
                    HashSet<Vector2Int> area = new HashSet<Vector2Int>();
                    FloodFill(tile, floorTiles, visited, area);
                    disconnectedAreas.Add(area);
                }
            }

            // Connect all disconnected areas
            for (int i = 0; i < disconnectedAreas.Count - 1; i++)
            {
                Vector2Int start = GetClosestTile(disconnectedAreas[i], disconnectedAreas[i + 1]);
                Vector2Int end = GetClosestTile(disconnectedAreas[i + 1], disconnectedAreas[i]);

                ConnectTiles(start, end, floorTiles);
            }
        }

        private static void FloodFill(
            Vector2Int start,
            HashSet<Vector2Int> floorTiles,
            HashSet<Vector2Int> visited,
            HashSet<Vector2Int> area
        )
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                area.Add(current);

                foreach (var direction in Direction2D.GetAllDirections())
                {
                    Vector2Int neighbor = current + direction;
                    if (floorTiles.Contains(neighbor) && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        private static Vector2Int GetClosestTile(
            HashSet<Vector2Int> area1,
            HashSet<Vector2Int> area2
        )
        {
            Vector2Int closestTile = Vector2Int.zero;
            float minDistance = float.MaxValue;

            foreach (var tile1 in area1)
            {
                foreach (var tile2 in area2)
                {
                    float distance = Vector2.Distance(tile1, tile2);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTile = tile1;
                    }
                }
            }

            return closestTile;
        }

        private static void ConnectTiles(
            Vector2Int start,
            Vector2Int end,
            HashSet<Vector2Int> floorTiles
        )
        {
            Vector2Int current = start;
            while (current != end)
            {
                floorTiles.Add(current);
                current = MoveTowards(current, end, 1);
            }
        }

        private static Vector2Int MoveTowards(
            Vector2Int current,
            Vector2Int target,
            int maxDistanceDelta
        )
        {
            int dx = target.x - current.x;
            int dy = target.y - current.y;

            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                return new Vector2Int(
                    current.x + Mathf.Clamp(dx, -maxDistanceDelta, maxDistanceDelta),
                    current.y
                );
            }
            else
            {
                return new Vector2Int(
                    current.x,
                    current.y + Mathf.Clamp(dy, -maxDistanceDelta, maxDistanceDelta)
                );
            }
        }
        #endregion
    }
}
