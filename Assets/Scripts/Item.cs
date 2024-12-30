using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        public GameObject itemPrefab;

        [Header("Item Attributes")]
        [SerializeField]
        public string itemName;

        [SerializeField]
        public Sprite icon;

        public bool isHidden;

        [SerializeField]
        public float attackBoost = 0;

        [SerializeField]
        public float defenseBoost = 0;

        [SerializeField]
        public float speedBoost = 0;

        [SerializeField]
        public float healthBoost = 0;

        public string ItemName => itemName;

        public Sprite Icon => icon;

        public float AttackBoost => attackBoost;

        public float DefenseBoost => defenseBoost;

        public float HealthBoost => healthBoost;
    }
}
