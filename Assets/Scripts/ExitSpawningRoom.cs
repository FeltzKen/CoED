using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class SpawningRoomExitTrigger : MonoBehaviour
    {
        public string warningMessage = "There is no turning back now!!!";
        private Vector3 seededExitPosition;
        private DungeonGeneratorMethods methods; 
        private void OnTriggerEnter2D(Collider2D other)
        {

                DisplayWarning();
            StartCoroutine(DelayedTransportPlayer(other.gameObject, 0.2f));
        }
        private void DisplayWarning()
        {
            Debug.Log(warningMessage);
        }
        private IEnumerator DelayedTransportPlayer(GameObject player, float delay)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();

            if (playerManager != null)
            {
                Debug.Log("Waiting briefly before transport...");
                yield return new WaitForSeconds(delay); // Introduce a small delay

                // Ensure that any movement-related flags are reset
               // playerManager.isMoving = false;
               // playerManager.isActionComplete = true; // Clear any previous planned actions

                // Proceed with transporting the player
                DungeonGenerator.Instance.TransportPlayerToDungeon(player);
            }
            else
            {
                Debug.LogError("PlayerManager component not found on the player object.");
            }
        }
        
    }
}
