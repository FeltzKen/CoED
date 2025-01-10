using UnityEngine;

namespace CoED
{
    public class ItemCollectible : MonoBehaviour
    {
        [SerializeField]
        public ConsumableItemWrapper consumeItem;
        public ConsumableItemWrapper ConsumeItem => consumeItem;

        [SerializeField]
        private string itemDescription;

        public void Awake()
        {
            itemDescription = consumeItem.GetDescription();
        }

    }
}
