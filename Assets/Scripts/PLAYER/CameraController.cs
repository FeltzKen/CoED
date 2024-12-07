using UnityEngine;

namespace CoED
{
    public class CameraController : MonoBehaviour
    {
        private Transform playerTransform;

        private Vector2 minBounds = Vector2.zero;
        private Vector2 maxBounds = Vector2.zero;
        private float halfHeight;
        private float halfWidth;
        private Camera mainCamera;
        private bool isInSpawningRoom = true;
        private bool boundsClampingEnabled = true;

        void Start()
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found!");
                return;
            }

            // Calculate camera size
            halfHeight = mainCamera.orthographicSize;
            halfWidth = halfHeight * mainCamera.aspect;

            // Position the camera over the spawning room initially
            Transform spawningRoom = GameObject.Find("SpawningRoom")?.transform;
            if (spawningRoom != null)
            {
                Vector3 spawningRoomPosition = spawningRoom.position;
                spawningRoomPosition.z = transform.position.z;
                transform.position = spawningRoomPosition;

                Debug.Log("Camera positioned above the spawning room.");
            }
            else
            {
                Debug.LogWarning("Spawning room not found! Defaulting to player position.");
                CenterCameraOnPlayerWithoutClamping();
            }
        }

        void LateUpdate()
        {
            // Handle spawning room behavior
            if (isInSpawningRoom)
            {
                return; // Camera stays stationary until the player exits the spawning room
            }

            // Dynamically find player if not already assigned
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform == null)
                {
                    Debug.LogWarning("Player Transform not found!");
                    return;
                }
            }

            // Follow the player with or without clamping
            if (boundsClampingEnabled)
            {
                CenterCameraOnPlayer();
            }
            else
            {
                CenterCameraOnPlayerWithoutClamping();
            }
        }

        private void CenterCameraOnPlayer()
        {
            if (playerTransform == null)
            {
                Debug.LogWarning("Cannot center camera; playerTransform is null!");
                return;
            }

            Vector3 playerPosition = playerTransform.position;

            // Clamp the player's position to the bounds of the floor
            float clampedX = Mathf.Clamp(playerPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            float clampedY = Mathf.Clamp(playerPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

            // Update the camera's position
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);

           // Debug.Log($"Camera centered on player at ({clampedX}, {clampedY}) with clamping.");
        }

        private void CenterCameraOnPlayerWithoutClamping()
        {
            if (playerTransform == null)
            {
                Debug.LogWarning("Cannot center camera; playerTransform is null!");
                return;
            }

            Vector3 playerPosition = playerTransform.position;

            // Directly update the camera's position without clamping
            transform.position = new Vector3(playerPosition.x, playerPosition.y, transform.position.z);

           // Debug.Log($"Camera centered on player at ({playerPosition.x}, {playerPosition.y}) without clamping.");
        }

        public void ExitSpawningRoom()
        {
            isInSpawningRoom = false;

            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            }

            if (playerTransform != null)
            {
                // Immediately transition to the player position and enable bounds clamping
                CenterCameraOnPlayerWithoutClamping();
                UpdateBounds(PlayerStats.Instance.GetCurrentFloor());
                boundsClampingEnabled = true;

                Debug.Log("Camera exited spawning room and is now following the player.");
            }
            else
            {
                Debug.LogWarning("Player Transform not found when exiting spawning room.");
            }
        }

        public void UpdateBounds(int floorNumber)
        {
            FloorData floorData = DungeonManager.Instance.GetFloorData(floorNumber);
            if (floorData == null)
            {
                Debug.LogError($"No floor data found for Floor {floorNumber}");
                return;
            }

            // Update bounds based on the floor's tilemap and parent offset
            Bounds floorBounds = floorData.FloorTilemap.localBounds;
            Vector3 floorOffset = floorData.FloorTilemap.transform.parent.position;

            minBounds = new Vector2(floorBounds.min.x + floorOffset.x, floorBounds.min.y + floorOffset.y);
            maxBounds = new Vector2(floorBounds.max.x + floorOffset.x, floorBounds.max.y + floorOffset.y);

            boundsClampingEnabled = true;

            Debug.Log($"Camera bounds updated for Floor {floorNumber}: Min({minBounds}), Max({maxBounds})");
            MinimapController.Instance.UpdateMinimapPosition(transform.position);
        }
    }
}
