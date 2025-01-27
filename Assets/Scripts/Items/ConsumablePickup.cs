using System.Collections.Generic;
using CoED;
using UnityEngine;

public class ConsumablePickup : MonoBehaviour
{
    private ConsumableItem itemData;

    [SerializeField]
    private string itemName;

    [SerializeField]
    private float attackBoost;

    [SerializeField]
    private float defenseBoost;

    [SerializeField]
    private float speedBoost;

    [SerializeField]
    private float healthBoost;

    [SerializeField]
    private float magicBoost;

    [SerializeField]
    private float staminaBoost;

    [SerializeField]
    private float dexterityBoost;

    [SerializeField]
    private float intelligenceBoost;

    [SerializeField]
    private float critChanceBoost;

    [SerializeField]
    private List<StatusEffectType> addedEffect = new List<StatusEffectType>();

    [SerializeField]
    private List<StatusEffectType> removedEffect = new List<StatusEffectType>();

    public void SetConsumable(ConsumableItem item)
    {
        itemData = item;
        itemName = itemData.name;
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        attackBoost = itemData.attackBoost;
        defenseBoost = itemData.defenseBoost;
        speedBoost = itemData.speedBoost;
        healthBoost = itemData.healthBoost;
        magicBoost = itemData.magicBoost;
        staminaBoost = itemData.staminaBoost;
        dexterityBoost = itemData.dexterityBoost;
        intelligenceBoost = itemData.intelligenceBoost;
        critChanceBoost = itemData.critChanceBoost;
        addedEffect = itemData.addedEffects;
        removedEffect = itemData.removedEffects;
    }

    public ConsumableItem GetConsumableData()
    {
        return itemData;
    }
}
