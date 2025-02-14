using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CoED
{
    public class SpellLearningPanel : MonoBehaviour
    {
        public static SpellLearningPanel Instance { get; private set; }

        [Header("UI References")]
        [SerializeField]
        private GameObject spellEntryPrefab; // Prefab for a single spell entry.

        [SerializeField]
        private Transform contentPanel; // Panel to hold the spell entries.

        [SerializeField]
        private Transform spellPanel;

        [SerializeField]
        private TextMeshProUGUI panelTitle; // Optional title text for the panel.

        // Callback to invoke when a spell is chosen.
        private Action<Spell> onSpellChosenCallback;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Optionally disable the panel on start.
                spellPanel.gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Displays the Spell Learning Panel with the given list of spells.
        /// </summary>
        /// <param name="availableSpells">The list of spells the player can choose from.</param>
        /// <param name="onSpellChosen">Callback invoked when a spell is selected.</param>
        public void Show(List<Spell> availableSpells, Action<Spell> onSpellChosen)
        {
            onSpellChosenCallback = onSpellChosen;
            // onSpellChosenCallback = onSpellChosen;

            // Clear any existing entries.
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            // Optionally update the panel title.
            if (panelTitle != null)
            {
                panelTitle.text = "Choose a New Spell";
            }

            // Create an entry for each available spell.
            foreach (Spell spell in availableSpells)
            {
                GameObject entryObj = Instantiate(spellEntryPrefab, contentPanel);
                SpellEntry entry = entryObj.GetComponent<SpellEntry>();
                if (entry != null)
                {
                    entry.Setup(spell, OnEntryClicked);
                }
            }

            spellPanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// Callback when a spell entry is clicked.
        /// </summary>
        /// <param name="spell">The spell selected by the player.</param>
        private void OnEntryClicked(Spell spell)
        {
            onSpellChosenCallback?.Invoke(spell);
            Hide();
        }

        /// <summary>
        /// Hides the Spell Learning Panel.
        /// </summary>
        public void Hide()
        {
            spellPanel.gameObject.SetActive(false);
        }
    }
}
