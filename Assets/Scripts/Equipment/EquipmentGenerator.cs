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
    public static Equipment GenerateRandomEquipment(int tier)
    {
        // Randomly pick an equipment type
        string[] possibleTypes = { "Weapon", "Armor", "Accessory" };
        string chosenType = possibleTypes[Random.Range(0, possibleTypes.Length)];

        // Grab a random base equipment using the chosen type
        Equipment baseEquipment = GetBaseEquipment(tier, chosenType);
        if (baseEquipment == null)
        {
            Debug.LogError(
                $"Failed to retrieve base equipment for tier={tier}, type={chosenType}."
            );
            return null;
        }

        // Create a new Equipment object and initialize it with the base stats
        Equipment generatedEquipment = new Equipment();
        InitializeEquipment(generatedEquipment, baseEquipment);

        // Base chances for enchantment and curse
        float baseEnchantmentChance = 0.2f; // 20% at tier 1
        float baseCurseChance = 0.05f; // 5% at tier 1

        // Scale enchantment and curse chances inversely based on tier
        float enchantmentChance = Mathf.Clamp(baseEnchantmentChance - (tier * 0.1f), 0.1f, 0.5f);
        float curseChance = Mathf.Clamp(baseCurseChance + (tier * 0.15f), 0.1f, 0.5f);

        // Roll for enchantment or curse
        if (RollChance(enchantmentChance))
        {
            // Apply enchantment
            generatedEquipment.prePrefix = EquipmentAffixesDatabase.pre_prefixes[0]; // "Enchanted"
            generatedEquipment.isEnchantedOrCursed = true;
        }
        else if (RollChance(enchantmentChance + curseChance))
        {
            // Apply curse
            generatedEquipment.prePrefix = EquipmentAffixesDatabase.pre_prefixes[1]; // "Cursed"
            generatedEquipment.isEnchantedOrCursed = true;
        }

        // Apply Prefix
        if (RollChance(0.2f))
        {
            EquipmentPrefixData prefix = EquipmentAffixesDatabase.basicPrefixes[
                Random.Range(0, EquipmentAffixesDatabase.basicPrefixes.Count)
            ];

            // 10% chance to upgrade to a greater prefix
            if (RollChance(0.1f))
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
            EquipmentSuffixData suffix = EquipmentAffixesDatabase.basicSuffixes[
                Random.Range(0, EquipmentAffixesDatabase.basicSuffixes.Count)
            ];

            // 10% chance to upgrade to a greater suffix
            if (RollChance(0.1f))
            {
                suffix = EquipmentAffixesDatabase.greaterSuffixes[
                    Random.Range(0, EquipmentAffixesDatabase.greaterSuffixes.Count)
                ];
            }

            generatedEquipment.suffix = suffix;
        }

        // Apply final stat modifications / naming changes based on affixes
        generatedEquipment.ApplyAffixes();

        return generatedEquipment;
    }

    public static Equipment GenerateShopEquipment(int tier)
    {
        Equipment finalItem = null;

        // Safety check: try multiple times in case the random rolls keep giving "Cursed"
        for (int attempts = 0; attempts < 100; attempts++)
        {
            // 2) Generate a random piece of equipment for the specified tier and random type
            Equipment candidate = GenerateRandomEquipment(tier);

            // 3) Check if it's cursed by looking at candidate.prePrefix
            //    (assuming pre_prefixes[1] = "Cursed" in EquipmentAffixesDatabase)
            if (candidate.itemName.Contains("Cursed"))
            {
                // Skip cursed items entirely; try again
                continue;
            }

            // 4) If it's enchanted (prePrefix == "Enchanted"), reveal it so the shop sells it fully identified
            //    (assuming pre_prefixes[0] = "Enchanted")
            if (candidate.itemName.Contains("Enchanted"))
            {
                candidate.RevealHiddenAttributes();
            }

            // 5) Use this item
            finalItem = candidate;
            break;
        }

        // If we somehow failed to find a non-cursed item after 100 attempts, finalItem stays null.
        // Return whatever we got; might be null if repeated curses, which you can handle if needed.
        return finalItem;
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
