using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character
{
    public ItemData itemToGive;
    public int expReward = 25;
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private GameObject healthUi;
    [SerializeField] private GameObject poisonIcon;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {

    }

    private void RefreshUI()
    {
        UpdateHealthBarUI();
        UpdatePoisonStatusIcon();
    }

    public void Act()
    {
        int dieRoll = Random.Range(0, 2);
        Character target = ChooseTarget();
        switch (dieRoll)
        {
            case 0:
                ActWithSpellOrAttack(target);
                break;
            case 1:
                ActWithAttack(target);
                break;

        }
    }

    private Character ChooseTarget()
    {
        return BattleController.instance.GetRandomPlayer();
    }

    private void ActWithSpellOrAttack(Character target)
    {
        Spell spellToCast = GetRandomSpell();
        if(spellToCast !=null&& spellToCast.spellType == Spell.SpellType.Heal)
        {
            target = BattleController.instance.GetWeakestEnemy();
        }

        if(spellToCast != null && CastSpell(spellToCast, target))
        {
            return;
        }
        else
        {
            ActWithAttack(target);
        }
    }

    private void ActWithAttack(Character target)
    {
        BattleController.instance.DoAttack(this, target);
    }

    private Spell GetRandomSpell()
    {
        if (spells.Count == 0)
            return null;
        return spells[Random.Range(0, spells.Count)];

    }

    private void UpdateHealthByValue(int value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, maxHealth);

        UpdateHealthBarUI();

        if (health <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthBarUI()
    {
        if(healthFillImage != null)
        {
            float healthPercent = Mathf.Clamp01((float)health / maxHealth);
            healthFillImage.fillAmount = healthPercent;
        }
    }

    private void UpdatePoisonStatusIcon()
    {
        if(poisonIcon != null)
        {
            poisonIcon.SetActive(isPoisoned);
        }
    }

    public override void Die()
    {
        base.Die();
        BattleController.instance.totalExpEarned += expReward;
        BattleController.instance.droppedItems.Add(itemToGive);
        RemoveFromBattleController();
        DisableHealthBar();
    }

    private void RemoveFromBattleController()
    {
        if(BattleController.instance !=null && BattleController.instance.characters.ContainsKey(1))
        {
            BattleController.instance.characters[1].Remove(this);
        }
    }

    private void DisableHealthBar()
    {
        if (healthUi != null)
        {
            healthUi.SetActive(false);
        }
    }
}
