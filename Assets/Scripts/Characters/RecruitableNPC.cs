using UnityEngine;

public class RecruitableNPC : MonoBehaviour
{
    public GameObject characterPrefab;
    public GameObject signObject;
    private bool hasJoined = false;
    private bool playerNearby = false;

    public DialogEntry[] npcDialog;
    public Sprite npcPortrait;
    public string npcName = "NPC";
    private bool waitingForDialogToEnd = false;
    public string questIDToComplete;
    private void Start()
    {
        if (TeamManager.Instance.IsInTeam(characterPrefab))
        {
            hasJoined = true;
            gameObject.SetActive(false); 
        }
    }
    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasJoined && !DialogController.Instance.dialogBox.activeInHierarchy && !waitingForDialogToEnd)
            {

                DialogController.Instance.ShowDialog(npcDialog, npcPortrait, npcName);
                waitingForDialogToEnd = true;
            }

        }

        if (waitingForDialogToEnd && !DialogController.Instance.dialogBox.activeInHierarchy)
        {
            if (!string.IsNullOrEmpty(questIDToComplete) && QuestManager.instance.GetQuestState(questIDToComplete) == QuestState.InProgress)
            {
                QuestManager.instance.CompleteQuest(questIDToComplete);
                QuestUIManager.instance.RefreshUI();
            }
            TryRecruit();
            waitingForDialogToEnd = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (signObject != null)
            {
                signObject.SetActive(true);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (signObject != null)
            {
                signObject.SetActive(false);
            }
        }
    }

    private void TryRecruit()
    {
        if (!hasJoined)
        {
            bool success = TeamManager.Instance.AddToTeam(characterPrefab);
            if (success)
            {
                hasJoined = true;
                Debug.Log($"{characterPrefab.name} has joined your team!");
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Team is full!");
            }
        }
    }
}
