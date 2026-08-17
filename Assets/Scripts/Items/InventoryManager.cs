using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public PlayerController player;
    [Header("Inventory Panel")]
    public GameObject inventoryPanel;
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    [Header("Character List UI")]

    public TextMeshProUGUI currentCharacterText;

    [Header("Selected Item Info")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName, selectedItemDescription, selectedItemstatName, selectedItemsStatValue;
    public GameObject useButton, equipButton, unequipButton;

    public List<Character> activeTeam = new List<Character>();
    private Character selectedCharacter;
    public TextMeshProUGUI goldText;

    [Header("Buttons (Fixed Team Slots)")]
    public Button[] teamButtons = new Button[5];

    [Header("UI Display")]
    public TextMeshProUGUI statsText;
    private int lastSelectedCharacterIndex = -1;

    private const int WeaponSlotIndex = 0;
    private const int HelmSlotIndex = 1;
    private const int ArmorSlotIndex = 2;
    private const int BootsSlotIndex = 3;
    private const int ReservedSlotCount = 4;
    private const int GeneralInventoryStartIndex = ReservedSlotCount;

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
    }

    void Start()
    {
        SetupButtons();

        slots = new ItemSlot[uiSlots.Length];
        for (int x = 0; x < slots.Length; x++)
        {
            slots[x] = new ItemSlot();
            uiSlots[x].index = x;
            uiSlots[x].Clear();
        }
        for (int i = 0; i < TeamManager.Instance.playerTeamPrefabs.Count; i++)
        {
            GameObject go = TeamManager.Instance.playerTeamPrefabs[i];
            Character ch=go.GetComponent<Character>();
            CharacterData cd = ch.characterData;

            CharacterSaveManager.LoadCharacter(cd, ch);
            ch.InitializeFromData();
        }

        SaveManager.instance.Load();
        ClearSelectedItemWindow();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void ShowCharacterStats(int index)
    {
        if (index >= TeamManager.Instance.playerTeamPrefabs.Count)
        {
            statsText.text = $"Slot {index + 1} is empty.";
            return;
        }

        GameObject characterGO = TeamManager.Instance.playerTeamPrefabs[index];
        Character character = characterGO.GetComponent<Character>();

        if (character == null || character.characterData == null)
        {
            statsText.text = $"Character data missing in slot {index + 1}.";
            return;
        }

        selectedCharacter = character;
        lastSelectedCharacterIndex = index;

        string stats = $"<b>{character.characterData.characterName}</b>\n" +
                       $"LV: {character.characterData.level}\n" +
                       $"XP: {character.characterData.currentExp} / {character.characterData.expToNextLevel}\n" +
                       $"HP: {character.GetTotalStat(StatsType.HP)}\n" +
                       $"MP: {character.GetTotalStat(StatsType.MP)}\n" +
                       $"AD: {character.GetTotalStat(StatsType.AD)}\n" +
                       $"DEF: {character.GetTotalStat(StatsType.DF)}\n" +
                       $"CR: {character.GetTotalStat(StatsType.CR) * 100f}%\n" +
                       $"CRD: x{character.GetTotalStat(StatsType.CRD)}\n" +
                       $"PD: {character.GetTotalStat(StatsType.PD)}";

        statsText.text = stats;

        UpdateUI();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < teamButtons.Length; i++)
        {
            int index = i;
            teamButtons[i].onClick.AddListener(() => ShowCharacterStats(index));

            if (index < TeamManager.Instance.playerTeamPrefabs.Count)
            {
                teamButtons[i].gameObject.SetActive(true);
                teamButtons[i].onClick.AddListener(() => ShowCharacterStats(index));
                GameObject characterGO = TeamManager.Instance.playerTeamPrefabs[index];
                Character character = characterGO.GetComponent<Character>();
                if (character != null && character.characterData != null && character.characterIcon != null)
                {
                    Image imageComponent = teamButtons[i].GetComponent<Image>();
                    if (imageComponent != null)
                    {
                        imageComponent.sprite = character.characterIcon;
                    }
                }
            }
            else
            {

                teamButtons[i].gameObject.SetActive(false);
            }
        }
    }
    ItemSlot GetEmptySlot()
    {
        for (int i = GeneralInventoryStartIndex; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i];
        }
        return null;
    }

    ItemSlot GetItemStack(ItemData item)
    {
        for (int i = GeneralInventoryStartIndex; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
                return slots[i];
        }
        return null;
    }

    public void AddItem(ItemData item)
    {
        if (item.canStack)
        {
            ItemSlot stackSlot = GetItemStack(item);
            if (stackSlot != null)
            {
                stackSlot.quantity++;
                UpdateUI();
                SaveManager.instance.SaveInventory();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            SaveManager.instance.SaveInventory();
            return;
        }
        ThrowItem(item);
        SaveManager.instance.SaveInventory();
    }
    public void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, player.transform.position, player.transform.rotation);
    }

    public void UpdateUI()
    {
        goldText.text = GoldManager.instance.gold.ToString();
        if (selectedCharacter != null)
        {
            slots[WeaponSlotIndex].item = selectedCharacter.equippedWeapon;
            slots[WeaponSlotIndex].quantity = selectedCharacter.equippedWeapon != null ? 1 : 0;

            slots[ArmorSlotIndex].item = selectedCharacter.equippedArmor;
            slots[ArmorSlotIndex].quantity = selectedCharacter.equippedArmor != null ? 1 : 0;

            slots[BootsSlotIndex].item = selectedCharacter.equippedBoots;
            slots[BootsSlotIndex].quantity = selectedCharacter.equippedBoots != null ? 1 : 0;

            slots[HelmSlotIndex].item = selectedCharacter.equippedHelm;
            slots[HelmSlotIndex].quantity = selectedCharacter.equippedHelm != null ? 1 : 0;
        }
        else
        {
            for (int i = 0; i < ReservedSlotCount; i++)
            {
                slots[i].item = null;
                slots[i].quantity = 0;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                uiSlots[i].Set(slots[i]);
            }
            else
            {
                uiSlots[i].Clear();
            }
        }

        if (currentCharacterText != null && selectedCharacter != null)
            currentCharacterText.text = $"Character: {selectedCharacter.characterName}";

        CraftingWindow.instance?.RefreshAllRecipes();
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemstatName.text = string.Empty;
        selectedItemsStatValue.text = string.Empty;

        foreach (var stat in selectedItem.item.statsOfItem)
        {
            selectedItemstatName.text += stat.type + "\n";
            selectedItemsStatValue.text += "+ " + stat.value + "\n";
        }

        bool isEquipSlot = index >= 0 && index < ReservedSlotCount;
        ItemType type = selectedItem.item.type;

        useButton.SetActive(type == ItemType.Consumable);
        equipButton.SetActive(!isEquipSlot && IsEquipableType(type));
        unequipButton.SetActive(isEquipSlot && selectedCharacter != null && selectedCharacter.GetEquippedItem(type) != null);
    }

    bool IsEquipableType(ItemType type)
    {
        return type == ItemType.EquipableWeapon ||
               type == ItemType.EquipableArmor ||
               type == ItemType.EquipableBoots ||
               type == ItemType.EquipableHelm;
    }
    void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemstatName.text = string.Empty;
        selectedItemsStatValue.text = string.Empty;
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
    }
    public void RemoveSelectedItem()
    {
        selectedItem.quantity--;
        if (selectedItem.quantity == 0)
        {
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }
        SaveManager.instance.SaveInventory();
        UpdateUI();
    }
    public void OnUseButton()
    {
        if (selectedItem == null || selectedItem.item == null || selectedCharacter == null)
            return;

        if (selectedItem.item.type == ItemType.Consumable)
        {
            foreach (var stat in selectedItem.item.statsOfItem)
            {
                switch (stat.type)
                {
                    case StatsType.HP:
                        selectedCharacter.maxHealth += Mathf.RoundToInt(stat.value);
                        break;
                    case StatsType.MP:
                        selectedCharacter.mana += Mathf.RoundToInt(stat.value);
                        break;
                    case StatsType.XP:
                        if (selectedCharacter is PartyMember partyMember)
                            partyMember.GainExp(Mathf.RoundToInt(stat.value));
                        else
                            selectedCharacter.characterData.currentExp += Mathf.RoundToInt(stat.value);
                        break;
                    default:
                        Debug.Log($"Stat {stat.type} is not used for consumables.");
                        break;
                }
            }

            selectedCharacter.InitializeFromData();
        }

        RemoveSelectedItem();
        selectedCharacter.InitializeFromData();

        if (lastSelectedCharacterIndex >= 0)
            ShowCharacterStats(lastSelectedCharacterIndex);

        UpdateUI();
        SaveManager.instance.SaveInventory();
    }
    public void OnEquipButton()
    {
        if (selectedCharacter == null || selectedItem == null)
            return;

        ItemData itemToEquip = selectedItem.item;
        ItemType itemType = itemToEquip.type;

        int slotIndex = itemType switch
        {
            ItemType.EquipableWeapon => WeaponSlotIndex,
            ItemType.EquipableArmor => ArmorSlotIndex,
            ItemType.EquipableBoots => BootsSlotIndex,
            ItemType.EquipableHelm => HelmSlotIndex,
            _ => -1
        };

        if (slotIndex == -1)
            return;

        ItemData oldItem = selectedCharacter.Unequip(itemType);
        if (oldItem != null)
            AddItem(oldItem);

        selectedCharacter.Equip(itemToEquip);
        selectedCharacter.InitializeFromData();

        slots[slotIndex].item = itemToEquip;
        slots[slotIndex].quantity = 1;

        slots[selectedItemIndex].quantity--;
        if (slots[selectedItemIndex].quantity <= 0)
            slots[selectedItemIndex].item = null;

        ClearSelectedItemWindow();

        if (lastSelectedCharacterIndex >= 0)
            ShowCharacterStats(lastSelectedCharacterIndex);

        UpdateUI();
        SaveManager.instance.SaveInventory();
    }

    public void OnUnequipButton()
    {
        if (selectedCharacter == null || selectedItem == null)
            return;

        ItemType type = selectedItem.item.type;
        int index = selectedItemIndex;

        if (index < 0 || index >= ReservedSlotCount)
            return;

        ItemData unequipped = selectedCharacter.Unequip(type);
        selectedCharacter.InitializeFromData();

        if (unequipped != null)
        {
            slots[index].item = null;
            slots[index].quantity = 0;
            AddItem(unequipped);
        }

        ClearSelectedItemWindow();

        if (lastSelectedCharacterIndex >= 0)
            ShowCharacterStats(lastSelectedCharacterIndex);
        SaveManager.instance.SaveInventory();
        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {

        for(int i = GeneralInventoryStartIndex; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].quantity--;
                if (slots[i].quantity == 0)
                {
                    slots[i].item = null;
                    ClearSelectedItemWindow() ;
                }
                UpdateUI();
                return;
            }
        }
    }
    public bool HasItem(ItemData item,int quantity)
    {
        if (slots == null) return false;

        int amount = 0;
        for(int i = GeneralInventoryStartIndex; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                amount += slots[i].quantity;
            }
            if (amount >= quantity)
            {
                return true;
            }
        }
        return false;
    }
    public void SortInventory()
    {
        List<ItemSlot> generalInventory = new List<ItemSlot>();

        for (int i = GeneralInventoryStartIndex; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                generalInventory.Add(new ItemSlot
                {
                    item = slots[i].item,
                    quantity = slots[i].quantity
                });
            }
        }

        generalInventory.Sort((a, b) => a.item.displayName.CompareTo(b.item.displayName));

        for (int i = GeneralInventoryStartIndex; i < slots.Length; i++)
        {
            slots[i].item = null;
            slots[i].quantity = 0;
        }

        for (int i = 0; i < generalInventory.Count; i++)
        {
            int slotIndex = GeneralInventoryStartIndex + i;
            slots[slotIndex].item = generalInventory[i].item;
            slots[slotIndex].quantity = generalInventory[i].quantity;
        }

        UpdateUI();
        SaveManager.instance.SaveInventory();
    }
    public void ToggleInventory()
    {
        SetupButtons();
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        if (inventoryPanel.activeSelf)
        {
            if (TeamManager.Instance.playerTeamPrefabs.Count > 0)
                ShowCharacterStats(0); 
        }

        ClearSelectedItemWindow();
        SortInventory();
    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int quantity;
}
