//CarvingAlgorithmType.cs
using System;
using CoED;
namespace CoED
{
    /// <summary>
    /// Defines the available carving algorithms for dungeon generation.
    /// </summary>
    public enum CarvingAlgorithmType
    {
        CellularAutomata,
        PerlinNoise,
        BSP,
        SpiralPattern,
        WaveFunctionCollapse,
        HybridAlgorithm,
        IslandGrowthAlgorithm,
        LSystemAlgorithm,
        RipplePropagationAlgorithm,
        BiomeGenerationAlgorithm,
        FractalMazeAlgorithm,
        RadialGrowthAlgorithm
    }
}
