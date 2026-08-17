using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class ItemInfoUIManager : MonoBehaviour
{
    public static ItemInfoUIManager instance;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI goldText;
    public GameObject buyButton;
    public ItemData selectedItem;
    public GameObject shopPanel;

    [Header("Shop Items")]
    public List<ItemData> shopItems = new List<ItemData>();

    [Header("Item Buttons")]
    public List<ItemButtonUI> itemButtons = new List<ItemButtonUI>();

    private void Awake()
    {
        instance= this;
    }
    private void Start()
    {
        UpdateGold();
        itemNameText.text = string.Empty;
        itemDescription.text = string.Empty;
        itemPrice.text = string.Empty;
        infoText.text = string.Empty;
        buyButton.SetActive(false);
        SetupShopButtons();
    }

    public void UpdateGold()
    {
        goldText.text = GoldManager.instance.gold.ToString();
    }

    public void DisplayItemInfo(ItemData item)
    {
        if (item == null) return;
        buyButton.SetActive(true);
        selectedItem = item;
        itemNameText.text = item.displayName;
        itemDescription.text = item.description;
        itemPrice.text = item.buyPrice.ToString();
        StringBuilder sb = new StringBuilder();

        if (item.statsOfItem != null && item.statsOfItem.Length > 0)
        {
            sb.AppendLine("<b>Stats:</b>");
            foreach (var stat in item.statsOfItem)
            {
                sb.AppendLine($"{stat.type} <color=green>+{stat.value}</color>");
            }
        }

        infoText.text = sb.ToString();
    }

    public void BuySelectedItem()
    {
        if (selectedItem == null) return;

        if (GoldManager.instance.gold >= selectedItem.buyPrice)
        {
            GoldManager.instance.ReduceGold(selectedItem.buyPrice);
            InventoryManager.instance.AddItem(selectedItem);
            Debug.Log("Item bought: " + selectedItem.displayName);
            UpdateGold();
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    public void OncloseShopgPanel()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnOpenShopPanel()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void SetupShopButtons()
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            if (i < shopItems.Count)
            {
                itemButtons[i].gameObject.SetActive(true);
                itemButtons[i].SetItem(shopItems[i]);
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
