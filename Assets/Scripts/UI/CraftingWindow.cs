using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingWindow : MonoBehaviour
{
    public GameObject craftingPanel;
    public CraftingRecipeUI[] recipeUIs;
    public static CraftingWindow instance;

    private void Awake()
    {
        instance = this;
    }

    public void Craft(CraftingRecipe recipe)
    {

        for (int i = 0; i < recipe.costs.Length; i++)
        {
            if (!InventoryManager.instance.HasItem(recipe.costs[i].item, recipe.costs[i].quantity))
                return;
        }

        for (int i = 0; i < recipe.costs.Length; i++)
        {

            for (int x = 0; x < recipe.costs[i].quantity; x++)
            {

                InventoryManager.instance.RemoveItem(recipe.costs[i].item);
            }
        }

        InventoryManager.instance.AddItem(recipe.itemToCraft);

        RefreshAllRecipes();
    }

    public void RefreshAllRecipes()
    {
        if (recipeUIs == null)
            return;

        for (int i = 0; i < recipeUIs.Length; i++)
        {
            recipeUIs[i].UpdateCanCraft();
        }
    }

    public void OncloseCraftingPanel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnOpenCraftingPanel()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;

        RefreshAllRecipes();
    }
}