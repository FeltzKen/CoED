using UnityEngine;

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
                DontDestroyOnLoad(gameObject);
                Debug.Log("PlayerSpawner: Instance initialized.");
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerSpawner: Duplicate instance destroyed.");
                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("PlayerSpawner: PlayerPrefab not assigned in the Inspector.");
            }
        }

        public GameObject SpawnPlayer()
        {
            Debug.Log("PlayerSpawner: SpawnPlayer called.");

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

            GameObject spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (spawnPointObj == null)
            {
                Debug.LogError(
                    "PlayerSpawner: No GameObject with tag 'SpawnPoint' found in the scene."
                );
                return null;
            }

            Vector3 spawnPosition = spawnPointObj.transform.position;
            Debug.Log($"PlayerSpawner: Spawning player at SpawnPoint position {spawnPosition}");
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            GameManager.Instance.RegisterPlayer(currentPlayer);
            Debug.Log($"PlayerSpawner: Player spawned at {spawnPosition}");

            if (currentPlayer != null)
            {
                Debug.Log($"PlayerSpawner: Spawned player at {spawnPosition}.");

                var renderer = currentPlayer.GetComponent<Renderer>();
                if (renderer != null && renderer.enabled)
                {
                    Debug.Log("PlayerSpawner: Renderer component is active.");
                }
                else
                {
                    Debug.LogWarning("PlayerSpawner: Renderer component is missing or disabled.");
                }

                if (currentPlayer.transform.parent != null)
                {
                    Debug.Log(
                        $"PlayerSpawner: Player is parented to {currentPlayer.transform.parent.name}."
                    );
                }
                else
                {
                    Debug.Log("PlayerSpawner: Player has no parent.");
                }
            }
            else
            {
                Debug.LogError("PlayerSpawner: Failed to instantiate player prefab.");
            }

            return currentPlayer;
        }
    }
}
