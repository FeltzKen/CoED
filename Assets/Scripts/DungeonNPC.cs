using UnityEngine;
using CoED;

namespace CoED
{
    public class DungeonNPC : MonoBehaviour
    {
        [SerializeField]
        private FloatingTextManager floatingTextManager; // Assign in Inspector

        private Quest assignedQuest;

        [SerializeField]
        private Item rewardItem;

        private QuestManager questManager;
        private Inventory playerInventory;

        private void Start()
        {
            questManager = QuestManager.Instance;
            playerInventory = FindAnyObjectByType<Inventory>();

            if (questManager != null && playerInventory != null)
            {
                assignedQuest = questManager.AssignNewQuest();
                floatingTextManager?.ShowFloatingText(
                    $"Quest Assigned: {assignedQuest.QuestName}",
                    transform.position,
                    Color.white
                );
                Debug.Log($"NPC Quest Assigned: {assignedQuest.QuestName}");
            }
            else
            {
                Debug.LogError("QuestManager or Inventory not found in the scene.");
            }
        }

        public void GrantReward()
        {
            if (playerInventory != null && rewardItem != null)
            {
                playerInventory.AddItem(rewardItem);
                floatingTextManager?.ShowFloatingText(
                    $"{rewardItem.ItemName} added to inventory!",
                    transform.position,
                    Color.green
                );
                Debug.Log($"{rewardItem.ItemName} has been added to your inventory!");
            }
            else
            {
                Debug.LogWarning("PlayerInventory or RewardItem is missing.");
            }
        }

        public void Interact()
        {
            if (assignedQuest != null)
            {
                floatingTextManager?.ShowFloatingText(
                    assignedQuest.Description,
                    transform.position,
                    Color.cyan
                );
                Debug.Log($"{assignedQuest.Description}");
            }
            else
            {
                floatingTextManager?.ShowFloatingText(
                    "No quest assigned to this NPC.",
                    transform.position,
                    Color.gray
                );
                Debug.Log("No quest assigned to this NPC.");
            }
        }
    }
}
