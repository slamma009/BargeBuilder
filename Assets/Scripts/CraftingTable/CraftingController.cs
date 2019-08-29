using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the UI For the Crafting Canvas
/// </summary>
public class CraftingController : MonoBehaviour
{
    /// <summary>Tabs for the crafting panel</summary>
    public CraftingTableTab[] Tabs;

    /// <summary>The selected color for a tab</summary>
    public Color TabSelectedColor;

    /// <summary>The unselected color for a tab</summary>
    public Color TabInactiveColor;

    /// <summary>The selected color for a Recipe</summary>
    public Color RecipeSelectedColor;

    /// <summary>The unselected color for a Recipe</summary>
    public Color RecipeInactiveColor;

    public Color EnoughItemsForRecipeTextColor;

    public Color NotEnoughItemsForRecipeTextColor;

    /// <summary>The Controller for the Inventory Console</summary>
    public UIConsole UIConsoleController;

    /// <summary>Prefab for a <see cref="CraftingItemDisplay"/></summary>
    public CraftingItemDisplay ItemDisplayPrefab;

    /// <summary>Prefab for a <see cref="RecipeItemDisplay"/></summary>
    public RecipeItemDisplay RecipeDisplayPrefab;

    /// <summary>The main panel to show <see cref="CraftingItemDisplay"/> on</summary>
    public Transform MainPanel;

    /// <summary>The main panel to show <see cref="RecipeItemDisplay"/> on</summary>
    public Transform CraftingPanel;

    /// <summary>The currently active items being displayed</summary>
    private List<CraftingItemDisplay> _activeCraftingItems = new List<CraftingItemDisplay>();

    /// <summary>The currently active items being displayed</summary>
    private List<RecipeItemDisplay> _activeRecipeItems = new List<RecipeItemDisplay>();

    /// <summary>Our current active index for the tab</summary>
    private int _activeTabIndex = -1;

    /// <summary>Our current active index for the recipe</summary>
    private int _activeRecipeIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        for (var i=0; i<Tabs.Length; ++i)
        {
            Tabs[i].TabImage.color = TabInactiveColor;
            Tabs[i].TabButton.enabled = true;
        }
        SetActiveTab(0);
    }

    /// <summary>
    /// Sets the tab as active and shows all <see cref="CraftingItemDisplay"/> for that tab
    /// </summary>
    /// <param name="index">Index of the tab</param>
    public void SetActiveTab(int index)
    {
        if (index == _activeTabIndex)
            return;

        // Reset the currently active tab
        if (_activeTabIndex >= 0)
        {
            Tabs[_activeTabIndex].TabImage.color = TabInactiveColor;
            Tabs[_activeTabIndex].TabButton.enabled = true;
        }

        // Destroy all crafting items being displayed
        foreach (var item in _activeCraftingItems)
        {
            Destroy(item.gameObject);
        }
        _activeCraftingItems.Clear();

        // Set the new tab to active
        _activeTabIndex = index;
        Tabs[_activeTabIndex].TabImage.color = TabSelectedColor;
        Tabs[_activeTabIndex].TabButton.enabled = false;

        _activeRecipeIndex = -1;
        SetActiveRecipe(-2);
        // Create all display items for the new tab
        for (var i = 0; i < Tabs[_activeTabIndex].Items.Length; ++i)
        {
            CraftingItemDisplay newDisplay = Instantiate(ItemDisplayPrefab, MainPanel);
            newDisplay.ArrayIndex = i;
            newDisplay.SetImage(Tabs[_activeTabIndex].Items[i].Image);
            newDisplay.button.onClick.AddListener(delegate { SetActiveRecipe(newDisplay.ArrayIndex); });
            newDisplay.SetDisplayColor(RecipeInactiveColor);
            _activeCraftingItems.Add(newDisplay);
        }
    }

    /// <summary>
    /// Sets the Recipe as active and shows all <see cref="RecipeItemDisplay"/> for that recipe
    /// </summary>
    /// <param name="index">Index of the Recipe</param>
    public void SetActiveRecipe(int index)
    {
        if (index == _activeRecipeIndex)
            return;
        
        // Reset the active index color if active index is set
        if (_activeRecipeIndex >= 0)
            _activeCraftingItems[_activeRecipeIndex].SetDisplayColor(RecipeInactiveColor);

        // Destroy all active recipe items
        for(var i=0; i< _activeRecipeItems.Count; ++i)
        {
            Destroy(_activeRecipeItems[i].gameObject);
        }
        _activeRecipeItems.Clear();

        _activeRecipeIndex = index;
        // If index is being set to a valid value, then show all recipe items for that index
        if (index >= 0)
        {
            _activeCraftingItems[_activeRecipeIndex].SetDisplayColor(RecipeSelectedColor);
            foreach(var item in Tabs[_activeTabIndex].Items[_activeCraftingItems[index].ArrayIndex].RecipeItems)
            {
                RecipeItemDisplay newDisplay = Instantiate(RecipeDisplayPrefab, CraftingPanel);
                newDisplay.SetText(
                    item.Item.name + ": " + item.Amount,
                    UIConsoleController.FindItemCount(item.Item.ID) >= item.Amount ?
                        EnoughItemsForRecipeTextColor :
                        NotEnoughItemsForRecipeTextColor
                    );
                newDisplay.SetImage(item.Item.Image);
                _activeRecipeItems.Add(newDisplay);
            }
        }

    }


    [System.Serializable]
    public struct CraftingTableTab
    {
        public string Name;
        public Image TabImage;
        public Button TabButton;
        public InventoryItem[] Items;
    }
}
