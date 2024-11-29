using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoED
{
    [System.Serializable]
    public class TilePalette
    {
        [Header("Tile Base Settings")]
        [Tooltip("Tiles to use for floor generation.")]
        public Tile[] floorTiles; // Tiles for the floor
        
        [Tooltip("Tiles to use for wall generation.")]
        public Tile[] wallTiles;  // Tiles for the walls

        [Tooltip("Tiles to use for wall generation.")]
        public Tile[] voidTiles;  // Tiles for the walls

        [Header("Stair Tile Settings")]
        [Tooltip("Tile to use for stairs going up.")]
        public GameObject stairsUpTile;  // Tile for stairs going up

        [Tooltip("Tile to use for stairs going down.")]
        public GameObject stairsDownTile; // Tile for stairs going down
    }
}