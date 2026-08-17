using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData;
    [Header("Equipment")]
    public ItemData equippedWeapon;
    public ItemData equippedArmor;
    public ItemData equippedBoots;
    public ItemData equippedHelm;

    public string characterName;
    public int level = 1;
    public int currentExp;
    public int expToNextLevel;
    public Sprite characterIcon;
    public int health;
    public int maxHealth;
    public int attackPower;
    public int defencePower;
    public int maxMana;
    public int mana;
    [Range(0f, 1f)] public float criticalChance = 0.1f;
    public float criticalMultiplier = 2f;

    public bool isPoisoned;
    public int poisonDamage;
    public int poisonTurnsRemaining;

    public List<Spell> spells;
    public Animator anim;
    public GameObject selectionCircle;
    public GameObject hoverIndicator;
    public Transform floatingTextSpawnPoint;
    [SerializeField] private GameObject floatingTextPrefab;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void InitializeFromData()
    {
        if (characterData == null) return;

        characterName = characterData.characterName;
        level = characterData.level;
        currentExp = characterData.currentExp;
        expToNextLevel = characterData.expToNextLevel;

        attackPower = Mathf.RoundToInt(GetTotalStat(StatsType.AD));
        defencePower = Mathf.RoundToInt(GetTotalStat(StatsType.DF));
        maxHealth = Mathf.RoundToInt(GetTotalStat(StatsType.HP));
        maxMana = Mathf.RoundToInt(GetTotalStat(StatsType.MP));
        criticalChance = GetTotalStat(StatsType.CR);
        criticalMultiplier = GetTotalStat(StatsType.CRD);
        poisonDamage = Mathf.RoundToInt(GetTotalStat(StatsType.PD));
        health = maxHealth;
        mana = maxMana;
        CharacterSaveManager.SaveCharacter(characterData, this);
    }

    public void Hurt(int amount, bool isCritical = false)
    {
        int damageAmount = Mathf.Max(amount - defencePower, 1);

        ShowDamageAmount(damageAmount, isCritical);
        health = Mathf.Max(health - damageAmount, 0);
        HandleDeath();
        UpdateEnemyHealthUI();
    }

    private void ShowDamageAmount(int damageAmount, bool isCritical)
    {
        string displayText = isCritical ? $"CRITICAL!\n{damageAmount}" : damageAmount.ToString();
        Color textColor = isCritical ? Color.yellow : Color.red;
        ShowFloatingText(displayText, textColor);
    }

    private void HandleDeath()
    {
        if (health == 0)
        {
            ScreenShakeManager.Instance.Shake(0.1f, 0.2f);
            SetSelected(false);

            Die();

        }
    }

    public virtual void Die()
    {
        anim.SetBool("Dead", true);
        GetComponent<BoxCollider2D>().enabled = false;
        SetSelected(false);
    }

    private void UpdateEnemyHealthUI()
    {
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.UpdateHealthBarUI();
        }
    }

    public void Heal(int amount)
    {
        int healAmount = amount;
        health = Mathf.Min(health + healAmount, maxHealth);
        ShowFloatingText(healAmount.ToString(), Color.green);
    }

    private void ShowFloatingText(string text, Color color)
    {
        if (floatingTextPrefab != null && floatingTextSpawnPoint != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, floatingTextSpawnPoint.position, Quaternion.identity);
            FloatingDamageText textScript = textObj.GetComponent<FloatingDamageText>();
            textScript.Initialize(text, color);
        }
    }

    public void Attack(Character target)
    {
        if (health <= 0) return;

        anim.SetTrigger("Attack");
        bool isCritical = DetermineCriticalHit();
        int finalDamage = CalculateFinalDamage(isCritical);

        if (isCritical)
        {
            ScreenShakeManager.Instance.Shake(0.1f, 0.2f);
        }
        target.Hurt(finalDamage, isCritical);
    }

    private bool DetermineCriticalHit()
    {
        return Random.value < criticalChance;
    }

    private int CalculateFinalDamage(bool isCritical)
    {
        return isCritical ? Mathf.RoundToInt(attackPower * criticalMultiplier) : attackPower;
    }

    public bool CastSpell(Spell spell, Character targetCharacter)
    {
        if (health <= 0) return false;
        if (mana < spell.manaCost) return false;

        Spell spellToCast = Instantiate(spell, transform.position, Quaternion.identity);
        mana -= spell.manaCost;
        spellToCast.Cast(targetCharacter);
        anim.SetTrigger("Attack");
        return true;
    }

    public void ApplyPoison(int newDamage, int newTurns)
    {
        if (isPoisoned)
        {
            poisonDamage += newDamage;
            poisonTurnsRemaining += newTurns;
        }
        else
        {
            isPoisoned = true;
            poisonDamage = newDamage;
            poisonTurnsRemaining = newTurns;
        }
        ShowFloatingText("Poisoned!", Color.magenta);
    }

    public void HandlePoisonEffect()
    {
        if (isPoisoned && poisonTurnsRemaining > 0)
        {
            ApplyPoisonDamage();
            poisonTurnsRemaining--;

            if (health <= 0)
            {
                health = 0;
                Die();
            }
            else if (poisonTurnsRemaining <= 0)
            {
                ClearPoison();
            }
        }
    }

    private void ApplyPoisonDamage()
    {
        health -= poisonDamage;
        ShowFloatingText("Poison " + poisonDamage, Color.magenta);
    }

    private void ClearPoison()
    {
        isPoisoned = false;
        poisonDamage = 0;
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(isSelected);
    }

    public void ShowHoverIndicator(bool state)
    {
        hoverIndicator.SetActive(state);
    }

    public void Equip(ItemData item)
    {
        switch (item.type)
        {
            case ItemType.EquipableWeapon: equippedWeapon = item; break;
            case ItemType.EquipableArmor: equippedArmor = item; break;
            case ItemType.EquipableBoots: equippedBoots = item; break;
            case ItemType.EquipableHelm: equippedHelm = item; break;
        }
    }

    public ItemData Unequip(ItemType type)
    {
        ItemData unequipped = null;
        switch (type)
        {
            case ItemType.EquipableWeapon: unequipped = equippedWeapon; equippedWeapon = null; break;
            case ItemType.EquipableArmor: unequipped = equippedArmor; equippedArmor = null; break;
            case ItemType.EquipableBoots: unequipped = equippedBoots; equippedBoots = null; break;
            case ItemType.EquipableHelm: unequipped = equippedHelm; equippedHelm = null; break;
        }
        return unequipped;
    }

    public ItemData GetEquippedItem(ItemType type)
    {
        return type switch
        {
            ItemType.EquipableWeapon => equippedWeapon,
            ItemType.EquipableArmor => equippedArmor,
            ItemType.EquipableBoots => equippedBoots,
            ItemType.EquipableHelm => equippedHelm,
            _ => null
        };
    }

    public float GetTotalStat(StatsType type)
    {
        float baseValue = GetBaseStat(type);
        float bonus = 0f;

        bonus += GetItemBonus(equippedWeapon, type);
        bonus += GetItemBonus(equippedArmor, type);
        bonus += GetItemBonus(equippedBoots, type);
        bonus += GetItemBonus(equippedHelm, type);
        CharacterSaveManager.SaveCharacter(characterData, this);
        return baseValue + bonus;
    }

    private float GetItemBonus(ItemData item, StatsType type)
    {
        if (item == null || item.statsOfItem == null) return 0f;

        float bonus = 0f;
        foreach (var stat in item.statsOfItem)
        {
            if (stat.type == type)
                bonus += stat.value;
        }
        return bonus;
    }

    private float GetBaseStat(StatsType type)
    {
        switch (type)
        {
            case StatsType.HP: return characterData.baseMaxHealth;
            case StatsType.MP: return characterData.baseMaxMana;
            case StatsType.AD: return characterData.baseAttackPower;
            case StatsType.DF: return characterData.baseDefencePower;
            case StatsType.CR: return characterData.BaseCriticalChance;
            case StatsType.CRD: return characterData.BaseCriticalMultiplier;
            case StatsType.PD: return characterData.BasePoisonDamage;
            case StatsType.XP: return characterData.currentExp;
            default: return 0f;
        }
    }

}