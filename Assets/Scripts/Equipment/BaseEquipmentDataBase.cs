using System.Collections.Generic;
using CoED;
using UnityEngine;

public static class EquipmentDatabase
{
    // 🔹 Tier One Weapons Database
    public static List<Equipment> tierOneWeapons = new List<Equipment>()
    {
        // Iron Sword: a balanced physical weapon with added accuracy.
        new Equipment(
            itemID: "sword001",
            itemName: "Iron Sword",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Iron_Sword")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 4 }, { Stat.Accuracy, 2 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Physical, 2 } },
            resistanceEffects = new List<Resistances> { Resistances.Physical },
            weaknessEffects = new List<Weaknesses> { Weaknesses.Fire },
        },
        // Flame Dagger: a quick dagger with fire bonus and a burn chance.
        new Equipment(
            itemID: "dagger002",
            itemName: "Flame Dagger",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Steel_Dagger")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 3 },
                { Stat.CritChance, 0.25f },
            }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Fire, 3 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Burn },
        },
        // Wooden Staff: supports magic with a moderate boost to MaxMagic.
        new Equipment(
            itemID: "staff001",
            itemName: "Wooden Staff",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Wooden_Staff")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 2 }, { Stat.MaxMagic, 5 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Arcane, 2 } },
            resistanceEffects = new List<Resistances> { Resistances.Arcane },
        },
        // Brittlebow: a lightweight bow with increased crit chance and a poison twist.
        new Equipment(
            itemID: "bow001",
            itemName: "Brittle Bow",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Light_Bow")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 3 },
                { Stat.CritChance, 0.3f },
            }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Poison, 2 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Poison },
        },
        // Rusty Axe: a heavier but imperfect weapon with a slight crit penalty.
        new Equipment(
            itemID: "axe001",
            itemName: "Rusty Axe",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Light_Axe")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 5 },
                { Stat.CritChance, -0.1f },
            }
        )
        {
            weaknessEffects = new List<Weaknesses> { Weaknesses.Physical },
        },
        // Simple Wand: a basic magic conduit with an arcane bonus.
        new Equipment(
            itemID: "wand001",
            itemName: "Simple Wand",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Simple_Wand")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 2 }, { Stat.MaxMagic, 7 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Arcane, 4 } },
        },
        // Heavy Mace: a blunt weapon with added defense.
        new Equipment(
            itemID: "mace001",
            itemName: "Heavy Mace",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Mace")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 5 },
                { Stat.CritChance, 0.5f },
                { Stat.Defense, 2 },
            }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Physical },
        },
        // Spear: a balanced polearm with enhanced accuracy.
        new Equipment(
            itemID: "spear001",
            itemName: "Spear",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Spear")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 4 }, { Stat.Accuracy, 3 } }
        ),
    };

    // 🔹 Tier One Armor Database
    public static List<Equipment> tierOneArmor = new List<Equipment>()
    {
        // Iron Helmet: basic headgear that adds a touch of accuracy.
        new Equipment(
            itemID: "helmet001",
            itemName: "Iron Helmet",
            slot: EquipmentSlot.Head,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Helmet")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 5 }, { Stat.Accuracy, 1 } }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Physical },
        },
        // Iron Chestplate: heavy torso armor with a slight speed penalty.
        new Equipment(
            itemID: "chestplate001",
            itemName: "Iron Chestplate",
            slot: EquipmentSlot.Chest,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Chestplate")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 10 },
                { Stat.Speed, -0.5f },
            }
        )
        {
            immunityEffects = new List<Immunities> { Immunities.Poison },
        },
        // Iron Leggings: offers balanced protection with a bonus to speed.
        new Equipment(
            itemID: "leggings001",
            itemName: "Iron Leggings",
            slot: EquipmentSlot.Legs,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Leggings")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 7 }, { Stat.Speed, 1 } }
        ),
        // Iron Gloves: light armor that improves dexterity.
        new Equipment(
            itemID: "gloves001",
            itemName: "Iron Gloves",
            slot: EquipmentSlot.Hands,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Gloves")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 3 },
                { Stat.Dexterity, 2 },
            }
        ),
        // Iron Shield: offers solid defense and physical resistance.
        new Equipment(
            itemID: "shield001",
            itemName: "Iron Shield",
            slot: EquipmentSlot.Shield,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Shield")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 8 } }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Physical },
        },
        // Iron Belt: provides minor defense and boosts MaxHP.
        new Equipment(
            itemID: "belt001",
            itemName: "Iron Belt",
            slot: EquipmentSlot.Waist,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Belt")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 4 }, { Stat.MaxHP, 5 } }
        ),
        // Iron Boots: durable boots that add speed and resist ice.
        new Equipment(
            itemID: "boots001",
            itemName: "Iron Boots",
            slot: EquipmentSlot.Boots,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Iron_Boots")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 6 }, { Stat.Speed, 1 } }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Ice },
        },
    };

    // 🔹 Tier One Accessories Database
    public static List<Equipment> tierOneAccessories = new List<Equipment>()
    {
        // Iron Amulet: increases defense and intelligence; immune to poison.
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
                { Stat.Defense, 3 },
                { Stat.Intelligence, 3 },
            }
        )
        {
            immunityEffects = new List<Immunities> { Immunities.Poison },
        },
        // Iron Ring: modest boost to defense and dexterity with a slight crit bonus.
        new Equipment(
            itemID: "ring001",
            itemName: "Iron Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Iron_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 2 },
                { Stat.Dexterity, 2 },
                { Stat.CritChance, 0.05f },
            }
        ),
        // Silver Ring: a step up in defense and dexterity.
        new Equipment(
            itemID: "ring002",
            itemName: "Silver Ring",
            slot: EquipmentSlot.Ring,
            rarity: Rarity.Common,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Accessories/Silver_Ring")
                ?? Resources.Load<Sprite>("Sprites/Items/Accessories/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 4 },
                { Stat.Dexterity, 3 },
            }
        ),
        // Silver Amulet: enhances defense, intelligence, and evasion.
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
                { Stat.Defense, 3 },
                { Stat.Intelligence, 4 },
                { Stat.Evasion, 1 },
            }
        ),
    };

    // 🔹 Tier Two Weapons
    public static List<Equipment> tierTwoWeapons = new List<Equipment>()
    {
        // Steel Sword: a refined blade with balanced attack and accuracy.
        new Equipment(
            itemID: "sword002",
            itemName: "Steel Sword",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Steel_Sword")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 6 }, { Stat.Accuracy, 2 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Physical, 2 } },
        },
        // Oak Staff: boosts both attack and magical potential.
        new Equipment(
            itemID: "staff002",
            itemName: "Oak Staff",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Oak_Staff")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 9 }, { Stat.MaxMagic, 15 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Arcane, 3 } },
        },
        // Long Bow: a precise ranged weapon with a poison twist.
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
                { Stat.Attack, 12 },
                { Stat.CritChance, 0.6f },
            }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Poison, 2 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Poison },
        },
        // Silver Dagger: a swift dagger with bleeding potential.
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
                { Stat.Attack, 12 },
                { Stat.CritChance, 0.15f },
            }
        )
        {
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Bleed },
        },
        // Battle Axe: powerful and heavy, but vulnerable to fire.
        new Equipment(
            itemID: "axe002",
            itemName: "Battle Axe",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Battle_Axe")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 17 } }
        )
        {
            weaknessEffects = new List<Weaknesses> { Weaknesses.Fire },
        },
        // Glowing Wand: channels lightning for a magical punch.
        new Equipment(
            itemID: "wand002",
            itemName: "Glowing Wand",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Glowing_Wand")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 6 }, { Stat.MaxMagic, 25 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Lightning, 5 } },
        },
        // War Mace: a brutal mace combining high attack with physical resistance.
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
                { Stat.Attack, 20 },
                { Stat.CritChance, 0.3f },
            }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Physical },
        },
        // Golden Spear: precision weapon with a holy bonus.
        new Equipment(
            itemID: "spear002",
            itemName: "Golden Spear",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Golden_Spear")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 15 }, { Stat.Accuracy, 7 } }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Holy, 3 } },
        },
    };

    // 🔹 Tier Two Armor Database
    public static List<Equipment> tierTwoArmor = new List<Equipment>()
    {
        // Steel Helmet: sturdy headgear with an intelligence boost.
        new Equipment(
            itemID: "helmet002",
            itemName: "Steel Helmet",
            slot: EquipmentSlot.Head,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Helmet")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 10 },
                { Stat.Intelligence, 2 },
            }
        ),
        // Steel Chestplate: heavy armor offering robust defense with a mobility penalty.
        new Equipment(
            itemID: "chestplate002",
            itemName: "Steel Chestplate",
            slot: EquipmentSlot.Chest,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Chestplate")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 20 }, { Stat.Speed, -1 } }
        ),
        // Steel Leggings: protective leg armor with a slight speed boost.
        new Equipment(
            itemID: "leggings002",
            itemName: "Steel Leggings",
            slot: EquipmentSlot.Legs,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Leggings")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 14 }, { Stat.Speed, 1 } }
        ),
        // Steel Gloves: enhance dexterity and provide modest defense.
        new Equipment(
            itemID: "gloves002",
            itemName: "Steel Gloves",
            slot: EquipmentSlot.Hands,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Gloves")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 6 },
                { Stat.Dexterity, 2 },
            }
        ),
        // Steel Shield: solid shield with high defense and physical resistance.
        new Equipment(
            itemID: "shield002",
            itemName: "Steel Shield",
            slot: EquipmentSlot.Shield,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Shield")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 16 } }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Physical },
        },
        // Steel Belt: adds a bonus to defense and MaxHP.
        new Equipment(
            itemID: "belt002",
            itemName: "Steel Belt",
            slot: EquipmentSlot.Waist,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Belt")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 8 }, { Stat.MaxHP, 5 } }
        ),
        // Steel Boots: durable boots that increase speed but are weak to ice.
        new Equipment(
            itemID: "boots002",
            itemName: "Steel Boots",
            slot: EquipmentSlot.Boots,
            rarity: Rarity.Uncommon,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Steel_Boots")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 12 }, { Stat.Speed, 2 } }
        )
        {
            weaknessEffects = new List<Weaknesses> { Weaknesses.Ice },
        },
    };

    // 🔹 Tier Two Accessories Database
    public static List<Equipment> tierTwoAccessories = new List<Equipment>()
    {
        // Steel Amulet: improves defense and intelligence; immune to poison.
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
        )
        {
            immunityEffects = new List<Immunities> { Immunities.Poison },
        },
        // Steel Ring: a robust ring boosting dexterity and adding a minor physical damage bonus.
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
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Physical, 1 } },
        },
        // Gold Ring: a refined ring that enhances dexterity and grants lightning resistance.
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
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Lightning },
        },
        // Gold Amulet: balances defense and intelligence with an evasion boost.
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
                { Stat.Evasion, 2 },
            }
        ),
    };

    // 🔹 Tier Three Weapons Database
    public static List<Equipment> tierThreeWeapons = new List<Equipment>()
    {
        // Titanium Sword: a masterfully forged sword with high attack and physical damage.
        new Equipment(
            itemID: "sword003",
            itemName: "Titanium Sword",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/Titanium_Sword")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Attack, 22 },
                { Stat.CritChance, 0.1f },
            }
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Physical, 4 } },
        },
        // Crystal Staff: channels icy magic with high MaxMagic and a chance to freeze foes.
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
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Ice, 8 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Freeze },
        },
        // Recurve Bow: a precise bow with high crit chance and a poison bonus.
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
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Poison, 4 } },
        },
        // Platinum Dagger: a swift dagger that inflicts bleeding.
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
        )
        {
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Bleed },
        },
        // War Axe: a fearsome axe that can stun enemies.
        new Equipment(
            itemID: "axe003",
            itemName: "War Axe",
            slot: EquipmentSlot.Weapon,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Weapons/War_Axe")
                ?? Resources.Load<Sprite>("Sprites/Items/Weapons/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Attack, 25 } }
        )
        {
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Stun },
        },
        // Mystic Wand: channels potent arcane energy.
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
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Arcane, 10 } },
        },
        // Heavy Mace: a crushing weapon with a curse effect and slower speed.
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
        )
        {
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Curse },
        },
        // Dragon Spear: a legendary spear with fiery properties.
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
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Fire, 6 } },
            inflictedStatusEffects = new List<StatusEffectType> { StatusEffectType.Burn },
        },
    };

    // 🔹 Tier Three Armor Database
    public static List<Equipment> tierThreeArmor = new List<Equipment>()
    {
        // Titanium Helmet: advanced head protection with enhanced evasion.
        new Equipment(
            itemID: "helmet003",
            itemName: "Titanium Helmet",
            slot: EquipmentSlot.Head,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Helmet")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 15 }, { Stat.Evasion, 2 } }
        ),
        // Titanium Chestplate: superior torso armor with fire resistance.
        new Equipment(
            itemID: "chestplate003",
            itemName: "Titanium Chestplate",
            slot: EquipmentSlot.Chest,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Chestplate")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 22 } }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Fire },
        },
        // Titanium Leggings: well-balanced leg armor with a speed bonus.
        new Equipment(
            itemID: "leggings003",
            itemName: "Titanium Leggings",
            slot: EquipmentSlot.Legs,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Leggings")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 18 }, { Stat.Speed, 2 } }
        ),
        // Titanium Gloves: precision gloves that enhance dexterity.
        new Equipment(
            itemID: "gloves003",
            itemName: "Titanium Gloves",
            slot: EquipmentSlot.Hands,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Gloves")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float>
            {
                { Stat.Defense, 12 },
                { Stat.Dexterity, 3 },
            }
        ),
        // Titanium Shield: state-of-the-art shield with dual resistance.
        new Equipment(
            itemID: "shield003",
            itemName: "Titanium Shield",
            slot: EquipmentSlot.Shield,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Shield")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 24 } }
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Physical, Resistances.Holy },
        },
        // Titanium Belt: enhances defense and boosts MaxHP.
        new Equipment(
            itemID: "belt003",
            itemName: "Titanium Belt",
            slot: EquipmentSlot.Waist,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Belt")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 12 }, { Stat.MaxHP, 10 } }
        ),
        // Titanium Boots: advanced boots offering high defense and speed.
        new Equipment(
            itemID: "boots003",
            itemName: "Titanium Boots",
            slot: EquipmentSlot.Boots,
            rarity: Rarity.Rare,
            baseSprite: Resources.Load<Sprite>("Sprites/Items/Armor/Titanium_Boots")
                ?? Resources.Load<Sprite>("Sprites/Items/Armor/Default"),
            isOneTimeEffect: false,
            statOverrides: new Dictionary<Stat, float> { { Stat.Defense, 16 }, { Stat.Speed, 3 } }
        ),
    };

    // 🔹 Tier Three Accessories Database
    public static List<Equipment> tierThreeAccessories = new List<Equipment>()
    {
        // Titanium Amulet: premium amulet with dual immunities.
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
        )
        {
            immunityEffects = new List<Immunities> { Immunities.Poison, Immunities.Shadow },
        },
        // Titanium Ring: elegant ring that boosts dexterity and channels lightning.
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
        )
        {
            damageModifiers = new Dictionary<DamageType, float> { { DamageType.Lightning, 5 } },
        },
        // Platinum Ring: an exquisite ring enhancing dexterity and granting arcane resistance.
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
        )
        {
            resistanceEffects = new List<Resistances> { Resistances.Arcane },
        },
        // Platinum Amulet: a prestigious amulet that increases defense, intelligence, and evasion.
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
                { Stat.Evasion, 2 },
            }
        ),
    };
}
