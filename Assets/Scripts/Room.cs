using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

public class Room
{
    [SerializeField]
    private Rect rect;

    [SerializeField]
    private Vector2 center;

    [SerializeField]
    private HashSet<Vector2Int> floorTiles;

    [SerializeField]
    private List<Vector2Int> carvedTiles;

    public Rect Rect => rect;
    public Vector2Int Center { get; private set; }
    public HashSet<Vector2Int> FloorTiles => floorTiles;
    public List<Vector2Int> CarvedTiles => carvedTiles;

    public Room(Rect roomRect, HashSet<Vector2Int> floorTiles)
    {
        this.rect = roomRect;
        this.center = rect.center;
        this.floorTiles = floorTiles;
        this.carvedTiles = new List<Vector2Int>();
    }
}
