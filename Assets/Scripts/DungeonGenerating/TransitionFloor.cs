using System;
using System.Linq;
using CoED.Pathfinding;
using UnityEngine;

namespace CoED
{
    public class TransitionFloor : MonoBehaviour
    {
        public int floorChangeValue; // +1 for down, -1 for up

        private Transform player;

        private PlayerNavigator playerNavigator;
        private PlayerMovement playerMovement;

        private void Start()
        {
            playerNavigator = PlayerStats.Instance.GetComponent<PlayerNavigator>();
            playerMovement = PlayerStats.Instance.GetComponent<PlayerMovement>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            player = PlayerStats.Instance.transform;
            if (!other.CompareTag("Player"))
                return;

            int currentFloor = PlayerStats.Instance.currentFloor;
            int newFloor = currentFloor + floorChangeValue;

            PlayerStats.Instance.currentFloor = newFloor;

            FloorData floorData = DungeonManager.Instance.GetFloorData(newFloor);
            player.GetComponent<PlayerNavigator>().SetTilemap(floorData.FloorTilemap);
            if (floorData == null)
            {
                Debug.LogError($"No FloorData found for Floor {newFloor}");
                return;
            }

            Transform targetFloorTransform = DungeonManager.Instance.GetFloorTransform(newFloor);
            if (targetFloorTransform == null)
            {
                Debug.LogError($"Target floor transform for Floor_{newFloor} not found.");
                return;
            }

            Vector3 triggeringStairsLocalPosition = transform.localPosition;
            Vector3 targetWorldPosition =
                triggeringStairsLocalPosition + targetFloorTransform.position;

            Vector3Int targetCellPosition = floorData.FloorTilemap.WorldToCell(targetWorldPosition);

            Vector3Int adjacentTile = FindAdjacentWalkableTile(floorData, targetCellPosition);

            Vector3 adjacentWorldPosition =
                floorData.FloorTilemap.CellToWorld(adjacentTile) + new Vector3(0.5f, 0.5f, 0);

            player.GetComponent<PlayerMovement>().UpdateCurrentTilePosition(adjacentWorldPosition);

            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.UpdateBounds(PlayerStats.Instance.currentFloor);
            }
        }

        private Vector3Int FindAdjacentWalkableTile(FloorData floorData, Vector3Int targetCell)
        {
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right,
            };

            foreach (Vector2Int direction in directions.OrderBy(_ => UnityEngine.Random.value))
            {
                Vector3Int checkPosition = targetCell + new Vector3Int(direction.x, direction.y, 0);
                if (floorData.FloorTiles.Contains((Vector2Int)checkPosition))
                {
                    return checkPosition;
                }
            }

            return targetCell; // Default to the original tile if no adjacent tile is found
        }
    }
}
