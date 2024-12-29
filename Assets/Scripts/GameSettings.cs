using UnityEngine;

namespace CoED
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings Instance { get; private set; }

        [Header("Difficulty Settings")]
        [SerializeField, Range(1, 10)]
        private int difficultyLevel = 5; // Default difficulty level

        private const float BaseEnemyStatMultiplier = 1f;
        private const float DifficultyMultiplierPerLevel = 0.1f;
        private float currentMultiplier;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                UpdateEnemyStatMultiplier();
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("GameSettings instance already exists. Destroying duplicate.");
            }
        }

        public int DifficultyLevel => difficultyLevel;

        public void SetDifficultyLevel(int level)
        {
            if (level < 1)
            {
                difficultyLevel = 1;
                Debug.LogWarning("GameSettings: Difficulty level cannot be less than 1. Set to 1.");
            }
            else if (level > 10)
            {
                difficultyLevel = 10;
                Debug.LogWarning(
                    "GameSettings: Difficulty level cannot be greater than 10. Set to 10."
                );
            }
            else
            {
                difficultyLevel = level;
            }

            UpdateEnemyStatMultiplier();
        }

        public float GetEnemyStatMultiplier()
        {
            return currentMultiplier;
        }

        private void UpdateEnemyStatMultiplier()
        {
            currentMultiplier =
                BaseEnemyStatMultiplier + (difficultyLevel - 1) * DifficultyMultiplierPerLevel;
        }

        public void ResetDifficulty()
        {
            difficultyLevel = 5;
            UpdateEnemyStatMultiplier();
        }
    }
}
