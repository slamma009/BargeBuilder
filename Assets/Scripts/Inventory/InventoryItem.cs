﻿using System.Collections;
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
    public Sprite Image;
    public GameObject Prefab;

}
