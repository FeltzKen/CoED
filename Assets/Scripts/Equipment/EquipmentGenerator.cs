using CoED;
using UnityEngine;

public class EquipmentGenerator
{
    /// <summary>
    /// Generates random equipment based on tier and equipment type.
    /// </summary>
    /// <param name="tier">Tier level: 1, 2, or 3</param>
    /// <param name="equipmentType">"Weapon", "Armor", or "Accessory"</param>
    /// <returns>Generated Equipment with affixes applied.</returns>
    public static Equipment GenerateRandomEquipment(int tier, string equipmentType)
    {
        Equipment baseEquipment = GetBaseEquipment(tier, equipmentType);
        Equipment generatedEquipment = new Equipment();

        InitializeEquipment(generatedEquipment, baseEquipment);

        // Base chances for enchantment and curse
        float baseEnchantmentChance = 0.2f; // 20% at tier 1
        float baseCurseChance = 0.05f; // 5% at tier 1

        // Scale enchantment and curse chances inversely based on tier
        float enchantmentChance = Mathf.Clamp(baseEnchantmentChance - (tier * 0.1f), 0.1f, 0.5f); // Min 10%, max 50%
        float curseChance = Mathf.Clamp(baseCurseChance + (tier * 0.15f), 0.1f, 0.5f); // Min 10%, max 50%

        // Debug the probabilities (optional)
        Debug.Log(
            $"Tier {tier} - Enchantment Chance: {enchantmentChance * 100:F2}% | Curse Chance: {curseChance * 100:F2}%"
        );

        // Roll for enchantment or curse
        if (RollChance(enchantmentChance))
        {
            // Apply enchantment
            generatedEquipment.prePrefix = EquipmentAffixesDatabase.pre_prefixes[0]; // "Enchanted"
            generatedEquipment.isEnchantedOrCursed = true;
            Debug.Log("Enchanted item created!");
        }
        else if (RollChance(enchantmentChance + curseChance))
        {
            // Apply curse
            generatedEquipment.prePrefix = EquipmentAffixesDatabase.pre_prefixes[1]; // "Cursed"
            generatedEquipment.isEnchantedOrCursed = true;
            Debug.Log("Cursed item created!");
        }
        else
        {
            Debug.Log("Normal item created!");
        }

        // Apply Prefix
        if (RollChance(0.2f))
        {
            PrefixData prefix = EquipmentAffixesDatabase.basicPrefixes[
                Random.Range(0, EquipmentAffixesDatabase.basicPrefixes.Count)
            ];

            if (RollChance(0.1f)) // 10% chance to upgrade
            {
                prefix = EquipmentAffixesDatabase.greaterPrefixes[
                    Random.Range(0, EquipmentAffixesDatabase.greaterPrefixes.Count)
                ];
            }

            generatedEquipment.prefix = prefix;
        }

        // Apply Suffix
        if (RollChance(0.2f))
        {
            SuffixData suffix = EquipmentAffixesDatabase.basicSuffixes[
                Random.Range(0, EquipmentAffixesDatabase.basicSuffixes.Count)
            ];

            if (RollChance(0.1f)) // 10% chance to upgrade
            {
                suffix = EquipmentAffixesDatabase.greaterSuffixes[
                    Random.Range(0, EquipmentAffixesDatabase.greaterSuffixes.Count)
                ];
            }

            generatedEquipment.suffix = suffix;
        }

        // Apply affix effects to stats and update item name
        generatedEquipment.ApplyAffixes();

        Debug.Log($"Generated Item: {generatedEquipment.itemName}");

        return generatedEquipment;
    }

    /// <summary>
    /// Retrieves a random base equipment based on tier and type.
    /// </summary>
    private static Equipment GetBaseEquipment(int tier, string equipmentType)
    {
        switch (equipmentType.ToLower())
        {
            case "weapon":
                return tier switch
                {
                    1 => EquipmentDatabase.tierOneWeapons[
                        Random.Range(0, EquipmentDatabase.tierOneWeapons.Count)
                    ],
                    2 => EquipmentDatabase.tierTwoWeapons[
                        Random.Range(0, EquipmentDatabase.tierTwoWeapons.Count)
                    ],
                    3 => EquipmentDatabase.tierThreeWeapons[
                        Random.Range(0, EquipmentDatabase.tierThreeWeapons.Count)
                    ],
                    _ => null,
                };

            case "armor":
                return tier switch
                {
                    1 => EquipmentDatabase.tierOneArmor[
                        Random.Range(0, EquipmentDatabase.tierOneArmor.Count)
                    ],
                    2 => EquipmentDatabase.tierTwoArmor[
                        Random.Range(0, EquipmentDatabase.tierTwoArmor.Count)
                    ],
                    3 => EquipmentDatabase.tierThreeArmor[
                        Random.Range(0, EquipmentDatabase.tierThreeArmor.Count)
                    ],
                    _ => null,
                };

            case "accessory":
                return tier switch
                {
                    1 => EquipmentDatabase.tierOneAccessories[
                        Random.Range(0, EquipmentDatabase.tierOneAccessories.Count)
                    ],
                    2 => EquipmentDatabase.tierTwoAccessories[
                        Random.Range(0, EquipmentDatabase.tierTwoAccessories.Count)
                    ],
                    3 => EquipmentDatabase.tierThreeAccessories[
                        Random.Range(0, EquipmentDatabase.tierThreeAccessories.Count)
                    ],
                    _ => null,
                };

            default:
                Debug.LogError("Invalid equipment type.");
                return null;
        }
    }

    /// <summary>
    /// Initializes a new Equipment instance based on base data.
    /// </summary>
    private static void InitializeEquipment(Equipment generatedEquipment, Equipment baseEquipment)
    {
        generatedEquipment.InitializeEquipment(
            baseEquipment.itemID,
            baseEquipment.itemName,
            baseEquipment.slot,
            baseEquipment.rarity,
            baseEquipment.baseSprite,
            baseEquipment.attack,
            baseEquipment.defense,
            baseEquipment.magic,
            baseEquipment.health,
            baseEquipment.stamina,
            baseEquipment.intelligence,
            baseEquipment.dexterity,
            baseEquipment.speed,
            baseEquipment.critChance,
            baseEquipment.damageModifiers,
            baseEquipment.resistanceEffects,
            baseEquipment.weaknessEffects,
            baseEquipment.isOneTimeEffect,
            baseEquipment.effectUsed,
            baseEquipment.activeStatusEffects,
            baseEquipment.inflictedStatusEffects
        );
    }

    /// <summary>
    /// Rolls a chance based on the given probability.
    /// </summary>
    private static bool RollChance(float chance)
    {
        return Random.value < chance;
    }
}
