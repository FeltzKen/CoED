using System.Collections.Generic;
using CoED;
using UnityEngine;

public static class EquipmentDatabase
{
    // 🔹 Tier One Weapons Database
    public static List<Equipment> tierOneWeapons = new List<Equipment>()
    {
        new Equipment(
            itemID: "sword001",
            itemName: "Iron Sword",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Sword")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 4 }
        ),
        new Equipment(
            itemID: "staff001",
            itemName: "Wooden Staff",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Wooden_Staff")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 2, [Stat.MaxMagic] = 5 }
        ),
        new Equipment(
            itemID: "bow001",
            itemName: "Light Bow",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Light_Bow")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Attack] = 3,
                [Stat.CritChance] = 0.3f,
            }
        ),
        new Equipment(
            itemID: "dagger001",
            itemName: "Steel Dagger",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Steel_Dagger")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Attack] = 5,
                [Stat.CritChance] = 0.7f,
            }
        ),
        new Equipment(
            itemID: "axe001",
            itemName: "Light Axe",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Light_Axe")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 6 }
        ),
        new Equipment(
            itemID: "wand001",
            itemName: "Simple Wand",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Simple_Wand")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 2, [Stat.MaxMagic] = 7 }
        ),
        new Equipment(
            itemID: "mace001",
            itemName: "Mace",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Mace")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Attack] = 5,
                [Stat.CritChance] = 0.5f,
            }
        ),
        new Equipment(
            itemID: "spear001",
            itemName: "Spear",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common, // or whatever
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Spear")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 4 }
        ),
    };

    // 🔹 Tier One Armor Database
    public static List<Equipment> tierOneArmor = new List<Equipment>()
    {
        new Equipment(
            itemID: "helmet001",
            itemName: "Iron Helmet",
            slot: EquipmentSlot.Head,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Helmet")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 5 }
        ),
        new Equipment(
            itemID: "chestplate001",
            itemName: "Iron Chestplate",
            slot: EquipmentSlot.Chest,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Chestplate")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 10 }
        ),
        new Equipment(
            itemID: "leggings001",
            itemName: "Iron Leggings",
            slot: EquipmentSlot.Legs,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Leggings")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 7 }
        ),
        new Equipment(
            itemID: "gloves001",
            itemName: "Iron Gloves",
            slot: EquipmentSlot.Hands,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Gloves")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 3 }
        ),
        new Equipment(
            itemID: "shield001",
            itemName: "Iron Shield",
            slot: EquipmentSlot.Shield,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Shield")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 8 }
        ),
        new Equipment(
            itemID: "belt001",
            itemName: "Iron Belt",
            slot: EquipmentSlot.Waist,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Belt")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 4 }
        ),
        new Equipment(
            itemID: "boots001",
            itemName: "Iron Boots",
            slot: EquipmentSlot.Boots,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Boots")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 6 }
        ),
    };

    // 🔹 Tier One Accessories Database
    public static List<Equipment> tierOneAccessories = new List<Equipment>()
    {
        new Equipment(
            itemID: "amulet001",
            itemName: "Iron Amulet",
            slot: EquipmentSlot.Amulet,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Iron_Amulet")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Defense] = 3,
                [Stat.Intelligence] = 3,
            }
        ),
        new Equipment(
            itemID: "ring001",
            itemName: "Iron Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Iron_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 2, [Stat.Dexterity] = 2 }
        ),
        new Equipment(
            itemID: "ring002",
            itemName: "Silver Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Silver_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 4, [Stat.Dexterity] = 3 }
        ),
        new Equipment(
            itemID: "amulet002",
            itemName: "Silver Amulet",
            slot: EquipmentSlot.Amulet,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Silver_Amulet")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Defense] = 3,
                [Stat.Intelligence] = 4,
            }
        ),
    };

    // 🔹 Tier Two Weapons
    public static List<Equipment> tierTwoWeapons = new List<Equipment>()
    {
        new Equipment(
            itemID: "sword002",
            itemName: "Steel Sword",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Steel_Sword")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 6 }
        ),
        new Equipment(
            itemID: "staff002",
            itemName: "Oak Staff",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Oak_Staff")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 9, [Stat.MaxMagic] = 15 }
        ),
        new Equipment(
            itemID: "bow002",
            itemName: "Long Bow",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Long_Bow")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Attack] = 12,
                [Stat.CritChance] = 0.6f,
            }
        ),
        new Equipment(
            itemID: "dagger002",
            itemName: "Silver Dagger",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Silver_Dagger")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Attack] = 12,
                [Stat.CritChance] = 0.15f,
            }
        ),
        new Equipment(
            itemID: "axe002",
            itemName: "Battle Axe",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Battle_Axe")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 17 }
        ),
        new Equipment(
            itemID: "wand002",
            itemName: "Glowing Wand",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Glowing_Wand")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 6, [Stat.MaxMagic] = 25 }
        ),
        new Equipment(
            itemID: "mace002",
            itemName: "War Mace",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/War_Mace")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                [Stat.Attack] = 20,
                [Stat.CritChance] = 0.3f,
            }
        ),
        new Equipment(
            itemID: "spear002",
            itemName: "Golden Spear",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Golden_Spear")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Attack] = 15, [Stat.Accuracy] = 7 }
        ),
    };

    // 🔹 Tier Two Armor
    public static List<Equipment> tierTwoArmor = new List<Equipment>()
    {
        new Equipment(
            itemID: "helmet002",
            itemName: "Steel Helmet",
            slot: EquipmentSlot.Head,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Helmet")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 10 }
        ),
        new Equipment(
            itemID: "chestplate002",
            itemName: "Steel Chestplate",
            slot: EquipmentSlot.Chest,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Chestplate")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 20 }
        ),
        new Equipment(
            itemID: "leggings002",
            itemName: "Steel Leggings",
            slot: EquipmentSlot.Legs,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Leggings")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 14 }
        ),
        new Equipment(
            itemID: "gloves002",
            itemName: "Steel Gloves",
            slot: EquipmentSlot.Hands,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Gloves")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 6 }
        ),
        new Equipment(
            itemID: "shield002",
            itemName: "Steel Shield",
            slot: EquipmentSlot.Shield,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Shield")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 16 }
        ),
        new Equipment(
            itemID: "belt002",
            itemName: "Steel Belt",
            slot: EquipmentSlot.Waist,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Belt")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 8 }
        ),
        new Equipment(
            itemID: "boots002",
            itemName: "Steel Boots",
            slot: EquipmentSlot.Boots,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Boots")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { [Stat.Defense] = 12 }
        ),
    };

    // 🔹 Tier Two Accessories
    public static List<Equipment> tierTwoAccessories = new List<Equipment>()
    {
        new Equipment(
            itemID: "amulet003",
            itemName: "Steel Amulet",
            slot: EquipmentSlot.Amulet,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Steel_Amulet")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 4 },
                { Stat.Intelligence, 4 },
            }
        ),
        new Equipment(
            itemID: "ring003",
            itemName: "Steel Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Steel_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 7 },
                { Stat.Dexterity, 5 },
            }
        ),
        new Equipment(
            itemID: "ring004",
            itemName: "Gold Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Gold_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 8 },
                { Stat.Dexterity, 6 },
            }
        ),
        new Equipment(
            itemID: "amulet004",
            itemName: "Gold Amulet",
            slot: EquipmentSlot.Amulet,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Gold_Amulet")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 6 },
                { Stat.Intelligence, 6 },
            }
        ),
    };

    // 🔹 Tier Three Weapons
    public static List<Equipment> tierThreeWeapons = new List<Equipment>()
    {
        new Equipment(
            itemID: "sword003",
            itemName: "Titanium Sword",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Titanium_Sword")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 22 } }
        ),
        new Equipment(
            itemID: "staff003",
            itemName: "Crystal Staff",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Crystal_Staff")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 18 },
                { Stat.MaxMagic, 35 },
            }
        ),
        new Equipment(
            itemID: "bow003",
            itemName: "Recurve Bow",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Recurve_Bow")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 24 },
                { Stat.CritChance, 0.45f },
            }
        ),
        new Equipment(
            itemID: "dagger003",
            itemName: "Platinum Dagger",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Platinum_Dagger")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 18 },
                { Stat.CritChance, 0.4f },
            }
        ),
        new Equipment(
            itemID: "axe003",
            itemName: "War Axe",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/War_Axe")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 25 } }
        ),
        new Equipment(
            itemID: "wand003",
            itemName: "Mystic Wand",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Mystic_Wand")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 15 },
                { Stat.MaxMagic, 45 },
            }
        ),
        new Equipment(
            itemID: "mace003",
            itemName: "Heavy Mace",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Heavy_Mace")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 27 },
                { Stat.CritChance, 0.5f },
                { Stat.Speed, -2 },
            }
        ),
        new Equipment(
            itemID: "spear003",
            itemName: "Dragon Spear",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Dragon_Spear")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 20 },
                { Stat.Accuracy, 10 },
            }
        ),
    };

    // 🔹 Tier Three Armor
    public static List<Equipment> tierThreeArmor = new List<Equipment>()
    {
        new Equipment(
            itemID: "helmet003",
            itemName: "Titanium Helmet",
            slot: EquipmentSlot.Head,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Helmet")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 15 } }
        ),
        new Equipment(
            itemID: "chestplate003",
            itemName: "Titanium Chestplate",
            slot: EquipmentSlot.Chest,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Chestplate")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 22 } }
        ),
        new Equipment(
            itemID: "leggings003",
            itemName: "Titanium Leggings",
            slot: EquipmentSlot.Legs,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Leggings")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 18 } }
        ),
        new Equipment(
            itemID: "gloves003",
            itemName: "Titanium Gloves",
            slot: EquipmentSlot.Hands,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Gloves")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 12 } }
        ),
        new Equipment(
            itemID: "shield003",
            itemName: "Titanium Shield",
            slot: EquipmentSlot.Shield,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Shield")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 24 } }
        ),
        new Equipment(
            itemID: "belt003",
            itemName: "Titanium Belt",
            slot: EquipmentSlot.Waist,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Belt")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 12 } }
        ),
        new Equipment(
            itemID: "boots003",
            itemName: "Titanium Boots",
            slot: EquipmentSlot.Boots,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Boots")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 16 } }
        ),
    };

    // 🔹 Tier Three Accessories
    public static List<Equipment> tierThreeAccessories = new List<Equipment>()
    {
        new Equipment(
            itemID: "amulet005",
            itemName: "Titanium Amulet",
            slot: EquipmentSlot.Amulet,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Titanium_Amulet")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 8 },
                { Stat.Intelligence, 8 },
            }
        ),
        new Equipment(
            itemID: "ring005",
            itemName: "Titanium Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Titanium_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 8 },
                { Stat.Dexterity, 9 },
            }
        ),
        new Equipment(
            itemID: "ring006",
            itemName: "Platinum Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Platinum_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 10 },
                { Stat.Dexterity, 9 },
            }
        ),
        new Equipment(
            itemID: "amulet006",
            itemName: "Platinum Amulet",
            slot: EquipmentSlot.Amulet,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Platinum_Amulet")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 9 },
                { Stat.Intelligence, 9 },
            }
        ),
    };
}
