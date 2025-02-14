using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class SpellEntry : MonoBehaviour
    {
        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private TextMeshProUGUI spellNameText;

        [SerializeField]
        private Button selectButton;

        private Spell spell;

        /// <summary>
        /// Initializes the spell entry with the spell data.
        /// </summary>
        /// <param name="spellData">The spell to display.</param>
        /// <param name="onClickCallback">Callback invoked when the entry is clicked.</param>
        public void Setup(Spell spellData, Action<Spell> onClickCallback)
        {
            spell = spellData;

            if (iconImage != null)
                iconImage.sprite = spell.Icon;

            if (spellNameText != null)
                spellNameText.text = spell.SpellName;

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => onClickCallback?.Invoke(spell));
            }
        }
    }
}
