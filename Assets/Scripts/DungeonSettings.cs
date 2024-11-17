using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DungeonSettings", menuName = "GameSettings/DungeonSettings")]
public class DungeonSettings : ScriptableObject
{
    [Header("Dungeon Settings")]
    public Vector2Int dungeonSizeRange = new Vector2Int(200, 200);
    public int maxFloors = 5;
    public int seed;

    [Header("Room Settings")]
    public Vector2Int roomSizeRange = new Vector2Int(15, 12);
    public int maxBSPIterations = 6;
    public int carvingIterationsMin = 2;
    public int carvingIterationsMax = 5;

    [Header("Corridor Settings")]
    [Range(0.0f, 1.0f)] public float connectionChance = 0.13f;
    [Range(1, 3)] public int minCorridorWidth = 2;
    [Range(2, 4)] public int maxCorridorWidth = 3;
    public int maxSegmentLength = 5;
    public float straightCorridorChance = 0.3f;

    [Header("Prefabs")]
    public GameObject spawningRoomPrefab;
    public GameObject stairsUpPrefab;
    public GameObject stairsDownPrefab;
    public GameObject strongEnemyPrefab; // Add this field
    public GameObject spawnEffectPrefab; // Add this field

    [Header("Tile Settings")]
    public TileBase floorTile;
    public TileBase corridorTile;
    public TileBase wallTile;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int numberOfEnemiesPerFloor = 5;
}
