using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class InstantiateDoors : MonoBehaviour
    {
        // Singleton instance for easy access across the project
        public static InstantiateDoors Instance { get; private set; }

        [Header("Door Settings")]
        [SerializeField]
        private GameObject doorPrefab; // Prefab for door to be instantiated

        private void Awake()
        {
            // Implement Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist across scenes if needed
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("InstantiateDoors instance already exists. Destroying duplicate.");
            }
        }

        // Creates doors between rooms based on floor data
        public void CreateDoors(FloorData floor, Transform doorsParent)
        {
            // Check if the floor data is valid
            if (floor == null)
            {
                Debug.LogError("InstantiateDoors: FloorData is null.");
                return;
            }

            // Check if door prefab is assigned
            if (doorPrefab == null)
            {
                Debug.LogError("InstantiateDoors: Door prefab is not assigned.");
                return;
            }

            // Check if doors parent transform is assigned
            if (doorsParent == null)
            {
                Debug.LogError("InstantiateDoors: Doors parent transform is not assigned.");
                return;
            }

            // Loop through each room connection on this floor to place doors
            foreach (var connection in floor.Connections)
            {
                Room roomA = connection.Item1;
                Room roomB = connection.Item2;

                // Calculate the midpoint between two connected rooms as door position
                Vector2 doorPos = (roomA.Center + roomB.Center) / 2;
                Instantiate(
                    doorPrefab,
                    new Vector3(doorPos.x, doorPos.y, 0),
                    Quaternion.identity,
                    doorsParent
                );
            }

            Debug.Log(
                $"InstantiateDoors: Created {floor.Connections.Count} doors for Floor {floor.FloorNumber}."
            );
        }
    }
}
