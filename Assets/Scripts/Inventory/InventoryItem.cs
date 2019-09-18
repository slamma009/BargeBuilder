using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for information about an Items Inventory aspects
/// </summary>
[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public int ID;
    public int StackSize;
    public EItemTag Tag;
    public int PlaceHeight;
    public Sprite Image;
    public GameObject Prefab;
    public bool IsPlacable;
    public GameObject[] SecondaryPrefabs;
    public RecipeItem[] RecipeItems;
    public int RecipeTimeInTicks = 25;
}

[System.Serializable]
public struct RecipeItem
{
    public InventoryItem Item;
    public int Amount;
}
public enum EItemTag { None, ConveyorBelt }
