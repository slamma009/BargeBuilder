using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    /// <summary>
    /// Unique Identifier for an item
    /// </summary>
    public int ID;

    /// <summary>
    /// The Amount of the item in this stack
    /// </summary>
    public int Amount = 1;

    /// <summary>
    /// The modifier for how fast this item burns. The higher the longer. 
    /// </summary>
    public float BurnModifier = 0;

    /// <summary>
    /// Determines wether the item can be used as a fuel source
    /// </summary>
    public bool IsFuel = false;

    /// <summary>
    /// The amount of ticks it takes to extract the item.
    /// </summary>
    public float ExtractTime = 15;
}
