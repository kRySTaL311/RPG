using UnityEngine;

public class FormationManager : MonoBehaviour
{
    public static FormationManager Instance;
    public Transform teamSlotParent;
    public Transform reserveSlotParent;
    public GameObject teamMemberSlotPrefab;
    public GameObject formationPanel;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in teamSlotParent)
            Destroy(child.gameObject);

        foreach (Transform child in reserveSlotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < TeamManager.Instance.playerTeamPrefabs.Count; i++)
        {
            GameObject slotGO = Instantiate(teamMemberSlotPrefab, teamSlotParent);
            TeamMemberSlot slot = slotGO.GetComponent<TeamMemberSlot>();
            GameObject charPrefab = TeamManager.Instance.playerTeamPrefabs[i];
            Sprite portrait = charPrefab.GetComponent<Character>()?.characterIcon;
            slot.Setup(portrait, i, TeamMemberSlot.SlotType.Team);
        }
        for (int i = 0; i < TeamManager.Instance.reserveTeamPrefabs.Count; i++)
        {
            GameObject slotGO = Instantiate(teamMemberSlotPrefab, reserveSlotParent);
            TeamMemberSlot slot = slotGO.GetComponent<TeamMemberSlot>();
            GameObject charPrefab = TeamManager.Instance.reserveTeamPrefabs[i];
            Sprite portrait = charPrefab.GetComponent<Character>()?.characterIcon;
            slot.Setup(portrait, i, TeamMemberSlot.SlotType.Reserve);
        }
    }

    public void ToggleFormationPanel()
    {
        RefreshUI();
        bool isActive = formationPanel.activeSelf;
        formationPanel.SetActive(!isActive);

    }
}
