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
    private void Start()
    {
        HoverBargeCanvas.SlotSelectedEvent = SlotChanged;
        OtherCanvas.SlotSelectedEvent = SlotChanged;
        HotBar.SlotSelectedEvent = SlotChanged;
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

                if(canvas.AttachedInventory.InventorySlots[index].Amount > 0 
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
}
