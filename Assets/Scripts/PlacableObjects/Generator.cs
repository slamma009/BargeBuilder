using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Generator : ElectricalPole, IPushableObject
{
    /// <summary>
    /// Amount of ticks the generator burns for
    /// </summary>
    public float BurnTime = 50;
    /// <summary>
    /// The amount of power generated over the burn time
    /// </summary>
    public float Watts = 10;
    /// <summary>
    /// The delay in ticks to wait until putting an item in the inventory
    /// </summary>
    public int ItemHoldLength = 5;
    private float _currentItemHeldLength = 0;

    private float _ticksPassed;

    private float _powerPerTick;

    private Inventory _objInventory;

    private bool _isBurningObject;

    private Item _currentItem;
    private void Start()
    {
        // Get our Objects Inventory controller, and calculate how much power to add per tick
        _objInventory = GetComponent<Inventory>();
        _powerPerTick = Watts / BurnTime;
    }

    public override void ObjectPlaced()
    {
        // Add the TickUpdate to the Tick Event
        TickController.TickEvent += TickUpdate;
        base.ObjectPlaced();
    }
    
    protected override void GroupChanged()
    {
        // When connected to a new group, add our watts to it's max power potential
        Power.NodeGroups[GroupId].MaxPowerLevels += Watts;
    }

    public void TickUpdate(object sender, TickArgs arg)
    {
        // See if any item we have inside the generator can be put in the inventory
        if (_currentItem != null)
        {
            if (_currentItemHeldLength <= 0)
            {
                if (_objInventory.Add(_currentItem.ID) == 0)
                {
                    Destroy(_currentItem.gameObject);
                    _currentItem = null;
                }
            }
            else
            {
                _currentItemHeldLength--;
            }
        }
    
        // Increment our ticks, and check if we should burn a new object
        _ticksPassed++;
        if(_ticksPassed >= BurnTime)
        {
            if (BurnObject())
            {
                // If a new object is burnt, make sure we set _IsBurning and reset our _TicksPassed
                _isBurningObject = true;
                _ticksPassed = 0;
            }
            else
            {
                // If no object could be burnt, make sure we set _IsBurning and decrement _TicksPassed so it trys again next tick
                _isBurningObject = false;
                _ticksPassed -= 1;
            }
        }

        // If we are burning an object, add our power to the grid.
        if (_isBurningObject)
        {
            lock (Power)
            {
                if (!Power.NodeGroups[GroupId].AddPowerToGrid(_powerPerTick))
                {
                    _ticksPassed -= 1;
                }
            }
        }


    }

    public void FixedUpdate()
    {
        // Move item towards center of object
        if (_currentItem != null)
        {
            _currentItem.gameObject.transform.position = Vector3.MoveTowards(
                _currentItem.gameObject.transform.position,
                transform.position, Time.deltaTime);
        }
    }

    private bool BurnObject()
    {
        if(_objInventory.Remove(0, 1) == 0)
        {
            return true;
        }

        return false;
    }




    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return _currentItem != null;
    }

    // Remove TickUpdate from the event list if it has been set.
    private void OnDestroy()
    {
        DestroyElectricalPole();
        if (Placed)
            TickController.TickEvent -= TickUpdate;
    }

    public void PushObject(GameObject item)
    {
        if (_currentItem != null)
            throw new InvalidOperationException("The Current Item in the Generator has not been removed, but another is attempting to be added");

        _currentItem = item.GetComponent<Item>();
        _currentItemHeldLength = ItemHoldLength;

        if (_currentItem == null)
            throw new MissingComponentException("The item " + item.name + " was expected to have the component Item but it was not found.");
    }
}
