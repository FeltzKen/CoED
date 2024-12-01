using System.Collections.Generic;
using UnityEngine;
using CoED;

namespace CoED
{
    public class MagicPanel : MonoBehaviour
    {
        public static MagicPanel Instance { get; private set; }

        [Header("Magic Panel Settings")]
        [SerializeField]
        private List<SpellSlot> spellSlots = new List<SpellSlot>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("MagicPanel instance already exists. Destroying duplicate.");
            }
        }
        public Spell GetSpellAtIndex(int index)
        {
            if (index < 0 || index >= spellSlots.Count)
            {
                Debug.LogWarning("MagicPanel: Invalid index. Unable to retrieve spell.");
                return null;
            }

            return spellSlots[index].GetAssignedSpell();
        }
        public void EquipSpell(Spell spell)
        {
            if (spell == null)
            {
                Debug.LogWarning("MagicPanel: Attempted to equip a null spell.");
                return;
            }

            foreach (var slot in spellSlots)
            {
                if (slot.IsEmpty())
                {
                    slot.AssignSpell(spell);
                    // Debug.Log($"MagicPanel: Equipped spell {spell.SpellName}.");
                    return;
                }
            }

            // Debug.Log("MagicPanel: All spell slots are full. Unable to equip new spell.");
        }

        public void UnequipSpell(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= spellSlots.Count)
            {
                Debug.LogWarning("MagicPanel: Invalid slot index. Unable to unequip spell.");
                return;
            }

            spellSlots[slotIndex].ClearSlot();
            // Debug.Log($"MagicPanel: Unequipped spell from slot {slotIndex}.");
        }

        public void DisplayEquippedSpells()
        {
            foreach (var slot in spellSlots)
            {
                Spell spell = slot.GetAssignedSpell();
                if (spell != null)
                {
                    // Debug.Log($"MagicPanel: Spell {spell.SpellName} is equipped.");
                }
                else
                {
                    // Debug.Log("MagicPanel: Slot is empty.");
                }
            }
        }

        public void ClearAllSpells()
        {
            foreach (var slot in spellSlots)
            {
                slot.ClearSlot();
            }
            // Debug.Log("MagicPanel: Cleared all spell slots.");
        }
    }
}
