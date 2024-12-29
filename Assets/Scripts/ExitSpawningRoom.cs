using UnityEngine;

namespace CoED
{
    public class SpawningRoomExitTrigger : MonoBehaviour
    {
        public string warningMessage = "There is no turning back now!!!!";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStats.Instance.HasEnteredDungeon = true;
                ExitSpawningRoom(other.transform);
            }
        }

        private void DisplayWarning()
        {
            FloatingTextManager.Instance.ShowFloatingText(
                warningMessage,
                PlayerMovement.Instance.transform,
                Color.red
            );
        }

        private void ExitSpawningRoom(Transform player)
        {
            DisplayWarning();

            Destroy(DungeonManager.Instance.SpawningRoomInstance);

            if (DungeonManager.Instance != null)
            {
                DungeonSpawner.Instance.TransportPlayerToDungeon(player);
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                if (cameraController != null)
                {
                    cameraController.ExitSpawningRoom();
                }
            }
            else
            {
                Debug.LogError("DungeonManager instance is not available.");
            }
        }
    }
}
