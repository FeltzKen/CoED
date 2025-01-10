using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    [System.Serializable]
    public class Quest
    {
        [Header("Quest Details")]
        [SerializeField]
        private string questName;

        [SerializeField]
        private string description;

        [SerializeField]
        private QuestType questType;

        [SerializeField]
        private List<QuestObjective> objectives;

        [SerializeField]
        private QuestItem rewardItem;

        [Header("Progress Tracking")]
        [SerializeField]
        private int currentObjectiveIndex = 0;

        public string QuestName => questName;
        public string Description => description;
        public QuestType QuestType => questType;
        public QuestItem RewardItem => rewardItem; // Add this line

        public QuestObjective CurrentObjective => objectives[currentObjectiveIndex];
        public bool IsCompleted => currentObjectiveIndex >= objectives.Count;
        private EquipmentInventory equipmentInventory;

        public void CompleteObjective()
        {
            if (IsCompleted)
            {
                Debug.Log($"Quest '{questName}' is already completed.");
                return;
            }

            Debug.Log(
                $"Quest '{questName}': Objective '{CurrentObjective.Description}' completed."
            );
            currentObjectiveIndex++;

            if (IsCompleted)
            {
                CompleteQuest();
            }
        }

        private void CompleteQuest()
        {
            Debug.Log(
                $"Quest '{questName}' completed! Reward: {rewardItem?.itemName ?? "No Reward"}."
            );
            GrantReward();
        }

        private void GrantReward()
        {
            equipmentInventory = EquipmentInventory.Instance;
            if (rewardItem != null && equipmentInventory != null)
            {
                equipmentInventory.AddQuestItem(rewardItem);
                Debug.Log(
                    $"Quest '{questName}': Special quest reward '{rewardItem.itemName}' added to player's inventory."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"Quest '{questName}': Special quest reward could not be granted."
                );
            }
        }

        public string GetProgressText()
        {
            return $"Objective {currentObjectiveIndex + 1}/{objectives.Count}: {CurrentObjective.Description}";
        }

        public void UpdateProgress(string targetName, int amount)
        {
            if (IsCompleted)
            {
                return;
            }

            QuestObjective currentObjective = CurrentObjective;
            if (
                currentObjective.TargetName.Equals(
                    targetName,
                    System.StringComparison.OrdinalIgnoreCase
                )
            )
            {
                currentObjective.UpdateProgress(amount);
                if (currentObjective.IsCompleted)
                {
                    CompleteObjective();
                }
            }
        }
    }

    [System.Serializable]
    public class QuestObjective
    {
        [SerializeField]
        private string description;

        [SerializeField]
        private ObjectiveType objectiveType;

        [SerializeField]
        private int targetAmount;

        [SerializeField]
        private string targetName;

        [Header("Progress Tracking")]
        [SerializeField]
        private int currentAmount = 0;

        public string Description => description;
        public ObjectiveType ObjectiveType => objectiveType;
        public string TargetName => targetName;
        public int CurrentAmount => currentAmount;
        public int TargetAmount => targetAmount;
        public bool IsCompleted => currentAmount >= targetAmount;

        public void UpdateProgress(int amount)
        {
            if (IsCompleted)
            {
                return;
            }

            currentAmount += amount;
            currentAmount = Mathf.Min(currentAmount, targetAmount);
            Debug.Log(
                $"Objective '{description}': Progress updated to {currentAmount}/{targetAmount}."
            );
        }
    }

    public enum QuestType
    {
        MainStory,
        SideQuest,
        CollectItems,
        DefeatEnemies,
        UnlockHiddenDoors,
        EscortNPC,
        SolvePuzzle,
        FindArtifact,
    }

    public enum ObjectiveType
    {
        Collect,
        Defeat,
        Find,
        Escort,
        Solve,
    }
}
