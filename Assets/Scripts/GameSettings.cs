using UnityEngine;
using CoED;

namespace CoED
{
    /// Manages global game settings such as difficulty level and related multipliers.
    public class GameSettings : MonoBehaviour
    {
        /// Singleton instance of GameSettings.
        public static GameSettings Instance { get; private set; }

        [Header("Difficulty Settings")]
        [SerializeField, Range(1, 10)]
        private int difficultyLevel = 5; // Default difficulty level

        private const float BaseEnemyStatMultiplier = 1f;
        private const float DifficultyMultiplierPerLevel = 0.1f;
        private float currentMultiplier;

        private void Awake()
        {
            // Implement Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
                UpdateEnemyStatMultiplier(); // Initialize multiplier
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("GameSettings instance already exists. Destroying duplicate.");
            }
        }

        /// Gets the current difficulty level.
        public int DifficultyLevel => difficultyLevel;

        /// Sets the difficulty level within the allowed range.
        /// <param name="level">The desired difficulty level (1-10).</param>
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
            Debug.Log($"GameSettings: Difficulty level set to {difficultyLevel}.");
        }

        /// Calculates the enemy stat multiplier based on the current difficulty level.
        public float GetEnemyStatMultiplier()
        {
            return currentMultiplier;
        }

        /// Updates the enemy stat multiplier based on the current difficulty level.
        private void UpdateEnemyStatMultiplier()
        {
            currentMultiplier =
                BaseEnemyStatMultiplier + (difficultyLevel - 1) * DifficultyMultiplierPerLevel;
        }

        /// Resets the difficulty level to default.
        public void ResetDifficulty()
        {
            difficultyLevel = 5;
            UpdateEnemyStatMultiplier();
            Debug.Log("GameSettings: Difficulty level reset to default (5).");
        }
    }
}
