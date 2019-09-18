using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueuedItemDisplay : MonoBehaviour
{
    public InventoryItem Item;
    public Button button;
    public Image DisplayImage;
    public Image ScreenImage;

    public Image BackgroundImage;

    private int _tickCount = 0;

    /// <summary>
    /// Adds 1 tick to the Tick Time
    /// </summary>
    /// <returns>True if Counted Ticks is greater then items required ticks</returns>
    public bool AddTick()
    {
        _tickCount += 1;
        float percentage = (float)_tickCount / (float)Item.RecipeTimeInTicks;
        if (percentage >= 1)
        {
            ScreenImage.fillAmount = 1;
            return true;
        }

        ScreenImage.fillAmount = percentage;
        return false;
    }

    [HideInInspector]
    public int ArrayIndex;

    public void SetItem(InventoryItem item)
    {
        Item = item;
        DisplayImage.sprite = item.Image;
    }

    public void SetDisplayColor(Color color)
    {
        BackgroundImage.color = color;
    }
}
