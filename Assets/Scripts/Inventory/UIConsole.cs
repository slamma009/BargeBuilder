using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConsole : MonoBehaviour
{
    public InventoryCanvas HoverBargeCanvas;
    public InventoryCanvas OtherCanvas;
    public InventoryCanvas HotBar;

    InventoryCanvas SelectedCanvas;
    int SelectedIndex = -1;
    private Color OriginalSlotColor;

    public event Inventory.InventoryUpdatedAction InventoryUpdated;

    private void Start()
    {
        HoverBargeCanvas.SlotSelectedEvent = SlotChanged;
        OtherCanvas.SlotSelectedEvent = SlotChanged;
        HotBar.SlotSelectedEvent = SlotChanged;

        HoverBargeCanvas.AttachedInventory.InventoryUpdated += InventoryUpdatedEvent;
        HotBar.AttachedInventory.InventoryUpdated += InventoryUpdatedEvent;
        OtherCanvas.InventoryUpdated = InventoryUpdatedEvent;
    }
    public void SlotChanged(int index, InventoryCanvas canvas)
    {

        if (SelectedIndex < 0)
        {
            if (canvas.AttachedInventory.InventorySlots[index].Amount > 0)
            {
                SelectedCanvas = canvas;
                SelectedIndex = index;
                OriginalSlotColor = canvas.currentSlots[index].OutlineImage.color;
                canvas.currentSlots[index].OutlineImage.color = Color.white;
            }
        }
        else
        {
            if (index != SelectedIndex || SelectedCanvas != canvas)
            {
                int amount = SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Amount;
                InventoryItem item = SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Item;
                canvas.AttachedInventory.TryAddItemToItemCache(item);

                if (canvas.AttachedInventory.InventorySlots[index].Amount > 0
                    && canvas.AttachedInventory.InventorySlots[index].Item != null
                    && item.ID == canvas.AttachedInventory.InventorySlots[index].Item.ID &&
                    canvas.AttachedInventory.InventorySlots[index].Amount < item.StackSize)
                {
                    int amountToAdd = amount;
                    if (amountToAdd + canvas.AttachedInventory.InventorySlots[index].Amount > item.StackSize)
                        amountToAdd = item.StackSize - canvas.AttachedInventory.InventorySlots[index].Amount;

                    canvas.AttachedInventory.InventorySlots[index].Amount += amountToAdd;
                    SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Amount -= amountToAdd;
                    if (SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Amount == 0)
                        SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Item = null;
                }
                else
                {
                    SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Item = canvas.AttachedInventory.InventorySlots[index].Item;
                    SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Amount = canvas.AttachedInventory.InventorySlots[index].Amount;
                    canvas.AttachedInventory.InventorySlots[index].Item = item;
                    canvas.AttachedInventory.InventorySlots[index].Amount = amount;
                }


                //InventoryItem tempItem = canvas.AttachedInventory.InventorySlots[index].Item;
                //int tempAmount = canvas.AttachedInventory.InventorySlots[index].Amount;
            }
            SelectedCanvas.currentSlots[SelectedIndex].OutlineImage.color = OriginalSlotColor;
            SelectedIndex = -1;
        }
    }

    public void InventoryUpdatedEvent()
    {
        if (InventoryUpdated != null)
            InventoryUpdated();
    }

    /// <summary>
    /// Finds the amount of the given itemId in the inventories
    /// </summary>
    /// <param name="itemId">The itemId to look for</param>
    /// <param name="includeConnectedInventory">Flag for using connected inventory</param>
    /// <returns>The amount of the item available</returns>
    public int FindItemCount(int itemId, bool includeConnectedInventory = true)
    {
        return HoverBargeCanvas.AttachedInventory.GetCountById(itemId) +
            HotBar.AttachedInventory.GetCountById(itemId) +
            (includeConnectedInventory && OtherCanvas.AttachedInventory != null ? OtherCanvas.AttachedInventory.GetCountById(itemId) : 0);
    }

    /// <summary>
    /// Tries to remove the provided <see cref="InventoryItem"/> from the connected inventories
    /// </summary>
    /// <param name="itemId">The id of the <see cref="InventoryItem"/></param>
    /// <param name="amount">The amount to try and remove</param>
    /// <returns>The amount not removed from the inventories</returns>
    public int RemoveItemFromInventories(int itemId, int amount = 1)
    {
        amount = HoverBargeCanvas.AttachedInventory.RemoveById(itemId, amount);
        if (amount == 0)
            return 0;

        if (OtherCanvas.AttachedInventory != null)
        {
            amount = OtherCanvas.AttachedInventory.RemoveById(itemId, amount);
            if (amount == 0)
                return 0;
        }
        amount = HotBar.AttachedInventory.RemoveById(itemId, amount);

        return amount;
    }

    /// <summary>
    /// Attempts to add the provided <see cref="InventoryItem"/> to the inventories
    /// </summary>
    /// <param name="itemId">The <see cref="InventoryItem"/> id</param>
    /// <param name="amount">The amount to add</param>
    /// <returns>The amount not added</returns>
    public int AddItemToInventories(int itemId, int amount = 1)
    {
        amount = HoverBargeCanvas.AttachedInventory.Add(itemId, amount);
        if (amount == 0)
            return 0;

        amount = HotBar.AttachedInventory.Add(itemId, amount);
        if (amount == 0)
            return 0;

        if(OtherCanvas.AttachedInventory != null)
        {
            amount = OtherCanvas.AttachedInventory.Add(itemId, amount);
        }

        return amount;
    }
}
