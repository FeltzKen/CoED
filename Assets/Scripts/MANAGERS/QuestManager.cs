using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CoED;

namespace CoED
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quest Settings")]
        [SerializeField]
        private List<Quest> availableQuests = new List<Quest>();

        [SerializeField]
        private List<Quest> activeQuests = new List<Quest>();

        private Inventory playerInventory;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("QuestManager instance already exists. Destroying duplicate.");
            }
        }

        private void Start()
        {
            playerInventory = Inventory.Instance;
            if (playerInventory == null)
            {
                Debug.LogError("QuestManager: Player Inventory not found.");
            }
        }

        public Quest AssignNewQuest()
        {
            if (availableQuests.Count == 0)
            {
                Debug.LogWarning("QuestManager: No available quests to assign.");
                return null;
            }

            int randomIndex = Random.Range(0, availableQuests.Count);
            Quest newQuest = availableQuests[randomIndex];
            activeQuests.Add(newQuest);
            availableQuests.RemoveAt(randomIndex);

            Debug.Log($"QuestManager: Assigned new quest '{newQuest.QuestName}'.");

            PlayerUI.Instance?.ShowQuestAssigned(newQuest);

            return newQuest;
        }

        public void CompleteQuest(Quest completedQuest)
        {
            if (activeQuests.Contains(completedQuest))
            {
                activeQuests.Remove(completedQuest);
                GrantReward(completedQuest);
                Debug.Log(
                    $"QuestManager: Quest '{completedQuest.QuestName}' completed and removed from active quests."
                );

                PlayerUI.Instance?.ShowQuestCompleted(completedQuest);
            }
            else
            {
                Debug.LogWarning(
                    $"QuestManager: Attempted to complete a quest '{completedQuest.QuestName}' that is not active."
                );
            }
        }

        private void GrantReward(Quest quest)
        {
            if (quest.RewardItem != null)
            {
                playerInventory.AddItem(quest.RewardItem);
                Debug.Log(
                    $"QuestManager: Granted reward '{quest.RewardItem.ItemName}' to the player."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"QuestManager: Quest '{quest.QuestName}' has no reward item assigned."
                );
            }
        }

        public void UpdateQuestProgress(string targetName, int amount)
        {
            var questsToUpdate = activeQuests.Where(quest =>
                !quest.IsCompleted
                && quest.CurrentObjective.TargetName.Equals(
                    targetName,
                    System.StringComparison.OrdinalIgnoreCase
                )
            );

            foreach (Quest quest in questsToUpdate)
            {
                quest.UpdateProgress(targetName, amount);
                if (quest.IsCompleted)
                {
                    CompleteQuest(quest);
                }
            }
        }

        public List<Quest> GetActiveQuests()
        {
            return new List<Quest>(activeQuests);
        }

        public List<Quest> GetAvailableQuests()
        {
            return new List<Quest>(availableQuests);
        }

        public void AddAvailableQuest(Quest quest)
        {
            if (quest != null && !availableQuests.Contains(quest))
            {
                availableQuests.Add(quest);
                Debug.Log($"QuestManager: Added quest '{quest.QuestName}' to available quests.");
            }
            else
            {
                Debug.LogWarning(
                    "QuestManager: Attempted to add a null or duplicate quest to available quests."
                );
            }
        }

        public void RemoveAvailableQuest(Quest quest)
        {
            if (availableQuests.Contains(quest))
            {
                availableQuests.Remove(quest);
                Debug.Log(
                    $"QuestManager: Removed quest '{quest.QuestName}' from available quests."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"QuestManager: Attempted to remove quest '{quest.QuestName}' that is not in available quests."
                );
            }
        }
    }
}
