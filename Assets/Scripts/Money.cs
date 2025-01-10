using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(fileName = "Money", menuName = "Item/Money")]
    public class Money : Item
    {
        [Header("Money Settings")]
        [SerializeField]
        private int baseAmount;

        public int GetBaseAmount()
        {
            return baseAmount;
        }

        public void SetAmount(int amount)
        {
            baseAmount = amount;
        }
    }
}
