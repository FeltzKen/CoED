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
    private float critDamageBoost;

    [SerializeField]
    private float projectileRangeBoost;

    [SerializeField]
    private float attackRangeBoost;

    [SerializeField]
    private float elementalDamageBoost;

    [SerializeField]
    private float ChanceToInflictBoost;

    [SerializeField]
    private float statusEffectDurationBoost;

    [SerializeField]
    private float fireRateBoost;

    [SerializeField]
    private float shieldBoost;

    [SerializeField]
    private float evasionBoost;

    [SerializeField]
    private float accuracyBoost;

    [SerializeField]
    private List<StatusEffectType> addedEffect = new List<StatusEffectType>();

    [SerializeField]
    private List<StatusEffectType> removedEffect = new List<StatusEffectType>();

    public void SetConsumable(ConsumableItem item)
    {
        itemData = item;
        itemName = itemData.GetName();
        GetComponent<SpriteRenderer>().sprite = itemData.GetSprite();
        attackBoost = itemData.consumableStats[Stat.Attack];
        defenseBoost = itemData.consumableStats[Stat.Defense];
        speedBoost = itemData.consumableStats[Stat.Speed];
        healthBoost = itemData.consumableStats[Stat.HP];
        magicBoost = itemData.consumableStats[Stat.Magic];
        staminaBoost = itemData.consumableStats[Stat.Stamina];
        dexterityBoost = itemData.consumableStats[Stat.Dexterity];
        intelligenceBoost = itemData.consumableStats[Stat.Intelligence];
        critChanceBoost = itemData.consumableStats[Stat.CritChance];
        critDamageBoost = itemData.consumableStats[Stat.CritDamage];
        projectileRangeBoost = itemData.consumableStats[Stat.ProjectileRange];
        attackRangeBoost = itemData.consumableStats[Stat.AttackRange];
        elementalDamageBoost = itemData.consumableStats[Stat.ElementalDamage];
        ChanceToInflictBoost = itemData.consumableStats[Stat.ChanceToInflict];
        statusEffectDurationBoost = itemData.consumableStats[Stat.StatusEffectDuration];
        fireRateBoost = itemData.consumableStats[Stat.FireRate];
        shieldBoost = itemData.consumableStats[Stat.Shield];
        addedEffect = itemData.addedEffects;
        removedEffect = itemData.removedEffects;
    }

    public ConsumableItem GetConsumableData()
    {
        return itemData;
    }
}
