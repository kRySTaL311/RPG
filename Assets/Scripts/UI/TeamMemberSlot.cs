using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamMemberSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum SlotType { Team,Reserve}
    public SlotType slotType;
    public Image portrait;
    public int index;
    private Transform originalParent;
    private int originalIndex;
    private Vector2 originalPos;

    public void Setup(Sprite sprite,int i,SlotType type)
    {
        portrait.sprite = sprite;
        index= i;
        slotType = type;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent=transform.parent;
        originalIndex=transform.GetSiblingIndex();
        originalPos= transform.position;

        transform.SetParent(originalParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);
        transform.position = originalPos;

        GameObject dropTarget = eventData.pointerEnter;

        if (dropTarget == null || !dropTarget.TryGetComponent(out TeamMemberSlot targetSlot))
            return;

        if (slotType == SlotType.Team && targetSlot.slotType == SlotType.Team)
        {
            TeamManager.Instance.SwapTeamMembers(index, targetSlot.index);
        }
        if (slotType == SlotType.Reserve && targetSlot.slotType == SlotType.Team)
        {
            if (index < TeamManager.Instance.reserveTeamPrefabs.Count &&
                targetSlot.index < TeamManager.Instance.playerTeamPrefabs.Count)
            {
                GameObject newMember = TeamManager.Instance.reserveTeamPrefabs[index];
                GameObject oldMember = TeamManager.Instance.playerTeamPrefabs[targetSlot.index];
                TeamManager.Instance.playerTeamPrefabs[targetSlot.index] = newMember;
                TeamManager.Instance.reserveTeamPrefabs.RemoveAt(index);
                TeamManager.Instance.reserveTeamPrefabs.Add(oldMember);
                TeamManager.Instance.SaveTeam();
            }
        }

        FormationManager.Instance.RefreshUI();
    }
}
