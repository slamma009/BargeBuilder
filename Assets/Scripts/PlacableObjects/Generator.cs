using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Generator : PlacableObject, IPushableObject
{
    public float BurnTime = 10;
    private float SavedTime; // Saved time for burning objects

    private Inventory ObjInventory;


    private void Start()
    {
        ObjInventory = GetComponent<Inventory>();
    }


    private void Update()
    {
        if (Time.timeSinceLevelLoad - SavedTime >= BurnTime)
        {
            BurnObject();
        }
    }

    private void BurnObject()
    {
        if(ObjInventory.Remove(0, 1) == 0)
        {
            SavedTime = Time.timeSinceLevelLoad;
        }
    }




    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return false;
    }


       
}
