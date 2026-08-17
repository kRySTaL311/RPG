using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BattleLauncher : MonoBehaviour
{
    public static BattleLauncher Instance { get; private set; }
    public string currentZoneID;
    private void Awake()
    {
        SetupSingleton();
    }

    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    public List<Character> Enemies { get; private set; }
    public void PrepareBattle(List<Character> enemies,string zoneID)
    {
        Enemies= enemies;
        currentZoneID = zoneID;
        LoadBattleScene();
    }
    public void LoadBattleScene()
    {
        SceneManager.LoadScene("Battle");
        Debug.Log("JIAZAI");
    }
    public void Launch()
    {
        BattleController.instance.StartBattle(TeamManager.Instance.playerTeamPrefabs,Enemies);
    }

    public string GetCurrentZoneID()
    {
        return currentZoneID;
    }
}