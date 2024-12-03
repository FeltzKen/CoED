using UnityEngine;

namespace CoED
{
    public class AmbushTriggerTile : MonoBehaviour
    {
        public int floorNumber;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered the trigger tile! Spawning ambush...");
                Vector3 spawnLocation = transform.position;
                int currentFloor = DungeonManager.Instance.GetCurrentFloor();
                DungeonSpawner.Instance.SpawnAmbush(spawnLocation, floorNumber);
              //  Destroy(gameObject);
            }
        }
    }
}