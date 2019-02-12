using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Generator : ElectricalPole, IPushableObject
{
    public float BurnTime = 50;
    public float Watts = 10;

    private float _TicksPassed;

    private float _PowerPerTick;

    private Inventory _ObjInventory;

    private bool _IsBurningObject;

    private void Start()
    {
        // Get our Objects Inventory controller, and calculate how much power to add per tick
        _ObjInventory = GetComponent<Inventory>();
        _PowerPerTick = Watts / BurnTime;
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
        // Increment our ticks, and check if we should burn a new object
        _TicksPassed++;
        if(_TicksPassed >= BurnTime)
        {
            if (BurnObject())
            {
                // If a new object is burnt, make sure we set _IsBurning and reset our _TicksPassed
                _IsBurningObject = true;
                _TicksPassed = 0;
            }
            else
            {
                // If no object could be burnt, make sure we set _IsBurning and decrement _TicksPassed so it trys again next tick
                _IsBurningObject = false;
                _TicksPassed -= 1;
            }
        }

        // If we are burning an object, add our power to the grid.
        if (_IsBurningObject)
        {
            if(!Power.NodeGroups[GroupId].AddPowerToGrid(_PowerPerTick))
            {
                _TicksPassed -= 1;
            }
        }
    }

    private bool BurnObject()
    {
        if(_ObjInventory.Remove(0, 1) == 0)
        {
            return true;
        }

        return false;
    }




    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return false;
    }

    // Remove TickUpdate from the event list if it has been set.
    private void OnDestroy()
    {
        if(Placed)
            TickController.TickEvent -= TickUpdate;
    }



}
