using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public string questIDToStart;
    public string questIDToComplete;
    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!string.IsNullOrEmpty(questIDToStart) && QuestManager.instance.GetQuestState(questIDToStart) == QuestState.NotStarted)
            {
                QuestManager.instance.StartQuest(questIDToStart);
                QuestUIManager.instance.RefreshUI();
            }
            else if (!string.IsNullOrEmpty(questIDToComplete) && QuestManager.instance.GetQuestState(questIDToComplete) == QuestState.InProgress)
            {
                QuestManager.instance.CompleteQuest(questIDToComplete);
                QuestUIManager.instance.RefreshUI();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
