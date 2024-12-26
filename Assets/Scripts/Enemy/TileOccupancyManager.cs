using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class TileOccupancyManager : MonoBehaviour
    {
        public static TileOccupancyManager Instance { get; private set; }

        /// <summary>
        /// Maps a tile position to the occupant ID that has reserved it.
        /// If a tile is free, it won't appear in the dictionary at all.
        /// </summary>
        private Dictionary<Vector2Int, int> tileReservations = new Dictionary<Vector2Int, int>();

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

        /// <summary>
        /// Attempts to reserve the given tile for occupantID.
        /// Returns true if successful (tile was free or already owned by occupantID).
        /// Returns false if another occupant currently holds it.
        /// </summary>
        public bool TryReserveTile(Vector2Int tilePosition, int occupantID)
        {
            // If tile not in dictionary => free, or occupantID already owns it
            if (
                !tileReservations.ContainsKey(tilePosition)
                || tileReservations[tilePosition] == occupantID
            )
            {
                tileReservations[tilePosition] = occupantID;
                return true;
            }

            // If someone else owns it, we fail.
            return false;
        }

        /// <summary>
        /// Releases the tile if occupantID currently owns it.
        /// </summary>
        public void ReleaseTile(Vector2Int tilePosition, int occupantID)
        {
            if (tileReservations.ContainsKey(tilePosition))
            {
                // Only the occupant who reserved the tile can release it
                if (tileReservations[tilePosition] == occupantID)
                {
                    tileReservations.Remove(tilePosition);
                }
            }
        }

        public bool IsTileOccupiedByPlayer(Vector2Int tilePosition)
        {
            return new Vector3(tilePosition.x, tilePosition.y, 0)
                == PlayerMovement.Instance.GetPlayerPosition();
        }

        /// <summary>
        /// Returns the occupant ID that currently holds tilePosition,
        /// or -1 if it's free / not in the dictionary.
        /// </summary>
        public int GetOccupantOfTile(Vector2Int tilePosition)
        {
            if (tileReservations.TryGetValue(tilePosition, out int occupantID))
            {
                return occupantID;
            }
            return -1;
        }
    }
}
