using System.Linq;
using UnityEngine;

namespace CoED
{
    public class TransitionFloor : MonoBehaviour
    {
        public int floorChangeValue; // +1 for down, -1 for up
        private Transform player;
        private Collider2D targetCollider;

        [SerializeField]
        private Transform playerTransform;

        private void Start()
        {
            player = playerTransform;
            ;
        }

        private void Update()
        {
            if (targetCollider != null && player != null)
            {
                targetCollider.enabled = false;

                Vector2 playerPosition = new Vector2(player.position.x, player.position.y);
                Vector2 targetPosition = new Vector2(
                    targetCollider.transform.position.x,
                    targetCollider.transform.position.y
                );
                Debug.Log(
                    $"Player Xposition: {playerPosition.x}, Player Yposition: {playerPosition.y}, Target Xposition: {targetPosition.x}, Target Yposition: {targetPosition.y}"
                );

                if (playerPosition != targetPosition)
                {
                    targetCollider.enabled = true;
                    targetCollider = null;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                int currentFloor = PlayerStats.Instance.currentFloor;
                int newFloor = currentFloor + floorChangeValue;
                PlayerStats.Instance.currentFloor = newFloor;

                FloorData floorData = DungeonManager.Instance.GetFloorData(newFloor);
                if (floorData == null)
                {
                    Debug.LogError($"No floor data found for Floor {newFloor}");
                    return;
                }

                Vector3 triggeringStairsWorldPosition = transform.position;

                Transform targetFloorTransform = GameObject.Find($"Floor_{newFloor}")?.transform;
                if (targetFloorTransform == null)
                {
                    Debug.LogError($"Target floor transform for Floor_{newFloor} not found.");
                    return;
                }

                Vector3 triggeringStairsLocalPosition =
                    triggeringStairsWorldPosition - transform.parent.position;
                Vector3 targetWorldPosition =
                    triggeringStairsLocalPosition
                    + targetFloorTransform.position
                    + new Vector3(-0.5f, -0.5f, 0);

                Vector3Int targetCellPosition = floorData.FloorTilemap.WorldToCell(
                    targetWorldPosition
                );

                Vector2Int[] directions = new Vector2Int[]
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right,
                };

                Vector3Int adjacentTile = targetCellPosition;

                Vector2Int[] randomizedDirections = directions.OrderBy(_ => Random.value).ToArray();

                foreach (Vector2Int direction in randomizedDirections)
                {
                    Vector3Int checkPosition =
                        targetCellPosition + new Vector3Int(direction.x, direction.y, 0);

                    if (
                        floorData.FloorTiles.Contains(
                            new Vector2Int(checkPosition.x, checkPosition.y)
                        )
                    )
                    {
                        adjacentTile = checkPosition;
                        break;
                    }
                }

                Vector3 adjacentWorldPosition =
                    floorData.FloorTilemap.CellToWorld(adjacentTile) + new Vector3(+1.5f, +1.5f, 0);

                PlayerMovement.Instance.UpdateCurrentTilePosition(adjacentWorldPosition);
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                if (cameraController != null)
                {
                    cameraController.UpdateBounds(PlayerStats.Instance.GetCurrentFloor());
                }
            }
        }
    }
}
