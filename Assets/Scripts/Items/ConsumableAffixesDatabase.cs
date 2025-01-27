using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class ConsumableAffixesDatabase
    {
        public static List<ConsumablePrefixData> prefixes = new List<ConsumablePrefixData>
        {
            new ConsumablePrefixData
            {
                prefixName = "Lesser",
                imageSize = 0.5f,
                modifierAmount = 0.5f,
            },
            new ConsumablePrefixData { prefixName = "Moderate", imageSize = 1.0f },
            new ConsumablePrefixData
            {
                prefixName = "Greater",
                imageSize = 1.5f,
                modifierAmount = 2f,
                glow = true,
            },
        };

        public static List<ConsumableSuffixData> suffixes = new List<ConsumableSuffixData>
        {
            new ConsumableSuffixData
            {
                suffixName = "of Healing",
                addedEffects = new List<StatusEffectType> { StatusEffectType.Regen },
            },
            new ConsumableSuffixData { suffixName = "of Power", attackBoost = 7 },
            new ConsumableSuffixData { suffixName = "of Speed", speedBoost = 7 },
            new ConsumableSuffixData { suffixName = "of the Bear", defenseBoost = 7 },
            new ConsumableSuffixData { suffixName = "of the Owl", intelligenceBoost = 7 },
            new ConsumableSuffixData { suffixName = "of the Fox", dexterityBoost = 7 },
        };
    }
}
