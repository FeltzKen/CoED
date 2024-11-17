using System.Collections;
using UnityEngine;

namespace YourGameNamespace
{
    public class SpawningRoomExitTrigger : MonoBehaviour
    {
        public string warningMessage = "There is no turning back now!!!";
        public float transportDelay = 0.2f;

        private PlayerTransporter playerTransporter;

        private void Start()
        {
            // Initialize PlayerTransporter using the existing GridManager
            if (DungeonManager.Instance != null && DungeonManager.Instance.gameObject.TryGetComponent(out GridManager gridManager))
            {
                playerTransporter = new PlayerTransporter(gridManager);
            }
            else
            {
                Debug.LogError("DungeonManager or GridManager not found! PlayerTransporter cannot be initialized.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) // Ensure only the player triggers this
            {
                DisplayWarning();
                StartCoroutine(DelayedTransportPlayer(other.gameObject, transportDelay));
            }
        }

        private void DisplayWarning()
        {
            Debug.Log(warningMessage);
        }

        private IEnumerator DelayedTransportPlayer(GameObject player, float delay)
        {
            if (playerTransporter == null)
            {
                Debug.LogError("PlayerTransporter is not initialized. Cannot transport the player.");
                yield break;
            }

            Debug.Log("Waiting briefly before transport...");
            yield return new WaitForSeconds(delay); // Introduce a small delay

            // Transport the player to the first dungeon floor
            playerTransporter.TransportPlayerToFloor(player, 1); // Always transport to Floor 1
        }
    }
}
