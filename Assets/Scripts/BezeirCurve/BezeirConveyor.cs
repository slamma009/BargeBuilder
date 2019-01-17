using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezeirConveyor : MonoBehaviour, IPushableObject {
    
    
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();
    
    public IPushableObject nextObject;

    private void OnTriggerEnter(Collider other)
    {
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
        for (var i = ActiveRigidBodies.Count - 1; i >= 0; --i)
        {
            if (ActiveRigidBodies[i] != null)
            {
                if (nextObject == null)
                    Debug.Log(transform.name + ": OH SHIT");
                Vector3 targetForce = nextObject == null || nextObject.ObjectIsFull() ?  Vector3.zero : transform.forward;
                ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce * 2, Time.deltaTime * 2);
            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }

    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return ActiveRigidBodies.Count >= 1 && (nextObject == null || nextObject.ObjectIsFull(CheckedObjects));
    }
}
