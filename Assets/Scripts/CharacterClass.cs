// CharacterClass.cs
using UnityEngine;
using CoED;

namespace CoED
{
    [CreateAssetMenu(fileName = "CharacterClass", menuName = "Character/CharacterClass")]
    public class CharacterClass : ScriptableObject
    {
        [Header("Basic Attributes")]
        public string ClassName;
        public Sprite CharacterSprite;
        public int BaseHealth;
        public int BaseStrength;
        public int BaseMagic;
        public int BaseDefense;

        [Header("Additional Attributes")]
        public int BaseSpeed = 5;
        public int BaseMana = 100;
        public float BaseCriticalChance = 0.05f; // 5% default chance
        public string SpecialAbilityDescription;

        /// <summary>
        /// Calculates health at the given level.
        /// </summary>
        public int CalculateHealthAtLevel(int level) => BaseHealth + (level * 10);

        /// <summary>
        /// Calculates strength at the given level.
        /// </summary>
        public int CalculateStrengthAtLevel(int level) => BaseStrength + (level * 2);

        /// <summary>
        /// Calculates defense at the given level.
        /// </summary>
        public int CalculateDefenseAtLevel(int level) => BaseDefense + (level * 2);

        /// <summary>
        /// Calculates critical chance at the given level.
        /// </summary>
        public float CalculateCriticalChanceAtLevel(int level) =>
            BaseCriticalChance + (level * 0.01f);
    }
}
