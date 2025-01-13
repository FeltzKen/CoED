using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class TileOccupancyManager : MonoBehaviour
    {
        public static TileOccupancyManager Instance { get; private set; }

        private Dictionary<Vector2Int, int> tileReservations = new Dictionary<Vector2Int, int>();

        public static readonly Vector2Int[] adjacentOffsets =
        {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0), // Right
            new Vector2Int(0, 1), // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 1), // Up-Left
            new Vector2Int(1, 1), // Up-Right
            new Vector2Int(-1, -1), // Down-Left
            new Vector2Int(1, -1), // Down-Right
        };

        private Vector2Int playerPosition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetPlayerPosition(Vector2Int position)
        {
            playerPosition = position;
        }

        public bool IsPlayerSurroundedByEnemies()
        {
            foreach (var offset in adjacentOffsets)
            {
                Vector2Int checkPos = playerPosition + offset;
                if (IsTileFree(checkPos)) // If any adjacent tile is free, the player is not surrounded
                    return false;
            }
            Debug.Log("Player is surrounded by enemies!");
            return true;
        }

        public bool TryReserveTile(Vector2Int tilePosition, int occupantID)
        {
            if (IsTileFree(tilePosition))
            {
                tileReservations.Add(tilePosition, occupantID);
                return true;
            }
            return false;
        }

        public void ReleaseTile(Vector2Int tilePosition, int occupantID)
        {
            if (
                tileReservations.TryGetValue(tilePosition, out int currentOccupantID)
                && currentOccupantID == occupantID
            )
            {
                tileReservations.Remove(tilePosition);
            }
        }

        public void ReleaseAllTiles(int occupantID)
        {
            List<Vector2Int> tilesToRelease = new List<Vector2Int>();
            foreach (var kvp in tileReservations)
            {
                if (kvp.Value == occupantID)
                {
                    tilesToRelease.Add(kvp.Key);
                }
            }

            foreach (var tile in tilesToRelease)
            {
                tileReservations.Remove(tile);
            }
        }

        public bool IsTileFree(Vector2Int tilePosition)
        {
            return !tileReservations.ContainsKey(tilePosition);
        }

        public bool IsAnyAdjacentTileFree(Vector2Int playerTilePos)
        {
            foreach (var offset in adjacentOffsets)
            {
                Vector2Int checkPos = playerTilePos + offset;
                if (IsTileFree(checkPos))
                    return true;
            }
            return false;
        }
    }
}
