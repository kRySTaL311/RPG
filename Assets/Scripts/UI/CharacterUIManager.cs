using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterUI
{
    public GameObject panel;
    public Image characterIcon;
    public TextMeshProUGUI nameText;
    public Slider healthBar;
    public Slider manaBar;
    public Image posionIcon;
}

public class CharacterUIManager : MonoBehaviour
{
    [SerializeField] private List<CharacterUI> characterUIList = new List<CharacterUI>();

    public static CharacterUIManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UpdateCharacterUI();
    }

    public void UpdateCharacterUI()
    {
        List<Character> characters = BattleController.instance.characters[0];

        for (int i=0;i<characterUIList.Count; i++)
        {
            if (i < characters.Count)
            {
                Character character = characters[i];
                CharacterUI ui = characterUIList[i];

                ui.panel.SetActive(true);

                ui.nameText.text = $"{character.characterName}";

                if (ui.characterIcon && character.characterIcon)
                    ui.characterIcon.sprite = character.characterIcon;

                bool isAlive = character.health > 0;
                ui.healthBar.gameObject.SetActive(isAlive);
                ui.manaBar.gameObject.SetActive(isAlive);

                if (isAlive)
                {
                    ui.healthBar.maxValue = character.maxHealth;
                    ui.healthBar.value = character.health;
                    ui.manaBar.maxValue = character.maxMana;
                    ui.manaBar.value = character.mana;
                }

                if (ui.posionIcon != null)
                    ui.posionIcon.enabled = character.isPoisoned && isAlive;
            }
            else
            {
                characterUIList[i].panel.SetActive(false);
            }

        }
    }
}
