using UnityEngine;

namespace CoED.Items
{
    /// <summary>
    /// This script is attached to hidden item prefabs.
    /// It implements ISearchable so the player can discover the hidden item by searching.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class HiddenItemController : MonoBehaviour, CoED.ISearchable
    {
        [Header("Hidden Item Settings")]
        public bool isHidden = false;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            // We assume there's a SpriteRenderer on this object or its child
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"{name} has HiddenItemController but no SpriteRenderer on it!");
            }
        }

        private void Start()
        {
            // If initially hidden, put it behind walls or hide it
            if (isHidden && spriteRenderer != null)
            {
                // Example: set sorting layer behind walls
                // spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sortingOrder = 4;
            }
        }

        /// <summary>
        /// The universal "search" method from ISearchable.
        /// Called by the player's search system if within range.
        /// </summary>
        public void OnSearch()
        {
            // Reveal the item if it was hidden
            if (isHidden)
            {
                RevealItem();
                Debug.Log($"{name} was discovered!");
                // PlayerActions.Instance.CollectItem(this);
            }
            else
            {
                Debug.Log($"{name} was already revealed.");
            }
        }

        /// <summary>
        /// Called internally to reveal the hidden item.
        /// Could also be used by other logic if needed.
        /// </summary>
        public void RevealItem()
        {
            if (isHidden)
            {
                isHidden = false;
                Debug.Log($"{name}: Hidden item discovered!");

                // Optionally set a normal sorting layer or order
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingLayerName = "items"; // or "Items", etc.
                    spriteRenderer.sortingOrder = 0;
                }
            }
        }
    }
}
