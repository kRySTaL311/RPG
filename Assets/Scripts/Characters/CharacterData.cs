using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Character/Data")]
public class CharacterData : ScriptableObject
{
    public GameObject characterPrefab;
    public string characterName;
    public int level = 1;
    public int currentExp;
    public int expToNextLevel;

    public int baseHealth;
    public int baseMaxHealth;
    public int baseAttackPower;
    public int baseDefencePower;
    public int baseMaxMana;
    public int baseMana;
    [Range(0f, 1f)] public float BaseCriticalChance = 0.1f;
    public float BaseCriticalMultiplier = 2f;
    public int BasePoisonDamage;

}