using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

[System.Serializable]
public class Quest
{
    public string questID;
    public string title;
    public string description;
    public QuestState state;

    public int rewardGold;
    public ItemData itemToGive;
    public bool requiresItemToComplete;
    public ItemData questItem;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public List<Quest> quests = new List<Quest>();
    private Dictionary<string, Quest> questDict = new Dictionary<string, Quest>();

    private void Awake()
    {
        if (instance == null) instance = this;

        foreach (var quest in quests)
        {
            questDict[quest.questID] = quest;
        }

        LoadQuests();
    }

    public void StartQuest(string questID)
    {
        if (questDict.TryGetValue(questID, out var quest))
        {
            if (quest.state == QuestState.NotStarted)
            {
                quest.state = QuestState.InProgress;
                Debug.Log($"Quest started: {quest.title}");
            }
        }
        else
        {
            Debug.LogWarning($"Quest ID {questID} not found.");
        }
    }

    public void CompleteQuest(string questID)
    {
        if (questDict.TryGetValue(questID, out var quest))
        {
            if (quest.state == QuestState.InProgress)
            {

                if (quest.requiresItemToComplete)
                {
                    if (quest.questItem != null && !InventoryManager.instance.HasItem(quest.questItem, 1))
                    {
                        Debug.LogWarning($"Missing required item to complete quest: {quest.title}");
                        return;
                    }

                    if (quest.questItem != null)
                    {
                        InventoryManager.instance.RemoveItem(quest.questItem);
                    }
                }

                quest.state = QuestState.Completed;
                Debug.Log($"Quest completed: {quest.title}");
                GrantQuestRewards(quest);
            }
        }
        else
        {
            Debug.LogWarning($"Quest ID {questID} not found.");
        }
    }

    private void GrantQuestRewards(Quest quest)
    {
        Debug.Log($"Granting rewards for quest: {quest.title}");

        GoldManager.instance.AddGold(quest.rewardGold);
        InventoryManager.instance.AddItem(quest.itemToGive);
    }

    public QuestState GetQuestState(string questID)
    {
        return questDict.ContainsKey(questID) ? questDict[questID].state : QuestState.NotStarted;
    }

    public void SaveQuests()
    {
        foreach (var quest in quests)
        {
            PlayerPrefs.SetInt($"Quest_{quest.questID}", (int)quest.state);
        }

        PlayerPrefs.Save();
    }

    public void LoadQuests()
    {
        foreach (var quest in quests)
        {
            if (PlayerPrefs.HasKey($"Quest_{quest.questID}"))
            {
                quest.state = (QuestState)PlayerPrefs.GetInt($"Quest_{quest.questID}");
            }
        }
    }

    public void ResetAllQuests()
    {
        foreach (var quest in quests)
        {
            quest.state = QuestState.NotStarted;
            PlayerPrefs.DeleteKey($"Quest_{quest.questID}");
        }

        PlayerPrefs.Save();
        Debug.Log("All quests have been reset.");
    }

    private void OnApplicationQuit()
    {
        SaveQuests();
    }
}
