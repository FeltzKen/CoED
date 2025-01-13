using UnityEngine;

public class EquipmentGenerator
{
    public static BaseEquipmentData GenerateRandomItem()
    {
        // Randomly select a weapon as a base item
        BaseEquipmentData baseItem = ItemDatabase.weapons[
            Random.Range(0, ItemDatabase.weapons.Count)
        ];

        // Randomly apply a prefix (basic or greater)
        PrefixData chosenPrefix =
            Random.value < 0.2f
                ? ItemDatabase.greaterPrefixes[Random.Range(0, ItemDatabase.greaterPrefixes.Count)]
                : ItemDatabase.basicPrefixes[Random.Range(0, ItemDatabase.basicPrefixes.Count)];

        // Randomly apply a suffix (basic or greater)
        SuffixData chosenSuffix =
            Random.value < 0.2f
                ? ItemDatabase.greaterSuffixes[Random.Range(0, ItemDatabase.greaterSuffixes.Count)]
                : ItemDatabase.basicSuffixes[Random.Range(0, ItemDatabase.basicSuffixes.Count)];

        Debug.Log(
            $"Generated Item: {chosenPrefix.prefixName} {baseItem.itemName} {chosenSuffix.suffixName}"
        );

        return baseItem;
    }
}
