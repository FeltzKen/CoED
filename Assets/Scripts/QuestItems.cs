using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class QuestItem : Item
    {
        [Header("Quest Item Details")]
        [SerializeField]
        private string loreDescription;

        [SerializeField]
        private string _itemName;

        [SerializeField]
        private bool isKeyItem;

        [Header("Stat Modifiers")]
        [SerializeField]
        private int attackModifier;

        [SerializeField]
        private int defenseModifier;

        [SerializeField]
        private int healthModifier;

        [SerializeField]
        private float speedModifier;

        [SerializeField]
        private ItemRarity rarity;

        public string LoreDescription => loreDescription;
        public bool IsKeyItem => isKeyItem;

        public int AttackModifier => attackModifier;
        public int DefenseModifier => defenseModifier;
        public int HealthModifier => healthModifier;
        public float SpeedModifier => speedModifier;
        public ItemRarity Rarity => rarity; // Getter for rarity

        // Constructor to initialize the QuestItem
        public QuestItem(
            string itemName,
            string loreDescription,
            bool isKeyItem,
            ItemRarity rarity,
            int attackModifier,
            int defenseModifier,
            int healthModifier,
            float speedModifier
        )
        {
            this.itemName = itemName; // Set itemName using the parameter
            this.loreDescription = loreDescription;
            this.isKeyItem = isKeyItem;
            this.rarity = rarity;
            this.attackModifier = attackModifier;
            this.defenseModifier = defenseModifier;
            this.healthModifier = healthModifier;
            this.speedModifier = speedModifier;
        }
    }

    public static class QuestItemDatabase
    {
        public static QuestItem AncientDragonGem =>
            new QuestItem(
                "Ancient Dragon Gem",
                "A radiant gem said to hold the essence of an ancient dragon. Crucial for unlocking the Dragon Gate.",
                true,
                ItemRarity.Legendary,
                15,
                10,
                50,
                0
            );

        public static QuestItem PhoenixFeather =>
            new QuestItem(
                "Phoenix Feather",
                "A rare feather from a mystical phoenix, capable of reviving the fallen and granting great power.",
                true,
                ItemRarity.Epic,
                0,
                0,
                100,
                0
            );

        public static QuestItem MysticalChalice =>
            new QuestItem(
                "Mystical Chalice",
                "An ancient chalice rumored to bestow everlasting wisdom to those deemed worthy.",
                false,
                ItemRarity.Rare,
                5,
                0,
                25,
                0
            );

        public static QuestItem SwordOfEternalFlame =>
            new QuestItem(
                "Sword of Eternal Flame",
                "A blazing sword imbued with the fire of an ancient phoenix, capable of burning away the darkness.",
                true,
                ItemRarity.Legendary,
                30,
                0,
                0,
                0
            );

        public static QuestItem AegisOfTheForgotten =>
            new QuestItem(
                "Aegis of the Forgotten",
                "A legendary shield said to protect its wielder from even the most formidable foes, lost to time.",
                true,
                ItemRarity.Legendary,
                0,
                25,
                75,
                0
            );

        public static QuestItem ArmorOfTheCelestialGuardian =>
            new QuestItem(
                "Armor of the Celestial Guardian",
                "An ethereal armor forged by celestial beings, offering unmatched protection and resilience against dark forces.",
                true,
                ItemRarity.Legendary,
                0,
                40,
                100,
                0
            );

        public static QuestItem HelmOfTheEternalWatch =>
            new QuestItem(
                "Helm of the Eternal Watch",
                "A mystical helm that grants the wearer enhanced perception and the ability to foresee incoming dangers.",
                false,
                ItemRarity.Epic,
                0,
                15,
                25,
                0
            );

        public static QuestItem BootsOfTheWindWalker =>
            new QuestItem(
                "Boots of the Wind Walker",
                "Lightweight boots that grant the wearer incredible speed and agility, as if they were running on the wind itself.",
                false,
                ItemRarity.Rare,
                0,
                0,
                0,
                1.5f
            );

        public static List<QuestItem> GetAllQuestItems()
        {
            return new List<QuestItem>
            {
                AncientDragonGem,
                PhoenixFeather,
                MysticalChalice,
                SwordOfEternalFlame,
                AegisOfTheForgotten,
                ArmorOfTheCelestialGuardian,
                HelmOfTheEternalWatch,
                BootsOfTheWindWalker,
            };
        }
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
    }
}
