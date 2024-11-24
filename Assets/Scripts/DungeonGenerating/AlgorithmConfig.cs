using UnityEngine;
using CoED;
[System.Serializable]
public class AlgorithmConfig
{
    public CarvingAlgorithmType algorithmType;
    [Range(0f, 1f)] public float initialWallDensity;   // Cellular Automata
    [Range(1, 10)] public int iterations;             // Shared
    [Range(0, 8)] public int neighborWallThreshold;   // Cellular Automata
    [Range(1f, 10f)] public float edgeBias;           // Perlin Noise
    [Range(1, 10)] public int bspSubdivisions;        // BSP
    [Range(0f, 1f)] public float bspCarveChance;      // BSP
}


