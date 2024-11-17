using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player Settings")]
        [SerializeField]
        private GameObject currentPlayer;
 
        [Header("Game Settings")]
        [SerializeField, Min(1)]
        private int difficultyLevel = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("GameManager instance already exists. Destroying duplicate.");
                return;
            }
        }

        private void Start()
        {
            // Only spawn player once at the beginning of the game
            if (PlayerSpawner.Instance != null)
            {
                PlayerSpawner.Instance.SpawnPlayer();
            }
            else
            {
                Debug.LogError("GameManager: PlayerSpawner instance or spawnPoint is missing.");
            }
        }

        public void OnPlayerDeath()
        {
         //   UIManager.Instance?.ShowDeathPanel();
            Time.timeScale = 0f; // Pause the game
            Debug.Log("GameManager: Player has died. Game paused.");
        }

        public void QuitGame()
        {
            Time.timeScale = 1f; // Ensure the game is unpaused
            Debug.Log("GameManager: Quitting game...");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public float GetSpawnMultiplier()
        {
            return 1f + (difficultyLevel * 0.1f);
        }

        public bool ShouldSpawnSuperGoblin()
        {
            float baseSuperGoblinChance = 0.01f;
            float spawnChance = baseSuperGoblinChance * difficultyLevel;
            bool shouldSpawn = Random.value < spawnChance;
            Debug.Log($"GameManager: Super Goblin spawn chance: {spawnChance * 100}% - Should Spawn: {shouldSpawn}");
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
