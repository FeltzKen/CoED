using CoED;
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
        public int attackBoost = 0;

        [SerializeField]
        public int defenseBoost = 0;

        [SerializeField]
        public int speedBoost = 0;

        [SerializeField]
        public int healthBoost = 0;

        public string ItemName => itemName;

        public Sprite Icon => icon;

        public int AttackBoost => attackBoost;

        public int DefenseBoost => defenseBoost;

        public int HealthBoost => healthBoost;
    }
}
