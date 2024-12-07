using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
namespace CoED.Tests
{
    public class ItemTest
    {
        private Item testItem;

        [SetUp]
        public void Setup()
        {
            // Create a new instance of the Item scriptable object for testing
            testItem = ScriptableObject.CreateInstance<Item>();
        }

        [Test]
        public void TestItemAttributes()
        {
            // Assign values to the item attributes
            testItem.name = "Test Sword";
            testItem.icon = null; // Assuming no icon for simplicity
            testItem.attackBoost = 10;
            testItem.defenseBoost = 5;
            testItem.speedBoost = 2;
            testItem.healthBoost = 20;

            // Verify the assigned values
            Assert.AreEqual("Test Sword", testItem.ItemName);
            Assert.IsNull(testItem.Icon);
            Assert.AreEqual(10, testItem.AttackBoost);
            Assert.AreEqual(5, testItem.DefenseBoost);
            Assert.AreEqual(2, testItem.speedBoost);
            Assert.AreEqual(20, testItem.HealthBoost);
        }

        [Test]
        public void TestInitializeItem()
        {
            // Uncomment the initialization logic in the Item class to test this
            // testItem.InitializeItem();

            // Assuming enchantmentChance and curseChance are defined in the Item class
            // float enchantmentChance = 0.3f;
            // float curseChance = 0.1f;

            // Verify the initialization logic
            // bool isEnchanted = Random.value < enchantmentChance;
            // bool isCursed = !isEnchanted && Random.value < curseChance;

            // Assert.AreEqual(isEnchanted, testItem.IsEnchanted);
            // Assert.AreEqual(isCursed, testItem.IsCursed);
        }

        public IEnumerator TestItemCollection()
        {
            // Create a GameObject to simulate the player
            GameObject player = new GameObject("Player");
            player.tag = "Player";

            // Create a GameObject to simulate the item
            GameObject itemObject = new GameObject("Item");
            itemObject.AddComponent<BoxCollider2D>().isTrigger = true;
            ItemCollectible collectible = itemObject.AddComponent<ItemCollectible>();
            //collectible.item = testItem;

            // Simulate the player walking over the item
            //player.transform.position = itemObject.transform.position;

            // Wait for a frame to allow the trigger to be processed
            yield return null;

            // Verify the item was collected (assuming a method to check inventory)
            // Assert.IsTrue(Inventory.Instance.HasItem(testItem));
        }
    }
}