using System.Collections.Generic;
using UnityEngine;

public class ZoneSaveManager : MonoBehaviour
{
    public static ZoneSaveManager Instance { get; private set; }

    private readonly HashSet<string> defeatedZoneIDs = new HashSet<string>();

    private const string ListKey = "DefeatedZoneList";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadDefeatedList();

    }

    private void LoadDefeatedList()
    {

        string csv = PlayerPrefs.GetString(ListKey, "");
        if (!string.IsNullOrEmpty(csv))
        {
            foreach (var id in csv.Split(','))
                defeatedZoneIDs.Add(id);
        }
    }

    private void SaveDefeatedList()
    {
        PlayerPrefs.SetString(ListKey, string.Join(",", defeatedZoneIDs));
        PlayerPrefs.Save();
    }

    public void RegisterDefeat(string zoneID)
    {
        if (defeatedZoneIDs.Add(zoneID))
            SaveDefeatedList();
    }

    public bool IsDefeated(string zoneID)
        => defeatedZoneIDs.Contains(zoneID);

    public void ClearAllDefeats()
    {
        foreach (var id in defeatedZoneIDs)
            PlayerPrefs.DeleteKey(id);
        defeatedZoneIDs.Clear();
        PlayerPrefs.DeleteKey(ListKey);
        PlayerPrefs.Save();
    }

}