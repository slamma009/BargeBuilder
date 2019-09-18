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

    public Button CraftButton;

    /// <summary>The Controller for the Inventory Console</summary>
    public UIConsole UIConsoleController;

    /// <summary>Prefab for a <see cref="CraftingItemDisplay"/></summary>
    public CraftingItemDisplay ItemDisplayPrefab;

    /// <summary>Prefab for a <see cref="RecipeItemDisplay"/></summary>
    public RecipeItemDisplay RecipeDisplayPrefab;

    /// <summary>Prefab for a <see cref="QueuedItemDisplay"/></summary>
    public QueuedItemDisplay QueuedItemDisplayPrefab;

    /// <summary>The main panel to show <see cref="CraftingItemDisplay"/> on</summary>
    public Transform MainPanel;

    /// <summary>The main panel to show <see cref="RecipeItemDisplay"/> on</summary>
    public Transform CraftingPanel;
    
    /// <summary>The main panel to show <see cref="QueuedItemDisplay"/> on</summary>
    public Transform QueuedPanel;

    /// <summary>The currently active items being displayed</summary>
    private List<CraftingItemDisplay> _activeCraftingItems = new List<CraftingItemDisplay>();

    /// <summary>The currently active items being displayed</summary>
    private List<RecipeItemDisplay> _activeRecipeItems = new List<RecipeItemDisplay>();
    
    /// <summary>The current list of items being crafted</summary>
    private List<QueuedItemDisplay> _queuedItems = new List<QueuedItemDisplay>();

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
        UIConsoleController.InventoryUpdated += UpdateCraftingButton;
        TickController.TickEvent += TickUpdate;
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
                newDisplay.SetImage(item.Item.Image);
                _activeRecipeItems.Add(newDisplay);
            }
        }

        UpdateCraftingButton();

    }


    public void UpdateCraftingButton()
    {
        if (UpdateRecipeItemCounts())
            CraftButton.interactable = true;
        else
            CraftButton.interactable = false;
    }

    /// <summary>
    /// Checks inventories for Recipe Items for the current recipe
    /// Updates the color of the items based on that count
    /// </summary>
    public bool UpdateRecipeItemCounts()
    {
        if(_activeRecipeIndex < 0)
            return false;

        var i = 0;
        bool allItemsPresent = true;
        foreach (var item in Tabs[_activeTabIndex].Items[_activeCraftingItems[_activeRecipeIndex].ArrayIndex].RecipeItems)
        {
            bool hasEnough = UIConsoleController.FindItemCount(item.Item.ID) >= item.Amount;
            _activeRecipeItems[i].SetText(
                item.Item.name + ": " + item.Amount,
                hasEnough ?
                    EnoughItemsForRecipeTextColor :
                    NotEnoughItemsForRecipeTextColor
                );

            if (!hasEnough)
                allItemsPresent = false;

            i++;
        }

        return allItemsPresent;
    }

    /// <summary>
    /// Enqueues the selected item to the crafting list
    /// </summary>
    public void QueueCraftItem()
    {
        InventoryItem itemToCraft = Tabs[_activeTabIndex].Items[_activeCraftingItems[_activeRecipeIndex].ArrayIndex];

        // Remove all items needed to craft item
        foreach (var recipeItem in itemToCraft.RecipeItems)
        {
            if (UIConsoleController.RemoveItemFromInventories(recipeItem.Item.ID, recipeItem.Amount) > 0)
            {
                throw new System.Exception("Inventories did not have at least " + recipeItem.Amount + " items with the id of " + recipeItem.Item.ID);
            }
        }

        // Create new display for the crafting item
        QueuedItemDisplay newItem = Instantiate(QueuedItemDisplayPrefab, QueuedPanel);
        newItem.SetItem(itemToCraft);
        _queuedItems.Add(newItem);
    }

    public void TickUpdate(object sender, TickArgs arg)
    {
        if(_queuedItems.Count > 0)
        {
            if (_queuedItems[0].AddTick())
            {
                // Item has finished crafting

                if (UIConsoleController.AddItemToInventories(_queuedItems[0].Item.ID) > 0)
                {
                    throw new System.Exception("Unable to add crafted item with id " + _queuedItems[0].Item.ID + " to the inventory");
                }

                // Remove the item
                Destroy(_queuedItems[0].gameObject);
                _queuedItems.RemoveAt(0);
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
