using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("Dialog Configuration")]
    public Sprite npcAvatar;
    public string npcName;
    public DialogEntry[] dialogBeforeQuest;
    public DialogEntry[] dialogAfterQuest;
    public bool questDone;
    public string questIDToStart;
    public string questIDToComplete;
    [Header("Interaction Settings")]
    public GameObject iconObject;
    public KeyCode interactionKey = KeyCode.E;

    private bool canActivate;

    private void Update()
    {
        if (!canActivate || DialogController.Instance == null || DialogController.Instance.dialogBox.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (QuestManager.instance.GetQuestState(questIDToStart) == QuestState.Completed)
                questDone = true;

            var dialog = questDone ? dialogAfterQuest : dialogBeforeQuest;
            DialogController.Instance.ShowDialog(dialog, npcAvatar, npcName);

            if (!string.IsNullOrEmpty(questIDToStart) && QuestManager.instance.GetQuestState(questIDToStart) == QuestState.NotStarted)
            {
                QuestManager.instance.StartQuest(questIDToStart);
                QuestUIManager.instance.RefreshUI();
            }
            else if (!string.IsNullOrEmpty(questIDToComplete) && QuestManager.instance.GetQuestState(questIDToComplete) == QuestState.InProgress)
            {
                QuestManager.instance.CompleteQuest(questIDToComplete);
                QuestUIManager.instance.RefreshUI();
                questDone = true;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        canActivate = true;

        SetIconVisibility(true);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        canActivate = false;
        SetIconVisibility(false);
    }

    private void SetIconVisibility(bool visible)
    {
        if (iconObject != null)
        {
            iconObject.SetActive(visible);
        }
    }
}