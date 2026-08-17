using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager instance;

    [Header("UI References")]
    public GameObject questPanel;
    public Transform questContentParent;
    public GameObject questUIItemPrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {

        foreach (Transform child in questContentParent)
        {
            Destroy(child.gameObject);
        }

        List<Quest> activeQuests = QuestManager.instance.quests.FindAll(q => q.state == QuestState.InProgress);

        foreach (var quest in activeQuests)
        {
            GameObject questItem = Instantiate(questUIItemPrefab, questContentParent);
            TextMeshProUGUI[] texts = questItem.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = quest.title;
                texts[1].text = quest.description;
            }
        }
    }

    public void TogglePanel()
    {
        questPanel.SetActive(!questPanel.activeSelf);
        if (questPanel.activeSelf)
            RefreshUI();
    }
}
