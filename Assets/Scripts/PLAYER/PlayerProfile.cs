using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class PlayerProfileManager : MonoBehaviour
    {
        public static PlayerProfileManager Instance { get; private set; }

        [Header("Player Profile Settings")]
        [SerializeField]
        private List<int> luckyNumbers = new List<int>();

        [SerializeField]
        private int cursedNumber;

        [SerializeField]
        private int dungeonSeed = 1253;

        public IReadOnlyList<int> LuckyNumbers => luckyNumbers;
        public int CursedNumber => cursedNumber;
        public int DungeonSeed => dungeonSeed;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("PlayerProfileManager: Duplicate instance detected. Destroying duplicate.");
            }
        }

        public void SetLuckyNumbers(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
            {
                Debug.LogWarning("PlayerProfileManager: LuckyNumbers array is null or empty.");
                return;
            }

            luckyNumbers = new List<int>(numbers);
            AssignCursedNumber();
        }

        private void AssignCursedNumber()
        {
            if (luckyNumbers == null || luckyNumbers.Count == 0)
            {
                Debug.LogWarning("PlayerProfileManager: Cannot assign cursed number, luckyNumbers list is empty.");
                return;
            }

            cursedNumber = luckyNumbers[Random.Range(0, luckyNumbers.Count)];
            Debug.Log($"PlayerProfileManager: Cursed Number (Hidden): {cursedNumber}");
        }

        public void SetDungeonSeed(int seed)
        {
            dungeonSeed = seed;
            Debug.Log($"PlayerProfileManager: Dungeon Seed Set: {dungeonSeed}");
        }
    }
}

/*
Changes made:
1. Retained all profile management functionality, as it does not interact directly with the turn or action system.
2. Made sure the script focuses solely on managing player settings and removed redundant components.
3. No direct interaction with PlayerManager or TurnManager is required here.
*/
