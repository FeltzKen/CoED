using UnityEngine;

namespace CoED.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HiddenItemController : MonoBehaviour, CoED.ISearchable
    {
        [Header("Hidden Item Settings")]
        public bool isHidden = false;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"{name} has HiddenItemController but no SpriteRenderer on it!");
            }
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
                Debug.Log($"{name} was discovered!");
                if (GetComponent<ItemCollectible>() != null)
                {
                    PlayerActions.Instance.CollectItem(GetComponent<ItemCollectible>());
                }
            }
            else
            {
                Debug.Log($"{name} was already revealed.");
            }
        }

        public void RevealItem()
        {
            if (isHidden)
            {
                isHidden = false;
                Debug.Log($"{name}: Hidden item discovered!");

                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingLayerName = "items";
                    spriteRenderer.sortingOrder = 0;
                }
            }
        }
    }
}
