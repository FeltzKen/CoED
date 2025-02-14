using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class MiniMapController : MonoBehaviour
    {
        public static MiniMapController Instance { get; private set; }

        [Header("References")]
        [Tooltip("The mini‑map camera that renders the mini‑map view.")]
        public Camera miniMapCamera;

        [Tooltip("The UI RawImage that displays the mini‑map RenderTexture.")]
        public RawImage miniMapDisplay;

        [Tooltip(
            "(Optional) The UI panel that contains fog‐of‐war icons (if using FogOfWarManager)."
        )]
        public RectTransform miniMapFogPanel;

        [Tooltip("The player transform to follow.")]
        public Transform playerTransform;

        [Header("Settings")]
        [Tooltip("How quickly the mini‑map camera follows the player.")]
        public float followSpeed = 5f;

        // If needed, you can add additional settings (such as zoom, toggle options, etc.)

        private void Awake()
        {
            Instance = this;
        }

        private void LateUpdate()
        {
            FollowPlayer();
        }

        /// <summary>
        /// Moves the mini‑map camera smoothly to follow the player.
        /// </summary>
        private void FollowPlayer()
        {
            if (playerTransform == null || miniMapCamera == null)
                return;

            // Get the target position from the player's x/y (keep z the same for the camera)
            Vector3 targetPos = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                miniMapCamera.transform.position.z
            );
            miniMapCamera.transform.position = Vector3.Lerp(
                miniMapCamera.transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );
        }

        public void AdjustMiniMapView(FloorData floorData)
        {
            if (miniMapCamera == null || floorData == null)
                return;

            // Center the minimap camera on the floor
            Bounds floorBounds = floorData.FloorTilemap.localBounds;
            Vector3 floorCenter = floorBounds.center;

            miniMapCamera.transform.position = new Vector3(
                floorCenter.x,
                floorCenter.y,
                miniMapCamera.transform.position.z
            );

            Debug.Log($"MiniMap adjusted: Center({floorCenter})");
        }

        public void UpdateMiniMapPosition(Vector3 newPosition)
        {
            if (miniMapCamera != null)
            {
                miniMapCamera.transform.position = new Vector3(
                    newPosition.x,
                    newPosition.y,
                    miniMapCamera.transform.position.z
                );
            }
        }

        /// <summary>
        /// Optionally toggle the mini‑map display on or off.
        /// </summary>
        public void ShowMiniMap(bool show)
        {
            if (miniMapDisplay != null)
            {
                miniMapDisplay.enabled = show;
            }
            if (miniMapFogPanel != null)
            {
                miniMapFogPanel.gameObject.SetActive(show);
            }
        }
    }
}
