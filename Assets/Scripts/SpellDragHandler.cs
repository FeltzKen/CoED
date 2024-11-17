// SpellDragHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class SpellDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Spell spell;

        [SerializeField]
        private Image iconImage;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Transform originalParent;

        public static SpellDragHandler Instance { get; private set; }

        private void Awake()
        {
            // Implement Singleton Pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("SpellDragHandler instance already exists. Destroying duplicate.");
                return;
            }

            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            if (iconImage != null && spell != null)
            {
                iconImage.sprite = spell.Icon;
                iconImage.enabled = true;
            }
            else
            {
                Debug.LogWarning("SpellDragHandler: IconImage or Spell is not assigned.");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = transform.parent;
            transform.SetParent(transform.root);
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            //rectTransform.anchoredPosition += 1;// eventData.delta / canvas.GetCanvasScaleFactor();
        }

        public void ClearDrag()
        {
            // implement clear drag
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(originalParent);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            // Handle drop logic
            if (eventData.pointerEnter != null)
            {
                SpellSlot targetSlot = eventData.pointerEnter.GetComponent<SpellSlot>();
                if (targetSlot != null)
                {
                    targetSlot.AssignSpell(spell);
                }
            }
        }

        public void SetSpell(Spell newSpell)
        {
            spell = newSpell;
            if (iconImage != null)
            {
                iconImage.sprite = spell != null ? spell.Icon : null;
                iconImage.enabled = spell != null;
            }
        }

        public Spell GetSpell()
        {
            return spell;
        }
    }
}
