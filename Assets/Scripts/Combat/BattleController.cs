using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    #region Fields & Properties

    public static BattleController instance { get; private set; }

    public Dictionary<int, List<Character>> characters = new Dictionary<int, List<Character>>();
    public int characterTurnIndex;
    public Spell playerSelectedSpell;
    public bool playerIsAttacking;
    public int totalExpEarned;
    public List<ItemData> droppedItems = new List<ItemData>();
    [SerializeField] private BattleSpawnPoint[] spawnPoints;

    private int actionTurn;
    public bool autoBattleEnabled = false;
    public float autoBattleSpeedMultiplier = 1f;
    public TMPro.TextMeshProUGUI speedButtonText;

    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeSingleton();
        InitializeBattleData();
        LaunchBattle();
        CharacterUIManager.instance.UpdateCharacterUI();
    }

    private void Update()
    {

    }

    #endregion

    #region Initialization

    private void InitializeSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    private void InitializeBattleData()
    {
        characters.Add(0, new List<Character>());
        characters.Add(1, new List<Character>());
    }

    private void LaunchBattle()
    {
        BattleLauncher.Instance.Launch();
    }

    #endregion

    #region Turn Handling

    private void NextTurn()
    {
        actionTurn = (actionTurn == 0) ? 1 : 0;
    }

    private void NextAction()
    {
        if (!AreBothTeamsAlive())
        {
            Debug.Log("Battle over!");
            return;
        }

        ResetPlayerInput();
        AdvanceTurnIndex();
        DeselectAllCharacters();

        Character currentCharacter = GetLivingCharacterWithRetries();
        if (currentCharacter != null && currentCharacter.health > 0)
        {
            currentCharacter.SetSelected(true);
            currentCharacter.HandlePoisonEffect();

            if (currentCharacter.health <= 0)
            {
                Debug.Log(currentCharacter.characterName + " died from poison!");
                CheckBattleOutcome();
                AdvanceTurnIndex();
                StartCoroutine(WaitAndContinue(1f));
                return;
            }

            ExecuteTurnForCurrentCharacter(currentCharacter);
        }
        else
        {
            Debug.LogWarning("No valid character found for this turn.");
            NextAction();
        }
    }

    private bool AreBothTeamsAlive()
    {
        return HasAliveCharacter(characters[0]) && HasAliveCharacter(characters[1]);
    }

    private bool HasAliveCharacter(List<Character> team)
    {
        foreach (Character character in team)
        {
            if (character != null && character.health > 0)
                return true;
        }
        return false;
    }

    private void ResetPlayerInput()
    {
        playerSelectedSpell = null;
        playerIsAttacking = false;
    }

    private void AdvanceTurnIndex()
    {
        characterTurnIndex++;
        if (characterTurnIndex >= characters[actionTurn].Count)
        {
            NextTurn();
            characterTurnIndex = 0;
        }
    }

    private void DeselectAllCharacters()
    {
        foreach (Character character in characters[0])
        {
            if (character != null)
                character.SetSelected(false);
        }
        foreach (Character character in characters[1])
        {
            if (character != null)
                character.SetSelected(false);
        }
    }

    private Character GetLivingCharacterWithRetries()
    {
        Character currentCharacter = GetCurrentCharacter();
        int tries = 0;
        while ((currentCharacter == null || currentCharacter.health <= 0) && tries < characters[actionTurn].Count)
        {
            characterTurnIndex = (characterTurnIndex + 1) % characters[actionTurn].Count;
            currentCharacter = GetCurrentCharacter();
            tries++;
        }
        return currentCharacter;
    }

    private void ExecuteTurnForCurrentCharacter(Character currentCharacter)
    {

        switch (actionTurn)
        {
            case 0:
                if (autoBattleEnabled)
                {
                    ActionButtonManager.instance.ToggleActionState(false);
                    StartCoroutine(PerformAutoPlayerTurn(currentCharacter));
                }
                else
                {
                    ActionButtonManager.instance.ToggleActionState(true);
                    SpellPanelManager.instance.BuildSpellList(currentCharacter.spells);
                }
                break;

            case 1:
                StartCoroutine(PerformAct());
                ActionButtonManager.instance.ToggleActionState(false);
                break;
        }
    }

    #endregion

    #region Act & Auto-Battle

    private IEnumerator PerformAct()
    {
        yield return new WaitForSeconds(1f);

        Character current = GetCurrentCharacter();

        if (current != null && current.health > 0)
        {
            Enemy enemy = current.GetComponent<Enemy>();

            if (enemy != null)
                enemy.Act();
            else
                Debug.LogWarning("Enemy component missing on character.");
        }
        else
        {
            Debug.LogWarning("No valid current character for enemy turn.");
        }

        CharacterUIManager.instance.UpdateCharacterUI();

        yield return new WaitForSeconds(2f);

        NextAction();
    }

    private IEnumerator PerformAutoPlayerTurn(Character character)
    {
        yield return new WaitForSeconds(1f / autoBattleSpeedMultiplier);

        if (AttemptAutoSpellCast(character))
            yield break;

        Character attackTarget = GetWeakestEnemy();
        character.Attack(attackTarget);

        CharacterUIManager.instance.UpdateCharacterUI();

        CheckBattleOutcome();

        yield return new WaitForSeconds(1f / autoBattleSpeedMultiplier);

        NextAction();
    }

    private bool AttemptAutoSpellCast(Character character)
    {
        if (character.spells != null && character.spells.Count > 0 && character.mana > 0)
        {
            Spell randomSpell = character.spells[Random.Range(0, character.spells.Count)];

            Character target = (randomSpell.spellType == Spell.SpellType.Heal)
                ? GetRandomPlayer()
                : GetWeakestEnemy();

            if (character.CastSpell(randomSpell, target))
            {
                CharacterUIManager.instance.UpdateCharacterUI();
                CheckBattleOutcome();
                StartCoroutine(WaitAndContinue(1f / autoBattleSpeedMultiplier));
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Character Selection, Attack & Spell Casting

    public void SelectCharacter(Character character)
    {
        Character currentCharacter = GetCurrentCharacter();

        if (playerIsAttacking)
        {
            if (IsAttackingOwnTeam(currentCharacter, character))
                return;

            DoAttack(currentCharacter, character);
        }
        else if (playerSelectedSpell != null)
        {
            HandleSpellCasting(currentCharacter, character);
        }
    }

    private bool IsAttackingOwnTeam(Character currentCharacter, Character target)
    {
        if (characters[0].Contains(currentCharacter) && characters[0].Contains(target))
        {
            Debug.Log("Can't attack your own team!");
            return true;
        }

        return false;
    }

    public void DoAttack(Character attacker, Character target)
    {
        attacker.Attack(target);
        CheckBattleOutcome();

        if (actionTurn == 0)
            NextAction();
    }

    private void HandleSpellCasting(Character caster, Character target)
    {
        if (playerSelectedSpell.isAoE)
        {
            if (caster.CastSpell(playerSelectedSpell, null))
            {
                CharacterUIManager.instance.UpdateCharacterUI();
                CheckBattleOutcome();
                NextAction();
            }
            else
            {
                Debug.LogWarning("Not enough mana to cast that AoE spell!");
            }

            return;
        }

        bool isHealingSpell = playerSelectedSpell.spellType == Spell.SpellType.Heal;

        if (isHealingSpell && !characters[0].Contains(target))
        {
            Debug.Log("Can only heal allies!");
            return;
        }
        else if (!isHealingSpell && characters[0].Contains(caster) && characters[0].Contains(target))
        {
            Debug.Log("Can't cast attack spells on teammates!");
            return;
        }

        if (caster.CastSpell(playerSelectedSpell, target))
        {
            CharacterUIManager.instance.UpdateCharacterUI();
            CheckBattleOutcome();
            NextAction();
        }
        else
        {
            Debug.LogWarning("Not enough mana to cast that spell!");
        }
    }

    #endregion

    #region Battle Setup

    public void StartBattle(List<GameObject> playerPrefabs, List<Character> enemies)
    {
        Debug.Log("Setup battle!");

        SpawnPlayerCharacters(playerPrefabs);
        SpawnEnemyCharacters(enemies);

        StartCoroutine(StartBattleRoutine());

        characterTurnIndex = -1;
        actionTurn = 0;
        NextAction();
    }

    private void SpawnPlayerCharacters(List<GameObject> playerPrefabs)
    {
        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            int spawnIndex = i + 5;

            if (spawnIndex < spawnPoints.Length && spawnPoints[spawnIndex] != null)
            {
                GameObject prefab = playerPrefabs[i];
                Character prefabChar = prefab.GetComponent<Character>();

                BattleSpawnPoint spawnPoint = spawnPoints[spawnIndex].GetComponent<BattleSpawnPoint>();
                Character spawnedChar = spawnPoint.Spawn(prefabChar);
                spawnedChar.characterData=prefabChar.characterData;

                spawnedChar.InitializeFromData();
                characters[0].Add(spawnedChar);
            }
        }
    }

    private void SpawnEnemyCharacters(List<Character> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (spawnPoints[i] != null)
            {
                Character enemyCharacter = spawnPoints[i].Spawn(enemies[i]);

                if (enemyCharacter != null)
                {
                    characters[1].Add(enemyCharacter);
                }
                else
                {
                    Debug.LogError("Failed to spawn enemy character at index " + i);
                }
            }
        }
    }

    private IEnumerator StartBattleRoutine()
    {
        yield return StartCoroutine(FadeController.instance.FadeIn(1.5f));
    }

    #endregion

    #region Getters

    public Character GetCurrentCharacter()
    {
        if (characters[actionTurn].Count == 0)
            return null;

        Character character = characters[actionTurn][characterTurnIndex];

        if (character.health > 0)
            return character;

        for (int i = 0; i < characters[actionTurn].Count; i++)
        {
            characterTurnIndex = (characterTurnIndex + 1) % characters[actionTurn].Count;
            Character next = characters[actionTurn][characterTurnIndex];

            if (next.health > 0)
                return next;
        }

        return null;
    }

    public Character GetRandomPlayer()
    {
        if (characters[0].Count == 0) return null;
        return characters[0][Random.Range(0, characters[0].Count)];
    }

    public Character GetWeakestEnemy()
    {
        Character weakestEnemy = characters[1][0];

        foreach (Character character in characters[1])
        {
            if (character.health < weakestEnemy.health)
            {
                weakestEnemy = character;
            }
        }

        return weakestEnemy;
    }

    #endregion

    #region Battle Outcome & End Battle

    private void CheckBattleOutcome()
    {
        bool playersAlive = characters[0].Exists(c => c.health > 0);
        bool enemiesAlive = characters[1].Exists(c => c.health > 0);

        if (!playersAlive)
            EndBattle(false);
        else if (!enemiesAlive)
            EndBattle(true);
    }

    private void EndBattle(bool playerWon)
    {
        StartCoroutine(EndBattleCoroutine(playerWon));
    }

    private IEnumerator EndBattleCoroutine(bool playerWon)
    {
        Debug.Log(playerWon ? "You won the battle!" : "You lost the battle!");

        Time.timeScale = 0.5f;

        yield return StartCoroutine(FadeController.instance.FadeOut(1.5f));

        yield return new WaitForSeconds(0.5f);

        BattleResultController.Instance.ShowEndScreen(playerWon);

        if (playerWon)
        {
            foreach(Character c in BattleController.instance.characters[0])
            {
                if(c is PartyMember pm)
                {
                    pm.GainExp(totalExpEarned);
                }
            }

            string zoneID=BattleLauncher.Instance.GetCurrentZoneID();
            if (!string.IsNullOrEmpty(zoneID))
            {
                PlayerPrefs.SetInt(zoneID, 1);
                PlayerPrefs.Save();
                ZoneSaveManager.Instance.RegisterDefeat(zoneID);
            }

            if (droppedItems.Count > 0)
            {
                ItemData rewardItem = droppedItems.Find(item => item != null);
                if (rewardItem != null)
                {
                    InventoryManager.instance.AddItem(rewardItem); 
                    WinBattleController.instance.ShowReward(rewardItem,totalExpEarned,50);
                    Debug.Log("Player received item:" + rewardItem.name);
                }
                droppedItems.Clear();
            }
        }

        enabled = false;

        Time.timeScale = 1f;

        autoBattleSpeedMultiplier = 1f;

        if (speedButtonText != null)
            speedButtonText.text = "Speed x1";
    }

    #endregion

    #region Auto Battle & Speed Control

    public void ToggleAutoBattle()
    {
        autoBattleEnabled = !autoBattleEnabled;

        Debug.Log("Auto Battle " + (autoBattleEnabled ? "Enabled" : "Disabled"));

        if (autoBattleEnabled && actionTurn == 0)
        {
            ActionButtonManager.instance.ToggleActionState(false);
            StopAllCoroutines();
            StartCoroutine(PerformAutoPlayerTurn(GetCurrentCharacter()));
        }
    }

    public void CycleAutoBattleSpeed()
    {
        if (!autoBattleEnabled)
        {
            Debug.Log("Auto battle must be enabled to change speed.");
            return;
        }

        if (autoBattleSpeedMultiplier == 1f)
            autoBattleSpeedMultiplier = 2f;
        else if (autoBattleSpeedMultiplier == 2f)
            autoBattleSpeedMultiplier = 3f;
        else
            autoBattleSpeedMultiplier = 1f;

        Time.timeScale = autoBattleSpeedMultiplier;

        if (speedButtonText != null)
            speedButtonText.text = "Speed x" + autoBattleSpeedMultiplier.ToString("0");

        Debug.Log("Auto Battle Speed set to " + autoBattleSpeedMultiplier + "x");
    }

    private IEnumerator WaitAndContinue(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextAction();
    }

    #endregion
}
