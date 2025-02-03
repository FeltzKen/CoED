using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class StatAllocationUI : MonoBehaviour
    {
        [Tooltip("The stat this UI element adjusts.")]
        public Stat stat;

        [SerializeField]
        private TextMeshProUGUI statValueText;

        [SerializeField]
        private TextMeshProUGUI statChangedValueText;

        [SerializeField]
        private Button statSelectButton;

        private float baseValue;
        private int allocatedPoints = 0;
        private bool isSelected = false;

        public Action<Stat> onStatSelected;

        private void Awake()
        {
            if (statSelectButton != null)
            {
                Debug.Log($"StatSelectButton assigned for {stat}: {statSelectButton.name}");
                statSelectButton.onClick.AddListener(() => onStatSelected?.Invoke(stat));
            }
            else
            {
                Debug.LogError($"StatSelectButton is missing for {stat}");
            }
        }

        /// <summary>
        /// Updates the displayed value for this stat.
        /// </summary>
        public void SetStatValue(float baseStat, int allocated)
        {
            baseValue = baseStat;
            allocatedPoints = allocated;

            // Display the total stat value (base + allocated)
            statValueText.text = $"{baseValue + allocatedPoints}";

            // Display the allocated points in brackets if any are allocated
            statChangedValueText.text = allocatedPoints > 0 ? $"[+{allocatedPoints}]" : "";
        }

        /// <summary>
        /// Updates the visual selection state.
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (statSelectButton.image != null)
            {
                ColorBlock colors = statSelectButton.colors;
                colors.normalColor = selected ? Color.green : Color.white;
                colors.selectedColor = selected ? Color.green : Color.white;
                statSelectButton.colors = colors;
            }
        }

        public bool CanDecreaseStat()
        {
            return allocatedPoints > 0;
        }
    }
}
