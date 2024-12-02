using UnityEngine;
using CoED;
using System.Collections;
using UnityEngine.Tilemaps;
//using System.Numerics;
namespace CoED
{
    public class TransitionFloor : MonoBehaviour
    {
        public int floorChangeValue; // +1 for down, -1 for up
        private Transform dungeonParent;
        private Transform player;
        public int StairID;
        private Collider2D targetCollider;

        private void Start()
        {
            // Find the dungeon parent dynamically
            dungeonParent = GameObject.Find("DungeonParent").transform;
            player = PlayerMovement.Instance.transform;
        }

        private void Update()
        {
            if (targetCollider != null && player != null)
            {
                Vector2 playerPosition = new Vector2(player.position.x, player.position.y);
                Vector2 targetPosition = new Vector2(targetCollider.transform.position.x, targetCollider.transform.position.y);

                if (playerPosition != targetPosition)
                {
                    targetCollider.enabled = true;
                    targetCollider = null;
                    // Debug.Log("Re-enabled collider for stairs.");
                }
            }
        }

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        // Transition logic
        int currentFloor = PlayerStats.Instance.GetCurrentFloor();
        int newFloor = currentFloor + floorChangeValue;
        PlayerStats.Instance.currentFloor = newFloor;

        // Find the target floor's data
        FloorData floorData = DungeonManager.Instance.GetFloorData(newFloor);
        if (floorData == null)
        {
            Debug.LogError($"No floor data found for Floor {newFloor}");
            return;
        }

        // Determine the stairs position
        Vector2Int stairsPosition = Vector2Int.zero;
        bool stairsFound = false;

        foreach (var stairTile in floorData.StairTiles)
        {
            if ((floorChangeValue > 0 && stairTile == Vector2Int.zero) ||
                (floorChangeValue < 0 && stairTile == Vector2Int.zero))
            {
                stairsPosition = stairTile;
                stairsFound = true;
                break;
            }
        }

        if (!stairsFound)
        {
            Debug.LogError("Target stairs not found.");
            return;
        }

        // Find a valid adjacent tile to the stairs
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        Vector2Int newTilePosition = stairsPosition;

        foreach (Vector2Int direction in directions)
        {
            Vector2Int adjacentTilePosition = stairsPosition + direction;
            if (floorData.FloorTiles.Contains(adjacentTilePosition))
            {
                newTilePosition = adjacentTilePosition;
                break;
            }
        }

        // Update the player's position to the new valid tile next to the stairs
        Vector3 newPosition = floorData.FloorTilemap.CellToWorld(new Vector3Int(newTilePosition.x, newTilePosition.y, 0)) + new Vector3(-0.5f, -0.5f, 0);
        PlayerMovement.Instance.UpdateCurrentTilePosition(newPosition);

        // Debug.Log($"Player transitioned to Floor {newFloor}, Position: {player.position}");
    }
}

    }
}
