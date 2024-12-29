using UnityEngine;

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

        public int CalculateHealthAtLevel(int level) => BaseHealth + (level * 10);

        public int CalculateStrengthAtLevel(int level) => BaseStrength + (level * 2);

        public int CalculateDefenseAtLevel(int level) => BaseDefense + (level * 2);

        public float CalculateCriticalChanceAtLevel(int level) =>
            BaseCriticalChance + (level * 0.01f);
    }
}
