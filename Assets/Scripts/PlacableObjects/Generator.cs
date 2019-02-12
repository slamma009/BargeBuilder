using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Generator : ElectricalPole, IPushableObject
{
    public float BurnTime = 50;

    private float _TicksPassed;

    private Inventory _ObjInventory;

    private void Start()
    {
        _ObjInventory = GetComponent<Inventory>();
    }

    public override void ObjectPlaced()
    {
        TickController.TickEvent += TickUpdate;
        base.ObjectPlaced();
    }
    
    protected override void GroupChanged()
    {
        Power.NodeGroups[GroupId].MaxPowerLevels += 10;
    }

    public void TickUpdate(object sender, TickArgs arg)
    {
        _TicksPassed++;
        if(_TicksPassed >= BurnTime)
        {
            _TicksPassed = 0;
            BurnObject();
        }
    }

    private void BurnObject()
    {
        if(_ObjInventory.Remove(0, 1) == 0)
        {
            Power.NodeGroups[GroupId].AddPowerToGrid(10);
        } 
    }




    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return false;
    }

    private void OnDestroy()
    {
        if(Placed)
            TickController.TickEvent -= TickUpdate;
    }



}
