using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BattleZone : MonoBehaviour
{
    [Header("敌人配置")]
    [SerializeField] private List<Character> enemies;
    [SerializeField] private string zoneID;

    private bool hasLaunched=false;

    private void Start()
    {
        if(ZoneSaveManager.Instance.IsDefeated(zoneID))
            gameObject.SetActive(false);
    }
    public void Launch()
    {
        PlayerController.instance.SavePosition();
        PrepareBattle();
    }
    private void PrepareBattle()
    {
        BattleLauncher.Instance.PrepareBattle(enemies,zoneID);
    }

    public void MarkZoneAsDefeated()
    {
        ZoneSaveManager.Instance.RegisterDefeat(zoneID);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!hasLaunched && collision.CompareTag("Player"))
        {
            hasLaunched = true;
            Launch();
        }
    }

    private void Reset()
    {
        GenerateZoneID();
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(zoneID))
            GenerateZoneID();
    }

    private void GenerateZoneID()
    {
        zoneID=Guid.NewGuid().ToString();
        if (!Application.isPlaying)
            EditorUtility.SetDirty(this);
    }
}
