using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Generator : ElectricalPole, IPushableObject
{
    public float BurnTime = 10;
    private float SavedTime; // Saved time for burning objects

    private Inventory ObjInventory;


    private void Start()
    {
        ObjInventory = GetComponent<Inventory>();
    }

    protected override void GroupChanged()
    {
        Power.NodeGroups[GroupId].MaxPowerLevels += 10;
    }
    private void Update()
    {
        if (!Placed)
            return;
        if (Time.timeSinceLevelLoad - SavedTime >= BurnTime)
        {
            BurnObject();
        }
    }

    private void BurnObject()
    {
        if(ObjInventory.Remove(0, 1) == 0)
        {
            Power.NodeGroups[GroupId].AddPowerToGrid(10);
            SavedTime = Time.timeSinceLevelLoad;
        } 
    }




    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return false;
    }


       
}
