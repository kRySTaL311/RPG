using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject spellPanel;
    [SerializeField] private GameObject spellDesBG;
    [SerializeField] private Button spellButtonPrefab;
    public TextMeshProUGUI spellDescriptionText;
    public static SpellPanelManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void ToggleSpellPanel(bool state)
    {
        if (spellPanel != null)
        {
            spellPanel.SetActive(state);

        }

        if (state)
        {
            List<Spell> spells = BattleController.instance.GetCurrentCharacter().spells;
            BuildSpellList(spells);
        }
        else
        {
            if (spellDescriptionText != null)
                spellDescriptionText.text = "";
            spellDesBG.SetActive(false);
        }
    }

    public void BuildSpellList(List<Spell> spells)
    {
        if (spells == null || spellPanel == null) return;

        foreach (Button btn in spellPanel.GetComponentsInChildren<Button>())
        {
            Destroy(btn.gameObject);
        }

        foreach (Spell spell in spells)
        {
            Button spellButton = Instantiate(spellButtonPrefab, spellPanel.transform);
            var spellText = spellButton.GetComponentInChildren<TextMeshProUGUI>();
            if (spellText != null)
                spellText.text = $"{spell.spellName} ({spell.manaCost} MP)";

            var iconImage = spellButton.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && spell.spellIcon != null)
                iconImage.sprite = spell.spellIcon;

            spellButton.onClick.AddListener(() => SelectSpell(spell));
        }
    }
    private void SelectSpell(Spell spell)
    {
        BattleController.instance.playerSelectedSpell = spell;
        BattleController.instance.playerIsAttacking = false;
        if (spellDescriptionText != null)
            spellDescriptionText.text = spell.GetSpellDescription();

        spellDesBG.SetActive(true);
    }
}
