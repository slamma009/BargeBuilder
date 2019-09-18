using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Inventory : MonoBehaviour
{
    public string InventoryName = "Inventory";
    public int InventorySize = 8;
    private InventorySlot[] _InventorySlots { get; set; }

    public delegate void InventoryUpdatedAction();
    public event InventoryUpdatedAction InventoryUpdated;
    
    public InventorySlot[] InventorySlots
    {
        get
        {
            return _InventorySlots;
        }
    }

    private Dictionary<int, InventoryItem> ItemCache = new Dictionary<int, InventoryItem>();
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
    public int Add(ItemInstance item)
    {
        return Add(item.ID, item.Amount);
    }
    /// <summary>
    /// Attempt to add the given item to the Inventory. Returns the amount left after.
    /// </summary>
    /// <returns>Returns the amount of items not added.</returns>
    public int Add(int id, int amount = 1)
    {
        if (!TryAddItemToItemCache(id))
        {
            return amount;
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
                if (_InventorySlots[i].Item.ID == ItemCache[id].ID && _InventorySlots[i].Amount < ItemCache[id].StackSize)
                {
                    amount -= AddItemToSlot(amount, i);
                    if (amount == 0)
                    {
                        if (InventoryUpdated != null)
                            InventoryUpdated();
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
                _InventorySlots[i].Item = ItemCache[id];
                amount -= AddItemToSlot(amount, i);
                if (amount == 0)
                {
                    if (InventoryUpdated != null)
                        InventoryUpdated();
                    return 0;
                }
                else if (amount < 0)
                {
                    throw new Exception("Somehow added more items then we have");
                }
            }
        }

        // Unable to put all items in the inventory
        if (InventoryUpdated != null)
            InventoryUpdated();
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

        if (InventoryUpdated != null)
            InventoryUpdated();

        return amountToAdd;
    }

    /// <summary>
    /// Gets the amount of an item in the inventory by id
    /// </summary>
    /// <param name="itemId">The Id of the item</param>
    /// <returns>The amount of the item in the inventory</returns>
    public int GetCountById(int itemId)
    {
        // If the ID isn't even in the cache, then it can't be in the inventory
        if (!ItemCache.ContainsKey(itemId))
        {
            return 0;
        }

        var slotsWithId = _InventorySlots.Where(x => x.Item != null && x.Item.ID == itemId);

        // If no slots with the itemId are found, then return 0
        if (slotsWithId.Count() == 0)
            return 0;

        // Count the amount of items in each slot
        int amount = 0;
        foreach(var slot in slotsWithId)
        {
            amount += slot.Amount;
        }

        return amount;
    }

    /// <summary>
    /// Checks to see if inventory can add the provided <see cref="InventoryItem"/>
    /// </summary>
    /// <param name="itemId">The item to check</param>
    /// <param name="amount">The amount to check</param>
    /// <returns>True if inventory can add all the items provided</returns>
    public bool CanAddItem(int itemId, int amount = 1)
    {
        if (!TryAddItemToItemCache(itemId))
            return false;

        for (var i = 0; i < InventorySize; ++i)
        {
            if (_InventorySlots[i].Item == null && _InventorySlots[i].Amount == 0)
            {
                amount -= ItemCache[itemId].StackSize;
            }
            else if(_InventorySlots[i].Item.ID == itemId)
            {
                amount -= _InventorySlots[i].Item.StackSize - _InventorySlots[i].Amount;
            }

            if (amount <= 0)
                return true;
        }

        return false;
    }

    public int RemoveByIndex(int index, int amount = 1)
    {
        if (_InventorySlots[index].Amount <= 0 || _InventorySlots[index].Item == null)
            return amount;

        int amountToRemove = amount;
        if (_InventorySlots[index].Amount - amountToRemove < 0)
            amountToRemove = _InventorySlots[index].Amount;

        _InventorySlots[index].Amount -= amountToRemove;
        if(_InventorySlots[index].Amount == 0)
            _InventorySlots[index].Item = null;

        if (InventoryUpdated != null)
            InventoryUpdated();
        return amount - amountToRemove;
        
    }

    /// <summary>
    /// Try to remove the provided <see cref="InventoryItem"/> from the inventory
    /// </summary>
    /// <param name="itemId">The id of the <see cref="InventoryItem"/></param>
    /// <param name="amount">The amount to try and remove</param>
    /// <returns>The amount not removed from the inventory</returns>
    public int RemoveById(int itemId, int amount = 1)
    {
        if (!ItemCache.ContainsKey(itemId))
            return amount;
        
        for (var i = 0; i < InventorySize; ++i)
        {
            if (_InventorySlots[i].Item != null && 
                _InventorySlots[i].Item.ID == ItemCache[itemId].ID &&
                _InventorySlots[i].Amount >= 0)
            {
                amount = RemoveByIndex(i, amount);
                if (amount == 0)
                    return 0;
            }
        }

        return amount;



    }

    public bool TryAddItemToItemCache(int id)
    {
        if (!ItemCache.ContainsKey(id))
        {
            InventoryItem newItem = Controller.GetItem(id);
            if (newItem == null)
            {
                Debug.LogError("Unabled to find item with id " + id);
                return false;
            }

            return TryAddItemToItemCache(newItem);
        }

        return true;
    }

    public bool TryAddItemToItemCache(InventoryItem item)
    {
        if (!ItemCache.ContainsKey(item.ID))
        {
            ItemCache.Add(item.ID, item);
        }

        return true;
    }
}

[Serializable]
public struct InventorySlot
{
    public int Amount;
    public InventoryItem Item;
}