using UnityEngine;

namespace CoED
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private Transform playerTransform;
        public bool IsPlayerSpawned => playerTransform != null;
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
            }
        }

        private void Start()
        {
            if (PlayerSpawner.Instance != null)
            {
                GameObject player = PlayerSpawner.Instance.SpawnPlayer();
                if (player != null)
                {
                    currentPlayer = player;
                }
                else
                {
                    Debug.LogError("GameManager: Failed to spawn player.");
                }
            }
            else
            {
                Debug.LogError("GameManager: PlayerSpawner instance is missing.");
            }
        }

        public void RegisterPlayer(GameObject player)
        {
            playerTransform = player.transform;
        }

        public Transform GetPlayerTransform()
        {
            return playerTransform;
        }

        public void OnPlayerDeath()
        {
            PlayerUI.Instance?.ShowDeathPanel();
            Time.timeScale = 0f;
            Debug.Log("GameManager: Player has died. Game paused.");
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
            Debug.Log("GameManager: Quitting game...");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public bool ShouldSpawnSuperGoblin()
        {
            float baseSuperGoblinChance = 0.01f;
            float spawnChance =
                baseSuperGoblinChance
                * DungeonManager.Instance.GetComponent<DungeonSettings>().difficultyLevel;
            bool shouldSpawn = Random.value < spawnChance;
            Debug.Log(
                $"GameManager: Super Goblin spawn chance: {spawnChance * 100}% - Should Spawn: {shouldSpawn}"
            );
            return shouldSpawn;
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public int level;
        public int playerHealth;
        public Vector3 playerPosition;
        public int experience;
        public int playerLevel;
    }
}
