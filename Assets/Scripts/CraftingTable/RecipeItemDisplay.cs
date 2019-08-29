using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used with the <see cref="CraftingController"/> to display the recipe items from a <see cref="CraftingItemDisplay"/>
/// </summary>
public class RecipeItemDisplay : CraftingItemDisplay
{
    public Text RecipeText;

    public void SetText(string text, Color? color = null) 
    {
        RecipeText.text = text;

        if (color.HasValue)
        {
            RecipeText.color = color.Value;
        }
    }
}
