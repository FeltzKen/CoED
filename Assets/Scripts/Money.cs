using UnityEngine;
using CoED;

namespace CoED
{
    public class Money : MonoBehaviour
    {
        [Header("Money Settings")]
        [SerializeField]
        private int amount = 1;

        private PlayerStats playerStats;

        private void Awake()
        {
            playerStats = PlayerStats.Instance;
            if (playerStats == null)
            {
                Debug.LogError("Money: PlayerStats instance not found.");
            }
        }

        public void SetAmount(int newAmount)
        {
            if (newAmount < 0)
            {
                Debug.LogWarning("Money: Attempted to set a negative amount. Defaulting to 0.");
                amount = 0;
            }
            else
            {
                amount = newAmount;
            }
        }

        public int Collect()
        {
            if (playerStats != null)
            {
                playerStats.GainCurrency(amount);
                Debug.Log($"Money: Collected {amount} currency.");
            }
            else
            {
                Debug.LogWarning("Money: PlayerStats instance not found. Cannot add currency.");
            }

            Destroy(gameObject, 0.1f);
            return amount;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Collect();
            }
        }
    }
}
