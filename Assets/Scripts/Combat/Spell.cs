using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string spellName;
    public float speed = 10;
    public int manaCost;

    public int poisonTurnes = 2;
    public Sprite spellIcon;
    [TextArea(2, 3)]
    public string spellDescription;
    public enum SpellType { Attack,Heal,Poison}
    public SpellType spellType;
    public bool isAoE = false;
    [UnityEngine.Range(0.1f, 5f)] public float damageMultiplier = 1.5f;

    private Vector3 targetPosition;
    private Character caster;

    void Update()
    {
        HandleSpellMovement();
    }
    public void Cast(Character target) 
    {
        BattleController controller = BattleController.instance;
        caster = controller.GetCurrentCharacter(); 

        if (!isAoE) 
        {
            HandleSingleTargetCast(target); 
        }
        else 
        {
            HandleAoECast(controller); 
        }

        Debug.Log($"{spellName} was cast{(isAoE ? " on all targets!" : $" on {target.name}!")}");
    }

    private void HandleSpellMovement()
    {
        if(targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.25f)
            {
                Destroy(gameObject, 0.75f);
            }
        }
    }

    private void HandleSingleTargetCast(Character target)
    {
        targetPosition=target.floatingTextSpawnPoint.position;
        ApplyEffect(target, caster);
    }

    private void HandleAoECast(BattleController controller)
    {
        List<Character> targets = GetAoETargets(controller, caster);

        foreach(Character t in targets)
        {
            ApplyEffect(t, caster);

        }

        if (targets.Count > 0)
        {
            targetPosition= targets[0].transform.position;
        }
    }

    private List<Character> GetAoETargets(BattleController controller, Character caster)
    {
        if(spellType == SpellType.Heal)
        {
            return new List<Character>(controller.characters[0]);

        }
        else
        {
            int casterSide = controller.characters[0].Contains(caster) ? 0 : 1;
            int targetSide = (casterSide == 0) ? 1 : 0;
            return new List<Character>(controller.characters[targetSide]);
        }
    }

    private void ApplyEffect(Character target,Character caster)
    {
        switch (spellType)
        {
            case SpellType.Attack:
                int scaledDamage = Mathf.RoundToInt(caster.attackPower * damageMultiplier);
                target.Hurt(scaledDamage);
                break;
            case SpellType.Heal:
                int scaledHealing = Mathf.RoundToInt(caster.attackPower * damageMultiplier);
                target.Heal(scaledHealing);
                break;
            case SpellType.Poison:
                int scaledPoisong = Mathf.RoundToInt(caster.attackPower * damageMultiplier);
                target.ApplyPoison(scaledPoisong,poisonTurnes);
                break;
        }
        CharacterUIManager.instance.UpdateCharacterUI();
    }

    public string GetSpellDescription()
    {
        string targetType = isAoE ? "ALL" : "One"; 

        switch (spellType)
        {
            case SpellType.Attack:
                return $"{spellDescription} <color=red>Deals</color> {damageMultiplier:F1}x power as damage(<color=orange>{targetType}</color>).";
            case SpellType.Heal:
                return $"{spellDescription} <color=green>Heals</color> {damageMultiplier:F1}x power as healing(<color=orange>{targetType}</color>).";
            case SpellType.Poison:
                return $"{spellDescription} <color=purple>Inflicts</color> ({damageMultiplier:F1})x power for (<color=yellow>{poisonTurnes} turns</color>) as poison damage(<color=orange>{targetType}</color>)";
            default:
                return spellDescription;
        }
    }
}
