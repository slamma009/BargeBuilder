using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : PlacableObject
{
    public int MaxItemsOnBelt = 3;
    public GameObject[] Walls;

    [HideInInspector]
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();


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
                Vector3 targetForce = transform.forward;
                ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce, Time.deltaTime);
            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }
}

