using UnityEngine;
using CoED;

namespace CoED
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance { get; private set; }

        [Header("Player Settings")]
        [SerializeField]
        private GameObject playerPrefab;

        private GameObject currentPlayer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerSpawner instance already exists. Destroying duplicate.");
                return;
            }
        }

        public GameObject SpawnPlayer()
        {
            if (currentPlayer != null)
            {
                Debug.LogWarning("PlayerSpawner: Player already exists. Skipping spawn.");
                return currentPlayer;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("PlayerSpawner: PlayerPrefab not assigned.");
                return null;
            }

            // Define spawn point within spawning room at (0, 0)
            Vector3 spawnPosition = new Vector3(-10.0f, -10.0f, 0); // Center of the spawning room
            spawnPosition += new Vector3(0, 0, 0); // Adjust if needed
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"PlayerSpawner: Spawned player at {spawnPosition}.");



            return currentPlayer;
        }
    }
}
