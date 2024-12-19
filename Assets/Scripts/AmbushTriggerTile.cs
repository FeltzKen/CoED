using UnityEngine;

namespace CoED
{
    public class AmbushTriggerTile : MonoBehaviour
    {
        public int floorNumber;
        public DungeonSettings dungeonSettings;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered the trigger tile! Spawning ambush...");
                Vector3 spawnLocation = transform.position;
                int currentFloor = PlayerStats.Instance.currentFloor;
                DungeonSpawner.Instance.SpawnAmbush(
                    spawnLocation,
                    DungeonManager.Instance.GetFloorData(currentFloor),
                    DungeonManager.Instance.GetFloorTransform(currentFloor).Find("EnemyParent")
                );
                Destroy(gameObject);
            }
        }
    }
}
