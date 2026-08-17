using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingRecipeUI : MonoBehaviour
{
    public CraftingRecipe recipe;
    public TextMeshProUGUI buttonText;

    public Image icon;
    public TextMeshProUGUI itemName;
    public Image[] resourceCosts;
    public Color canCraftColor, cannotCraftColor;
    public bool canCraft;

    private void OnEnable()
    {
        UpdateCanCraft();
    }

    public void UpdateCanCraft()
    {

        if (recipe == null || InventoryManager.instance == null)
        {
            canCraft = false;
            return;
        }

        canCraft = true;

        for (int i = 0; i < recipe.costs.Length; i++)
        {

            if (!InventoryManager.instance.HasItem(recipe.costs[i].item, recipe.costs[i].quantity))
            {
                canCraft = false;
                break;
            }
        }

        buttonText.color = canCraft ? canCraftColor : cannotCraftColor;

    }

    void Start()
    {
        icon.sprite = recipe.itemToCraft.icon;
        itemName.text = recipe.itemToCraft.displayName;

        for (int i = 0; i < resourceCosts.Length; i++)
        {
            if (i < recipe.costs.Length)
            {
                resourceCosts[i].gameObject.SetActive(true);
                resourceCosts[i].sprite = recipe.costs[i].item.icon;
                resourceCosts[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = recipe.costs[i].quantity.ToString();
            }
            else
            {
                resourceCosts[i].gameObject.SetActive(false);
            }

        }
    }

    public void OnClickCraftingButton()
    {

        UpdateCanCraft();
        if (canCraft)
        {
            CraftingWindow.instance.Craft(recipe);
        }
    }

}
