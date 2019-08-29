using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used with the <see cref="CraftingController"/> to display craftable items
/// </summary>
public class CraftingItemDisplay : MonoBehaviour
{
    public Button button;
    public Image DisplayImage;

    public Image BackgroundImage;


    [HideInInspector]
    public int ArrayIndex;

    public void SetImage(Sprite newImage)
    {
        DisplayImage.sprite = newImage;
    }

    public void SetDisplayColor(Color color)
    {
        BackgroundImage.color = color;
    }
}
