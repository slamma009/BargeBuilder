using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConsole : MonoBehaviour
{
    public InventoryCanvas HoverBargeCanvas;
    public InventoryCanvas OtherCanvas;

    InventoryCanvas SelectedCanvas;
    int SelectedIndex = -1;
    private Color OriginalSlotColor;
    private void Start()
    {
        HoverBargeCanvas.SlotSelectedEvent = SlotChanged;
        OtherCanvas.SlotSelectedEvent = SlotChanged;
    }
    public void SlotChanged(int index, InventoryCanvas canvas)
    {
        string extra = "";
        if (SelectedIndex > -1)
            extra = "\n" + SelectedCanvas.name + " - " + SelectedIndex;
        Debug.Log(canvas.name + " - " + index + extra);

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
                InventoryItem tempItem = canvas.AttachedInventory.InventorySlots[index].Item;
                int tempAmount = canvas.AttachedInventory.InventorySlots[index].Amount;
                canvas.AttachedInventory.InventorySlots[index].Item = SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Item;
                canvas.AttachedInventory.InventorySlots[index].Amount = SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Amount;
                SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Item = tempItem;
                SelectedCanvas.AttachedInventory.InventorySlots[SelectedIndex].Amount = tempAmount;
            }
            SelectedCanvas.currentSlots[SelectedIndex].OutlineImage.color = OriginalSlotColor;
            SelectedIndex = -1;
        }
    }
}
