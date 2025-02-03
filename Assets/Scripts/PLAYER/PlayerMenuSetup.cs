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

        private Dictionary<Stat, int> additionalStatPoints = new Dictionary<Stat, int>();
        private Dictionary<Stat, float> baseStatValues = new Dictionary<Stat, float>();
        private int availableStatPoints = 10;
        private int selectedClassIndex = 0;
        private Stat selectedStat = Stat.None; // currently selected stat

        private void Start()
        {
            Time.timeScale = 0;

            foreach (StatAllocationUI statUI in statUIElements)
            {
                additionalStatPoints[statUI.stat] = 0;
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

            // Ensure that the selectedStat is reset to prevent issues
            selectedStat = Stat.None;

            Debug.Log(
                $"Class changed to {CharacterClasses.Classes[selectedClassIndex].ClassName}, UI updating."
            );

            UpdateUI(); // ✅ Ensures the stat panel and description panel update immediately
        }

        private void ResetStatAllocations()
        {
            availableStatPoints = 10;
            selectedStat = Stat.None;

            // ✅ Clear additional stat points
            additionalStatPoints.Clear();

            // ✅ Reset base stat values to prevent old values from persisting
            baseStatValues.Clear();

            foreach (StatAllocationUI statUI in statUIElements)
            {
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

            foreach (StatAllocationUI statUI in statUIElements)
            {
                float baseValue = selectedClass.BaseStats.ContainsKey(statUI.stat)
                    ? selectedClass.BaseStats[statUI.stat]
                    : 0;

                baseStatValues[statUI.stat] = baseValue;

                // ✅ Reset allocated points before updating UI
                additionalStatPoints[statUI.stat] = 0;

                statUI.SetStatValue(baseValue, 0);
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

            UpdateUI(); // ✅ Ensures UI reflects the new selection immediately
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

            foreach (KeyValuePair<Stat, int> kvp in additionalStatPoints)
            {
                if (finalClass.BaseStats.ContainsKey(kvp.Key))
                {
                    finalClass.BaseStats[kvp.Key] += kvp.Value;
                }
                else
                {
                    finalClass.BaseStats[kvp.Key] = kvp.Value;
                }
            }

            GameManager.SelectedClass = finalClass;
            Debug.Log($"Player selected class: {finalClass.ClassName}");

            GameManager.Instance.SpawnPlayer();
            gameObject.SetActive(false);
            UIPanelToggleManager.Instance.UnpauseGame();
        }
    }
}
