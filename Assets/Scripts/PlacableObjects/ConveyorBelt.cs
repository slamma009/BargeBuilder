using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : PlacableObject, IPushableObject
{
    public int MaxItemsOnBelt = 6;
    public GameObject[] Walls;

    [HideInInspector]
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();

    public IPushableObject AttachechedObject;


    public override void ObjectPlaced()
    {
        // Set the anchor changed delegate to handle disabling walls
        for (int i = 0; i < Anchors.Length; ++i)
        {
            Anchors[i].AnchorChanged = AnchorChanged;
        }
        base.ObjectPlaced();
    }

    // Disables walls if anchored to an object.
    public void AnchorChanged(AnchorObject obj)
    {
        switch (obj.Anchor.name)
        {
            case "AnchorPoint F":
                Walls[0].SetActive(obj.ConnectAnchor == null);

                if(obj.ConnectAnchor == null)
                    AttachechedObject = null;
                else
                {
                    PlaneBezeir bezier = obj.ConnectAnchor.GetComponent<PlaneBezeir>();
                    if(bezier != null)
                    {
                        AttachechedObject = bezier.FirstTriggerBox;
                    }
                    else
                    {
                        AttachechedObject = obj.ConnectAnchor.transform.GetTopParent().GetComponent<IPushableObject>();
                    }
                }
                break;
            case "AnchorPoint R":
                Walls[1].SetActive(obj.ConnectAnchor == null);
                break;
            case "AnchorPoint B":
                Walls[2].SetActive(obj.ConnectAnchor == null);
                break;
            case "AnchorPoint L":
                Walls[3].SetActive(obj.ConnectAnchor == null);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid != null && !rigid.isKinematic)
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
        for(var i=ActiveRigidBodies.Count - 1; i >= 0; --i)
        {
            if (ActiveRigidBodies[i] != null)
            {
                    Vector3 targetForce = AttachechedObject == null || (AttachechedObject.ObjectIsFull()) ? Vector3.zero : transform.forward;
                    ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce, Time.deltaTime);
                
            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }

    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        if (Anchors.Where(x => x.ConnectAnchor != null).Count<AnchorObject>() > 1)
        {
            if (CheckedObjects == null)
                CheckedObjects = new List<IPushableObject>();
            else if (CheckedObjects.Contains(this))
            {
                return ActiveRigidBodies.Count >= MaxItemsOnBelt;
            }
            CheckedObjects.Add(this);
        }

        return ActiveRigidBodies.Count >= MaxItemsOnBelt && (AttachechedObject == null || AttachechedObject.ObjectIsFull(CheckedObjects));
    }
}

