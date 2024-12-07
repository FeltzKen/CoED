using UnityEngine;

namespace CoED
{
    public class ItemCollectible : MonoBehaviour
    {
        [SerializeField]
        public Consumable item; // Ensure this is of type Consumable

        public Consumable Item => item;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
             //   Debug.Log("ItemCollectible: Player entered trigger.");
                CollectItem(PlayerActions.Instance);
            }
        }

        private void CollectItem(PlayerActions playerActions)
        {
            if (playerActions != null)
            {
                playerActions.CollectItem(this);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("ItemCollectible: PlayerActions is null.");
            }
        }
    }
}