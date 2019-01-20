﻿using System;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    public int InventorySize = 8;
    public InventorySlot[] _InventorySlots;
    
    public InventorySlot[] InventorySlots
    {
        get
        {
            return _InventorySlots;
        }
    }

    private Dictionary<int, InventoryItem> Items = new Dictionary<int, InventoryItem>();
    private InventoryController Controller;

    private void Start()
    {
        _InventorySlots = new InventorySlot[InventorySize];
        GameObject[] inventoryController = GameObject.FindGameObjectsWithTag("InventoryController");

        if (inventoryController.Length == 0)
            Debug.LogError("Unabled to find Inventory Controller");
        else
        {
            if (inventoryController.Length > 1)
                Debug.LogError("Found more then 1 Inventory Controller, defaulting to 0");

            Controller = inventoryController[0].GetComponent<InventoryController>();
            if(Controller == null)
                Debug.LogError("The Inventory Controller object doesn't have the inventory controller class");
            
        }
    }

    /// <summary>
    /// Attempt to add the given item to the Inventory. Returns the amount left after.
    /// </summary>
    /// <returns>Returns the amount of items not added.</returns>
    public int Add(int id, int amount = 1)
    {
        if (!Items.ContainsKey(id))
        {
            InventoryItem newItem = Controller.GetItem(id);
            if(newItem == null)
            {
                Debug.LogError("Unabled to find item with id " + id);
                return amount;
            }
            Items.Add(id, newItem);
        }


        List<int> emptySlots = new List<int>();
        for (var i = 0; i < InventorySize; ++i)
        {
            if (_InventorySlots[i].Amount == 0 || _InventorySlots[i].Item == null)
            {
                // Keep track of our empty slots for later if needed
                emptySlots.Add(i);
            }
            else
            {
                // Try to place item in existing slots first
                if (_InventorySlots[i].Item.ID == Items[id].ID && _InventorySlots[i].Amount < Items[id].StackSize)
                {
                    amount -= AddItemToSlot(amount, i);
                    if (amount == 0)
                    {
                        return 0;
                    }
                    else if (amount < 0)
                    {
                        throw new Exception("Somehow added more items then we have");
                    }
                }
            }
        }

        // Amount should have been returned already if it was 0. Amount should never be negative.
        if (amount <= 0)
        {
            throw new Exception("Somehow escaped the add method with " + amount + " items");
        }

        // If we have any empty slots, try to add the items to those
        if (emptySlots.Count > 0)
        {
            foreach (int i in emptySlots)
            {
                _InventorySlots[i].Item = Items[id];
                amount -= AddItemToSlot(amount, i);
                if (amount == 0)
                {
                    return 0;
                }
                else if (amount < 0)
                {
                    throw new Exception("Somehow added more items then we have");
                }
            }
        }

        // Unable to put all items in the inventory
        return amount;
    }

    private int AddItemToSlot(int amount, int index)
    {
        int amountToAdd = _InventorySlots[index].Item.StackSize - _InventorySlots[index].Amount;
        if (amountToAdd > amount)
        {
            amountToAdd = amount;
        }
        _InventorySlots[index].Amount += amountToAdd;

        return amountToAdd;
    }

}

[Serializable]
public struct InventorySlot
{
    public int Amount;
    public InventoryItem Item;
}