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
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerSpawner: Duplicate instance destroyed.");
                return;
            }
        }

        public GameObject SpawnPlayer()
        {
            GameObject spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
            Vector3 spawnPosition = spawnPointObj.transform.position;
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            GameManager.Instance.RegisterPlayer(currentPlayer);

            return currentPlayer;
        }
    }
}
