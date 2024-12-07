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
                // need to call spawn ambush from DungeonSpawner with these parameters (int enemyCount, float radius, Vector3 center, FloorData floorData, Transform enemyParent)
                DungeonSpawner.Instance.SpawnAmbush(spawnLocation, DungeonManager.Instance.GetFloorData(currentFloor), DungeonManager.Instance.GetFloorTransform(currentFloor));
                Destroy(gameObject);
            }
        }
    }
}