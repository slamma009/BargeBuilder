using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : PlacableObject
{
    public int MaxItemsOnBelt = 3;
    public Transform[] Anchors;

    [HideInInspector]
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();

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

