using System.Collections.Generic;
using CoED;
using Mono.Cecil;
using UnityEngine;

public static class EquipmentDatabase
{
    // 🔹 Tier One Weapons Database
    public static List<Equipment> tierOneWeapons = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "sword001",
            itemName = "Iron Sword",
            slot = EquipmentSlot.Weapon,
            attack = 10,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Sword"),
        },
        new Equipment
        {
            itemID = "staff001",
            itemName = "Wooden Staff",
            slot = EquipmentSlot.Weapon,
            attack = 2,
            defense = 0,
            magic = 10,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Wooden_Staff"),
        },
        new Equipment
        {
            itemID = "bow001",
            itemName = "light Bow",
            slot = EquipmentSlot.Weapon,
            attack = 8,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 3,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Light_Bow"),
        },
        new Equipment
        {
            itemID = "dagger001",
            itemName = "Steel Dagger",
            slot = EquipmentSlot.Weapon,
            attack = 6,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 7,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Steel_Dagger"),
        },
        new Equipment
        {
            itemID = "axe001",
            itemName = "Light Axe",
            slot = EquipmentSlot.Weapon,
            attack = 13,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Light_Axe"),
        },
        new Equipment
        {
            itemID = "wand001",
            itemName = "Simple Wand",
            slot = EquipmentSlot.Weapon,
            attack = 4,
            defense = 0,
            magic = 15,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Simple_Wand"),
        },
        new Equipment
        {
            itemID = "mace001",
            itemName = "Mace",
            slot = EquipmentSlot.Weapon,
            attack = 12,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 5,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Mace"),
        },
        new Equipment
        {
            itemID = "spear001",
            itemName = "Spear",
            slot = EquipmentSlot.Weapon,
            attack = 9,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Spear"),
        },
    };

    // 🔹 Tier One Armor Database
    public static List<Equipment> tierOneArmor = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "helmet001",
            itemName = "Iron Helmet",
            slot = EquipmentSlot.Head,
            attack = 0,
            defense = 5,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,
            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
        new Equipment
        {
            itemID = "chestplate001",
            itemName = "Iron Chestplate",
            slot = EquipmentSlot.Chest,
            attack = 0,
            defense = 10,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
        new Equipment
        {
            itemID = "leggings001",
            itemName = "Iron Leggings",
            slot = EquipmentSlot.Legs,
            attack = 0,
            defense = 7,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
        new Equipment
        {
            itemID = "gloves001",
            itemName = "Iron Gloves",
            slot = EquipmentSlot.Hands,
            attack = 0,
            defense = 3,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
        new Equipment
        {
            itemID = "shield001",
            itemName = "Iron Shield",
            slot = EquipmentSlot.Shield,
            attack = 0,
            defense = 8,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
        new Equipment
        {
            itemID = "belt001",
            itemName = "Iron Belt",
            slot = EquipmentSlot.Waist,
            attack = 0,
            defense = 4,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
        new Equipment
        {
            itemID = "boots001",
            itemName = "Iron Boots",
            slot = EquipmentSlot.Boots,
            attack = 0,
            defense = 6,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
        },
    };

    // 🔹 Tier One Accessories Database
    public static List<Equipment> tierOneAccessories = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "amulet001",
            itemName = "Iron Amulet",
            slot = EquipmentSlot.Amulet,
            attack = 0,
            defense = 1,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
        },
        new Equipment
        {
            itemID = "ring001",
            itemName = "Iron Ring",
            slot = EquipmentSlot.Ring,
            attack = 0,
            defense = 2,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
        },
        new Equipment
        {
            itemID = "ring002",
            itemName = "Silver Ring",
            slot = EquipmentSlot.Ring,
            attack = 0,
            defense = 3,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
        },
        new Equipment
        {
            itemID = "amulet002",
            itemName = "Silver Amulet",
            slot = EquipmentSlot.Amulet,
            attack = 0,
            defense = 2,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
        },
    };

    // 🔹 Tier Two Weapons
    public static List<Equipment> tierTwoWeapons = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "sword002",
            itemName = "Steel Sword",
            slot = EquipmentSlot.Weapon,
            attack = 20,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
        },
        new Equipment
        {
            itemID = "staff002",
            itemName = "Oak Staff",
            slot = EquipmentSlot.Weapon,
            attack = 5,
            defense = 0,
            magic = 20,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
        },
        new Equipment
        {
            itemID = "bow002",
            itemName = "Long Bow",
            slot = EquipmentSlot.Weapon,
            attack = 16,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 6,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
        },
        new Equipment
        {
            itemID = "dagger002",
            itemName = "Silver Dagger",
            slot = EquipmentSlot.Weapon,
            attack = 12,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 14,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
        },
        new Equipment
        {
            itemID = "axe002",
            itemName = "Battle Axe",
            slot = EquipmentSlot.Weapon,
            attack = 26,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
        },
        new Equipment
        {
            itemID = "wand002",
            itemName = "Enchanted Wand",
            slot = EquipmentSlot.Weapon,
            attack = 8,
            defense = 0,
            magic = 30,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
        },
        new Equipment
        {
            itemID = "mace002",
            itemName = "War Mace",
            slot = EquipmentSlot.Weapon,
            attack = 24,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 10,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "spear002",
            itemName = "Golden Spear",
            slot = EquipmentSlot.Weapon,
            attack = 18,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
    };

    // 🔹 Tier Two Armor
    public static List<Equipment> tierTwoArmor = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "helmet002",
            itemName = "Steel Helmet",
            slot = EquipmentSlot.Head,
            attack = 0,
            defense = 10,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "chestplate002",
            itemName = "Steel Chestplate",
            slot = EquipmentSlot.Chest,
            attack = 0,
            defense = 20,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "leggings002",
            itemName = "Steel Leggings",
            slot = EquipmentSlot.Legs,
            attack = 0,
            defense = 14,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "gloves002",
            itemName = "Steel Gloves",
            slot = EquipmentSlot.Hands,
            attack = 0,
            defense = 6,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "shield002",
            itemName = "Steel Shield",
            slot = EquipmentSlot.Shield,
            attack = 0,
            defense = 16,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "belt002",
            itemName = "Steel Belt",
            slot = EquipmentSlot.Waist,
            attack = 0,
            defense = 8,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "boots002",
            itemName = "Steel Boots",
            slot = EquipmentSlot.Boots,
            attack = 0,
            defense = 12,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
    };

    // 🔹 Tier Two Accessories
    public static List<Equipment> tierTwoAccessories = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "amulet003",
            itemName = "Steel Amulet",
            slot = EquipmentSlot.Amulet,
            attack = 0,
            defense = 3,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "ring003",
            itemName = "Steel Ring",
            slot = EquipmentSlot.Ring,
            attack = 0,
            defense = 4,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "ring004",
            itemName = "Gold Ring",
            slot = EquipmentSlot.Ring,
            attack = 0,
            defense = 5,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "amulet004",
            itemName = "Gold Amulet",
            slot = EquipmentSlot.Amulet,
            attack = 0,
            defense = 4,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
    };

    // 🔹 Tier Three Weapons
    public static List<Equipment> tierThreeWeapons = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "sword003",
            itemName = "Titanium Sword",
            slot = EquipmentSlot.Weapon,
            attack = 30,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "staff003",
            itemName = "Crystal Staff",
            slot = EquipmentSlot.Weapon,
            attack = 10,
            defense = 0,
            magic = 30,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "bow003",
            itemName = "Recurve Bow",
            slot = EquipmentSlot.Weapon,
            attack = 24,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 9,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "dagger003",
            itemName = "Platinum Dagger",
            slot = EquipmentSlot.Weapon,
            attack = 18,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 21,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "axe003",
            itemName = "War Axe",
            slot = EquipmentSlot.Weapon,
            attack = 39,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "wand003",
            itemName = "Mystic Wand",
            slot = EquipmentSlot.Weapon,
            attack = 12,
            defense = 0,
            magic = 45,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "mace003",
            itemName = "Heavy Mace",
            slot = EquipmentSlot.Weapon,
            attack = 36,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 15,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "spear003",
            itemName = "Dragon Spear",
            slot = EquipmentSlot.Weapon,
            attack = 27,
            defense = 0,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
    };

    // 🔹 Tier Three Armor
    public static List<Equipment> tierThreeArmor = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "helmet003",
            itemName = "Titanium Helmet",
            slot = EquipmentSlot.Head,
            attack = 0,
            defense = 15,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "chestplate003",
            itemName = "Titanium Chestplate",
            slot = EquipmentSlot.Chest,
            attack = 0,
            defense = 30,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "leggings003",
            itemName = "Titanium Leggings",
            slot = EquipmentSlot.Legs,
            attack = 0,
            defense = 21,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "gloves003",
            itemName = "Titanium Gloves",
            slot = EquipmentSlot.Hands,
            attack = 0,
            defense = 9,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "shield003",
            itemName = "Titanium Shield",
            slot = EquipmentSlot.Shield,
            attack = 0,
            defense = 24,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "belt003",
            itemName = "Titanium Belt",
            slot = EquipmentSlot.Waist,
            attack = 0,
            defense = 12,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "boots003",
            itemName = "Titanium Boots",
            slot = EquipmentSlot.Boots,
            attack = 0,
            defense = 18,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
    };

    // 🔹 Tier Three Accessories
    public static List<Equipment> tierThreeAccessories = new List<Equipment>()
    {
        new Equipment
        {
            itemID = "amulet005",
            itemName = "Titanium Amulet",
            slot = EquipmentSlot.Amulet,
            attack = 0,
            defense = 5,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "ring005",
            itemName = "Titanium Ring",
            slot = EquipmentSlot.Ring,
            attack = 0,
            defense = 6,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "ring006",
            itemName = "Platinum Ring",
            slot = EquipmentSlot.Ring,
            attack = 0,
            defense = 7,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
        new Equipment
        {
            itemID = "amulet006",
            itemName = "Platinum Amulet",
            slot = EquipmentSlot.Amulet,
            attack = 0,
            defense = 6,
            magic = 0,
            health = 0,
            stamina = 0,
            intelligence = 0,
            dexterity = 0,
            speed = 0,
            critChance = 0,

            isOneTimeEffect = false,
            baseSprite = Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Fury"),
        },
    };
}
