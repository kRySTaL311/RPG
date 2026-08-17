using UnityEngine;

public class PartyMember : Character
{
    private void Start()
    {
        anim=GetComponent<Animator>();
        CharacterSaveManager.LoadCharacter(characterData,this);
        InitializeFromData();
    }

    public override void Die()
    {
        anim.SetBool("Dead", true);
        base.Die();
        BattleController.instance.characters[0].Remove(this);

        CharacterUIManager.instance.UpdateCharacterUI();
    }

    public void GainExp(int amount)
    {
        characterData.currentExp += amount;

        while (characterData.currentExp >= characterData.expToNextLevel)
        {
            LevelUp();
        }

        CharacterSaveManager.SaveCharacter(characterData, this);
    }
    private void LevelUp()
    {
        characterData.level++;
        characterData.currentExp -= characterData.expToNextLevel;
        characterData.expToNextLevel = Mathf.RoundToInt(characterData.expToNextLevel * 1.25f);

        characterData.baseMaxHealth += 10;
        characterData.baseAttackPower += 2;
        characterData.baseDefencePower += 1;
        characterData.baseMaxMana += 5;

        Debug.Log($"{characterName} leveled up to level {characterData.level}!");
    }
}
