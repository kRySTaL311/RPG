using UnityEngine;
using UnityEngine.UI;

public class ActionButtonManager : MonoBehaviour
{
    [SerializeField] private Button[] actionButtons;
    [SerializeField] private SpellPanelManager spellPanelManager;
    public static ActionButtonManager instance;
    private void Awake()
    {
        instance = this;   
    }
    public void ToggleActionState(bool state)
    {
        spellPanelManager.ToggleSpellPanel(false);
        foreach(Button btn in actionButtons)
        {
            btn.interactable=state;
            btn.image.color=state?Color.white:Color.gray;
        }
    }

    public void SelectAttack()
    {
        Debug.Log("Attack selected.");
        BattleController.instance.playerSelectedSpell = null;
        BattleController.instance.playerIsAttacking = true;
    }
}
