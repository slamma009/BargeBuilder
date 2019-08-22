using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Splitter : PlacableObject//, IPushableObject
{

    public Transform[] SpawnPoints;
    private IPushableObject[] AttachedObjects;
    int LastOutput = 0;
    
    private Inventory SplitterInventory;

    private List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(transform.GetTopParent().name + " - " + other.name);
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null && !rigid.isKinematic)
        {
            ActiveRigidBodies.Add(rigid);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null && ActiveRigidBodies.Contains(rigid))
        {
            ActiveRigidBodies.Remove(rigid);
        }
    }

    private void FixedUpdate()
    {
        bool canPushObject = ObjectIsFull();
        for (var i = ActiveRigidBodies.Count - 1; i >= 0; --i)
        {
            if (ActiveRigidBodies[i] != null)
            {
                Vector3 targetForce = canPushObject ? Vector3.zero : transform.forward;
                ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce, Time.deltaTime);

            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }

    public override void ObjectPlaced()
    {
        SplitterInventory = GetComponent<Inventory>();
        AttachedObjects = new IPushableObject[3];
        // Set the anchor changed delegate to handle disabling walls
        for (int i = 0; i < Anchors.Length; ++i)
        {
            Anchors[i].AnchorChanged = AnchorChanged;
        }
        base.ObjectPlaced();
    }

    public void AnchorChanged(AnchorObject obj)
    {
        switch (obj.Anchor.name)
        {
            case "AnchorPoint L":
                SetAttachedObject(0, obj);
                break;
            case "AnchorPoint F":
                SetAttachedObject(1, obj);
                break;
            case "AnchorPoint R":
                SetAttachedObject(2, obj);
                break;
        }
    }

    private void SetAttachedObject(int index, AnchorObject obj)
    {
        if (obj.ConnectAnchor == null)
            AttachedObjects[index] = null;
        else
        {
            AttachedObjects[index] = obj.ConnectAnchor.transform.GetTopParent().GetComponent<IPushableObject>();
        }
    }

    public void Update()
    {
        if (SplitterInventory == null)
            return;
        for(var i=0; i< SplitterInventory.InventorySize; ++i)
        {
            int amount = SplitterInventory.InventorySlots[i].Amount;
            for(var j = 0; j<amount; ++j)
            {
                bool CanOutput = false;
                for(var k =0; k<3; ++k)
                {
                    if(AttachedObjects[LastOutput] != null && !AttachedObjects[LastOutput].CanTakeItem(null))
                    {
                        CanOutput = true;
                        break;
                    }
                    LastOutput++;
                    if (LastOutput > 2)
                        LastOutput = 0;
                }

                if (CanOutput)
                {
                    Instantiate(SplitterInventory.InventorySlots[i].Item.Prefab, SpawnPoints[LastOutput].position, Quaternion.identity);
                    if(SplitterInventory.RemoveByIndex(i) > 0)
                    {
                        throw new Exception("Iventory didn't remove item we placed.");
                    }

                    LastOutput++;
                    if (LastOutput > 2)
                        LastOutput = 0;
                } else
                {
                    break;
                }
            }

            if (SplitterInventory.InventorySlots[i].Amount > 0)
                break;
        }
    }

    
    private bool InventoryIsFull()
    {
        if (SplitterInventory == null)
            return true;
        for(var i=0; i<SplitterInventory.InventorySize; ++i)
        {
            if (SplitterInventory.InventorySlots[i].Amount == 0 || SplitterInventory.InventorySlots[i].Item == null)
                return false;
            else if (SplitterInventory.InventorySlots[i].Amount < SplitterInventory.InventorySlots[i].Item.StackSize)
                return false;
        }

        return true;
    }
    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        //if (Anchors.Where(x => x.ConnectAnchor != null).Count<AnchorObject>() > 1)
        //{
        //    if (CheckedObjects == null)
        //        CheckedObjects = new List<IPushableObject>();
        //    else if (CheckedObjects.Contains(this))
        //    {
        //        return ActiveRigidBodies.Count >= MaxItemsOnBelt;
        //    }
        //    CheckedObjects.Add(this);
        //}

        return InventoryIsFull() && ActiveRigidBodies.Count > 6;// && (AttachechedObject == null || AttachechedObject.ObjectIsFull(CheckedObjects));
    }

    public void PushItem(GameObject item)
    {
        throw new NotImplementedException();
    }
}
