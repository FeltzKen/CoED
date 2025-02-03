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

            halfHeight = mainCamera.orthographicSize;
            halfWidth = halfHeight * mainCamera.aspect;

            Transform spawningRoom = GameObject.Find("SpawningRoom")?.transform;
            if (spawningRoom != null)
            {
                Vector3 spawningRoomPosition = spawningRoom.position;
                spawningRoomPosition.z = transform.position.z;
                transform.position = spawningRoomPosition + new Vector3(0, 3, 0);

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
            if (isInSpawningRoom)
            {
                return; // Camera stays stationary until the player exits the spawning room
            }

            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform == null)
                {
                    Debug.LogWarning("Player Transform not found!");
                    return;
                }
            }

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

            float clampedX = Mathf.Clamp(
                playerPosition.x,
                minBounds.x + halfWidth + 0.5f,
                maxBounds.x - halfWidth - 0.5f
            );
            float clampedY = Mathf.Clamp(
                playerPosition.y,
                minBounds.y + halfHeight + 0.5f,
                maxBounds.y - halfHeight - 0.5f
            );

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

        private void CenterCameraOnPlayerWithoutClamping()
        {
            if (playerTransform == null)
            {
                Debug.LogWarning("Cannot center camera; playerTransform is null!");
                return;
            }

            Vector3 playerPosition = playerTransform.position;

            transform.position = new Vector3(
                playerPosition.x,
                playerPosition.y,
                transform.position.z
            );
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
                CenterCameraOnPlayerWithoutClamping();
                UpdateBounds(PlayerStats.Instance.currentFloor);
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

            Bounds floorBounds = floorData.FloorTilemap.localBounds;
            Vector3 floorOffset = floorData.FloorTilemap.transform.parent.position;

            minBounds = new Vector2(
                floorBounds.min.x + floorOffset.x,
                floorBounds.min.y + floorOffset.y
            );
            maxBounds = new Vector2(
                floorBounds.max.x + floorOffset.x,
                floorBounds.max.y + floorOffset.y
            );

            boundsClampingEnabled = true;

            Debug.Log(
                $"Camera bounds updated for Floor {floorNumber}: Min({minBounds}), Max({maxBounds})"
            );
            MinimapController.Instance.UpdateMinimapPosition(transform.position);
        }
    }
}
