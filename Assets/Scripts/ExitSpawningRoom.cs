using UnityEngine;

namespace CoED
{
    public class SpawningRoomExitTrigger : MonoBehaviour
    {
        public string warningMessage = "There is no turning back now!!!!";
        private Vector3 seededExitPosition;
        private DungeonSettings dungeonSettings;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ExitSpawningRoom(other.gameObject);
            }
        }

        private void DisplayWarning()
        {
            // Debug.Log(warningMessage);
        }

        private void ExitSpawningRoom(GameObject player)
        {

            DisplayWarning();

            // Destroy spawning room instance via DungeonManager
            if (DungeonManager.Instance.SpawningRoomInstance != null)
            {
                Destroy(DungeonManager.Instance.SpawningRoomInstance);
            }


            // Transport player using DungeonManager or DungeonSpawner
            if (DungeonManager.Instance != null)
            {
                DungeonSpawner.Instance.TransportPlayerToDungeon(player);
            }
            else
            {
                Debug.LogError("DungeonManager instance is not available.");
            }
        }
    }
}
