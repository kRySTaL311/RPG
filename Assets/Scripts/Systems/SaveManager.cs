using System.Collections;
using UnityEngine;

[System.Serializable]
public class CharacterEquipData
{
    public string characterName;
    public string weaponId;
    public string armorId;
    public string bootsId;
    public string helmId;
}

[System.Serializable]
public class SaveData
{

    public SInventorySlot[] inventory;

    public CharacterEquipData[] characterEquipment;
}

[System.Serializable]
public struct SInventorySlot
{
    public bool occupied;    
    public string itemId;    
    public int quantity;      
}

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;
    [Header("Auto-Loaded Items")]
    public ItemData[] items;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        items = Resources.LoadAll<ItemData>("Items");
    }

    private void Start()
    {
        StartCoroutine(LoadInventory()); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) SaveInventory();              
        if (Input.GetKeyDown(KeyCode.M)) PlayerPrefs.DeleteAll(); 
    }

    IEnumerator LoadInventory()
    {
        yield return new WaitForEndOfFrame(); 
        if (PlayerPrefs.HasKey("Save"))       
        {
            Load(); 
        }
    }
    public void SaveInventory()
    {
        SaveData data = new SaveData();
        data.inventory = new SInventorySlot[InventoryManager.instance.slots.Length];

        for (int i = 0; i < InventoryManager.instance.slots.Length; i++)
        {
            var slot = InventoryManager.instance.slots[i]; 

            data.inventory[i] = new SInventorySlot
            {
                occupied = slot.item != null,                     
                itemId = slot.item != null ? slot.item.id : null,
                quantity = slot.quantity                          
            };
        }

        int count = TeamManager.Instance.playerTeamPrefabs.Count;
        data.characterEquipment = new CharacterEquipData[count];
        for (int i = 0; i < count; i++)
        {
            var go = TeamManager.Instance.playerTeamPrefabs[i];
            var chr = go.GetComponent<Character>();
            var ce = new CharacterEquipData
            {
                characterName = chr.characterData.characterName,
                weaponId = chr.equippedWeapon?.id,
                armorId = chr.equippedArmor?.id,
                bootsId = chr.equippedBoots?.id,
                helmId = chr.equippedHelm?.id
            };
            data.characterEquipment[i] = ce;
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Save", json); 
        Debug.Log("Saved inventory:\n" + JsonUtility.ToJson(data, true)); 
    }

    public void Load()
    {
        string json = PlayerPrefs.GetString("Save");
        if (string.IsNullOrEmpty(json))
            return;

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        if (saveData == null || saveData.inventory == null)
        {

            Debug.LogWarning("Load skipped: no valid save data.");
            return;
        }

        int count = Mathf.Min(saveData.inventory.Length, InventoryManager.instance.slots.Length);
        for (int i = 0; i < count; i++)
        {
            var slotData = saveData.inventory[i];
            if (!slotData.occupied)
            {
                InventoryManager.instance.slots[i].item = null;
                InventoryManager.instance.slots[i].quantity = 0;
            }
            else
            {
                ItemData item = GetItemByID(slotData.itemId);
                InventoryManager.instance.slots[i].item = item;
                InventoryManager.instance.slots[i].quantity = slotData.quantity;
            }
        }

        if (saveData.characterEquipment != null)
        {
            int charCount = saveData.characterEquipment.Length;
            for (int i = 0; i < charCount; i++)
            {
                if (i >= TeamManager.Instance.playerTeamPrefabs.Count)
                    break;

                CharacterEquipData ce = saveData.characterEquipment[i];
                GameObject go = TeamManager.Instance.playerTeamPrefabs[i];
                Character chr = go.GetComponent<Character>();

                chr.equippedWeapon = GetItemByID(ce.weaponId);
                chr.equippedArmor = GetItemByID(ce.armorId);
                chr.equippedBoots = GetItemByID(ce.bootsId);
                chr.equippedHelm = GetItemByID(ce.helmId);

                chr.InitializeFromData();
            }
        }

        InventoryManager.instance.UpdateUI();

    }

    public ItemData GetItemByID(string id)
    {
        foreach (var item in items)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }
}

