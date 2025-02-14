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
        /// Returns true if this stat is one of the percentage-based stats.
        /// </summary>
        private bool IsPercentageStat()
        {
            return stat == Stat.CritChance || stat == Stat.ChanceToInflict;
        }

        /// <summary>
        /// Returns true if this stat is one of the multiplier-based stats.
        /// </summary>
        private bool IsMultiplierStat()
        {
            return stat == Stat.MaxHP || stat == Stat.MaxStamina || stat == Stat.MaxMagic;
        }

        /// <summary>
        /// Updates the displayed value for this stat.
        /// </summary>
        /// <param name="baseStat">The base value from the class.</param>
        /// <param name="allocated">The extra points allocated to this stat.</param>
        public void SetStatValue(float baseStat, int allocated)
        {
            baseValue = baseStat;
            allocatedPoints = allocated;

            if (IsPercentageStat())
            {
                // For percentage-based stats, 1 pool point = 0.01 added.
                float total = baseValue + allocatedPoints * 0.01f;
                // Format as percentage (e.g., 0.09 becomes "9%").
                statValueText.text = $"{total:P0}";
                statChangedValueText.text =
                    allocatedPoints > 0 ? $"[+{allocatedPoints * 0.01f:P0}]" : "";
            }
            else if (IsMultiplierStat())
            {
                // For MaxHP, MaxStamina, and MaxMagic: 1 pool point = 20 added.
                float total = baseValue + allocatedPoints * 10f;
                statValueText.text = total.ToString();
                statChangedValueText.text = allocatedPoints > 0 ? $"[+{allocatedPoints * 10}]" : "";
            }
            else
            {
                // For all other stats, 1 pool point = 1 added.
                float total = baseValue + allocatedPoints;
                statValueText.text = total.ToString();
                statChangedValueText.text = allocatedPoints > 0 ? $"[+{allocatedPoints}]" : "";
            }
        }

        /// <summary>
        /// Updates the visual selection state.
        /// </summary>
        /// <param name="selected">True if this stat is selected.</param>
        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (statSelectButton.image != null)
            {
                // Change the button color to indicate selection.
                statSelectButton.image.color = isSelected ? Color.red : Color.white;
            }
        }

        public bool CanDecreaseStat()
        {
            return allocatedPoints > 0;
        }
    }
}
