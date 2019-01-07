using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : PlacableObject
{
    public GameObject[] Walls;
    [HideInInspector]
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();
    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid != null)
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
        foreach (Rigidbody rigid in ActiveRigidBodies) {
            rigid.AddForce(transform.forward * 10 * Time.deltaTime);
        }
    }

    public override void UpdateBlock()
    {

        base.UpdateBlock();
    }
}

