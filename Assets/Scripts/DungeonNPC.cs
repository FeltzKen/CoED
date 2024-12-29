using UnityEngine;

namespace CoED
{
    public class DungeonNPC : MonoBehaviour
    {
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
                FloatingTextManager.Instance.ShowFloatingText(
                    $"Quest Assigned: {assignedQuest.QuestName}",
                    transform,
                    Color.white
                );
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
                FloatingTextManager.Instance.ShowFloatingText(
                    $"{rewardItem.ItemName} added to inventory!",
                    transform,
                    Color.green
                );
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
                FloatingTextManager.Instance.ShowFloatingText(
                    assignedQuest.Description,
                    transform,
                    Color.cyan
                );
            }
            else
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "No quest assigned to this NPC.",
                    transform,
                    Color.gray
                );
            }
        }
    }
}
