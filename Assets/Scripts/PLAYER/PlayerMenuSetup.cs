using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class PlayerSetupMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private TMP_Dropdown classDropdown;

        [SerializeField]
        private TextMeshProUGUI classDescriptionText;

        [SerializeField]
        private Transform statAllocationPanel;

        [SerializeField]
        private TextMeshProUGUI availablePointsText;

        [SerializeField]
        private Button confirmButton;

        [SerializeField]
        private Button globalIncreaseButton;

        [SerializeField]
        private Button globalDecreaseButton;

        [Header("Stats to Adjust")]
        [SerializeField]
        private List<StatAllocationUI> statUIElements;

        [SerializeField]
        private Image classImage;

        [Header("Difficulty Settings")]
        [SerializeField]
        private Slider difficultySlider;

        [SerializeField]
        private TextMeshProUGUI difficultyLevelText;

        // Holds the extra points allocated for each stat.
        private Dictionary<Stat, int> additionalStatPoints = new Dictionary<Stat, int>();

        // Holds the base stat values (from the selected class).
        private Dictionary<Stat, float> baseStatValues = new Dictionary<Stat, float>();

        private int availableStatPoints = 10;
        private int selectedClassIndex = 0;
        private Stat selectedStat = Stat.None; // currently selected stat

        private void Start()
        {
            // Initialize the allocation dictionary.
            foreach (StatAllocationUI statUI in statUIElements)
            {
                additionalStatPoints[statUI.stat] = 0;
            }

            // Initialize the difficulty slider if assigned.
            if (difficultySlider != null)
            {
                // Set slider value to current difficulty level.
                difficultySlider.value = 1;
                difficultyLevelText.text = "1";
                difficultySlider.onValueChanged.AddListener(OnDifficultySliderChanged);
            }

            PopulateClassDropdown();
            classDropdown.onValueChanged.AddListener(OnClassDropdownChanged);
            confirmButton.onClick.AddListener(OnConfirm);

            SetupStatUI();

            // Hook up the global increase/decrease buttons.
            globalIncreaseButton.onClick.AddListener(OnIncreaseGlobal);
            globalDecreaseButton.onClick.AddListener(OnDecreaseGlobal);
            Debug.Log($"Total Stat UI Elements: {statUIElements.Count}");

            UpdateUI();
        }

        private void OnDifficultySliderChanged(float value)
        {
            // Round the slider value to an integer (1 to 10)
            int newDifficulty = Mathf.RoundToInt(value);

            // Update the dungeon difficulty setting.
            DungeonManager.Instance.dungeonDifficultySetting = newDifficulty;

            // Update the difficulty text value.
            if (difficultyLevelText != null)
            {
                difficultyLevelText.text = newDifficulty.ToString();

                // Compute an interpolation fraction:
                // When newDifficulty == 1, fraction == 0; when newDifficulty == 10, fraction == 1.
                float fraction = (newDifficulty - 1) / 9f;

                // Interpolate from white (1,1,1) to red (1,0,0)
                // The red channel remains 1, while green and blue decrease from 1 to 0.
                Color newColor = new Color(1f, 1f - fraction, 1f - fraction);

                // Apply the new color.
                difficultyLevelText.color = newColor;
            }
        }

        private void SetupStatUI()
        {
            foreach (StatAllocationUI statUI in statUIElements)
            {
                statUI.onStatSelected = SelectStat;
            }
        }

        private void PopulateClassDropdown()
        {
            classDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (CharacterClass characterClass in CharacterClasses.Classes)
            {
                options.Add(characterClass.ClassName);
            }
            classDropdown.AddOptions(options);
        }

        private void OnClassDropdownChanged(int index)
        {
            selectedClassIndex = index;
            ResetStatAllocations();

            // Reset selected stat when a new class is chosen.
            selectedStat = Stat.None;

            Debug.Log(
                $"Class changed to {CharacterClasses.Classes[selectedClassIndex].ClassName}, UI updating."
            );
            UpdateUI(); // Immediately update the UI
        }

        private void ResetStatAllocations()
        {
            availableStatPoints = 10;
            selectedStat = Stat.None;

            // Reset each stat’s extra allocation to 0 and update the base values.
            foreach (StatAllocationUI statUI in statUIElements)
            {
                additionalStatPoints[statUI.stat] = 0;

                float baseValue = CharacterClasses
                    .Classes[selectedClassIndex]
                    .BaseStats.ContainsKey(statUI.stat)
                    ? CharacterClasses.Classes[selectedClassIndex].BaseStats[statUI.stat]
                    : 0;

                baseStatValues[statUI.stat] = baseValue;
                statUI.SetStatValue(baseValue, 0);
                statUI.SetSelected(false);
            }

            Debug.Log("Stat allocations reset, UI updated.");
        }

        private void UpdateUI()
        {
            CharacterClass selectedClass = CharacterClasses.Classes[selectedClassIndex];
            classDescriptionText.text = GetClassDescription(selectedClass);
            availablePointsText.text = availableStatPoints.ToString();

            // Update the class image.
            classImage.sprite = selectedClass.CharacterSprite;
            Color color = classImage.color;
            color.a = 1;
            classImage.color = color;

            // Update each stat UI element without resetting the extra points.
            foreach (StatAllocationUI statUI in statUIElements)
            {
                float baseValue = selectedClass.BaseStats.ContainsKey(statUI.stat)
                    ? selectedClass.BaseStats[statUI.stat]
                    : 0;

                baseStatValues[statUI.stat] = baseValue;
                int allocated = additionalStatPoints.ContainsKey(statUI.stat)
                    ? additionalStatPoints[statUI.stat]
                    : 0;

                statUI.SetStatValue(baseValue, allocated);
                statUI.SetSelected(statUI.stat == selectedStat);
            }
        }

        /// <summary>
        /// Called when a stat’s own button is clicked.
        /// </summary>
        private void SelectStat(Stat stat)
        {
            Debug.Log($"Selected Stat: {stat}");
            selectedStat = stat;

            foreach (StatAllocationUI statUI in statUIElements)
            {
                statUI.SetSelected(statUI.stat == selectedStat);
            }

            UpdateUI(); // Refresh the UI to show the new selection
        }

        /// <summary>
        /// Called when the global Increase button is pressed.
        /// </summary>
        private void OnIncreaseGlobal()
        {
            if (selectedStat == Stat.None)
            {
                Debug.Log("No stat selected.");
                return;
            }

            if (availableStatPoints <= 0)
            {
                Debug.Log("No more points available.");
                return;
            }

            // For both normal and percentage stats, we simply add one point.
            additionalStatPoints[selectedStat]++;
            availableStatPoints--;

            Debug.Log($"Increased {selectedStat} to {additionalStatPoints[selectedStat]}");
            UpdateUI();
        }

        /// <summary>
        /// Called when the global Decrease button is pressed.
        /// </summary>
        private void OnDecreaseGlobal()
        {
            if (selectedStat == Stat.None)
            {
                Debug.Log("No stat selected.");
                return;
            }

            if (additionalStatPoints[selectedStat] <= 0)
                return;

            float baseValue = baseStatValues.ContainsKey(selectedStat)
                ? baseStatValues[selectedStat]
                : 0;
            // Only allow decreasing if extra points have been added.
            if (baseValue + additionalStatPoints[selectedStat] > baseValue)
            {
                additionalStatPoints[selectedStat]--;
                availableStatPoints++;

                Debug.Log(
                    $"Removed point from {selectedStat}. New Value: {additionalStatPoints[selectedStat]}"
                );
                UpdateUI();
            }
        }

        private string GetClassDescription(CharacterClass characterClass)
        {
            string description = $"{characterClass.ClassName}\n";
            foreach (var stat in characterClass.BaseStats)
            {
                description += $"{stat.Key}: {stat.Value}\n";
            }
            return description;
        }

        private void OnConfirm()
        {
            CharacterClass selectedClass = CharacterClasses.Classes[selectedClassIndex];

            CharacterClass finalClass = new CharacterClass
            {
                ClassName = selectedClass.ClassName,
                CharacterSprite = selectedClass.CharacterSprite,
                BaseStats = new Dictionary<Stat, float>(selectedClass.BaseStats),
            };

            // Apply additional stat points.
            foreach (KeyValuePair<Stat, int> kvp in additionalStatPoints)
            {
                float addition;
                if (kvp.Key == Stat.CritChance || kvp.Key == Stat.ChanceToInflict)
                {
                    // Percentage-based: each point adds 0.01.
                    addition = kvp.Value * 0.01f;
                }
                else if (
                    kvp.Key == Stat.MaxHP
                    || kvp.Key == Stat.MaxStamina
                    || kvp.Key == Stat.MaxMagic
                )
                {
                    // For these stats, each pool point adds 20.
                    addition = kvp.Value * 10f;
                }
                else
                {
                    addition = kvp.Value;
                }

                if (finalClass.BaseStats.ContainsKey(kvp.Key))
                {
                    finalClass.BaseStats[kvp.Key] += addition;
                }
                else
                {
                    finalClass.BaseStats[kvp.Key] = addition;
                }
            }

            finalClass.LevelToLearnSpells = selectedClass.LevelToLearnSpells;
            GameManager.SelectedClass = finalClass;
            GameManager.Instance.playerTransform.GetComponent<SpriteRenderer>().sprite =
                finalClass.CharacterSprite;
            PlayerStats.Instance.baseStats = finalClass.BaseStats;
            PlayerStats.Instance.CopyBaseToPlayerStats();
            PlayerStats.Instance.CalculateStats();
            Debug.Log($"Player selected class: {GameManager.SelectedClass.ClassName}");

            gameObject.SetActive(false);
            DungeonSpawner.Instance.SpawnEnemiesForAllFloors();
        }
    }
}
