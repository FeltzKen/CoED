using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public static class MonsterGenerator
    {
        /// <summary>
        /// Returns a new Monster instance for the given floor.
        /// The candidate pool includes monsters from tiers 1 up to floorNumber (capped at 6),
        /// with a 20% chance (if floorNumber < 6) to include monsters from the next tier.
        /// The returned monster is a copy of the chosen template, and its stats are modified additively.
        /// </summary>
        public static Monster GetMonsterForFloor(int floorNumber)
        {
            List<Monster> candidates = new List<Monster>();

            // Determine maximum tier available (tiers are defined from 1 to 6)
            int currentMaxTier = Mathf.Min(floorNumber, 6);

            // Add monsters from all tiers 1 to currentMaxTier.
            for (int tier = 1; tier <= currentMaxTier; tier++)
            {
                candidates.AddRange(GetMonstersFromTier(tier));
            }

            // For floors less than 6, with 20% chance include monsters from tier currentMaxTier+1.
            if (floorNumber < 6)
            {
                if (Random.value < 0.2f)
                {
                    candidates.AddRange(GetMonstersFromTier(currentMaxTier + 1));
                }
            }

            if (candidates.Count == 0)
            {
                Debug.LogWarning("No candidate monsters found for this floor.");
                return null;
            }

            // Pick a random candidate.
            int index = Random.Range(0, candidates.Count);
            Monster template = candidates[index];

            // Create a copy using the copy constructor so that we don't overwrite the base dictionary.
            Monster monsterData = new Monster(template);

            // Determine the monster's level. For example, we use floorNumber * random factor.
            int randomLevel = Mathf.RoundToInt(floorNumber * Random.Range(0.8f, 1.6f));
            randomLevel = Mathf.Clamp(randomLevel, 1, 30);
            monsterData.level = randomLevel;

            // Recalculate the monster's stats for this level additively.
            MonsterInitializer.CalculateMonsterBaseStatsFromLevel(monsterData, monsterData.level);

            return monsterData;
        }

        /// <summary>
        /// Returns the list of monsters for the given tier.
        /// </summary>
        private static List<Monster> GetMonstersFromTier(int tier)
        {
            switch (tier)
            {
                case 1:
                    return MonstersDatabase.tierOneEnemies;
                case 2:
                    return MonstersDatabase.tierTwoEnemies;
                case 3:
                    return MonstersDatabase.tierThreeEnemies;
                case 4:
                    return MonstersDatabase.tierFourEnemies;
                case 5:
                    return MonstersDatabase.tierFiveEnemies;
                case 6:
                    return MonstersDatabase.tierSixEnemies;
                default:
                    return new List<Monster>();
            }
        }
    }
}
