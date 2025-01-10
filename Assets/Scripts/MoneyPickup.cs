using UnityEngine;

namespace CoED
{
    public class MoneyPickup : MonoBehaviour
    {
        [SerializeField]
        public Money moneyData;

        public void Initialize(Money money, int scaledAmount = 0)
        {
            moneyData = money;
            if (scaledAmount > 0)
            {
                moneyData.SetAmount(scaledAmount); // Optional scaling logic
            }
        }

        public void SetAmount(int amount)
        {
            moneyData.SetAmount(amount);
        }

        public void Collect()
        {
            int amount = moneyData.GetBaseAmount();
            PlayerStats.Instance.GainCurrency(amount);
            Debug.Log($"Player collected {amount} gold.");
            Destroy(gameObject); // Remove the money object after collection
        }
    }
}
