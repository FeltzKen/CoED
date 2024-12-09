using UnityEngine;

namespace CoED
{
    public class TransitionFloor : MonoBehaviour
    {
        public int floorChangeValue; // +1 for down, -1 for up
        private Transform dungeonParent;
        private Transform player;
        private Collider2D targetCollider;

        [SerializeField]
        private Transform playerTransform;

        private void Start()
        {
            // Dynamically find dungeon parent and player
            dungeonParent = GameObject.Find("DungeonParent").transform;
            player = playerTransform;
            ;
        }

        private void Update()
        {
            if (targetCollider != null && player != null)
            {
                targetCollider.enabled = false;

                // Compare player and target positions
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
                // Transition logic
                int currentFloor = PlayerStats.Instance.currentFloor;
                int newFloor = currentFloor + floorChangeValue;
                PlayerStats.Instance.currentFloor = newFloor;

                // Get the target floor's data
                FloorData floorData = DungeonManager.Instance.GetFloorData(newFloor);
                if (floorData == null)
                {
                    Debug.LogError($"No floor data found for Floor {newFloor}");
                    return;
                }

                // Get the world position of the triggering stairs
                Vector3 triggeringStairsWorldPosition = transform.position;

                // Get the target floor's parent transform
                Transform targetFloorTransform = GameObject.Find($"Floor_{newFloor}")?.transform;
                if (targetFloorTransform == null)
                {
                    Debug.LogError($"Target floor transform for Floor_{newFloor} not found.");
                    return;
                }

                // Calculate the new position
                Vector3 triggeringStairsLocalPosition =
                    triggeringStairsWorldPosition - transform.parent.position;
                Vector3 targetWorldPosition =
                    triggeringStairsLocalPosition
                    + targetFloorTransform.position
                    + new Vector3(-0.5f, -0.5f, 0);

                Vector3Int targetCellPosition = floorData.FloorTilemap.WorldToCell(
                    targetWorldPosition
                );

                // Define directions for adjacent tiles
                Vector2Int[] directions = new Vector2Int[]
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right,
                };

                // Find a valid adjacent tile
                Vector3Int adjacentTile = targetCellPosition;
                foreach (Vector2Int direction in directions)
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

                // Convert the adjacent tile to world space
                Vector3 adjacentWorldPosition =
                    floorData.FloorTilemap.CellToWorld(adjacentTile) + new Vector3(-0.5f, -0.5f, 0);
                // Update the player's position
                PlayerMovement.Instance.UpdateCurrentTilePosition(adjacentWorldPosition);
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                if (cameraController != null)
                {
                    cameraController.UpdateBounds(PlayerStats.Instance.GetCurrentFloor());
                }

                Debug.Log(
                    $"Trigger stairs world position: {triggeringStairsWorldPosition}, Target Parent position: {targetFloorTransform.position}, Target world position: {targetWorldPosition}"
                );
            }
        }
    }
}
