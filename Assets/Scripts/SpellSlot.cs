using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CoED;

namespace CoED
{
    public class SpellSlot : MonoBehaviour//, IDropHandler
    {
        [Header("Slot Settings")]
        [SerializeField]
        private Image spellIconImage;

        [SerializeField]
        private Text cooldownText;

        [SerializeField]
        private Spell assignedSpell;

        private bool isOnCooldown = false;
        private float cooldownTimer = 0f;

        private void Update()
        {
            if (isOnCooldown)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    isOnCooldown = false;
                    cooldownText.enabled = false;
                }
                else
                {
                    cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
                }
            }
        }

        public void AssignSpell(Spell spell)
        {
            if (spell == null)
            {
                Debug.LogWarning("SpellSlot: Attempted to assign a null spell.");
                return;
            }

            assignedSpell = spell;
            spellIconImage.sprite = spell.Icon;
            spellIconImage.enabled = true;
        }

        public void ClearSlot()
        {
            assignedSpell = null;
            spellIconImage.sprite = null;
            spellIconImage.enabled = false;
            cooldownText.enabled = false;
        }

        public bool IsEmpty()
        {
            return assignedSpell == null;
        }

        public Spell GetAssignedSpell()
        {
            return assignedSpell;
        }

     /*   public void OnDrop(PointerEventData eventData)
        {
            SpellDragHandler dragHandler = eventData.pointerDrag?.GetComponent<SpellDragHandler>();
            if (dragHandler != null)
            {
                Spell draggedSpell = dragHandler.GetSpell();
                if (draggedSpell != null)
                {
                    AssignSpell(draggedSpell);
                    dragHandler.ClearDrag();
                    // Debug.Log($"SpellSlot: Assigned spell '{draggedSpell.SpellName}' to slot.");
                }
                else
                {
                    Debug.LogWarning("SpellSlot: Dragged spell is null.");
                }
            }
        }
*/
        public void StartCooldown()
        {
            if (assignedSpell != null)
            {
                isOnCooldown = true;
                cooldownTimer = assignedSpell.Cooldown;
                cooldownText.enabled = true;
                // Debug.Log($"SpellSlot: Started cooldown for spell '{assignedSpell.SpellName}'.");
            }
            else
            {
                Debug.LogWarning("SpellSlot: Attempted to start cooldown with no spell assigned.");
            }
        }
    }
}
