using UnityEngine;

namespace YourGameNamespace
{
    public class PlayerTransporter
    {
        private GridManager gridManager;

        public PlayerTransporter(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        /// <summary>
        /// Transports the player to the starting position of a specified floor.
        /// </summary>
        public void TransportPlayerToFloor(GameObject player, int floorNumber)
        {
            FloorData floor = DungeonManager.Instance.GetFloor(floorNumber);
            if (floor == null)
            {
                Debug.LogError($"Cannot transport player. Floor {floorNumber} does not exist.");
                return;
            }

            Vector2Int startTile = DungeonManager.Instance.GetRandomTileFromFloor(floorNumber);
            Vector3 worldPosition = gridManager.GridToWorldPosition(new Vector3Int(startTile.x, startTile.y, 0));

            MovePlayerToPosition(player, worldPosition);
            ActivateFloor(floorNumber);

            Debug.Log($"Player transported to floor {floorNumber} at {worldPosition}.");
        }

        /// <summary>
        /// Moves the player to a specified position and aligns it with the grid.
        /// </summary>
        private void MovePlayerToPosition(GameObject player, Vector3 position)
        {
            player.transform.position = position;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = position; // Synchronize Rigidbody position with transform
            }
        }

        /// <summary>
        /// Activates the specified floor and deactivates all other floors.
        /// </summary>
        private void ActivateFloor(int floorNumber)
        {
            foreach (var floor in DungeonManager.Instance.Floors)
            {
                bool isActive = floor.FloorNumber == floorNumber;

                Transform floorTransform = GameObject.Find($"Floor_{floor.FloorNumber}")?.transform;
                if (floorTransform != null)
                {
                    floorTransform.gameObject.SetActive(isActive);
                }
            }
        }
    }
}
