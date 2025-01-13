using System.Collections.Generic;
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
        public List<StatusEffectType> removedEffects;
        public List<StatusEffectType> addedEffects;
        public Sprite Icon => icon;
    }
}
