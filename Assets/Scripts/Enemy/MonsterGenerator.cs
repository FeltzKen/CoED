using System.Linq;
using UnityEngine;

namespace CoED
{
    public static class MonsterGenerator
    {
        /// <summary>
        /// Returns a random Monster from MonstersDatabase.
        /// Returns null if there are no monsters.
        /// </summary>
        public static Monster GetRandomMonster()
        {
            if (MonstersDatabase.monsters.Count == 0)
            {
                Debug.LogWarning("No monsters found in MonstersDatabase!");
                return null;
            }
            int index = Random.Range(0, MonstersDatabase.monsters.Count);
            return MonstersDatabase.monsters[index];
        }

        /// <summary>
        /// Returns a random MiniBoss from MiniBossesDatabase.
        /// Returns null if there are no mini bosses.
        /// </summary>
        public static MiniBoss GetRandomMiniBoss()
        {
            if (MiniBossesDatabase.miniBosses.Count == 0)
            {
                Debug.LogWarning("No mini bosses found in MiniBossesDatabase!");
                return null;
            }
            int index = Random.Range(0, MiniBossesDatabase.miniBosses.Count);
            return MiniBossesDatabase.miniBosses[index];
        }

        /// <summary>
        /// Returns the FinalBoss with the given name from FinalBossesDatabase.
        /// Returns null if not found.
        /// </summary>
        public static FinalBoss GetFinalBoss(string finalBossName)
        {
            return FinalBossesDatabase.finalBosses.FirstOrDefault(f =>
                f.name.Equals(finalBossName, System.StringComparison.OrdinalIgnoreCase)
            );
        }

        /// <summary>
        /// Returns a random FinalBoss from FinalBossesDatabase.
        /// Returns null if there are no final bosses.
        /// </summary>
        public static FinalBoss GetRandomFinalBoss()
        {
            if (FinalBossesDatabase.finalBosses.Count == 0)
            {
                Debug.LogWarning("No final bosses found in FinalBossesDatabase!");
                return null;
            }
            int index = Random.Range(0, FinalBossesDatabase.finalBosses.Count);
            return FinalBossesDatabase.finalBosses[index];
        }
    }
}
