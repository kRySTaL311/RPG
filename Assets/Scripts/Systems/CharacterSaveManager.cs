using System.IO;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    public string characterName;
    public int level;
    public int currentExp;
    public int expToNextLevel;
    public int baseHealth;
    public int baseMaxHealth;
    public int baseAttackPower;
    public int baseDefencePower;
    public int baseMaxMana;
    public int mana;
    public float BaseCriticalChance;
    public float BaseCriticalMultiplier;
    public int BasePoisonDamage;

    public string equippedWeaponID;
    public string equippedArmorID;
    public string equippedBootsID;
    public string equippedHelmID;
}

public static class CharacterSaveManager
{
    private static string SavePath(string characterName) =>
        Path.Combine(Application.persistentDataPath, characterName + "_data.json");

    public static void SaveCharacter(CharacterData data, Character runtimeCharacter)
    {
        CharacterSaveData saveData = new CharacterSaveData
        {
            characterName = data.characterName,
            level = data.level,
            currentExp = data.currentExp,
            expToNextLevel = data.expToNextLevel,
            baseHealth = data.baseHealth,
            baseMaxHealth = data.baseMaxHealth,
            baseAttackPower = data.baseAttackPower,
            baseDefencePower = data.baseDefencePower,
            baseMaxMana = data.baseMaxMana,
            mana = data.baseMana,
            BaseCriticalChance = data.BaseCriticalChance,
            BaseCriticalMultiplier = data.BaseCriticalMultiplier,
            BasePoisonDamage = data.BasePoisonDamage,

            equippedWeaponID = runtimeCharacter.equippedWeapon?.id,
            equippedArmorID = runtimeCharacter.equippedArmor?.id,
            equippedBootsID = runtimeCharacter.equippedBoots?.id,
            equippedHelmID = runtimeCharacter.equippedHelm?.id,
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath(data.characterName), json);
    }

    public static void LoadCharacter(CharacterData data, Character runtimeCharacter)
    {
        string path = SavePath(data.characterName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CharacterSaveData loaded = JsonUtility.FromJson<CharacterSaveData>(json);

            data.level = loaded.level;
            data.currentExp = loaded.currentExp;
            data.expToNextLevel = loaded.expToNextLevel;
            data.baseHealth = loaded.baseHealth;
            data.baseMaxHealth = loaded.baseMaxHealth;
            data.baseAttackPower = loaded.baseAttackPower;
            data.baseDefencePower = loaded.baseDefencePower;
            data.baseMaxMana = loaded.baseMaxMana;
            data.baseMana = loaded.mana;
            data.BaseCriticalChance = loaded.BaseCriticalChance;
            data.BaseCriticalMultiplier = loaded.BaseCriticalMultiplier;
            data.BasePoisonDamage = loaded.BasePoisonDamage;

            runtimeCharacter.equippedWeapon = SaveManager.instance.GetItemByID(loaded.equippedWeaponID);
            runtimeCharacter.equippedArmor = SaveManager.instance.GetItemByID(loaded.equippedArmorID);
            runtimeCharacter.equippedBoots = SaveManager.instance.GetItemByID(loaded.equippedBootsID);
            runtimeCharacter.equippedHelm = SaveManager.instance.GetItemByID(loaded.equippedHelmID);

            runtimeCharacter.InitializeFromData();
        }
        else
        {
            Debug.LogWarning($"No save file found for {data.characterName}.");
        }
    }
    public static void DeleteAllSaves()
    {
        string dir = Application.persistentDataPath;
        if (!Directory.Exists(dir)) return;
        foreach (string file in Directory.GetFiles(dir, "*_data.json"))
            File.Delete(file);
    }
}
