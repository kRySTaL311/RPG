using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeamManager : MonoBehaviour
{

    public static TeamManager Instance;
    [Header("Team Setup")]
    public List<GameObject> playerTeamPrefabs = new List<GameObject>();
    public List<GameObject> reserveTeamPrefabs = new List<GameObject>();
    public int maxTeamSize = 5;
    private const string SaveTeamMember = "SavedTeam";
    private void Awake()
    {
        SetupSingleton();
        LoadTeam();
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DeleteSavedTeam();
            Debug.Log("Saved team deleted.");
        }
    }
    private void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private bool CanAddToTeam()
    {
        return playerTeamPrefabs.Count < maxTeamSize;
    }
    public bool AddToTeam(GameObject characterprefab)
    {
        if (CanAddToTeam())
        {
            playerTeamPrefabs.Add(characterprefab);
            SaveTeam();
            return true;
        }
        else
        {
            if (!reserveTeamPrefabs.Contains(characterprefab))
            {
                reserveTeamPrefabs.Add(characterprefab);
                SaveTeam();
                return true;
            }
        }
        return false;
    }
    public void RemoveFromTeam(GameObject characterprefab)
    {

        playerTeamPrefabs.Remove(characterprefab);
        SaveTeam();
    }
    public void SaveTeam()
    {
        List<string> mainNames = GetPrefabNames(playerTeamPrefabs);
        List<string> reserveNames = GetPrefabNames(reserveTeamPrefabs);
        SaveData data = new SaveData
        {
            names = mainNames,
            reserveNames = reserveNames
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveTeamMember, json);
        PlayerPrefs.Save();
    }

    public void LoadTeam()
    {
        if (PlayerPrefs.HasKey(SaveTeamMember))
        {
            string json=PlayerPrefs.GetString(SaveTeamMember);
            SaveData data=JsonUtility.FromJson<SaveData>(json);
            RestoreTeam(data);
        }
    }
    private void RestoreTeam(SaveData data)
    {
        playerTeamPrefabs.Clear();
        reserveTeamPrefabs.Clear();

        foreach (string name in data.names)
        {
            GameObject prefab = Resources.Load<GameObject>("Characters/" + name);
            if(prefab != null) playerTeamPrefabs.Add(prefab);
            else Debug.LogWarning($"Could not load team prefab:{name}");
        }

        foreach(string name in data.reserveNames)
        {
            GameObject prefab = Resources.Load<GameObject>("Characters/" + name);
            if (prefab != null) reserveTeamPrefabs.Add(prefab);
            else Debug.LogWarning($"Could not load reserve prefab:{name}");
        }
    }
    private List<string> GetPrefabNames(List<GameObject> prefabList)
    {
        List<string> names = new List<string>();
        foreach(GameObject prefab in prefabList)
        {
            string cleanName = prefab.name.Replace("(Clone)", "").Trim();
            names.Add(cleanName);
        }
        return names;
    }
    private void DeleteSavedTeam()
    {
        PlayerPrefs.DeleteKey(SaveTeamMember);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class SaveData
    {
        public List<string> names;
        public List<string> reserveNames;
    }

    public void SwapTeamMembers(int indexA,int indexB)
    {
        if (IsValidIndex(indexA) && IsValidIndex(indexB))
        {
            GameObject temp = playerTeamPrefabs[indexA];
            playerTeamPrefabs[indexA] = playerTeamPrefabs[indexB];
            playerTeamPrefabs[indexB] = temp;
            SaveTeam();
        }
    }

    public void MoveTeamMember(int fromIndex,int toIndex)
    {
        if (IsValidIndex(fromIndex) && IsValidIndex(toIndex) && fromIndex != toIndex)
        {
            GameObject movingMember=playerTeamPrefabs[fromIndex];
            playerTeamPrefabs.RemoveAt(fromIndex);
            playerTeamPrefabs.Insert(toIndex, movingMember);
            SaveTeam();
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < playerTeamPrefabs.Count;
    }

    public bool IsInTeam(GameObject character)
    {
        return playerTeamPrefabs.Contains(character) || reserveTeamPrefabs.Contains(character);
    }
}
