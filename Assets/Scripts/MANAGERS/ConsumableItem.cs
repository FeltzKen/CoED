using CoED;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]
public class Consumable : Item
{
    [SerializeField]
    public float attackBoost = 0;

    [SerializeField]
    public float defenseBoost = 0;

    [SerializeField]
    public float speedBoost = 0;

    [SerializeField]
    public float healthBoost = 0;

    [SerializeField]
    public float magicBoost = 0;

    [SerializeField]
    public float staminaBoost = 0;

    [SerializeField]
    private DescriptionField descriptionField = DescriptionField.None; // Default to None

    [SerializeField, TextArea]
    private string description = ""; // Optional custom text

    public string ItemName => itemName;
    public float AttackBoost => attackBoost;
    public float DefenseBoost => defenseBoost;
    public float SpeedBoost => speedBoost;
    public float HealthBoost => healthBoost;
    public float MagicBoost => magicBoost;
    public float StaminaBoost => staminaBoost;

    public string Description
    {
        get
        {
            float value = GetDescriptionValue();
            string baseDescription = $"{itemName} +{value}";

            // Append the custom description, if it exists
            if (!string.IsNullOrEmpty(description))
            {
                baseDescription += $" {description}";
            }

            return baseDescription;
        }
    }

    private float GetDescriptionValue()
    {
        float value = descriptionField switch
        {
            DescriptionField.AttackBoost => attackBoost,
            DescriptionField.DefenseBoost => defenseBoost,
            DescriptionField.SpeedBoost => speedBoost,
            DescriptionField.HealthBoost => healthBoost,
            DescriptionField.MagicBoost => magicBoost,
            DescriptionField.StaminaBoost => staminaBoost,
            _ => 0, // Handle None or undefined cases
        };
        return value;
    }

    public void Consume(GameObject entity)
    {
        // Apply boosts
        var playerStats = entity.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.Heal(healthBoost);
            playerStats.RefillMagic(magicBoost);
            playerStats.GainStamina(staminaBoost);
        }

        // Add effects
        foreach (var effectType in addedEffects)
        {
            var statusEffectIconLibrary = new StatusEffectIconLibrary();
            var effectPrefab = statusEffectIconLibrary.GetEffectPrefab(effectType);
            if (effectPrefab != null)
            {
                StatusEffectManager.Instance.AddStatusEffect(
                    entity,
                    effectPrefab.GetComponent<StatusEffect>()
                );
            }
            else
            {
                Debug.LogWarning($"Effect prefab for {effectType} not found.");
            }
        }

        // Remove effects
        foreach (var effectType in removedEffects)
        {
            StatusEffectManager.Instance.RemoveSpecificEffect(entity, effectType);
        }
    }

    public enum DescriptionField
    {
        None, // To handle cases where no description field is selected
        AttackBoost,
        DefenseBoost,
        SpeedBoost,
        HealthBoost,
        MagicBoost,
        StaminaBoost,
    }
}
