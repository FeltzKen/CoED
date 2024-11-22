using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class SpawningRoomExitTrigger : MonoBehaviour
    {
        public string warningMessage = "There is no turning back now!!!!";
        private Vector3 seededExitPosition;
        private DungeonGeneratorMethods methods;



private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        ExitSpawningRoom(other.gameObject);
    }
}

        private void DisplayWarning()
        {
            Debug.Log(warningMessage);
        }

        private void ExitSpawningRoom(GameObject player)
        {            

            
            if (gameObject != null)
            {
                Destroy(DungeonGenerator.Instance.spawningRoomInstance);
            }

            DungeonGenerator.Instance.TransportPlayerToDungeon(player);
        }
    }
}
