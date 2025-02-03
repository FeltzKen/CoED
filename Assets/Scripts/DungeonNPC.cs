using UnityEngine;

namespace CoED
{
    public class DungeonNPC : MonoBehaviour
    {
        private Quest assignedQuest;

        [SerializeField]
        private Equipment equipmentRewardItem;
        private ConsumableItem consumableRewardItem;

        private QuestManager questManager;
        private EquipmentInventory equipmentInventory;
        private ConsumableInventory consumableInventory;

        private void Start()
        {
            questManager = QuestManager.Instance;
            equipmentInventory = FindAnyObjectByType<EquipmentInventory>();

            if (questManager != null && equipmentInventory != null)
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
            if (equipmentInventory != null && equipmentRewardItem != null)
            {
                equipmentInventory.AddEquipment(equipmentRewardItem);
                FloatingTextManager.Instance.ShowFloatingText(
                    $"{equipmentRewardItem.itemName} added to inventory!",
                    transform,
                    Color.green
                );
            }
            else if (equipmentInventory == null || consumableRewardItem == null)
            {
                consumableInventory.AddItem(consumableRewardItem);
                FloatingTextManager.Instance.ShowFloatingText(
                    $"{consumableRewardItem.GetName()} added to inventory!",
                    transform,
                    Color.green
                );
            }
            else
            {
                Debug.LogWarning("EquipmentInventory or RewardItem not found.");
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
