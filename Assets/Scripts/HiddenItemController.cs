using UnityEngine;

namespace CoED.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HiddenItemController : MonoBehaviour, ISearchable
    {
        [Header("Hidden Item Settings")]
        public bool isHidden = false;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (isHidden && spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sortingOrder = 3;
            }
        }

        public void OnSearch()
        {
            if (isHidden)
            {
                RevealItem();
            }
        }

        public void RevealItem()
        {
            if (isHidden)
            {
                isHidden = false;

                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingLayerName = "items";
                    spriteRenderer.sortingOrder = 0;
                }
            }
        }
    }
}
